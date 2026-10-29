using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.Datamodel
{
    public partial class TblHorizontalBanner
    {
        public int Id { get; set; }
        public string BannerImage { get; set; }
        public string Link { get; set; }
        public string WebsiteId { get; set; }
        public bool? IsNewWindow { get; set; }
        public bool? Isactive { get; set; }
    }
}
