using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.Datamodel
{
    public partial class BlockAirlineRule
    {
        public int RuleId { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public string CabinClassOb { get; set; }
        public string CabinClassIb { get; set; }
        public string TimeOb { get; set; }
        public string TimeIb { get; set; }
        public int? AdvanceDays { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string BlockDates { get; set; }
        public bool? Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string Company { get; set; }
        public string CampId { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string AirlineCodes { get; set; }
    }
}
