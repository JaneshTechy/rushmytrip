using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.Datamodel
{
    public partial class Enquiry
    {
        public Enquiry()
        {
            Responses = new HashSet<Response>();
        }

        public long Id { get; set; }
        public long BuyerId { get; set; }
        public long ProductId { get; set; }
        public long? SupplierId { get; set; }
        public string Quantity { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public DateTime? CreatedAt { get; set; }

        public virtual ICollection<Response> Responses { get; set; }
    }
}
