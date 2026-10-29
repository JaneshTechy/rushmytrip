using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.Datamodel
{
    public partial class Response
    {
        public long Id { get; set; }
        public long EnquiryId { get; set; }
        public long SupplierId { get; set; }
        public string Message { get; set; }
        public decimal? Price { get; set; }
        public string Status { get; set; }
        public DateTime? CreatedAt { get; set; }

        public virtual Enquiry Enquiry { get; set; }
    }
}
