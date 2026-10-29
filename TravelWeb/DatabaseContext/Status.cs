using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.DatabaseContext
{
    public partial class Status
    {
        public Status()
        {
            BookingTransactions = new HashSet<BookingTransaction>();
            FlightBookings = new HashSet<FlightBooking>();
            RoleStatuses = new HashSet<RoleStatus>();
        }

        public int StatusId { get; set; }
        public string Status1 { get; set; }
        public bool Delind { get; set; }
        public bool? Visible { get; set; }

        public virtual ICollection<BookingTransaction> BookingTransactions { get; set; }
        public virtual ICollection<FlightBooking> FlightBookings { get; set; }
        public virtual ICollection<RoleStatus> RoleStatuses { get; set; }
    }
}
