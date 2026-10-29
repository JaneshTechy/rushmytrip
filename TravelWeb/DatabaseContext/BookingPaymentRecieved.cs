using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.DatabaseContext
{
    public partial class BookingPaymentRecieved
    {
        public long Id { get; set; }
        public long? BookingId { get; set; }
        public decimal? PaymentRecived { get; set; }
        public string PamentDate { get; set; }
        public string PaymentMode { get; set; }
    }
}
