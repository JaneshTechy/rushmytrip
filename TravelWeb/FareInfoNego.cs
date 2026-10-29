using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb
{
    public partial class FareInfoNego
    {
        public string Id { get; set; }
        public DateTime? CrDt { get; set; }
        public DateTime? LastDt { get; set; }
        public string Depart { get; set; }
        public string Arrive { get; set; }
        public string Cabin { get; set; }
        public string Airline { get; set; }
        public string Ftype { get; set; }
        public decimal? AdtFare { get; set; }
        public decimal? ChdFare { get; set; }
        public decimal? InfFare { get; set; }
        public string Class { get; set; }
        public string FareBasic { get; set; }
        public string RuleKey { get; set; }
        public string WebsiteId { get; set; }
    }
}
