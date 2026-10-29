using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.Datamodel
{
    public partial class StCity
    {
        public string CityCode { get; set; }
        public string ProvinceCode { get; set; }
        public string ProvinceCountryCode { get; set; }
        public string CountryCode { get; set; }
        public byte SortPriority { get; set; }
        public bool? CityWithPort { get; set; }
        public string Id { get; set; }
    }
}
