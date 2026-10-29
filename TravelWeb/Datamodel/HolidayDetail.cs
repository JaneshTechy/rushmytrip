using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.Datamodel
{
    public partial class HolidayDetail
    {
        public int Id { get; set; }
        public string WebsiteId { get; set; }
        public string Image1 { get; set; }
        public string Image2 { get; set; }
        public string HolidayText { get; set; }
    }
}
