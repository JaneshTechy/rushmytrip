using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TravelWeb.Models;

namespace TravelWeb.Utilities
{
    /// <summary>
    /// Separate multicity helper: runs one-way searches in parallel, then combines legs into itineraries.
    /// Does not modify Common.SearchFlight or ClearAllSessions.
    /// </summary>
    public static class MulticityAsyncSearchHelper
    {
        //private const string BaseUrl = "https://rushmyflight.wikiproject.in/";
        private const string BaseUrl = "https://rushmyflight.wikiproject.in/nv3";   
        private const int TopPerLeg = 8;
        private const int MaxCombinations = 40;

        public static async Task<MulticityCombinedResult> SearchAsync(MulticitySearchRequest request)
        {
            var result = new MulticityCombinedResult
            {
                SearchId = Guid.NewGuid().ToString(),
                Request = request
            };

            if (request?.Legs == null || request.Legs.Count < 1)
            {
                result.Error = "Multicity requires at least 1 leg.";
                return result;
            }

            if (request.Legs.Count > 4)
            {
                request.Legs = request.Legs.Take(4).ToList();
            }

            var cabin = MapCabin(request.Cabin);
            var currency = string.IsNullOrWhiteSpace(request.Currency) ? "USD" : request.Currency;

            var tasks = request.Legs.Select((leg, index) =>
                SearchOneWayAsync(leg, request, cabin, currency, result.SearchId, index + 1)
            ).ToArray();

            FlightsResult[] legResults;
            try
            {
                legResults = await Task.WhenAll(tasks);
            }
            catch (Exception ex)
            {
                result.Error = ex.Message;
                return result;
            }

            result.LegResults = legResults.ToList();

            if (legResults.Any(r => r == null || r.FlightsList == null || !r.FlightsList.Any()))
            {
                var failed = legResults
                    .Select((r, i) => new { r, i })
                    .Where(x => x.r == null || x.r.FlightsList == null || !x.r.FlightsList.Any())
                    .Select(x => $"Leg {x.i + 1}")
                    .ToList();
                result.Error = "No flights found for: " + string.Join(", ", failed);
                return result;
            }

            result.Itineraries = CombineLegs(legResults, result.SearchId, currency);

            if (result.Itineraries.Any())
            {
                result.MinPrice = (int)Math.Floor(result.Itineraries.Min(x => x.TotalCost));
                result.MaxPrice = (int)Math.Ceiling(result.Itineraries.Max(x => x.TotalCost));
            }

            return result;
        }

        private static async Task<FlightsResult> SearchOneWayAsync(
            MulticityLegRequest leg,
            MulticitySearchRequest request,
            FlightClass cabin,
            string currency,
            string parentSearchId,
            int legNumber)
        {
            var fromCode = ExtractAirportCode(leg.From);
            var toCode = ExtractAirportCode(leg.To);
            var depDate = ParseDepDate(leg.DepDate);

            var flightSearch = new FlightsSearch
            {
                SearchId = $"{parentSearchId}-L{legNumber}",
                From = Common.GetAirport(fromCode),
                To = Common.GetAirport(toCode),
                DepDate = depDate,
                Adult = request.Adult,
                Child = request.Child,
                Infant = request.Infant,
                FlightWay = FlightWay.OneWay,
                FlightClass = cabin,
                Currency = currency,
                SiteCode = request.UtmSource ?? "HOME",
                SourceMedia = request.UtmSource ?? "HOME",
                IsDeepLink = true,
                IsDirect = false,
                IsFlexi = true
            };

            return await FetchOneWayAsync(flightSearch);
        }

