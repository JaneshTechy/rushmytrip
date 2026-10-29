using System;

namespace TravelWeb.Models
{
    public class MultiselectFlight
    {
        public Airport From { get; set; }
        public Airport To { get; set; }
        public DateTime DepDate { get; set; }
        public DateTime RetDate { get; set; }
    }
}
