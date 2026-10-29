using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb
{
    public partial class QuoteTripDetail
    {
        public int QuoteTripId { get; set; }
        public int QuoteId { get; set; }
        public int QuoteFlightOptionId { get; set; }
        public string QuotationOrBooking { get; set; }
        public string AirlineName { get; set; }
        public string AirlineLogo { get; set; }
        public string FlightNum { get; set; }
        public string BookingClass { get; set; }
        public string DepartureAirport { get; set; }
        public DateTime? DepartureDateTime { get; set; }
        public string ArrivalAirport { get; set; }
        public DateTime? ArrivalDateTime { get; set; }
        public string StopSummary { get; set; }
        public string StopAirports { get; set; }
        public string TotalTravelTime { get; set; }
        public decimal? TotalPrice { get; set; }
        public string PassengerCountDetail { get; set; }
        public string LayoverDetail { get; set; }
        public string Apiname { get; set; }
        public string OfferIdOrResponseId { get; set; }
        public string TripType { get; set; }
        public string PriceBreakdownJson { get; set; }
        public DateTime? DepartureDateTimeFromRsp { get; set; }
        public string DepartureTerminal { get; set; }
        public DateTime? ArrivalDateTimeFromRsp { get; set; }
        public string ArrivalTerminal { get; set; }
        public int? TravelDurationMinutes { get; set; }
        public string AirlineCode { get; set; }
        public string FinalDestination { get; set; }
        public string BaggageAllowanceAdt { get; set; }
        public string BaggageAllowanceCnn { get; set; }
        public string BaggageAllowanceInf { get; set; }
        public string BaggageAllowanceIns { get; set; }
        public string MarriageGrp { get; set; }
        public string BookDesigCode { get; set; }
        public int? TripSegment { get; set; }
        public string AirSegmentKey { get; set; }
        public int? AirSegmentGroup { get; set; }
        public string MarketingAirlineCode { get; set; }
        public string MarketingAirlineName { get; set; }
        public string MarketingAirlineFlightNumber { get; set; }
        public decimal? AdditionalEarning { get; set; }
        public string BookingFareBasis { get; set; }
    }
}
