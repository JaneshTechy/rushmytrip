using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TravelWeb.Models
{
    public class GoogleSearchConsoleVM
    {
        public double TotalClicks { get; set; }
        public double TotalImpressions { get; set; }
        public double AverageCtr { get; set; }
        public double AveragePosition { get; set; }

        public List<SearchConsoleQueryVM> Queries { get; set; }
            = new List<SearchConsoleQueryVM>();

        public List<SearchConsolePageVM> Pages { get; set; }
            = new List<SearchConsolePageVM>();

        public List<SearchConsoleDateVM> Dates { get; set; }
            = new List<SearchConsoleDateVM>();

        public List<SearchConsoleCountryVM> Countries { get; set; }
            = new List<SearchConsoleCountryVM>();

        public List<SearchConsoleDeviceVM> Devices { get; set; }
            = new List<SearchConsoleDeviceVM>();
    }

    public class SearchConsoleQueryVM
    {
        public string Query { get; set; } = string.Empty;
        public double Clicks { get; set; }
        public double Impressions { get; set; }
        public double Ctr { get; set; }
        public double Position { get; set; }
    }

    public class SearchConsolePageVM
    {
        public string Page { get; set; } = string.Empty;
        public double Clicks { get; set; }
        public double Impressions { get; set; }
        public double Ctr { get; set; }
        public double Position { get; set; }
    }

    public class SearchConsoleDateVM
    {
        public string Date { get; set; } = string.Empty;
        public double Clicks { get; set; }
        public double Impressions { get; set; }
        public double Ctr { get; set; }
        public double Position { get; set; }
    }

    public class SearchConsoleCountryVM
    {
        public string Country { get; set; } = string.Empty;
        public double Clicks { get; set; }
        public double Impressions { get; set; }
        public double Ctr { get; set; }
        public double Position { get; set; }
    }

    public class SearchConsoleDeviceVM
    {
        public string Device { get; set; } = string.Empty;
        public double Clicks { get; set; }
        public double Impressions { get; set; }
        public double Ctr { get; set; }
        public double Position { get; set; }
    }

    public class SearchConsoleApiResponse
    {
        [JsonPropertyName("rows")]
        public List<SearchConsoleApiRow> Rows { get; set; }
            = new List<SearchConsoleApiRow>();
    }

    public class SearchConsoleApiRow
    {
        [JsonPropertyName("keys")]
        public List<string> Keys { get; set; }
            = new List<string>();

        [JsonPropertyName("clicks")]
        public double Clicks { get; set; }

        [JsonPropertyName("impressions")]
        public double Impressions { get; set; }

        [JsonPropertyName("ctr")]
        public double Ctr { get; set; }

        [JsonPropertyName("position")]
        public double Position { get; set; }
    }
}
