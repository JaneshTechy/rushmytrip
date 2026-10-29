using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.Datamodel
{
    public partial class DestinationAirportMapping
    {
        public int Id { get; set; }
        public string Destination { get; set; }
        public string AirPort { get; set; }
        public string WebsiteId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool? IsActive { get; set; }
    }
}
