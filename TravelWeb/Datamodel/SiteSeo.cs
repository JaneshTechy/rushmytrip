using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.Datamodel
{
    public partial class SiteSeo
    {
        public int Id { get; set; }
        public string PageName { get; set; }
        public string PageTitle { get; set; }
        public string MetaKeyword { get; set; }
        public string MetaDescription { get; set; }
        public string WebsiteId { get; set; }
        public DateTime SystemDate { get; set; }
    }
}
