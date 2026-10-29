using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.Datamodel
{
    public partial class OffersSpecial
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string Offer { get; set; }
        public decimal Price { get; set; }
        public string WebsiteId { get; set; }
        public bool? IsActive { get; set; }
        public DateTime SystemDate { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public DateTime? Fromdate { get; set; }
        public DateTime? Todate { get; set; }
    }
}
