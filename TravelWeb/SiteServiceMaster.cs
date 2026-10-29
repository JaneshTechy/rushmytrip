using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb
{
    public partial class SiteServiceMaster
    {
        public int ServiceMasterId { get; set; }
        public string ServiceName { get; set; }
        public string ServiceText { get; set; }
        public string WebsiteId { get; set; }
    }
}
