using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using BankTransfer.Models.Dtos.Responses;

namespace BankTransfer.BLL.MappingConfigurations
{
    public class KudaProviderProfile : Profile
    {
        public KudaProviderProfile()
        {
            CreateMap<KudaBankList, BankResponse>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.BankName))
                .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.BankCode));

            CreateMap<KudaBankTransferRecipient, ValidateAccountResponse>()
                .ForMember(dest => dest.AccountNumber, opt => opt.MapFrom(src => src.BeneficiaryAccountNumber))
                .ForMember(dest => dest.AccountName, opt => opt.MapFrom(src => src.BeneficiaryName))
                .ForMember(dest => dest.BankCode, opt => opt.MapFrom(src => src.BeneficiaryBankCode));

            CreateMap<KudaProviderBankTransferResponse, BankTransferResponse>()
                .ForMember(dest => dest.TransactionReference, opt => opt.MapFrom(src => src.RequestReference))
                .ForMember(dest => dest.ResponseMessage, opt => opt.MapFrom(src => src.Message));


        }
    }
}
