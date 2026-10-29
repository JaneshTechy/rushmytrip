using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.Datamodel
{
    public partial class MarkupGroupMaster
    {
        public int Id { get; set; }
        public string AirlineGroup { get; set; }
        public string GroupName { get; set; }
        public DateTime? SystemDate { get; set; }
    }
}
