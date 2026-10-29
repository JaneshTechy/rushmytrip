using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb
{
    public partial class PopularDestination
    {
        public int Id { get; set; }
        public string DestinationName { get; set; }
        public string DestinationCode { get; set; }
        public string Description { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? SystemDate { get; set; }
        public string PageTitle { get; set; }
        public string Keyword { get; set; }
        public string MetaDescription { get; set; }
        public string DestinationDesc { get; set; }
    }
}
