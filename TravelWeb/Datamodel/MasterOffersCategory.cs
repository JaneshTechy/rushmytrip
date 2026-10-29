using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.Datamodel
{
    public partial class MasterOffersCategory
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? SystemDate { get; set; }
        public string WebsiteId { get; set; }
        public bool? IsFeatured { get; set; }
        public string ImageName { get; set; }
        public string Description { get; set; }
        public string CategoryCode { get; set; }
    }
}
