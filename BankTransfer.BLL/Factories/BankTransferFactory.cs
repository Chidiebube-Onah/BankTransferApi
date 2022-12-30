using BankTransfer.BLL.Implementations;
using BankTransfer.BLL.Interfaces;
using System;

namespace BankTransfer.BLL.Factories
{
    public class BankTransferFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public BankTransferFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public IBankTransferService GetBankTransferService(string? provider)
        {
            return provider?.ToLower().Trim() switch
            {
                null => (IBankTransferService) _serviceProvider.GetService(typeof(KudaProviderService)),
                "kuda" => (IBankTransferService) _serviceProvider.GetService(typeof(KudaProviderService)),
                "flutterwave" => (IBankTransferService) _serviceProvider.GetService(typeof(FlutterWaveProviderService)),
                _ => throw new InvalidOperationException("Invalid Provider !")
            };
        }
    }
}
