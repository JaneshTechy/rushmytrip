using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb
{
    public partial class PermittedAirline
    {
        public int Id { get; set; }
        public string AirlineCode { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public string WebsiteId { get; set; }
    }
}
