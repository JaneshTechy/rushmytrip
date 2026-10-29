using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb
{
    public partial class PopularDestinationFlight
    {
        public int Id { get; set; }
        public string Destination { get; set; }
        public string AirlineCode { get; set; }
        public string CabinClass { get; set; }
        public decimal? Fare { get; set; }
        public decimal? ChdFare { get; set; }
        public decimal? InfFare { get; set; }
        public string ViaPoint { get; set; }
        public DateTime? TravelFrom { get; set; }
        public DateTime? TravelTo { get; set; }
        public string Baggage { get; set; }
        public DateTime? SalesTill { get; set; }
        public bool? IsActive { get; set; }
    }
}
