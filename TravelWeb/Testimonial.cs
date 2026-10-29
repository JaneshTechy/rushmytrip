using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb
{
    public partial class Testimonial
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CompanyName { get; set; }
        public string Designation { get; set; }
        public string Rating { get; set; }
        public string UserImage { get; set; }
        public string Message { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool? IsActive { get; set; }
        public string WebSiteId { get; set; }
    }
}
