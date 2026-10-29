using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.DatabaseContext
{
    public partial class OneDflightFareNew
    {
        public string UniqueId { get; set; }
        public string FlightSegIdx { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public string Airline { get; set; }
        public string FareType { get; set; }
        public string FlightClass { get; set; }
        public string FlightWay { get; set; }
        public DateTime? DepDateFrom { get; set; }
        public DateTime? DepDateTo { get; set; }
        public DateTime? SellDateTo { get; set; }
        public int? AdultFare { get; set; }
        public int? AdultTax { get; set; }
        public int? ChildFare { get; set; }
        public int? ChildTax { get; set; }
        public int? InfantFare { get; set; }
        public int? InfantTax { get; set; }
        public int? MaxStay { get; set; }
        public int? MarkupJc { get; set; }
        public int? MarkupOd { get; set; }
        public int? IsJc { get; set; }
        public int? IsOd { get; set; }
        public int? MarkupAdtChd { get; set; }
        public long? FlightId { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string Notes { get; set; }
        public int? TotalFareJc { get; set; }
        public int? TotalFareOd { get; set; }
    }
}
