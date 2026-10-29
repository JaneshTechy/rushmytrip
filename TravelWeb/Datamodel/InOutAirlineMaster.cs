using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.Datamodel
{
    public partial class InOutAirlineMaster
    {
        public int Id { get; set; }
        public string DepartAirlineCode { get; set; }
        public string ReturnAirlineCode { get; set; }
        public string WebsiteId { get; set; }
        public bool? IsActive { get; set; }
    }
}
