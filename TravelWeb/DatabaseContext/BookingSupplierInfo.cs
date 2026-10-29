using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.DatabaseContext
{
    public partial class BookingSupplierInfo
    {
        public long Id { get; set; }
        public long? BookingId { get; set; }
        public string Type { get; set; }
        public int? SupplierAmt { get; set; }
        public string SupplerRefrence { get; set; }
        public string SupplierName { get; set; }
        public bool? IsfullPaid { get; set; }
        public decimal? FirstDepositPaidAmt { get; set; }
        public string FirstDepositPaidDate { get; set; }
        public decimal? SecondDepositPaidAmt { get; set; }
        public string SecondDepositDate { get; set; }
        public decimal? BalanceDueAmt { get; set; }
        public string BalanceDueDate { get; set; }
    }
}
