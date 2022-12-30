using System.Collections.Generic;

namespace BankTransfer.Models.Dtos.Responses
{
    public class FlutterWaveBankListResponse
    {
        public IEnumerable<FlutterWaveBankList> Data { get; set; }
    }


    public class FlutterWaveBankList
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }


    public class FlutterWaveAccountVerificationResponse
    {
        public FlutterWaveBankTransferRecipient Data { get; set; }
    }
    public class FlutterWaveBankTransferRecipient
    {
        public string account_number { get; set; }
        public string account_name { get; set; }
    }

    public class FlutterWaveBankTransferDataResponse
    {
        public string id { get; set; }
        public string account_number { get; set; }
        public string bank_code { get; set; }
        public string full_name { get; set; }
        public string created_at { get; set; }
        public string currency { get; set; }
        public string debit_currency { get; set; }
        public string amount { get; set; }
        public string fee { get; set; }
        public string status { get; set; }
        public string reference { get; set; }
        public string complete_message { get; set; }
        public string bank_name { get; set; }
    }
    public class FlutterWaveBankTransferResponse
    {

        public string success { get; set; }
        public string message { get; set; }
        public FlutterWaveBankTransferDataResponse data { get; set; }
}
}