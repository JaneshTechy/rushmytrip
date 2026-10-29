using DocumentFormat.OpenXml.Bibliography;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using TravelWeb.Datamodel;
using TravelWeb.Helper;
using TravelWeb.Models;
using TravelWeb.Utilities;
using static System.Web.Razor.Parser.SyntaxConstants;
using static TravelWeb.Models.MetaRequestDto;
using static TravelWeb.Models.MetaResponseDto;

namespace TravelWeb.Controllers
{
    [ApiController]
    public class MetaFlightController : ControllerBase
    {
        DeepLinkTrackerService _deepLinkTrackerService;
        private static readonly HttpClient _client = new HttpClient { Timeout = TimeSpan.FromSeconds(60) };
        private static readonly object _lock = new object();
        private static List<BlockAirlineRuleDto> _cachedBlockRules;
        private static DateTime _lastLoad;
        private readonly ILogger _logger;
        //private const string BaseUrl = "https://rushmyflight.wikiproject.in/";
        private const string BaseUrl = "https://rushmyflight.wikiproject.in/nv3";

        private const string DefaultCurrency = "USD";
        private const string ApiUser = "rushmy";
        private const string ApiPass = "RUrM5Lf";
        private const string ApiMetaId = "2020";
        private const string ApiWebsite = "rushmyflight";
        private readonly IServiceScopeFactory _scopeFactory;

        public MetaFlightController(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;

        }
        static MetaFlightController()
        {
            var handler = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };
            _client = new HttpClient(handler)
            {
                Timeout = TimeSpan.FromSeconds(60)
            };
        }

        [HttpPost("api/flight/Search")]
        public async Task<IActionResult> SearchOptimized([FromBody] MetaFlightSearchRequest req)
        {

            
            try
            {
                var AirportFromDetails = new Airport();
                AirportFromDetails = Common.GetAirport(req.Segments.Select(x => x.DepartApt).FirstOrDefault());
                var AirportToDetails = new Airport();
                AirportToDetails = Common.GetAirport(req.Segments.Select(x => x.ArrivalApt).FirstOrDefault());

                var context = new RSFLY();
                string ip = req.GeneralInfo.IPAddress;

                var TotalCount = (req.PaxDetails.Adult + req.PaxDetails.Child + req.PaxDetails.Infant);
                string campaignId = GetCampaignId(req.Source);
                var Campeign = context.TblCampaigns.Where(x => x.CampId == campaignId && x.Status == true && TotalCount >= x.PaxCount).FirstOrDefault();

                //if (req.Source == "Trabber")
                //{
                //    Campeign = new TravelWeb.Datamodel.TblCampaign();
                //    Campeign.Status = true;
                //    Campeign.ExtraMarkup = 0;
                //}
                //Campeign.Status = true;
                if (Campeign != null)
                {
                    if (Campeign != null && Campeign.Status == true)
                    {
                        if (req.GeneralInfo.Username == "LinkToFSRMF" && req.GeneralInfo.Password == "RMF@11@2025")
                        {
                            var fs = MapToFlightsSearch(req);
                            fs.From = AirportFromDetails;
                            fs.To = AirportToDetails;
                            fs.SearchId ??= Guid.NewGuid().ToString("N");
                            fs.SiteCode = req.Source;
                            fs.CampId = campaignId;
                            var engine = await SearchFlightOptimizedAsyncV1(fs, Campeign.ExtraMarkup);

                            req.GeneralInfo.GUID = fs.SearchId;
                            if (req.Source == "Trabber")
                            {
                                engine.Source = "Trabber";

                            }
                            else
                            {
                                engine.Source = "FS";
                            }
                            if (engine.FlightsList != null && engine.FlightsList.Count > 0)
                            {
                                try
                                {
                                    var scope = _scopeFactory.CreateScope();
                                    var trackerService = scope.ServiceProvider.GetRequiredService<DeepLinkTrackerService>();
                                    _ = trackerService.CreateOrGetTrackerAsync(req, "Listing", null, ip);
                                }
                                catch { }
                            }

                            if (!string.IsNullOrWhiteSpace(engine.Error))
                            {
                                //AplicationHelper.Log("Engine Error: " + engine.Error);
                                return StatusCode(502, new { error = engine.Error });
                            }

                            var metaResp = MapToMetaResponse(engine);

                            return Ok(new { searchId = fs.SearchId, result = metaResp });
                        }
                        else
                        {
                            //AplicationHelper.Log("Invalid Credentials Attempt");
                            return Unauthorized(new { error = "Invalid credentials" });
                        }
                    }

                    //AplicationHelper.Log("Campaign Status Invalid");
                    return Unauthorized(new { error = "Invalid credentials" });
                }
                else
                {
                    //AplicationHelper.Log("Campaign OFF");
                    return Unauthorized(new { error = "Campaign is off" });
                }
            }
            catch (Exception ex)
            {
                //AplicationHelper.Log("Search API Exception: " + ex.Message);
                return StatusCode(500, new { error = "Internal error", detail = ex.Message });
            }
        }

