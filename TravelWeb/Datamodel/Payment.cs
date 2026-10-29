using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.Datamodel
{
    public partial class Payment
    {
        public int Id { get; set; }
        public string OrderId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string AnetTransactionId { get; set; }
        public string Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string GatewayEnv { get; set; }
        public string AuthCode { get; set; }
        public string AvsResult { get; set; }
        public string CvvResult { get; set; }
        public string CustomerEmail { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string InvoiceNumber { get; set; }
        public string RawResponseJson { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string GatewayAccount { get; set; }
    }
}
