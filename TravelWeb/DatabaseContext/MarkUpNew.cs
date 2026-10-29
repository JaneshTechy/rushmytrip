using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.DatabaseContext
{
    public partial class MarkUpNew
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Code { get; set; }
        public int Value { get; set; }
        public string FareType { get; set; }
        public int IsInt { get; set; }
        public string Airline { get; set; }
        public string OutboundAirline { get; set; }
        public string InboundAirline { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? OutboundStartDate { get; set; }
        public DateTime? OutboundEndDate { get; set; }
        public string StopCount { get; set; }
        public string PaxCount { get; set; }
        public string OutboundClasses { get; set; }
        public string JourneyType { get; set; }
        public string PaxType { get; set; }
        public string FromCity { get; set; }
        public string ToCity { get; set; }
        public string FromCountry { get; set; }
        public string ToCountry { get; set; }
        public string OutBoundRestriction { get; set; }
        public string InBoundRestriction { get; set; }
        public string AirlineRestriction { get; set; }
        public DateTime? InboundStartDate { get; set; }
        public DateTime? InboundEndDate { get; set; }
        public string InboundClasses { get; set; }
        public string Channel { get; set; }
        public int? IsRet { get; set; }
        public string FromCountryRestriction { get; set; }
        public string ToCountryRestriction { get; set; }
        public string FromCityRestriction { get; set; }
        public string ToCityRestriction { get; set; }
    }
}
