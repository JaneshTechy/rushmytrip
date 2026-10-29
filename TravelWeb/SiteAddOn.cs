using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb
{
    public partial class SiteAddOn
    {
        public int Id { get; set; }
        public string AddOnValue { get; set; }
        public string AddOnDisplayName { get; set; }
        public decimal? Charges { get; set; }
        public string ChargeType { get; set; }
        public string Websiteid { get; set; }
        public bool IsActive { get; set; }
    }
}
