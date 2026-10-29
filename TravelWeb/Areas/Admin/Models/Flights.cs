using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web;

namespace TravelWeb.Admin.Models
{
    public class Flights
    {
        public string Provider { get; set; }
        public string FlightId { get; set; }
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
    }

    public class FareMarkUp
    {
        public int Id { get; set; }
        [Required]
        public string Origin { get; set; }
        [Required]
        public string Destination { get; set; }
        [Required]
        public string Airlines { get; set; }
        [Required]
        public string MarkUpType { get; set; }
        [Required]
        public double MarkUp { get; set; }
        [Required]
        public string Class { get; set; }
        [Required]
        public string StartDate { get; set; }
        [Required]
        public string EndDate { get; set; }
        [Required]
        public string EffectiveDate { get; set; }
        [Required]
        public string Status { get; set; }
        [Required]
        public string PaxType { get; set; }
    }

    public class FareMarkup
    {
        public double OutboundMarkUp { get; set; }
        public double InboundMarkUp { get; set; }
        public double OutboundMarkUpC { get; set; }
        public double InboundMarkUpC { get; set; }
        public double OutboundMarkUpI { get; set; }
        public double InboundMarkUpI { get; set; }
        public double WorldspanFare { get; set; }
        public double WorldspanTax { get; set; }
        public double WorldspanFareC { get; set; }
        public double WorldspanTaxC { get; set; }
        public double WorldspanFareI { get; set; }
        public double WorldspanTaxI { get; set; }
    }

