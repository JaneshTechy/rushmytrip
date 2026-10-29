using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace TravelWeb.Models
{
    public class PageModel
    {
        public string Title { get; set; }
        public int Id { get; set; }
        public int WebsiteId { get; set; }
        public bool IsActive { get; set; }
        public bool Delind { get; set; }

        public string Content { get; set; }
        public string PageKey { get; set; }
        public string Slug { get; set; }
        public string ViewName { get; set; }
        public string FeatureImageUrl { get; set; }
        public string AltImageText { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public IFormFile FeatureImageFile { get; set; }
        public List<FaqVM> FAQs { get; set; } = new List<FaqVM>();



    }
}
