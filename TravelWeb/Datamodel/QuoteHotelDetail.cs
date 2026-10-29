using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.Datamodel
{
    public partial class QuoteHotelDetail
    {
        public int QuoteHotelDetailId { get; set; }
        public int QuoteId { get; set; }
        public string QuoteHotelName { get; set; }
        public string QuoteHotelAddress { get; set; }
        public string QuoteOption { get; set; }
        public DateTime? CheckInDate { get; set; }
        public DateTime? CheckOutDate { get; set; }
        public string DestinationQuery { get; set; }
        public int? NoOfNights { get; set; }
        public int? NoOfRooms { get; set; }
        public int? NoOfAdults { get; set; }
        public int? NoOfChildren { get; set; }
        public string RoomCategory { get; set; }
        public string BordType { get; set; }
        public decimal? FareAmount { get; set; }
        public decimal? TaxAmount { get; set; }
        public decimal? AdditionalEarnings { get; set; }
        public string BookingQuoteId { get; set; }
        public string CancellationDetail { get; set; }
        public string Apiname { get; set; }
    }
}
