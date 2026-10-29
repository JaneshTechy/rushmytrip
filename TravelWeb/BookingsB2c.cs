using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb
{
    public partial class BookingsB2c
    {
        public int BookingId { get; set; }
        public string PnrNo { get; set; }
        public string Bookingstatus { get; set; }
        public string WebsiteId { get; set; }
        public DateTime? SystemDate { get; set; }
        public DateTime? TravelDateFrom { get; set; }
        public DateTime? TravelDateTo { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public string TotalPax { get; set; }
        public string RandomNo { get; set; }
        public string Emailid { get; set; }
        public string PaylaterUrl { get; set; }
    }
}
