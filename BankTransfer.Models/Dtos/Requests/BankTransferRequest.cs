using System.ComponentModel.DataAnnotations;

namespace BankTransfer.Models.Dtos.Requests
{
    public class BankTransferRequest
    {
        [Required]
        public string Amount { get; set; }

        [Required]
        public string CurrencyCode { get; set; }

        [Required]
        public string Narration { get; set; }

        [Required]
        public string BeneficiaryAccountName { get; set; }

        [Required]
        public string BeneficiaryAccountNumber { get; set; }

        [Required]
        public string BeneficiaryBankCode { get; set; }

        [Required]
        public string TransactionReference { get; set; }
        public int MaxRetryAttempt { get; set; }
        public string? CallBackUrl { get; set; }
        public string? Provider { get; set; }
    }
}