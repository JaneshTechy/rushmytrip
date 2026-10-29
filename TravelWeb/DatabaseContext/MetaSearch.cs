using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.DatabaseContext
{
    public partial class MetaSearch
    {
        public string FlightId { get; set; }
        public string FlightData { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
