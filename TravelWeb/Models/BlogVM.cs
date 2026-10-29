using System.Collections.Generic;
using TravelWeb.Datamodel;

namespace TravelWeb.Models
{
    public class BlogVM
    {
        public BlogPost Blog { get; set; }

        public List<FaqVM> FAQs { get; set; } = new List<FaqVM>();
    }
}
