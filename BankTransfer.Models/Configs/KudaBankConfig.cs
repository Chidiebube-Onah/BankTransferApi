using System;
using System.Collections.Generic;
using System.Text;

namespace BankTransfer.Models.Configs
{
    public class KudaBankConfig : Config
    {
        public string Email { get; set; }
        public string ApiKey { get; set; }
        public string SenderName { get; set; }
        public string ClientAccountNumber { get; set; }
        
    }
}