using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.Datamodel
{
    public partial class DynamicPage
    {
        public int ServiceId { get; set; }
        public string ServicePageName { get; set; }
        public string Images { get; set; }
        public string Description { get; set; }
        public string WebsiteId { get; set; }
        public bool? IsActive { get; set; }
        public DateTime SystemDate { get; set; }
    }
}
