using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb
{
    public partial class PaymentsBlock
    {
        public int Id { get; set; }
        public string Airline { get; set; }
        public string ToCity { get; set; }
        public string WebsiteId { get; set; }
        public DateTime SystemDate { get; set; }
    }
}
