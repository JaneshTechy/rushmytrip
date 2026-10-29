using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb
{
    public partial class MarkupDetailsHotel
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
        public bool? PerNight { get; set; }
        public bool? PerRoom { get; set; }
        public int? Rating { get; set; }
        public decimal? Markup { get; set; }
        public DateTime? TravelDateFrom { get; set; }
        public DateTime? TravelDateTo { get; set; }
        public DateTime? SearchDateFrom { get; set; }
        public DateTime? SearchDateTo { get; set; }
        public string SaAccountNo { get; set; }
        public bool? IsRecursive { get; set; }
        public string MarkupType { get; set; }
        public DateTime SystemDate { get; set; }
    }
}
