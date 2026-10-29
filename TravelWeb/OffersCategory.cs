using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb
{
    public partial class OffersCategory
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string CategoryImg { get; set; }
        public bool IsHtml { get; set; }
        public bool? IsCountrySpecial { get; set; }
        public bool IsFlightSpecial { get; set; }
        public bool IsHotelSpecial { get; set; }
        public bool? IsFeatured { get; set; }
        public string WebsiteId { get; set; }
        public bool? IsActive { get; set; }
        public DateTime SystemDate { get; set; }
    }
}
