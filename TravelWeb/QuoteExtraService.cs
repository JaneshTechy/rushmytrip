using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb
{
    public partial class QuoteExtraService
    {
        public int QuoteServiceId { get; set; }
        public int? QuoteId { get; set; }
        public string ServiceName { get; set; }
        public decimal? ServiceAmount { get; set; }
        public DateTime? SystemDate { get; set; }
    }
}
