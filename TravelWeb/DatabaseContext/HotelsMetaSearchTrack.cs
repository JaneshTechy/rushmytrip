using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.DatabaseContext
{
    public partial class HotelsMetaSearchTrack
    {
        public int SearchTrackId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string Dcode { get; set; }
        public string Destination { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string Marketing { get; set; }
    }
}
