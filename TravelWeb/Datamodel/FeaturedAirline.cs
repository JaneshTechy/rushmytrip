using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.Datamodel
{
    public partial class FeaturedAirline
    {
        public int Id { get; set; }
        public string AirCode { get; set; }
        public string AirName { get; set; }
        public DateTime SystemDate { get; set; }
    }
}
