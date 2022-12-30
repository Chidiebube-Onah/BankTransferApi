using AutoMapper;
using BankTransfer.Models.Dtos.Requests;
using BankTransfer.Models.Dtos.Responses;

namespace BankTransfer.BLL.MappingConfigurations
{
    public class FlutterWaveProviderProfile : Profile
    {
        public FlutterWaveProviderProfile()
        {
            CreateMap<FlutterWaveBankList, BankResponse>();

            CreateMap<FlutterWaveBankTransferRecipient, ValidateAccountResponse>()
                .ForMember(dest => dest.AccountNumber, opt => opt.MapFrom(src => src.account_number))
                .ForMember(dest => dest.AccountName, opt => opt.MapFrom(src => src.account_name));

            CreateMap<FlutterWaveBankTransferResponse, BankTransferResponse>()
                .ForMember(dest => dest.ResponseMessage, opt => opt.MapFrom(src => src.message))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.data.amount))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.data.status))
                .ForMember(dest => dest.TransactionReference, opt => opt.MapFrom(src => src.data.id))
                .ForMember(dest => dest.BeneficiaryAccountName, opt => opt.MapFrom(src => src.data.full_name))
                .ForMember(dest => dest.TransactionDateTime, opt => opt.MapFrom(src => src.data.created_at))
                .ForMember(dest => dest.CurrencyCode, opt => opt.MapFrom(src => src.data.currency))
                .ForMember(dest => dest.BeneficiaryBankCode, opt => opt.MapFrom(src => src.data.bank_code))
                .ForMember(dest => dest.BeneficiaryAccountNumber, opt => opt.MapFrom(src => src.data.account_number));
        }
    }
}