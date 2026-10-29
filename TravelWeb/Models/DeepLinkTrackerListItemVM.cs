using System;
using System.Collections.Generic;
using TravelWeb.Datamodel;

namespace TravelWeb.Models
{
    public class DeepLinkTrackerListItemVM
    {
        public int Id { get; set; }
        public string SearchId { get; set; }
        public string Source { get; set; }
        public string FromLocation { get; set; }
        public string ToLocation { get; set; }
        public DateTime? DepartureDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public string TripType { get; set; }
        public int Adults { get; set; }
        public int Children { get; set; }
        public int Infants { get; set; }
        public string Airline { get; set; }
        public int? Stops { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal? Markup { get; set; }
        public string Stage { get; set; }
    }

    public class DeepLinkTrackerDetailVM
    {
        public DeepLinkTracker Data { get; set; }
    }

    public class DeepLinkTrackerListVM
    {
        public List<DeepLinkTrackerListItemVM> Records { get; set; } = new List<DeepLinkTrackerListItemVM>();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }

        public int Last3DaysFSCount { get; set; }  // ⭐ NEW

        public string? SearchId { get; set; }
        public string? Source { get; set; }
    }

}
