using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb
{
    public partial class BlockedAirline
    {
        public int Id { get; set; }
        public string Airlinecodes { get; set; }
        public string WebsiteId { get; set; }
        public DateTime? SystemDate { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
    }
}
