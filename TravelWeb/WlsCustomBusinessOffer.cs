using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb
{
    public partial class WlsCustomBusinessOffer
    {
        public int Id { get; set; }
        public string DepartureFrom { get; set; }
        public string Destination { get; set; }
        public string AirlineCode { get; set; }
        public string CabinClassCode { get; set; }
        public decimal? Fare { get; set; }
        public bool? IsActive { get; set; }
        public string SaAccountNo { get; set; }
        public DateTime SystemDate { get; set; }
    }
}
