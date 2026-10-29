using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.Datamodel
{
    public partial class Fareinfo
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
        public decimal? YthFare { get; set; }
        public decimal? ChdFare { get; set; }
        public decimal? InfFare { get; set; }
        public string Class { get; set; }
        public string FareBasic { get; set; }
        public string RuleKey { get; set; }
        public string WebsiteId { get; set; }
        public DateTime? DepartDate { get; set; }
        public DateTime? ReturnDate { get; set; }
    }
}
