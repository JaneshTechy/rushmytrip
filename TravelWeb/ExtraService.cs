using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb
{
    public partial class ExtraService
    {
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }
        public string ServiceDescription { get; set; }
        public decimal? Amount { get; set; }
        public string DestinationCode { get; set; }
        public string WebsiteId { get; set; }
        public bool? IsActive { get; set; }
        public DateTime SystemDate { get; set; }
    }
}
