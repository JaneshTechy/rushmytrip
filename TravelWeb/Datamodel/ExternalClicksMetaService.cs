using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.Datamodel
{
    public partial class ExternalClicksMetaService
    {
        public int Id { get; set; }
        public string UserHit { get; set; }
        public long? BookingRef { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public DateTime? DepartDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public int? Adt { get; set; }
        public int? Chd { get; set; }
        public int? Inf { get; set; }
        public string JType { get; set; }
        public string CabinClass { get; set; }
        public string Airline { get; set; }
        public string WebsiteId { get; set; }
        public int? Segments { get; set; }
        public DateTime SystemDate { get; set; }
        public string UserIpadress { get; set; }
        public string Source { get; set; }
    }
}
