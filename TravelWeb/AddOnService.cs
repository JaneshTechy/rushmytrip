using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb
{
    public partial class AddOnService
    {
        public int Id { get; set; }
        public string Service { get; set; }
        public bool? Status { get; set; }
        public string WebsiteId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? IsActive { get; set; }
    }
}
