using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb
{
    public partial class WlsCustomHeader
    {
        public int Id { get; set; }
        public string SaAccount { get; set; }
        public string Timing { get; set; }
        public bool? AbtaImg { get; set; }
        public bool? AtolImg { get; set; }
        public bool? LowestPriceImg { get; set; }
        public bool? AboutUsLink { get; set; }
        public bool? ContactUsLink { get; set; }
        public bool? ContactNo { get; set; }
    }
}
