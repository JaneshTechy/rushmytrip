using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.DatabaseContext
{
    public partial class FlightsSearchNew
    {
        public Guid SearchId { get; set; }
        public string SearchResult { get; set; }
        public DateTime? DateCreated { get; set; }
    }
}