        /// <summary>
        /// One-way API call (same endpoint as Common.SearchFlight) without ClearAllSessions.
        /// </summary>
        private static async Task<FlightsResult> FetchOneWayAsync(FlightsSearch flightSearch)
        {
            var flightsResult = new FlightsResult
            {
                FlightsList = new List<Flights>(),
                FlightsSearch = flightSearch,
                SearchId = flightSearch.SearchId
            };

            string classCode = "Y";
            switch (flightSearch.FlightClass)
            {
                case FlightClass.First: classCode = "F"; break;
                case FlightClass.Business: classCode = "B"; break;
                case FlightClass.EconomyPremium: classCode = "P"; break;
                default: classCode = "Y"; break;
            }

            string query = $"?currency={((flightSearch.Currency ?? "USD").Equals("MXN", StringComparison.OrdinalIgnoreCase) ? "USD" : (flightSearch.Currency ?? "USD"))}" +
                   $"&JType=oneway" +
                   $"&org={flightSearch.From?.Code}" +
                   $"&dest={flightSearch.To?.Code}" +
                   $"&depDt={flightSearch.DepDate:MMM-dd-yyyy}" +
                   $"&adt={flightSearch.Adult}" +
                   $"&chd={flightSearch.Child}" +
                   $"&inf={flightSearch.Infant}" +
                   $"&ct={classCode}" +
                   $"&userid=rushmy" +
                   $"&password=RUrM5Lf" +
                   $"&metaId=2020" +
                   $"&website=rushmyflight" +
                   $"&limit=50";

            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(90);
                var response = await client.GetAsync(BaseUrl + query);
                if (!response.IsSuccessStatusCode)
                {
                    flightsResult.Error = $"API Error: {response.StatusCode}";
                    return flightsResult;
                }

                string json = await response.Content.ReadAsStringAsync();
                var apiResult = JsonConvert.DeserializeObject<ApiResponse>(json);
                if (apiResult?.data == null)
                {
                    return flightsResult;
                }

                int flightiddefaultcount = 1;
                foreach (var item in apiResult.data)
                {
                    double baseFare = SafeDouble(item.price?.@base);
                    double fees = SafeDouble(item.price?.fees);
                    double grandTotal = SafeDouble(item.price?.grandTotal);

                    var flight = new Flights
                    {
                        ApiId = item.id.ToString(),
                        FlightId = Guid.NewGuid().ToString(),
                        SearchId = flightSearch.SearchId,
                        SessionId = flightiddefaultcount.ToString(),
                        Currency = item.price?.currency ?? flightSearch.Currency ?? "USD",
                        TotalCost = grandTotal,
                        Airline = new Airline
                        {
                            Code = item.validatingAirlineCodes?.FirstOrDefault() ?? "",
                            Name = item.itineraries?.FirstOrDefault()?.segments?.FirstOrDefault()?.airline ?? "Unknown Airline"
                        },
                        Outbound = new List<FlightsSegment>(),
                        Inbound = new List<FlightsSegment>(),
                        FlightFare = new FlightsFare(),
                        Stops = 0
                    };
                    flightiddefaultcount++;

                    int totalStops = 0;
                    int totalMinutes = 0;

                    if (item.itineraries != null && item.itineraries.Count > 0)
                    {
                        var outbound = item.itineraries[0];
                        if (outbound.segments != null)
                        {
                            foreach (var seg in outbound.segments)
                            {
                                flight.Outbound.Add(new FlightsSegment
                                {
                                    FromAirport = new Airport { Code = seg.departure?.iataCode ?? "", CityName = seg.departure?.iataCode },
                                    ToAirport = new Airport { Code = seg.arrival?.iataCode ?? "", CityName = seg.arrival?.iataCode },
                                    DepartureDate = SafeDate(seg.departure?.at),
                                    ArrivalDate = SafeDate(seg.arrival?.at),
                                    Airline = new Airline { Code = seg.carrierCode ?? "", Name = seg.airline ?? "" },
                                    FlightNo = seg.number ?? "",
                                    IsReturn = false,
                                    OperatingAirline = new Airline { Code = seg.operating?.carrierCode },
                                    ElapsedTime = seg.duration ?? "",
                                    Distance = "",
                                    ClassOfService = item.classofService,
                                    CabinClass = item.travelerPricings?.SelectMany(x => x.fareDetailsBySegment ?? Enumerable.Empty<FareDetailsBySegment>())
                                        .Select(x => x.@class).FirstOrDefault() ?? "",
                                    Availability = item.numberOfBookableSeats.ToString(),
                                    BookingCode = item.jsonId ?? "",
                                    DepDate = seg.departure?.at ?? "",
                                    ArrDate = seg.arrival?.at ?? "",
                                    TotalTime = item.itineraries[0].duration,
                                    FromTerminal = seg.departure?.terminal ?? "",
                                    ToTerminal = seg.arrival?.terminal ?? ""
                                });

                                totalStops += seg.numberOfStops;
                                totalMinutes += ConvertDurationToMinutes(seg.duration);
                            }
                        }
                    }

                    flight.Stops = Math.Max(0, flight.Outbound.Count - 1);
                    flight.TotalTime = totalMinutes;

                    double adultBase = 0, adultTax = 0, childBase = 0, childTax = 0, infantBase = 0, infantTax = 0;
                    if (item.travelerPricings != null)
                    {
                        var adult = item.travelerPricings.FirstOrDefault(x => x.travelerType == "ADULT");
                        if (adult != null)
                        {
                            adultBase = SafeDouble(adult.price?.@base);
                            adultTax = SafeDouble(adult.price?.fees);
                        }
                        var child = item.travelerPricings.FirstOrDefault(x => x.travelerType == "CHILD");
                        if (child != null)
                        {
                            childBase = SafeDouble(child.price?.@base);
                            childTax = SafeDouble(child.price?.fees);
                        }
                        var infant = item.travelerPricings.FirstOrDefault(x => x.travelerType == "INFANT");
                        if (infant != null)
                        {
                            infantBase = SafeDouble(infant.price?.@base);
                            infantTax = SafeDouble(infant.price?.fees);
                        }
                    }

                    flight.FlightFare = new FlightsFare
                    {
                        AdultFare = adultBase,
                        AdultTax = adultTax,
                        ChildFare = childBase,
                        ChildTax = childTax,
                        InfantFare = infantBase,
                        InfantTax = infantTax,
                        Adult = flightSearch.Adult,
                        Child = flightSearch.Child,
                        Infant = flightSearch.Infant,
                        MarkupType = "Fixed",
                        MarkupValue = "0",
                        AdultMarkup = 0
                    };

                    typeof(FlightsFare).GetProperty("AvgCost")?.SetValue(flight.FlightFare, baseFare + fees);

                    if (flight.Outbound.Any())
                    {
                        flight.Airline.Code = flight.Outbound[0].Airline.Code;
                        flightsResult.OutBoundDep = flight.Outbound.First().FromAirport.Code;
                        flightsResult.OutBoundArr = flight.Outbound.Last().ToAirport.Code;
                    }

                    flightsResult.FlightsList.Add(flight);
                }
            }

