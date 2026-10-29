using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.Datamodel
{
    public partial class InclusionExclusion
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string Type { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? SystemDate { get; set; }
    }
}
