using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb
{
    public partial class MarkupMaster
    {
        public int MarkupId { get; set; }
        public string AirlineCode { get; set; }
        public decimal? Markup { get; set; }
        public string SaAccountNo { get; set; }
        public DateTime SystemDate { get; set; }
    }
}
