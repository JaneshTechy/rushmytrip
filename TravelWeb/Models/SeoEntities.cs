using System;

namespace TravelWeb.Models
{
    public class SeoEntities
    {
        public int Id { get; set; }
        public int WebsiteId { get; set; }

        public string PageType { get; set; }
        public int? ReferenceId { get; set; }
        public string Slug { get; set; }
        public string MetaTitle { get; set; }
        public string MetaDescription { get; set; }
        public string MetaKeywords { get; set; }
        public string CanonicalUrl { get; set; }
        public string OgTitle { get; set; }
        public string OgDescription { get; set; }
        public string OgImageUrl { get; set; }
        public string OgType { get; set; }
        public string Robots { get; set; }
        public string SchemaJson { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool? Delind { get; set; }
        public string AltImageText { get; set; }
    }
}
