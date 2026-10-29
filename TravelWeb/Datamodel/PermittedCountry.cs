using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.Datamodel
{
    public partial class PermittedCountry
    {
        public int Id { get; set; }
        public string Countrycodes { get; set; }
        public string WebsiteId { get; set; }
        public DateTime? SystemDate { get; set; }
    }
}
