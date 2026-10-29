using System.Collections.Generic;
using TravelWeb.Datamodel;
using TravelWeb.Models;

namespace TravelWeb.Areas.Admin.Models
{
    public class EmailbookingModel
    {
        public List<FlightsSegment> InBoundFlights { get; set; }
        public List<FlightsSegment> OutBoundFlights { get; set; }
        public FlightBooking FlightBooking { get; set; }
        public List<BookingPaxDetail> BookingPaxDetail { get; set; }

        public Airport FromLocation { get; set; }
        public Airport ToLocation { get; set; }

        public string FromTime { get; set; }
        public string ToTime { get; set; }
    }
}
