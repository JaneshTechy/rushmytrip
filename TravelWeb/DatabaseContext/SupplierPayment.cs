using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.DatabaseContext
{
    public partial class SupplierPayment
    {
        public int SupplierPaymentId { get; set; }
        public int SupplierId { get; set; }
        public long BookingId { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DatePaid { get; set; }
        public decimal Amount { get; set; }
        public int? UserId { get; set; }
        public string SupplierReference { get; set; }
        public string Remarks { get; set; }
        public bool DelInd { get; set; }

        public virtual FlightBooking Booking { get; set; }
        public virtual Supplier Supplier { get; set; }
        public virtual User User { get; set; }
    }
}
