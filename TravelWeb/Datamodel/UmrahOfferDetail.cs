using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.Datamodel
{
    public partial class UmrahOfferDetail
    {
        public int Id { get; set; }
        public string Heading { get; set; }
        public string Duration { get; set; }
        public string Destination { get; set; }
        public string Image { get; set; }
        public string WebsiteId { get; set; }
    }
}
