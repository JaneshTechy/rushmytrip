using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.Datamodel
{
    public partial class SiteMiscService
    {
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }
        public string ServiceValue { get; set; }
        public string WebsiteId { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
