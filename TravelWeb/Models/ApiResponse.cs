using System.Collections.Generic;

namespace TravelWeb.Models
{
    public class ApiResponse
    {
        public Meta meta { get; set; }
        public List<ApiFlightData> data { get; set; }
    }

    public class Meta
    {
        public int count { get; set; }
    }

    public class ApiFlightData
    {
        public string id { get; set; }
        public bool oneWay { get; set; }
        public Price price { get; set; }
        public List<Itinerary> itineraries { get; set; }
        public List<TravelerPricing> travelerPricings { get; set; }
        public List<string> validatingAirlineCodes { get; set; }
        public int numberOfBookableSeats { get; set; }
        public string classofService { get; set; }
        public string jsonId { get; set; }
    }

    public class Price
    {
        public string currency { get; set; }
        public string @base { get; set; }
        public string fees { get; set; }
        public string grandTotal { get; set; }
    }

    public class Itinerary
    {
        public string duration { get; set; }
        public List<Segment> segments { get; set; }
    }

    public class Segment
    {
        public string airline { get; set; }
        public Departure departure { get; set; }
        public Arrival arrival { get; set; }
        public string carrierCode { get; set; }
        public string number { get; set; }
        public string duration { get; set; }
        public int numberOfStops { get; set; }
        public operating operating { get; set; }

    }
    public class operating
    {
        public string carrierCode { get; set; }
    }

    public class Departure
    {
        public string iataCode { get; set; }
        public string at { get; set; }
        public string terminal { get; set; }
    }

    public class Arrival
    {
        public string iataCode { get; set; }
        public string at { get; set; }
        public string terminal { get; set; }
    }

    public class TravelerPricing
    {
        public int travelerId { get; set; }
        public string travelerType { get; set; }
        public Price price { get; set; }
        public List<FareDetailsBySegment> fareDetailsBySegment { get; set; }
    }
    public class FareDetailsBySegment
    {
        public string cabin { get; set; }
        public string @class { get; set; }

    }


}
