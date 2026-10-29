using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.Datamodel
{
    public partial class RecentSearch
    {
        public int Id { get; set; }
        public string Depart { get; set; }
        public string Arrival { get; set; }
        public string DepartDate { get; set; }
        public string ArrivalDate { get; set; }
        public string Class { get; set; }
        public string Airline { get; set; }
        public string Jtype { get; set; }
        public string UserHit { get; set; }
        public string Adt { get; set; }
        public string Chd { get; set; }
        public string Inf { get; set; }
        public string WebsiteId { get; set; }
        public string UserId { get; set; }
        public DateTime SystemDate { get; set; }
    }
}
