using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.Datamodel
{
    public partial class CacheGd
    {
        public int Id { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public string DepDate { get; set; }
        public string RetDate { get; set; }
        public string Class { get; set; }
        public int? Adults { get; set; }
        public int? Children { get; set; }
        public int? Infants { get; set; }
        public string UserHit { get; set; }
        public string GdsType { get; set; }
        public string WebsiteId { get; set; }
        public DateTime SystemDate { get; set; }
    }
}
