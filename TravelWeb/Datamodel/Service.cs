using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.Datamodel
{
    public partial class Service
    {
        public int ServiceId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        public string Type { get; set; }
        public string WebsiteId { get; set; }
        public double? Amount { get; set; }
        public bool? IsActive { get; set; }
        public DateTime SystemDate { get; set; }
    }
}
