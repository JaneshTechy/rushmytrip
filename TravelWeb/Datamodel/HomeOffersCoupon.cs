using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.Datamodel
{
    public partial class HomeOffersCoupon
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public double? Value { get; set; }
        public string ValueType { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool? IsActive { get; set; }
        public string WebsiteId { get; set; }
        public string ImageUrl { get; set; }
        public string DestinationCode { get; set; }
        public string Title { get; set; }
    }
}
