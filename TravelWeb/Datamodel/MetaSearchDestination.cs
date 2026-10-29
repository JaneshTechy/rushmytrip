using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.Datamodel
{
    public partial class MetaSearchDestination
    {
        public int Id { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public string CabinClass { get; set; }
        public DateTime SystemDate { get; set; }
    }
}
