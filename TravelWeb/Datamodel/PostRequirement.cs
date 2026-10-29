using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.Datamodel
{
    public partial class PostRequirement
    {
        public long RequirementId { get; set; }
        public string ProductService { get; set; }
        public string Quantity { get; set; }
        public string Measurement { get; set; }
        public string SupplierPreference { get; set; }
        public string MobileNumber { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public bool? DelInd { get; set; }
    }
}
