using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using TravelWeb.Helper;
using TravelWeb.Models;

namespace TravelWeb.Services
{
    public class FlightVerifierService
    {
        private static readonly HttpClient _client;

        private readonly string _baseUrl;
        private readonly string _userId;
        private readonly string _password;

        static FlightVerifierService()
        {
            var handler = new HttpClientHandler
            {
                AutomaticDecompression =
                    System.Net.DecompressionMethods.GZip |
                    System.Net.DecompressionMethods.Deflate |
                    System.Net.DecompressionMethods.Brotli
            };
            _client = new HttpClient(handler) { Timeout = TimeSpan.FromSeconds(45) };
        }

        public FlightVerifierService(IConfiguration configuration)
        {
            _baseUrl = configuration["FlightApi:VerifierUrl"]
                       ?? "https://rushmyflight.wikiproject.in/verifier";
            _userId = configuration["FlightApi:UserId"] ?? "rushmy";
            _password = configuration["FlightApi:Password"] ?? "RUrM5Lf";
        }

        public async Task<VerifierApiResponse> VerifyAsync(string offerId, string searchId, string flightId)
        {
            if (string.IsNullOrWhiteSpace(offerId))
                return null;

            var url =
                $"{_baseUrl.TrimEnd('/')}?userid={Uri.EscapeDataString(_userId)}" +
                $"&password={Uri.EscapeDataString(_password)}" +
                $"&id={Uri.EscapeDataString(offerId)}";

            using var response = await _client.GetAsync(url);
            var raw = await response.Content.ReadAsStringAsync();

            VerifierApiResponse typed = ParseVerifierResponse(raw);

            if (!string.IsNullOrWhiteSpace(searchId))
            {
                var fileKey = "verifier_" + Sanitize(flightId ?? offerId);
                try
                {
                    var folder = AplicationHelper.GetSearchFolder(searchId);
                    AplicationHelper.WriteAllTextSafe(
                        System.IO.Path.Combine(folder, fileKey + "_raw.txt"),
                        raw ?? string.Empty);
                    if (typed != null)
                        AplicationHelper.Save(searchId, fileKey, typed);
                }
                catch (Exception ex)
                {
                    AplicationHelper.Log("Verifier save error: " + ex.Message);
                }
            }

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Verifier API {(int)response.StatusCode}: {Truncate(raw, 300)}");

            var optCount = typed?.meta?.bookingOptions?.Count ?? 0;
            AplicationHelper.Log($"Verifier ok offerId={offerId} bookingOptions={optCount}");
            return typed;
        }

        /// <summary>
        /// Deserialize verifier JSON and recover bookingOptions from raw token if needed.
        /// </summary>
        public static VerifierApiResponse ParseVerifierResponse(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
                return null;

            VerifierApiResponse typed = null;
            try
            {
                typed = JsonConvert.DeserializeObject<VerifierApiResponse>(raw,
                    new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore
                    });
            }
            catch (Exception ex)
            {
                AplicationHelper.Log("Verifier deserialize error: " + ex.Message);
            }

            try
            {
                var root = Newtonsoft.Json.Linq.JObject.Parse(raw);
                var optsToken = root["meta"]?["bookingOptions"] as Newtonsoft.Json.Linq.JArray;
                if (optsToken != null && optsToken.Count > 0)
                {
                    var fromToken = optsToken.ToObject<List<VerifierBookingOption>>()
                                    ?? new List<VerifierBookingOption>();
                    if (typed == null)
                        typed = new VerifierApiResponse { meta = new VerifierMeta() };
                    if (typed.meta == null)
                        typed.meta = new VerifierMeta();

                    if (typed.meta.bookingOptions == null || typed.meta.bookingOptions.Count == 0)
                        typed.meta.bookingOptions = fromToken;
                }
            }
            catch (Exception ex)
            {
                AplicationHelper.Log("Verifier bookingOptions parse error: " + ex.Message);
            }

            return typed;
        }

