using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.Datamodel
{
    public partial class MarkupDetailsAdvance
    {
        public int Id { get; set; }
        public int MarkupDetailsId { get; set; }
        public string PaxType { get; set; }
        public int? PaxCount { get; set; }
        public string SplCondition { get; set; }
        public decimal? MarkAmount { get; set; }
        public DateTime SystemDate { get; set; }
    }
}
