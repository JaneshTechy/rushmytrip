using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.Datamodel
{
    public partial class BlogPost
    {
        public long BlogPostId { get; set; }
        public string Title { get; set; }
        public string SubHeading { get; set; }
        public string Content { get; set; }
        public string FeatureImageUrl { get; set; }
        public string MetaKeywords { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string EditedBy { get; set; }
        public DateTime? EditedDate { get; set; }
        public bool IsDeleted { get; set; }
        public string MetaDescription { get; set; }
        public string MetaTitle { get; set; }
        public string AdditionalImageUrls { get; set; }
        public string Slug { get; set; }
        public int WebsiteId { get; set; }
        public string AltImageText { get; set; }
        public string ContentType { get; set; }
        public bool Trending { get; set; }
        public string SocialProfileUrl { get; set; }
    }
}