            if (flightsResult.FlightsList.Any())
            {
                flightsResult.MinPrice = (int)flightsResult.FlightsList.Min(f => f.TotalCost);
                flightsResult.MaxPrice = (int)flightsResult.FlightsList.Max(f => f.TotalCost);

                try
                {
                    MarkupService.ApplyMarkupNew(flightsResult, flightSearch, "Rush", "Rush");
                }
                catch
                {
                    // Markup is optional for multicity; keep raw prices if markup fails.
                }
            }

            return flightsResult;
        }

        private static List<MulticityCombinedItinerary> CombineLegs(FlightsResult[] legResults, string searchId, string currency)
        {
            var tops = legResults
                .Select(r => r.FlightsList.OrderBy(f => f.TotalCost).Take(TopPerLeg).ToList())
                .ToList();

            var combinations = new List<MulticityCombinedItinerary>();

            void Recurse(int depth, List<Flights> path)
            {
                if (combinations.Count >= MaxCombinations) return;

                if (depth == tops.Count)
                {
                    combinations.Add(new MulticityCombinedItinerary
                    {
                        CombinedId = Guid.NewGuid().ToString(),
                        SearchId = searchId,
                        TotalCost = path.Sum(p => p.TotalCost),
                        Currency = currency,
                        Legs = path.ToList(),
                        LegFlightIds = path.Select(p => p.FlightId).ToList(),
                        LegRoutes = path.Select(p =>
                        {
                            var from = p.Outbound?.FirstOrDefault()?.FromAirport?.Code ?? "";
                            var to = p.Outbound?.LastOrDefault()?.ToAirport?.Code ?? "";
                            return $"{from} → {to}";
                        }).ToList()
                    });
                    return;
                }

                foreach (var flight in tops[depth])
                {
                    if (combinations.Count >= MaxCombinations) break;
                    path.Add(flight);
                    Recurse(depth + 1, path);
                    path.RemoveAt(path.Count - 1);
                }
            }

            Recurse(0, new List<Flights>());
            return combinations.OrderBy(c => c.TotalCost).ToList();
        }

