using System.Collections.Generic;
using System;
using System.Web.Mvc;

namespace TravelWeb.Models
{
    public class FlightFilterVM
    {
        [AllowHtml]  
        public string ApiJson { get; set; }

        public string Airline { get; set; }
        public int? Stops { get; set; }

        public TimeSpan? DepartFrom { get; set; }
        public TimeSpan? DepartTo { get; set; }
        public TimeSpan? ArriveFrom { get; set; }
        public TimeSpan? ArriveTo { get; set; }

        public List<ApiFlightData> Results { get; set; }

        public FlightFilterVM()
        {
            Results = new List<ApiFlightData>();
        }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 50;
    }
}
