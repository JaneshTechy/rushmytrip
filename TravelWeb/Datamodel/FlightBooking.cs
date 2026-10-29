using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.Datamodel
{
    public partial class FlightBooking
    {
        public long BookingId { get; set; }
        public string BookingNumber { get; set; }
        public DateTime? BookingDate { get; set; }
        public string FromDeparture { get; set; }
        public string ToDestination { get; set; }
        public string DepartDate { get; set; }
        public string ReturnDate { get; set; }
        public string FlightWay { get; set; }
        public string Airline { get; set; }
        public string FlightClass { get; set; }
        public int? SupportPackage { get; set; }
        public string Marketing { get; set; }
        public string Provider { get; set; }
        public int? Adult { get; set; }
        public int? Child { get; set; }
        public int? Infant { get; set; }
        public decimal? AdultFare { get; set; }
        public decimal? AdutlTax { get; set; }
        public decimal? AdultMarkup { get; set; }
        public decimal? ChildFare { get; set; }
        public decimal? ChildTax { get; set; }
        public decimal? ChildMarkup { get; set; }
        public decimal? InfantFare { get; set; }
        public decimal? InfantTax { get; set; }
        public decimal? InfantMarkup { get; set; }
        public string SupportPlan { get; set; }
        public decimal? SupportCost { get; set; }
        public decimal? BaggageCost { get; set; }
        public int? BaggageWeight { get; set; }
        public string BaggageSize { get; set; }
        public decimal? Gross { get; set; }
        public decimal? Nett { get; set; }
        public decimal? Profitt { get; set; }
        public string Status { get; set; }
        public string Agent { get; set; }
        public DateTime? AssignDate { get; set; }
        public string PassangerData { get; set; }
        public string FlightDetailsOutbound { get; set; }
        public string FlightDetailsInbound { get; set; }
        public string ContactPerson { get; set; }
        public string ContactEmail { get; set; }
        public string PaxFullName { get; set; }
        public string BillAddress1 { get; set; }
        public string BillAddress2 { get; set; }
        public string BillPostalCode { get; set; }
        public string BillCity { get; set; }
        public string BillState { get; set; }
        public string BillCountry { get; set; }
        public string CardName { get; set; }
        public string CardType { get; set; }
        public string CardNumber { get; set; }
        public string CardValMonth { get; set; }
        public int? CardValYear { get; set; }
        public string CardSecCode { get; set; }
        public DateTime? CreatedDate { get; set; }
        public decimal? CardCharge { get; set; }
        public decimal? Amount { get; set; }
        public string Comments { get; set; }
        public string IsStatus { get; set; }
        public string Ipaddress { get; set; }
        public int? StatusId { get; set; }
        public int? AgentId { get; set; }
        public decimal? Mco { get; set; }
        public int? SupplierId { get; set; }
        public bool? Isclaimed { get; set; }
        public string MarkupType { get; set; }
        public string MarkupValue { get; set; }
        public int SeatPrefAmount { get; set; }
        public decimal? TotalMco { get; set; }
        public decimal? TotalAmount { get; set; }
        public decimal? FlightCost { get; set; }
        public decimal? Initialmco { get; set; }
        public decimal? MerchantFeeInPercent { get; set; }
        public decimal? MercahntFeeAfterCal { get; set; }
        public int? SupplierIdInFeeList { get; set; }
        public decimal? SupplierFee { get; set; }
        public string PaymentType { get; set; }
        public decimal? TotalFare { get; set; }
        public decimal? OurFare { get; set; }
        public string FlightNo { get; set; }
        public int Stop { get; set; }
        public string PassengerTitle { get; set; }
        public string PhoneCountry { get; set; }
        public bool DelInd { get; set; }
        public string Currency { get; set; }
        public string SearchId { get; set; }
    }
}
