using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb
{
    public partial class BannerDetail
    {
        public int Id { get; set; }
        public string WebsiteId { get; set; }
        public string Image { get; set; }
        public string AirlineText { get; set; }
        public string PageType { get; set; }
        public bool? IsPopup { get; set; }
        public string UrlAddress { get; set; }
    }
}