        /// <summary>
        /// Builds a FlightsResult compatible with DetailsController (same as /result Select).
        /// All multicity legs are merged into one Flights row (Outbound = all segments in order).
        /// </summary>
        public static FlightsResult BuildBookingFlightsResult(MulticityCombinedItinerary itinerary, MulticitySearchRequest request)
        {
            if (itinerary == null || itinerary.Legs == null || itinerary.Legs.Count == 0)
            {
                return null;
            }

            var cabin = MapCabin(request?.Cabin);
            var adult = request?.Adult > 0 ? request.Adult : 1;
            var child = request?.Child ?? 0;
            var infant = request?.Infant ?? 0;
            var currency = string.IsNullOrWhiteSpace(itinerary.Currency)
                ? (request?.Currency ?? "USD")
                : itinerary.Currency;

            var allOutbound = new List<FlightsSegment>();
            var outboundList = new List<List<FlightsSegment>>();
            foreach (var leg in itinerary.Legs)
            {
                var segs = leg.Outbound ?? new List<FlightsSegment>();
                if (segs.Count > 0)
                {
                    allOutbound.AddRange(segs);
                    outboundList.Add(segs.ToList());
                }
            }

            if (allOutbound.Count == 0)
            {
                return null;
            }

            // Ensure ClassOfService + date fields for details page (same as normal search)
            foreach (var seg in allOutbound)
            {
                if (string.IsNullOrWhiteSpace(seg.ClassOfService))
                {
                    seg.ClassOfService = string.IsNullOrWhiteSpace(seg.BookingCode) ? "Y" : seg.BookingCode;
                }
                if (seg.DepartureDate == default && !string.IsNullOrEmpty(seg.DepDate)
                    && DateTime.TryParse(seg.DepDate, out var depParsed))
                {
                    seg.DepartureDate = depParsed;
                }
                if (seg.ArrivalDate == default && !string.IsNullOrEmpty(seg.ArrDate)
                    && DateTime.TryParse(seg.ArrDate, out var arrParsed))
                {
                    seg.ArrivalDate = arrParsed;
                }
                if (string.IsNullOrEmpty(seg.DepDate) && seg.DepartureDate != default)
                {
                    seg.DepDate = seg.DepartureDate.ToString("yyyy-MM-ddTHH:mm:ss");
                }
                if (string.IsNullOrEmpty(seg.ArrDate) && seg.ArrivalDate != default)
                {
                    seg.ArrDate = seg.ArrivalDate.ToString("yyyy-MM-ddTHH:mm:ss");
                }
            }

            double adultFare = 0, childFare = 0, infantFare = 0;
            double adultTax = 0, childTax = 0, infantTax = 0;
            double adultMarkup = 0, childMarkup = 0, infantMarkup = 0;
            string markupType = null;
            string markupValue = null;

            foreach (var leg in itinerary.Legs)
            {
                var ff = leg.FlightFare;
                if (ff == null) continue;
                adultFare += ff.AdultFare;
                childFare += ff.ChildFare;
                infantFare += ff.InfantFare;
                adultTax += ff.AdultTax;
                childTax += ff.ChildTax;
                infantTax += ff.InfantTax;
                adultMarkup += ff.AdultMarkup;
                childMarkup += ff.ChildMarkup;
                infantMarkup += ff.InfantMarkup;
                if (markupType == null && !string.IsNullOrWhiteSpace(ff.MarkupType))
                {
                    markupType = ff.MarkupType;
                    markupValue = ff.MarkupValue;
                }
            }

            // If API only stored total on legs, fall back to splitting total across adult fare
            if (adultFare <= 0 && itinerary.TotalCost > 0 && adult > 0)
            {
                adultFare = itinerary.TotalCost / adult;
            }

            var firstLeg = itinerary.Legs[0];
            var lastLeg = itinerary.Legs[itinerary.Legs.Count - 1];
            var firstSeg = allOutbound.First();
            var lastSeg = allOutbound.Last();

            var fromCode = firstSeg.FromAirport?.Code
                ?? ExtractAirportCode(request?.Legs?.FirstOrDefault()?.From);
            var toCode = lastSeg.ToAirport?.Code
                ?? ExtractAirportCode(request?.Legs?.LastOrDefault()?.To);

            var depDate = ParseDepDate(request?.Legs?.FirstOrDefault()?.DepDate);
            if (!string.IsNullOrEmpty(firstSeg.DepDate) && DateTime.TryParse(firstSeg.DepDate, out var segDep))
            {
                depDate = segDep;
            }

            DateTime retDate = DateTime.MinValue;
            var lastReqDate = request?.Legs?.LastOrDefault()?.DepDate;
            if (!string.IsNullOrWhiteSpace(lastReqDate))
            {
                retDate = ParseDepDate(lastReqDate);
            }

            var flightId = string.IsNullOrWhiteSpace(itinerary.CombinedId)
                ? Guid.NewGuid().ToString()
                : itinerary.CombinedId;

            var searchId = string.IsNullOrWhiteSpace(itinerary.SearchId)
                ? Guid.NewGuid().ToString()
                : itinerary.SearchId;

            var flight = new Flights
            {
                FlightId = flightId,
                SearchId = searchId,
                ApiId = firstLeg.ApiId,
                Provider = firstLeg.Provider,
                Currency = currency,
                TotalCost = itinerary.TotalCost,
                FlightClass = cabin,
                Airline = firstLeg.Airline ?? firstSeg.Airline,
                FareRules = firstLeg.FareRules,
                Outbound = allOutbound,
                Inbound = new List<FlightsSegment>(),
                OutboundList = outboundList,
                InboundList = new List<List<FlightsSegment>>(),
                Stops = Math.Max(0, allOutbound.Count - 1),
                FlightFare = new FlightsFare
                {
                    Adult = adult,
                    Child = child,
                    Infant = infant,
                    AdultFare = adultFare,
                    ChildFare = childFare,
                    InfantFare = infantFare,
                    AdultTax = adultTax,
                    ChildTax = childTax,
                    InfantTax = infantTax,
                    AdultMarkup = adultMarkup,
                    ChildMarkup = childMarkup,
                    InfantMarkup = infantMarkup,
                    MarkupType = markupType ?? "None",
                    MarkupValue = markupValue ?? "0"
                }
            };

            var flightsSearch = new FlightsSearch
            {
                SearchId = searchId,
                From = Common.GetAirport(fromCode) ?? firstSeg.FromAirport,
                To = Common.GetAirport(toCode) ?? lastSeg.ToAirport,
                DepDate = depDate,
                RetDate = retDate,
                Adult = adult,
                Child = child,
                Infant = infant,
                FlightWay = FlightWay.OneWay,
                FlightClass = cabin,
                Currency = currency,
                SourceMedia = request?.UtmSource ?? "HOME",
                IsDeepLink = false
            };

            return new FlightsResult
            {
                SearchId = searchId,
                Error = null,
                MinPrice = (int)Math.Floor(itinerary.TotalCost),
                MaxPrice = (int)Math.Ceiling(itinerary.TotalCost),
                FlightsSearch = flightsSearch,
                FlightsList = new List<Flights> { flight }
            };
        }

