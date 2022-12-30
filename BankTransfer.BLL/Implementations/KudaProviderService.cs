using AutoMapper;
using BankTransfer.BLL.Extensions;
using BankTransfer.BLL.Interfaces;
using BankTransfer.Models.Dtos.Requests;
using BankTransfer.Models.Dtos.Responses;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using BankTransfer.Models.Configs;

namespace BankTransfer.BLL.Implementations
{
    public class KudaProviderService : IBankTransferService
    {
        private readonly RestClientHandler _restClientHandler;
        private readonly IMapper _mapper;
        private readonly KudaBankConfig _config;
       
        private readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };



        public KudaProviderService(RestClientHandler restClientHandler, IMapper mapper, KudaBankConfig config)
        {
            _restClientHandler = restClientHandler;
            _mapper = mapper;
            _config = config;
        }
        public async Task<IEnumerable<BankResponse>> GetBanks()
        {

            await AuthenticateConnectionAsync();

            var request = new
            {
                ServiceType = "BANK_LIST"
            };

            string body = JsonConvert.SerializeObject(request, _jsonSerializerSettings);

            IRestResponse providerResponse = await _restClientHandler.PostAsync("v2.1", body);

            providerResponse.HandleAnyErrors();

            KudaBankListResponse? bankListRes = JsonConvert.DeserializeObject<KudaBankListResponse>(providerResponse.Content);

            return _mapper.Map<IEnumerable<BankResponse>>(bankListRes?.Data.Banks);

        }

        public async Task<ValidateAccountResponse> ValidateAccount(ValidateAccountRequest request)
        {
            await AuthenticateConnectionAsync();

            var providerRequest = new
            {
                ServiceType = "NAME_ENQUIRY",
                Data = new
                {
                    BeneficiaryAccountNumber = request.AccountNumber,
                    BeneficiaryBankCode = request.BankCode,
                }
            };

            string body = JsonConvert.SerializeObject(providerRequest, _jsonSerializerSettings);

            IRestResponse providerResponse = await _restClientHandler.PostAsync("v2.1", body);

            providerResponse.HandleAnyErrors();

            KudaBankAccountVerificationResponse? accountVerificationResponse =
                JsonConvert.DeserializeObject<KudaBankAccountVerificationResponse>(providerResponse.Content);

            string? bankName =
                (await GetBanks()).SingleOrDefault(b =>
                    b.Code == accountVerificationResponse?.Data.BeneficiaryBankCode)?.Name;

            return _mapper.Map<ValidateAccountResponse>(accountVerificationResponse?.Data, opt =>
                opt.AfterMap((src, dest) =>
                {
                    dest.BankName = bankName;
                }));

        }

        public async Task<BankTransferResponse> Transfer(BankTransferRequest request)
        {

            await AuthenticateConnectionAsync();

            decimal amountToKobo;

            if (!decimal.TryParse(request.Amount, out amountToKobo))
            {
                throw new InvalidOperationException("amount is Invalid!");
            }

            amountToKobo *= 100;

            var providerRequest = new
            {
                ServiceType = "SINGLE_FUND_TRANSFER",
                RequestRef = request.TransactionReference,
                Data = new
                {
                    ClientAccountNumber = _config.ClientAccountNumber,
                    BeneficiarybankCode = request.BeneficiaryBankCode,
                    BeneficiaryAccount = request.BeneficiaryAccountNumber,
                    BeneficiaryName = request.BeneficiaryAccountName,
                    Amount = amountToKobo.ToString(CultureInfo.InvariantCulture),
                    Narration = request.Narration,
                    SenderName =_config.SenderName
                }
            };

            string body = JsonConvert.SerializeObject(providerRequest, _jsonSerializerSettings);

            IRestResponse providerResponse =
                await _restClientHandler.PostAsync("v2.1", body)
                    .RetryAsync(request.MaxRetryAttempt);

            providerResponse.HandleAnyErrors();

            KudaProviderBankTransferResponse? bankTransferResponse = JsonConvert.DeserializeObject<KudaProviderBankTransferResponse>(providerResponse.Content);

            return _mapper.Map<BankTransferResponse>(bankTransferResponse, opt => opt.AfterMap((src, dest) =>
            {
                dest.BeneficiaryAccountNumber = request.BeneficiaryAccountNumber;
                dest.BeneficiaryAccountName = request.BeneficiaryAccountName;
                dest.BeneficiaryBankCode = request.BeneficiaryBankCode;
                dest.Amount = request.Amount;
                dest.TransactionDateTime = DateTime.Now.ToString(CultureInfo.InvariantCulture);
                dest.CurrencyCode = request.CurrencyCode;
            }));

        }

        public async Task<BankTransferResponse> GetTransactionStatus(string transactionReference)
        {

            await AuthenticateConnectionAsync();

            var providerRequest = new
            {
                ServiceType = "TRANSACTION_STATUS_QUERY",
                Data = new
                {
                    IsThirdPartyBankTransfer = false,
                    transactionRequestReference = transactionReference,
                }
            };

            string body = JsonConvert.SerializeObject(providerRequest, _jsonSerializerSettings);

            IRestResponse providerResponse = await _restClientHandler.PostAsync("v2.1", body);

            providerResponse.HandleAnyErrors();

            KudaProviderBankTransferResponse? bankTransferResponse = JsonConvert.DeserializeObject<KudaProviderBankTransferResponse>(providerResponse.Content);

            return _mapper.Map<BankTransferResponse>(bankTransferResponse);
        }

        private async Task AuthenticateConnectionAsync()
        {
            _restClientHandler.InstantiateClient(baseUri: _config.BaseUri);

            var tokenRequest = new
            {
                Email = _config.Email,
                ApiKey = _config.ApiKey
            };

            string serializedTokenRequest = JsonConvert.SerializeObject(tokenRequest, _jsonSerializerSettings);

            IRestResponse providerResponse = await _restClientHandler
                .PostAsync("v2/Account/GetToken", serializedTokenRequest)
                .RetryAsync(3);

            providerResponse.HandleAnyErrors(validateJsonFormat:false);

            string? token = JsonConvert.DeserializeObject<string>(providerResponse.Content);

            _restClientHandler.InstantiateClient(baseUri: _config.BaseUri,
                authenticator: new JwtAuthenticator(token ?? string.Empty));
        }
    }

}