        public static string ResolveOfferId(Flights flight)
        {
            if (flight == null) return null;
            if (!string.IsNullOrWhiteSpace(flight.ApiId))
                return flight.ApiId.Trim();

            // Fallback reconstruct (best-effort) when older search payloads lack ApiId.
            try
            {
                var outbound = flight.Outbound?.FirstOrDefault();
                var inbound = flight.Inbound?.FirstOrDefault();
                if (outbound == null) return null;

                var org = outbound.FromAirport?.Code ?? "";
                var dest = outbound.ToAirport?.Code ?? "";
                var dep = outbound.DepartureDate != default
                    ? outbound.DepartureDate
                    : DateTime.Parse(outbound.DepDate);
                var depPart = dep.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
                var outFn = (outbound.Airline?.Code ?? "") + (outbound.FlightNo ?? "");
                var outTm = dep.ToString("HHmm", CultureInfo.InvariantCulture);

                string retPart = "00000000";
                string inFn = "0";
                string inTm = "0000";
                if (inbound != null)
                {
                    var ret = inbound.DepartureDate != default
                        ? inbound.DepartureDate
                        : DateTime.Parse(inbound.DepDate);
                    retPart = ret.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
                    inFn = (inbound.Airline?.Code ?? "") + (inbound.FlightNo ?? "");
                    inTm = ret.ToString("HHmm", CultureInfo.InvariantCulture);
                }

                var priceInt = (int)Math.Round(flight.TotalCost > 0
                    ? flight.TotalCost
                    : (flight.FlightFare?.GrandTotal ?? 0));
                var cur = (flight.Currency ?? "USD").ToUpperInvariant();
                return $"{org}{dest}-{depPart}-{retPart}-{outFn}-{outTm}-{inFn}-{inTm}-{priceInt}-0{cur}";
            }
            catch
            {
                return null;
            }
        }

        public static VerifierPageState BuildPageState(Flights flight, VerifierApiResponse verifier)
        {
            var options = NormalizeBookingOptions(verifier);

            var state = new VerifierPageState
            {
                HasVerifier = verifier?.meta != null,
                Currency = flight?.Currency ?? verifier?.meta?.currency ?? "USD",
                OfferId = verifier?.meta?.offerId ?? flight?.ApiId,
                BookingOptions = options,
                Available = verifier?.meta?.live?.available ?? true
            };

            if (verifier?.meta == null)
                return state;

            state.OfferedTotal = ParseMoney(verifier.meta.offered?.grandTotal);
            // Listing card uses TotalCost (marked-up). Prefer that for "previous" comparison.
            var listed = (decimal)(flight?.TotalCost ?? 0);
            if (listed <= 0)
                listed = (decimal)(flight?.FlightFare?.GrandTotal ?? 0);
            if (listed > 0)
                state.OfferedTotal = listed;

            var liveList = ParseMoney(verifier.meta.live?.listPrice?.grandTotal);
            if (liveList <= 0 && verifier.data?.Count > 0)
                liveList = ParseMoney(verifier.data[0]?.price?.grandTotal);

            state.LiveTotal = liveList > 0 ? liveList : state.OfferedTotal;
            state.Delta = (decimal)(verifier.meta.live?.delta ?? (double)(state.LiveTotal - state.OfferedTotal));
            var rawOffered = ParseMoney(verifier.meta.offered?.grandTotal);

            // Price increased (totals only — does not control popup)
            state.PriceIncreased = (verifier.meta.mismatch && liveList > rawOffered)
                                   || (liveList > listed && (liveList - listed) > (decimal)verifier.meta.tolerance);

            // Popup: ONLY when verifier meta.mismatch == true
            state.ShowPriceChangePopup = verifier.meta.mismatch == true;

            state.DisplayTotal = state.PriceIncreased ? state.LiveTotal : state.OfferedTotal;
            if (state.DisplayTotal <= 0)
                state.DisplayTotal = (decimal)(flight?.TotalCost ?? flight?.FlightFare?.GrandTotal ?? 0);

            return state;
        }

        /// <summary>
        /// Only airline-direct booking options (airlineDirect == true) are shown in Choose your ticket.
        /// </summary>
        public static List<VerifierBookingOption> NormalizeBookingOptions(VerifierApiResponse verifier)
        {
            return verifier?.meta?.bookingOptions?
                .Where(o => o != null && o.price > 0 && o.airlineDirect)
                .OrderBy(o => o.price)
                .ToList() ?? new List<VerifierBookingOption>();
        }

