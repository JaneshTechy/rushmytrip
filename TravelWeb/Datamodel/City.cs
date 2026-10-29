using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.Datamodel
{
    public partial class City
    {
        public int CityId { get; set; }
        public string Name { get; set; }
        public string State { get; set; }
        public string CountryCode { get; set; }
        public string Slug { get; set; }
        public string ImageUrl { get; set; }
        public bool? IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
