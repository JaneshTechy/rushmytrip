using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb
{
    public partial class Cart
    {
        public int Id { get; set; }
        public string Service { get; set; }
        public string Code { get; set; }
        public decimal? Amount { get; set; }
        public bool? IsCart { get; set; }
        public string WebsiteId { get; set; }
    }
}
