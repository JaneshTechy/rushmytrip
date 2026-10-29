using System.Collections.Generic;

namespace TravelWeb.Models
{
   
    public class FarescraperSearchReq
    {
        public AdvanceSearch AdvanceSearch { get; set; }
        public GeneralInfo GeneralInfo { get; set; }
        public PaxDetails PaxDetails { get; set; }
        public List<Segments> Segments { get; set; }
        public string Source { get; set; }
        public string Device { get; set; }
    }

    public class AdvanceSearch
    {
        public bool DirectFlight { get; set; }
    }

    public class GeneralInfo
    {
        public string Cabin { get; set; }
        public string Currency { get; set; }
        public string GUID { get; set; }
        public string JourneyType { get; set; }
        public string TripType { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string IPAddress { get; set; }
    }

    public class PaxDetails
    {
        public int Adult { get; set; }
        public int Child { get; set; }
        public int Infant { get; set; }
        public int Youth { get; set; }
    }

    public class Segments
    {
        public string ArrivalApt { get; set; }
        public string DepartApt { get; set; }
        public string DepartDate { get; set; }
    }
    public class FlightSearchResponse {
        public List<Flight> Airlines { get; set; } = new List<Flight>();

    }
    public class Flight
    {
        public string Id { get; set; }
        public string CabinClass { get; set; }
        public decimal TotalAmt { get; set; }
        public decimal TotalTax { get; set; }
        public int Stop { get; set; }
        public int TotalDuration { get; set; }
        public List<OSeg> Segments { get; set; }
        public string BookUrl { get; set; } // enrichment
    }
    public class OSeg { public List<ISeg> ISeg { get; set; } }
    public class ISeg
    {
        public string DDat { get; set; }
        public string ADat { get; set; }
        public string DTime { get; set; }
        public string ATime { get; set; }
        public string DApt { get; set; }
        public string AApt { get; set; }
        public string MA { get; set; }
        public string AirlineNm { get; set; }
        public string FN { get; set; }
        public string FBA { get; set; }
    }
}
