using System.Collections.Generic;
using Newtonsoft.Json;

namespace TravelWeb.Models
{
    public class VerifierApiResponse
    {
        [JsonProperty("meta")]
        public VerifierMeta meta { get; set; }

        [JsonProperty("data")]
        public List<ApiFlightData> data { get; set; }
    }

    public class VerifierMeta
    {
        [JsonProperty("count")]
        public int count { get; set; }

        [JsonProperty("source")]
        public string source { get; set; }

        [JsonProperty("outFn")]
        public string outFn { get; set; }

        [JsonProperty("inFn")]
        public string inFn { get; set; }

        [JsonProperty("offerId")]
        public string offerId { get; set; }

        [JsonProperty("mismatch")]
        public bool mismatch { get; set; }

        [JsonProperty("mismatchKind")]
        public string mismatchKind { get; set; }

        [JsonProperty("tolerance")]
        public double tolerance { get; set; }

        [JsonProperty("offered")]
        public VerifierMoney offered { get; set; }

        [JsonProperty("live")]
        public VerifierLive live { get; set; }

        [JsonProperty("bookingOptions")]
        public List<VerifierBookingOption> bookingOptions { get; set; }

        [JsonProperty("ResponseTTL")]
        public string ResponseTTL { get; set; }

        [JsonProperty("org")]
        public string org { get; set; }

        [JsonProperty("dest")]
        public string dest { get; set; }

        [JsonProperty("depDt")]
        public string depDt { get; set; }

        [JsonProperty("retDt")]
        public string retDt { get; set; }

        [JsonProperty("currency")]
        public string currency { get; set; }
    }

    public class VerifierMoney
    {
        [JsonProperty("currency")]
        public string currency { get; set; }

        [JsonProperty("grandTotal")]
        public string grandTotal { get; set; }
    }

    public class VerifierLive
    {
        [JsonProperty("listPrice")]
        public VerifierMoney listPrice { get; set; }

        [JsonProperty("delta")]
        public double delta { get; set; }

        [JsonProperty("available")]
        public bool available { get; set; }
    }

    public class VerifierBookingOption
    {
        [JsonProperty("vendor")]
        public string vendor { get; set; }

        [JsonProperty("vendorCode")]
        public string vendorCode { get; set; }

        [JsonProperty("airlineDirect")]
        public bool airlineDirect { get; set; }

        [JsonProperty("fare")]
        public string fare { get; set; }

        [JsonProperty("fare_type")]
        public string fare_type { get; set; }

        [JsonProperty("price")]
        public double price { get; set; }

        [JsonProperty("currency")]
        public string currency { get; set; }

        [JsonProperty("airlineCode")]
        public string airlineCode { get; set; }

        [JsonProperty("features")]
        public List<VerifierFeature> features { get; set; }

        [JsonProperty("featureText")]
        public List<string> featureText { get; set; }

        /// <summary>Display title: fare → fare_type → vendor.</summary>
        [JsonIgnore]
        public string DisplayName
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(fare)) return fare;
                if (!string.IsNullOrWhiteSpace(fare_type)) return fare_type;
                if (!string.IsNullOrWhiteSpace(vendor)) return vendor;
                if (!string.IsNullOrWhiteSpace(vendorCode)) return vendorCode;
                return "Fare option";
            }
        }
    }

    public class VerifierFeature
    {
        [JsonProperty("text")]
        public string text { get; set; }

        [JsonProperty("status")]
        public string status { get; set; }
    }

    public class VerifierPageState
    {
        public bool HasVerifier { get; set; }
        public bool ShowPriceChangePopup { get; set; }
        public bool PriceIncreased { get; set; }
        public bool Available { get; set; } = true;
        public string OfferId { get; set; }
        public string Currency { get; set; } = "USD";
        public decimal OfferedTotal { get; set; }
        public decimal LiveTotal { get; set; }
        public decimal DisplayTotal { get; set; }
        public decimal Delta { get; set; }
        public List<VerifierBookingOption> BookingOptions { get; set; } = new List<VerifierBookingOption>();
        public string Error { get; set; }
    }

    public class BookingOptionsPartialVM
    {
        public Flights Flight { get; set; }
        public List<VerifierBookingOption> Options { get; set; } = new List<VerifierBookingOption>();
        public string Currency { get; set; } = "USD";
    }
}
