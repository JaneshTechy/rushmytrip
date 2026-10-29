using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.Datamodel
{
    public partial class EquipmentAir
    {
        public int Id { get; set; }
        public string Iata { get; set; }
        public string Manufacturer { get; set; }
        public string Type { get; set; }
        public string Wake { get; set; }
    }
}
