using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using TravelWeb.Admin.Models;
using TravelWeb.DatabaseContext;

namespace TravelWeb.Models
{
    public class Flights
    {
        public string Provider { get; set; }
        public string ApiId { get; set; }
        public string FlightId { get; set; }
        public string SearchId { get; set; }
        public string SessionId { get; set; }
        public FlightClass FlightClass { get; set; }
        public Airline Airline { get; set; }
        public string Currency { get; set; }
        public bool IsDeal { get; set; }
        public string DeepLink { get; set; }
        public int TotalTime { get; set; }
        public double TotalCost { get; set; }
        public int Stops { get; set; }
        public string FareRules { get; set; }
        public FlightsFare FlightFare { get; set; }
        public List<FlightsSegment> Outbound { get; set; }
        public List<FlightsSegment> Inbound { get; set; }
        public List<List<FlightsSegment>> OutboundList { get; set; }
        public List<List<FlightsSegment>> InboundList { get; set; }
        public double OutboundMarkUp { get; set; }
        public double InboundMarkUp { get; set; }
        public double OutboundMarkUpC { get; set; }
        public double InboundMarkUpC { get; set; }
        public double WorldspanFare { get; set; }
        public double WorldspanTax { get; set; }
        public double WorldspanFareC { get; set; }
        public double WorldspanTaxC { get; set; }
        public Baglimit Baglimit { get; set; }
        public List<BagsPrice> ListBagsPrice { get; set; }
    }
    public class FlightsSegment
    {
        public string DepDate { get; set; }
        public string ArrDate { get; set; }
        public string FareClasses { get; set; }
        public string ClassOfService { get; set; }

        public string OptionRef { get; set; }
        public string SagementRef { get; set; }
        public DateTime DepartureDate { get; set; }
        public DateTime ArrivalDate { get; set; }
        public Airport FromAirport { get; set; }
        public Airport ToAirport { get; set; }
        public Airline Airline { get; set; }
        public Airline OperatingAirline { get; set; }
        public string FromTerminal { get; set; }
        public string ToTerminal { get; set; }
        public string FlightNo { get; set; }
        public string EquipmentType { get; set; }
        public string Distance { get; set; }
        public string ETicket { get; set; }
        public string ChangePlane { get; set; }
        public string BaggageAllowance { get; set; }
        public string ElapsedTime { get; set; }
        public string TotalTime { get; set; }
        public string CabinClass { get; set; }
        public string Availability { get; set; }
        public string BookingCode { get; set; }
        public bool IsReturn { get; set; }
    }
    public class FareMarkUp
    {
        public double OutboundMarkUp { get; set; }
        public double InboundMarkUp { get; set; }
        public double OutboundMarkUpC { get; set; }
        public double InboundMarkUpC { get; set; }
        public double WorldspanFare { get; set; }
        public double WorldspanTax { get; set; }
        public double WorldspanFareC { get; set; }
        public double WorldspanTaxC { get; set; }
    }

    public class FlightsSeSagementRefgment
    {
        public string OptionRef { get; set; }
        public string SagementRef { get; set; }
        public DateTime DepartureDate { get; set; }
        public DateTime ArrivalDate { get; set; }
        public Airport FromAirport { get; set; }
        public Airport ToAirport { get; set; }
        public Airline Airline { get; set; }
        public Airline OperatingAirline { get; set; }
        public string FromTerminal { get; set; }
        public string ToTerminal { get; set; }
        public string FlightNo { get; set; }
        public string EquipmentType { get; set; }
        public string Distance { get; set; }
        public string ETicket { get; set; }
        public string ChangePlane { get; set; }
        public string BaggageAllowance { get; set; }
        public string ElapsedTime { get; set; }
        public string TotalTime { get; set; }
        public string CabinClass { get; set; }
        public string ClassOfService { get; set; }
        public string Availability { get; set; }
        public string BookingCode { get; set; }
        public string FareClasses { get; set; }
        
        public bool IsReturn { get; set; }
        public string DepDate { get; set; }
        public string ArrDate { get; set; }
    }

