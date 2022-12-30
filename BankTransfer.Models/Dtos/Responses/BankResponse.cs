using System;
using System.Collections.Generic;
using System.Text;

namespace BankTransfer.Models.Dtos.Responses
{
    public class BankResponse
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string LongCode { get; set; }
    }
}
