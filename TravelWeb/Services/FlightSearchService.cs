using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Web;
using TravelWeb.Helper;
using TravelWeb.Models;
using static TravelWeb.Models.MetaResponseDto;

namespace TravelWeb.Services
{
    public class FlightSearchService
    {
        private static readonly HttpClient _client;

        //private const string BaseUrl = "https://rushmyflight.wikiproject.in/";
        private const string BaseUrl = "https://rushmyflight.wikiproject.in/nv3";
        private const string DefaultCurrency = "USD";
        private const string ApiUser = "rushmy";
        private const string ApiPass = "RUrM5Lf";
        private const string ApiMetaId = "2020";
        private const string ApiWebsite = "rushmyflight";
        static FlightSearchService()
        {
            var handler = new HttpClientHandler
            {
                AutomaticDecompression =
                    System.Net.DecompressionMethods.GZip |
                    System.Net.DecompressionMethods.Deflate |
                    System.Net.DecompressionMethods.Brotli
            };

            _client = new HttpClient(handler);
            _client.Timeout = TimeSpan.FromSeconds(60);
        }


        public async Task<FlightsResult> SearchFlightOptimizedAsyncV1(FlightsSearch flightSearch, decimal ExtraMarkup)
        {
            //AplicationHelper.Log("SearchFlightOptimizedAsyncV1 Start");
            
            string fareScrapper = "Rush_FS";
           
            var flightsResult = new FlightsResult
            {
                FlightsList = new List<Flights>(),
                FlightsSearch = flightSearch
            };

            var allowRules = AplicationHelper.LoadAllowRulesFromFile(fareScrapper);
            if (!RouteControlService.ApplyAllowAirlineRules(flightSearch, allowRules))
                return flightsResult;

            try
            {
                var cacheKey = AplicationHelper.BuildSearchKey(
                    flightSearch.From.Code,
                    flightSearch.To.Code,
                    flightSearch.DepDate,
                    flightSearch.FlightWay == FlightWay.RoundTrip
                        ? flightSearch.RetDate
                        : (DateTime?)null,
                    flightSearch.Adult,
                    flightSearch.Child,
                    flightSearch.Infant,
                    (int)flightSearch.FlightWay,
                    (int)flightSearch.FlightClass
                );

                var cachePath = AplicationHelper.GetApiCachePath(cacheKey);

                ApiResponse apiFlightsResult = null;

                if (AplicationHelper.IsApiCacheValid(cachePath))
                {
                    apiFlightsResult = AplicationHelper.LoadApiResponse(cachePath);

                    if (apiFlightsResult?.data == null || !apiFlightsResult.data.Any())
                        apiFlightsResult = null;
                }

                if (apiFlightsResult == null)
                {
                    //AplicationHelper.Log("Cache Miss - Calling External API");

                    string classCode = flightSearch.FlightClass switch
                    {
                        FlightClass.First => "F",
                        FlightClass.Business => "B",
                        FlightClass.EconomyPremium => "P",
                        _ => "Y"
                    };

                    string query =
         $"?currency={ResolveSearchCurrency(flightSearch.Currency)}" +
         $"&JType={(flightSearch.FlightWay == FlightWay.RoundTrip ? "roundtrip" : "oneway")}" +
         $"&org={flightSearch.From.Code}" +
         $"&dest={flightSearch.To.Code}" +
         $"&depDt={flightSearch.DepDate:MMM-dd-yyyy}" +
         (flightSearch.FlightWay == FlightWay.RoundTrip
             ? $"&retDt={flightSearch.RetDate:MMM-dd-yyyy}"
             : string.Empty) +
         $"&adt={flightSearch.Adult}" +
         $"&chd={flightSearch.Child}" +
         $"&inf={flightSearch.Infant}" +
         $"&ct={classCode}" +
         //$"&limit=1"+
         //$"&cheapest=5000" +
         $"&userid={ApiUser}&password={ApiPass}&metaId={ApiMetaId}&website={ApiWebsite}";

                    var url = BaseUrl + query;

                    using var request = new HttpRequestMessage(HttpMethod.Get, url);
                    request.Headers.AcceptEncoding.ParseAdd("gzip, deflate");

                    using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(12));
                    using var response = await _client.SendAsync(
                        request,
                        HttpCompletionOption.ResponseHeadersRead,
                        cts.Token
                    );

                    if (!response.IsSuccessStatusCode)
                    {
                        //AplicationHelper.Log("API Error: " + response.StatusCode);

                        flightsResult.Error =
                            $"API Error: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}";
                        return flightsResult;
                    }
                    var jsondd = await response.Content.ReadAsStringAsync();

                    await using var stream = await response.Content.ReadAsStreamAsync();

                    apiFlightsResult = await System.Text.Json.JsonSerializer.DeserializeAsync<ApiResponse>(
                        stream,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true,
                            MaxDepth = 64
                        });
                    //AplicationHelper.Log("Step 7: After Deserialize");
                    //AplicationHelper.Log("API Data Count: " + apiFlightsResult?.data?.Count);
                    await AplicationHelper.SaveApiResponse(cachePath, apiFlightsResult);
                }

