using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.Datamodel
{
    public partial class Continent
    {
        public int Id { get; set; }
        public string ContinentName { get; set; }
        public string Country { get; set; }
        public string CountryCode { get; set; }
        public string SaAccountNo { get; set; }
        public DateTime SystemDate { get; set; }
        public string ContinentImage { get; set; }
        public bool? IsFeatured { get; set; }
        public string Description { get; set; }
    }
}
