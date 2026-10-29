using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.Datamodel
{
    public partial class PaymentLink
    {
        public int PaymentId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ClientMail { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public decimal? Amount { get; set; }
        public DateTime SystemDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public bool IsActive { get; set; }
    }
}
