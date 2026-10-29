using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.DatabaseContext
{
    public partial class BookingTransaction
    {
        public int TransactionId { get; set; }
        public int? UserId { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public long? BookingId { get; set; }
        public int? StatusId { get; set; }
        public int? AssignAgentId { get; set; }
        public string Comment { get; set; }

        public virtual User AssignAgent { get; set; }
        public virtual FlightBooking Booking { get; set; }
        public virtual Status Status { get; set; }
        public virtual User User { get; set; }
    }
}
