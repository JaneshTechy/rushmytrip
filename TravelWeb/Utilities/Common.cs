using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using ServiceStack.Redis;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using TravelWeb.Helper;
using TravelWeb.Models;
using TravelWeb.Services;


namespace TravelWeb.Utilities
{
    public class Common
    {

        public static string WebSiteDomain = "";
        public static string FolderName = "";
        //public static string WebSitePath = "";
        public static string SourceMedia = "";
        public static string WebApiService = "";
        public static string ConnectionString = "";
        public static string EmailAddress = "";
        public static string Password = "";
        public static string Host = "";
        public static string BusinessMailTo = "";
        public static DataSet GetAirportDataX = null;
        public static DataSet GetAirlineDataX = null;
        public static string IsProduction;
        public static string BccMail;
        public static string TagLine;
        public static string Destination;
        public static string BaggageLine;
        public static string BaggageAllownce;
        public static string MainCabinAllowed;
        public static string SeatPrefrenceAmount;
        //public static string InfantTaxRoundTrip;
        //public static string InfantTaxOneWay;
        private const string TargetBranch = "P1885281";
        private const string ProviderCode = "1P";
        private const string Endpoint = "https://emea.universal-api.travelport.com/B2BGateway/connect/uAPI/AirService";
        private readonly string _username;
        private readonly string _password;
        private static IHttpContextAccessor _httpContextAccessor;
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private static IDistributedCache _cache;
        private static string _webRootPath;
        private static readonly object _airportLock = new object();


        public static void Configure(IHttpContextAccessor accessor, IDistributedCache cache, string webRootPath = null)
        {
            _httpContextAccessor = accessor;
            _cache = cache;
            if (!string.IsNullOrWhiteSpace(webRootPath))
                _webRootPath = webRootPath;
        }

        /// <summary>
        /// Prefer local wwwroot/xml file (fast). Fall back to site URL only if needed.
        /// </summary>
        private static string ResolveAirportXmlPath()
        {
            if (!string.IsNullOrWhiteSpace(_webRootPath))
            {
                var cityXml = Path.Combine(_webRootPath, "xml", "City.xml");
                if (File.Exists(cityXml))
                    return cityXml;

                var cityXmlLower = Path.Combine(_webRootPath, "xml", "city.xml");
                if (File.Exists(cityXmlLower))
                    return cityXmlLower;
            }

            return WebSitePath + "xml/city.xml";
        }

        public static void WarmAirportCache()
        {
            EnsureAirportsLoaded();
        }

        private static void EnsureAirportsLoaded()
        {
            if (_allAirports != null)
                return;

            lock (_airportLock)
            {
                if (_allAirports != null)
                    return;

                string xmlPath = ResolveAirportXmlPath();
                DataSet ds = new DataSet();
                ds.ReadXml(xmlPath);

                _allAirports = ds.Tables[0].AsEnumerable()
                    .Select(dr => new Airport
                    {
                        Code = dr["Code"]?.ToString() ?? "",
                        Name = dr["Name"]?.ToString() ?? "",
                        CityCode = dr["CityCode"]?.ToString() ?? "",
                        CityName = dr["CityName"]?.ToString() ?? "",
                        StateCode = dr["StateCode"]?.ToString() ?? "",
                        StateName = dr["StateName"]?.ToString() ?? "",
                        CountryCode = dr["CountryCode"]?.ToString() ?? "",
                        CountryName = dr["CountryName"]?.ToString() ?? "",
                        Continent = dr["Continent"]?.ToString() ?? "",
                        Latitude = dr["Latitude"]?.ToString() ?? "",
                        Longtitude = dr["Longtitude"]?.ToString() ?? ""
                    }).ToList();
            }
        }
        public static string WebSitePath
        {
            get
            {
                var req = _httpContextAccessor?.HttpContext?.Request;
                if (req == null)
                    return "/";

                return $"{req.Scheme}://{req.Host}/";
            }
        }
        public static void SaveSession(string key, string value)
        {
            _httpContextAccessor?.HttpContext?.Session.SetString(key, value);
        }

        public static string GetSession(string key)
        {
            return _httpContextAccessor?.HttpContext?.Session.GetString(key);
        }
        public static void SaveObjectSession<T>(string key, T obj)
        {
            var json = JsonConvert.SerializeObject(obj);
            _httpContextAccessor?.HttpContext?.Session.SetString(key, json);

            // ✅ Also save in in-memory cache for cross-client access
            FlightCache[key] = obj;
        }
        public static void ClearSession(string key)
        {
            _httpContextAccessor?.HttpContext?.Session.Remove(key);

            if (FlightCache != null)
            {
                FlightCache.TryRemove(key, out _);
            }
        }
        public static void ClearAllSessions()
        {
            _httpContextAccessor?.HttpContext?.Session.Clear();

            FlightCache?.Clear();
        }
        public static T GetObjectSession<T>(string key)
        {
            var json = _httpContextAccessor?.HttpContext?.Session.GetString(key);
            if (!string.IsNullOrEmpty(json))
                return JsonConvert.DeserializeObject<T>(json);

            // ✅ fallback to in-memory cache if session is missing
            if (FlightCache.TryGetValue(key, out var obj))
                return (T)obj;

            return default;
        }

        // ✅ In-memory cache for FlightBooking
        private static readonly ConcurrentDictionary<string, object> FlightCache = new ConcurrentDictionary<string, object>();


