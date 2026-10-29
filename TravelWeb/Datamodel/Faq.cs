using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.Datamodel
{
    public partial class Faq
    {
        public int FaqId { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public string WebsiteId { get; set; }
        public bool? IsActive { get; set; }
        public DateTime SystemDate { get; set; }
        public string SearchTags { get; set; }
    }
}