                if (apiFlightsResult?.data == null || !apiFlightsResult.data.Any())
                {
                    //AplicationHelper.Log("No Flights Found");

                    flightsResult.Error = "No flights found.";
                    return flightsResult;
                }

                var flights = new List<Flights>();
                var airlineGroups = apiFlightsResult.data
    .Where(x => x.itineraries != null && x.itineraries.Any())
    .Select(x => new
    {
        Flight = x,
        Airline = x.itineraries[0].segments[0].carrierCode,
        Duration = ConvertDurationToMinutes(x.itineraries[0].duration),
        Price = decimal.TryParse(x.price?.grandTotal, out var p) ? p : decimal.MaxValue
    })
    .GroupBy(x => x.Airline);


                var finalFlights = new List<ApiFlightData>();

                foreach (var group in airlineGroups)
                {
                    var directFlights = group
                    .Where(x => x.Flight.itineraries[0].segments.All(s => s.numberOfStops == 0))
                    .OrderBy(x => x.Price)
                    .ToList();

                    var nonDirectFlights = group
                        .Where(x => x.Flight.itineraries[0].segments.Any(s => s.numberOfStops > 0))
                        .OrderBy(x => x.Price)
                        .ToList();

                    // 🔹 Step 3: Combine (Direct first)
                    finalFlights.AddRange(directFlights.Select(x => x.Flight));
                    finalFlights.AddRange(nonDirectFlights.Select(x => x.Flight));
                }


                foreach (var item in finalFlights)
                {
                    var flight = BuildFlight(item, flightSearch);
                    if (flight != null)
                        flights.Add(flight);
                }

                flightsResult.FlightsList = flights;

                var blockRules = AplicationHelper.LoadBlockRulesFromFile(fareScrapper);
                RouteControlService.ApplyBlockAirlineRules(flightsResult, flightSearch, blockRules);

                if (flights.Any())
                {
                    flightsResult.MinPrice = (int)flights.Min(f => f.TotalCost);
                    flightsResult.MaxPrice = (int)flights.Max(f => f.TotalCost);
                }
                //AplicationHelper.Log("Step 10: Before Markup");
                MarkupService.ApplyMarkupV3
                    (flightsResult, flightSearch, fareScrapper, fareScrapper, ExtraMarkup);
                //AplicationHelper.Log("ApplyMarkupV3 return ");