    public class FlightsFare
    {
        public double AdultFare { get; set; }
        public string MarkupRuleName { get; set; }
        public string CampId { get; set; }
        public string UpdatedBy { get; set; }
        public double ChildFare { get; set; }
        public double InfantFare { get; set; }
        public double AdultTax { get; set; }
        public double ChildTax { get; set; }
        public double InfantTax { get; set; }
        public double AdultMarkup { get; set; }
        public decimal AdultMarkupafterTax { get; set; }
        public double ChildMarkup { get; set; }
        public decimal ChildMarkupafterTax { get; set; }

        public double InfantMarkup { get; set; }
        public decimal InfantMarkupafterTax { get; set; }

        public int Adult { get; set; }
        public int Child { get; set; }
        public int Infant { get; set; }
        public double AtolCharge { get; set; }
        public double CardCharge { get; set; }
        public string MarkupType { get; set; }
        public string MarkupValue { get; set; }
        public double AvgCost
        {
            set { }
            get
            {
                //return (GrandTotal / (Adult + Child + Infant));
                return ((GrandTotal - InfantFare) / (Adult + Child));
            }
        }
        //public double GrandTotal
        //{
        //    set { }
        //    get
        //    {
        //        //OLD CODE WITH TAX
        //        //return (((AdultFare + AdultMarkup + AdultTax) * Adult) + ((ChildFare + ChildMarkup + ChildTax) * Child) + ((InfantFare + InfantMarkup + InfantTax) * Infant));

        //           return (((AdultFare + AdultMarkup+ AdultTax) * Adult) + ((ChildFare + ChildMarkup) * Child) + ((InfantFare + InfantMarkup + InfantTax) * Infant));


        //    }
        //}


        public double GrandTotal
        {
            set { }
            get
            {
                //OLD CODE WITH TAX
                return (((AdultFare + AdultMarkup + AdultTax) * Adult) + ((ChildFare + ChildMarkup + ChildTax) * Child) + ((InfantFare + InfantMarkup + InfantTax) * Infant));

                //return (((AdultFare + AdultMarkup + AdultTax) * Adult) + ((ChildFare + ChildMarkup + ChildTax) * Child) + ((InfantFare + InfantMarkup + InfantTax) * Infant));
                //return (((AdultFare + AdultMarkup + AdultTax)) + ((ChildFare + ChildMarkup)) + ((InfantFare + InfantMarkup + InfantTax)));


            }
        }
    }

    public class BagsPrice
    {
        public string BagsPcs { get; set; }
        public float BagsPcsPrice { get; set; }
    }

    public class Baglimit
    {
        public string hand_width { get; set; }
        public string hand_height { get; set; }
        public string hand_length { get; set; }
        public string hand_weight { get; set; }
        public string hold_width { get; set; }
        public string hold_height { get; set; }
        public string hold_length { get; set; }
        public string hold_weight { get; set; }
        public string hold_dimensions_sum { get; set; }
    }

    public class FlightsSearch
    {
        public long Id { get; set; }
        public string SearchId { get; set; }
        public Airport From { get; set; }
        public Airport To { get; set; }
        public DateTime DepDate { get; set; }
        public DateTime RetDate { get; set; }
        public int Adult { get; set; }
        public int Child { get; set; }
        public int Infant { get; set; }
        public FlightWay FlightWay { get; set; }
        public FlightClass FlightClass { get; set; }
        public Airline Airline { get; set; }
        public bool IsDirect { get; set; }
        public bool IsFlexi { get; set; }
        public string Currency { get; set; }
        public string SiteCode { get; set; }
        public string SourceMedia { get; set; }
        public bool IsDeepLink { get; set; }
        public string ApiKey { get; set; }
        public string IPAddress { get; set; }
        public string CampId { get; set; }

        //public List<MultiselectFlight> Multiflights { get; set; }
    }

    //public class MultipleFlightList
    //{
    //    public Airport From { get; set; }
    //    public Airport To { get; set; }
    //    public DateTime DepDate { get; set; }
    //    public DateTime RetDate { get; set; }