    public class JetcostFlight
    {
        public long Id { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public string Airline { get; set; }
        public string LastTicketingDate { get; set; }
        public string FareType { get; set; }
        public string FlightClass { get; set; }
        public string FlightWay { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public List<JetcostFlightFare> FareDetails { get; set; }
        public List<JetcostFlightSagement> SectorDetails { get; set; }
    }
    public class JetcostFlightFareMasterExport
    {
        public string Origin { get; set; }
        public string Destination { get; set; }
        public string Airline { get; set; }
        public string Cabin { get; set; }
        public string TripType { get; set; }
        public string DepFrom { get; set; }
        public string DepTo { get; set; }
        public string RetFrom { get; set; }
        public string RetTo { get; set; }
        public string SellFrom { get; set; }
        public string SellTo { get; set; }
        public string AdtFare { get; set; }
        public string AdtTax { get; set; }
        public string AdtMarkup { get; set; }
        public string ChdFare { get; set; }
        public string ChdTax { get; set; }
        public string ChdMarkup { get; set; }
        public string InfFare { get; set; }
        public string InfTax { get; set; }
        public string InfMarkup { get; set; }
        public string MaxStay { get; set; }
    }
    public class JetcostFlightSagement
    {
        public long Id { get; set; }
        public long FlightId { get; set; }
        public string Airline { get; set; }
        public string FromAirportCode { get; set; }
        public string FromTime { get; set; }
        public int FromDayInterval { get; set; }
        public string ToAirportCode { get; set; }
        public string ToTime { get; set; }
        public int ToDayInterval { get; set; }
        public string FlightNo { get; set; }
        public string EquipmentType { get; set; }
        public string FlightClass { get; set; }
        public string FromTerminal { get; set; }
        public string ToTerminal { get; set; }
        public string CodeShare { get; set; }
        public string EFT { get; set; }
        public bool IsReturn { get; set; }
        public string Baggage { get; set; }
        public string Status { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
    public class FlightBookingExport
    {
        public string BookingId { get; set; }
        public string PNRNo { get; set; }
        public string SourceMedia { get; set; }
        public string Provider { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Airline { get; set; }
        public string FullName { get; set; }
        public string FlightFare { get; set; }
        public string Profit { get; set; }
        public string BookingDate { get; set; }
        public string Payment { get; set; }
        public string Agent { get; set; }
        public string Status { get; set; }
    }
    public class FlightsSegment
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
        public string Availability { get; set; }
        public string BookingCode { get; set; }
        public bool IsReturn { get; set; }
    }

    public class FlightSearchExport
    {
        public string SiteCode { get; set; }
        public string SourceMedia { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string DepDate { get; set; }
        public string RetDate { get; set; }
        public string SearchDate { get; set; }
        public string Adult { get; set; }
        public string Child { get; set; }
        public string Infant { get; set; }
        public string FlightWay { get; set; }
        public string FlightClass { get; set; }
        public string Airline { get; set; }
    }


    public class SupplierReportExport
    {
        public string BookingId { get; set; }
        public string FullName { get; set; }
        public string BookingDate { get; set; }
        public string Reference { get; set; }
        public string FlightInfo { get; set; }
        public string DepartureDate { get; set; }
        public string Amount { get; set; }
        public string Status { get; set; }
    }
    public class FlightsFare
    {
        public double AdultFare { get; set; }
        public double ChildFare { get; set; }
        public double InfantFare { get; set; }
        public double AdultTax { get; set; }
        public double ChildTax { get; set; }
        public double InfantTax { get; set; }
        public double AdultMarkup { get; set; }
        public double ChildMarkup { get; set; }
        public double InfantMarkup { get; set; }
        public int Adult { get; set; }
        public int Child { get; set; }
        public int Infant { get; set; }
        public double AtolCharge { get; set; }
        public double CardCharge { get; set; }
        public double HotelCardCharge { get; set; }
        public double CarCardCharge { get; set; }
        public double AddonCharge { get; set; }
        public double ThirdAtolCharge { get; set; }
        public double PnrCharge { get; set; }
        public double AvgCost
        {
            set { }
            get
            {
                return (GrandTotal / (Adult + Child + Infant));
            }
        }
        public double GrandTotal
        {
            set { }
            get
            {
                //return (((AdultFare + AdultMarkup + AdultTax) * Adult) + ((ChildFare + ChildMarkup + ChildTax) * Child) + ((InfantFare + InfantMarkup + InfantTax) * Infant));
                return (((AdultFare + AdultMarkup) * Adult) + ((ChildFare + ChildMarkup) * Child) + ((InfantFare + InfantMarkup + InfantTax) * Infant));
            }
        }
    }
    public class FlightFare
    {
        public int AdultCount { get; set; }
        public double AdultGrossAmountPP { get; set; }
        public double AdultGrossAmount { get; set; }
        public double AdultNetAmount { get; set; }
        public double AdultTaxAmount { get; set; }
        public double AdultTotalAmount { get; set; }
        public double AdultProfitAmount { get; set; }
        public string AdultProvider { get; set; }

        public int YouthCount { get; set; }
        public double YouthGrossAmountPP { get; set; }
        public double YouthGrossAmount { get; set; }
        public double YouthNetAmount { get; set; }
        public double YouthTaxAmount { get; set; }
        public double YouthTotalAmount { get; set; }
        public double YouthProfitAmount { get; set; }
        public string YouthProvider { get; set; }

        public int ChildCount { get; set; }
        public double ChildGrossAmountPP { get; set; }
        public double ChildGrossAmount { get; set; }
        public double ChildNetAmount { get; set; }
        public double ChildTaxAmount { get; set; }
        public double ChildTotalAmount { get; set; }
        public double ChildProfitAmount { get; set; }
        public string ChildProvider { get; set; }

        public int InfantCount { get; set; }
        public double InfantGrossAmountPP { get; set; }
        public double InfantGrossAmount { get; set; }
        public double InfantNetAmount { get; set; }
        public double InfantTaxAmount { get; set; }
        public double InfantTotalAmount { get; set; }
        public double InfantProfitAmount { get; set; }
        public string InfantProvider { get; set; }


        public int AdminCount { get; set; }
        public double AdminGrossAmountPP { get; set; }
        public double AdminGrossAmount { get; set; }
        public double AdminNetAmount { get; set; }
        public double AdminTaxAmount { get; set; }
        public double AdminTotalAmount { get; set; }
        public double AdminProfitAmount { get; set; }
        public string AdminProvider { get; set; }


        public int AtolCount { get; set; }
        public double AtolGrossAmountPP { get; set; }
        public double AtolGrossAmount { get; set; }
        public double AtolNetAmount { get; set; }
        public double AtolTaxAmount { get; set; }
        public double AtolTotalAmount { get; set; }
        public double AtolProfitAmount { get; set; }
        public string AtolProvider { get; set; }

        public int CardChargesCount { get; set; }
        public double CardChargesGrossAmountPP { get; set; }
        public double CardChargesGrossAmount { get; set; }
        public double CardChargesNetAmount { get; set; }
        public double CardChargesTaxAmount { get; set; }
        public double CardChargesTotalAmount { get; set; }
        public double CardChargesProfitAmount { get; set; }
        public string CardChargesProvider { get; set; }
        public double SupplierTotalAmount { get; set; }
        public double GrossTotal { get; set; }
        public double NetTotal { get; set; }
        public double TaxTotal { get; set; }
        public double ProfitTotal { get; set; }

        public double AtolCharge { get; set; }
        public double CardCharge { get; set; }

        public double HotelCardCharge { get; set; }

        public double CarCardCharge { get; set; }

        public double GrandTotal
        {
            set { }
            get
            {
                return 0;//(((AdultFare + AdultMarkup + AdultTax) * Adult) + ((ChildFare + ChildMarkup + ChildTax) * Child) + ((InfantFare + InfantMarkup + InfantTax) * Infant));
            }
        }

    }
    
    public class SupplierFare
    {
        public string Id { get; set; }
        public string Product { get; set; }
        public string Supplier { get; set; }

        private string _Amount = "0";
        public string Amount { get; set; }
        public string BookingReferenceNo { get; set; }
        public string TicketIssuedDate { get; set; }
        public string PaymentStatus { get; set; }

        public string FlightProduct { get; set; }
        public string FlightSupplier { get; set; }

        private string _FlightAmount = "0";
        public string FlightAmount { get; set; }
        public string FlightBookingReferenceNo { get; set; }
        public string FlightTicketIssuedDate { get; set; }
        public string FlightPaymentStatus { get; set; }

        public string FlightSupplierDepositAmount1 { get; set; }
        public string FlightSupplierDepositDate1 { get; set; }

        public string FlightSupplierDepositAmount2 { get; set; }
        public string FlightSupplierDepositDate2 { get; set; }

        public string FlightSupplierDepositAmount3 { get; set; }
        public string FlightSupplierDepositDate3 { get; set; }
        public string FlightSupplierBalance { get; set; }

        public string HotelProduct { get; set; }
        public string HotelSupplier { get; set; }

        private string _HotelAmount = "0";
        public string HotelAmount { get; set; }
        public string HotelBookingReferenceNo { get; set; }
        public string HotelTicketIssuedDate { get; set; }
        public string HotelPaymentStatus { get; set; }

        public string HotelSupplierDepositAmount1 { get; set; }
        public string HotelSupplierDepositDate1 { get; set; }
        public string HotelSupplierDepositAmount2 { get; set; }
        public string HotelSupplierDepositDate2 { get; set; }
        public string HotelSupplierDepositAmount3 { get; set; }
        public string HotelSupplierDepositDate3 { get; set; }
        public string HotelSupplierBalance { get; set; }

        public string CarProduct { get; set; }
        public string CarSupplier { get; set; }

        private string _CarAmount = "0";
        public string CarAmount { get; set; }
        public string CarBookingReferenceNo { get; set; }
        public string CarTicketIssuedDate { get; set; }
        public string CarPaymentStatus { get; set; }

        public string CarSupplierDepositAmount1 { get; set; }
        public string CarSupplierDepositDate1 { get; set; }
        public string CarSupplierDepositAmount2 { get; set; }
        public string CarSupplierDepositDate2 { get; set; }
        public string CarSupplierDepositAmount3 { get; set; }
        public string CarSupplierDepositDate3 { get; set; }
        public string CarSupplierBalance { get; set; }

        public string ActivityProduct { get; set; }
        public string ActivitySupplier { get; set; }

        private string _ActivityAmount = "0";
        public string ActivityAmount { get; set; }
        public string ActivityBookingReferenceNo { get; set; }
        public string ActivityTicketIssuedDate { get; set; }
        public string ActivityPaymentStatus { get; set; }

        public string ActivitySupplierDepositAmount1 { get; set; }
        public string ActivitySupplierDepositDate1 { get; set; }
        public string ActivitySupplierDepositAmount2 { get; set; }
        public string ActivitySupplierDepositDate2 { get; set; }
        public string ActivitySupplierDepositAmount3 { get; set; }
        public string ActivitySupplierDepositDate3 { get; set; }
        public string ActivitySupplierBalance { get; set; }
        //passenger's received amount
        public string PassengersReceivedAmount1 { get; set; }
        public string PassengersReceivedDate1 { get; set; }
        public string PassengersReceivedMode1 { get; set; }
        public string PassengersReceivedCardCharge1 { get; set; }

        public string PassengersReceivedAmount2 { get; set; }
        public string PassengersReceivedDate2 { get; set; }
        public string PassengersReceivedMode2 { get; set; }
        public string PassengersReceivedCardCharge2 { get; set; }
        public string PassengersReceivedAmount3 { get; set; }
        public string PassengersReceivedDate3 { get; set; }
        public string PassengersReceivedMode3 { get; set; }
        public string PassengersReceivedCardCharge3 { get; set; }
        public string PassengersBalanceAmount { get; set; }
        public string PassengersReceivedReminderDate { get; set; }
    }

    public class JetcostFlightFare
    {
        public long Id { get; set; }
        public long FlightId { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public string Airline { get; set; }
        public string LastTicketingDate { get; set; }
        public string FareType { get; set; }
        public string FlightClass { get; set; }
        public string FlightWay { get; set; }
        public DateTime DepDateFrom { get; set; }
        public DateTime DepDateTo { get; set; }
        public DateTime RetDateFrom { get; set; }
        public DateTime RetDateTo { get; set; }
        public DateTime SellDateFrom { get; set; }
        public DateTime SellDateTo { get; set; }
        public float AdultFare { get; set; }
        public float AdultTax { get; set; }
        public float AdultMarkup { get; set; }
        public float ChildFare { get; set; }
        public float ChildTax { get; set; }
        public float ChildMarkup { get; set; }
        public float InfantFare { get; set; }
        public float InfantTax { get; set; }
        public float InfantMarkup { get; set; }
        public float Markup { get; set; }
        public string SellType { get; set; }
        public int MaxStay { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
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
        public string ApiKey { get; set; }
    }
   
    

    public class FlightsResult
    {
        public string Error { get; set; }
        public int MinPrice { get; set; }
        public int MaxPrice { get; set; }
        public List<FlightsExtra> StopExtra { get; set; }
        public List<FlightsExtra> BarExtra { get; set; }
        public List<FlightsExtra> AirlineExtra { get; set; }
        public FlightsSearch FlightsSearch { get; set; }
        public List<Flights> FlightsList { get; set; }
    }

    public class FlightsExtra
    {
        public string FlightId { get; set; }
        public string ExtraType { get; set; }
        public Airline Airline { get; set; }
        public double TotalCost { get; set; }
        public int TotalTime { get; set; }
        public int TotalStop { get; set; }
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

    public class Airline
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }

    public class Country
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }
    
    public class Markup
    {
        public int Id { get; set; }
        public string SiteCode { get; set; }
        public string FetchType { get; set; }
        public string Provider { get; set; }
        public double AdultMarkup { get; set; }
        public double ChildMarkup { get; set; }
        public double InfantMarkup { get; set; }
    }

    public class OneDFlight
    {
        public long Id { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public string Airline { get; set; }
        public string LastTicketingDate { get; set; }
        public string FareType { get; set; }
        public string FlightClass { get; set; }
        public string FlightWay { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public List<OneDFlightFare> FareDetails { get; set; }
        public List<OneDFlightSagement> SectorDetails { get; set; }
    }

    public class OneDFlightFare
    {
        public long Id { get; set; }
        public long FlightId { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public string Airline { get; set; }
        public string LastTicketingDate { get; set; }
        public string FareType { get; set; }
        public string FlightClass { get; set; }
        public string FlightWay { get; set; }
        public DateTime DepDateFrom { get; set; }
        public DateTime DepDateTo { get; set; }
        public DateTime RetDateFrom { get; set; }
        public DateTime RetDateTo { get; set; }
        public DateTime SellDateFrom { get; set; }
        public DateTime SellDateTo { get; set; }
        public float AdultFare { get; set; }
        public float AdultTax { get; set; }
        public float AdultMarkup { get; set; }
        public float ChildFare { get; set; }
        public float ChildTax { get; set; }
        public float ChildMarkup { get; set; }
        public float InfantFare { get; set; }
        public float InfantTax { get; set; }
        public float InfantMarkup { get; set; }
        public float Markup { get; set; }
        public string SellType { get; set; }
        public int MaxStay { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
    }

    public class OneDFlightSagement
    {
        public long Id { get; set; }
        public long FlightId { get; set; }
        public string Airline { get; set; }
        public string FromAirportCode { get; set; }
        public string FromTime { get; set; }
        public int FromDayInterval { get; set; }
        public string ToAirportCode { get; set; }
        public string ToTime { get; set; }
        public int ToDayInterval { get; set; }
        public string FlightNo { get; set; }
        public string EquipmentType { get; set; }
        public string FlightClass { get; set; }
        public string FromTerminal { get; set; }
        public string ToTerminal { get; set; }
        public string CodeShare { get; set; }
        public string EFT { get; set; }
        public bool IsReturn { get; set; }
        public string Baggage { get; set; }
        public string Status { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
    }

    public class OneDFareMasterExport
    {
        public string Origin { get; set; }
        public string Destination { get; set; }
        public string Airline { get; set; }
        public string Cabin { get; set; }
        public string TripType { get; set; }
        public string DepFrom { get; set; }
        public string DepTo { get; set; }
        public string RetFrom { get; set; }
        public string RetTo { get; set; }
        public string SellFrom { get; set; }
        public string SellTo { get; set; }
        public string AdtFare { get; set; }
        public string AdtTax { get; set; }
        public string AdtMarkup { get; set; }
        public string ChdFare { get; set; }
        public string ChdTax { get; set; }
        public string ChdMarkup { get; set; }
        public string InfFare { get; set; }
        public string InfTax { get; set; }
        public string InfMarkup { get; set; }
        public string MaxStay { get; set; }
    }
    public class DealPhoneMasterExport
    {
        public string Origin { get; set; }
        public string Destination { get; set; }
        public string Airline { get; set; }
        public string FlightClass { get; set; }
        public string FlightWay { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string AdultFare { get; set; }
        public string ChildFare { get; set; }
        public string InfantFare { get; set; }

    }
    public class HotelSearchExport
    {
        public string SiteCode { get; set; }
        public string SourceMedia { get; set; }
        public string CityName { get; set; }
        public string CountryName { get; set; }
        public string CheckIn { get; set; }
        public string CheckOut { get; set; }
        public string SearchDate { get; set; }
        public string Rooms { get; set; }
        public string Adults { get; set; }
        public string Childs { get; set; }

    }
}