        public static async Task<FlightsResult> SearchFlightOptimizedAsync(FlightsSearch flightSearch)
        {
            string fareScrapper = "Rush_FS";



            //var allowRules = RouteControlService.LoadAllowRulesFromDb(fareScrapper);

            //AplicationHelper.SaveAllowRulesToFile(allowRules);
            var allowRules = AplicationHelper.LoadAllowRulesFromFile(fareScrapper);
            // var Json = JsonConvert.SerializeObject(allowRules);
            bool allowtoHit = RouteControlService.ApplyAllowAirlineRules(flightSearch, allowRules);
            var flightsResult = new FlightsResult
            {
                FlightsList = new List<Flights>(),
                FlightsSearch = flightSearch
            };

            if (allowtoHit == true)
            {


                try
                {
                    string classCode = flightSearch.FlightClass switch
                    {
                        FlightClass.First => "F",
                        FlightClass.Business => "B",
                        FlightClass.EconomyPremium => "P",
                        _ => "Y"
                    };

                    string query = $"?currency={((flightSearch.Currency ?? DefaultCurrency).Equals("MXN", StringComparison.OrdinalIgnoreCase) ? DefaultCurrency : (flightSearch.Currency ?? DefaultCurrency))}" +
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
                                   $"&userid={ApiUser}&password={ApiPass}&metaId={ApiMetaId}&website={ApiWebsite}";

                    var url = BaseUrl + query;

                    using var request = new HttpRequestMessage(HttpMethod.Get, url);
                    request.Headers.AcceptEncoding.ParseAdd("gzip, deflate");

                    //using var response = await _client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
                    using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(12));
                    using var response = await _client.SendAsync(
                        request,
                        HttpCompletionOption.ResponseHeadersRead,
                        cts.Token
                    );

                    if (!response.IsSuccessStatusCode)
                    {
                        var err = await response.Content.ReadAsStringAsync();
                        flightsResult.Error = $"API Error: {response.StatusCode} - {err}";
                        return flightsResult;
                    }

                    await using var stream = await response.Content.ReadAsStreamAsync();
                    var apiFlightsResult = await System.Text.Json.JsonSerializer.DeserializeAsync<ApiResponse>(
                        stream,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true, MaxDepth = 64 }
                    );

                    if (apiFlightsResult?.data == null || !apiFlightsResult.data.Any())
                    {
                        flightsResult.Error = "No flights found.";
                        return flightsResult;
                    }


                    var flights = new List<Flights>();
                    foreach (var item in apiFlightsResult.data)
                    {
                        try
                        {
                            var flight = BuildFlight(item, flightSearch);
                            if (flight != null)
                                flights.Add(flight);
                        }
                        catch
                        {
                        }
                    }

                    flightsResult.FlightsList = flights;
                    //AplicationHelper.SaveBlockRulesToFile(blockRules);
                    var blockRules = AplicationHelper.LoadBlockRulesFromFile(fareScrapper);
                    //AplicationHelper.SaveBlockRulesToFile(blockRules);
                    RouteControlService.ApplyBlockAirlineRules(flightsResult, flightSearch, blockRules);

                    if (flightsResult.FlightsList.Any())
                    {
                        flightsResult.MinPrice = (int)flightsResult.FlightsList.Min(f => f.TotalCost);
                        flightsResult.MaxPrice = (int)flightsResult.FlightsList.Max(f => f.TotalCost);
                    }

                    MarkupService.ApplyMarkupV3(flightsResult, flightSearch, fareScrapper);

                    //Common.SaveObjectSession($"search_{flightSearch.SearchId}", flightsResult);

                    return flightsResult;
                }
                catch (Exception ex)
                {
                    flightsResult.Error = $"Unhandled Error: {ex.Message}";
                    return flightsResult;
                }
            }
            else
            {

            }
            return flightsResult;
        }

       

        public static async Task<FlightsResult> SearchFlightOptimizedAsyncV1(FlightsSearch flightSearch, decimal ExtraMarkup)
        {
            //AplicationHelper.Log("SearchFlightOptimizedAsyncV1 Start");

            //string fareScrapper = "Rush_FS";

            var flightsResult = new FlightsResult
            {
                FlightsList = new List<Flights>(),
                FlightsSearch = flightSearch
            };
            if(flightSearch.CampId!= "Rush_FC")
            {
                var allowRules = AplicationHelper.LoadAllowRulesFromFile(flightSearch.CampId);
                if (!RouteControlService.ApplyAllowAirlineRules(flightSearch, allowRules))
                    return flightsResult;
            }
            

            
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

                //if (AplicationHelper.IsApiCacheValid(cachePath))
                //{
                //    apiFlightsResult = AplicationHelper.LoadApiResponse(cachePath);

                //    if (apiFlightsResult?.data == null || !apiFlightsResult.data.Any())
                //        apiFlightsResult = null;
                //}

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
                        $"?userid={Uri.EscapeDataString(ApiUser)}" +
                        $"&password={Uri.EscapeDataString(ApiPass)}" +
                        $"&org={Uri.EscapeDataString(flightSearch.From.Code)}" +
                        $"&dest={Uri.EscapeDataString(flightSearch.To.Code)}" +
                        $"&depDt={flightSearch.DepDate:MMM-dd-yyyy}" +
                        (flightSearch.FlightWay == FlightWay.RoundTrip
                            ? $"&retDt={flightSearch.RetDate:MMM-dd-yyyy}"
                            : string.Empty) +
                        $"&JType={(flightSearch.FlightWay == FlightWay.RoundTrip ? "roundtrip" : "oneway")}" +
                        $"&adt={flightSearch.Adult}" +
                        $"&chd={flightSearch.Child}" +
                        $"&inf={flightSearch.Infant}" +
                        $"&ct={classCode}" +
                         
                           $"&metaId=2020" +

                           $"&website=rushmyflight"+
                    $"&currency={Uri.EscapeDataString((flightSearch.Currency ?? DefaultCurrency).Equals("MXN", StringComparison.OrdinalIgnoreCase) ? DefaultCurrency : (flightSearch.Currency ?? DefaultCurrency))}";

                    string url;
                    string baseUrl = BaseUrl;

                    if (flightSearch.FlightWay == FlightWay.OneWay)
                        baseUrl = "https://rushmyflight.wikiproject.in/";

                    if (flightSearch.CampId== "Rush_FS")
                    {
                        url = "https://rushmyflight.wikiproject.in" + query;
                    }
                    else
                    {
                        url = baseUrl + query;

                    }


                    // First API call
                    using (var request = new HttpRequestMessage(HttpMethod.Get, url))
                    {
                        request.Headers.AcceptEncoding.ParseAdd("gzip, deflate");

                        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(60));

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

                        await using var stream = await response.Content.ReadAsStreamAsync();

                        apiFlightsResult =
                            await System.Text.Json.JsonSerializer.DeserializeAsync<ApiResponse>(
                                stream,
                                new JsonSerializerOptions
                                {
                                    PropertyNameCaseInsensitive = true,
                                    MaxDepth = 64
                                });
                    }

                    // =========================================================
                    // ONE TIME RETRY ONLY IF DATA IS NULL OR EMPTY
                    // =========================================================
                    if (apiFlightsResult?.data == null || !apiFlightsResult.data.Any())
                    {
                        // Second API call - only once
                        using (var retryRequest = new HttpRequestMessage(HttpMethod.Get, url))
                        {
                            retryRequest.Headers.AcceptEncoding.ParseAdd("gzip, deflate");

                            using var retryCts = new CancellationTokenSource(TimeSpan.FromSeconds(60));

                            using var retryResponse = await _client.SendAsync(
                                retryRequest,
                                HttpCompletionOption.ResponseHeadersRead,
                                retryCts.Token
                            );

                            if (retryResponse.IsSuccessStatusCode)
                            {
                                await using var retryStream =
                                    await retryResponse.Content.ReadAsStreamAsync();

                                apiFlightsResult =
                                    await System.Text.Json.JsonSerializer.DeserializeAsync<ApiResponse>(
                                        retryStream,
                                        new JsonSerializerOptions
                                        {
                                            PropertyNameCaseInsensitive = true,
                                            MaxDepth = 64
                                        });
                            }
                        }
                    }

                    // Save only the final API response
                    await AplicationHelper.SaveApiResponse(cachePath, apiFlightsResult);
                }

                if (apiFlightsResult?.data == null)
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
                        Price = decimal.TryParse(
                            x.price?.grandTotal,
                            out var p
                        ) ? p : decimal.MaxValue
                    })
                    .GroupBy(x => x.Airline);

                var finalFlights = new List<ApiFlightData>(500);

                foreach (var group in airlineGroups)
                {
                    // Step 1: Direct flights (0 stop) FIRST
                    var directFlights = group
                        .Where(x =>
                            x.Flight.itineraries[0].segments.All(
                                s => s.numberOfStops == 0
                            ))
                        .OrderBy(x => x.Price)
                        .Take(1100)
                        .ToList();

                    var remainingCount = 800 - directFlights.Count;

                    var nonDirectFlights = group
                        .Where(x =>
                            x.Flight.itineraries[0].segments.Any(
                                s => s.numberOfStops > 0
                            ))
                        .OrderBy(x => x.Price)
                        .Take(remainingCount > 0 ? remainingCount : 0)
                        .ToList();

                    // Step 3: Combine (Direct first)
                    finalFlights.AddRange(
                        directFlights.Select(x => x.Flight)
                    );

                    finalFlights.AddRange(
                        nonDirectFlights.Select(x => x.Flight)
                    );
                }

                foreach (var item in finalFlights)
                {
                    var flight = BuildFlight(item, flightSearch);

                    if (flight != null)
                        flights.Add(flight);
                }

                flightsResult.FlightsList = flights;

                var blockRules =
                    AplicationHelper.LoadBlockRulesFromFile(flightSearch.CampId);

                RouteControlService.ApplyBlockAirlineRules(
                    flightsResult,
                    flightSearch,
                    blockRules
                );

                if (flights.Any())
                {
                    flightsResult.MinPrice =
                        (int)flights.Min(f => f.TotalCost);

                    flightsResult.MaxPrice =
                        (int)flights.Max(f => f.TotalCost);
                }

                if (flightSearch.SiteCode == "Trabber")
                {
                    MarkupService.ApplyMarkupV4(
                        flightsResult,
                        flightSearch,
                        flightSearch.CampId,
                        flightSearch.CampId,
                        ExtraMarkup
                    );
                }
                else
                {
                    MarkupService.ApplyMarkupV3(
                        flightsResult,
                        flightSearch,
                        flightSearch.CampId,
                        flightSearch.CampId,
                        ExtraMarkup
                    );
                }

                return flightsResult;
            }
            catch (Exception ex)
            {
                AplicationHelper.Log(
                    "SearchFlight Exception: " + ex.Message
                );

                flightsResult.Error =
                    $"Unhandled Error: {ex.Message}";

                return flightsResult;
            }


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
            { ApiId = item.id,
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
        private static int CalculateStopsFromSegments(List<Segment> segments)
        {
            if (segments == null || segments.Count <= 1)
                return 0;

            int stops = 0;

            for (int i = 0; i < segments.Count - 1; i++)
            {
                var currentArrival = segments[i].arrival?.iataCode;
                var nextDeparture = segments[i + 1].departure?.iataCode;

                if (!string.Equals(currentArrival, nextDeparture, StringComparison.OrdinalIgnoreCase))
                    stops++;
            }

            return stops;
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

        private static int ConvertDurationToMinutes(string duration)
        {
            if (string.IsNullOrWhiteSpace(duration)) return 0;
            try { return (int)XmlConvert.ToTimeSpan(duration).TotalMinutes; }
            catch { return 0; }
        }
        public static int ConvertDurationToMinutesV1(string duration)
        {
            // Example: PT5H51M
            var time = duration.Replace("PT", "");
            int hours = 0, minutes = 0;

            if (time.Contains("H"))
            {
                var parts = time.Split('H');
                hours = int.Parse(parts[0]);
                time = parts.Length > 1 ? parts[1] : "";
            }

            if (time.Contains("M"))
            {
                minutes = int.Parse(time.Replace("M", ""));
            }

            return (hours * 60) + minutes;
        }
        private static List<BlockAirlineRuleDto> GetCachedBlockRules(string fareScrapper)
        {
            if (_cachedBlockRules == null || (DateTime.Now - _lastLoad).TotalMinutes > 30 || _cachedBlockRules.Count == 0)
            {

                _cachedBlockRules = RouteControlService.LoadBlockRulesFromDb(fareScrapper);
                _lastLoad = DateTime.Now;
            }
            return _cachedBlockRules;
        }
        private static FlightsSearch MapToFlightsSearch(MetaFlightSearchRequest req)
        {
            bool isRoundTrip = req?.GeneralInfo?.TripType?.Equals("R", StringComparison.OrdinalIgnoreCase) ?? false;

            var cabin = req?.GeneralInfo?.Cabin ?? "EC";
            FlightClass cls = cabin.ToUpper() switch
            {
                "FC" => FlightClass.First,
                "BC" => FlightClass.Business,
                "PE" => FlightClass.EconomyPremium,
                _ => FlightClass.Economy
            };

            DateTime dep = ParseYmd(req?.Segments?.FirstOrDefault()?.DepartDate);
            DateTime ret = isRoundTrip && req?.Segments?.Count > 1
                ? ParseYmd(req.Segments[1].DepartDate)
                : DateTime.MinValue;

            return new FlightsSearch
            {
                SearchId = Guid.NewGuid().ToString("N"),
                From = new Airport { Code = req?.Segments?.FirstOrDefault()?.DepartApt ?? "" },
                To = new Airport { Code = req?.Segments?.FirstOrDefault()?.ArrivalApt ?? "" },
                DepDate = dep,
                RetDate = ret,
                Adult = req?.PaxDetails?.Adult ?? 1,
                Child = req?.PaxDetails?.Child ?? 0,
                Infant = req?.PaxDetails?.Infant ?? 0,
                FlightWay = isRoundTrip ? FlightWay.RoundTrip : FlightWay.OneWay,
                FlightClass = cls,
                IsDirect = req?.AdvanceSearch?.DirectFlight ?? false,
                Currency = req?.GeneralInfo?.Currency ?? DefaultCurrency,
                IPAddress = req?.GeneralInfo?.IPAddress ?? ""
            };
        }


        private static DateTime ParseYmd(string ymd)
        {
            return DateTime.TryParseExact(ymd ?? "", "yyyy-MM-dd", null, DateTimeStyles.None, out var dt)
                ? dt
                : DateTime.MinValue;
        }

        public static MetaFlightSearchResponse MapToMetaResponse(FlightsResult engine)
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
                    BookUrl = BuildBookUrl(searchId, f.FlightId, engine.Source)
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

        private static string BuildBookUrl(string searchId, string flightId, string Source)
        {
            return $"https://www.rushmyflight.com/Flight/FlightBooking?Guid={HttpUtility.UrlEncode(searchId)}&FId={HttpUtility.UrlEncode(flightId)}&Source={HttpUtility.UrlEncode(Source)}";

        }

        private static DateTime SafeDate(string iso)
        {
            return DateTime.TryParse(iso, out var dt) ? dt : default;
        }

        private static double SafeDouble(object o)
        {
            if (o == null) return 0;
            try { return Convert.ToDouble(o); } catch { return 0; }
        }

        [HttpGet("flight/deeplink")]
        public IActionResult DeepLink(
    string FromLocation,
    string ToLocation,
    string DepartureDate,
    bool IsRoundTrip = false,
    string ReturnDate = "",
    int Adults = 1,
    int Children = 0,
    int Infants = 0,
    string Class = "EC",
    string Source = "DEEPLINK",
    string Device = "Web"
)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(FromLocation) ||
                    string.IsNullOrWhiteSpace(ToLocation) ||
                    string.IsNullOrWhiteSpace(DepartureDate))
                {
                    return BadRequest(new { error = "Missing required parameters" });
                }

                var searchId = Guid.NewGuid().ToString("N");

                string departOn = ConvertDateFormat(DepartureDate, "dd/MM/yyyy", "yyyy-MM-dd");
                string returnOn = IsRoundTrip && !string.IsNullOrEmpty(ReturnDate)
                    ? ConvertDateFormat(ReturnDate, "dd/MM/yyyy", "yyyy-MM-dd")
                    : null;

                var req = new MetaFlightSearchRequest
                {
                    Source = Source,
                    Device = Device,
                    GeneralInfo = new MetaGeneralInfo
                    {
                        GUID = searchId,
                        TripType = IsRoundTrip ? "R" : "O",
                        Cabin = Class
                    },
                    PaxDetails = new MetaPaxDetails
                    {
                        Adult = Adults,
                        Child = Children,
                        Infant = Infants
                    },
                    Segments = new List<MetaSegment>
            {
                new MetaSegment
                {
                    DepartApt = FromLocation,
                    ArrivalApt = ToLocation,
                    DepartDate = departOn
                }
            }
                };

                if (IsRoundTrip && returnOn != null)
                {
                    req.Segments.Add(new MetaSegment
                    {
                        DepartApt = ToLocation,
                        ArrivalApt = FromLocation,
                        DepartDate = returnOn
                    });
                }

                var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
                var ua = HttpContext.Request.Headers["User-Agent"].ToString();

                //_ = Task.Run(async () =>
                //{
                //    try
                //    {
                //        using var scope = _scopeFactory.CreateScope();
                //        var trackerService = scope.ServiceProvider.GetRequiredService<DeepLinkTrackerService>();

                //        await trackerService.CreateOrGetTrackerAsync(
                //            req,
                //            page: "DeepLink",
                //            parentSearchId: null,
                //            ip: ip,
                //            userAgent: ua
                //        );
                //    }
                //    catch { }
                //});

                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var trackerService = scope.ServiceProvider.GetRequiredService<DeepLinkTrackerService>();

                    _ = trackerService.CreateOrGetTrackerAsync(
                        req,
                        page: "DeepLink",
                        parentSearchId: null,
                        ip: ip,
                        userAgent: ua
                    );
                }
                catch
                {
                    // optional log
                }


                string trip = IsRoundTrip ? "round" : "oneway";

                string cabin = Class.ToUpperInvariant() switch
                {
                    "BC" => "B",
                    "FC" => "F",
                    "PE" => "P",
                    _ => "Y"
                };

                string url = $"/result?depart={FromLocation}&arrival={ToLocation}" +
                             $"&trip={trip}&departOn[]={departOn}&returnOn={returnOn}" +
                             $"&adult={Adults}&child={Children}&infant={Infants}&seniors=0" +
                             $"&cabin={cabin}&utm_source={Source}&utm_medium={Device}" +
                             $"&utm_campaign=deeplink&af=deeplink&searchID={Guid.NewGuid()}";

                return Redirect(url);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Internal server error", detail = ex.Message });
            }
        }


        private static string ConvertDateFormat(string input, string fromFormat, string toFormat)
        {
            if (DateTime.TryParseExact(input, fromFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
                return dt.ToString(toFormat);
            return input;
        }

        [HttpGet("Flight/FlightBooking")]
        public IActionResult FlightBooking(string Guid, string FId, string Source = "Rush_FS")
        { if (Source == null)
                Source = "Rush_FS";
            if (string.IsNullOrEmpty(Guid) || string.IsNullOrEmpty(FId))
                return BadRequest("Invalid parameters.");

            FlightsResult flightSearch = AplicationHelper.Get<FlightsResult>(Guid, "search_");
            //var flightSearch = Common.GetObjectSession<FlightsResult>($"search_{Guid}");
            if (flightSearch == null)
                return RedirectToAction("Index", "Home");

            var selectedFlight = flightSearch.FlightsList.FirstOrDefault(f => f.FlightId == FId);
            if (selectedFlight == null)
                return RedirectToAction("Index", "Home");

            var redirectUrl = Url.Action(
                "Index",
                "Details",
                new
                {
                    depart = flightSearch.FlightsSearch.From.Code,
                    arrival = flightSearch.FlightsSearch.To.Code,
                    trip = flightSearch.FlightsSearch.FlightWay.ToString().ToLowerInvariant(),
                    departOn = new[] { flightSearch.FlightsSearch.DepDate.ToString("yyyy-MM-dd") },
                    af = Source,
                    ch = "rushmyfligh",
                    adult = flightSearch.FlightsSearch.Adult,
                    child = flightSearch.FlightsSearch.Child,
                    infant = flightSearch.FlightsSearch.Infant,
                    seniors = 0,
                    cabin = flightSearch.FlightsSearch.FlightClass,
                    searchID = Guid,
                    returnOn = flightSearch.FlightsSearch.RetDate != DateTime.MinValue
                                ? flightSearch.FlightsSearch.RetDate.ToString("yyyy-MM-dd")
                                : "01-01-1900",
                    utm_source = Source,
                    utm_medium = "Web",
                    utm_campaign = $"deeplink{Guid}{Guid}",
                    flightId = FId,
                    moreadultcount = flightSearch.FlightsSearch.Adult,
                    morechildcount = flightSearch.FlightsSearch.Child
                });
            _ = Task.Run(async () =>
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var trackerService = scope.ServiceProvider.GetRequiredService<DeepLinkTrackerService>();

                    await trackerService.UpdateStageAsync(FId, "Detail");
                }
                catch { }
            });

            return Redirect(redirectUrl);
        }

        private const string ReportApiKey = "Rushmyflight2026";


        //[HttpGet]
        //[Route("api/meta/reports")]
        //public async Task<IActionResult> GetMetaReports(
        //    [FromQuery] string apikey,
        //    [FromQuery] DateTime? from,
        //    [FromQuery] DateTime? to)
        //{
        //    if (string.IsNullOrEmpty(apikey) || apikey != ReportApiKey)
        //        return Unauthorized(new { message = "Invalid API key" });

        //    var fromDate = from ?? DateTime.UtcNow.AddDays(-90);
        //    var toDate = (to ?? DateTime.UtcNow).AddDays(1); // inclusive of "to" day

        //    using var scope = _scopeFactory.CreateScope();
        //    var db = scope.ServiceProvider.GetRequiredService<RSFLY>(); // <-- your DbContext type here

        //    var rows = await db.FlightBookings // <-- your DbSet<FlightBooking> here
        //        .AsNoTracking()
        //        .Where(fb => fb.DelInd == false
        //                  && fb.BookingDate >= fromDate
        //                  && fb.BookingDate < toDate&& fb.Agent=="Trabber")
        //        .OrderByDescending(fb => fb.BookingDate)
        //        .Select(fb => new
        //        {
        //            OrderId = fb.BookingId,
        //            OrderRef = fb.BookingNumber,
        //            BookingDateTime = fb.BookingDate,
        //            TravelDate = fb.DepartDate,
        //            BookingAmount = fb.Gross,
        //            //Currency = fb.Currency ?? "USD",
        //            // ASSUMPTION: commission = 5% of Gross.
        //            // Swap to fb.Mco / fb.TotalMco if that column already
        //            // holds the actual commission earned.
        //            Agent = fb.Agent,
        //            TotalPax = fb.Adult + fb.Child + fb.Infant,

        //            Currency = "USD",
        //            CommissionAmount = fb.Gross * 0.05m,
        //            SubId = fb.SearchId,
        //            CustomerCountry = fb.BillCountry ?? fb.PhoneCountry,
        //            BookingStatus = fb.IsStatus
        //        })
        //        .ToListAsync();

        //    return Ok(rows);
        //}

        //    public static void SaveSearchCache(
        //string origin,
        //string destination,
        //DateTime fromDate,
        //DateTime? toDate,
        //int adult,
        //int child,
        //int infant,
        //string searchId, FlightWay FlightWay,
        //string fileName)
        //    {
        //        var db = new RSFLY();
        //        int tripType = (int)FlightWay;
        //        var key = AplicationHelper.BuildSearchKey(origin, destination, fromDate, toDate, adult, child, infant,tripType);

        //        //var cache = null;/*db.FlightSearchCaches.SingleOrDefault(x => x.SearchKey == searchId)*/;

        //        //if (cache == null)
        //        //{
        //            var cache = new FlightSearchCache
        //            {
        //                //SearchKey = searchId,
        //                SearchId = searchId,
        //                Origin = origin,
        //                Destination = destination,
        //                FromDate = fromDate,
        //                ToDate = toDate,
        //                Adult = adult,
        //                Child = child,
        //                Infant = infant,
        //                TripType = tripType,
        //                FileName = fileName,
        //                CacheKey = key,
        //                CreatedAt = DateTime.UtcNow,
        //            };

        //            db.FlightSearchCaches.Add(cache);
        //        //}
        //        //else
        //        //{
        //        //    cache.SearchId = searchId;
        //        //    cache.FileName = fileName;

        //        //}

        //        db.SaveChanges();
        //    }


        //public static FlightsResult GetCachedSearch(FlightsSearch search)
        //{
        //    int tripType = (int)search.FlightWay;

        //    string key = AplicationHelper.BuildSearchKey(
        //        search.From.Code,
        //        search.To.Code,
        //        search.DepDate,
        //        search.RetDate,
        //        search.Adult,
        //        search.Child,
        //        search.Infant,
        //        tripType,search.FlightClass
        //    );

        //    using var db = new RSFLY();

        //    var cache = db.FlightSearchCaches
        //                  .AsNoTracking()
        //                  .FirstOrDefault(x =>
        //                        x.CacheKey == key &&
        //                        x.CreatedAt >= DateTime.UtcNow.AddHours(-2)
        //                  );

        //    if (cache == null)
        //        return null;

        //    var result = AplicationHelper.Get<FlightsResult>(cache.SearchId, "search_");

        //    if (result != null)
        //    {
        //        result.SearchCasheKey = key;      
        //    }

        //    return result;
        //}


        [HttpGet]
        [Route("api/meta/reports")]
        public async Task<IActionResult> GetMetaReports(
    [FromQuery] string apikey,
    [FromQuery] DateTime? from,
    [FromQuery] DateTime? to)
        {
            if (string.IsNullOrWhiteSpace(apikey) || apikey != ReportApiKey)
                return Unauthorized(new { message = "Invalid API key" });

            // Default: last 90 days
            var fromDate = from?.Date ?? DateTime.UtcNow.Date.AddDays(-90);

            // 'to' date should be fully inclusive
            // Example: to=2026-09-06 means until 2026-09-06 23:59:59.999...
            var toDateExclusive = to?.Date.AddDays(1)
                                ?? DateTime.UtcNow.Date.AddDays(1);

            if (fromDate >= toDateExclusive)
            {
                return BadRequest(new
                {
                    message = "From date must be earlier than To date."
                });
            }

            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<RSFLY>();

            var rows = await db.FlightBookings
                .AsNoTracking()
                .Where(fb =>
                    fb.DelInd == false &&
                    fb.BookingDate >= fromDate &&
                    fb.BookingDate < toDateExclusive &&
                    fb.Agent == "Trabber"
                )
                .OrderByDescending(fb => fb.BookingDate)
                .Select(fb => new
                {
                    OrderId = fb.BookingId,
                    OrderRef = fb.BookingNumber,
                    BookingDateTime = fb.BookingDate,
                    TravelDate = fb.DepartDate,
                    BookingAmount = fb.Gross,

                    Agent = fb.Agent,
                    TotalPax = fb.Adult + fb.Child + fb.Infant,

                    Currency = "USD",
                    CommissionAmount = fb.Gross * 0.05m,

                    SubId = fb.SearchId,
                    CustomerCountry = fb.BillCountry ?? fb.PhoneCountry,
                    BookingStatus = fb.IsStatus
                })
                .ToListAsync();

            return Ok(rows);
        }

        private string GetCampaignId(string source)
        {
            if (!Enum.TryParse<FlightSearchSource>(source, true, out var searchSource))
                return "Rush_FS";

            return searchSource switch
            {
                FlightSearchSource.FS => "Rush_FS",
                FlightSearchSource.Trabber => "Rush_Trabber",
                FlightSearchSource.FC => "Rush_FC",
                _ => "Rush_FS"
            };
        }
    }
}