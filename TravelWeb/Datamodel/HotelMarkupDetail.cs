using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.Datamodel
{
    public partial class HotelMarkupDetail
    {
        public int Id { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public DateTime? CheckInDate { get; set; }
        public DateTime? CheckOutDate { get; set; }
        public DateTime? BookingFromDate { get; set; }
        public DateTime? BookingToDate { get; set; }
        public int? Rating { get; set; }
        public decimal? Markup { get; set; }
        public string SaAccountNo { get; set; }
        public DateTime SystemDate { get; set; }
        public bool? PerNight { get; set; }
        public bool? PerRoom { get; set; }
        public string MarkupType { get; set; }
    }
}
