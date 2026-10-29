using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.Datamodel
{
    public partial class HomeSequence
    {
        public int Id { get; set; }
        public string ControlName { get; set; }
        public int? Sequence { get; set; }
        public string Websiteid { get; set; }
        public DateTime? SystemDate { get; set; }
    }
}
