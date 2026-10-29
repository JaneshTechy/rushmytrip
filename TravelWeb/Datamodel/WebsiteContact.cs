using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.Datamodel
{
    public partial class WebsiteContact
    {
        public int Id { get; set; }
        public string WebsiteId { get; set; }
        public string CabinClass { get; set; }
        public string ContactNo { get; set; }
    }
}
