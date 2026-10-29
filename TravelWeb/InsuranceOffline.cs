using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb
{
    public partial class InsuranceOffline
    {
        public int Id { get; set; }
        public string DestinationCode { get; set; }
        public decimal? Amount { get; set; }
        public string AmountType { get; set; }
        public string WebsiteId { get; set; }
        public DateTime? ValidTill { get; set; }
        public bool? IsActive { get; set; }
    }
}
