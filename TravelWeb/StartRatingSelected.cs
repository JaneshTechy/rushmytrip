using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb
{
    public partial class StartRatingSelected
    {
        public int Id { get; set; }
        public int? StarRating { get; set; }
        public string Destination { get; set; }
        public string WebsiteId { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? SystemDate { get; set; }
    }
}
