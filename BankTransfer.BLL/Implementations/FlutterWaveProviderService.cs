using AutoMapper;
using BankTransfer.BLL.Extensions;
using BankTransfer.BLL.Interfaces;
using BankTransfer.Models.Dtos.Requests;
using BankTransfer.Models.Dtos.Responses;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RestSharp;
using RestSharp.Authenticators;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BankTransfer.Models.Configs;

namespace BankTransfer.BLL.Implementations
{
    public class FlutterWaveProviderService : IBankTransferService
    {
        private readonly RestClientHandler _restClientHandler;
        private readonly IMapper _mapper;
        private readonly FlutterWaveConfig _config;

        private readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        public FlutterWaveProviderService(RestClientHandler restClientHandler, IMapper mapper, FlutterWaveConfig config)
        {
            _restClientHandler = restClientHandler;
            _mapper = mapper;
            _config = config;
        }
        public async Task<IEnumerable<BankResponse>> GetBanks()
        {
            _restClientHandler.InstantiateClient(baseUri: _config.BaseUri, new JwtAuthenticator(_config.Token));

            List<Parameter> parameters = new List<Parameter>()
            {
                new Parameter("country", "NG", ParameterType.UrlSegment, encode:false)
            };

            IRestResponse providerResponse =
                await _restClientHandler.GetAsync(resource: "v3/banks/{country}", parameters: parameters);

            providerResponse.HandleAnyErrors();

            FlutterWaveBankListResponse? response =
                JsonConvert.DeserializeObject<FlutterWaveBankListResponse>(providerResponse.Content);

            return _mapper.Map<IEnumerable<BankResponse>>(response?.Data);

        }

        public async Task<ValidateAccountResponse> ValidateAccount(ValidateAccountRequest request)
        {
            _restClientHandler.InstantiateClient(baseUri: _config.BaseUri, new JwtAuthenticator(_config.Token));

            var providerRequest = new
            {
                account_number = request.AccountNumber,
                account_bank = request.BankCode
            };

            string body = JsonConvert.SerializeObject(providerRequest);

            IRestResponse providerResponse = await _restClientHandler.PostAsync("v3/accounts/resolve", body);

            providerResponse.HandleAnyErrors();

            FlutterWaveAccountVerificationResponse? accountVerificationResponse =
                JsonConvert.DeserializeObject<FlutterWaveAccountVerificationResponse>(providerResponse.Content);

            BankResponse? bank = (await GetBanks()).SingleOrDefault(b => b.Code == request.BankCode);


            return _mapper.Map<ValidateAccountResponse>(accountVerificationResponse?.Data, opt =>
                opt.AfterMap((src, dest) =>
                {
                    dest.BankCode = bank?.Code;
                    dest.BankName = bank?.Name;
                }));

        }

        public async Task<BankTransferResponse> Transfer(BankTransferRequest request)
        {
            _restClientHandler.InstantiateClient(baseUri: _config.BaseUri, new JwtAuthenticator(_config.Token));

            var providerRequest = new
            {
                account_bank = request.BeneficiaryBankCode,
                account_number = request.BeneficiaryAccountNumber,
                amount = request.Amount,
                narration = request.Narration,
                currency = request.CurrencyCode,
                reference = request.TransactionReference,

            };

            string body = JsonConvert.SerializeObject(providerRequest);

            IRestResponse providerResponse =
                await _restClientHandler.PostAsync("v3/transfers", body)
                    .RetryAsync(request.MaxRetryAttempt);

            providerResponse.HandleAnyErrors();

            FlutterWaveBankTransferResponse? bankTransferResponse = JsonConvert.DeserializeObject<FlutterWaveBankTransferResponse>(providerResponse.Content);

            return _mapper.Map<BankTransferResponse>(bankTransferResponse);

        }

        public async Task<BankTransferResponse> GetTransactionStatus(string transactionReference)
        {
            _restClientHandler.InstantiateClient(baseUri: _config.BaseUri, new JwtAuthenticator(_config.Token));

            List<Parameter> parameters = new List<Parameter>()
            {
                new Parameter("id", transactionReference, ParameterType.UrlSegment, encode:false)
            };

            IRestResponse providerResponse = await _restClientHandler.GetAsync(resource: "v3/transfers/{id}", parameters: parameters);

            providerResponse.HandleAnyErrors();


            FlutterWaveBankTransferResponse? bankTransferResponse = JsonConvert.DeserializeObject<FlutterWaveBankTransferResponse>(providerResponse.Content);

            return _mapper.Map<BankTransferResponse>(bankTransferResponse);
        }
    }
}