using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.Datamodel
{
    public partial class BankDetails2
    {
        public int BankId { get; set; }
        public int UserId { get; set; }
        public string AccountNumber2 { get; set; }
        public string Ifsc2 { get; set; }
        public string AccountHolderName2 { get; set; }
        public string BranchName2 { get; set; }
        public string BankName2 { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
