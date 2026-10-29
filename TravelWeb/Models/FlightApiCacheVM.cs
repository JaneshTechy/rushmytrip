using System;

namespace TravelWeb.Models
{
    public class FlightApiCacheVM
    {
        public string FileName { get; set; }
        public string Route { get; set; }
        public string JourneyType { get; set; }
        public DateTime CreatedOn { get; set; }
        public string FullPath { get; set; }
        public string TravelDates { get; set; }
        public int Pax { get; set; }
        public DateTime SearchTime
        {
            get; set;
        }
    }
}
