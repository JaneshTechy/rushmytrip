using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.Datamodel
{
    public partial class MarkupRule
    {
        public int RuleId { get; set; }
        public string RuleName { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public string Airline { get; set; }
        public string CabinClass { get; set; }
        public string JourneyType { get; set; }
        public string ClassOb { get; set; }
        public string ClassIb { get; set; }
        public string MarkType { get; set; }
        public decimal? MarkUp { get; set; }
        public bool? Status { get; set; }
        public int? MinDays { get; set; }
        public int? MaxDays { get; set; }
        public int? AdvanceDays { get; set; }
        public decimal? MarkApplicable2 { get; set; }
        public string Rairline { get; set; }
        public string Pairline { get; set; }
        public int? Stops { get; set; }
        public int? Ttime { get; set; }
        public string RestrictedClass { get; set; }
        public string MinimumAmount { get; set; }
        public int? PaxCount { get; set; }
        public string PaxType { get; set; }
        public DateTime? TravelDateFrom { get; set; }
        public DateTime? TravelDateTo { get; set; }
        public DateTime? SaleDateFrom { get; set; }
        public DateTime? SaleDateTo { get; set; }
        public DateTime? BlockOdateFrom { get; set; }
        public DateTime? BlockOdateTo { get; set; }
        public string Company { get; set; }
        public string CampId { get; set; }
        public string UpdateBy { get; set; }
        public int? InboundTime { get; set; }
        public int? OutboundTime { get; set; }
        public DateTime? CreateDate { get; set; }
    }
}
