using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb
{
    public partial class AirlineCodeShare
    {
        public int Id { get; set; }
        public string AirlineCode { get; set; }
        public string AirlineCodeCodeShare { get; set; }
        public string WebsiteId { get; set; }
        public DateTime SystemDate { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
    }
}
