using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.Datamodel
{
    public partial class UserSearch
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string SavedUrl { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public DateTime? TravelDateFrom { get; set; }
        public DateTime? TravelDateTo { get; set; }
        public DateTime? SystemDate { get; set; }
    }
}
