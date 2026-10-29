using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb
{
    public partial class Quotation1
    {
        public int QuotationId { get; set; }
        public string FromCity { get; set; }
        public string ToCity { get; set; }
        public string Class { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string Message { get; set; }
        public string Email { get; set; }
        public int? Passenger { get; set; }
        public string PhoneNumber { get; set; }
    }
}
