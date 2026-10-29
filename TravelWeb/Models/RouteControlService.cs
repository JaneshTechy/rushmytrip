using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using ServiceStack.Text;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Caching;
using System.Xml.Linq;
using TravelWeb.Admin.Models;
using TravelWeb.Datamodel;

namespace TravelWeb.Models
{
    // DTO for rules
    public class BlockAirlineRuleDto
    {
        public int RuleId { get; set; }
        public HashSet<string> Origins { get; set; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        public HashSet<string> Destinations { get; set; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        public string CabinClassOB { get; set; } = ""; // e.g. "Y" or "J"
        public string CabinClassIB { get; set; } = "";
        public int? TimeOBFrom { get; set; } // e.g. 600 meaning 06:00
        public int? TimeOBTo { get; set; }
        public int? TimeIBFrom { get; set; }
        public int? TimeIBTo { get; set; }
        public Dictionary<string, string> ReplaceAirlineOB { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        public Dictionary<string, string> ReplaceAirlineIB { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        public int? AdvanceDays { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public HashSet<DateTime> BlockDates { get; set; } = new HashSet<DateTime>();
        public HashSet<string> BlockAirlines { get; set; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        public bool Status { get; set; }
        // other metadata if needed
    }
    public class AllowAirlineRuleDto
    {
        public int RoutesId { get; set; }
        public HashSet<string> Origins { get; set; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        public HashSet<string> Destinations { get; set; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        public string Departur { get; set; }
        public string Arrival { get; set; }
        public bool Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string Company { get; set; } = "";
        public string CampID { get; set; } = "";
        public string UpdatedBy { get; set; } = "";
        public DateTime? UpdatedAt { get; set; }
    }
    // Service
    public static class RouteControlService
    {
        public static List<BlockAirlineRuleDto> LoadBlockRulesFromDb(string blockFor)
        {
            using (var db = new RSFLY())
            {
                var activeRules = db.BlockAirlineRules
                    .AsNoTracking()
                    .Where(r => r.Status == true && r.CampId == blockFor)
                    .ToList();

                var parsedList = activeRules.Select(r => new BlockAirlineRuleDto
                {
                    RuleId = r.RuleId,
                    Origins = ParseCsvSet(r.Origin),
                    Destinations = ParseCsvSet(r.Destination),
                    CabinClassOB = r.CabinClassOb?.Trim(),
                    CabinClassIB = r.CabinClassIb?.Trim(),
                    TimeOBFrom = ParseTimeInt(r.TimeOb?.ToString()),
                    TimeOBTo = ParseTimeInt(r.TimeOb?.ToString()),
                    TimeIBFrom = ParseTimeInt(r.TimeIb?.ToString()),
                    TimeIBTo = ParseTimeInt(r.TimeIb?.ToString()),
                    //ReplaceAirlineOB = ParseReplaceMapping(r.ReplaceAirlineOb),
                    //ReplaceAirlineIB = ParseReplaceMapping(r.ReplaceAirlineIb),
                    AdvanceDays = r.AdvanceDays,
                    DateFrom = r.DateFrom,
                    DateTo = r.DateTo,
                    BlockDates = ParseDateCsv(r.BlockDates),
                    BlockAirlines = ParseCsvSet(r.AirlineCodes),
                    Status = Convert.ToBoolean(r.Status)
                }).ToList();

                return parsedList;
            }
        }

        

        public static List<AllowAirlineRuleDto> LoadAllowRulesFromDb(string blockFor)
            {
                using var db = new RSFLY();

                var activeRules = db.AirlineRoutes
                    .AsNoTracking()
                    .Where(r => r.Status == true && r.CampId == blockFor)
                    .ToList();

                return activeRules.Select(r => new AllowAirlineRuleDto
                {
                    RoutesId = r.RoutesId,
                    Origins = ParseCsvSet(r.Origin),
                    Destinations = ParseCsvSet(r.Destination),
                    Status = r.Status == true,
                    CreatedAt = r.CreatedAt,
                    Company = r.Company ?? "",
                    CampID = r.CampId ?? "",
                    UpdatedBy = r.UpdatedBy ?? "",
                    UpdatedAt = r.UpdatedAt,
                }).ToList();
            }
        // Parse helper utilities
        private static HashSet<string> ParseCsvSet(string input)
        {
            var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (string.IsNullOrWhiteSpace(input)) return set;
            foreach (var p in input.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries))
                set.Add(p.Trim().ToUpperInvariant());
            return set;
        }
        private static Dictionary<string, string> ParseReplaceMapping(string input)
        {
            var dic = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (string.IsNullOrWhiteSpace(input)) return dic;
            foreach (var pair in input.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var tok = pair.Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToArray();
                if (tok.Length == 2)
                {
                    dic[tok[0].ToUpperInvariant()] = tok[1].ToUpperInvariant();
                }
                else if (tok.Length == 1)
                {
                    // if only single token, treat as block-only or no-op; here ignore
                }
            }
            return dic;
        }
        private static HashSet<DateTime> ParseDateCsv(string input)
        {
            var set = new HashSet<DateTime>();
            if (string.IsNullOrWhiteSpace(input)) return set;
            foreach (var s in input.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (DateTime.TryParse(s.Trim(), out var d))
                    set.Add(d.Date);
            }
            return set;
        }
        private static int? ParseTimeInt(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return null;
            if (int.TryParse(input.Trim(), out var v)) return v;
            return null;
        }
        // convert int like 600 -> TimeSpan 06:00
        private static TimeSpan? IntToTimeSpan(int? hhmm)
        {
            if (!hhmm.HasValue) return null;
            var v = hhmm.Value;
            int hours = v / 100;
            int minutes = v % 100;
            if (hours < 0 || hours > 23 || minutes < 0 || minutes > 59) return null;
            return new TimeSpan(hours, minutes, 0);
        }

        public class AirportMaster
        {
            public string AirportCode { get; set; } = "";
            public string CityCode { get; set; } = "";
            public string CityName { get; set; } = "";

            public string CountryCode { get; set; } = "";
            public string CountryName { get; set; } = "";

        }

        private static readonly object _airportIndexLock = new object();
        private static Dictionary<string, AirportMaster>? _airportIndex = null;

        // Path to your uploaded XML (change if different). The assistant used your uploaded file.
        // You can keep this path as-is if you stored the file at this location: /mnt/data/a8dcfe57-7aa9-4dfd-b49c-eb4353ef396a.xml
        private const string AirportXmlPath = "https://www.rushmyflight.com/xml/city.xml";
        private static string NormalizeForCompare(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return "";
            var cleaned = new string(s.Where(ch => char.IsLetterOrDigit(ch)).ToArray());
            return cleaned.ToUpperInvariant();
        }

        public static void EnsureAirportIndexLoaded()
        {
            if (_airportIndex != null) return;
            lock (_airportIndexLock)
            {
                if (_airportIndex != null) return;

                var dict = new Dictionary<string, AirportMaster>(StringComparer.OrdinalIgnoreCase);

                try
                {
                    var doc = XDocument.Load(AirportXmlPath);
                    var cities = doc.Descendants("City");
                    int count = 0;
                    foreach (var c in cities)
                    {
                        var code = ((string?)c.Element("Code") ?? "").Trim().ToUpperInvariant();
                        var cityCode = ((string?)c.Element("CityCode") ?? "").Trim().ToUpperInvariant();
                        var cityName = ((string?)c.Element("CityName") ?? "").Trim();
                        var countryCode = ((string?)c.Element("CountryCode") ?? "").Trim().ToUpperInvariant();
                        var countryName = ((string?)c.Element("CountryName") ?? "").Trim();

                        if (!string.IsNullOrWhiteSpace(code))
                        {
                            var am = new AirportMaster
                            {
                                AirportCode = code,
                                CityCode = cityCode,
                                CityName = cityName,
                                CountryCode = countryCode,
                                CountryName = countryName
                            };
                            if (!dict.ContainsKey(am.AirportCode))
                            {
                                dict.Add(am.AirportCode, am);
                                count++;
                            }
                        }
                    }

                    Console.WriteLine($"[EnsureAirportIndexLoaded] Loaded {count} airport records from {AirportXmlPath}");
                    _airportIndex = dict;
                }
                catch (Exception ex)
                {
                    _airportIndex = new Dictionary<string, AirportMaster>(StringComparer.OrdinalIgnoreCase);
                    Console.WriteLine($"[AirportIndexLoadError] {ex.Message}");
                    return;
                }
            }
        }

        /// <summary>
        /// If the given code is an airport code (JFK,LAX) return it.
        /// If it's a city code (NYC) or isn't an exact airport key, try to find a representative airport code in that city.
        /// Fallback: return input uppercase.
        /// </summary>
        private static string NormalizeAirportCodeForLookup(string code)
        {
            if (string.IsNullOrWhiteSpace(code)) return "";
            var c = code.Trim().ToUpperInvariant();
            EnsureAirportIndexLoaded();

            // exact airport key
            if (_airportIndex != null && _airportIndex.ContainsKey(c)) return c;

            // treat as city-code or city-name: find first airport with matching CityCode
            if (_airportIndex != null)
            {
                var byCityCode = _airportIndex.Values.FirstOrDefault(a => !string.IsNullOrWhiteSpace(a.CityCode) && a.CityCode.Equals(c, StringComparison.OrdinalIgnoreCase));
                if (byCityCode != null) return byCityCode.AirportCode;

                // if user passed full city name (e.g., "New York") or country name, try normalized compare
                var byCityName = _airportIndex.Values.FirstOrDefault(a => !string.IsNullOrWhiteSpace(a.CityName) && NormalizeForCompare(a.CityName) == NormalizeForCompare(c));
                if (byCityName != null) return byCityName.AirportCode;
            }

            // fallback to original (uppercase)
            return c;
        }

        private static bool RuleMatchesLocation(string ruleValue, string airportCode)
        {
            if (string.IsNullOrWhiteSpace(ruleValue) || string.IsNullOrWhiteSpace(airportCode)) return false;

            EnsureAirportIndexLoaded();

            var rv = ruleValue.Trim().ToUpperInvariant();
            // airportCode may be a city-code; normalize lookup first
            var acLookup = NormalizeAirportCodeForLookup(airportCode); // ensures we have an airport key if possible

            // direct airport match (rule specified airport)
            if (rv == acLookup) return true;

            // try lookup using acLookup
            if (_airportIndex != null && _airportIndex.TryGetValue(acLookup, out var info))
            {
                if (!string.IsNullOrWhiteSpace(info.CityCode) && rv == info.CityCode) return true;
                if (!string.IsNullOrWhiteSpace(info.CountryCode) && rv == info.CountryCode) return true;

                // match by normalized names (allow 'United States' etc.)
                if (!string.IsNullOrWhiteSpace(info.CountryName) && NormalizeForCompare(rv) == NormalizeForCompare(info.CountryName)) return true;
                if (!string.IsNullOrWhiteSpace(info.CityName) && NormalizeForCompare(rv) == NormalizeForCompare(info.CityName)) return true;
            }

            // As last resort: if ruleValue itself looks like a city and there exists an airport whose CityCode equals ruleValue,
            // and airportCode (original) equals some airport in that city, treat it as a match.
            if (_airportIndex != null)
            {
                // if ruleValue is a city code, and original airportCode belongs to same city
                var anyAirportWithRuleCity = _airportIndex.Values.FirstOrDefault(a => !string.IsNullOrWhiteSpace(a.CityCode) && a.CityCode.Equals(rv, StringComparison.OrdinalIgnoreCase));
                if (anyAirportWithRuleCity != null)
                {
                    // check if the original airport (acLookup) is one of that city's airports (we already checked acLookup info)
                    if (_airportIndex.TryGetValue(acLookup, out var acInfo))
                    {
                        if (!string.IsNullOrWhiteSpace(acInfo.CityCode) && acInfo.CityCode.Equals(rv, StringComparison.OrdinalIgnoreCase))
                            return true;
                    }
                }
            }

            return false;
        }

        // MAIN method to call after building FlightsList but before markup/paging
        //    public static void ApplyBlockAirlineRules(FlightsResult flightsResult, FlightsSearch search, List<BlockAirlineRuleDto> rules)
        //    {
        //        if (flightsResult == null || flightsResult.FlightsList == null || !flightsResult.FlightsList.Any()) return;
        //        if (rules == null || !rules.Any()) return;

        //        // Pre-calc today's date for AdvanceDays checks
        //        DateTime today = DateTime.UtcNow.Date; // or DateTime.Now if you prefer local

        //        // We'll produce a new filtered list
        //        var output = new List<Flights>();

        //        foreach (var flight in flightsResult.FlightsList)
        //        {
        //            bool blocked = false;
        //            // travel date - pick outbound primary
        //            DateTime travelDate = GetPrimaryTravelDateSafe(flight, search);
        //            // airline for this flight (validating or first segment)
        //            string flightAirline = (flight.Airline?.Code ?? "").ToUpperInvariant();

        //            foreach (var rule in rules)
        //            {
        //                if (!rule.Status) continue; 

        //                bool originOk = !rule.Origins.Any() ||
        //(flight.Outbound?.Any() == true && rule.Origins.Contains(flight.Outbound.First().FromAirport.Code)) ||
        //(flight.Inbound?.Any() == true && rule.Origins.Contains(flight.Inbound.First().FromAirport.Code));

        //                bool destOk = !rule.Destinations.Any() ||
        //                    (flight.Outbound?.Any() == true && rule.Destinations.Contains(flight.Outbound.Last().ToAirport.Code)) ||
        //                    (flight.Inbound?.Any() == true && rule.Destinations.Contains(flight.Inbound.Last().ToAirport.Code));

        //                if (originOk || destOk)
        //                {
        //                    blocked = true;
        //                    continue;

        //                }

        //                // 2) date range check
        //                if (rule.DateFrom.HasValue && rule.DateTo.HasValue)
        //                {
        //                    if (travelDate < rule.DateFrom.Value.Date || travelDate > rule.DateTo.Value.Date) continue;
        //                }

        //                //// 3) advance days check
        //                //if (rule.AdvanceDays.HasValue)
        //                //{
        //                //    var daysToTravel = (int)(travelDate - today).TotalDays;
        //                //    if (daysToTravel < rule.AdvanceDays.Value) continue;
        //                //}

        //                // 4) block specific dates check
        //                if (rule.BlockDates != null && rule.BlockDates.Any())
        //                {
        //                    if (rule.BlockDates.Contains(travelDate.Date))
        //                    {
        //                        // if blockdate matched -> block flight
        //                        blocked = true;
        //                        break;
        //                    }
        //                }

        //                // 5) cabin checks outbound/inbound (if specified)
        //                if (!string.IsNullOrWhiteSpace(rule.CabinClassOB))
        //                {
        //                    var cabin = flight.Outbound.FirstOrDefault()?.CabinClass ?? "";
        //                    if (!string.Equals(cabin, rule.CabinClassOB, StringComparison.OrdinalIgnoreCase)) continue;
        //                }
        //                if (!string.IsNullOrWhiteSpace(rule.CabinClassIB) && flight.Inbound.Any())
        //                {
        //                    var cabinIB = flight.Inbound.FirstOrDefault()?.CabinClass ?? "";
        //                    if (!string.Equals(cabinIB, rule.CabinClassIB, StringComparison.OrdinalIgnoreCase)) continue;
        //                }

        //                // 6) time windows check - compare first segment departure time of outbound/inbound
        //                var outboundDep = TryParseTimeFromString(flight.Outbound.FirstOrDefault()?.DepDate);
        //                var inboundDep = TryParseTimeFromString(flight.Inbound.FirstOrDefault()?.DepDate);

        //                var obFrom = IntToTimeSpan(rule.TimeOBFrom);
        //                var obTo = IntToTimeSpan(rule.TimeOBTo);
        //                var ibFrom = IntToTimeSpan(rule.TimeIBFrom);
        //                var ibTo = IntToTimeSpan(rule.TimeIBTo);

        //                if (obFrom.HasValue && obTo.HasValue && outboundDep.HasValue)
        //                {
        //                    // check if outboundDep time lies within [obFrom, obTo]
        //                    var t = outboundDep.Value.TimeOfDay;
        //                    if (!(t >= obFrom.Value && t <= obTo.Value)) continue;
        //                }
        //                if (ibFrom.HasValue && ibTo.HasValue && inboundDep.HasValue)
        //                {
        //                    var t = inboundDep.Value.TimeOfDay;
        //                    if (!(t >= ibFrom.Value && t <= ibTo.Value)) continue;
        //                }

        //                // 7) BlockAirlines: if flight airline is in rule.BlockAirlines -> block flight
        //                if (rule.BlockAirlines != null && rule.BlockAirlines.Any())
        //                {
        //                    // check any segment airline or validating airline
        //                    bool foundBlocked = false;
        //                    if (rule.BlockAirlines.Contains(flightAirline)) foundBlocked = true;
        //                    if (!foundBlocked)
        //                    {
        //                        // also check segments for operating carriers
        //                        foreach (var seg in flight.Outbound.Concat(flight.Inbound))
        //                        {
        //                            if (seg?.Airline?.Code != null && rule.BlockAirlines.Contains(seg.Airline.Code.ToUpperInvariant()))
        //                            {
        //                                foundBlocked = true; break;
        //                            }
        //                        }
        //                    }
        //                    if (foundBlocked)
        //                    {
        //                        blocked = true;
        //                        break;
        //                    }
        //                }

        //                // 8) ReplaceAirline mappings: if flightAirline matches a key, replace it
        //                if (rule.ReplaceAirlineOB != null && rule.ReplaceAirlineOB.TryGetValue(flightAirline, out var newAirline))
        //                {
        //                    // apply replacement on flight and outbound segments
        //                    flight.Airline.Code = newAirline;
        //                    foreach (var seg in flight.Outbound)
        //                    {
        //                        if (!string.IsNullOrWhiteSpace(seg.Airline?.Code))
        //                            seg.Airline.Code = newAirline;
        //                        if (!string.IsNullOrWhiteSpace(seg.OperatingAirline?.Code))
        //                            seg.OperatingAirline.Code = newAirline;
        //                    }
        //                }
        //                if (rule.ReplaceAirlineIB != null && flight.Inbound.Any() && rule.ReplaceAirlineIB.TryGetValue(flightAirline, out var newAirlineIB))
        //                {
        //                    flight.Airline.Code = newAirlineIB;
        //                    foreach (var seg in flight.Inbound)
        //                    {
        //                        if (!string.IsNullOrWhiteSpace(seg.Airline?.Code))
        //                            seg.Airline.Code = newAirlineIB;
        //                        if (!string.IsNullOrWhiteSpace(seg.OperatingAirline?.Code))
        //                            seg.OperatingAirline.Code = newAirlineIB;
        //                    }
        //                }

        //                // If we reached here and not blocked, then rule matched and did replacement (if any).
        //                // We do NOT automatically block unless BlockDates/BlockAirlines indicated so.
        //                // Continue to next rule because other rules might also block. However, if you want first-match semantics,
        //                // you can break here.
        //            } // foreach rule

        //            if (!blocked) output.Add(flight);
        //            // else drop flight (do not add)
        //        } // foreach flight

        //        flightsResult.FlightsList = output;
        //    }
        public static void ApplyBlockAirlineRules(FlightsResult flightsResult, FlightsSearch search, List<BlockAirlineRuleDto> rules)
        {
            // safety serializations (you had these)
            

            if (flightsResult == null || flightsResult.FlightsList == null || !flightsResult.FlightsList.Any()) return;
            if (rules == null || !rules.Any()) return;

            EnsureAirportIndexLoaded();

            DateTime today = DateTime.UtcNow.Date;
            var output = new List<Flights>();

            foreach (var flight in flightsResult.FlightsList)
            {
                bool blocked = false;
                DateTime travelDate = GetPrimaryTravelDateSafe(flight, search);
                string flightAirline = (flight.Airline?.Code ?? "").ToUpperInvariant();

                // original values from flight
                var fromAirportRaw = flight.Outbound.FirstOrDefault()?.FromAirport?.Code ?? "";
                var toAirportRaw = flight.Outbound.LastOrDefault()?.ToAirport?.Code ?? "";

                // normalize for lookup (maps NYC -> a representative airport like JFK)
                var fromLookup = NormalizeAirportCodeForLookup(fromAirportRaw);
                var toLookup = NormalizeAirportCodeForLookup(toAirportRaw);

                //Console.WriteLine($"[DBG] Flight {flight.FlightId} rawFrom='{fromAirportRaw}' lookupFrom='{fromLookup}' rawTo='{toAirportRaw}' lookupTo='{toLookup}' travelDate={travelDate:yyyy-MM-dd}");
                var fromTokens = BuildFlightTokensFromAirportCode(fromAirportRaw);
                var toTokens = BuildFlightTokensFromAirportCode(toAirportRaw);
                foreach (var rule in rules)
                {
                    if (!rule.Status) continue;

                    //Console.WriteLine($"[DBG] Checking Rule {rule.RuleId} Origins=[{string.Join(",", rule.Origins)}] Dests=[{string.Join(",", rule.Destinations)}] DateFrom={rule.DateFrom?.ToString("yyyy-MM-dd")} DateTo={rule.DateTo?.ToString("yyyy-MM-dd")} ");

                    

                    bool isRoundTrip = search.FlightWay == FlightWay.RoundTrip;

                    bool originDestMatch = BlockRuleMatchesV1(rule, fromTokens, toTokens,search);

                    if (!originDestMatch && isRoundTrip)
                    {
                        originDestMatch = BlockRuleMatchesV1(rule, toTokens, fromTokens);
                    }

                    //Console.WriteLine($"[DBG]  originDestMatch={originDestMatch}");

                    if (!originDestMatch)
                        continue;

                    // date range
                    if (rule.DateFrom.HasValue && rule.DateTo.HasValue)
                    {
                        if (travelDate >= rule.DateFrom.Value.Date &&
                            travelDate <= rule.DateTo.Value.Date)
                        {
                            //Console.WriteLine("[DBG] Travel date falls in blocked range");
                            blocked = true;
                            //Console.WriteLine($"[ApplyBlock] Flight {flight.FlightId} DROPPED by Rule {rule.RuleId} due to DateRange");
                            break;
                        }
                    }

                    // blockdates
                    if (rule.BlockDates != null && rule.BlockDates.Any() && rule.BlockDates.Contains(travelDate.Date))
                    {
                        blocked = true;
                        //Console.WriteLine($"[ApplyBlock] Flight {flight.FlightId} DROPPED by Rule {rule.RuleId} due to BlockDates");
                        break;
                    }

                    // cabin checks (unchanged)
                    if (!string.IsNullOrWhiteSpace(rule.CabinClassOB))
                    {
                        var cabin = flight.Outbound.FirstOrDefault()?.CabinClass ?? "";
                        if (!string.Equals(cabin, rule.CabinClassOB, StringComparison.OrdinalIgnoreCase)) { Console.WriteLine($"[DBG]  CabinMismatch OB"); continue; }
                    }
                    if (!string.IsNullOrWhiteSpace(rule.CabinClassIB) && flight.Inbound.Any())
                    {
                        var cabinIB = flight.Inbound.FirstOrDefault()?.CabinClass ?? "";
                        if (!string.Equals(cabinIB, rule.CabinClassIB, StringComparison.OrdinalIgnoreCase)) { Console.WriteLine($"[DBG]  CabinMismatch IB"); continue; }
                    }

                    // time windows (unchanged)
                    var outboundDep = TryParseTimeFromString(flight.Outbound.FirstOrDefault()?.DepDate);
                    var inboundDep = TryParseTimeFromString(flight.Inbound.FirstOrDefault()?.DepDate);

                    var obFrom = IntToTimeSpan(rule.TimeOBFrom);
                    var obTo = IntToTimeSpan(rule.TimeOBTo);
                    var ibFrom = IntToTimeSpan(rule.TimeIBFrom);
                    var ibTo = IntToTimeSpan(rule.TimeIBTo);

                    if (obFrom.HasValue && obTo.HasValue && outboundDep.HasValue)
                    {
                        var t = outboundDep.Value.TimeOfDay;
                        if (!(t >= obFrom.Value && t <= obTo.Value)) { Console.WriteLine($"[DBG]  TimeWindowMismatch OB"); continue; }
                    }
                    if (ibFrom.HasValue && ibTo.HasValue && inboundDep.HasValue)
                    {
                        var t = inboundDep.Value.TimeOfDay;
                        if (!(t >= ibFrom.Value && t <= ibTo.Value)) { Console.WriteLine($"[DBG]  TimeWindowMismatch IB"); continue; }
                    }

                    // BlockAirlines (unchanged)
                    if (rule.BlockAirlines != null && rule.BlockAirlines.Any())
                    {
                        bool foundBlocked = false;
                        if (rule.BlockAirlines.Contains(flightAirline)) foundBlocked = true;
                        if (!foundBlocked)
                        {
                            foreach (var seg in flight.Outbound.Concat(flight.Inbound))
                            {
                                if (seg?.Airline?.Code != null && rule.BlockAirlines.Contains(seg.Airline.Code.ToUpperInvariant()))
                                {
                                    foundBlocked = true; break;
                                }
                                if (seg?.OperatingAirline?.Code != null && rule.BlockAirlines.Contains(seg.OperatingAirline.Code.ToUpperInvariant()))
                                {
                                    foundBlocked = true; break;
                                }
                            }
                        }
                        if (foundBlocked)
                        {
                            blocked = true;
                            //Console.WriteLine($"[ApplyBlock] Flight {flight.FlightId} DROPPED by Rule {rule.RuleId} due to BlockAirlines");
                            break;
                        }
                    }

                    // ReplaceAirline mappings (unchanged)
                    if (rule.ReplaceAirlineOB != null && rule.ReplaceAirlineOB.TryGetValue(flightAirline, out var newAirline))
                    {
                        flight.Airline.Code = newAirline;
                        foreach (var seg in flight.Outbound)
                        {
                            if (!string.IsNullOrWhiteSpace(seg.Airline?.Code))
                                seg.Airline.Code = newAirline;
                            if (!string.IsNullOrWhiteSpace(seg.OperatingAirline?.Code))
                                seg.OperatingAirline.Code = newAirline;
                        }
                    }
                    if (rule.ReplaceAirlineIB != null && flight.Inbound.Any() && rule.ReplaceAirlineIB.TryGetValue(flightAirline, out var newAirlineIB))
                    {
                        flight.Airline.Code = newAirlineIB;
                        foreach (var seg in flight.Inbound)
                        {
                            if (!string.IsNullOrWhiteSpace(seg.Airline?.Code))
                                seg.Airline.Code = newAirlineIB;
                            if (!string.IsNullOrWhiteSpace(seg.OperatingAirline?.Code))
                                seg.OperatingAirline.Code = newAirlineIB;
                        }
                    }

                    // If we reached here, the rule matched origin/destination and passed other checks -> BLOCK
                    //blocked = true;
                    //Console.WriteLine($"[ApplyBlock] Flight {flight.FlightId} DROPPED by Rule {rule.RuleId} (originToken match or destToken match)");
                    //break;
                } // foreach rule

                if (!blocked) output.Add(flight);
                /*else*/ /*Console.WriteLine($"[ApplyBlock] Flight {flight.FlightId} removed from results.");*/
            } // foreach flight

            flightsResult.FlightsList = output;
        }


        //public static void ApplyAllowAirlineRules(FlightsResult flightsResult, FlightsSearch search, List<AllowAirlineRuleDto> rules)
        //{
        //    if (flightsResult?.FlightsList == null || !flightsResult.FlightsList.Any()) return;
        //    if (rules == null || !rules.Any()) return;

        //    var output = new List<Flights>();

        //    foreach (var flight in flightsResult.FlightsList)
        //    {
        //        bool allowed = false;

        //        string origin = flight.Outbound.FirstOrDefault()?.FromAirport?.Code?.ToUpperInvariant() ?? "";
        //        string destination = flight.Outbound.LastOrDefault()?.ToAirport?.Code?.ToUpperInvariant() ?? "";

        //        if (string.IsNullOrEmpty(origin) || string.IsNullOrEmpty(destination))
        //            continue;

        //        foreach (var rule in rules)
        //        {
        //            if (!rule.Status) continue;

        //            bool originOk = !rule.Origins.Any() || rule.Origins.Contains(origin);
        //            bool destOk = !rule.Destinations.Any() || rule.Destinations.Contains(destination);

        //            if (originOk && destOk)
        //            {
        //                allowed = true;
        //                break;
        //            }
        //        }

        //        if (allowed)
        //            output.Add(flight);
        //    }

        //    flightsResult.FlightsList = output;
        //}
        public static bool ApplyAllowAirlineRules(
            FlightsSearch search,
            List<AllowAirlineRuleDto> rules)
        {
            if (search?.From == null || search?.To == null)
                return false;

            if (rules == null || rules.Count == 0)
                return false;

            var fromTokens = BuildSearchTokens(search.From);
            var toTokens = BuildSearchTokens(search.To);

            bool isRoundTrip = search.FlightWay == FlightWay.RoundTrip;

            foreach (var rule in rules)
            {
                if (!rule.Status)
                    continue;

                // OUTBOUND: From → To
                if (RuleMatches(rule, fromTokens, toTokens))
                    return true;

                // INBOUND (RoundTrip only): To → From
                //if (isRoundTrip && RuleMatches(rule, toTokens, fromTokens))
                //    return true;
            }

            return false;
        }
        private static bool RuleMatches(
            AllowAirlineRuleDto rule,
            HashSet<string> originTokens,
            HashSet<string> destinationTokens, FlightsSearch search = null)
        {
            bool originMatch =
                //rule.Origins == null || rule.Origins.Count == 0 ||
                rule.Origins.Any(o => originTokens.Contains(o));

            if (!originMatch)
                return false;

            bool destinationMatch =
                //rule.Destinations == null || rule.Destinations.Count == 0 ||
                rule.Destinations.Any(d => destinationTokens.Contains(d));

            return destinationMatch;
        }

        private static bool BlockRuleMatches(
            BlockAirlineRuleDto rule,
            HashSet<string> originTokens,
            HashSet<string> destinationTokens, FlightsSearch search = null)
        {
            bool originMatch;
            bool destinationMatch;
            if (search != null)
            {
                originMatch =
                               rule.Origins == null || rule.Origins.Count == 0 ||
                               rule.Origins.Any(o => o.Contains(search.From.CountryCode));

                if (!originMatch)
                    return false;

                destinationMatch =
                   rule.Destinations == null || rule.Destinations.Count == 0 ||
                   rule.Destinations.Any(d => d.Contains(search.To.CountryCode));
                return destinationMatch;
            }
            originMatch =
               rule.Origins == null || rule.Origins.Count == 0 ||
               rule.Origins.Any(o => originTokens.Contains(o));

            if (!originMatch)
                return false;

            destinationMatch =
               rule.Destinations == null || rule.Destinations.Count == 0 ||
               rule.Destinations.Any(d => destinationTokens.Contains(d));

            return destinationMatch;
        }
        private static bool BlockRuleMatchesV1(
    BlockAirlineRuleDto rule,
    HashSet<string> originTokens,
    HashSet<string> destinationTokens,
    FlightsSearch search = null)
        {
            if (search != null)
            {
                var fromTokens = BuildAirportTokens(search.From);
                var toTokens = BuildAirportTokens(search.To);

                bool originMatch =
                    rule.Origins == null || rule.Origins.Count == 0 ||
                    rule.Origins.Any(o => fromTokens.Contains(o?.Trim().ToUpper()));

                if (!originMatch)
                    return false;

                bool destinationMatch =
                    rule.Destinations == null || rule.Destinations.Count == 0 ||
                    rule.Destinations.Any(d => toTokens.Contains(d?.Trim().ToUpper()));

                return destinationMatch;
            }

            bool normalOriginMatch =
                rule.Origins == null || rule.Origins.Count == 0 ||
                rule.Origins.Any(o => originTokens.Contains(o?.Trim().ToUpper()));

            if (!normalOriginMatch)
                return false;

            bool normalDestinationMatch =
                rule.Destinations == null || rule.Destinations.Count == 0 ||
                rule.Destinations.Any(d => destinationTokens.Contains(d?.Trim().ToUpper()));

            return normalDestinationMatch;
        }

        private static HashSet<string> BuildAirportTokens(Airport airport)
        {
            var tokens = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            if (airport == null)
                return tokens;

            if (!string.IsNullOrWhiteSpace(airport.Code))
                tokens.Add(airport.Code.Trim().ToUpper());

            if (!string.IsNullOrWhiteSpace(airport.CityCode))
                tokens.Add(airport.CityCode.Trim().ToUpper());

            if (!string.IsNullOrWhiteSpace(airport.CountryCode))
                tokens.Add(airport.CountryCode.Trim().ToUpper());

            return tokens;
        }
        public static HashSet<string> BuildSearchTokens(Airport airport)
        {
            return new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        airport.Code,
        airport.CityCode,
        airport.CityName,
        airport.StateCode,
        airport.StateName,
        airport.CountryCode,
        airport.CountryName,
        airport.Continent
    }
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Trim())
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        }

        public static void ApplyAllowAirlineRules1(FlightsResult flightsResult, FlightsSearch search, List<AllowAirlineRuleDto> rules)
        {
            if (flightsResult?.FlightsList == null || !flightsResult.FlightsList.Any()) return;
            if (rules == null || !rules.Any()) return;

            var output = new List<Flights>();

            foreach (var flight in flightsResult.FlightsList)
            {
                bool allowed = false;

                string origin = flight.Outbound.FirstOrDefault()?.FromAirport?.Code?.ToUpperInvariant() ?? "";
                string destination = flight.Outbound.LastOrDefault()?.ToAirport?.Code?.ToUpperInvariant() ?? "";

                if (string.IsNullOrEmpty(origin) || string.IsNullOrEmpty(destination))
                    continue;

                foreach (var rule in rules)
                {
                    if (!rule.Status) continue;

                    bool originOk = !rule.Origins.Any() || rule.Origins.Contains(origin);
                    bool destOk = !rule.Destinations.Any() || rule.Destinations.Contains(destination);

                    if (originOk && destOk)
                    {
                        allowed = true;
                        break;
                    }
                }

                if (allowed)
                    output.Add(flight);
            }

            flightsResult.FlightsList = output;
        }

        // helpers
        private static DateTime GetPrimaryTravelDateSafe(Flights flight, FlightsSearch search)
        {
            if (flight?.Outbound?.FirstOrDefault()?.DepDate != null)
            {
                if (DateTime.TryParse(flight.Outbound.First().DepDate, out var d)) return d.Date;
            }
            if (search != null) return search.DepDate.Date;
            return DateTime.UtcNow.Date;
        }

        private static DateTime? TryParseTimeFromString(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return null;
            if (DateTime.TryParse(s, out var d)) return d;
            return null;
        }
        // ================= BLOCK RULE TOKEN HELPERS =================

        private static HashSet<string> BuildFlightTokensFromAirportCode(string airportCode)
        {
            if (string.IsNullOrWhiteSpace(airportCode))
                return new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            EnsureAirportIndexLoaded();

            var normalized = NormalizeAirportCodeForLookup(airportCode);

            if (_airportIndex != null && _airportIndex.TryGetValue(normalized, out var airport))
            {
                return new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            airport.AirportCode,
            airport.CityCode,
            airport.CityName,
            airport.CountryCode,
            airport.CountryName
        }
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim())
                .ToHashSet(StringComparer.OrdinalIgnoreCase);
            }

            // fallback: just airport code
            return new HashSet<string>(StringComparer.OrdinalIgnoreCase) { normalized };
        }

        private static bool RuleMatches(
            BlockAirlineRuleDto rule,
            HashSet<string> originTokens,
            HashSet<string> destinationTokens)
        {
            bool originMatch =
                rule.Origins == null || rule.Origins.Count == 0 ||
                rule.Origins.Any(o => originTokens.Contains(o));

            if (!originMatch)
                return false;

            bool destinationMatch =
                rule.Destinations == null || rule.Destinations.Count == 0 ||
                rule.Destinations.Any(d => destinationTokens.Contains(d));

            return destinationMatch;
        }

    }

}
