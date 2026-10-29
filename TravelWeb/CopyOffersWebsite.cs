using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb
{
    public partial class CopyOffersWebsite
    {
        public int Id { get; set; }
        public string CopyFromWebsiteId { get; set; }
        public string CopyToWebsiteId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
    }
}
