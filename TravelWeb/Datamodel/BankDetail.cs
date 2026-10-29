using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.Datamodel
{
    public partial class BankDetail
    {
        public int BankId { get; set; }
        public int UserId { get; set; }
        public string AccountNumber { get; set; }
        public string Ifsc { get; set; }
        public string AccountHolderName { get; set; }
        public string BranchName { get; set; }
        public string BankName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
