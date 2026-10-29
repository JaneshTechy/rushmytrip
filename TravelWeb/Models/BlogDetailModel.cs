using System.Collections.Generic;
using TravelWeb.Datamodel;

namespace TravelWeb.Models
{
    public class BlogDetailModel
    {
        public BlogPost blog { get; set; }

        public List<BlogPost> RecentPosts { get; set; }
        public List<FaqMaster> FAQs { get; set; } = new List<FaqMaster>();


    }

}
