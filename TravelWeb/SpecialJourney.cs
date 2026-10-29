using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb
{
    public partial class SpecialJourney
    {
        public int Id { get; set; }
        public int? SpecialReturnDays { get; set; }
        public string InboundCabinCode { get; set; }
        public string OutboundCabinCode { get; set; }
        public string AirlineCode { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public string WebsiteId { get; set; }
    }
}
