using System;

namespace TravelWeb.Models
{
    public class TrackerVM
    {
        public int Id { get; set; }
        public string IPAddress { get; set; }
        public string RootSource { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Airline { get; set; }
        public string SubClass { get; set; }
        public DateTime? Depart { get; set; }
        public DateTime? Return { get; set; }
        public string TripType { get; set; }
        public int? ADT { get; set; }
        public int? CHD { get; set; }
        public int? INF { get; set; }
        public DateTime? CreatedDate { get; set; }
        public decimal? TotalFare { get; set; }
        public decimal? Markup { get; set; }
        public decimal? TotalAfterMarkup { get; set; }
        public string Rule { get; set; }

    }
}
