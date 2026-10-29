using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb
{
    public partial class CityMaster
    {
        public int CityId { get; set; }
        public string CountryCode { get; set; }
        public string CityCode { get; set; }
        public string CityName { get; set; }
        public string Image { get; set; }
        public decimal? Price { get; set; }
        public string Summary { get; set; }
        public string Inclusion { get; set; }
        public string Description { get; set; }
        public string Itinerary { get; set; }
        public string WebsiteId { get; set; }
        public bool? IsFeatured { get; set; }
    }
}
