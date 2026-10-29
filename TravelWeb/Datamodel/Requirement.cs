using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.Datamodel
{
    public partial class Requirement
    {
        public int RequirementId { get; set; }
        public int? UserId { get; set; }
        public string ProductService { get; set; }
        public decimal? Quantity { get; set; }
        public string Unit { get; set; }
        public string SupplierPreference { get; set; }
        public string MobileNumber { get; set; }
        public bool Del { get; set; }
        public string CountryCode { get; set; }
        public DateTime DateCreated { get; set; }
        public bool? IsActive { get; set; }
    }
}
