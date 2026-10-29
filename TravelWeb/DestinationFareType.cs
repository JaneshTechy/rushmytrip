using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb
{
    public partial class DestinationFareType
    {
        public int Id { get; set; }
        public string DestinationCode { get; set; }
        public string FareType { get; set; }
        public bool? IsActive { get; set; }
        public DateTime SystemDate { get; set; }
    }
}
