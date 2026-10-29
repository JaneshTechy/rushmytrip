using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TravelWeb.Models
{
    public class MarkupRuleDto
    {
        public int RuleId { get; set; }
        public string RuleName { get; set; }

        public HashSet<string> Origins { get; set; }
        public HashSet<string> Destinations { get; set; }
        public HashSet<string> Airlines { get; set; }
        public HashSet<string> CabinClasses { get; set; }
        public string CabinClass { get; set; }
        public string JourneyType { get; set; }
        public string MarkType { get; set; }

        public decimal MarkUp { get; set; }
        public bool Status { get; set; }

        public int? MinDays { get; set; }
        public int? MaxDays { get; set; }
        public int? AdvanceDays { get; set; }

        public decimal? MarkApplicable2 { get; set; }

        public HashSet<string> RAirlines { get; set; }
        public HashSet<string> PAirlines { get; set; }

        public int? Stops { get; set; }
        
        public HashSet<string> RestrictedClass { get; set; }

        public (decimal? min, decimal? max) MinimumAmountRange { get; set; }

        public int? PaxCount { get; set; }
        public string PaxType { get; set; }

        public DateTime? TravelDateFrom { get; set; }
        public DateTime? TravelDateTo { get; set; }
        public DateTime? SaleDateFrom { get; set; }
        public DateTime? SaleDateTo { get; set; }

        public DateTime? BlockODateFrom { get; set; }
        public DateTime? BlockODateTo { get; set; }

        public string UpdatedBy { get; set; }

        public string ClassOb { get; set; }
        public string ClassIb { get; set; }

        public string Company { get; set; }
        public string CampId { get; set; }

        public string ReplaceAirline { get; set; }
        public string ParallelAirline { get; set; }
        public int? InboundTime { get; set; }
        public int? OutboundTime { get; set; }


    }

}
