using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.Datamodel
{
    public partial class CouponMaster
    {
        public int Id { get; set; }
        public decimal? MinValue { get; set; }
        public string CouponCode { get; set; }
        public decimal? CouponValue { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? ActiveFromDate { get; set; }
        public DateTime? ActiveTillDate { get; set; }
        public int? PerUserLimit { get; set; }
        public string WebsiteId { get; set; }
        public DateTime? SystemDate { get; set; }
        public string CreatedBy { get; set; }
    }
}
