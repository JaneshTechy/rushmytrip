using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb
{
    public partial class AirlineRoute
    {
        public int RoutesId { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public bool? Status { get; set; }
        public string UpdatedBy { get; set; }
        public string Company { get; set; }
        public string CampId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
