using System;
using System.Collections.Generic;

namespace TravelWeb.Models
{
    public class TrackerPageVM
    {
        public List<TrackerVM> Trackers { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
        public string Website { get; set; }
        public string CurrentFilter { get; set; }   // 🔥 Important
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string RootSourceFilter { get; set; }
        public string FromLocationFilter { get; set; }
        public string ToLocationFilter { get; set; }
        public string TripTypeFilter { get; set; }

    }
}
