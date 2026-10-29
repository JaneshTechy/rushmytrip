using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb
{
    public partial class MarkupGroup
    {
        public int Id { get; set; }
        public string MarkupDetailId { get; set; }
        public string WebsiteMaterId { get; set; }
        public string Title { get; set; }
        public DateTime? SystemDate { get; set; }
    }
}