        public static void ApplyLiveOfferToFlight(Flights flight, VerifierApiResponse verifier)
        {
            if (flight?.FlightFare == null || verifier == null)
                return;

            var liveOffer = verifier.data?.FirstOrDefault();
            var liveTotal = ParseMoney(verifier.meta?.live?.listPrice?.grandTotal);
            if (liveTotal <= 0)
                liveTotal = ParseMoney(liveOffer?.price?.grandTotal);
            if (liveTotal <= 0)
                return;

            var ff = flight.FlightFare;

            // Keep listing markup dollars so details stays consistent with listing style.
            var keepAdultMarkup = ff.AdultMarkup;
            var keepChildMarkup = ff.ChildMarkup;
            var keepInfantMarkup = ff.InfantMarkup;

            var adult = liveOffer?.travelerPricings?.FirstOrDefault(x =>
                string.Equals(x.travelerType, "ADULT", StringComparison.OrdinalIgnoreCase));
            var child = liveOffer?.travelerPricings?.FirstOrDefault(x =>
                string.Equals(x.travelerType, "CHILD", StringComparison.OrdinalIgnoreCase));
            var infant = liveOffer?.travelerPricings?.FirstOrDefault(x =>
                string.Equals(x.travelerType, "INFANT", StringComparison.OrdinalIgnoreCase));

            if (adult?.price != null)
            {
                ff.AdultFare = ParseDouble(adult.price.@base);
                ff.AdultTax = ParseDouble(adult.price.fees);
            }
            else if (liveOffer?.price != null)
            {
                ff.AdultFare = ParseDouble(liveOffer.price.@base);
                ff.AdultTax = ParseDouble(liveOffer.price.fees);
            }

            if (child?.price != null)
            {
                ff.ChildFare = ParseDouble(child.price.@base);
                ff.ChildTax = ParseDouble(child.price.fees);
            }
            if (infant?.price != null)
            {
                ff.InfantFare = ParseDouble(infant.price.@base);
                ff.InfantTax = ParseDouble(infant.price.fees);
            }

            ff.AdultMarkup = keepAdultMarkup;
            ff.ChildMarkup = keepChildMarkup;
            ff.InfantMarkup = keepInfantMarkup;

            flight.TotalCost = Math.Round(ff.GrandTotal > 0 ? ff.GrandTotal : (double)liveTotal + keepAdultMarkup, 2);

            if (!string.IsNullOrWhiteSpace(liveOffer?.id))
                flight.ApiId = liveOffer.id;

            var cur = liveOffer?.price?.currency
                      ?? verifier.meta?.live?.listPrice?.currency
                      ?? verifier.meta?.currency
                      ?? flight.Currency;
            if (!string.IsNullOrWhiteSpace(cur)
                && !cur.Equals("MXN", StringComparison.OrdinalIgnoreCase))
                flight.Currency = cur;
            else
                flight.Currency = "USD";
        }

        /// <summary>
        /// Sets Choose-your-ticket option price, keeping listing markup + AtolCharge on top.
        /// Payable = option.price + markup + AtolCharge (Atol shown separately on details).
        /// </summary>
        public static void ApplyBookingOptionToFlight(Flights flight, VerifierBookingOption option)
        {
            if (flight?.FlightFare == null || option == null || option.price <= 0)
                return;

            var ff = flight.FlightFare;
            var keepAdultMarkup = ff.AdultMarkup;
            var keepChildMarkup = ff.ChildMarkup;
            var keepInfantMarkup = ff.InfantMarkup;
            var keepAtol = ff.AtolCharge;

            var paying = Math.Max(1, ff.Adult + Math.Max(0, ff.Child));
            var tripTotal = option.price;
            var perPaying = tripTotal / paying;

            // Option price becomes fare basis; markup & ATOL stay on top.
            ff.AdultTax = 0;
            ff.ChildTax = 0;
            ff.InfantTax = 0;
            ff.AdultFare = ff.Adult > 0 ? perPaying : 0;
            ff.ChildFare = ff.Child > 0 ? perPaying : 0;
            if (ff.Infant > 0)
                ff.InfantFare = 0;

            ff.AdultMarkup = keepAdultMarkup;
            ff.ChildMarkup = keepChildMarkup;
            ff.InfantMarkup = keepInfantMarkup;
            ff.AtolCharge = keepAtol;

            flight.TotalCost = Math.Round(ff.GrandTotal, 2);

            if (!string.IsNullOrWhiteSpace(option.currency)
                && !option.currency.Equals("MXN", StringComparison.OrdinalIgnoreCase))
                flight.Currency = option.currency;
            else
                flight.Currency = "USD";

            if (!string.IsNullOrWhiteSpace(option.fare) || !string.IsNullOrWhiteSpace(option.vendor))
                ff.MarkupRuleName = option.DisplayName;
        }

        public static void EnsureAtolCharge(Flights flight, double configuredAtol)
        {
            if (flight?.FlightFare == null) return;
            if (flight.FlightFare.AtolCharge <= 0 && configuredAtol > 0)
                flight.FlightFare.AtolCharge = configuredAtol;
        }

        public static double GetMarkupTripTotal(FlightsFare ff)
        {
            if (ff == null) return 0;
            return (ff.AdultMarkup * ff.Adult) + (ff.ChildMarkup * ff.Child) + (ff.InfantMarkup * ff.Infant);
        }

        public static double GetPayableTotal(Flights flight)
        {
            if (flight?.FlightFare == null) return flight?.TotalCost ?? 0;
            return Math.Round(flight.FlightFare.GrandTotal + flight.FlightFare.AtolCharge + flight.FlightFare.CardCharge, 2);
        }

