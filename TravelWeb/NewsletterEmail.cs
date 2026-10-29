using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb
{
    public partial class NewsletterEmail
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string EmailId { get; set; }
        public string WebsiteId { get; set; }
        public DateTime Createdate { get; set; }
    }
}
