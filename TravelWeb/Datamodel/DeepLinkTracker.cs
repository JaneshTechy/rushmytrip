using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.Datamodel
{
    public partial class DeepLinkTracker
    {
        public int Id { get; set; }
        public string SearchId { get; set; }
        public string ParentSearchId { get; set; }
        public string Source { get; set; }
        public string RootSource { get; set; }
        public string Stage { get; set; }
        public string PreviousStage { get; set; }
        public string FromLocation { get; set; }
        public string ToLocation { get; set; }
        public DateTime? DepartureDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public string TripType { get; set; }
        public int Adults { get; set; }
        public int Children { get; set; }
        public int Infants { get; set; }
        public string CabinClass { get; set; }
        public string Ipaddress { get; set; }
        public string UserAgent { get; set; }
        public string BookingReference { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public decimal? PriceWithoutMarkup { get; set; }
        public decimal? Markup { get; set; }
        public string Rule { get; set; }
        public string Airline { get; set; }
        public string SubClass { get; set; }
        public string ModifiedBy { get; set; }
        public string SubDetailStage { get; set; }
        public decimal TotalAmount { get; set; }
        public string InboundAirline { get; set; }
        public string OutboundAirline { get; set; }
        public int? Stops { get; set; }
        public string InboundClass { get; set; }
        public string OutboundClass { get; set; }
    }
}
