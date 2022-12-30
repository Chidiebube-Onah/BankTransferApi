using System.ComponentModel.DataAnnotations;

namespace BankTransfer.Models.Dtos.Requests
{
    public class ValidateAccountRequest
    {
        [Required]
        public string AccountNumber { get; set; }

        [Required]
        public string BankCode { get; set; }
        public string? Provider { get; set; }
    }
}