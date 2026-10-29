using System.Collections.Generic;
using TravelWeb.Datamodel;

namespace TravelWeb.Models
{
    public class BlogListVM
    {
        public List<Website> Websites { get; set; }
        public string Domain { get; set; }

        public int? SelectedWebsiteId { get; set; }

        public List<BlogPost> Blogs { get; set; }
    }
}
