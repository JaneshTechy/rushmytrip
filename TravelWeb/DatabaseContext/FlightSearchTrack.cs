using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.DatabaseContext
{
    public partial class FlightSearchTrack
    {
        public DateTime? CreatedDate { get; set; }
        public string Dcode { get; set; }
        public string Gcode { get; set; }
        public string DepartDate { get; set; }
        public string ReturnDate { get; set; }
        public string FareTrip { get; set; }
        public string Marketing { get; set; }
    }
}
