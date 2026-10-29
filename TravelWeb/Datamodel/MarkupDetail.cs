using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.Datamodel
{
    public partial class MarkupDetail
    {
        public int Id { get; set; }
        public string Airline { get; set; }
        public string CabinClass { get; set; }
        public string BookingClass { get; set; }
        public string FareBasis { get; set; }
        public string From { get; set; }
        public string Destination { get; set; }
        public string FromCity { get; set; }
        public string ToCity { get; set; }
        public string FareType { get; set; }
        public string PaxType { get; set; }
        public decimal? Markup { get; set; }
        public DateTime? TravelDateFrom { get; set; }
        public DateTime? TravelDateTo { get; set; }
        public DateTime? SearchDateFrom { get; set; }
        public DateTime? SearchDateTo { get; set; }
        public string SaAccountNo { get; set; }
        public bool? IsRecursive { get; set; }
        public DateTime SystemDate { get; set; }
        public string MarkupType { get; set; }
        public int? DepartDurationInHours { get; set; }
        public string CityGroup { get; set; }
        public string Continent { get; set; }
        public string ReturnBookingClass { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? ReturnTravelDateFrom { get; set; }
        public DateTime? ReturnTravelDateTo { get; set; }
        public string DepartDurationCondition { get; set; }
        public int? ReturnDurationInHours { get; set; }
        public string ReturnDurationCondition { get; set; }
        public int? DepartStopOverCount { get; set; }
        public string DepartStopOverCondition { get; set; }
        public int? ReturnStopOverCount { get; set; }
        public string ReturnStopOverCondition { get; set; }
        public string JType { get; set; }
        public decimal? StartingPrice { get; set; }
        public decimal? EndingPrice { get; set; }
        public int? PaxCount { get; set; }
        public string PaxCondition { get; set; }
        public string DepartCountry { get; set; }
        public string OriginCityGroup { get; set; }
        public string RAirline { get; set; }
    }
}