    //}
    public class MetaFlightSearch
    {
        public Flights Flights { get; set; }
        public Airport From { get; set; }
        public Airport To { get; set; }
        public Airline Airline { get; set; }
        public float Amount { get; set; }
        public FlightWay FlightWay { get; set; }
        public DateTime TripDate { get; set; }
        public DateTime SearchDate { get; set; }
        public string Provider { get; set; }
    }
    public class MultiFlightsSearch
    {
        public MultiFlightsSearch()
        {
            FlightLegs = new List<FlightLeg>();
        }
        public long Id { get; set; }
        public string SearchId { get; set; }
        public string TranId { get; set; }
        public List<FlightLeg> FlightLegs { get; set; }
        public int Adult { get; set; }
        public int Child { get; set; }
        public int Infant { get; set; }
        public FlightWay FlightWay { get; set; }
        public FlightClass FlightClass { get; set; }
        public Airline Airline { get; set; }
        public bool IsDirect { get; set; }
        public bool IsFlexi { get; set; }
        public string Currency { get; set; }
        public string SiteCode { get; set; }
        public string SourceMedia { get; set; }
        public bool IsDeepLink { get; set; }
        public string ApiKey { get; set; }
        public string Marketing { get; set; }
        public string PlatForms { get; set; }
        public DataTable ProviderList { get; set; }
        public int FlightCount { get; set; }

        public string IPAddress { get; set; }

    }
    public class MultiCityFlightsResult
    {
        public string Error { get; set; }
        public int MinPrice { get; set; }
        public int MaxPrice { get; set; }
        public string SearchId { get; set; }
        public List<FlightsExtra> StopExtra { get; set; }
        public List<FlightsExtra> BarExtra { get; set; }
        public List<FlightsExtra> AirlineExtra { get; set; }
        public MultiFlightsSearch FlightsSearch { get; set; }
        public List<MultiFlightsList> MultiFlightsList { get; set; }

    }
    public class MultiFlightsList
    {
        public MultiFlightsList()
        {
            FlightsList = new List<Flights>();
        }
        public List<Flights> FlightsList { get; set; }
    }
    public class FlightsResult
    {
        public string Error { get; set; }
        public int MinPrice { get; set; }
        public int MaxPrice { get; set; }
        public string Source { get; set; }

        public string SearchId { get; set; }
         public string OutBoundDep { get; set; }
        public string OutBoundArr { get; set; }
        public string InBoundDep { get; set; }
        public string InBoundArr { get; set; }
        public List<FlightsExtra> StopExtra { get; set; }
        public List<FlightsExtra> BarExtra { get; set; }
        public List<FlightsExtra> AirlineExtra { get; set; }
        public FlightsSearch FlightsSearch { get; set; }
        public List<Flights> FlightsList { get; set; }
        public List<Airline> ListAirline { get; set; }
        public string SearchCasheKey { get; set; }
    }

    public class FilterResult
    {
        public string Airline { get; set; }
        public float Price { get; set; }
        public List<FilterSegment> OutboundFilter { get; set; }
        public List<FilterSegment> InboundFilter { get; set; }
    }
    public class FilterSegment
    {
        public int Stops { get; set; }
        public int PlaneTime { get; set; }
    }

    public class FlightsExtra
    {
        public string FlightId { get; set; }
        public string ExtraType { get; set; }
        public Airline Airline { get; set; }
        public double TotalCost { get; set; }
        public int TotalTime { get; set; }
        public int TotalStop { get; set; }
        public string Provider { get; set; }
    }

    public class Airport
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string CityCode { get; set; }
        public string CityName { get; set; }
        public string StateCode { get; set; }
        public string StateName { get; set; }
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
        public string Continent { get; set; }
        public string Latitude { get; set; }
        public string Longtitude { get; set; }
    }
    public class FlightLeg
    {
        public Airport From { get; set; }
        public Airport To { get; set; }
        public DateTime DepDate { get; set; }
    }

    public class Airline
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
      
        public int TotalCost { get; set; }
        public int Stops { get; set; }
    }

    public class Markup
    {
        public double AdultMarkup { get; set; }
        public double ChildMarkup { get; set; }
        public double InfantMarkup { get; set; }
    }

    public class Passenger
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string PaxType { get; set; }
        public string Title { get; set; }
    }

    public class AcknowledgeData
    {
        public string Reference{ get; set; }
        public string TravelDate { get; set; }
        public string PassengerName { get; set; }
        public string PaidTo { get; set; }
        public string PaymentMethod { get; set; }
        public string Subject { get; set; }
        public string AmountPaid { get; set; }
    }
    public class BookingEmail
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
    public class Contact
    {
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string UserMessage { get; set; }
    }

    public class FlightsSearchNew
    {
        public Guid SearchId { get; set; }
        public string SearchResult { get; set; }
    }
}