        public static string Encrypt(string PlainText)
        {
            string _securityKey = "PRAVEEN";

            byte[] toEncryptedArray = UTF8Encoding.UTF8.GetBytes(PlainText);
            MD5CryptoServiceProvider objMD5CryptoService = new MD5CryptoServiceProvider();
            byte[] securityKeyArray = objMD5CryptoService.ComputeHash(UTF8Encoding.UTF8.GetBytes(_securityKey));
            objMD5CryptoService.Clear();
            var objTripleDESCryptoService = new TripleDESCryptoServiceProvider();
            objTripleDESCryptoService.Key = securityKeyArray;
            objTripleDESCryptoService.Mode = CipherMode.ECB;
            objTripleDESCryptoService.Padding = PaddingMode.PKCS7;
            var objCrytpoTransform = objTripleDESCryptoService.CreateEncryptor();
            byte[] resultArray = objCrytpoTransform.TransformFinalBlock(toEncryptedArray, 0, toEncryptedArray.Length);

            objTripleDESCryptoService.Clear();

            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }
        public static string Decrypt(string CipherText)
        {
            string _securityKey = "PRAVEEN";

            byte[] toEncryptArray = Convert.FromBase64String((CipherText).Replace(" ", "+"));
            MD5CryptoServiceProvider objMD5CryptoService = new MD5CryptoServiceProvider();
            byte[] securityKeyArray = objMD5CryptoService.ComputeHash(UTF8Encoding.UTF8.GetBytes(_securityKey));
            objMD5CryptoService.Clear();
            var objTripleDESCryptoService = new TripleDESCryptoServiceProvider();
            objTripleDESCryptoService.Key = securityKeyArray;
            objTripleDESCryptoService.Mode = CipherMode.ECB;
            objTripleDESCryptoService.Padding = PaddingMode.PKCS7;
            var objCrytpoTransform = objTripleDESCryptoService.CreateDecryptor();
            byte[] resultArray = objCrytpoTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            objTripleDESCryptoService.Clear();
            return UTF8Encoding.UTF8.GetString(resultArray);
        }

        public static DataSet GetAirportData()
        {
            string xmlPath = ResolveAirportXmlPath();
            DataSet dsx = new DataSet();
            dsx.ReadXml(xmlPath);
            return dsx;
        }
        public static DataSet GetAirlineData()
        {

            string xmlPath = WebSitePath + "xml/airline.xml";
            XmlDocument xdoc = new XmlDocument();
            xdoc.Load(xmlPath);
            DataSet dsx = new DataSet();
            dsx.ReadXml(xmlPath);
            return dsx;
        }

        public static Airport GetAirport(string Code)
        {
            Airport airport = new Airport();
            DataTable dt = new DataTable();
            DataSet dsx = GetAirportDataX;
            //  DataSet dsx = GetAirportData();


            if (dsx == null)
            {
                dsx = GetAirportData();
            }

            DataRow[] drx;
            drx = dsx.Tables[0].Select("Code LIKE '" + Code.Replace("'", "''") + "%'");


            dt = dsx.Tables[0].Clone();
            foreach (DataRow dr in drx)
            {
                dt.ImportRow(dr);

            }
            dt.AcceptChanges();

            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];

                airport.Code = dr["Code"].ToString();
                airport.Name = dr["Name"].ToString();
                airport.CityCode = dr["CityCode"].ToString();
                airport.CityName = dr["CityName"].ToString();
                airport.StateCode = dr["StateCode"].ToString();
                airport.StateName = dr["StateName"].ToString();
                airport.CountryCode = dr["CountryCode"].ToString();
                airport.CountryName = dr["CountryName"].ToString();
                airport.Continent = dr["Continent"].ToString();
                airport.Latitude = dr["Latitude"].ToString();
                airport.Longtitude = dr["Longtitude"].ToString();
            }
            else
            {
                airport.Code = Code;
                airport.Name = Code;
                airport.CityCode = Code;
                airport.CityName = Code;
                airport.StateCode = Code;
                airport.StateName = Code;
                airport.CountryCode = Code;
                airport.CountryName = Code;
                airport.Continent = Code;
                airport.Latitude = Code;
                airport.Longtitude = Code;
            }

