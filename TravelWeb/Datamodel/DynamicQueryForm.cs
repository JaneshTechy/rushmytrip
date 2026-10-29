using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.Datamodel
{
    public partial class DynamicQueryForm
    {
        public int Id { get; set; }
        public string ControlName { get; set; }
        public int? Sequence { get; set; }
        public string Type { get; set; }
        public string Websiteid { get; set; }
        public bool? IsActive { get; set; }
    }
}
