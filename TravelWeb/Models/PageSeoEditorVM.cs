using System.Collections.Generic;
using TravelWeb.Datamodel;

namespace TravelWeb.Models
{
    public class PageSeoEditorVM
    {
        public PageModel Page { get; set; }

        public SeoEntities Seo { get; set; }
        public List<WesbiteVM> Websites { get; set; }
        public List<FaqVM> FAQs { get; set; } = new List<FaqVM>();


    }
}