                return flightsResult;
            }
            catch (Exception ex)
            {
                AplicationHelper.Log("SearchFlight Exception: " + ex.Message);

                flightsResult.Error = $"Unhandled Error: {ex.Message}";
                return flightsResult;
            }
        }

        public async Task<FlightsResult> SearchFlightOptimizedAsyncV2(FlightsSearch flightSearch, decimal ExtraMarkup)
        {
            //AplicationHelper.Log("SearchFlightOptimizedAsyncV1 Start");

            string fareScrapper = "Rush_FS";

            var flightsResult = new FlightsResult
            {
                FlightsList = new List<Flights>(),
                FlightsSearch = flightSearch
            };

            var allowRules = AplicationHelper.LoadAllowRulesFromFile(fareScrapper);
            if (!RouteControlService.ApplyAllowAirlineRules(flightSearch, allowRules))
                return flightsResult;

            try
            {
                var cacheKey = AplicationHelper.BuildSearchKey(
                    flightSearch.From.Code,
                    flightSearch.To.Code,
                    flightSearch.DepDate,
                    flightSearch.FlightWay == FlightWay.RoundTrip
                        ? flightSearch.RetDate
                        : (DateTime?)null,
                    flightSearch.Adult,
                    flightSearch.Child,
                    flightSearch.Infant,
                    (int)flightSearch.FlightWay,
                    (int)flightSearch.FlightClass
                );

                var cachePath = AplicationHelper.GetApiCachePath(cacheKey);

                ApiResponse apiFlightsResult = null;

                if (AplicationHelper.IsApiCacheValid(cachePath))
                {
                    apiFlightsResult = AplicationHelper.LoadApiResponse(cachePath);

                    if (apiFlightsResult?.data == null || !apiFlightsResult.data.Any())
                        apiFlightsResult = null;
                }

                if (apiFlightsResult == null)
                {
                    //AplicationHelper.Log("Cache Miss - Calling External API");

                    string classCode = flightSearch.FlightClass switch
                    {
                        FlightClass.First => "F",
                        FlightClass.Business => "B",
                        FlightClass.EconomyPremium => "P",
                        _ => "Y"
                    };

                    string query =
         $"?currency={ResolveSearchCurrency(flightSearch.Currency)}" +
         $"&JType={(flightSearch.FlightWay == FlightWay.RoundTrip ? "roundtrip" : "oneway")}" +
         $"&org={flightSearch.From.Code}" +
         $"&dest={flightSearch.To.Code}" +
         $"&depDt={flightSearch.DepDate:MMM-dd-yyyy}" +
         (flightSearch.FlightWay == FlightWay.RoundTrip
             ? $"&retDt={flightSearch.RetDate:MMM-dd-yyyy}"
             : string.Empty) +
         $"&adt={flightSearch.Adult}" +
         $"&chd={flightSearch.Child}" +
         $"&inf={flightSearch.Infant}" +
         $"&ct={classCode}" +
         //$"&limit=1"+
         //$"&cheapest=5000" +
         $"&userid={ApiUser}&password={ApiPass}&metaId={ApiMetaId}&website={ApiWebsite}";

                    var url = BaseUrl + query;

                    using var request = new HttpRequestMessage(HttpMethod.Get, url);
                    request.Headers.AcceptEncoding.ParseAdd("gzip, deflate");

                    using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(12));
                    using var response = await _client.SendAsync(
                        request,
                        HttpCompletionOption.ResponseHeadersRead,
                        cts.Token
                    );

                    if (!response.IsSuccessStatusCode)
                    {
                        //AplicationHelper.Log("API Error: " + response.StatusCode);

                        flightsResult.Error =
                            $"API Error: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}";
                        return flightsResult;
                    }
                    var jsondd = await response.Content.ReadAsStringAsync();

                    await using var stream = await response.Content.ReadAsStreamAsync();

                    apiFlightsResult = await System.Text.Json.JsonSerializer.DeserializeAsync<ApiResponse>(
                        stream,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true,
                            MaxDepth = 64
                        });
                    //AplicationHelper.Log("Step 7: After Deserialize");
                    //AplicationHelper.Log("API Data Count: " + apiFlightsResult?.data?.Count);
                    await AplicationHelper.SaveApiResponse(cachePath, apiFlightsResult);
                }

                if (apiFlightsResult?.data == null || !apiFlightsResult.data.Any())
                {
                    //AplicationHelper.Log("No Flights Found");

                    flightsResult.Error = "No flights found.";
                    return flightsResult;
                }

                var flights = new List<Flights>();
                var airlineGroups = apiFlightsResult.data
    .Where(x => x.itineraries != null && x.itineraries.Any())
    .Select(x => new
    {
        Flight = x,
        Airline = x.itineraries[0].segments[0].carrierCode,
        Duration = ConvertDurationToMinutes(x.itineraries[0].duration),
        Price = decimal.TryParse(x.price?.grandTotal, out var p) ? p : decimal.MaxValue
    })
    .GroupBy(x => x.Airline);


                var finalFlights = new List<ApiFlightData>();

                foreach (var group in airlineGroups)
                {
                    var directFlights = group
                    .Where(x => x.Flight.itineraries[0].segments.All(s => s.numberOfStops == 0))
                    .OrderBy(x => x.Price)
                    .ToList();

                    var nonDirectFlights = group
                        .Where(x => x.Flight.itineraries[0].segments.Any(s => s.numberOfStops > 0))
                        .OrderBy(x => x.Price)
                        .ToList();

                    // 🔹 Step 3: Combine (Direct first)
                    finalFlights.AddRange(directFlights.Select(x => x.Flight));
                    finalFlights.AddRange(nonDirectFlights.Select(x => x.Flight));
                }


                foreach (var item in finalFlights)
                {
                    var flight = BuildFlight(item, flightSearch);
                    if (flight != null)
                        flights.Add(flight);
                }

                flightsResult.FlightsList = flights;

                var blockRules = AplicationHelper.LoadBlockRulesFromFile(fareScrapper);
                RouteControlService.ApplyBlockAirlineRules(flightsResult, flightSearch, blockRules);

                if (flights.Any())
                {
                    flightsResult.MinPrice = (int)flights.Min(f => f.TotalCost);
                    flightsResult.MaxPrice = (int)flights.Max(f => f.TotalCost);
                }
                //AplicationHelper.Log("Step 10: Before Markup");
                MarkupService.ApplyMarkupV4
                    (flightsResult, flightSearch, fareScrapper, fareScrapper, ExtraMarkup);
                //AplicationHelper.Log("ApplyMarkupV3 return ");

                return flightsResult;
            }
            catch (Exception ex)
            {
                AplicationHelper.Log("SearchFlight Exception: " + ex.Message);

                flightsResult.Error = $"Unhandled Error: {ex.Message}";
                return flightsResult;
            }
        }

        private static int ConvertDurationToMinutes(string duration)
        {
            if (string.IsNullOrWhiteSpace(duration)) return 0;
            try { return (int)XmlConvert.ToTimeSpan(duration).TotalMinutes; }
            catch { return 0; }
        }


        private static FlightsSegment MapSegment(Segment seg, ApiFlightData item, bool isReturn)
        {
            return new FlightsSegment
            {
                FromAirport = new Airport { Code = seg.departure?.iataCode ?? "", CityName = seg.departure?.iataCode ?? "" },
                ToAirport = new Airport { Code = seg.arrival?.iataCode ?? "", CityName = seg.arrival?.iataCode ?? "" },
                DepartureDate = SafeDate(seg.departure?.at),
                ArrivalDate = SafeDate(seg.arrival?.at),
                Airline = new Airline { Code = seg.carrierCode ?? "", Name = seg.airline ?? "" },
                FlightNo = seg.number ?? "",
                IsReturn = isReturn,
                OperatingAirline = new Airline { Code = seg.operating?.carrierCode ?? "" },
                ElapsedTime = seg.duration ?? "",
                CabinClass = item.classofService ?? "",
                ClassOfService = item.classofService ?? "",
                Availability = item.numberOfBookableSeats.ToString(),
                BookingCode = item.jsonId ?? "",
                TotalTime = item.itineraries?[isReturn ? 1 : 0]?.duration ?? "",
                FromTerminal = seg.departure?.terminal ?? "",
                ToTerminal = seg.arrival?.terminal ?? ""
            };
        }
        private static DateTime SafeDate(string iso)
        {
            return DateTime.TryParse(iso, out var dt) ? dt : default;
        }

        private static Flights BuildFlight(ApiFlightData item, FlightsSearch flightSearch)
        {
            double adultFare = 0, adultTax = 0;
            double childFare = 0, childTax = 0;
            double infantFare = 0, infantTax = 0;

            if (item.travelerPricings != null)
            {
                var adult = item.travelerPricings.FirstOrDefault(x => x.travelerType == "ADULT");
                if (adult != null)
                {
                    adultFare = SafeDouble(adult.price?.@base);
                    adultTax = SafeDouble(adult.price?.fees);
                }

                var child = item.travelerPricings.FirstOrDefault(x => x.travelerType == "CHILD");
                if (child != null)
                {
                    childFare = SafeDouble(child.price?.@base);
                    childTax = SafeDouble(child.price?.fees);
                }

                var infant = item.travelerPricings.FirstOrDefault(x => x.travelerType == "INFANT");
                if (infant != null)
                {
                    infantFare = SafeDouble(infant.price?.@base);
                    infantTax = SafeDouble(infant.price?.fees);
                }
            }

            double grandTotal = SafeDouble(item.price?.grandTotal);

            var flight = new Flights
            {
                FlightId = Guid.NewGuid().ToString("N"),
                SearchId = flightSearch.SearchId,
                SessionId = Guid.NewGuid().ToString("N"),
                Currency = item.price?.currency ?? DefaultCurrency,
                TotalCost = Convert.ToDouble(item.price.@base),
                Outbound = new List<FlightsSegment>(),
                Inbound = new List<FlightsSegment>(),
                Airline = new Airline
                {
                    Code = item.validatingAirlineCodes?.FirstOrDefault() ?? string.Empty,
                    Name = item.itineraries?.FirstOrDefault()?.segments?.FirstOrDefault()?.airline ?? "Unknown Airline"
                },
                FlightFare = new FlightsFare(),
                Stops = 0
            };

            int totalStops = 0;
            int totalMinutes = 0;

            int outboundStops = 0;
            int inboundStops = 0;

            if (item.itineraries?.Count > 0)
            {
                var outSegs = item.itineraries[0].segments ?? new List<Segment>();

                foreach (var seg in outSegs)
                {
                    flight.Outbound.Add(MapSegment(seg, item, false));
                    totalMinutes += ConvertDurationToMinutes(seg.duration);
                }

                outboundStops = Math.Max(0, outSegs.Count - 1);
            }

            if (item.itineraries?.Count > 1)
            {
                var inSegs = item.itineraries[1].segments ?? new List<Segment>();

                foreach (var seg in inSegs)
                {
                    flight.Inbound.Add(MapSegment(seg, item, true));
                    totalMinutes += ConvertDurationToMinutes(seg.duration);
                }

                inboundStops = Math.Max(0, inSegs.Count - 1);
            }
            if (Math.Max(outboundStops, inboundStops) >= 2)
                return null;

            flight.Stops = Math.Max(outboundStops, inboundStops);
            flight.TotalTime = totalMinutes;

            flight.FlightFare = new FlightsFare
            {
                AdultFare = adultFare,
                AdultTax = adultTax,
                ChildFare = childFare,
                ChildTax = childTax,
                InfantFare = infantFare,
                InfantTax = infantTax,

                Adult = flightSearch.Adult,
                Child = flightSearch.Child,
                Infant = flightSearch.Infant,

                MarkupType = "Fixed",
                MarkupValue = "0",
                AdultMarkup = 0,
                ChildMarkup = 0,
                InfantMarkup = 0,

                AvgCost =
            ((adultFare + adultTax) * flightSearch.Adult) +
            ((childFare + childTax) * flightSearch.Child) +
            ((infantFare + infantTax) * flightSearch.Infant)
            };

            return flight;
        }
        private static double SafeDouble(object o)
        {
            if (o == null) return 0;
            try { return Convert.ToDouble(o); } catch { return 0; }
        }
        public MetaFlightSearchResponse MapToMetaResponse(FlightsResult engine)
        {

            var meta = new MetaFlightSearchResponse();
            if (engine?.FlightsList == null) return meta;

            var searchId = engine.FlightsSearch?.SearchId ?? Guid.NewGuid().ToString("N");

            AplicationHelper.Save(engine.FlightsSearch?.SearchId, "search_", engine);
            var flightssearch = engine.FlightsSearch;
            string fileName = $"search_{searchId}";


            foreach (var f in engine.FlightsList)
            {
                var item = new MetaFlightItem
                {
                    Id = f.FlightId,
                    CabinClass = MapCabin(f.FlightClass),
                    TotalAmt = SafeDouble(f.TotalCost),
                    TotalTax = (double)Math.Round(
    (decimal)SafeDouble(f.FlightFare?.AdultMarkupafterTax) * (f.FlightFare?.Adult ?? 0) +
    (decimal)SafeDouble(f.FlightFare?.ChildMarkupafterTax) * (f.FlightFare?.Child ?? 0) +
    (decimal)SafeDouble(f.FlightFare?.InfantMarkupafterTax) * (f.FlightFare?.Infant ?? 0),
    2,
    MidpointRounding.AwayFromZero
),
                    Stop = f.Stops,
                    TotalDuration = f.TotalTime,
                    BookUrl = BuildBookUrl(searchId, f.FlightId, engine.Source),
                    
                };

                var segmentWrapper = new MetaResponseDto.MetaOSeg();

                int totalSegments = 0;

                // Outbound
                if (f.Outbound != null && f.Outbound.Any())
                {
                    var outboundSeg = new Metaseggg();
                    outboundSeg.ISeg = new List<MetaResponseDto.MetaISeg>();
                    segmentWrapper.OSeg = new List<Metaseggg>();
                    foreach (var s in f.Outbound)
                        outboundSeg.ISeg.Add(MapSeg(s));

                    totalSegments += outboundSeg.ISeg.Count;
                    segmentWrapper.OSeg.Add(outboundSeg);
                }

                // Inbound
                if (f.Inbound != null && f.Inbound.Any())
                {
                    var inboundSeg = new Metaseggg();
                    inboundSeg.ISeg = new List<MetaResponseDto.MetaISeg>();
                    foreach (var s in f.Inbound)
                        inboundSeg.ISeg.Add(MapSeg(s));

                    totalSegments += inboundSeg.ISeg.Count;
                    segmentWrapper.OSeg.Add(inboundSeg);
                }

                if (segmentWrapper.OSeg.Any())
                    item.Segments.Add(segmentWrapper);

                // --- Calculate Stop count ---
                //item.Stop = totalSegments > 0 ? totalSegments - 1 : 0;
                meta.Airlines = meta.Airlines.OrderBy(x => x.Stop).ThenBy(x => x.TotalAmt).ToList(); meta.Airlines.Add(item);
            }

            return meta;
        }
        private static string ResolveSearchCurrency(string currency)
        {
            if (string.IsNullOrWhiteSpace(currency) ||
                currency.Equals("MXN", StringComparison.OrdinalIgnoreCase))
                return DefaultCurrency;
            return currency;
        }

        private static MetaResponseDto.MetaISeg MapSeg(FlightsSegment s)
        {
            var dep = s.DepartureDate != default ? s.DepartureDate : SafeDate(s.DepDate);
            var arr = s.ArrivalDate != default ? s.ArrivalDate : SafeDate(s.ArrDate);

            return new MetaResponseDto.MetaISeg
            {
                DDat = dep.ToString("yyyy-MM-dd"),
                ADat = arr.ToString("yyyy-MM-dd"),
                DTime = dep.ToString("HH:mm"),
                ATime = arr.ToString("HH:mm"),
                DApt = s.FromAirport?.Code ?? "",
                AApt = s.ToAirport?.Code ?? "",
                MA = s.Airline?.Code ?? "",
                AirlineNm = s.Airline?.Name ?? "",
                FN = s.FlightNo ?? "",
                FBA = s.BaggageAllowance ?? "",
                DTer = s.FromTerminal ?? "",
                ATer = s.ToTerminal ?? "",
                BC = s.CabinClass ?? "",
                SegmentDuration = ConvertDurationToMinutes(s.ElapsedTime)
            };
        }

        private static string BuildBookUrl(string searchId, string flightId, string Source)
        {
            return $"https://www.rushmyflight.com/Flight/FlightBooking?Guid={HttpUtility.UrlEncode(searchId)}&FId={HttpUtility.UrlEncode(flightId)}&Source={HttpUtility.UrlEncode(Source)}";
        }
        private static string MapCabin(FlightClass c)
        {
            return c switch
            {
                FlightClass.First => "FC",
                FlightClass.Business => "BC",
                FlightClass.EconomyPremium => "PE",
                _ => "EC"
            };
        }

    }

}
