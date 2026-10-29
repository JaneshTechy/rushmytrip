using System.Collections.Generic;
using TravelWeb.Datamodel;

namespace TravelWeb.Models
{
    public class SeoPagesVM
    {
        public int? SelectedWebsiteId { get; set; }

        public List<WebsiteDropdownVM> Websites { get; set; }

        public List<SeoPageRowVM> Pages { get; set; }
    }
    public class WebsiteDropdownVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class SeoPageRowVM
    {
        public int Id { get; set; }
        public string PageKey { get; set; }
        public string Title { get; set; }
        public string ViewName { get; set; }
        public bool IsActive { get; set; }
        public string Slug { get; set; }
    }
}
