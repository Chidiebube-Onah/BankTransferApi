using System.Collections.Generic;

namespace BankTransfer.Models.Dtos.Responses
{
    public class KudaBankListResponse
    {
        public KudaBankListData Data { get; set; }
    }

    public class KudaBankListData
    {
        public IEnumerable<KudaBankList> Banks { get; set; }
    }

    public class KudaBankList
    {
        public string BankCode { get; set; }
        public string BankName { get; set; }
    }


    public class KudaBankAccountVerificationResponse
    {
        public KudaBankTransferRecipient Data { get; set; }
    }

    public class KudaBankTransferRecipient
    {
        public string BeneficiaryAccountNumber { get; set; }
        public string BeneficiaryName { get; set; }
        public string SenderAccountNumber { get; set; }
        public string SenderName { get; set; }
        public string BeneficiaryCustomerID { get; set; }
        public string BeneficiaryBankCode { get; set; }
        public string NameEnquiryID { get; set; }
        public string ResponseCode { get; set; }
        public string TransferCharge { get; set; }
        public string SessionID { get; set; }
    }

    public class KudaProviderBankTransferResponse
    {
        public string RequestReference { get; set; }
        public string TransactionReference { get; set; }
        public string Status { get; set; }
        public string ResponseCode { get; set; }
        public string Message { get; set; }

}


}
