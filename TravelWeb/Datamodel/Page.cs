using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.Datamodel
{
    public partial class Page
    {
        public int Id { get; set; }
        public int WebsiteId { get; set; }
        public string PageKey { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool? Delind { get; set; }
        public string ViewName { get; set; }
        public string FeatureImageUrl { get; set; }
        public string AltImageText { get; set; }
    }
}
