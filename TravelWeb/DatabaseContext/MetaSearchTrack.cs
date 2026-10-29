using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.DatabaseContext
{
    public partial class MetaSearchTrack
    {
        public int SearchTrackId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Dcode { get; set; }
        public string Gcode { get; set; }
        public DateTime? DepartDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public string FareTrip { get; set; }
        public string Marketing { get; set; }
        public string Ipaddress { get; set; }
    }
}
