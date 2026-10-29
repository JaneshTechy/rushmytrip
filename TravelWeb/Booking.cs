using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb
{
    public partial class Booking
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string EmailId { get; set; }
        public string Website { get; set; }
        public string SourceStation { get; set; }
        public string Destination { get; set; }
        public string DepartureDate { get; set; }
        public string ReturnDate { get; set; }
        public string CabinClass { get; set; }
        public int? Adult { get; set; }
        public int? Children { get; set; }
        public int? Infrants { get; set; }
        public string Airline { get; set; }
        public decimal? TotalFare { get; set; }
        public string RequestUrl { get; set; }
        public bool? AddCard { get; set; }
    }
}