        public class SelectedTicketState
        {
            public int? BookingOptionIndex { get; set; }
            public double Total { get; set; }
            public string Currency { get; set; }
            public string Label { get; set; }
            public DateTime SavedAt { get; set; } = DateTime.UtcNow;
        }

        public static void SaveSelectedTicket(string searchId, string flightId, SelectedTicketState state)
        {
            if (string.IsNullOrWhiteSpace(searchId) || string.IsNullOrWhiteSpace(flightId) || state == null)
                return;
            AplicationHelper.Save(searchId, "selected_ticket_" + Sanitize(flightId), state);
        }

        public static SelectedTicketState LoadSelectedTicket(string searchId, string flightId)
        {
            if (string.IsNullOrWhiteSpace(searchId) || string.IsNullOrWhiteSpace(flightId))
                return null;
            return AplicationHelper.Get<SelectedTicketState>(searchId, "selected_ticket_" + Sanitize(flightId));
        }

        public static VerifierApiResponse LoadVerifierFromStore(string searchId, string flightId, string offerId = null)
        {
            if (string.IsNullOrWhiteSpace(searchId))
                return null;

            var key = "verifier_" + Sanitize(flightId ?? offerId ?? "unknown");
            try
            {
                var folder = AplicationHelper.GetSearchFolder(searchId);
                var rawPath = System.IO.Path.Combine(folder, key + "_raw.txt");
                if (System.IO.File.Exists(rawPath))
                {
                    var raw = AplicationHelper.ReadAllTextSafe(rawPath);
                    var parsed = ParseVerifierResponse(raw);
                    if (parsed != null)
                        return parsed;
                }
            }
            catch (Exception ex)
            {
                AplicationHelper.Log("LoadVerifierFromStore raw error: " + ex.Message);
            }

            return AplicationHelper.Get<VerifierApiResponse>(searchId, key);
        }

        /// <summary>
        /// Returns cached verifier only if the raw file is newer than maxAge.
        /// </summary>
        public static VerifierApiResponse TryLoadFreshVerifier(
            string searchId, string flightId, string offerId, TimeSpan maxAge)
        {
            if (string.IsNullOrWhiteSpace(searchId))
                return null;

            try
            {
                var key = "verifier_" + Sanitize(flightId ?? offerId ?? "unknown");
                var folder = AplicationHelper.GetSearchFolder(searchId);
                var rawPath = System.IO.Path.Combine(folder, key + "_raw.txt");
                if (!System.IO.File.Exists(rawPath))
                    return null;

                var age = DateTime.UtcNow - System.IO.File.GetLastWriteTimeUtc(rawPath);
                if (age > maxAge)
                    return null;

                return LoadVerifierFromStore(searchId, flightId, offerId);
            }
            catch (Exception ex)
            {
                AplicationHelper.Log("TryLoadFreshVerifier error: " + ex.Message);
                return null;
            }
        }

        public class DetailsContext
        {
            public Flights Flight { get; set; }
            public FlightsSearch FlightsSearch { get; set; }
            public DateTime SavedAt { get; set; } = DateTime.UtcNow;
        }

        public static void SaveDetailsContext(string searchId, Flights flight, FlightsSearch search)
        {
            if (string.IsNullOrWhiteSpace(searchId) || flight == null)
                return;
            try
            {
                AplicationHelper.Save(searchId, "details_" + Sanitize(flight.FlightId),
                    new DetailsContext { Flight = flight, FlightsSearch = search, SavedAt = DateTime.UtcNow });
            }
            catch (Exception ex)
            {
                AplicationHelper.Log("SaveDetailsContext error: " + ex.Message);
            }
        }

        public static DetailsContext LoadDetailsContext(string searchId, string flightId)
        {
            if (string.IsNullOrWhiteSpace(searchId) || string.IsNullOrWhiteSpace(flightId))
                return null;
            try
            {
                return AplicationHelper.Get<DetailsContext>(searchId, "details_" + Sanitize(flightId));
            }
            catch (Exception ex)
            {
                AplicationHelper.Log("LoadDetailsContext error: " + ex.Message);
                return null;
            }
        }

        private static decimal ParseMoney(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return 0;
            return decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var d) ? d : 0;
        }

        private static double ParseDouble(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return 0;
            return double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var d) ? d : 0;
        }

        private static string Sanitize(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return "unknown";
            foreach (var c in System.IO.Path.GetInvalidFileNameChars())
                value = value.Replace(c, '_');
            return value.Length > 80 ? value.Substring(0, 80) : value;
        }

        private static string Truncate(string s, int max)
            => string.IsNullOrEmpty(s) || s.Length <= max ? s : s.Substring(0, max) + "...";
    }
}
