using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb
{
    public partial class StCountryName
    {
        public string LanguageCode { get; set; }
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
        public string Nationality { get; set; }
        public string Images { get; set; }
        public string Description { get; set; }
        public string WebsiteId { get; set; }
        public bool? IsFeatured { get; set; }
        public int Id { get; set; }
    }
}
