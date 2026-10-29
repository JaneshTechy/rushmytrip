using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TravelWeb.Models
{
    /// <summary>
    /// Separate multicity models (async one-way per leg). Does not alter existing FlightsSearch flow.
    /// </summary>
    public class MulticityLegRequest
    {
        [JsonProperty("from")]
        public string From { get; set; }
        [JsonProperty("to")]
        public string To { get; set; }
        [JsonProperty("depDate")]
        public string DepDate { get; set; }
    }

    public class MulticitySearchRequest
    {
        [JsonProperty("legs")]
        public List<MulticityLegRequest> Legs { get; set; } = new List<MulticityLegRequest>();
        [JsonProperty("adult")]
        public int Adult { get; set; } = 1;
        [JsonProperty("child")]
        public int Child { get; set; }
        [JsonProperty("infant")]
        public int Infant { get; set; }
        [JsonProperty("cabin")]
        public string Cabin { get; set; } = "Y";
        [JsonProperty("currency")]
        public string Currency { get; set; } = "USD";
        [JsonProperty("utmSource")]
        public string UtmSource { get; set; } = "HOME";
    }

    public class MulticityCombinedItinerary
    {
        public string CombinedId { get; set; }
        public string SearchId { get; set; }
        public double TotalCost { get; set; }
        public string Currency { get; set; }
        public List<Flights> Legs { get; set; } = new List<Flights>();
        public List<string> LegFlightIds { get; set; } = new List<string>();
        public List<string> LegRoutes { get; set; } = new List<string>();
    }

    public class MulticityCombinedResult
    {
        public string SearchId { get; set; }
        public string Error { get; set; }
        public int MinPrice { get; set; }
        public int MaxPrice { get; set; }
        public MulticitySearchRequest Request { get; set; }
        public List<MulticityCombinedItinerary> Itineraries { get; set; } = new List<MulticityCombinedItinerary>();
        public List<FlightsResult> LegResults { get; set; } = new List<FlightsResult>();
    }
}
