using System.Collections.Generic;

namespace TravelWeb.Models
{
    public class PaymentSettings
    {
        //public string ApiLoginId { get; set; } = "";
        //public string TransactionKey { get; set; } = "";
        //public bool UseSandbox { get; set; } = true;
        //public string AcceptJsApiLoginId { get; set; } = "";
        //public string AcceptJsClientKey { get; set; } = "";
        //public int DuplicateWindowSeconds { get; set; } = 120;

        public string DefaultAccount { get; set; } = "Account1";
        public int DuplicateWindowSeconds { get; set; } = 120;
        public Dictionary<string, PaymentAccount> Accounts { get; set; } = new Dictionary<string, PaymentAccount>();
    }
    public class PaymentAccount
    {
        public string DisplayName { get; set; } = "";
        public string ApiLoginId { get; set; } = "";
        public string TransactionKey { get; set; } = "";
        public string AcceptJsApiLoginId { get; set; } = "";
        public string AcceptJsClientKey { get; set; } = "";
        public bool UseSandbox { get; set; } = true;
    }
}