        private static FlightClass MapCabin(string cabin)
        {
            if (string.IsNullOrWhiteSpace(cabin)) return FlightClass.Economy;
            switch (cabin.Trim().ToUpperInvariant())
            {
                case "F":
                case "1": return FlightClass.First;
                case "B":
                case "2": return FlightClass.Business;
                case "P":
                case "3": return FlightClass.EconomyPremium;
                case "Y":
                case "4":
                default: return FlightClass.Economy;
            }
        }

        private static string ExtractAirportCode(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return "";
            var parts = value.Split('-');
            return parts[0].Trim().ToUpperInvariant();
        }

        private static DateTime ParseDepDate(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return DateTime.Today.AddDays(7);

            // mm-dd-yyyy from home datepicker
            if (DateTime.TryParseExact(value.Trim(), new[] { "MM-dd-yyyy", "yyyy-MM-dd", "MMM-dd-yyyy", "dd-MMM-yyyy" },
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out var dt))
            {
                return dt;
            }

            if (DateTime.TryParse(value, out dt)) return dt;
            return DateTime.Today.AddDays(7);
        }

        private static double SafeDouble(string value)
        {
            if (double.TryParse(value, out double result)) return result;
            return 0;
        }

        private static DateTime SafeDate(string dateString)
        {
            if (DateTime.TryParse(dateString, out DateTime date)) return date;
            return DateTime.MinValue;
        }

        private static int ConvertDurationToMinutes(string isoDuration)
        {
            if (string.IsNullOrEmpty(isoDuration)) return 0;
            int hours = 0, minutes = 0;
            var hMatch = Regex.Match(isoDuration, @"(\d+)H");
            var mMatch = Regex.Match(isoDuration, @"(\d+)M");
            if (hMatch.Success) hours = int.Parse(hMatch.Groups[1].Value);
            if (mMatch.Success) minutes = int.Parse(mMatch.Groups[1].Value);
            return (hours * 60) + minutes;
        }
    }
}
