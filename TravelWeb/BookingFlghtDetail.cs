using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb
{
    public partial class BookingFlghtDetail
    {
        public long FlightSegmentId { get; set; }
        public long? BookingId { get; set; }
        public string DepartDate { get; set; }
        public string ArrivalDate { get; set; }
        public string FlightNo { get; set; }
        public string Airline { get; set; }
        public string DepartOn { get; set; }
        public string Dcode { get; set; }
        public string Gcode { get; set; }
        public string ArriveOn { get; set; }
        public string FlightType { get; set; }
        public DateTime? DepDate { get; set; }
        public DateTime? ArrDate { get; set; }
        public string BoundClass { get; set; }
        public bool? Delind { get; set; }
    }
}
