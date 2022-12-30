using System.Collections.Generic;
using System.Threading.Tasks;
using BankTransfer.Models.Dtos.Requests;
using BankTransfer.Models.Dtos.Responses;

namespace BankTransfer.BLL.Interfaces
{
    public interface IBankTransferService
    {
        Task<IEnumerable<BankResponse>> GetBanks();
        Task<ValidateAccountResponse> ValidateAccount(ValidateAccountRequest request);
        Task<BankTransferResponse> Transfer(BankTransferRequest request);
        Task<BankTransferResponse> GetTransactionStatus(string transactionReference);

    }
}