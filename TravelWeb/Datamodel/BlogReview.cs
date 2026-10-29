using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.Datamodel
{
    public partial class BlogReview
    {
        public int ReviewId { get; set; }
        public int BlogId { get; set; }
        public string ReviewerName { get; set; }
        public string ReviewerEmail { get; set; }
        public string ReviewerComment { get; set; }
        public int? ReviewerStars { get; set; }
        public string ReviewStatus { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