            return airport;
        }

        public static Airline GetAirline(string Code)
        {
            DataTable dt = new DataTable();

            Airline airline = new Airline();

            DataSet dsx = GetAirlineDataX;
            //  DataSet dsx = GetAirlineData();


            DataRow[] drx;
            drx = dsx.Tables[0].Select("Code LIKE '" + Code.Replace("'", "''") + "%'");


            dt = dsx.Tables[0].Clone();
            foreach (DataRow dr in drx)
            {
                dt.ImportRow(dr);

            }
            dt.AcceptChanges();



            //DataSet ds = SqlData.Data.SqlHelperData.ExecuteDataset(SqlHelperData.ConnectionString , CommandType.Text, "SELECT * FROM Airline WHERE Code='" + Code + "'");

            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];

                airline.Code = dr["Code"].ToString();
                airline.Name = dr["Name"].ToString();
            }
            else
            {
                airline.Code = Code;
                airline.Name = Code;
            }

            return airline;
        }

        private static List<Airport> _allAirports;

        public static List<Airport> GetAllAirport(string query)
        {
            EnsureAirportsLoaded();

            if (string.IsNullOrWhiteSpace(query))
                return _allAirports.Take(20).ToList();

            string q = query.Trim();
            if (q.Length < 2)
                return new List<Airport>();

            // Prioritized search: Code > City > Name > Country (prefix first, then contains)
            var results = _allAirports
                .Select(a =>
                {
                    int rank = 0;
                    if (!string.IsNullOrEmpty(a.Code) && a.Code.StartsWith(q, StringComparison.OrdinalIgnoreCase))
                        rank = 1;
                    else if (!string.IsNullOrEmpty(a.CityName) && a.CityName.StartsWith(q, StringComparison.OrdinalIgnoreCase))
                        rank = 2;
                    else if (!string.IsNullOrEmpty(a.Name) && a.Name.StartsWith(q, StringComparison.OrdinalIgnoreCase))
                        rank = 3;
                    else if (!string.IsNullOrEmpty(a.CountryName) && a.CountryName.StartsWith(q, StringComparison.OrdinalIgnoreCase))
                        rank = 4;
                    else if (!string.IsNullOrEmpty(a.CityName) && a.CityName.IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0)
                        rank = 5;
                    else if (!string.IsNullOrEmpty(a.Name) && a.Name.IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0)
                        rank = 6;
                    return new { Airport = a, Rank = rank };
                })
                .Where(x => x.Rank > 0)
                .OrderBy(x => x.Rank)
                .ThenBy(x => x.Airport.CityName)
                .Select(x => x.Airport)
                .Take(20)
                .ToList();

            return results;
        }

        public static DateTime ConvertDateTime(string datex)
        {

            return Convert.ToDateTime(datex);
        }


        public static bool IsLocalIpAddress(string host)
        {
            try
            { // get host IP addresses
                IPAddress[] hostIPs = Dns.GetHostAddresses(host);
                // get local IP addresses
                IPAddress[] localIPs = Dns.GetHostAddresses(Dns.GetHostName());

                // test if any host IP equals to any local IP or to localhost
                foreach (IPAddress hostIP in hostIPs)
                {
                    // is localhost
                    if (IPAddress.IsLoopback(hostIP)) return true;
                    // is local address
                    foreach (IPAddress localIP in localIPs)
                    {
                        if (hostIP.Equals(localIP)) return true;
                    }
                }
            }
            catch { }
            return false;
        }

        public static MultiCityFlightsResult MultisearchSearchFlight(MultiFlightsSearch flightSearch)
        {
            MultiCityFlightsResult flightsResult = new MultiCityFlightsResult();
            try
            {
                if (flightSearch != null)
                {
                    // string url = ApiURL + "flights/search";
                    string url = WebApiService + "multiflights/search";
                    //string url = WebApiService;

                    //http://localhost:56343/flights/search
                    string serialisedData = JsonConvert.SerializeObject(flightSearch);
                    WebClient webClient = new WebClient();
                    webClient.Headers[HttpRequestHeader.ContentType] = "application/json";
                    string result = webClient.UploadString(url, serialisedData);
                    flightsResult = JsonConvert.DeserializeObject<MultiCityFlightsResult>(result);
                    //AplicationHelper.Save("localhost", flightsResult.SearchId, result);
                    //AplicationHelper.Save("localhost", flightsResult.FlightsSearch.SearchId, result);
                    //Common.SaveSession("search_" + flightsResult.FlightsSearch.SearchId, JsonConvert.SerializeObject(result));
                    Common.SaveObjectSession("search_" + flightsResult.FlightsSearch.SearchId, result);

                }
            }
            catch (WebException ex)
            {
                string message = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
                flightsResult.Error = message;// e.Message.ToString();
                flightsResult.FlightsSearch = flightSearch;
                throw ex;

            }
            return flightsResult;
        }
        private static bool Save(string host, string key, string value)
        {
            bool isSuccess = false;
            using (RedisClient redisClient = new RedisClient(host))
            {
                if (redisClient.Get<string>(key) == null)
                {
                    //isSuccess = redisClient.Set(key, value);
                    isSuccess = redisClient.Set(key, value, TimeSpan.FromHours(2));
                }
            }
            return isSuccess;
        }
        public static Object XMLToObject(string XMLString, Object oObject)
        {

            XmlSerializer oXmlSerializer = new XmlSerializer(oObject.GetType());
            oObject = oXmlSerializer.Deserialize(new StringReader(XMLString));
            return oObject;
        }
        private static readonly string BaseUrl = "https://rushmyflight.wikiproject.in/nv3";
        //private static readonly string BaseUrl = "https://rushmyflight.wikiproject.in/";

        public static void ClearSearchSessions()
        {
            var context = _httpContextAccessor?.HttpContext;
            if (context == null) return;

            string[] prefixes =
            {
        "search_",
        "Flights",
        "FlightsSearch",
        "flightsResult"
    };

            var keysToRemove = context.Session.Keys
                .Where(k => prefixes.Any(p => k.StartsWith(p)))
                .ToList();

            foreach (var key in keysToRemove)
            {
                context.Session.Remove(key);
            }
        }

        public static FlightsResult SearchFlight(FlightsSearch flightSearch)
        {
            
            ClearAllSessions();
            FlightsResult flightsResult = new FlightsResult();

            //try
            //{
            //    var requestModel = MapToRequest(flightSearch);

            //    using (var client = new HttpClient())
            //    {
            //        client.BaseAddress = new Uri("https://localhost:44393/api/flights/");

            //        var response = client.PostAsJsonAsync("search", requestModel)
            //                             .GetAwaiter()
            //                             .GetResult();


            //        if (response.IsSuccessStatusCode)
            //        {
            //            var json = response.Content.ReadAsStringAsync()
            //               .GetAwaiter()
            //               .GetResult();
            //            var result = JsonConvert.DeserializeObject<FlightsResult>(json);
            //            result.FlightsSearch = flightSearch;
            //            return result;
            //        }
            //        else
            //        {
            //            //flightsResult.ErrorMessage = $"API failed: {response.StatusCode}";
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    //flightsResult.ErrorMessage = ex.Message;
            //}



            var FlightsList = new List<Flights>();
            try
            {
                string classCode = "Y"; // default to Economy

                if (flightSearch != null)
                {
                    switch (flightSearch.FlightClass)
                    {
                        case FlightClass.First:
                            classCode = "F";
                            break;
                        case FlightClass.Business:
                            classCode = "B";
                            break;
                        case FlightClass.EconomyPremium:
                            classCode = "P";
                            break;
                        case FlightClass.Economy:
                            classCode = "Y";
                            break;
                        default:
                            classCode = "Y";
                            break;
                    }

                    string query = $"?currency={((flightSearch.Currency ?? "USD").Equals("MXN", StringComparison.OrdinalIgnoreCase) ? "USD" : (flightSearch.Currency ?? "USD"))}" +
                           $"&JType={(flightSearch.FlightWay == FlightWay.RoundTrip ? "roundtrip" : "oneway")}" +
                           $"&org={flightSearch.From?.Code}" +
                           $"&dest={flightSearch.To?.Code}" +
                           $"&depDt={flightSearch.DepDate:MMM-dd-yyyy}" +
                           (flightSearch.FlightWay == FlightWay.RoundTrip ? $"&retDt={flightSearch.RetDate:MMM-dd-yyyy}" : "") +
                           $"&adt={flightSearch.Adult}" +
                           $"&chd={flightSearch.Child}" +
                           $"&inf={flightSearch.Infant}" +
                            //$"&ct={(flightSearch.FlightClass == FlightClass.Business ? "C" : "Y")}" + // 'Y' = Economy
                            $"&ct={classCode}" +
                           $"&userid=rushmy" +
                           $"&password=RUrM5Lf" +
                           $"&metaId=2020" +
                           //$"&cheapest=1500" +
                           $"&website=rushmyflight"/*+ $"&limit=70"*/;

                    using (HttpClient client = new HttpClient())
                    {
                        string baseUrl = BaseUrl;

                        if (flightSearch.FlightWay == FlightWay.OneWay)
                            baseUrl = "https://rushmyflight.wikiproject.in/";


                        string url = baseUrl + query;

                        HttpResponseMessage response = client.GetAsync(url).Result;
                        if (!response.IsSuccessStatusCode)
                        {
                            string err = response.Content.ReadAsStringAsync().Result;
                            flightsResult.Error = $"API Error: {response.StatusCode} - {err}";
                            return flightsResult;
                        }
                        var startTime = DateTime.UtcNow;

                        string json = response.Content.ReadAsStringAsync().Result;
                        var apiResult = JsonConvert.DeserializeObject<ApiResponse>(json);

                        int flightCount = apiResult?.data?.Count ?? 0;
                        var durationMs = (DateTime.UtcNow - startTime).TotalMilliseconds;

                        _logger.Info(
                            "Flight API Timing | SearchId={0} | StatusCode={1} | Count={2} | DurationMs={3}",
                            flightSearch.SearchId,
                            response,
                            flightCount,
                            durationMs
                        );

                        var ApiflightsResult = JsonConvert.DeserializeObject<ApiResponse>(json);
                        int flightiddefaultcount = 1;
                        foreach (var item in ApiflightsResult.data)
                        {
                            double baseFare = SafeDouble(item.price?.@base);
                            double fees = SafeDouble(item.price?.fees);
                            double grandTotal = SafeDouble(item.price?.grandTotal);

                            Flights flight = new Flights
                            {
                                ApiId = item.id.ToString(),
                                FlightId = Guid.NewGuid().ToString(),
                                SearchId = flightiddefaultcount.ToString(),
                                SessionId = flightiddefaultcount.ToString(),
                                Currency = item.price?.currency ?? "USD",
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

                            // --- Outbound mapping ---
                            if (item.itineraries != null && item.itineraries.Count > 0)
                            {
                                var outbound = item.itineraries[0];
                                if (outbound.segments != null)
                                {
                                    foreach (var seg in outbound.segments)
                                    {
                                        flight.Outbound.Add(new FlightsSegment
                                        {
                                            FromAirport = new Airport { Code = seg.departure?.iataCode ?? "", CityName = seg.departure.iataCode },
                                            ToAirport = new Airport { Code = seg.arrival?.iataCode ?? "", CityName = seg.arrival.iataCode },
                                            DepartureDate = SafeDate(seg.departure?.at),
                                            ArrivalDate = SafeDate(seg.arrival?.at),
                                            Airline = new Airline { Code = seg.carrierCode ?? "", Name = seg.airline ?? "" },
                                            FlightNo = seg.number ?? "",
                                            IsReturn = false,
                                            OperatingAirline = new Airline { Code = seg.operating.carrierCode },
                                            ElapsedTime = seg.duration ?? "",
                                            Distance = "",
                                            //CabinClass = item.classofService ?? "",
                                            ClassOfService = item.classofService,
                                            CabinClass = item.travelerPricings.Select(x => x.fareDetailsBySegment.Select(x => x.@class).FirstOrDefault()).FirstOrDefault() ?? "",
                                            Availability = item.numberOfBookableSeats.ToString(),
                                            BookingCode = item.jsonId ?? "",
                                            DepDate = seg.departure?.at ?? "",
                                            ArrDate = seg.arrival?.at ?? "",
                                            TotalTime = item.itineraries[0].duration,
                                            FromTerminal = seg.departure?.terminal ?? "",
                                            ToTerminal = seg.arrival?.terminal ?? ""
                                        });

                                        totalStops += seg.numberOfStops; // may be 0
                                        totalMinutes += ConvertDurationToMinutes(seg.duration);
                                    }
                                }
                            }

                            // --- Inbound mapping (for round trips) ---
                            if (item.itineraries != null && item.itineraries.Count > 1)
                            {
                                var inbound = item.itineraries[1];
                                if (inbound.segments != null)
                                {
                                    foreach (var seg in inbound.segments)
                                    {
                                        flight.Inbound.Add(new FlightsSegment
                                        {
                                            FromAirport = new Airport { Code = seg.departure?.iataCode ?? "", CityName = seg.departure.iataCode },
                                            ToAirport = new Airport { Code = seg.arrival?.iataCode ?? "", CityName = seg.arrival.iataCode },
                                            DepartureDate = SafeDate(seg.departure?.at),
                                            ArrivalDate = SafeDate(seg.arrival?.at),
                                            Airline = new Airline { Code = seg.carrierCode ?? "", Name = seg.airline ?? "" },
                                            FlightNo = seg.number ?? "",
                                            IsReturn = true,
                                            OperatingAirline = new Airline { Code = seg.operating.carrierCode },
                                            ElapsedTime = seg.duration ?? "",                             // e.g. "PT5H0M"
                                            Distance = "",                                                // API doesn't provide distance — keep empty or calculate later

                                            //CabinClass = item.classofService ?? "",                     // e.g. "Y"
                                            ClassOfService = item.classofService,


                                            CabinClass = item.travelerPricings.Select(x => x.fareDetailsBySegment.Select(x => x.@class).FirstOrDefault()).FirstOrDefault() ?? "",
                                            Availability = item.numberOfBookableSeats.ToString(),       // e.g. "4"
                                            BookingCode = item.jsonId ?? "",                         // If available in API; else leave empty
                                            DepDate = seg.departure?.at ?? "",
                                            ArrDate = seg.arrival?.at ?? "",
                                            TotalTime = item.itineraries[1].duration,                         // e.g. "PT11H10M"
                                            FromTerminal = seg.departure?.terminal ?? "",
                                            ToTerminal = seg.arrival?.terminal ?? ""
                                        });

                                        totalStops += seg.numberOfStops; // handle even if missing
                                        totalMinutes += ConvertDurationToMinutes(seg.duration);
                                    }
                                }
                            }

                            // --- Flight summary ---
                            flight.Stops = totalStops;
                            flight.TotalTime = totalMinutes; // minutes of flight time

                            //Add By Janesh _21dec25
                            double adultBase = 0, adultTax = 0;
                            double childBase = 0, childTax = 0;
                            double infantBase = 0, infantTax = 0;



                            if (item.travelerPricings != null)
                            {
                                var adult = item.travelerPricings
                                    .FirstOrDefault(x => x.travelerType == "ADULT");

                                if (adult != null)
                                {
                                    adultBase = SafeDouble(adult.price?.@base);
                                    adultTax = SafeDouble(adult.price?.fees);
                                }

                                var child = item.travelerPricings
                                    .FirstOrDefault(x => x.travelerType == "CHILD");

                                if (child != null)
                                {
                                    childBase = SafeDouble(child.price?.@base);
                                    childTax = SafeDouble(child.price?.fees);
                                }

                                var infant = item.travelerPricings
                                    .FirstOrDefault(x => x.travelerType == "INFANT");

                                if (infant != null)
                                {
                                    infantBase = SafeDouble(infant.price?.@base);
                                    infantTax = SafeDouble(infant.price?.fees);
                                }
                            }
                            double totalCost = baseFare + fees;
                            // --- Fare ---

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

                            // Set AvgCost safely
                            typeof(FlightsFare).GetProperty("AvgCost")?
                                .SetValue(flight.FlightFare, totalCost);


                            // --- Airline ---
                            if (flight.Outbound.Any())
                            {
                                flight.Airline.Code = flight.Outbound[0].Airline.Code;
                            }

                            // --- Stops ---
                            flight.Stops = flight.Outbound.Count - 1;

                            // --- FlightsResult top-level ---
                            if (flight.Outbound.Any())
                            {
                                flightsResult.OutBoundDep = flight.Outbound.First().FromAirport.Code;
                                flightsResult.OutBoundArr = flight.Outbound.Last().ToAirport.Code;
                            }
                            if (flight.Inbound.Any())
                            {
                                flightsResult.InBoundDep = flight.Inbound.First().FromAirport.Code;
                                flightsResult.InBoundArr = flight.Inbound.Last().ToAirport.Code;
                            }
                            FlightsList.Add(flight);
                        }

                        flightsResult.FlightsList = FlightsList.ToList();


                        var Jsom = JsonConvert.SerializeObject(flightSearch);
                        string rushMyFlight = "Rush";

                        if (flightsResult.FlightsList.Any())
                        {
                            flightsResult.MinPrice = (int)flightsResult.FlightsList.Min(f => f.TotalCost);
                            flightsResult.MaxPrice = (int)flightsResult.FlightsList.Max(f => f.TotalCost);
                        }
                        flightsResult.FlightsSearch = flightSearch;
                        //Add Apiresponse Actual price 
                        string folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "RawFlightLogs");
                        if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
                        string filePath = Path.Combine(folder, $"RawFlights_{flightsResult.FlightsSearch.SearchId}.json");
                        File.WriteAllText(filePath, JsonConvert.SerializeObject(flightsResult.FlightsList, Newtonsoft.Json.Formatting.Indented));

                        MarkupService.ApplyMarkupNew(flightsResult, flightSearch, rushMyFlight, rushMyFlight);
                        //Common.SaveSession("search_" + flightsResult.FlightsSearch.SearchId, JsonConvert.SerializeObject(flightsResult));
                        Common.SaveObjectSession("search_" + flightsResult.FlightsSearch.SearchId, flightsResult);

                        AplicationHelper.Save(flightsResult.FlightsSearch.SearchId, "search_", flightsResult);

                    }
                }
            }
            catch (WebException ex)
            {
                string message = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
                flightsResult.Error = message;// e.Message.ToString();
                flightsResult.FlightsSearch = flightSearch;
                throw ex;

            }
            return flightsResult;
        }

        private static double SafeDouble(string value)
        {
            if (double.TryParse(value, out double result))
                return result;
            return 0;
        }

        private static DateTime SafeDate(string dateString)
        {
            if (DateTime.TryParse(dateString, out DateTime date))
                return date;
            return DateTime.MinValue;
        }

        private static int ConvertDurationToMinutes(string isoDuration)
        {
            if (string.IsNullOrEmpty(isoDuration)) return 0;

            int hours = 0, minutes = 0;
            var hMatch = System.Text.RegularExpressions.Regex.Match(isoDuration, @"(\d+)H");
            var mMatch = System.Text.RegularExpressions.Regex.Match(isoDuration, @"(\d+)M");

            if (hMatch.Success) hours = int.Parse(hMatch.Groups[1].Value);
            if (mMatch.Success) minutes = int.Parse(mMatch.Groups[1].Value);

            return (hours * 60) + minutes;
        }
        private static void ParseFlights(XmlDocument finalResult, FlightsResult flightsResult)
        {
            XDocument doc = XDocument.Parse(finalResult.InnerXml);
            XNamespace air = "http://www.travelport.com/schema/air_v50_0";
            int flightCounter = 1;

            foreach (var solution in doc.Descendants(air + "AirPricingSolution"))
            {
                Flights flight = new Flights
                {
                    Outbound = new List<FlightsSegment>(),
                    Inbound = new List<FlightsSegment>(),
                    OutboundList = new List<List<FlightsSegment>>(),
                    InboundList = new List<List<FlightsSegment>>(),
                    FlightFare = new FlightsFare(),
                    FlightId = "FL-" + (flightCounter++).ToString(),
                    Currency = solution.Attribute("TotalPrice").Value.Substring(0, 3),
                    Airline = new Airline(),
                };

                // --- Total cost ---
                flight.TotalCost = double.Parse(solution.Attribute("TotalPrice").Value.Replace(flight.Currency, ""));

                // --- Journeys are inside solution, not parent ---
                var journeys = solution.Elements(air + "Journey").ToList();
                for (int j = 0; j < journeys.Count; j++)
                {
                    var journey = journeys[j];
                    List<FlightsSegment> segList = new List<FlightsSegment>();

                    foreach (var segRef in journey.Descendants(air + "AirSegmentRef"))
                    {
                        string segKey = segRef.Attribute("Key").Value;
                        var seg = doc.Descendants(air + "AirSegment")
                                     .FirstOrDefault(s => s.Attribute("Key").Value == segKey);

                        if (seg != null)
                        {
                            FlightsSegment segment = new FlightsSegment
                            {
                                OptionRef = seg.Attribute("Key").Value,
                                SagementRef = seg.Attribute("Key").Value,
                                FlightNo = seg.Attribute("FlightNumber")?.Value,
                                DepartureDate = DateTime.Parse(seg.Attribute("DepartureTime").Value),
                                ArrivalDate = DateTime.Parse(seg.Attribute("ArrivalTime").Value),
                                FromAirport = new Airport
                                {
                                    Code = seg.Attribute("Origin").Value,
                                    CityName = seg.Attribute("Origin").Value // TODO: replace with actual mapping later
                                },
                                ToAirport = new Airport
                                {
                                    Code = seg.Attribute("Destination").Value,
                                    CityName = seg.Attribute("Destination").Value // TODO: replace with actual mapping later
                                },
                                Airline = new Airline
                                {
                                    Code = seg.Attribute("Carrier").Value,
                                    Name = seg.Attribute("Carrier").Value // TODO: map carrier code to full airline name
                                },
                                OperatingAirline = new Airline { Code = seg.Attribute("Carrier").Value },
                                FromTerminal = seg.Attribute("OriginTerminal")?.Value,
                                ToTerminal = seg.Attribute("DestinationTerminal")?.Value,
                                EquipmentType = seg.Attribute("Equipment")?.Value,
                                Distance = seg.Attribute("Distance")?.Value,
                                ETicket = seg.Attribute("ETicketability")?.Value,
                                CabinClass = solution.Descendants(air + "BookingInfo")
                                                     .FirstOrDefault(b => b.Attribute("SegmentRef").Value == segKey)?
                                                     .Attribute("CabinClass")?.Value,
                                Availability = solution.Descendants(air + "BookingInfo")
                                                       .FirstOrDefault(b => b.Attribute("SegmentRef").Value == segKey)?
                                                       .Attribute("BookingCode")?.Value,
                                BookingCode = solution.Descendants(air + "BookingInfo")
                                                      .FirstOrDefault(b => b.Attribute("SegmentRef").Value == segKey)?
                                                      .Attribute("BookingCode")?.Value,
                                IsReturn = (j > 0),
                                DepDate = seg.Attribute("DepartureTime").Value,
                                ArrDate = seg.Attribute("ArrivalTime").Value,
                                TotalTime = seg.Attribute("FlightTime")?.Value ?? "0",   // << set default 0
                                ElapsedTime = seg.Attribute("TravelTime")?.Value ?? "0"
                            };

                            segList.Add(segment);
                        }
                    }

                    // Assign to outbound / inbound
                    if (j == 0)
                    {
                        flight.Outbound.AddRange(segList);
                        flight.OutboundList.Add(segList);
                    }
                    else
                    {
                        flight.Inbound.AddRange(segList);
                        flight.InboundList.Add(segList);
                    }

                    // Journey total time
                    string travelTime = journey.Attribute("TravelTime")?.Value;
                    if (!string.IsNullOrEmpty(travelTime))
                    {
                        TimeSpan ts = XmlConvert.ToTimeSpan(travelTime);
                        flight.TotalTime += (int)ts.TotalMinutes;
                    }
                }

                // --- Fare info ---
                var pricingInfo = solution.Element(air + "AirPricingInfo");
                if (pricingInfo != null)
                {
                    flight.FlightFare.Adult = doc.Descendants(air + "PassengerType")
                                                 .Count(p => p.Attribute("Code").Value == "ADT");
                    flight.FlightFare.Child = doc.Descendants(air + "PassengerType")
                                                 .Count(p => p.Attribute("Code").Value == "CNN");
                    flight.FlightFare.Infant = doc.Descendants(air + "PassengerType")
                                                  .Count(p => p.Attribute("Code").Value == "INF");

                    flight.FlightFare.AdultFare = double.Parse(pricingInfo.Attribute("BasePrice").Value.Replace(flight.Currency, ""));
                    flight.FlightFare.AdultTax = double.Parse(pricingInfo.Attribute("Taxes").Value.Replace(flight.Currency, ""));
                }

                // --- Airline ---
                if (flight.Outbound.Any())
                {
                    flight.Airline.Code = flight.Outbound[0].Airline.Code;
                }

                // --- Stops ---
                flight.Stops = flight.Outbound.Count - 1;

                // --- FlightsResult top-level ---
                if (flight.Outbound.Any())
                {
                    flightsResult.OutBoundDep = flight.Outbound.First().FromAirport.Code;
                    flightsResult.OutBoundArr = flight.Outbound.Last().ToAirport.Code;
                }
                if (flight.Inbound.Any())
                {
                    flightsResult.InBoundDep = flight.Inbound.First().FromAirport.Code;
                    flightsResult.InBoundArr = flight.Inbound.Last().ToAirport.Code;
                }

                // --- Add flight ---
                flightsResult.FlightsList.Add(flight);
            }

            // --- Min / Max prices ---
            if (flightsResult.FlightsList.Any())
            {
                flightsResult.MinPrice = (int)flightsResult.FlightsList.Min(f => f.TotalCost);
                flightsResult.MaxPrice = (int)flightsResult.FlightsList.Max(f => f.TotalCost);
            }
        }


        /// <summary>
        /// Build LowFareSearch XML from object
        /// </summary>
        private static string BuildLowFareSearchXml(FlightsSearch flightSearch)
        {
            string fromCode = flightSearch.From.Code;
            string toCode = flightSearch.To.Code;
            string depDate = flightSearch.DepDate.ToString("yyyy-MM-dd");
            string retDate = flightSearch.RetDate.ToString("yyyy-MM-dd");

            int adult = flightSearch.Adult;
            int child = flightSearch.Child;
            int infant = flightSearch.Infant;
            int flightClass = (int)flightSearch.FlightClass;
            bool isDirect = flightSearch.IsDirect;
            isDirect = true;
            string cabinType = flightClass switch
            {
                1 => "First",
                2 => "Business",
                _ => "Economy"
            };

            StringBuilder sb = new StringBuilder();

            // Root element
            sb.AppendLine("<LowFareSearchReq xmlns=\"http://www.travelport.com/schema/air_v50_0\" TargetBranch=\"P1885281\" SolutionResult=\"true\">");
            sb.AppendLine("  <BillingPointOfSaleInfo xmlns=\"http://www.travelport.com/schema/common_v50_0\" OriginApplication=\"UAPI\"/>");

            // Outbound leg
            sb.AppendLine("  <SearchAirLeg>");
            sb.AppendLine($"    <SearchOrigin><City xmlns=\"http://www.travelport.com/schema/common_v50_0\" Code=\"{fromCode}\"/></SearchOrigin>");
            sb.AppendLine($"    <SearchDestination><City xmlns=\"http://www.travelport.com/schema/common_v50_0\" Code=\"{toCode}\"/></SearchDestination>");
            sb.AppendLine($"    <SearchDepTime PreferredTime=\"{depDate}\"/>");
            sb.AppendLine($"    <AirLegModifiers AllowDirectAccess=\"{isDirect.ToString().ToLower()}\">");
            sb.AppendLine("      <PreferredCabins>");
            sb.AppendLine($"        <CabinClass xmlns=\"http://www.travelport.com/schema/common_v50_0\" Type=\"{cabinType}\"/>");
            sb.AppendLine("      </PreferredCabins>");
            sb.AppendLine("    </AirLegModifiers>");
            sb.AppendLine("  </SearchAirLeg>");

            // Return leg only for round-trip
            if (flightSearch.FlightWay == FlightWay.RoundTrip)
            {
                sb.AppendLine("  <SearchAirLeg>");
                sb.AppendLine($"    <SearchOrigin><City xmlns=\"http://www.travelport.com/schema/common_v50_0\" Code=\"{toCode}\"/></SearchOrigin>");
                sb.AppendLine($"    <SearchDestination><City xmlns=\"http://www.travelport.com/schema/common_v50_0\" Code=\"{fromCode}\"/></SearchDestination>");
                sb.AppendLine($"    <SearchDepTime PreferredTime=\"{retDate}\"/>");
                sb.AppendLine($"    <AirLegModifiers AllowDirectAccess=\"{isDirect.ToString().ToLower()}\">");
                sb.AppendLine("      <PreferredCabins>");
                sb.AppendLine($"        <CabinClass xmlns=\"http://www.travelport.com/schema/common_v50_0\" Type=\"{cabinType}\"/>");
                sb.AppendLine("      </PreferredCabins>");
                sb.AppendLine("    </AirLegModifiers>");
                sb.AppendLine("  </SearchAirLeg>");
            }



            // AirSearchModifiers after passengers
            sb.AppendLine("  <AirSearchModifiers MaxSolutions=\"50\">");
            sb.AppendLine("    <PreferredProviders>");
            sb.AppendLine("      <Provider xmlns=\"http://www.travelport.com/schema/common_v50_0\" Code=\"1P\"/>");
            sb.AppendLine("    </PreferredProviders>");
            sb.AppendLine("  </AirSearchModifiers>");

            // Passengers — ADT first, then children, infants
            for (int i = 0; i < adult; i++)
                sb.AppendLine("  <SearchPassenger xmlns=\"http://www.travelport.com/schema/common_v50_0\" Code=\"ADT\"/>");
            for (int i = 0; i < child; i++)
                sb.AppendLine("  <SearchPassenger xmlns=\"http://www.travelport.com/schema/common_v50_0\" Code=\"CNN\"/>");
            for (int i = 0; i < infant; i++)
                sb.AppendLine("  <SearchPassenger xmlns=\"http://www.travelport.com/schema/common_v50_0\" Code=\"INF\"/>");

            // AirPricingModifiers
            sb.AppendLine("  <AirPricingModifiers FaresIndicator=\"PublicFaresOnly\" CurrencyType=\"GBP\"/>");

            sb.AppendLine("</LowFareSearchReq>");

            return sb.ToString();
        }

        private static HttpWebRequest CreateRequestObject()
        {
            Uri uri = new Uri("https://emea.universal-api.travelport.com/B2BGateway/connect/uAPI/AirService");
            string UserName = "Universal API/uAPI1252564161-e8c87225";
            string Password = "t?6E8zP*eW";
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);
            httpWebRequest.Method = "POST";
            httpWebRequest.ContentType = "text/xml";
            ServicePointManager.UseNagleAlgorithm = false;
            ServicePointManager.Expect100Continue = false;
            httpWebRequest.AutomaticDecompression = DecompressionMethods.GZip;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            byte[] bytes = Encoding.UTF8.GetBytes((UserName + ":" + Password).ToCharArray());
            httpWebRequest.Headers["Authorization"] = "Basic " + Convert.ToBase64String(bytes);
            return httpWebRequest;
        }
        private static byte[] GetSoapEnvelope(XmlDocument request)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("<SOAP-ENV:Envelope xmlns:SOAP-ENV=\"http://schemas.xmlsoap.org/soap/envelope/\">");
            stringBuilder.Append("<SOAP-ENV:Body>");
            stringBuilder.Append(request.DocumentElement.OuterXml);
            stringBuilder.Append("</SOAP-ENV:Body>");
            stringBuilder.Append("</SOAP-ENV:Envelope>");
            UTF8Encoding uTF8Encoding = new UTF8Encoding();
            return uTF8Encoding.GetBytes(stringBuilder.ToString());
        }


        private static XmlDocument GetResponseDocument(string result)
        {
            string LastError = "";
            XmlDocument xmlDocument = new XmlDocument();
            XmlDocument xmlDocument2 = null;
            try
            {
                xmlDocument.LoadXml(result);
                XmlNode xmlNode = xmlDocument.SelectSingleNode("//*[local-name()='Body']/*");
                xmlDocument2 = new XmlDocument();
                xmlDocument2.LoadXml(xmlNode.OuterXml);
            }
            catch (Exception ex)
            {
                if (string.IsNullOrEmpty(LastError))
                {
                    if (ex.Message.StartsWith("'xsd' is an undeclared prefix", StringComparison.OrdinalIgnoreCase))
                    {
                        LastError = "The server could not find a valid schema document for the schema referenced in your request. Please check the version number of each schema referenced.";
                    }
                    else
                    {
                        LastError = ex.Message;
                    }
                }
            }

            return xmlDocument2;
        }

        private static void SetErrorMessage(WebException exception)
        {
            string LastError;
            if (exception.Response != null && ((HttpWebResponse)exception.Response).StatusCode == HttpStatusCode.Unauthorized)
            {
                LastError = "The server returned Unauthorized. Please ensure that you are using the correct user name and password.";
            }
            else if (exception.Response != null && ((HttpWebResponse)exception.Response).StatusCode == HttpStatusCode.NotFound)
            {
                LastError = "The service could not be found on the server. Please check that you are using the correct URL.";
                LastError = LastError + Environment.NewLine + Environment.NewLine;
                LastError += "The URL will vary depending on the service you want to access.";
            }
            else
            {
                LastError = exception.Message;
            }
        }

        private static FlightSearchRequest MapToRequest(FlightsSearch flightSearch)
        {
            return new FlightSearchRequest
            {
                Origin = flightSearch.From?.Code,
                Destination = flightSearch.To?.Code,
                DepartureDate = flightSearch.DepDate,
                ReturnDate = flightSearch.FlightWay == FlightWay.RoundTrip
             ? flightSearch.RetDate
             : (DateTime?)null,

                Adult = flightSearch.Adult,
                Child = flightSearch.Child,
                Infant = flightSearch.Infant,

                CabinClass = MapCabinClass(flightSearch.FlightClass),

                DirectFlight = flightSearch.IsDirect,
                PreferredAirline = flightSearch.Airline?.Code,
                FlexibleDate = flightSearch.IsFlexi,
                Youth = 0
            };
        }
        private static CabinClass MapCabinClass(FlightClass flightClass)
        {
            return flightClass switch
            {
                FlightClass.First => CabinClass.First,
                FlightClass.Business => CabinClass.Business,
                FlightClass.EconomyPremium => CabinClass.PremiumEconomy,
                _ => CabinClass.Economy
            };
        }
    }
}