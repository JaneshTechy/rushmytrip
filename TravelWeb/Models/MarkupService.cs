using DocumentFormat.OpenXml.Drawing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;
using System.Linq;
using System.Runtime.Caching;
using TravelWeb.Admin.Models;
using TravelWeb.Controllers;
using TravelWeb.Datamodel;
using TravelWeb.Helper;
using TravelWeb.Utilities;
using static TravelWeb.Models.RouteControlService;


namespace TravelWeb.Models
{
    public class MarkupService
    {
        private const string CACHE_KEY = "MARKUP_RULES_CACHE_V1";
        private static readonly ObjectCache Cache = System.Runtime.Caching.MemoryCache.Default;
        private static readonly TimeSpan CACHE_DURATION = TimeSpan.FromMinutes(30); // tune as needed
        private readonly IConfiguration _configuration;
        public MarkupService(IConfiguration configuration)
        {
            _configuration = configuration;

           
        }
        // Call to load/refresh rules from DB (implement DB fetch as you use EF/Dapper)
        public static List<MarkupRuleDto> LoadRulesFromDb(FlightsResult flightsResult, string website)
        {
            try
            {

                //var allRules = AplicationHelper.LoadMarkupRulesFromFile();
                //AplicationHelper.SaveMarkupRulesToFile(allRules);


                using (var db = new RSFLY())
                {
                    var depDate = flightsResult.FlightsSearch.DepDate;
                    var retDate = flightsResult.FlightsSearch.RetDate;
                    bool isRoundTrip = flightsResult.FlightsSearch.FlightWay == FlightWay.RoundTrip;

                    IQueryable<MarkupRule> query = db.MarkupRules.AsNoTracking();


                    if (isRoundTrip)
                    {
                        query = query.Where(r => (r.Status == true && r.CampId == website));
                        //(r.TravelDateFrom <= depDate && r.TravelDateTo >= depDate) ||
                        //(r.TravelDateFrom <= retDate && r.TravelDateTo >= retDate));
                    }
                    else
                    {
                        query = query.Where(r => (r.Status == true && r.CampId == website));
                        //query = query.Where(r =>
                        //    r.TravelDateFrom <= depDate && r.TravelDateTo >= depDate);
                    }

                    var activeRules = query.ToList();


                    var parsedList = new List<MarkupRuleDto>();
                    foreach (var row in activeRules)
                    {
                        parsedList.Add(ParseRuleFromEntity(row));
                    }

                    return parsedList;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading MarkupRules from DB: " + ex.Message);
                return new List<MarkupRuleDto>();
            }
        }

        public static MarkupRuleDto ParseRuleFromEntity(MarkupRule row)
        {
            return new MarkupRuleDto
            {
                RuleId = row.RuleId,
                RuleName = row.RuleName,
                Origins = ParseCsvToSet(row.Origin),
                Destinations = ParseCsvToSet(row.Destination),
                Airlines = ParseCsvToSet(row.Airline),
                CabinClasses = ParseCsvToSet(row.CabinClass),
                JourneyType = string.IsNullOrWhiteSpace(row.JourneyType) ? null : row.JourneyType.Trim().ToUpper(),
                MarkType = string.IsNullOrWhiteSpace(row.MarkType) ? null : row.MarkType.Trim().ToUpper(),
                MarkUp = row.MarkUp ?? 0,
                Status = row.Status ?? false,
                MinDays = row.MinDays,
                MaxDays = row.MaxDays,
                AdvanceDays = row.AdvanceDays,
                MarkApplicable2 = row.MarkApplicable2,
                RAirlines = ParseCsvToSet(row.Rairline),
                PAirlines = ParseCsvToSet(row.Pairline),
                Stops = row.Stops,
                RestrictedClass = ParseCsvToSet(row.RestrictedClass),
                MinimumAmountRange = ParseAmountRange(row.MinimumAmount),
                PaxCount = row.PaxCount,
                PaxType = string.IsNullOrWhiteSpace(row.PaxType) ? "ANY" : row.PaxType.Trim().ToUpper(),
                TravelDateFrom = row.TravelDateFrom,
                TravelDateTo = row.TravelDateTo,
                BlockODateFrom = row.BlockOdateFrom,
                BlockODateTo = row.BlockOdateTo,
                UpdatedBy = row.UpdateBy
            };
        }

        private static MarkupRuleDto ParseRuleFromRow(dynamic row)
        {
            // convert dynamic DB row to MarkupRuleDto with parsing
            var dto = new MarkupRuleDto
            {
                RuleId = (int)row.RuleId,
                RuleName = (string)row.RuleName,
                Origins = ParseCsvToSet((string)row.Origin),
                Destinations = ParseCsvToSet((string)row.Destination),
                Airlines = ParseCsvToSet((string)row.Airline),
                CabinClasses = ParseCsvToSet((string)row.CabinClass),
                JourneyType = string.IsNullOrWhiteSpace((string)row.JourneyType) ? null : ((string)row.JourneyType).Trim().ToUpper(),
                MarkType = string.IsNullOrWhiteSpace((string)row.MarkType) ? null : ((string)row.MarkType).Trim().ToUpper(),
                MarkUp = row.MarkUp == null ? 0 : (decimal)row.MarkUp,
                Status = row.Status == null ? false : (bool)row.Status,
                MinDays = row.MinDays as int?,
                MaxDays = row.MaxDays as int?,
                AdvanceDays = row.AdvanceDays as int?,
                MarkApplicable2 = row.MarkApplicable2 as decimal?,
                RAirlines = ParseCsvToSet((string)row.RAirline),
                PAirlines = ParseCsvToSet((string)row.PAirline),
                Stops = row.Stops as int?,
                RestrictedClass = ParseCsvToSet((string)row.RestrictedClass),
                MinimumAmountRange = ParseAmountRange((string)row.MinimumAmount),
                PaxCount = row.PaxCount as int?,
                PaxType = string.IsNullOrWhiteSpace((string)row.PaxType) ? "ANY" : ((string)row.PaxType).Trim().ToUpper(),
                TravelDateFrom = row.TravelDateFrom as DateTime?,
                TravelDateTo = row.TravelDateTo as DateTime?,
                BlockODateFrom = row.BlockODateFrom as DateTime?,
                BlockODateTo = row.BlockODateTo as DateTime?,
                UpdatedBy = row.UpdateBy
            };
            return dto;
        }

        private static HashSet<string> ParseCsvToSet(string input)
        {
            var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (string.IsNullOrWhiteSpace(input)) return set;
            var parts = input.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                             .Select(s => s.Trim().ToUpperInvariant());
            foreach (var p in parts) set.Add(p);
            return set;
        }

        private static (decimal? min, decimal? max) ParseAmountRange(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return (null, null);
            // Accept formats: "200-400", "200", ">=200" (basic)
            input = input.Trim();
            if (input.Contains("-"))
            {
                var tok = input.Split('-');
                if (decimal.TryParse(tok[0], out var a) && decimal.TryParse(tok[1], out var b))
                    return (a, b);
            }
            if (decimal.TryParse(input, out var single))
                return (single, single);
            return (null, null);
        }
        public static List<MarkupRuleDto> GetRulesV1(FlightsResult flightsResult, string website, string CampID = "Rush")
        {
            CampID = "Rush_FS";
            //AplicationHelper.Log($"CampID: {CampID}");
            string cacheKey = $"MarkupRules_{CampID}_{flightsResult.FlightsSearch.SearchId}";
            var rules = AplicationHelper.LoadMarkupRulesFromFileV1(CampID);
            return rules;
        }
        public static List<MarkupRuleDto> GetRulesV2(FlightsResult flightsResult, string website, string CampID = "Rush")
        {
            CampID = "Rush_FS";
            //AplicationHelper.Log($"CampID: {CampID}");
            string cacheKey = $"MarkupRules_{CampID}_{flightsResult.FlightsSearch.SearchId}";
            var rules = AplicationHelper.LoadMarkupRulesFromFileV3("Rush_Trabber");
            return rules;
        }
        // Public accessor: get cached rules (load if not present)
        public static List<MarkupRuleDto> GetRules(FlightsResult flightsResult, string website , string CampID="Rush")
        {
            //string cacheKey = "MarkupRules_" + flightsResult.FlightsSearch.SearchId;
            //AplicationHelper.Log($"CampID: {CampID}");
            string cacheKey = $"MarkupRules_{CampID}_{flightsResult.FlightsSearch.SearchId}";

            //var cached = Cache.Get(cacheKey) as List<MarkupRuleDto>;
            //if (cached != null) return cached;

            //var cached = Cache.Get(CACHE_KEY) as List<MarkupRuleDto>;
            //if (cached != null) return cached;

            //var rules = LoadRulesFromDb(flightsResult, website);
            //AplicationHelper.SaveMarkupRulesToFile(rules);
            var rules = AplicationHelper.LoadMarkupRulesFromFile(CampID);
            //AplicationHelper.Log($"Rules returned: {rules?.Count}");
            //Cache.Set(CACHE_KEY, rules, new CacheItemPolicy { AbsoluteExpiration = DateTimeOffset.UtcNow.Add(CACHE_DURATION) });
            return rules;
        }

        // Core: apply markup to flights list
        // flightsResult.FlightsList is list of Flights (your existing type)
        //public static void ApplyMarkup(FlightsResult flightsResult, FlightsSearch search)
        //{
        //    var rules = GetRules(flightsResult).ToList();
        //    if (!rules.Any()) return;

        //    foreach (var flight in flightsResult.FlightsList)
        //    {
        //        // Determine flight-level properties used for matching
        //        //if (flight.Airline?.Code == "WN")
        //        //{
        //        //    string Message = "jjhjhjhjhj";
        //        //}
        //        var flightAirline = (flight.Airline?.Code ?? "").ToUpperInvariant();
        //        var cabin = (flight.FlightFare?.MarkupType == null ? (flight.Outbound.FirstOrDefault()?.CabinClass ?? "") : flight.Outbound.FirstOrDefault()?.CabinClass ?? "").ToUpperInvariant();
        //        var stops = flight.Stops; // 0 = direct in your mapping
        //        var travelDate = GetPrimaryTravelDate(flight, search); // helper below
        //        decimal totalAmount = Convert.ToDecimal(flight.TotalCost);

        //        decimal lowestMarkupAmount = decimal.MaxValue;
        //        string appliedMarkupDesc = "";
        //        MarkupRuleDto? selectedRule = null;

        //        //decimal totalMarkupAmount = 0m;
        //        //string appliedMarkupDesc = "";

        //        foreach (var rule in rules)
        //        {
        //            //if (rule.RuleName == "kailash")
        //            //{

        //            //}
        //            //Handle Markup Only for CityCode the Direct match With CityCode

        //            // 1) Origin / Destination match: if rule has origins/dests, check outbound departure/arrival codes
        //            //when both have value orgin ,destination

        //            if ((rule.Origins != null && rule.Origins.Count > 1) && (rule.Destinations != null && rule.Destinations.Count > 1))
        //            {
        //                bool originMatches = !rule.Origins.Any() || (flight.Outbound.Any() && rule.Origins.Contains(flight.Outbound.First().FromAirport.Code.ToUpperInvariant()));
        //                bool destMatches = !rule.Destinations.Any() || (flight.Outbound.Any() && rule.Destinations.Contains(flight.Outbound.Last().ToAirport.Code.ToUpperInvariant()));
        //                if (!originMatches || !destMatches) continue;
        //            }


        //            if (rule.TravelDateFrom.HasValue && rule.TravelDateTo.HasValue)
        //            {
        //                DateTime from = rule.TravelDateFrom.Value;
        //                DateTime to = rule.TravelDateTo.Value;

        //                DateTime depDate = search.DepDate.Date;
        //                DateTime? retDate = search.RetDate;

        //                bool depInRange = depDate >= from && depDate <= to;
        //                bool retInRange = retDate.HasValue && retDate.Value.Date >= from && retDate.Value.Date <= to;


        //                if (!depInRange && !retInRange)
        //                    continue; // Rule skip
        //            }

        //            if (rule.Airlines != null && rule.Airlines.Count > 0)
        //            {
        //                if (!rule.Airlines.Contains(flightAirline))
        //                    continue;
        //            }   
        //            // 2) Airline handling: if PAirlines contains current airline -> replace with RAirlines (if present)
        //            //var effectiveAirline = flightAirline;
        //            //if (rule.PAirlines.Contains(effectiveAirline) && rule.RAirlines.Any())
        //            //{
        //            //    // Choose first RAirline as replacement (assumption)
        //            //    effectiveAirline = rule.RAirlines.First();
        //            //}

        //            //if (rule.Airlines.Any() && !rule.Airlines.Contains(effectiveAirline)) continue;

        //            // 3) Cabin class match

        //            if (rule.CabinClasses.Any() && !rule.CabinClasses.Contains(cabin)) continue;

        //            // 4) Restricted class: if rule restricts classes and flight's cabin in restricted list => rule NOT APPLICABLE
        //            if (rule.RestrictedClass.Any() && rule.RestrictedClass.Contains(cabin)) continue;

        //            // 5) JourneyType
        //            if (!string.IsNullOrWhiteSpace(rule.JourneyType))
        //            {
        //                var needed = rule.JourneyType.ToUpperInvariant();
        //                var isRound = (search.FlightWay == FlightWay.RoundTrip);
        //                if (needed == "O" && isRound) continue;
        //                if (needed == "R" && !isRound) continue;
        //                // if other markers, treat conservatively
        //            }

        //            // 6) Travel date range check
        //            if (rule.TravelDateFrom.HasValue && rule.TravelDateTo.HasValue)
        //            {
        //                if (!(travelDate >= rule.TravelDateFrom.Value && travelDate <= rule.TravelDateTo.Value)) continue;
        //            }

        //            // 7) Blocked dates check: if travel date falls inside blocked range => skip rule
        //            if (rule.BlockODateFrom.HasValue && rule.BlockODateTo.HasValue)
        //            {
        //                if (travelDate >= rule.BlockODateFrom.Value && travelDate <= rule.BlockODateTo.Value) continue;
        //            }

        //            // 8) PaxType & PaxCount
        //            if (!string.IsNullOrWhiteSpace(rule.PaxType) && rule.PaxType.ToUpperInvariant() != "ANY")
        //            {
        //                // map search counts to a type: if Adult >0 -> "A", if Child>0 -> "C", if Infant>0 -> "I"
        //                string reqType = rule.PaxType.ToUpperInvariant();
        //                bool typeOk = false;
        //                if (reqType == "A" && search.Adult > 0) typeOk = true;
        //                if (reqType == "C" && search.Child > 0) typeOk = true;
        //                if (reqType == "I" && search.Infant > 0) typeOk = true;
        //                if (!typeOk) continue;
        //            }
        //            int totalPax = search.Adult + search.Child + search.Infant;

        //            if (rule.PaxCount.HasValue && rule.PaxCount.Value > 0)
        //            {
        //                // Only apply if total passenger count matches the rule
        //                if (totalPax != rule.PaxCount.Value)
        //                    continue;
        //            }

        //            // 9) MinimumAmount range
        //            if (rule.MinimumAmountRange.min.HasValue)
        //            {
        //                var min = rule.MinimumAmountRange.min.Value;
        //                var max = rule.MinimumAmountRange.max ?? decimal.MaxValue;
        //                if (!(totalAmount >= min && totalAmount <= max)) continue;
        //            }

        //            // 10) Stops
        //            if (rule.Stops.HasValue&& rule.Stops.Value>0)
        //            {
        //                if (rule.Stops.Value != stops) continue;
        //            }

        //            // 11) MinDays / MaxDays / AdvanceDays (days between today and travel date)
        //            //var daysToTravel = (int?)null;
        //            //if (search != null && travelDate != DateTime.MinValue)
        //            //{
        //            //    daysToTravel = (int)((travelDate - DateTime.UtcNow.Date).TotalDays);
        //            //}
        //            //if (daysToTravel.HasValue)
        //            //{
        //            //    if (rule.MinDays.HasValue && rule.MinDays.Value > 0 && daysToTravel < rule.MinDays.Value)
        //            //        continue;

        //            //    // ✅ MaxDays check
        //            //    if (rule.MaxDays.HasValue && rule.MaxDays.Value > 0 && daysToTravel > rule.MaxDays.Value)
        //            //        continue;

        //            //    // ✅ AdvanceDays check (days before travel)
        //            //    if (rule.AdvanceDays.HasValue && rule.AdvanceDays.Value > 0 && daysToTravel < rule.AdvanceDays.Value)
        //            //        continue;
        //            //}

        //            if (rule.TravelDateFrom.HasValue && rule.TravelDateTo.HasValue)
        //            {
        //                // Calculate min/max offset logic
        //                int minOffset = rule.MinDays.HasValue ? rule.MinDays.Value : 0;
        //                int maxOffset = rule.MaxDays.HasValue ? rule.MaxDays.Value : 0;
        //                int advanceOffset = rule.AdvanceDays.HasValue ? rule.AdvanceDays.Value : 0;

        //                // Apply MinDays / MaxDays / AdvanceDays
        //                DateTime effectiveStart = rule.TravelDateFrom.Value.AddDays(minOffset);
        //                DateTime effectiveEnd = rule.TravelDateTo.Value;

        //                if (maxOffset > 0)
        //                    effectiveEnd = rule.TravelDateFrom.Value.AddDays(maxOffset);

        //                // Apply AdvanceDays on top of that
        //                if (advanceOffset > 0)
        //                    effectiveEnd = effectiveEnd.AddDays(advanceOffset);

        //                // Now check user travel date
        //                if (travelDate < effectiveStart || travelDate > effectiveEnd)
        //                {
        //                    // Not in valid range → skip
        //                    continue;
        //                }
        //            }


        //            decimal markupThisRule = 0m;

        //            if (rule.MarkType == "P")
        //                markupThisRule = Math.Round(totalAmount * (rule.MarkUp / 100m), 2);
        //            else if (rule.MarkType == "A" || rule.MarkType == "F")
        //                markupThisRule = Convert.ToDecimal(rule.MarkUp);

        //            if (rule.MarkApplicable2.HasValue && rule.MarkApplicable2.Value > 0)
        //                markupThisRule += rule.MarkApplicable2.Value;


        //            if (markupThisRule < lowestMarkupAmount)
        //            {
        //                lowestMarkupAmount = markupThisRule;
        //                selectedRule = rule;
        //            }



        //            // Accumulate to total markup
        //            //totalMarkupAmount += markupThisRule;

        //            // If you want only first-match, uncomment below:
        //            // break;
        //        } // end foreach rule

        //        // finally apply markup to flight fares
        //        if (lowestMarkupAmount != decimal.MaxValue && selectedRule != null)
        //        {
        //            flight.FlightFare ??= new FlightsFare();
        //            flight.FlightFare.AdultMarkup = (double)lowestMarkupAmount;
        //            flight.FlightFare.MarkupValue = lowestMarkupAmount.ToString("0.##", CultureInfo.InvariantCulture);
        //            flight.FlightFare.MarkupType = "Applied";
        //            flight.TotalCost = Math.Round(flight.TotalCost + (double)lowestMarkupAmount, 2);
        //        }
        //        //if (flight.FlightFare == null) flight.FlightFare = new FlightsFare();
        //        //flight.FlightFare.AdultMarkup = (double)totalMarkupAmount;
        //        //flight.FlightFare.MarkupValue = totalMarkupAmount.ToString("0.##", CultureInfo.InvariantCulture);
        //        //flight.FlightFare.MarkupType = totalMarkupAmount > 0 ? "Applied" : "None";
        //        //// update total cost
        //        //flight.TotalCost = Math.Round(flight.TotalCost + (double)totalMarkupAmount, 2);

        //        // optional: attach description
        //        // assuming Flights has a Notes or Meta property - if not, you can add one
        //        // flight.Meta = appliedMarkupDesc;
        //    }
        //}
        private static decimal CalculateMarkup(MarkupRuleDto rule, decimal basePrice)
        {
            decimal price;

            switch (rule.MarkType?.ToUpper())
            {
                case "F":
                    price = rule.MarkUp;
                    break;

                case "P":
                    price = basePrice + (basePrice * (rule.MarkUp / 100m));
                    break;

                case "A":
                    price = basePrice + rule.MarkUp;
                    break;

                default:
                    price = basePrice;
                    break;
            }

            if (rule.MarkApplicable2.HasValue)
                price += rule.MarkApplicable2.Value;

            // return ONLY markup difference (added on top of base)
            return price - basePrice;
        }



        public static void ApplyMarkup(FlightsResult flightsResult, FlightsSearch search, string website )
        {

            //var jsonserach = JsonConvert.SerializeObject(search);
            if (flightsResult == null || flightsResult.FlightsList == null || !flightsResult.FlightsList.Any()) return;
            if (search == null) return;

            var allRules = GetRules(flightsResult, website) ?? new List<MarkupRuleDto>();
            if (!allRules.Any()) return;
            //var json = JsonConvert.SerializeObject(allRules);
            var locationMatchedRules = allRules.Where(rule =>
            {
                bool hasOrigins = rule.Origins != null && rule.Origins.Any();
                bool hasDestinations = rule.Destinations != null && rule.Destinations.Any();

                bool originMatches = true;
                if (hasOrigins)
                {
                    originMatches = rule.Origins.Any(rv => MatchTokenAgainstSearchEndpoint(rv, search, true));
                }

                bool destMatches = true;
                if (hasDestinations)
                {
                    destMatches = rule.Destinations.Any(rv => MatchTokenAgainstSearchEndpoint(rv, search, false));
                }

                return originMatches && destMatches;
            }).ToList();

            if (!locationMatchedRules.Any()) return;

            foreach (var flight in flightsResult.FlightsList)
            {
                var flightAirline = (flight.Airline?.Code ?? "").ToUpperInvariant();
                //var cabin = (flight.FlightFare?.MarkupType == null
                //             ? (flight.Outbound?.FirstOrDefault()?.CabinClass ?? "")
                //             : flight.Outbound?.FirstOrDefault()?.CabinClass ?? "").ToUpperInvariant();

                var cabin = "";
                if (search.FlightClass == FlightClass.Economy)
                    cabin = "Y";
                var stops = flight.Stops;
                DateTime travelDate = GetPrimaryTravelDate(flight, search);
                decimal baseAmount = Convert.ToDecimal(flight.TotalCost);

                decimal lowestMarkupAmount = decimal.MaxValue;
                MarkupRuleDto selectedRule = null;

                foreach (var rule in locationMatchedRules)
                {
                    if (!rule.Status) continue;

                    // ---------- COMMON FILTERS ----------
                    // Airline
                    if (rule.Airlines != null && rule.Airlines.Any())
                    {
                        if (!rule.Airlines.Contains(flightAirline))
                            continue;
                    }

                    // Cabin classes allowed
                    if (rule.CabinClasses != null && rule.CabinClasses.Any() &&
                        !rule.CabinClasses.Contains(cabin))
                        continue;

                    // Restricted class
                    if (rule.RestrictedClass != null && rule.RestrictedClass.Contains(cabin))
                        continue;

                    // Journey type (O=OneWay, R=RoundTrip)
                    if (!string.IsNullOrWhiteSpace(rule.JourneyType))
                    {
                        bool isRoundTrip = search.FlightWay == FlightWay.RoundTrip;
                        if (rule.JourneyType == "O" && isRoundTrip) continue;
                        if (rule.JourneyType == "R" && !isRoundTrip) continue;
                    }

                    // Travel date range
                    if (rule.TravelDateFrom.HasValue && travelDate < rule.TravelDateFrom.Value) continue;
                    if (rule.TravelDateTo.HasValue && travelDate > rule.TravelDateTo.Value) continue;

                    // Block date range (both ends required by original logic)
                    if (rule.BlockODateFrom.HasValue && rule.BlockODateTo.HasValue)
                    {
                        if (!(travelDate >= rule.BlockODateFrom.Value && travelDate <= rule.BlockODateTo.Value))
                            continue;
                    }

                    // Pax type
                    if (!string.IsNullOrWhiteSpace(rule.PaxType) && rule.PaxType.ToUpper() != "ANY")
                    {
                        if (!(rule.PaxType == "A" && search.Adult > 0 ||
                              rule.PaxType == "C" && search.Child > 0 ||
                              rule.PaxType == "I" && search.Infant > 0))
                            continue;
                    }

                    // Pax count
                    int totalPax = search.Adult + search.Child + search.Infant;
                    if (rule.PaxCount.HasValue && totalPax < rule.PaxCount.Value) continue;

                    // Price / minimum amount range
                    // Support two variants: MinimumAmountRange (object with min/max) OR MinimumAmount field (legacy)
                    //if (rule.MinimumAmountRange != null && rule.MinimumAmountRange.min.HasValue)
                    //{
                    //    decimal min = rule.MinimumAmountRange.min.Value;
                    //    decimal max = rule.MinimumAmountRange.max ?? decimal.MaxValue;
                    //    if (baseAmount < min || baseAmount > max) continue;
                    //}
                    //else if (rule.MinimumAmount.HasValue)
                    //{
                    //    // if your DTO uses MinimumAmount instead of an object
                    //    decimal min = rule.MinimumAmount.Value;
                    //    if (baseAmount < min) continue;
                    //}

                    // Stops
                    if (rule.Stops.HasValue)
                    {
                        int required = rule.Stops.Value;
                        if (required >= 0)
                        {
                            if (stops != required) continue;
                        }
                        else if (required == -1)
                        {
                            // allow any stops
                        }
                    }

                    
                    // Advance / Min / Max days relative to today
                    if (travelDate != DateTime.MinValue)
                    {
                        int daysToTravel = (travelDate - DateTime.UtcNow.Date).Days;

                        if (rule.MinDays.GetValueOrDefault() > 0 && daysToTravel < rule.MinDays.Value) continue;
                        if (rule.MaxDays.GetValueOrDefault() > 0 && daysToTravel > rule.MaxDays.Value) continue;
                        if (rule.AdvanceDays.GetValueOrDefault() > 0 && daysToTravel < rule.AdvanceDays.Value) continue;
                    }


                    // Calculate markup using rule (you can replace with your own CalculateMarkup if you have a richer version)
                    decimal markup = CalculateMarkup(rule, baseAmount);

                    // Choose lowest markup (business rule from your earlier code)
                    if (markup < lowestMarkupAmount)
                    {
                        lowestMarkupAmount = markup;
                        selectedRule = rule;
                    }
                } // end rules loop

                // Apply markup if found
                if (lowestMarkupAmount != decimal.MaxValue && selectedRule != null)
                {
                    //var jsonsesrach = JsonConvert.SerializeObject(flight);
                    string paxType = selectedRule.PaxType?.ToUpper();

                    flight.FlightFare ??= new FlightsFare();
                    if (string.IsNullOrEmpty(paxType) || paxType == "ANY")
                    {
                        // Apply to ALL pax
                        flight.FlightFare.AdultMarkup = (double)lowestMarkupAmount;

                        if (search.Child > 0)
                            flight.FlightFare.ChildMarkup = (double)lowestMarkupAmount;

                        if (search.Infant > 0)
                            flight.FlightFare.InfantMarkup = (double)lowestMarkupAmount;
                    }
                    else if (paxType == "A")
                    {
                        flight.FlightFare.AdultMarkup = (double)lowestMarkupAmount;
                    }
                    else if (paxType == "C")
                    {
                        flight.FlightFare.ChildMarkup = (double)lowestMarkupAmount;
                    }
                    else if (paxType == "I")
                    {
                        flight.FlightFare.InfantMarkup = (double)lowestMarkupAmount;
                    }

                    flight.FlightFare.AdultMarkup = (double)lowestMarkupAmount;
                    flight.FlightFare.MarkupValue = lowestMarkupAmount.ToString("0.##");
                    flight.FlightFare.MarkupType = selectedRule.MarkType;
                    flight.FlightFare.MarkupRuleName = selectedRule.RuleName;
                    flight.FlightFare.UpdatedBy = selectedRule.UpdatedBy;
                    flight.TotalCost = Math.Round(flight.TotalCost + (double)lowestMarkupAmount, 2);
                }
            } // foreach flight
        }


        public static void ApplyMarkupNew(FlightsResult flightsResult, FlightsSearch search, string website, string CampId = "Rush_FS")
        {
            if (flightsResult?.FlightsList == null || !flightsResult.FlightsList.Any()) return;
            if (search == null) return;

            if(flightsResult.FlightsSearch.SiteCode== "FareCompare")
            {
                CampId = "Rush_FC";
            }
            var allRules = GetRules(flightsResult, website, CampId) ?? new List<MarkupRuleDto>();
            //var json = JsonConvert.SerializeObject(allRules);
            if (!allRules.Any()) return;

            var locationMatchedRules = allRules.Where(rule =>
            {
                bool hasOrigins = rule.Origins != null && rule.Origins.Any();
                bool hasDestinations = rule.Destinations != null && rule.Destinations.Any();

                bool originMatches = !hasOrigins || rule.Origins.Any(rv => MatchTokenAgainstSearchEndpoint(rv, search, true));
                bool destMatches = !hasDestinations || rule.Destinations.Any(rv => MatchTokenAgainstSearchEndpoint(rv, search, false));

                return originMatches && destMatches;
            }).ToList();

            if (!locationMatchedRules.Any()) return;

            foreach (var flight in flightsResult.FlightsList)
            {
                var flightAirline = (flight.Airline?.Code ?? "").ToUpperInvariant();

                var cabin = search.FlightClass == FlightClass.Economy ? "Y" : "";
                var stops = flight.Stops;
                DateTime travelDate = GetPrimaryTravelDate(flight, search);

                decimal lowestMarkupAmount = decimal.MaxValue;
                MarkupRuleDto selectedRule = null;
                int bestScore = -1;

                foreach (var rule in locationMatchedRules)
                {
                    if (!rule.Status) continue;

                    if (rule.Airlines?.Any() == true && !rule.Airlines.Contains(flightAirline)) continue;
                    if (rule.CabinClasses?.Any() == true && !rule.CabinClasses.Contains(cabin)) continue;
                    if (rule.RestrictedClass?.Contains(cabin) == true) continue;

                    bool isRoundTrip = search.FlightWay == FlightWay.RoundTrip;
                    if (rule.JourneyType == "O" && isRoundTrip) continue;
                    if (rule.JourneyType == "R" && !isRoundTrip) continue;

                    if (rule.TravelDateFrom.HasValue && travelDate < rule.TravelDateFrom.Value) continue;
                    if (rule.TravelDateTo.HasValue && travelDate > rule.TravelDateTo.Value) continue;

                    if (rule.BlockODateFrom.HasValue && rule.BlockODateTo.HasValue)
                    {
                        if (!(travelDate >= rule.BlockODateFrom.Value && travelDate <= rule.BlockODateTo.Value))
                            continue;
                    }

                    if (!string.IsNullOrWhiteSpace(rule.PaxType) && rule.PaxType.ToUpper() != "ANY")
                    {
                        if (!(rule.PaxType == "A" && search.Adult > 0 ||
                              rule.PaxType == "C" && search.Child > 0 ||
                              rule.PaxType == "I" && search.Infant > 0))
                            continue;
                    }

                    int totalPax = search.Adult + search.Child + search.Infant;
                    if (rule.PaxCount.HasValue && totalPax < rule.PaxCount.Value) continue;

                    if (travelDate != DateTime.MinValue)
                    {
                        int daysToTravel = (travelDate - DateTime.UtcNow.Date).Days;

                        if (rule.MinDays.GetValueOrDefault() > 0 && daysToTravel < rule.MinDays.Value) continue;
                        if (rule.MaxDays.GetValueOrDefault() > 0 && daysToTravel > rule.MaxDays.Value) continue;
                        if (rule.AdvanceDays.GetValueOrDefault() > 0 && daysToTravel < rule.AdvanceDays.Value) continue;
                    }
                    int score = 0;

                    if (rule.Airlines?.Any() == true) score += 40;
                    if (rule.CabinClasses?.Any() == true) score += 25;
                    if (!string.IsNullOrEmpty(rule.JourneyType)) score += 20;
                    if (rule.TravelDateFrom.HasValue || rule.TravelDateTo.HasValue) score += 15;
                    if (!string.IsNullOrEmpty(rule.PaxType) && rule.PaxType != "ANY") score += 10;
                    if (rule.PaxCount.HasValue) score += 5;

                    if (rule.Stops.HasValue)
                    {
                        if (rule.Stops.Value != -1)
                        {
                            if (rule.Stops.Value != flight.Stops)
                                continue;

                            score += 30;
                        }
                        else
                        {
                            score += 5;
                        }
                    }


                    var ff = flight.FlightFare ?? new FlightsFare();

                    decimal adultBase = (decimal)ff.AdultFare + (decimal)ff.AdultTax;
                    decimal childBase = (decimal)ff.ChildFare + (decimal)ff.ChildTax;
                    decimal infantBase = (decimal)ff.InfantFare + (decimal)ff.InfantTax;

                    int adult = search.Adult;
                    int child = search.Child;
                    int infant = search.Infant;

                    decimal totalBaseBefore =
                        (adultBase * adult) +
                        (childBase * child) +
                        (infantBase * infant);

                    if (rule.MinimumAmountRange.min.HasValue || rule.MinimumAmountRange.max.HasValue)
                    {
                        decimal total = totalBaseBefore;

                        if (rule.MinimumAmountRange.min.HasValue && total < rule.MinimumAmountRange.min.Value)
                            continue;

                        if (rule.MinimumAmountRange.max.HasValue && total > rule.MinimumAmountRange.max.Value)
                            continue;
                    }

                    decimal adultFinalBase = ApplyMarkupOnBase(adultBase, rule);
                    decimal childFinalBase = ApplyMarkupOnBase(childBase, rule);
                    decimal infantFinalBase = ApplyMarkupOnBase(infantBase, rule);

                    decimal totalBaseAfter =
    (adult > 0 ? adultFinalBase * adult : 0) +
    (child > 0 ? childFinalBase * child : 0) +
    (infant > 0 ? infantFinalBase * infant : 0);



                    decimal totalMarkup = totalBaseAfter - totalBaseBefore;

                    if (score > bestScore ||
                       (score == bestScore && totalMarkup < lowestMarkupAmount))
                    {
                        bestScore = score;
                        lowestMarkupAmount = totalMarkup;
                        selectedRule = rule;
                    }
                }

                // ================= APPLY IF RULE FOUND =================
                if (lowestMarkupAmount != decimal.MaxValue && selectedRule != null)
                {
                    var ff = flight.FlightFare ??= new FlightsFare();

                    int adult = search.Adult;
                    int child = search.Child;
                    int infant = search.Infant;

                    decimal adultBase = (decimal)ff.AdultFare + (decimal)ff.AdultTax;
                    decimal childBase = (decimal)ff.ChildFare + (decimal)ff.ChildTax;
                    decimal infantBase = (decimal)ff.InfantFare + (decimal)ff.InfantTax;

                    decimal adultFinalBase = ApplyMarkupOnBase(adultBase, selectedRule);
                    decimal childFinalBase = ApplyMarkupOnBase(childBase, selectedRule);
                    decimal infantFinalBase = ApplyMarkupOnBase(infantBase, selectedRule);





                    ff.AdultMarkupafterTax = CalculateTaxMarkup((decimal)ff.AdultTax, selectedRule);
                    ff.ChildMarkupafterTax = CalculateTaxMarkup((decimal)ff.ChildTax, selectedRule);
                    ff.InfantMarkupafterTax = CalculateTaxMarkup((decimal)ff.InfantTax, selectedRule);
                    
                   
                    ff.AtolCharge = 20.00;
                    ff.AdultMarkup = (double)((adultFinalBase - adultBase) * adult);
                    ff.AdultMarkup = (ff.AdultMarkup / adult);
                    ff.ChildMarkup = (double)((childFinalBase - childBase) * child);
                    ff.ChildMarkup = (ff.ChildMarkup / adult);

                    ff.InfantMarkup = (double)((infantFinalBase - infantBase) * infant);
                    ff.InfantMarkup = (ff.InfantMarkup / adult);

                    ff.MarkupValue = lowestMarkupAmount.ToString("0.##");
                    ff.MarkupType = selectedRule.MarkType;
                    ff.MarkupRuleName = selectedRule.RuleName;
                    ff.UpdatedBy = selectedRule.UpdatedBy;
                    ff.CampId = CampId;
                        decimal totalTax =
                        ((decimal)ff.AdultTax * adult) +
                        ((decimal)ff.ChildTax * child) +
                        ((decimal)ff.InfantTax * infant);

                    decimal newTotal =
                        (adultFinalBase * adult) +
                        (childFinalBase * child) +
                        (infantFinalBase * infant) /*+
                        totalTax*/;
                    if (ff.AtolCharge > 0)
                    {
                        newTotal += (decimal)ff.AtolCharge;
                    }
                    flight.TotalCost = Math.Round((double)newTotal, 2);
                }
            }
        }


        public static void ApplyMarkupV2(FlightsResult flightsResult,FlightsSearch search,string website,string CampId = "Rush_FS")
        {
            if (flightsResult?.FlightsList == null || !flightsResult.FlightsList.Any()) return;
            if (search == null) return;

            var allRules = GetRules(flightsResult, website, CampId) ?? new List<MarkupRuleDto>();
            if (!allRules.Any()) return;

            var locationMatchedRules = allRules.Where(rule =>
            {
                bool hasOrigins = rule.Origins != null && rule.Origins.Any();
                bool hasDestinations = rule.Destinations != null && rule.Destinations.Any();

                bool originMatches = !hasOrigins || rule.Origins.Any(rv => MatchTokenAgainstSearchEndpoint(rv, search, true));
                bool destMatches = !hasDestinations || rule.Destinations.Any(rv => MatchTokenAgainstSearchEndpoint(rv, search, false));

                return originMatches && destMatches;
            }).ToList();

            if (!locationMatchedRules.Any()) return;

            foreach (var flight in flightsResult.FlightsList)
            {
                var flightAirline = (flight.Airline?.Code ?? "").ToUpperInvariant();
                var cabin = search.FlightClass == FlightClass.Economy ? "Y" : "";
                DateTime travelDate = GetPrimaryTravelDate(flight, search);

                int flightStopBucket =
                    flight.Stops == 0 ? 0 :
                    flight.Stops == 1 ? 1 :
                    2;

                decimal lowestMarkupAmount = decimal.MaxValue;
                MarkupRuleDto selectedRule = null;

                int bestStopPriority = -1;
                int bestAirlinePriority = -1;
                int bestScore = -1;

                foreach (var rule in locationMatchedRules)
                {
                    if (!rule.Status) continue;

                    // ---------- CABIN ----------
                    if (rule.CabinClasses?.Any() == true && !rule.CabinClasses.Contains(cabin)) continue;
                    if (rule.RestrictedClass?.Contains(cabin) == true) continue;

                    // ---------- AIRLINE ----------
                    int airlinePriority;
                    if (rule.Airlines?.Any() == true)
                    {
                        if (!rule.Airlines.Contains(flightAirline))
                            continue;

                        airlinePriority = 2; // specific airline
                    }
                    else
                    {
                        airlinePriority = 1; // generic
                    }

                    // ---------- JOURNEY TYPE ----------
                    bool isRoundTrip = search.FlightWay == FlightWay.RoundTrip;
                    if (rule.JourneyType == "O" && isRoundTrip) continue;
                    if (rule.JourneyType == "R" && !isRoundTrip) continue;

                    // ---------- TRAVEL DATE ----------
                    if (rule.TravelDateFrom.HasValue && travelDate < rule.TravelDateFrom.Value) continue;
                    if (rule.TravelDateTo.HasValue && travelDate > rule.TravelDateTo.Value) continue;

                    if (rule.BlockODateFrom.HasValue && rule.BlockODateTo.HasValue)
                    {
                        if (travelDate >= rule.BlockODateFrom.Value &&
                            travelDate <= rule.BlockODateTo.Value)
                            continue;
                    }

                    // ---------- PAX ----------
                    if (!string.IsNullOrWhiteSpace(rule.PaxType) && rule.PaxType != "ANY")
                    {
                        if (!(rule.PaxType == "A" && search.Adult > 0 ||
                              rule.PaxType == "C" && search.Child > 0 ||
                              rule.PaxType == "I" && search.Infant > 0))
                            continue;
                    }

                    int totalPax = search.Adult + search.Child + search.Infant;
                    if (rule.PaxCount.HasValue && totalPax < rule.PaxCount.Value) continue;

                    // ---------- ADVANCE / MIN / MAX DAYS ----------
                    if (travelDate != DateTime.MinValue)
                    {
                        int daysToTravel = (travelDate - DateTime.UtcNow.Date).Days;

                        if (rule.MinDays > 0 && daysToTravel < rule.MinDays) continue;
                        if (rule.MaxDays > 0 && daysToTravel > rule.MaxDays) continue;
                        if (rule.AdvanceDays > 0 && daysToTravel < rule.AdvanceDays) continue;
                    }

                    // ---------- STOP LOGIC (HARD FILTER + PRIORITY) ----------
                    int stopPriority;

                    if (rule.Stops.HasValue)
                    {
                        if (rule.Stops.Value == -1)
                        {
                            stopPriority = 1; // ANY stop
                        }
                        else
                        {
                            int ruleStopBucket =
                                rule.Stops.Value == 0 ? 0 :
                                rule.Stops.Value == 1 ? 1 :
                                2;

                            if (ruleStopBucket != flightStopBucket)
                                continue; 

                            stopPriority = 3; 
                        }
                    }
                    else
                    {
                        stopPriority = 2; 
                    }

                    var ff = flight.FlightFare ?? new FlightsFare();

                    int adult = search.Adult;
                    int child = search.Child;
                    int infant = search.Infant;

                    // ---------- GROSS TOTAL (FOR MINIMUM AMOUNT) ----------
                    decimal grossTotal =
                        ((decimal)ff.AdultFare + (decimal)ff.AdultTax) * adult +
                        ((decimal)ff.ChildFare + (decimal)ff.ChildTax) * child +
                        ((decimal)ff.InfantFare + (decimal)ff.InfantTax) * infant;

                    // 🚨 HARD MINIMUM AMOUNT FILTER
                    if (rule.MinimumAmountRange.min.HasValue &&
                        grossTotal < rule.MinimumAmountRange.min.Value)
                        continue;

                    if (rule.MinimumAmountRange.max.HasValue &&
                        grossTotal > rule.MinimumAmountRange.max.Value)
                        continue;

                    // ---------- SCORE ----------
                    int score = 0;
                    if (rule.Airlines?.Any() == true) score += 40;
                    if (rule.CabinClasses?.Any() == true) score += 25;
                    if (!string.IsNullOrEmpty(rule.JourneyType)) score += 20;
                    if (rule.TravelDateFrom.HasValue || rule.TravelDateTo.HasValue) score += 15;
                    if (!string.IsNullOrEmpty(rule.PaxType) && rule.PaxType != "ANY") score += 10;
                    if (rule.PaxCount.HasValue) score += 5;

                    // ---------- MARKUP CALC ----------
                    decimal adultBase = (decimal)ff.AdultFare + (decimal)ff.AdultTax;
                    decimal childBase = (decimal)ff.ChildFare + (decimal)ff.ChildTax;
                    decimal infantBase = (decimal)ff.InfantFare + (decimal)ff.InfantTax;

                    decimal adultFinal = ApplyMarkupOnBase(adultBase, rule);
                    decimal childFinal = ApplyMarkupOnBase(childBase, rule);
                    decimal infantFinal = ApplyMarkupOnBase(infantBase, rule);

                    decimal totalBefore = adultBase * adult + childBase * child + infantBase * infant;
                    decimal totalAfter = adultFinal * adult + childFinal * child + infantFinal * infant;

                    decimal totalMarkup = totalAfter - totalBefore;

                    // ---------- FINAL RULE SELECTION ----------
                    bool isBetter =
                        stopPriority > bestStopPriority ||
                        (stopPriority == bestStopPriority && airlinePriority > bestAirlinePriority) ||
                        (stopPriority == bestStopPriority && airlinePriority == bestAirlinePriority && score > bestScore) ||
                        (stopPriority == bestStopPriority && airlinePriority == bestAirlinePriority &&
                         score == bestScore && totalMarkup < lowestMarkupAmount);

                    if (isBetter)
                    {
                        bestStopPriority = stopPriority;
                        bestAirlinePriority = airlinePriority;
                        bestScore = score;
                        lowestMarkupAmount = totalMarkup;
                        selectedRule = rule;
                    }
                }

                // ---------- APPLY SELECTED RULE ----------
                if (selectedRule == null) continue;

                var fare = flight.FlightFare ??= new FlightsFare();

                int adt = search.Adult;
                int chd = search.Child;
                int inf = search.Infant;

                decimal adtBase = (decimal)fare.AdultFare + (decimal)fare.AdultTax;
                decimal chdBase = (decimal)fare.ChildFare + (decimal)fare.ChildTax;
                decimal infBase = (decimal)fare.InfantFare + (decimal)fare.InfantTax;

                decimal adtFinal = ApplyMarkupOnBase(adtBase, selectedRule);
                decimal chdFinal = ApplyMarkupOnBase(chdBase, selectedRule);
                decimal infFinal = ApplyMarkupOnBase(infBase, selectedRule);

                fare.AdultMarkup = (double)(adtFinal - adtBase);
                fare.ChildMarkup = (double)(chdFinal - chdBase);
                fare.InfantMarkup = (double)(infFinal - infBase);

                fare.MarkupValue = lowestMarkupAmount.ToString("0.##");
                fare.MarkupType = selectedRule.MarkType;
                fare.MarkupRuleName = selectedRule.RuleName;
                fare.UpdatedBy = selectedRule.UpdatedBy;

                decimal newTotal =
                    adtFinal * adt +
                    chdFinal * chd +
                    infFinal * inf;

                flight.TotalCost = Math.Round((double)newTotal, 2);
            }
        }

        public static void ApplyMarkupV3(
 FlightsResult flightsResult,
 FlightsSearch search,
 string website,
 string CampId = "Rush_FS", decimal Extramarkup = 0)
        {
            try
            {
                if (flightsResult?.FlightsList == null || flightsResult.FlightsList.Count == 0) return;
                if (search == null) return;

                var allRules = GetRulesV1(flightsResult, website, CampId);
                if(search.SourceMedia== "Trabber")
                {
                    allRules = GetRulesV2(flightsResult, website, CampId);
                }
                if (allRules == null || allRules.Count == 0) return;

                var locationMatchedRules = allRules.Where(rule =>
                {
                    if (rule.Origins?.Count > 0 &&
                        !rule.Origins.Any(rv => MatchTokenAgainstSearchEndpoint(rv, search, true)))
                        return false;

                    if (rule.Destinations?.Count > 0 &&
                        !rule.Destinations.Any(rv => MatchTokenAgainstSearchEndpoint(rv, search, false)))
                        return false;

                    return true;
                }).ToList();

                if (locationMatchedRules.Count == 0)
                {
                    flightsResult.FlightsList.Clear();
                    return;
                }
                    
                bool isRoundTrip = search.FlightWay == FlightWay.RoundTrip;
                int adt = search.Adult;
                int chd = search.Child;
                int inf = search.Infant;
                int totalPax = adt + chd + inf;
                string cabin = GetSearchCabinCode(search);
                var today = DateTime.UtcNow.Date;

                var originalFlights = flightsResult.FlightsList;
                var finalFlights = new List<Flights>(originalFlights.Count * 2);
                var duplicateFlights = new List<Flights>();

                foreach (var flight in originalFlights)
                {
                    try
                    {
                        var fare = flight.FlightFare;
                        if (fare == null) continue;

                        string airline = (flight.Airline?.Code ?? "").ToUpperInvariant();
                        DateTime travelDate = GetPrimaryTravelDate(flight, search);

                        int stopBucket = flight.Stops == 0 ? 0 : (flight.Stops == 1 ? 1 : 2);

                        decimal adtBase = (decimal)fare.AdultFare + (decimal)fare.AdultTax;
                        decimal chdBase = (decimal)fare.ChildFare + (decimal)fare.ChildTax;
                        decimal infBase = (decimal)fare.InfantFare + (decimal)fare.InfantTax;

                        decimal grossTotal = adtBase * adt + chdBase * chd + infBase * inf;

                        int daysToTravel = travelDate == DateTime.MinValue
                            ? int.MaxValue
                            : (travelDate - today).Days;

                        MarkupRuleDto selectedRule = null;
                        decimal lowestMarkup = decimal.MaxValue;
                        int bestCabinPriority = -1;
                        int bestStopPriority = -1;
                        int bestAirlinePriority = -1;
                        int bestScore = -1;

                        foreach (var rule in locationMatchedRules)
                        {
                            if (!rule.Status) continue;

                            if (rule.CabinClass != null && rule.CabinClass != "" && !rule.CabinClass.Contains(cabin)) continue;
                            int cabinPriority = (!string.IsNullOrWhiteSpace(rule.CabinClass) && rule.CabinClass.Contains(cabin)) ? 2 : 1;
                            if (rule.RestrictedClass?.Contains(cabin) == true) continue;

                            int airlinePriority = 1;
                            if (rule.Airlines?.Count > 0)
                            {
                                bool directMatch = rule.Airlines.Contains(airline);
                                bool replaceMatch = rule.RAirlines != null && rule.RAirlines.Contains(airline);
                                if (!directMatch && !replaceMatch) continue;
                                airlinePriority = 2;
                            }

                            if (rule.JourneyType == "O" && isRoundTrip) continue;
                            if (rule.JourneyType == "R" && !isRoundTrip) continue;

                            if (rule.TravelDateFrom.HasValue && travelDate < rule.TravelDateFrom.Value) continue;
                            if (rule.TravelDateTo.HasValue && travelDate > rule.TravelDateTo.Value) continue;

                            if (rule.BlockODateFrom.HasValue &&
                                rule.BlockODateTo.HasValue &&
                                travelDate >= rule.BlockODateFrom.Value &&
                                travelDate <= rule.BlockODateTo.Value)
                                continue;

                            if (rule.PaxCount.HasValue && totalPax < rule.PaxCount.Value) continue;

                            if (!string.IsNullOrEmpty(rule.PaxType) && rule.PaxType != "ANY")
                            {
                                if ((rule.PaxType == "A" && adt == 0) ||
                                    (rule.PaxType == "C" && chd == 0) ||
                                    (rule.PaxType == "I" && inf == 0))
                                    continue;
                            }

                            if (rule.MinDays > 0 && daysToTravel < rule.MinDays) continue;
                            if (rule.MaxDays > 0 && daysToTravel > rule.MaxDays) continue;

                            if (rule.AdvanceDays > 0)
                            {
                                if (daysToTravel == int.MaxValue || daysToTravel < 0) continue;
                                if (daysToTravel < rule.AdvanceDays) continue;
                            }

                            int stopPriority;
                            if (rule.Stops.HasValue)
                            {
                                if (rule.Stops.Value == -1)
                                    stopPriority = 1;
                                else
                                {
                                    int ruleBucket = rule.Stops.Value <= 1 ? rule.Stops.Value : 2;
                                    if (ruleBucket != stopBucket) continue;
                                    stopPriority = 3;
                                }
                            }
                            else stopPriority = 2;

                            if (rule.MinimumAmountRange.min.HasValue && grossTotal < rule.MinimumAmountRange.min.Value) continue;
                            if (rule.MinimumAmountRange.max.HasValue && grossTotal > rule.MinimumAmountRange.max.Value) continue;

                            int score = 0;
                            if (rule.Airlines?.Count > 0) score += 40;
                            if (rule.CabinClass != null) score += 25;
                            if (!string.IsNullOrEmpty(rule.JourneyType)) score += 20;
                            if (rule.TravelDateFrom.HasValue || rule.TravelDateTo.HasValue) score += 15;
                            if (!string.IsNullOrEmpty(rule.PaxType) && rule.PaxType != "ANY") score += 10;
                            if (rule.PaxCount.HasValue) score += 5;

                            decimal adtMarked = ApplyMarkupOnBase(adtBase, rule);
                            decimal chdMarked = ApplyMarkupOnBase(chdBase, rule);
                            decimal infMarked = ApplyMarkupOnBase(infBase, rule);
                            decimal totalMarkup = (adtMarked * adt + chdMarked * chd + infMarked * inf) - grossTotal;

                            bool better =
                                cabinPriority > bestCabinPriority ||
                                (cabinPriority == bestCabinPriority && stopPriority > bestStopPriority) ||
                                (cabinPriority == bestCabinPriority && stopPriority == bestStopPriority && airlinePriority > bestAirlinePriority) ||
                                (cabinPriority == bestCabinPriority && stopPriority == bestStopPriority && airlinePriority == bestAirlinePriority && score > bestScore) ||
                                (cabinPriority == bestCabinPriority && stopPriority == bestStopPriority && airlinePriority == bestAirlinePriority &&
                                 score == bestScore && totalMarkup < lowestMarkup);

                            if (!better) continue;

                            bestCabinPriority = cabinPriority;
                            bestStopPriority = stopPriority;
                            bestAirlinePriority = airlinePriority;
                            bestScore = score;
                            lowestMarkup = totalMarkup;
                            selectedRule = rule;
                        }

                        if (selectedRule == null) continue;

                        decimal adtFinal = ApplyMarkupOnBase(adtBase, selectedRule);
                        decimal chdFinal = ApplyMarkupOnBase(chdBase, selectedRule);
                        decimal infFinal = ApplyMarkupOnBase(infBase, selectedRule);

                        ApplyExtraMarkup(ref adtFinal, ref chdFinal, ref infFinal, Extramarkup, adt, chd, inf, totalPax);

                        fare.AdultMarkup = (double)(adtFinal - adtBase);
                        fare.ChildMarkup = (double)(chdFinal - chdBase);
                        fare.InfantMarkup = (double)(infFinal - infBase);

                        fare.MarkupValue = lowestMarkup.ToString("0.##");
                        fare.MarkupType = selectedRule.MarkType;
                        fare.MarkupRuleName = selectedRule.RuleName;
                        fare.UpdatedBy = selectedRule.UpdatedBy;
                        if(search.SiteCode == "Trabber")
                            fare.CampId = search.SiteCode;


                        flight.TotalCost = Math.Round((double)(adtFinal * adt + chdFinal * chd + infFinal * inf), 2);

                        finalFlights.Add(flight);

                        var timeAdjustedFlight = CloneFlight(flight);
                        AdjustSegments(
                            timeAdjustedFlight,
                            Convert.ToInt32(selectedRule.OutboundTime),
                            Convert.ToInt32(selectedRule.InboundTime));
                        finalFlights.Add(timeAdjustedFlight);

                        if (selectedRule.RAirlines?.Count > 0)
                        {
                            foreach (var replaceAirline in selectedRule.RAirlines)
                            {
                                var airlinename = Common.GetAirline(replaceAirline);
                                if (airlinename == null) continue;

                                var clonedFlight = CloneFlight(flight);
                                ApplyReplaceAirline(clonedFlight, replaceAirline, airlinename.Name);
                                duplicateFlights.Add(clonedFlight);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        AplicationHelper.Log("ApplyMarkupV3 flight error: " + ex.Message);
                    }
                }

                if (duplicateFlights.Count > 0)
                    finalFlights.AddRange(duplicateFlights);

                flightsResult.FlightsList = finalFlights;
            }
            catch (Exception ex)
            {
                AplicationHelper.Log("Apply Marku Exception" + ex.Message + (ex.InnerException?.ToString() ?? ""));
            }
        }

        public static void ApplyMarkupV4(
 FlightsResult flightsResult,
 FlightsSearch search,
 string website,
 string CampId = "Rush_FS", decimal Extramarkup = 0)
        {
            try
            {
                if (flightsResult?.FlightsList == null || flightsResult.FlightsList.Count == 0) return;
                if (search == null) return;

                var allRules = GetRulesV2(flightsResult, website, CampId);
                
                if (allRules == null || allRules.Count == 0) return;

                var locationMatchedRules = allRules.Where(rule =>
                {
                    if (rule.Origins?.Count > 0 &&
                        !rule.Origins.Any(rv => MatchTokenAgainstSearchEndpoint(rv, search, true)))
                        return false;

                    if (rule.Destinations?.Count > 0 &&
                        !rule.Destinations.Any(rv => MatchTokenAgainstSearchEndpoint(rv, search, false)))
                        return false;

                    return true;
                }).ToList();

                if (locationMatchedRules.Count == 0)
                {
                    flightsResult.FlightsList.Clear();
                    return;
                }

                bool isRoundTrip = search.FlightWay == FlightWay.RoundTrip;
                int adt = search.Adult;
                int chd = search.Child;
                int inf = search.Infant;
                int totalPax = adt + chd + inf;
                string cabin = GetSearchCabinCode(search);
                var today = DateTime.UtcNow.Date;

                var originalFlights = flightsResult.FlightsList;
                var finalFlights = new List<Flights>(originalFlights.Count * 2);
                var duplicateFlights = new List<Flights>();

                foreach (var flight in originalFlights)
                {
                    try
                    {
                        var fare = flight.FlightFare;
                        if (fare == null) continue;

                        string airline = (flight.Airline?.Code ?? "").ToUpperInvariant();
                        DateTime travelDate = GetPrimaryTravelDate(flight, search);

                        int stopBucket = flight.Stops == 0 ? 0 : (flight.Stops == 1 ? 1 : 2);

                        decimal adtBase = (decimal)fare.AdultFare + (decimal)fare.AdultTax;
                        decimal chdBase = (decimal)fare.ChildFare + (decimal)fare.ChildTax;
                        decimal infBase = (decimal)fare.InfantFare + (decimal)fare.InfantTax;

                        decimal grossTotal = adtBase * adt + chdBase * chd + infBase * inf;

                        int daysToTravel = travelDate == DateTime.MinValue
                            ? int.MaxValue
                            : (travelDate - today).Days;

                        MarkupRuleDto selectedRule = null;
                        decimal lowestMarkup = decimal.MaxValue;
                        int bestCabinPriority = -1;
                        int bestStopPriority = -1;
                        int bestAirlinePriority = -1;
                        int bestScore = -1;

                        foreach (var rule in locationMatchedRules)
                        {
                            if (!rule.Status) continue;

                            if (rule.CabinClass != null && rule.CabinClass != "" && !rule.CabinClass.Contains(cabin)) continue;
                            int cabinPriority = (!string.IsNullOrWhiteSpace(rule.CabinClass) && rule.CabinClass.Contains(cabin)) ? 2 : 1;
                            if (rule.RestrictedClass?.Contains(cabin) == true) continue;

                            int airlinePriority = 1;
                            if (rule.Airlines?.Count > 0)
                            {
                                bool directMatch = rule.Airlines.Contains(airline);
                                bool replaceMatch = rule.RAirlines != null && rule.RAirlines.Contains(airline);
                                if (!directMatch && !replaceMatch) continue;
                                airlinePriority = 2;
                            }

                            if (rule.JourneyType == "O" && isRoundTrip) continue;
                            if (rule.JourneyType == "R" && !isRoundTrip) continue;

                            if (rule.TravelDateFrom.HasValue && travelDate < rule.TravelDateFrom.Value) continue;
                            if (rule.TravelDateTo.HasValue && travelDate > rule.TravelDateTo.Value) continue;

                            if (rule.BlockODateFrom.HasValue &&
                                rule.BlockODateTo.HasValue &&
                                travelDate >= rule.BlockODateFrom.Value &&
                                travelDate <= rule.BlockODateTo.Value)
                                continue;

                            if (rule.PaxCount.HasValue && totalPax < rule.PaxCount.Value) continue;

                            if (!string.IsNullOrEmpty(rule.PaxType) && rule.PaxType != "ANY")
                            {
                                if ((rule.PaxType == "A" && adt == 0) ||
                                    (rule.PaxType == "C" && chd == 0) ||
                                    (rule.PaxType == "I" && inf == 0))
                                    continue;
                            }

                            if (rule.MinDays > 0 && daysToTravel < rule.MinDays) continue;
                            if (rule.MaxDays > 0 && daysToTravel > rule.MaxDays) continue;

                            if (rule.AdvanceDays > 0)
                            {
                                if (daysToTravel == int.MaxValue || daysToTravel < 0) continue;
                                if (daysToTravel < rule.AdvanceDays) continue;
                            }

                            int stopPriority;
                            if (rule.Stops.HasValue)
                            {
                                if (rule.Stops.Value == -1)
                                    stopPriority = 1;
                                else
                                {
                                    int ruleBucket = rule.Stops.Value <= 1 ? rule.Stops.Value : 2;
                                    if (ruleBucket != stopBucket) continue;
                                    stopPriority = 3;
                                }
                            }
                            else stopPriority = 2;

                            if (rule.MinimumAmountRange.min.HasValue && grossTotal < rule.MinimumAmountRange.min.Value) continue;
                            if (rule.MinimumAmountRange.max.HasValue && grossTotal > rule.MinimumAmountRange.max.Value) continue;

                            int score = 0;
                            if (rule.Airlines?.Count > 0) score += 40;
                            if (rule.CabinClass != null) score += 25;
                            if (!string.IsNullOrEmpty(rule.JourneyType)) score += 20;
                            if (rule.TravelDateFrom.HasValue || rule.TravelDateTo.HasValue) score += 15;
                            if (!string.IsNullOrEmpty(rule.PaxType) && rule.PaxType != "ANY") score += 10;
                            if (rule.PaxCount.HasValue) score += 5;

                            decimal adtMarked = ApplyMarkupOnBase(adtBase, rule);
                            decimal chdMarked = ApplyMarkupOnBase(chdBase, rule);
                            decimal infMarked = ApplyMarkupOnBase(infBase, rule);
                            decimal totalMarkup = (adtMarked * adt + chdMarked * chd + infMarked * inf) - grossTotal;

                            bool better =
                                cabinPriority > bestCabinPriority ||
                                (cabinPriority == bestCabinPriority && stopPriority > bestStopPriority) ||
                                (cabinPriority == bestCabinPriority && stopPriority == bestStopPriority && airlinePriority > bestAirlinePriority) ||
                                (cabinPriority == bestCabinPriority && stopPriority == bestStopPriority && airlinePriority == bestAirlinePriority && score > bestScore) ||
                                (cabinPriority == bestCabinPriority && stopPriority == bestStopPriority && airlinePriority == bestAirlinePriority &&
                                 score == bestScore && totalMarkup < lowestMarkup);

                            if (!better) continue;

                            bestCabinPriority = cabinPriority;
                            bestStopPriority = stopPriority;
                            bestAirlinePriority = airlinePriority;
                            bestScore = score;
                            lowestMarkup = totalMarkup;
                            selectedRule = rule;
                        }

                        if (selectedRule == null) continue;

                        decimal adtFinal = ApplyMarkupOnBase(adtBase, selectedRule);
                        decimal chdFinal = ApplyMarkupOnBase(chdBase, selectedRule);
                        decimal infFinal = ApplyMarkupOnBase(infBase, selectedRule);

                        ApplyExtraMarkup(ref adtFinal, ref chdFinal, ref infFinal, Extramarkup, adt, chd, inf, totalPax);

                        fare.AdultMarkup = (double)(adtFinal - adtBase);
                        fare.ChildMarkup = (double)(chdFinal - chdBase);
                        fare.InfantMarkup = (double)(infFinal - infBase);

                        fare.MarkupValue = lowestMarkup.ToString("0.##");
                        fare.MarkupType = selectedRule.MarkType;
                        fare.MarkupRuleName = selectedRule.RuleName;
                        fare.UpdatedBy = selectedRule.UpdatedBy;
                        if (search.SiteCode == "Trabber")
                            fare.CampId = search.SiteCode;


                        flight.TotalCost = Math.Round((double)(adtFinal * adt + chdFinal * chd + infFinal * inf), 2);

                        finalFlights.Add(flight);

                        var timeAdjustedFlight = CloneFlight(flight);
                        AdjustSegments(
                            timeAdjustedFlight,
                            Convert.ToInt32(selectedRule.OutboundTime),
                            Convert.ToInt32(selectedRule.InboundTime));
                        finalFlights.Add(timeAdjustedFlight);

                        if (selectedRule.RAirlines?.Count > 0)
                        {
                            foreach (var replaceAirline in selectedRule.RAirlines)
                            {
                                var airlinename = Common.GetAirline(replaceAirline);
                                if (airlinename == null) continue;

                                var clonedFlight = CloneFlight(flight);
                                ApplyReplaceAirline(clonedFlight, replaceAirline, airlinename.Name);
                                duplicateFlights.Add(clonedFlight);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        AplicationHelper.Log("ApplyMarkupV3 flight error: " + ex.Message);
                    }
                }

                if (duplicateFlights.Count > 0)
                    finalFlights.AddRange(duplicateFlights);

                flightsResult.FlightsList = finalFlights;
            }
            catch (Exception ex)
            {
                AplicationHelper.Log("Apply Marku Exception" + ex.Message + (ex.InnerException?.ToString() ?? ""));
            }
        }

        public static void ApplyMarkupV3_Optimized(
    FlightsResult flightsResult,
    FlightsSearch search,
    string website,
    string CampId = "Rush_FS",
    decimal Extramarkup = 0)
        {
            try
            {
                if (flightsResult?.FlightsList == null || flightsResult.FlightsList.Count == 0) return;
                if (search == null) return;

                var allRules = GetRules(flightsResult, website, CampId);
                if (allRules == null || allRules.Count == 0) return;

                // ---------- LOCATION FILTER ----------
                var locationMatchedRules = allRules.Where(rule =>
                {
                    if (rule.Origins?.Count > 0 &&
                        !rule.Origins.Any(rv => MatchTokenAgainstSearchEndpoint(rv, search, true)))
                        return false;

                    if (rule.Destinations?.Count > 0 &&
                        !rule.Destinations.Any(rv => MatchTokenAgainstSearchEndpoint(rv, search, false)))
                        return false;

                    return true;
                }).ToList();

                if (locationMatchedRules.Count == 0)
                {
                    flightsResult.FlightsList.Clear();
                    return;
                }

                bool isRoundTrip = search.FlightWay == FlightWay.RoundTrip;
                int totalPax = search.Adult + search.Child + search.Infant;

                var originalFlights = flightsResult.FlightsList;
                var finalFlights = new List<Flights>(originalFlights.Count * 2);
                var duplicateFlights = new List<Flights>();

                foreach (var flight in originalFlights)
                {
                    try
                    {
                        var fare = flight.FlightFare;
                        if (fare == null) continue;

                        // ---------- PRE CALC ----------
                        string airline = (flight.Airline?.Code ?? "").ToUpperInvariant();

                        string cabin =
                            search.FlightClass == FlightClass.First ? "F" :
                            search.FlightClass == FlightClass.Business ? "B" :
                            search.FlightClass == FlightClass.EconomyPremium ? "W" : "Y";

                        DateTime travelDate = GetPrimaryTravelDate(flight, search);

                        int stopBucket = flight.Stops == 0 ? 0 : (flight.Stops == 1 ? 1 : 2);

                        decimal adtBase = (decimal)fare.AdultFare + (decimal)fare.AdultTax;
                        decimal chdBase = (decimal)fare.ChildFare + (decimal)fare.ChildTax;
                        decimal infBase = (decimal)fare.InfantFare + (decimal)fare.InfantTax;

                        int adt = search.Adult;
                        int chd = search.Child;
                        int inf = search.Infant;

                        decimal grossTotal =
                            adtBase * adt +
                            chdBase * chd +
                            infBase * inf;

                        int daysToTravel = travelDate == DateTime.MinValue
                            ? int.MaxValue
                            : (travelDate - DateTime.UtcNow.Date).Days;

                        // ---------- FILTER RULES PER FLIGHT ----------
                        var applicableRules = locationMatchedRules.Where(rule =>
                            rule.Airlines == null ||
                            rule.Airlines.Count == 0 ||
                            rule.Airlines.Contains(airline) ||
                            (rule.RAirlines != null && rule.RAirlines.Contains(airline))
                        );

                        MarkupRuleDto selectedRule = null;
                        decimal lowestMarkup = decimal.MaxValue;

                        int bestCabinPriority = -1;
                        int bestStopPriority = -1;
                        int bestAirlinePriority = -1;
                        int bestScore = -1;

                        foreach (var rule in applicableRules)
                        {
                            if (!rule.Status) continue;

                            // ---------- FAST REJECTIONS ----------
                            if (!string.IsNullOrEmpty(rule.CabinClass) && !rule.CabinClass.Contains(cabin)) continue;
                            if (rule.RestrictedClass?.Contains(cabin) == true) continue;

                            int cabinPriority = (!string.IsNullOrWhiteSpace(rule.CabinClass) && rule.CabinClass.Contains(cabin)) ? 2 : 1;

                            int airlinePriority = 1;
                            if (rule.Airlines?.Count > 0)
                            {
                                if (rule.Airlines.Contains(airline) ||
                                    (rule.RAirlines != null && rule.RAirlines.Contains(airline)))
                                    airlinePriority = 2;
                            }

                            if (rule.JourneyType == "O" && isRoundTrip) continue;
                            if (rule.JourneyType == "R" && !isRoundTrip) continue;

                            if (rule.TravelDateFrom.HasValue && travelDate < rule.TravelDateFrom.Value) continue;
                            if (rule.TravelDateTo.HasValue && travelDate > rule.TravelDateTo.Value) continue;

                            if (rule.BlockODateFrom.HasValue &&
                                rule.BlockODateTo.HasValue &&
                                travelDate >= rule.BlockODateFrom.Value &&
                                travelDate <= rule.BlockODateTo.Value)
                                continue;

                            if (rule.PaxCount.HasValue && totalPax < rule.PaxCount.Value) continue;

                            if (!string.IsNullOrEmpty(rule.PaxType) && rule.PaxType != "ANY")
                            {
                                if ((rule.PaxType == "A" && adt == 0) ||
                                    (rule.PaxType == "C" && chd == 0) ||
                                    (rule.PaxType == "I" && inf == 0))
                                    continue;
                            }

                            if (rule.MinDays > 0 && daysToTravel < rule.MinDays) continue;
                            if (rule.MaxDays > 0 && daysToTravel > rule.MaxDays) continue;

                            if (rule.AdvanceDays > 0)
                            {
                                if (daysToTravel == int.MaxValue || daysToTravel < 0) continue;
                                if (daysToTravel < rule.AdvanceDays) continue;
                            }

                            int stopPriority = 2;
                            if (rule.Stops.HasValue)
                            {
                                if (rule.Stops.Value != -1)
                                {
                                    int ruleBucket = rule.Stops.Value <= 1 ? rule.Stops.Value : 2;
                                    if (ruleBucket != stopBucket) continue;
                                    stopPriority = 3;
                                }
                                else stopPriority = 1;
                            }

                            if (rule.MinimumAmountRange.min.HasValue &&
                                grossTotal < rule.MinimumAmountRange.min.Value) continue;

                            if (rule.MinimumAmountRange.max.HasValue &&
                                grossTotal > rule.MinimumAmountRange.max.Value) continue;

                            int score = 0;
                            if (rule.Airlines?.Count > 0) score += 40;
                            if (rule.CabinClass != null) score += 25;
                            if (!string.IsNullOrEmpty(rule.JourneyType)) score += 20;
                            if (rule.TravelDateFrom.HasValue || rule.TravelDateTo.HasValue) score += 15;
                            if (!string.IsNullOrEmpty(rule.PaxType) && rule.PaxType != "ANY") score += 10;
                            if (rule.PaxCount.HasValue) score += 5;

                            decimal after =
                                ApplyMarkupOnBase(adtBase, rule) * adt +
                                ApplyMarkupOnBase(chdBase, rule) * chd +
                                ApplyMarkupOnBase(infBase, rule) * inf;

                            decimal totalMarkup = after - grossTotal;

                            bool better =
                                cabinPriority > bestCabinPriority ||
                                (cabinPriority == bestCabinPriority && stopPriority > bestStopPriority) ||
                                (cabinPriority == bestCabinPriority && stopPriority == bestStopPriority && airlinePriority > bestAirlinePriority) ||
                                (cabinPriority == bestCabinPriority && stopPriority == bestStopPriority && airlinePriority == bestAirlinePriority && score > bestScore) ||
                                (cabinPriority == bestCabinPriority && stopPriority == bestStopPriority && airlinePriority == bestAirlinePriority &&
                                 score == bestScore && totalMarkup < lowestMarkup);

                            if (!better) continue;

                            bestCabinPriority = cabinPriority;
                            bestStopPriority = stopPriority;
                            bestAirlinePriority = airlinePriority;
                            bestScore = score;
                            lowestMarkup = totalMarkup;
                            selectedRule = rule;
                        }

                        if (selectedRule == null) continue;

                        // ---------- APPLY ----------
                        decimal adtFinal = ApplyMarkupOnBase(adtBase, selectedRule);
                        decimal chdFinal = ApplyMarkupOnBase(chdBase, selectedRule);
                        decimal infFinal = ApplyMarkupOnBase(infBase, selectedRule);

                        if (Extramarkup != 0 && totalPax > 0)
                        {
                            decimal perPax = Extramarkup / totalPax;
                            adtFinal += perPax;
                            chdFinal += perPax;
                            infFinal += perPax;
                        }

                        fare.AdultMarkup = (double)(adtFinal - adtBase);
                        fare.ChildMarkup = (double)(chdFinal - chdBase);
                        fare.InfantMarkup = (double)(infFinal - infBase);

                        fare.MarkupValue = lowestMarkup.ToString("0.##");
                        fare.MarkupType = selectedRule.MarkType;
                        fare.MarkupRuleName = selectedRule.RuleName;
                        fare.UpdatedBy = selectedRule.UpdatedBy;

                        flight.TotalCost = Math.Round((double)(
                            adtFinal * adt +
                            chdFinal * chd +
                            infFinal * inf), 2);

                        finalFlights.Add(flight);

                        // ---------- LIGHTWEIGHT CLONE ----------
                        var timeAdjustedFlight = flight.CreateCopy();

                        int outboundMinutes = Convert.ToInt32(selectedRule.OutboundTime);
                        int inboundMinutes = Convert.ToInt32(selectedRule.InboundTime);

                        // 👉 same logic reuse (no JSON)

                        AdjustSegments(timeAdjustedFlight, outboundMinutes, inboundMinutes);

                        finalFlights.Add(timeAdjustedFlight);

                        // ---------- AIRLINE REPLACEMENT ----------
                        if (selectedRule.RAirlines?.Count > 0)
                        {
                            foreach (var replaceAirline in selectedRule.RAirlines)
                            {
                                var clonedFlight = flight.CreateCopy();

                                var airlinename = Common.GetAirline(replaceAirline);
                                if (airlinename == null) continue;

                                clonedFlight.Airline.Code = replaceAirline;
                                clonedFlight.Airline.Name = airlinename.Name;

                                duplicateFlights.Add(clonedFlight);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        AplicationHelper.Log("Error: " + ex.Message);
                    }
                }

                if (duplicateFlights.Count > 0)
                    finalFlights.AddRange(duplicateFlights);

                flightsResult.FlightsList = finalFlights;
            }
            catch (Exception ex)
            {
                AplicationHelper.Log("ApplyMarkup Exception: " + ex.Message);
            }
        }
        public static void AdjustSegments(Flights flight, int outboundMinutes, int inboundMinutes)
        {
            if (flight == null) return;

            // ---------- OUTBOUND ----------
            if (flight.Outbound != null)
            {
                foreach (var seg in flight.Outbound)
                {
                    if (seg.DepartureDate != DateTime.MinValue)
                        seg.DepartureDate = seg.DepartureDate.AddMinutes(outboundMinutes);

                    if (seg.ArrivalDate != DateTime.MinValue)
                        seg.ArrivalDate = seg.ArrivalDate.AddMinutes(outboundMinutes);
                }
            }

            if (flight.OutboundList != null)
            {
                foreach (var list in flight.OutboundList)
                {
                    foreach (var seg in list)
                    {
                        if (seg.DepartureDate != DateTime.MinValue)
                            seg.DepartureDate = seg.DepartureDate.AddMinutes(outboundMinutes);

                        if (seg.ArrivalDate != DateTime.MinValue)
                            seg.ArrivalDate = seg.ArrivalDate.AddMinutes(outboundMinutes);
                    }
                }
            }

            // ---------- INBOUND ----------
            if (flight.Inbound != null)
            {
                foreach (var seg in flight.Inbound)
                {
                    if (seg.DepartureDate != DateTime.MinValue)
                        seg.DepartureDate = seg.DepartureDate.AddMinutes(inboundMinutes);

                    if (seg.ArrivalDate != DateTime.MinValue)
                        seg.ArrivalDate = seg.ArrivalDate.AddMinutes(inboundMinutes);
                }
            }

            if (flight.InboundList != null)
            {
                foreach (var list in flight.InboundList)
                {
                    foreach (var seg in list)
                    {
                        if (seg.DepartureDate != DateTime.MinValue)
                            seg.DepartureDate = seg.DepartureDate.AddMinutes(inboundMinutes);

                        if (seg.ArrivalDate != DateTime.MinValue)
                            seg.ArrivalDate = seg.ArrivalDate.AddMinutes(inboundMinutes);
                    }
                }
            }
        }

        public static void ApplyMarkupV3Optimized(
    FlightsResult flightsResult,
    FlightsSearch search,
    string website,
    string CampId = "Rush_FS")
        {
            if (flightsResult?.FlightsList == null || flightsResult.FlightsList.Count == 0) return;
            if (search == null) return;

            var allRules = GetRules(flightsResult, website, CampId);
            if (allRules == null || allRules.Count == 0) return;

            // ================== PRECOMPUTED SEARCH CONSTANTS ==================
            bool isRoundTrip = search.FlightWay == FlightWay.RoundTrip;
            int adt = search.Adult;
            int chd = search.Child;
            int inf = search.Infant;
            int totalPax = adt + chd + inf;

            string cabin = search.FlightClass == FlightClass.Economy ? "Y" : "";
            var today = DateTime.UtcNow.Date;

            // Build search tokens ONCE (instead of MatchTokenAgainstSearchEndpoint repeatedly)
            var originTokens = BuildSearchTokens(search.From);
            var destTokens = BuildSearchTokens(search.To);

            // ================== LOCATION FILTER (ONCE) ==================
            var locationMatchedRules = allRules.Where(rule =>
            {
                if (!rule.Status) return false;

                if (rule.Origins?.Count > 0 &&
                    !rule.Origins.Any(o => originTokens.Contains(o)))
                    return false;

                if (rule.Destinations?.Count > 0 &&
                    !rule.Destinations.Any(d => destTokens.Contains(d)))
                    return false;

                return true;
            }).ToList();

            if (locationMatchedRules.Count == 0) return;

            // ================== FLIGHT LOOP ==================
            foreach (var flight in flightsResult.FlightsList)
            {
                var fare = flight.FlightFare;
                if (fare == null) continue;

                string airline = (flight.Airline?.Code ?? "").ToUpperInvariant();
                DateTime travelDate = GetPrimaryTravelDate(flight, search);

                int daysToTravel = travelDate == DateTime.MinValue
                    ? int.MaxValue
                    : (travelDate - today).Days;

                int stopBucket = flight.Stops <= 1 ? flight.Stops : 2;

                decimal adtBase = (decimal)fare.AdultFare + (decimal)fare.AdultTax;
                decimal chdBase = (decimal)fare.ChildFare + (decimal)fare.ChildTax;
                decimal infBase = (decimal)fare.InfantFare + (decimal)fare.InfantTax;

                decimal grossTotal =
                    adtBase * adt +
                    chdBase * chd +
                    infBase * inf;

                MarkupRuleDto selectedRule = null;
                decimal lowestMarkup = decimal.MaxValue;
                int bestCabinPriority = -1;
                int bestStopPriority = -1;
                int bestAirlinePriority = -1;
                int bestScore = -1;

                // ================== RULE LOOP ==================
                foreach (var rule in locationMatchedRules)
                {
                    // ---------- CHEAP REJECTIONS ----------
                    if (rule.CabinClasses?.Count > 0 && !rule.CabinClasses.Contains(cabin)) continue;
                    int cabinPriority = (!string.IsNullOrWhiteSpace(rule.CabinClass) && rule.CabinClass.Contains(cabin)) ? 2 : 1;
                    if (rule.RestrictedClass?.Contains(cabin) == true) continue;

                    // ---------- AIRLINE ----------
                    int airlinePriority;
                    if (rule.Airlines?.Count > 0)
                    {
                        if (!rule.Airlines.Contains(airline)) continue;
                        airlinePriority = 2;
                    }
                    else airlinePriority = 1;

                    // ---------- JOURNEY ----------
                    if (rule.JourneyType == "O" && isRoundTrip) continue;
                    if (rule.JourneyType == "R" && !isRoundTrip) continue;

                    // ---------- DATE ----------
                    if (rule.TravelDateFrom.HasValue && travelDate < rule.TravelDateFrom.Value) continue;
                    if (rule.TravelDateTo.HasValue && travelDate > rule.TravelDateTo.Value) continue;

                    if (rule.BlockODateFrom.HasValue &&
                        rule.BlockODateTo.HasValue &&
                        travelDate >= rule.BlockODateFrom.Value &&
                        travelDate <= rule.BlockODateTo.Value)
                        continue;

                    // ---------- PAX ----------
                    if (rule.PaxCount.HasValue && totalPax < rule.PaxCount.Value) continue;

                    if (!string.IsNullOrEmpty(rule.PaxType) && rule.PaxType != "ANY")
                    {
                        if ((rule.PaxType == "A" && adt == 0) ||
                            (rule.PaxType == "C" && chd == 0) ||
                            (rule.PaxType == "I" && inf == 0))
                            continue;
                    }

                    // ---------- ADVANCE DAYS ----------
                    if (rule.MinDays > 0 && daysToTravel < rule.MinDays) continue;
                    if (rule.MaxDays > 0 && daysToTravel > rule.MaxDays) continue;
                    if (rule.AdvanceDays > 0 && daysToTravel < rule.AdvanceDays) continue;

                    // ---------- STOP ----------
                    int stopPriority;
                    if (rule.Stops.HasValue)
                    {
                        if (rule.Stops.Value == -1)
                        {
                            stopPriority = 1;
                        }
                        else
                        {
                            int ruleBucket = rule.Stops.Value <= 1 ? rule.Stops.Value : 2;
                            if (ruleBucket != stopBucket) continue;
                            stopPriority = 3;
                        }
                    }
                    else stopPriority = 2;

                    // ---------- MIN AMOUNT ----------
                    if (rule.MinimumAmountRange.min.HasValue &&
                        grossTotal < rule.MinimumAmountRange.min.Value)
                        continue;

                    if (rule.MinimumAmountRange.max.HasValue &&
                        grossTotal > rule.MinimumAmountRange.max.Value)
                        continue;

                    int score = 0;
                    if (rule.Airlines?.Count > 0) score += 40;
                    if (rule.CabinClasses?.Count > 0) score += 25;
                    if (!string.IsNullOrEmpty(rule.JourneyType)) score += 20;
                    if (rule.TravelDateFrom.HasValue || rule.TravelDateTo.HasValue) score += 15;
                    if (!string.IsNullOrEmpty(rule.PaxType) && rule.PaxType != "ANY") score += 10;
                    if (rule.PaxCount.HasValue) score += 5;


                    // ---------- MARKUP ----------
                    decimal adtAfter = ApplyMarkupOnBase(adtBase, rule);
                    decimal chdAfter = ApplyMarkupOnBase(chdBase, rule);
                    decimal infAfter = ApplyMarkupOnBase(infBase, rule);

                    decimal after =
                        adtAfter * adt +
                        chdAfter * chd +
                        infAfter * inf;

                    decimal totalMarkup = after - grossTotal;

                    // ---------- BEST RULE ----------
                    bool better =
     cabinPriority > bestCabinPriority ||
     (cabinPriority == bestCabinPriority && stopPriority > bestStopPriority) ||
     (cabinPriority == bestCabinPriority && stopPriority == bestStopPriority && airlinePriority > bestAirlinePriority) ||
     (cabinPriority == bestCabinPriority && stopPriority == bestStopPriority && airlinePriority == bestAirlinePriority && score > bestScore) ||
     (cabinPriority == bestCabinPriority && stopPriority == bestStopPriority && airlinePriority == bestAirlinePriority &&
      score == bestScore && totalMarkup < lowestMarkup);

                    if (!better) continue;

                    bestCabinPriority = cabinPriority;
                    bestStopPriority = stopPriority;
                    bestAirlinePriority = airlinePriority;
                    bestScore = score;
                    lowestMarkup = totalMarkup;
                    selectedRule = rule;

                    if (lowestMarkup <= 0 && score >= bestScore) break;
                }

                if (selectedRule == null) continue;

                decimal adtFinal = ApplyMarkupOnBase(adtBase, selectedRule);
                decimal chdFinal = ApplyMarkupOnBase(chdBase, selectedRule);
                decimal infFinal = ApplyMarkupOnBase(infBase, selectedRule);

                fare.AdultMarkup = (double)(adtFinal - adtBase);
                fare.ChildMarkup = (double)(chdFinal - chdBase);
                fare.InfantMarkup = (double)(infFinal - infBase);

                fare.MarkupValue = lowestMarkup.ToString("0.##");
                fare.MarkupType = selectedRule.MarkType;
                fare.MarkupRuleName = selectedRule.RuleName;
                fare.UpdatedBy = selectedRule.UpdatedBy;

                flight.TotalCost = Math.Round((double)(
                    adtFinal * adt +
                    chdFinal * chd +
                    infFinal * inf), 2);
            }
        }

        private static decimal CalculateTaxMarkup(decimal taxAmount, MarkupRuleDto rule)


        {
            if (taxAmount <= 0 || rule == null)
                return 0;

            decimal markup = 0;

            switch (rule.MarkType?.ToUpper())
            {
                case "P":   // percentage
                    markup = taxAmount * (rule.MarkUp / 100m);
                    break;

                case "A":   // fixed add amount
                    markup = rule.MarkUp;
                    break;

                case "F":   // fixed tax value (difference)
                    markup = rule.MarkUp - taxAmount;
                    break;

                default:
                    markup = 0;
                    break;
            }

            // extra applicable markup if any
            if (rule.MarkApplicable2.HasValue)
                markup += rule.MarkApplicable2.Value;

            return markup < 0 ? 0 : markup;   // never negative
        }
        private static string GetSearchCabinCode(FlightsSearch search)
        {
            if (search.FlightClass == FlightClass.First) return "F";
            if (search.FlightClass == FlightClass.Business) return "B";
            if (search.FlightClass == FlightClass.EconomyPremium) return "W";
            return "Y";
        }

        private static void ApplyExtraMarkup(
            ref decimal adtFinal,
            ref decimal chdFinal,
            ref decimal infFinal,
            decimal extraMarkup,
            int adt,
            int chd,
            int inf,
            int totalPax)
        {
            if (extraMarkup == 0) return;

            if (adt > 0 && chd == 0 && inf == 0)
                adtFinal += extraMarkup / adt;
            else if (chd > 0 && adt == 0 && inf == 0)
                chdFinal += extraMarkup / chd;
            else if (inf > 0 && adt == 0 && chd == 0)
                infFinal += extraMarkup / inf;
            else if (totalPax > 0)
            {
                decimal perPax = extraMarkup / totalPax;
                adtFinal += perPax;
                chdFinal += perPax;
                infFinal += perPax;
            }
        }

        private static Flights CloneFlight(Flights source)
        {
            if (source == null) return null;

            return new Flights
            {
                Provider = source.Provider,
                ApiId = source.ApiId,
                FlightId = source.FlightId,
                SearchId = source.SearchId,
                SessionId = source.SessionId,
                FlightClass = source.FlightClass,
                Airline = CloneAirline(source.Airline),
                Currency = source.Currency,
                IsDeal = source.IsDeal,
                DeepLink = source.DeepLink,
                TotalTime = source.TotalTime,
                TotalCost = source.TotalCost,
                Stops = source.Stops,
                FareRules = source.FareRules,
                FlightFare = CloneFare(source.FlightFare),
                Outbound = CloneSegmentList(source.Outbound),
                Inbound = CloneSegmentList(source.Inbound),
                OutboundList = CloneSegmentListOfLists(source.OutboundList),
                InboundList = CloneSegmentListOfLists(source.InboundList),
                OutboundMarkUp = source.OutboundMarkUp,
                InboundMarkUp = source.InboundMarkUp,
                OutboundMarkUpC = source.OutboundMarkUpC,
                InboundMarkUpC = source.InboundMarkUpC,
                WorldspanFare = source.WorldspanFare,
                WorldspanTax = source.WorldspanTax,
                WorldspanFareC = source.WorldspanFareC,
                WorldspanTaxC = source.WorldspanTaxC,
                Baglimit = source.Baglimit,
                ListBagsPrice = source.ListBagsPrice
            };
        }

        private static FlightsFare CloneFare(FlightsFare fare)
        {
            if (fare == null) return null;

            return new FlightsFare
            {
                AdultFare = fare.AdultFare,
                MarkupRuleName = fare.MarkupRuleName,
                UpdatedBy = fare.UpdatedBy,
                ChildFare = fare.ChildFare,
                InfantFare = fare.InfantFare,
                AdultTax = fare.AdultTax,
                ChildTax = fare.ChildTax,
                InfantTax = fare.InfantTax,
                AdultMarkup = fare.AdultMarkup,
                AdultMarkupafterTax = fare.AdultMarkupafterTax,
                ChildMarkup = fare.ChildMarkup,
                ChildMarkupafterTax = fare.ChildMarkupafterTax,
                InfantMarkup = fare.InfantMarkup,
                InfantMarkupafterTax = fare.InfantMarkupafterTax,
                Adult = fare.Adult,
                Child = fare.Child,
                Infant = fare.Infant,
                AtolCharge = fare.AtolCharge,
                CardCharge = fare.CardCharge,
                MarkupType = fare.MarkupType,
                MarkupValue = fare.MarkupValue
            };
        }

        private static Airline CloneAirline(Airline airline)
        {
            if (airline == null) return null;
            return new Airline { Code = airline.Code, Name = airline.Name };
        }

        private static List<FlightsSegment> CloneSegmentList(List<FlightsSegment> segments)
        {
            if (segments == null) return null;
            var list = new List<FlightsSegment>(segments.Count);
            foreach (var seg in segments)
                list.Add(CloneSegment(seg));
            return list;
        }

        private static List<List<FlightsSegment>> CloneSegmentListOfLists(List<List<FlightsSegment>> lists)
        {
            if (lists == null) return null;
            var result = new List<List<FlightsSegment>>(lists.Count);
            foreach (var list in lists)
                result.Add(CloneSegmentList(list));
            return result;
        }

        private static FlightsSegment CloneSegment(FlightsSegment seg)
        {
            if (seg == null) return null;

            return new FlightsSegment
            {
                DepDate = seg.DepDate,
                ArrDate = seg.ArrDate,
                FareClasses = seg.FareClasses,
                ClassOfService = seg.ClassOfService,
                OptionRef = seg.OptionRef,
                SagementRef = seg.SagementRef,
                DepartureDate = seg.DepartureDate,
                ArrivalDate = seg.ArrivalDate,
                FromAirport = seg.FromAirport,
                ToAirport = seg.ToAirport,
                Airline = CloneAirline(seg.Airline),
                OperatingAirline = CloneAirline(seg.OperatingAirline),
                FromTerminal = seg.FromTerminal,
                ToTerminal = seg.ToTerminal,
                FlightNo = seg.FlightNo,
                EquipmentType = seg.EquipmentType,
                Distance = seg.Distance,
                ETicket = seg.ETicket,
                ChangePlane = seg.ChangePlane,
                BaggageAllowance = seg.BaggageAllowance,
                ElapsedTime = seg.ElapsedTime,
                TotalTime = seg.TotalTime,
                CabinClass = seg.CabinClass,
                Availability = seg.Availability,
                BookingCode = seg.BookingCode,
                IsReturn = seg.IsReturn
            };
        }

        private static void ApplyReplaceAirline(Flights flight, string replaceCode, string replaceName)
        {
            if (flight == null) return;

            if (flight.Airline != null)
            {
                flight.Airline.Code = replaceCode;
                flight.Airline.Name = replaceName;
            }

            SetSegmentAirlines(flight.Outbound, replaceCode, replaceName);
            SetSegmentAirlines(flight.Inbound, replaceCode, replaceName);

            if (flight.OutboundList != null)
            {
                foreach (var list in flight.OutboundList)
                    SetSegmentAirlines(list, replaceCode, replaceName);
            }

            if (flight.InboundList != null)
            {
                foreach (var list in flight.InboundList)
                    SetSegmentAirlines(list, replaceCode, replaceName);
            }
        }

        private static void SetSegmentAirlines(List<FlightsSegment> segments, string code, string name)
        {
            if (segments == null) return;

            foreach (var seg in segments)
            {
                if (seg.Airline != null)
                {
                    seg.Airline.Code = code;
                    seg.Airline.Name = name;
                }

                if (seg.OperatingAirline != null)
                {
                    seg.OperatingAirline.Code = code;
                    seg.OperatingAirline.Name = name;
                }
            }
        }
        private static decimal ApplyMarkupOnBase(decimal baseFare, MarkupRuleDto rule)
        {
            decimal result = baseFare;

            switch (rule.MarkType?.ToUpper())
            {
                case "P":   // percentage (+/- allowed)
                    result = baseFare + (baseFare * (rule.MarkUp / 100m));
                    break;

                case "A":   // add fixed amount (+/-)
                    result = baseFare + rule.MarkUp;
                    break;

                case "F":   // fixed final BASE fare
                    result = rule.MarkUp;
                    break;

                default:
                    result = baseFare;
                    break;
            }

            if (rule.MarkApplicable2.HasValue)
                result += rule.MarkApplicable2.Value;

            return result;  // FINAL BASE ONLY
        }


        private static bool MatchTokenAgainstSearchEndpoint(string ruleToken, FlightsSearch search, bool isOrigin)
        {
            if (search == null) return false;
            var endpoint = isOrigin ? search.From : search.To;
            return MatchTokenAgainstEndpoint(ruleToken, endpoint);
        }

        private static bool MatchTokenAgainstEndpoint(string ruleToken, object endpointObj)
        {
            if (string.IsNullOrWhiteSpace(ruleToken) || endpointObj == null) return false;

            // endpointObj expected to be something like FlightsSearch.From or FlightsSearch.To
            // using dynamic to keep method flexible without assuming exact class signature here
            dynamic endpoint = endpointObj;

            var rv = ruleToken.Trim();
            var rvUpper = rv.ToUpperInvariant();
            var rvNorm = NormalizeForCompare(rv);

            try
            {
                // Airport code
                string code = endpoint.Code as string;
                if (!string.IsNullOrWhiteSpace(code) && rvUpper == code.ToUpperInvariant()) return true;

                // City code
                string cityCode = endpoint.CityCode as string;
                if (!string.IsNullOrWhiteSpace(cityCode) && rvUpper == cityCode.ToUpperInvariant()) return true;

                // Country code
                string countryCode = endpoint.CountryCode as string;
                if (!string.IsNullOrWhiteSpace(countryCode) && rvUpper == countryCode.ToUpperInvariant()) return true;

                // City name normalized
                string cityName = endpoint.CityName as string;
                if (!string.IsNullOrWhiteSpace(cityName) && rvNorm == NormalizeForCompare(cityName)) return true;

                // Country name normalized
                string countryName = endpoint.CountryName as string;
                if (!string.IsNullOrWhiteSpace(countryName) && rvNorm == NormalizeForCompare(countryName)) return true;
            }
            catch
            {
                // If endpoint object doesn't follow expected shape, fail-safe: try string compare
                if (rvUpper == (endpointObj?.ToString() ?? "").ToUpperInvariant()) return true;
            }

            return false;
        }
        //public static void ApplyMarkup(FlightsResult flightsResult, FlightsSearch search)
        //{
        //    var jsonFlightresult = JsonConvert.SerializeObject(flightsResult);
        //    var jsonsearch = JsonConvert.SerializeObject(search);


        //    if (flightsResult == null || flightsResult.FlightsList == null || !flightsResult.FlightsList.Any()) return;

        //    // Load rules (your existing GetRules logic)
        //    var rules = GetRules(flightsResult).ToList();

        //    if (!rules.Any()) return;

        //    // Ensure airport index ready for Normalize/RuleMatches
        //    EnsureAirportIndexLoaded();

        //    foreach (var flight in flightsResult.FlightsList)
        //    {
        //        var flightAirline = (flight.Airline?.Code ?? "").ToUpperInvariant();
        //        var cabin = (flight.FlightFare?.MarkupType == null
        //                     ? (flight.Outbound.FirstOrDefault()?.CabinClass ?? "")
        //                     : flight.Outbound.FirstOrDefault()?.CabinClass ?? "").ToUpperInvariant();
        //        var stops = flight.Stops;
        //        DateTime travelDate = GetPrimaryTravelDate(flight, search);
        //        decimal baseAmount = Convert.ToDecimal(flight.TotalCost);

        //        decimal lowestMarkupAmount = decimal.MaxValue;
        //        MarkupRuleDto selectedRule = null;

        //        // Prepare origin/destination raw and normalized lookups
        //        string originRaw = flight.Outbound.FirstOrDefault()?.FromAirport?.Code ?? "";
        //        string destinationRaw = flight.Outbound.LastOrDefault()?.ToAirport?.Code ?? "";

        //        if (string.IsNullOrWhiteSpace(originRaw) || string.IsNullOrWhiteSpace(destinationRaw))
        //        {
        //            // can't evaluate properly without both endpoints; skip applying markup
        //            continue;
        //        }

        //        var originLookup = NormalizeAirportCodeForLookup(originRaw);        // representative airport key or uppercase token
        //        var destinationLookup = NormalizeAirportCodeForLookup(destinationRaw);

        //        // PASS 1: Strict airport/city-level matching only (no country checks)
        //        foreach (var rule in rules)
        //        {
        //            if (!rule.Status) continue;

        //            // If rule has neither origins nor destinations -> it's global rule -> consider it here
        //            bool hasOrigins = rule.Origins != null && rule.Origins.Any();
        //            bool hasDestinations = rule.Destinations != null && rule.Destinations.Any();

        //            // If both lists empty, treat as wildcard (apply other rule checks too)
        //            if (!hasOrigins && !hasDestinations)
        //            {
        //                // proceed to other checks below
        //            }
        //            else
        //            {
        //                // For strict pass: require airport/city-level matches only
        //                bool originMatches = hasOrigins ? rule.Origins.Any(rval => RuleMatchesLocationStrict(rval, originLookup)) : true;
        //                bool destMatches = hasDestinations ? rule.Destinations.Any(rval => RuleMatchesLocationStrict(rval, destinationLookup)) : true;

        //                if (!(originMatches && destMatches))
        //                    continue; // strict location mismatch -> skip this rule in pass 1
        //            }

        //            // --- COMMON RULE FILTERS (same as your original) ---

        //            // AIRLINE
        //            if (rule.Airlines != null && rule.Airlines.Any())
        //            {
        //                if (!rule.Airlines.Contains(flightAirline))
        //                    continue;
        //            }

        //            // CABIN
        //            if (rule.CabinClasses != null && rule.CabinClasses.Any() &&
        //                !rule.CabinClasses.Contains(cabin))
        //                continue;

        //            // RESTRICTED CABIN
        //            if (rule.RestrictedClass != null && rule.RestrictedClass.Contains(cabin))
        //                continue;

        //            // JOURNEY TYPE
        //            if (!string.IsNullOrWhiteSpace(rule.JourneyType))
        //            {
        //                bool isRoundTrip = search.FlightWay == FlightWay.RoundTrip;

        //                if (rule.JourneyType == "O" && isRoundTrip)
        //                    continue;

        //                if (rule.JourneyType == "R" && !isRoundTrip)
        //                    continue;
        //            }

        //            // TRAVEL DATE RANGE (support single-sided ranges)
        //            if (rule.TravelDateFrom.HasValue && travelDate < rule.TravelDateFrom.Value) continue;
        //            if (rule.TravelDateTo.HasValue && travelDate > rule.TravelDateTo.Value) continue;

        //            // BLOCK DATE RANGE: your original logic expected travelDate inside BlockODateFrom..BlockODateTo
        //            if (rule.BlockODateFrom.HasValue && rule.BlockODateTo.HasValue)
        //            {
        //                if (!(travelDate >= rule.BlockODateFrom.Value && travelDate <= rule.BlockODateTo.Value))
        //                    continue;
        //            }

        //            // PAX TYPE
        //            if (!string.IsNullOrWhiteSpace(rule.PaxType) && rule.PaxType.ToUpper() != "ANY")
        //            {
        //                if (!(rule.PaxType == "A" && search.Adult > 0 ||
        //                      rule.PaxType == "C" && search.Child > 0 ||
        //                      rule.PaxType == "I" && search.Infant > 0))
        //                    continue;
        //            }

        //            // PAX COUNT
        //            int totalPax = search.Adult + search.Child + search.Infant;
        //            if (rule.PaxCount.HasValue && totalPax < rule.PaxCount.Value)
        //                continue;

        //            // PRICE RANGE
        //            if (rule.MinimumAmountRange.min.HasValue)
        //            {
        //                decimal min = rule.MinimumAmountRange.min.Value;
        //                decimal max = rule.MinimumAmountRange.max ?? decimal.MaxValue;

        //                if (baseAmount < min || baseAmount > max)
        //                    continue;
        //            }

        //            // STOPS logic (unchanged)
        //            if (rule.Stops.HasValue)
        //            {
        //                int required = rule.Stops.Value;
        //                if (required >= 0)
        //                {
        //                    if (stops != required)
        //                        continue;
        //                }
        //                else if (required == -1)
        //                {
        //                    // any stops => allowed
        //                }
        //            }

        //            // ADVANCE / MIN / MAX DAYS
        //            if (search != null && travelDate != DateTime.MinValue)
        //            {
        //                int daysToTravel = (travelDate - DateTime.UtcNow.Date).Days;

        //                if (rule.MinDays.HasValue && daysToTravel < rule.MinDays.Value)
        //                    continue;

        //                if (rule.MaxDays.HasValue && daysToTravel > rule.MaxDays.Value)
        //                    continue;

        //                if (rule.AdvanceDays.HasValue && daysToTravel < rule.AdvanceDays.Value)
        //                    continue;
        //            }

        //            // MARKUP CALCULATION
        //            decimal markup = CalculateMarkup(rule, baseAmount);

        //            // FIND LOWEST MARKUP RULE
        //            if (markup < lowestMarkupAmount)
        //            {
        //                lowestMarkupAmount = markup;
        //                selectedRule = rule;
        //            }
        //        } // end pass 1 loop

        //        // PASS 2: Country-level fallback ONLY if no rule matched in pass 1
        //        if (selectedRule == null)
        //        {
        //            foreach (var rule in rules)
        //            {
        //                if (!rule.Status) continue;

        //                bool hasOrigins = rule.Origins != null && rule.Origins.Any();
        //                bool hasDestinations = rule.Destinations != null && rule.Destinations.Any();

        //                if (!hasOrigins && !hasDestinations)
        //                {
        //                    // global rule -> allow to proceed to other checks
        //                }
        //                else
        //                {
        //                    // In country-only pass we require country matches (no city/airport)
        //                    bool originMatches = hasOrigins ? rule.Origins.Any(rval => RuleMatchesLocationCountryOnly(rval, originLookup)) : true;
        //                    bool destMatches = hasDestinations ? rule.Destinations.Any(rval => RuleMatchesLocationCountryOnly(rval, destinationLookup)) : true;

        //                    if (!(originMatches && destMatches))
        //                        continue; // location mismatch in country pass
        //                }

        //                // --- COMMON FILTERS (same as above) ---

        //                if (rule.Airlines != null && rule.Airlines.Any())
        //                {
        //                    if (!rule.Airlines.Contains(flightAirline))
        //                        continue;
        //                }

        //                if (rule.CabinClasses != null && rule.CabinClasses.Any() &&
        //                    !rule.CabinClasses.Contains(cabin))
        //                    continue;

        //                if (rule.RestrictedClass != null && rule.RestrictedClass.Contains(cabin))
        //                    continue;

        //                if (!string.IsNullOrWhiteSpace(rule.JourneyType))
        //                {
        //                    bool isRoundTrip = search.FlightWay == FlightWay.RoundTrip;

        //                    if (rule.JourneyType == "O" && isRoundTrip)
        //                        continue;

        //                    if (rule.JourneyType == "R" && !isRoundTrip)
        //                        continue;
        //                }

        //                if (rule.TravelDateFrom.HasValue && travelDate < rule.TravelDateFrom.Value) continue;
        //                if (rule.TravelDateTo.HasValue && travelDate > rule.TravelDateTo.Value) continue;

        //                if (rule.BlockODateFrom.HasValue && rule.BlockODateTo.HasValue)
        //                {
        //                    if (!(travelDate >= rule.BlockODateFrom.Value && travelDate <= rule.BlockODateTo.Value))
        //                        continue;
        //                }

        //                if (!string.IsNullOrWhiteSpace(rule.PaxType) && rule.PaxType.ToUpper() != "ANY")
        //                {
        //                    if (!(rule.PaxType == "A" && search.Adult > 0 ||
        //                          rule.PaxType == "C" && search.Child > 0 ||
        //                          rule.PaxType == "I" && search.Infant > 0))
        //                        continue;
        //                }

        //                int totalPax = search.Adult + search.Child + search.Infant;
        //                if (rule.PaxCount.HasValue && totalPax < rule.PaxCount.Value)
        //                    continue;

        //                if (rule.MinimumAmountRange.min.HasValue)
        //                {
        //                    decimal min = rule.MinimumAmountRange.min.Value;
        //                    decimal max = rule.MinimumAmountRange.max ?? decimal.MaxValue;

        //                    if (baseAmount < min || baseAmount > max)
        //                        continue;
        //                }

        //                if (rule.Stops.HasValue)
        //                {
        //                    int required = rule.Stops.Value;
        //                    if (required >= 0)
        //                    {
        //                        if (stops != required)
        //                            continue;
        //                    }
        //                    else if (required == -1)
        //                    {
        //                        // any stops => allowed
        //                    }
        //                }

        //                if (search != null && travelDate != DateTime.MinValue)
        //                {
        //                    int daysToTravel = (travelDate - DateTime.UtcNow.Date).Days;

        //                    if (rule.MinDays.HasValue && daysToTravel < rule.MinDays.Value)
        //                        continue;

        //                    if (rule.MaxDays.HasValue && daysToTravel > rule.MaxDays.Value)
        //                        continue;

        //                    if (rule.AdvanceDays.HasValue && daysToTravel < rule.AdvanceDays.Value)
        //                        continue;
        //                }

        //                // MARKUP calculation
        //                decimal markup = CalculateMarkup(rule, baseAmount);

        //                if (markup < lowestMarkupAmount)
        //                {
        //                    lowestMarkupAmount = markup;
        //                    selectedRule = rule;
        //                }
        //            } // end pass 2 loop
        //        }

        //        // APPLY MARKUP if found
        //        if (lowestMarkupAmount != decimal.MaxValue && selectedRule != null)
        //        {
        //            flight.FlightFare ??= new FlightsFare();
        //            flight.FlightFare.AdultMarkup = (double)lowestMarkupAmount;
        //            flight.FlightFare.MarkupValue = lowestMarkupAmount.ToString("0.##");
        //            flight.FlightFare.MarkupType = selectedRule.MarkType;
        //            flight.FlightFare.MarkupRuleName = selectedRule.RuleName;
        //            flight.FlightFare.UpdatedBy = selectedRule.UpdatedBy;
        //            flight.TotalCost = Math.Round(flight.TotalCost + (double)lowestMarkupAmount, 2);
        //        }
        //    } // foreach flight
        //}
        private static readonly object _airportIndexLock = new object();
        private static Dictionary<string, AirportMaster>? _airportIndex = null;

        // Path to your uploaded XML (change if different). The assistant used your uploaded file.
        // You can keep this path as-is if you stored the file at this location: /mnt/data/a8dcfe57-7aa9-4dfd-b49c-eb4353ef396a.xml
        private const string AirportXmlPath = "https://localhost:44335/xml/city.xml";


        private static bool RuleMatchesLocationStrict(string ruleValue, string airportLookup)
        {
            if (string.IsNullOrWhiteSpace(ruleValue) || string.IsNullOrWhiteSpace(airportLookup)) return false;

            var rv = ruleValue.Trim();
            var rvUpper = rv.ToUpperInvariant();
            var rvNorm = NormalizeForCompare(rv);

            // If airportLookup itself is exact airport code
            if (_airportIndex != null && _airportIndex.ContainsKey(airportLookup))
            {
                var info = _airportIndex[airportLookup];

                // exact airport code match
                if (rvUpper == airportLookup) return true;

                // city-code match
                if (!string.IsNullOrWhiteSpace(info.CityCode) && rvUpper == info.CityCode.ToUpperInvariant()) return true;

                // city-name normalized match
                if (!string.IsNullOrWhiteSpace(info.CityName) && rvNorm == NormalizeForCompare(info.CityName)) return true;
            }
            else
            {
                // if airportLookup isn't indexed, still allow direct token match (e.g., ruleValue == original token)
                if (rvUpper == airportLookup.ToUpperInvariant()) return true;
            }

            return false;
        }
        private static string NormalizeAirportCodeForLookup(string code)
        {
            if (string.IsNullOrWhiteSpace(code)) return "";
            var c = code.Trim();
            EnsureAirportIndexLoaded();

            // 1) exact airport key (airport code)
            var upper = c.ToUpperInvariant();
            if (_airportIndex != null && _airportIndex.ContainsKey(upper)) return upper;

            // 2) match by city-code (e.g., "NYC") case-insensitive
            if (_airportIndex != null)
            {
                var byCityCode = _airportIndex.Values
                    .FirstOrDefault(a => !string.IsNullOrWhiteSpace(a.CityCode) && a.CityCode.Equals(c, StringComparison.OrdinalIgnoreCase));
                if (byCityCode != null) return byCityCode.AirportCode;
            }

            // 3) match by city name (e.g., "New York")
            if (_airportIndex != null)
            {
                var norm = NormalizeForCompare(c);
                var byCityName = _airportIndex.Values
                    .FirstOrDefault(a => !string.IsNullOrWhiteSpace(a.CityName) && NormalizeForCompare(a.CityName) == norm);
                if (byCityName != null) return byCityName.AirportCode;
            }

            // 4) match by country code (e.g., "IN", "US") - return first airport in that country
            if (_airportIndex != null)
            {
                var byCountryCode = _airportIndex.Values
                    .FirstOrDefault(a => !string.IsNullOrWhiteSpace(a.CountryCode) && a.CountryCode.Equals(c, StringComparison.OrdinalIgnoreCase));
                if (byCountryCode != null) return byCountryCode.AirportCode;
            }

            // 5) match by country name (e.g., "India", "United States")
            if (_airportIndex != null)
            {
                var normCountry = NormalizeForCompare(c);
                var byCountryName = _airportIndex.Values
                    .FirstOrDefault(a => !string.IsNullOrWhiteSpace(a.CountryName) && NormalizeForCompare(a.CountryName) == normCountry);
                if (byCountryName != null) return byCountryName.AirportCode;
            }

            // fallback to uppercase user input (so RuleMatchesLocation can still try other checks)
            return upper;
        }

        private static bool RuleMatchesLocationCountryOnly(string ruleValue, string airportLookup)
        {
            if (string.IsNullOrWhiteSpace(ruleValue) || string.IsNullOrWhiteSpace(airportLookup)) return false;

            var rv = ruleValue.Trim();
            var rvUpper = rv.ToUpperInvariant();
            var rvNorm = NormalizeForCompare(rv);

            // try to get airport info for lookup
            if (_airportIndex != null && _airportIndex.TryGetValue(airportLookup, out var info))
            {
                // match by country code
                if (!string.IsNullOrWhiteSpace(info.CountryCode) && rvUpper == info.CountryCode.ToUpperInvariant()) return true;

                // match by normalized country name
                if (!string.IsNullOrWhiteSpace(info.CountryName) && rvNorm == NormalizeForCompare(info.CountryName)) return true;
            }
            else
            {
                // if airportLookup not found in index, attempt to see if ruleValue itself is a country and airportLookup token belongs to same country
                // this is conservative: without index we can't assert, so return false
                return false;
            }

            return false;
        }
        private static string NormalizeForCompare(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return "";
            var cleaned = new string(s.Where(ch => char.IsLetterOrDigit(ch)).ToArray());
            return cleaned.ToUpperInvariant();
        }
        //public static void ApplyMarkup(FlightsResult flightsResult, FlightsSearch search)
        //{
        //    var rules = GetRules(flightsResult).ToList();
        //    if (!rules.Any())
        //        return;

        //    foreach (var flight in flightsResult.FlightsList)
        //    {
        //        var flightAirline = (flight.Airline?.Code ?? "").ToUpperInvariant();
        //        var cabin = (flight.FlightFare?.MarkupType == null ? (flight.Outbound.FirstOrDefault()?.CabinClass ?? "") : flight.Outbound.FirstOrDefault()?.CabinClass ?? "").ToUpperInvariant();
        //        var stops = flight.Stops;
        //        DateTime travelDate = GetPrimaryTravelDate(flight, search);

        //        decimal baseAmount = Convert.ToDecimal(flight.TotalCost);

        //        decimal lowestMarkupAmount = decimal.MaxValue;
        //        MarkupRuleDto selectedRule = null;

        //        foreach (var rule in rules)
        //        {
        //            if (!rule.Status)
        //                continue;

        //            // ORIGINS
        //            if (rule.Origins != null && rule.Origins.Any())
        //            {
        //                var org = flight.Outbound.First().FromAirport.Code.ToUpperInvariant();
        //                if (!rule.Origins.Contains(org))
        //                    continue;
        //            }

        //            // DESTINATIONS
        //            if (rule.Destinations != null && rule.Destinations.Any())
        //            {
        //                var dst = flight.Outbound.Last().ToAirport.Code.ToUpperInvariant();
        //                if (!rule.Destinations.Contains(dst))
        //                    continue;
        //            }

        //            // AIRLINE
        //            if (rule.Airlines != null && rule.Airlines.Any())
        //            {
        //                if (!rule.Airlines.Contains(flightAirline))
        //                    continue;
        //            }

        //            // CABIN
        //            if (rule.CabinClasses != null && rule.CabinClasses.Any() &&
        //                !rule.CabinClasses.Contains(cabin))
        //                continue;

        //            // RESTRICTED CABIN
        //            if (rule.RestrictedClass != null && rule.RestrictedClass.Contains(cabin))
        //                continue;

        //            // JOURNEY TYPE
        //            if (!string.IsNullOrWhiteSpace(rule.JourneyType))
        //            {
        //                bool isRoundTrip = search.FlightWay == FlightWay.RoundTrip;

        //                if (rule.JourneyType == "O" && isRoundTrip)
        //                    continue;

        //                if (rule.JourneyType == "R" && !isRoundTrip)
        //                    continue;
        //            }

        //            // TRAVEL DATE RANGE
        //            if (rule.TravelDateFrom.HasValue && rule.TravelDateTo.HasValue)
        //            {
        //                if (travelDate < rule.TravelDateFrom.Value ||
        //                    travelDate > rule.TravelDateTo.Value)
        //                    continue;
        //            }

        //            // BLOCK DATE RANGE
        //            if (rule.BlockODateFrom.HasValue && rule.BlockODateTo.HasValue)
        //            {
        //                //if (travelDate >= rule.BlockODateFrom.Value &&
        //                //    travelDate <= rule.BlockODateTo.Value)
        //                //{

        //                //    continue;

        //                //}

        //                if (travelDate >= rule.BlockODateFrom.Value &&
        //                    travelDate <= rule.BlockODateTo.Value)
        //                {



        //                }
        //                else
        //                {
        //                    continue;
        //                }
        //            }

        //            // PAX TYPE
        //            if (!string.IsNullOrWhiteSpace(rule.PaxType) && rule.PaxType.ToUpper() != "ANY")
        //            {
        //                if (!(rule.PaxType == "A" && search.Adult > 0 ||
        //                      rule.PaxType == "C" && search.Child > 0 ||
        //                      rule.PaxType == "I" && search.Infant > 0))
        //                    continue;
        //            }

        //            // PAX COUNT
        //            int totalPax = search.Adult + search.Child + search.Infant;
        //            if (rule.PaxCount.HasValue && totalPax < rule.PaxCount.Value)
        //                continue;

        //            // PRICE RANGE
        //            if (rule.MinimumAmountRange.min.HasValue)
        //            {
        //                decimal min = rule.MinimumAmountRange.min.Value;
        //                decimal max = rule.MinimumAmountRange.max ?? decimal.MaxValue;

        //                if (baseAmount < min || baseAmount > max)
        //                    continue;
        //            }

        //            // ✅ UPDATED STOPS LOGIC
        //            // STOPS
        //            if (rule.Stops.HasValue)
        //            {
        //                int required = rule.Stops.Value;

        //                if (required >= 0)
        //                {
        //                    // exact stops required (0 = direct, 1 = 1-stop, 2 = 2-stop)
        //                    if (stops != required)
        //                        continue;
        //                }
        //                else if (required == -1)
        //                {
        //                    // -1 means ANY stops → always allow (no continue)
        //                }
        //            }

        //            // ADVANCE / MIN / MAX DAYS
        //            if (search != null && travelDate != DateTime.MinValue)
        //            {
        //                int daysToTravel = (travelDate - DateTime.UtcNow.Date).Days;

        //                if (rule.MinDays.HasValue && daysToTravel < rule.MinDays.Value)
        //                    continue;

        //                if (rule.MaxDays.HasValue && daysToTravel > rule.MaxDays.Value)
        //                    continue;

        //                if (rule.AdvanceDays.HasValue && daysToTravel < rule.AdvanceDays.Value)
        //                    continue;
        //            }

        //            // MARKUP CALCULATION
        //            decimal markup = CalculateMarkup(rule, baseAmount);

        //            // FIND LOWEST MARKUP RULE
        //            if (markup < lowestMarkupAmount)
        //            {
        //                lowestMarkupAmount = markup;
        //                selectedRule = rule;
        //            }
        //        }

        //        // APPLY MARKUP
        //        if (lowestMarkupAmount != decimal.MaxValue && selectedRule != null)
        //        {
        //            flight.FlightFare ??= new FlightsFare();
        //            flight.FlightFare.AdultMarkup = (double)lowestMarkupAmount;
        //            flight.FlightFare.MarkupValue = lowestMarkupAmount.ToString("0.##");
        //            flight.FlightFare.MarkupType = selectedRule.MarkType;
        //            flight.FlightFare.MarkupRuleName = selectedRule.RuleName;
        //            flight.FlightFare.UpdatedBy = selectedRule.UpdatedBy;
        //            flight.TotalCost = Math.Round(flight.TotalCost + (double)lowestMarkupAmount, 2);
        //        }
        //    }
        //}


        // Apply markup for  Roundtrip 


        // Helper: pick travel date (for outbound use departure date of first outbound segment)
        private static DateTime GetPrimaryTravelDate(Flights flight, FlightsSearch search)
        {
            try
            {
                var outSeg = flight.Outbound?.FirstOrDefault();
                if (outSeg != null && DateTime.TryParse(outSeg.DepDate, out var d))
                    return d.Date;
            }
            catch { }
            // fallback to search dep date if available
            if (search != null) return search.DepDate.Date;
            return DateTime.MinValue;
        }
    }
}
