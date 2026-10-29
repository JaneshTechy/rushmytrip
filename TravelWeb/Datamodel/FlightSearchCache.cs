using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.Datamodel
{
    public partial class FlightSearchCache
    {
        public int SearchKey { get; set; }
        public string SearchId { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? Adult { get; set; }
        public int? Child { get; set; }
        public int? Infant { get; set; }
        public int TripType { get; set; }
        public string FileName { get; set; }
        public string CacheKey { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
