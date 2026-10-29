using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.DatabaseContext
{
    public partial class FareMarkUpNew
    {
        public int? Id { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public string Airlines { get; set; }
        public string MarkUpType { get; set; }
        public decimal? MarkUp { get; set; }
        public string Classes { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public string PaxType { get; set; }
        public bool? Status { get; set; }
        public int? IsInt { get; set; }
        public int? ClassType { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? LastUpdate { get; set; }
        public string Notes { get; set; }
        public int? IsRet { get; set; }
    }
}
