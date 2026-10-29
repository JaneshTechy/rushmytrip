using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.DatabaseContext
{
    public partial class Airport
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string CityCode { get; set; }
        public string CityName { get; set; }
        public string StateCode { get; set; }
        public string StateName { get; set; }
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
        public string Continent { get; set; }
        public string Latitude { get; set; }
        public string Longtitude { get; set; }
    }
}
