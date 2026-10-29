using AngleSharp.Io;
using FlightServices.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NLog;
using ServiceStack.Redis;
using SqlData.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using TravelWeb.Datamodel;
using TravelWeb.Helper;
using TravelWeb.Models;
using TravelWeb.Services;
using TravelWeb.Utilities;
using static TravelWeb.Models.MetaResponseDto;
namespace TravelWeb.Controllers
{

    public class ResultController : Controller
    {
        public readonly ILogger<ResultController> _logger;
        private readonly DeepLinkTrackerService _deepLinkTrackerService;

        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IConfiguration _configuration;
        private readonly FlightSearchService _flightSearchService;


        public ResultController(ILogger<ResultController> logger, IServiceScopeFactory scopeFactory, IConfiguration configuration, FlightSearchService flightSearchService, DeepLinkTrackerService deepLinkTrackerService)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _configuration = configuration;
            _flightSearchService = flightSearchService;
            _deepLinkTrackerService = deepLinkTrackerService;

            if (Common.GetAirportDataX == null)
                Common.GetAirportDataX = Common.GetAirportData();
            if (Common.GetAirlineDataX == null)
                Common.GetAirlineDataX = Common.GetAirlineData();
        }

        [Route("result")]
        [Route("{folder}/result")]
        public async Task<IActionResult> result()
        {
            try
            {


                FlightSearchModel model = null;

                string utm_source = "";
                string utm_medium = "";
                string utm_campaign = "";
                string searchID = "";
                string seniors = "0";
                string currency = "USD";

                // ===============================
                // 1️⃣ TempData (POST Flow)
                // ===============================
                if (TempData["FlightSearchModel"] != null)
                {
                    model = JsonConvert.DeserializeObject<FlightSearchModel>(
                        TempData["FlightSearchModel"].ToString());

                    utm_source = HttpContext.Session.GetString("utm_source") ?? "HOME";
                    utm_medium = HttpContext.Session.GetString("utm_medium") ?? "";
                    utm_campaign = HttpContext.Session.GetString("utm_campaign") ?? "";
                }

                // ===============================
                // 2️⃣ QueryString Fallback
                // ===============================
                if (model == null && Request.Query.Count > 0)
                {
                    int adult = 0;
                    int child = 0;
                    int infant = 0;

                    int.TryParse(Request.Query["adult"].ToString(), out adult);
                    int.TryParse(Request.Query["child"].ToString(), out child);
                    int.TryParse(Request.Query["infant"].ToString(), out infant);

                    model = new FlightSearchModel
                    {
                        Depart = Request.Query["depart"],
                        Arrival = Request.Query["arrival"],
                        Trip = Request.Query["trip"],
                        DepartDate = Request.Query["departOn[]"],
                        ReturnDate = Request.Query["returnOn"],
                        Adult = adult,
                        Child = child,
                        Infant = infant,
                        Cabin = Request.Query["cabin"]
                    };

                    seniors = Request.Query["seniors"];
                    utm_source = Request.Query["utm_source"];
                    utm_medium = Request.Query["utm_medium"];
                    utm_campaign = Request.Query["utm_campaign"];
                    searchID = Request.Query["searchID"];
                    currency = Request.Query["currency"];
                    if (string.IsNullOrWhiteSpace(currency))
                    {
                        currency = "USD";
                    }
                    Response.Cookies.Append(
    "site_currency",
    currency,
    new CookieOptions
    {
        Expires = DateTimeOffset.UtcNow.AddYears(1),
        HttpOnly = false,
        IsEssential = true
    });
                }

                if (model == null)
                    return RedirectToAction("Index", "Home");

                // ===============================
                // SESSION COMMON DATA
                // ===============================
                if (Common.GetAirportDataX == null)
                    Common.GetAirportDataX = Common.GetAirportData();
                if (Common.GetAirlineDataX == null)
                    Common.GetAirlineDataX = Common.GetAirlineData();
                if (HttpContext.Session.GetObjectFromJson<string>("CommonData") == null)
                {
                    HttpContext.Session.SetObjectAsJson("CommonData", "TRUE");
                }

                FlightsSearch flightSearch = new FlightsSearch();

                // ===============================
                // Folder Name Handle
                // ===============================
                string folderName = "";
                string URLStr = Request.Path;
                if (URLStr.Split('/').Length == 3)
                {
                    folderName = URLStr.Split('/')[1];
                }
                Common.FolderName = folderName;

                // ===============================
                // Trip Conversion
                // ===============================
                string trips = (model.Trip == "round" || model.Trip == "RoundTrip") ? "2" : "1";

                // ===============================
                // Cabin Conversion
                // ===============================
                string cabin = model.Cabin;
                if (cabin == "Y" || cabin == "y" || cabin == "Economy") cabin = "4";
                else if (cabin == "B") cabin = "2";
                else if (cabin == "P") cabin = "3";
                else if (cabin == "F") cabin = "1";

                // ===============================
                // UTM Handling
                // ===============================
                //string utm_sources =
                //                   (utm_source.Equals("kayak", StringComparison.OrdinalIgnoreCase) ||
                //                    utm_source.Equals("Trabber", StringComparison.OrdinalIgnoreCase)|| utm_source.Equals("FareCompare", StringComparison.OrdinalIgnoreCase))
                //                   ? utm_source
                //                   : "HOME";
                string utm_sources =
    utm_source.Equals("Media", StringComparison.OrdinalIgnoreCase)
        ? "FareCompare"
        : (utm_source.Equals("kayak", StringComparison.OrdinalIgnoreCase) ||
           utm_source.Equals("Trabber", StringComparison.OrdinalIgnoreCase) ||
           utm_source.Equals("FareCompare", StringComparison.OrdinalIgnoreCase))
            ? utm_source
            : "HOME";
                // ===============================
                // Search ID
                // ===============================
                if (utm_campaign == "deeplink" && !string.IsNullOrEmpty(searchID))
                    flightSearch.SearchId = searchID;
                else
                    flightSearch.SearchId = Guid.NewGuid().ToString();

                var subId = Request.Query["subId"].ToString();

                if (utm_sources.Equals("Trabber", StringComparison.OrdinalIgnoreCase) &&
                    !string.IsNullOrEmpty(subId))
                {
                    flightSearch.SearchId = subId;
                }
                // ===============================
                // FlightSearch Setup
                // ===============================
                flightSearch.FlightWay = (FlightWay)Convert.ToByte(trips);
                flightSearch.From = Common.GetAirport(model.Depart);
                flightSearch.To = Common.GetAirport(model.Arrival);
                DateTime departDate;
                DateTime returnDate;
                if (!string.IsNullOrEmpty(model.DepartDate))
                {
                    departDate = Common.ConvertDateTime(model.DepartDate);
                }
                else
                {
                    departDate = DateTime.Today.AddDays(7);  // Default: 7 days from today
                }

                // 2️⃣ Return Date
                if (!string.IsNullOrEmpty(model.ReturnDate))
                {
                    returnDate = Common.ConvertDateTime(model.ReturnDate);
                }
                else
                {
                    returnDate = departDate.AddDays(10);  // Default: 10 days after depart
                }

                flightSearch.Adult = model.Adult;
                flightSearch.Child = model.Child;
                flightSearch.Infant = model.Infant;
                flightSearch.FlightClass = (FlightClass)Convert.ToByte(cabin);
                flightSearch.IsDirect = false;
                flightSearch.IsFlexi = true;
                flightSearch.SiteCode = utm_sources;
                flightSearch.SourceMedia = utm_sources;
                flightSearch.IsDeepLink = true;
                // Flight fare API must use USD
                flightSearch.Currency = "USD";
                currency = "USD";
                Common.SourceMedia = utm_sources;

                // ===============================
                // ViewBag Setup (ALL VALUES)
                // ===============================
                ViewBag.depart = model.Depart;
                ViewBag.arrival = model.Arrival;
                ViewBag.trip = trips;
                ViewBag.departOn = departDate;
                ViewBag.returnOn = returnDate;
                ViewBag.adult = model.Adult;
                ViewBag.child = model.Child;
                ViewBag.infant = model.Infant;
                ViewBag.MorderAdult = model.Adult;
                ViewBag.MoreChild = model.Child;
                ViewBag.seniors = seniors;
                ViewBag.cabin = cabin;
                ViewBag.IsDirect = false;
                ViewBag.utm_source = utm_sources;
                ViewBag.utm_medium = utm_medium;
                ViewBag.RoundTrip = flightSearch.FlightWay.GetHashCode();

                ViewBag.DepCode = flightSearch.From.Code.ToUpper();
                ViewBag.DestCode = flightSearch.To.Code.ToUpper();
                ViewBag.flightClass = flightSearch.FlightClass;
                ViewBag.departure = flightSearch.From.Code + "-" + flightSearch.From.CityName + "," + flightSearch.From.CountryName;
                ViewBag.destination = flightSearch.To.Code + "-" + flightSearch.To.CityName + "," + flightSearch.To.CountryName;
                ViewBag.DepartDate = departDate.ToString("yyyy-MM-dd");
                ViewBag.ReturnDate = returnDate.ToString("yyyy-MM-dd");
                ViewBag.FlyClass = cabin;
                ViewBag.FlightClass = flightSearch.FlightClass;
                ViewBag.ToCity = flightSearch.To.CityName;
                ViewBag.Currency = currency;
                if (utm_source == "Trabber")
                {
                    flightSearch.DepDate = departDate;
                    flightSearch.RetDate = returnDate;
                    var engine = await _flightSearchService.SearchFlightOptimizedAsyncV2(flightSearch, 0);
                    engine.Source = "Trabber";

                    var metaResp = _flightSearchService.MapToMetaResponse(engine);

                    return Ok(new
                    {
                        searchId = flightSearch.SearchId,
                        subId = flightSearch.SearchId,
                        result = metaResp
                    });
                }
                if (utm_sources.Equals("FareCompare", StringComparison.OrdinalIgnoreCase))
                {
                    var clientIp = GetClientIp(HttpContext);
                    var userAgent = Request.Headers["User-Agent"].ToString();

                    flightSearch.IPAddress = clientIp;

                    //await _deepLinkTrackerService.SaveFareCompareTrafficAsync(
                    //    flightSearch,
                    //    departDate,
                    //    returnDate,
                    //    utm_sources,
                    //    utm_campaign,
                    //    subId
                    //);

    //                await _deepLinkTrackerService.SaveFareCompareTrafficAsync(
    //flightSearch, departDate, returnDate,
    //utm_source, utm_campaign, subId,
    //clientIp, userAgent);

                    //return View("MetaView");
                }
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in result()");
                return RedirectToAction("Index", "Home");
            }
        }


        //[Route("result")]
        //[Route("{folder}/result")]
        //public IActionResult result( /*string trip, string adult, string child, string infant, string returnOn, string seniors, string utm_campaign = "", string flightshall = "", string af = ""*/)
        //{
        //    try
        //    {
        //        FlightSearchModel model = null;

        //        if (TempData["FlightSearchModel"] != null)
        //        {
        //            model = JsonConvert.DeserializeObject<FlightSearchModel>(
        //                TempData["FlightSearchModel"].ToString());


        //            string depart = model.Depart;
        //            string arrival = model.Arrival;
        //            string departOn = model.DepartDate;
        //            string trip = model.Trip;
        //            string adult = model.Adult.ToString();
        //            string child = model.Child.ToString();
        //            string infant = model.Infant.ToString();
        //            string returnOn = model.ReturnDate;
        //            string cabin = model.Cabin;
        //        }
        //        if (model == null)
        //            return RedirectToAction("Index", "Home");

        //        string depart = HttpContext.Request.Query["depart"].ToString();
        //        string arrival = HttpContext.Request.Query["arrival"].ToString();
        //        string departOn = HttpContext.Request.Query["departOn[]"].ToString();
        //        string trip = HttpContext.Request.Query["trip"].ToString();

        //        string adult = HttpContext.Request.Query["adult"].ToString();

        //        string Moreadult = HttpContext.Request.Query["adult"].ToString();
        //        string MoreChild = HttpContext.Request.Query["child"].ToString();

        //        string child = HttpContext.Request.Query["child"].ToString();

        //        string infant = HttpContext.Request.Query["infant"].ToString();

        //        string returnOn = HttpContext.Request.Query["returnOn"].ToString();

        //        string seniors = HttpContext.Request.Query["seniors"].ToString();

        //        string utm_campaign = HttpContext.Request.Query["child"].ToString();

        //        string flightshall = HttpContext.Request.Query["infant"].ToString();

        //        string af = HttpContext.Request.Query["returnOn"].ToString();
        //        string cabin = HttpContext.Request.Query["cabin"].ToString();
        //        string utm_source = HttpContext.Request.Query["utm_source"].ToString();

        //        string utm_medium = HttpContext.Request.Query["utm_medium"].ToString();

        //        string searchID = HttpContext.Request.Query["searchID"].ToString();

        //        _logger.LogInformation(depart.ToString());
        //        if (HttpContext.Session.GetObjectFromJson<string>("CommonData") == null)
        //        {
        //            Common.GetAirportDataX = Common.GetAirportData();
        //            Common.GetAirlineDataX = Common.GetAirlineData();
        //            HttpContext.Session.SetObjectAsJson("CommonData", "TRUE");
        //        }
        //        bool IsLocal = Common.IsLocalIpAddress("localhost");
        //        FlightsSearch flightSearch = new FlightsSearch();
        //        string folderName = "";
        //        string URLStr = Request.Path;
        //        if (URLStr.Split('/').Length == 3)
        //        {
        //            folderName = URLStr.Split('/')[1];

        //        }
        //        Common.FolderName = folderName;

        //        var trips = "";
        //        if (trip == "round" || trip == "roundtrip")
        //        {
        //            trips = "2";
        //        }
        //        else
        //        {
        //            trips = "1";
        //        }

        //        if (cabin == "Y" || cabin == "y"|| cabin == "Economy")
        //        {
        //            cabin = "4";
        //        }
        //        else if (cabin == "B")
        //        {
        //            cabin = "2";
        //        }
        //        else if (cabin == "P")
        //        {
        //            cabin = "3";
        //        }
        //        else if (cabin == "F")
        //        {
        //            cabin = "1";
        //        }
        //        string utm_sources = "";

        //        if (utm_source == "kayak")
        //        {
        //            utm_sources = utm_source;
        //        }

        //        else
        //        {
        //            utm_sources = "HOME";
        //        }
        //        ViewBag.depart = depart;
        //        ViewBag.arrival = arrival;
        //        ViewBag.trip = trips;
        //        ViewBag.departOn = departOn;
        //        ViewBag.adult = adult;
        //        ViewBag.child = child;
        //        ViewBag.infant = infant;
        //        ViewBag.returnOn = returnOn;
        //        ViewBag.MorderAdult = Moreadult;
        //        ViewBag.MoreChild = MoreChild;
        //        // ViewBag.Home = "Home";
        //        ViewBag.IsDirect = false;
        //        ViewBag.seniors = seniors;
        //        ViewBag.cabin = cabin;
        //        if (utm_campaign == "deeplink")
        //        {
        //            flightSearch.SearchId = searchID;
        //        }
        //        else
        //        {
        //            flightSearch.SearchId = Guid.NewGuid().ToString();
        //        }
        //        flightSearch.FlightWay = (FlightWay)Convert.ToByte(trips);
        //        flightSearch.From = Common.GetAirport(depart.ToString().ToString());
        //        flightSearch.To = Common.GetAirport(arrival);
        //        //if (ViewBag.RoundTrip == null)
        //        //{
        //        //    ViewBag.RoundTrip = "1";
        //        //}
        //        ViewBag.RoundTrip = trips;
        //        flightSearch.DepDate = Common.ConvertDateTime(departOn);
        //        if (flightSearch.FlightWay == FlightWay.RoundTrip)
        //        {
        //            ViewBag.RoundTrip = "2";
        //            flightSearch.RetDate = Common.ConvertDateTime(returnOn);
        //        }
        //        flightSearch.Adult = Convert.ToInt32(adult);
        //        flightSearch.Child = Convert.ToInt32(child);
        //        flightSearch.Infant = Convert.ToInt32(infant);
        //        flightSearch.FlightClass = (FlightClass)Convert.ToByte(cabin);
        //        //flightSearch.Airline = Common.GetAirline(QSArr[11]);
        //        flightSearch.IsDirect = Convert.ToBoolean(false);
        //        flightSearch.IsFlexi = true;
        //        // flightSearch.Currency = QSStrArr[12];
        //        flightSearch.SiteCode = utm_sources;
        //        flightSearch.SourceMedia = utm_sources;
        //        flightSearch.IsDeepLink = true;
        //        Common.SourceMedia = utm_sources;
        //        ViewBag.utm_source = Common.SourceMedia;
        //        ViewBag.RoundTrip = flightSearch.FlightWay.GetHashCode();

        //        ViewBag.DepCode = flightSearch.From.Code.ToUpper();
        //        ViewBag.DestCode = flightSearch.To.Code.ToUpper();
        //        ViewBag.adult = flightSearch.Adult;
        //        ViewBag.child = flightSearch.Child;
        //        ViewBag.infant = flightSearch.Infant;
        //        ViewBag.flightClass = flightSearch.FlightClass;
        //        ViewBag.departure = flightSearch.From.Code + "-" + flightSearch.From.CityName + "," + flightSearch.From.CountryName;
        //        ViewBag.destination = flightSearch.To.Code + "-" + flightSearch.To.CityName + "," + flightSearch.To.CountryName;
        //        ViewBag.DepartDate = flightSearch.DepDate.ToString("yyyy-MM-dd");
        //        ViewBag.ReturnDate = flightSearch.RetDate.ToString("yyyy-MM-dd");
        //        ViewBag.FlyClass = "4";
        //        ViewBag.FlightClass = flightSearch.FlightClass;
        //        ViewBag.ToCity = flightSearch.To.CityName;

        //        return View();
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogInformation("something went wrong");
        //        throw ex;
        //    }
        //}

        public IActionResult result(string qs)
        {
            try
            {
                _logger.LogInformation(qs.ToString());
                if (Common.GetAirportDataX == null)
                    Common.GetAirportDataX = Common.GetAirportData();
                if (Common.GetAirlineDataX == null)
                    Common.GetAirlineDataX = Common.GetAirlineData();
                if (HttpContext.Session.GetObjectFromJson<string>("CommonData") == null)
                {
                    HttpContext.Session.SetObjectAsJson("CommonData", "TRUE");
                }
                bool IsLocal = Common.IsLocalIpAddress("localhost");
                FlightsSearch flightSearch = new FlightsSearch();
                string folderName = "";
                string URLStr = Request.Path;
                if (URLStr.Split('/').Length == 3)
                {
                    folderName = URLStr.Split('/')[1];

                }
                Common.FolderName = folderName;
                // qs = "PRAVEEN|Home|LHR|BKK|2020-09-11|2020-12-11|1|0|0|2|4|false";
                if (string.IsNullOrEmpty(qs))
                {
                    // Response.Redirect(_appsetting.WebSitePath);
                }

                string[] QParam = qs.Split('|');

                if (QParam.Length > 1)
                {
                    ViewBag.QS = qs;

                    string[] QSStrArr = QParam;

                    flightSearch.SearchId = Guid.NewGuid().ToString();
                    flightSearch.FlightWay = (FlightWay)Convert.ToByte(QSStrArr[9]);
                    flightSearch.From = Common.GetAirport(QSStrArr[2].ToString().ToString());
                    flightSearch.To = Common.GetAirport(QSStrArr[3]);
                    // ViewBag.RoundTrip = "1";
                    flightSearch.DepDate = Common.ConvertDateTime(QSStrArr[4]);
                    if (flightSearch.FlightWay == FlightWay.RoundTrip)
                    {
                        ViewBag.RoundTrip = "2";
                        flightSearch.RetDate = Common.ConvertDateTime(QSStrArr[5]);
                    }
                    flightSearch.Adult = Convert.ToInt32(QSStrArr[6]);
                    flightSearch.Child = Convert.ToInt32(QSStrArr[7]);
                    flightSearch.Infant = Convert.ToInt32(QSStrArr[8]);
                    flightSearch.FlightClass = (FlightClass)Convert.ToByte(QSStrArr[10]);
                    //flightSearch.Airline = Common.GetAirline(QSArr[11]);
                    flightSearch.IsDirect = Convert.ToBoolean(QSStrArr[11]);
                    flightSearch.IsFlexi = true;
                    // flightSearch.Currency = QSStrArr[12];
                    flightSearch.SiteCode = QSStrArr[1];
                    flightSearch.SourceMedia = QSStrArr[0];
                    flightSearch.IsDeepLink = true;
                    Common.SourceMedia = QSStrArr[1];

                    ViewBag.RoundTrip = flightSearch.FlightWay.GetHashCode();

                    ViewBag.DepCode = flightSearch.From.Code.ToUpper();
                    ViewBag.DestCode = flightSearch.To.Code.ToUpper();
                    ViewBag.adult = flightSearch.Adult;
                    ViewBag.child = flightSearch.Child;
                    ViewBag.infant = flightSearch.Infant;
                    ViewBag.flightClass = flightSearch.FlightClass;
                    ViewBag.departure = flightSearch.From.Code + "-" + flightSearch.From.CityName + "," + flightSearch.From.CountryName;
                    ViewBag.destination = flightSearch.To.Code + "-" + flightSearch.To.CityName + "," + flightSearch.To.CountryName;
                    ViewBag.DepartDate = flightSearch.DepDate.ToString("yyyy-MM-dd");
                    ViewBag.ReturnDate = flightSearch.RetDate.ToString("yyyy-MM-dd");
                    ViewBag.FlyClass = QSStrArr[10];
                    ViewBag.FlightClass = flightSearch.FlightClass;
                    ViewBag.ToCity = flightSearch.To.CityName;

                }
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogInformation("something went wrong");
                throw ex;
            }
        }

        //public IActionResult
        public async Task<IActionResult> search(string depart, string Flight1Depart, string Multiselect, string Flight1arrival, string arrival, string trip, string departOn, string adult, string child, string infant, string cabin, string returnOn, Boolean IsDirect, string utm_source, string seniors, string SearchId, string currency)
        {
            var clientIp = GetClientIp(HttpContext);
            var userAgent = Request.Headers["User-Agent"].ToString();
            var device = GetDeviceType(userAgent);

            if (Multiselect == null)
            {
                try
                {
                    FlightsResult flightsResult = new FlightsResult();

                    //List<MultiselectFlight> multifightlist = new List<MultiselectFlight>();
                    //multifightlist = JsonConvert.DeserializeObject<List<MultiselectFlight>>(Multiselect);
                    var remoteIpAddress = Request.HttpContext.Connection.RemoteIpAddress;

                    //remoteIpAddress = clientIp;
                    FlightsSearch flightSearch = new FlightsSearch();
                    //MultipleFlightsSearch flightSearches = new MultipleFlightsSearch();
                    //var List = flightSearches.multiplelFlightlist.ToList();

                    //foreach( var item in List)
                    //{
                    //    item.From = Common.GetAirport(depart.ToString().ToString());

                    //}


                    //flightSearch.Multiflights = multifightlist;

                    flightSearch.SearchId = Guid.NewGuid().ToString();

                    flightSearch.FlightWay = (FlightWay)Convert.ToByte(trip);
                    flightSearch.From = Common.GetAirport(depart.ToString().ToString());
                    flightSearch.To = Common.GetAirport(arrival);
                    flightSearch.DepDate = Common.ConvertDateTime(departOn);

                    if (flightSearch.FlightWay == FlightWay.RoundTrip)
                        flightSearch.RetDate = Common.ConvertDateTime(returnOn);
                    flightSearch.Adult = Convert.ToInt32(adult);
                    flightSearch.Child = Convert.ToInt32(child);
                    flightSearch.Infant = Convert.ToInt32(infant);
                    flightSearch.FlightClass = (FlightClass)Convert.ToByte(cabin);

                    flightSearch.IsDirect = Convert.ToBoolean(IsDirect);
                    flightSearch.IsFlexi = true;
                    // Flight fare API must use USD (do not pass MXN/other UI currencies)
                    flightSearch.Currency = "USD";
                    try
                    {
                        Response.Cookies.Append("SelectedCurrency", "USD", new CookieOptions
                        {
                            Expires = DateTimeOffset.UtcNow.AddHours(24),
                            IsEssential = true,
                            Path = "/"
                        });
                        Response.Cookies.Append("site_currency", "USD", new CookieOptions
                        {
                            Expires = DateTimeOffset.UtcNow.AddYears(1),
                            HttpOnly = false,
                            IsEssential = true,
                            Path = "/"
                        });
                        HttpContext.Session.SetString("SelectedCurrency", "USD");
                    }
                    catch { /* ignore cookie/session write failures */ }
                    int Fligtway;
                    if (flightSearch.FlightWay == FlightWay.RoundTrip)
                    {
                        Fligtway = 0;
                    }
                    else
                    {
                        Fligtway = 1;
                    }

                    flightSearch.SiteCode = utm_source;
                    flightSearch.SourceMedia = utm_source;

                    flightSearch.IsDeepLink = true;
                    Common.SourceMedia = utm_source;
                    flightSearch.IPAddress = remoteIpAddress.ToString();

                    if (SearchId != null && SearchId != "")
                    {
                        DataTable dtFlight = SqlData.Data.SqlHelperData.ExecuteDataset(Common.ConnectionString, CommandType.Text, "SELECT * FROM FlightsSearch_New Where SearchId='" + SearchId + "'").Tables[0];
                        List<FlightsResult> lstFlight = new List<FlightsResult>();
                        FlightsResult obj = new FlightsResult();
                        if (dtFlight.Rows.Count != 0)
                        {
                            var XMLString = dtFlight.Rows[0].ItemArray[1].ToString();
                            flightsResult = JsonConvert.DeserializeObject<FlightsResult>(XMLString);
                            flightsResult.SearchId = SearchId;
                        }
                        else
                        {

                            flightsResult = Common.SearchFlight(flightSearch);

                        }
                    }
                    else
                    {
                        flightsResult = Common.SearchFlight(flightSearch);

                        //var context = new RSFLY();
                        //var TotalCount = Convert.ToInt32(adult) + Convert.ToInt32(child) + Convert.ToInt32(infant);
                        //var Campeign = context.TblCampaigns.Where(x => x.CampId == "Rush_FS" && x.Status == true && x.PaxCount <= TotalCount).FirstOrDefault();
                        //if (Campeign != null)
                        //{
                        //    if (Campeign.Status == true)
                        //    {
                        //        flightsResult = Common.SearchFlight(flightSearch);

                        //    }
                        //}
                    }

                    FlightsResult newFlightsResult = new FlightsResult();

                    if (flightsResult != null && flightsResult.FlightsList != null)
                    {
                        string[] Stops = new string[5];
                        // Stops[0] = "0";
                        string[] Airlines = new string[30];

                        var avgAmount = (flightsResult.FlightsList.Select(x => x.FlightFare.AvgCost).FirstOrDefault());
                        string MinPrice = ((int)Math.Floor(flightsResult.FlightsList.Min(f => f.TotalCost))).ToString();

                        //var avgAmount = (flightsResult.FlightsList.Select(x => x.FlightFare.AvgCost).FirstOrDefault());
                        // FlightsResult newFlightsResult = FillFlightPrice(flightsResult);

                        //string MinPrice = ((int)Math.Floor(flightsResult.FlightsList.Min(f => f.TotalCost))).ToString();
                        //MinPrice = Convert.ToString(avgAmount);

                        //string MaxPrice = ((int)Math.Floor(flightsResult.FlightsList.Max(f => f.TotalCost))).ToString();
                        string MaxPrice = flightsResult.FlightsList.Max(f => f.TotalCost).ToString("0.00");
                        List<Airline> lstAir = new List<Airline>();
                        newFlightsResult = GetFilterFlightList(true, flightsResult, Stops, Airlines, MinPrice, MaxPrice, true, "0", "0", "0", "price", out lstAir);
                        //newFlightsResult.SearchId = flightsResult.SearchId;
                        newFlightsResult.SearchId = flightsResult.FlightsSearch.SearchId;

                        AplicationHelper.Save(newFlightsResult.SearchId, "flightsResult", flightsResult);

                        newFlightsResult.ListAirline = lstAir;
                        ViewBag.Currency = LocaleCurrencyHelper.GetCurrencySymbol(
                            LocaleCurrencyHelper.GetSelectedCurrency(HttpContext));
                        string sitepath = Common.WebSitePath;
                        ViewBag.WebSitePath = sitepath;
                        ViewBag.ResultCount = newFlightsResult.FlightsList.Count;
                        ViewBag.IsAvailable = true;

                        // Tracker off critical path
                        // Tracker off critical path — capture everything BEFORE Task.Run
                        var searchIdForTracker = flightsResult.SearchId;
                        var scopeFactory = _scopeFactory;
                        var logger = _logger;
                        var cabinCopy = cabin;
                        var departCopy = depart;
                        var arrivalCopy = arrival;
                        var departOnCopy = departOn;
                        var returnOnCopy = returnOn;
                        var adultCopy = adult;
                        var childCopy = child;
                        var infantCopy = infant;
                        var fligtwayCopy = Fligtway;
                        var utmCopy = utm_source;
                        var searchIdParam = SearchId;
                        var fromCode = flightSearch.From?.Code ?? "";
                        var toCode = flightSearch.To?.Code ?? "";
                        var isRound = flightSearch.FlightWay == FlightWay.RoundTrip;
                        var sourceCopy = string.IsNullOrWhiteSpace(flightSearch.SourceMedia) ? "Internal" : flightSearch.SourceMedia;
                        var ipCopy = clientIp;
                        var uaCopy = string.IsNullOrWhiteSpace(userAgent) ? "unknown" : userAgent;
                        var deviceCopy = device;

                        if (string.IsNullOrEmpty(SearchId))
                        {
                            flightSearch.SearchId = newFlightsResult.SearchId;   // keep IDs consistent

                            await _deepLinkTrackerService.SaveFareCompareTrafficAsync(
                                flightSearch,
                                flightSearch.DepDate,
                                flightSearch.FlightWay == FlightWay.RoundTrip ? flightSearch.RetDate : (DateTime?)null,
                                utm_source,
                                null,          // utm_campaign — add as action parameter if you need it
                                null,          // subId — same
                                clientIp,
                                userAgent);
                        }

                        //    _ = Task.Run(async () =>
                        //    {
                        //        try
                        //        {
                        //            using (var scope = scopeFactory.CreateScope())
                        //            {
                        //                var trackerService = scope.ServiceProvider.GetRequiredService<DeepLinkTrackerService>();
                        //                string stage = "ListingPage";
                        //                if (string.IsNullOrEmpty(searchIdForTracker)) return;

                        //                bool isDeepLink = utmCopy == "deeplink" || utmCopy != "HOME";

                        //                if (isDeepLink)
                        //                {
                        //                    await trackerService.UpdateStageAsync(searchIdParam, stage);
                        //                }
                        //                else
                        //                {
                        //                    var metaRequest = new MetaRequestDto.MetaFlightSearchRequest
                        //                    {
                        //                        Source = sourceCopy,          // SourceMedia (e.g. Farecompare)
                        //                        Device = deviceCopy,          // Desktop / Mobile / Tablet
                        //                        GeneralInfo = new MetaRequestDto.MetaGeneralInfo
                        //                        {
                        //                            GUID = searchIdForTracker,
                        //                            Cabin = cabinCopy,
                        //                            TripType = fligtwayCopy == 0 ? "ROUND" : "ONE-WAY",
                        //                            JourneyType = "DOM",
                        //                            Currency = "USD",
                        //                            IPAddress = ipCopy
                        //                        },
                        //                        PaxDetails = new MetaRequestDto.MetaPaxDetails
                        //                        {
                        //                            Adult = Convert.ToInt32(adultCopy),
                        //                            Child = Convert.ToInt32(childCopy),
                        //                            Infant = Convert.ToInt32(infantCopy),
                        //                            Youth = 0
                        //                        },
                        //                        Segments = new List<MetaRequestDto.MetaSegment>
                        //{
                        //    new MetaRequestDto.MetaSegment
                        //    {
                        //        DepartApt = departCopy,
                        //        ArrivalApt = arrivalCopy,
                        //        DepartDate = departOnCopy
                        //    },
                        //    isRound
                        //        ? new MetaRequestDto.MetaSegment
                        //        {
                        //            DepartApt = toCode,
                        //            ArrivalApt = fromCode,
                        //            DepartDate = returnOnCopy
                        //        }
                        //        : null
                        //}.Where(x => x != null).ToList()
                        //                    };

                        //                    await trackerService.CreateOrGetTrackerAsync(metaRequest, stage, null, ipCopy, uaCopy);
                        //                }
                        //            }
                        //        }
                        //        catch (Exception ex)
                        //        {
                        //            logger.LogError(ex, "Error in tracker Task.Run");
                        //        }
                        //    });



                        const int pageSize = 14;
                        ViewBag.Trip = newFlightsResult.FlightsSearch.FlightWay.ToString();
                        ViewBag.Depart = newFlightsResult.FlightsSearch.DepDate.ToString();
                        ViewBag.Return = newFlightsResult.FlightsSearch.RetDate.ToString();
                        try { HttpContext.Session.SetObjectAsJson("Paginationresult", newFlightsResult); } catch { }

                        var pageResult = new FlightsResult
                        {
                            SearchId = newFlightsResult.SearchId,
                            FlightsSearch = newFlightsResult.FlightsSearch,
                            FlightsList = newFlightsResult.FlightsList.Take(pageSize).ToList(),
                            ListAirline = newFlightsResult.ListAirline,
                            StopExtra = newFlightsResult.StopExtra,
                            BarExtra = newFlightsResult.BarExtra,
                            MinPrice = newFlightsResult.MinPrice,
                            MaxPrice = newFlightsResult.MaxPrice
                        };

                        var leftHtml = await RenderPartialToStringAsync("_LeftPanelResult", newFlightsResult);
                        var resultsHtml = await RenderPartialToStringAsync("_result", pageResult);
                        var bundle =
                            "<div id=\"rmf-search-bundle\">" +
                            "<div id=\"rmf-left\">" + leftHtml + "</div>" +
                            "<div id=\"rmf-results\" data-total=\"" + newFlightsResult.FlightsList.Count + "\">" + resultsHtml + "</div>" +
                            "</div>";
                        return Content(bundle, "text/html; charset=utf-8");
                    }
                    else
                    {
                        newFlightsResult = flightsResult;
                        ViewBag.IsAvailable = false;
                        return PartialView("_LeftPanelResult", newFlightsResult);

                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else
            {
                try
                {
                    MultiCityFlightsResult flightsResult = new MultiCityFlightsResult();
                    List<FlightLeg> multifightlist = new List<FlightLeg>();
                    multifightlist = JsonConvert.DeserializeObject<List<FlightLeg>>(Multiselect);
                    var remoteIpAddress = Request.HttpContext.Connection.RemoteIpAddress;
                    MultiFlightsSearch flightSearch = new MultiFlightsSearch();
                    //MultipleFlightsSearch flightSearches = new MultipleFlightsSearch();
                    //var List = flightSearches.multiplelFlightlist.ToList();

                    //foreach( var item in List)
                    //{
                    //    item.From = Common.GetAirport(depart.ToString().ToString());

                    //}
                    flightSearch.FlightLegs = multifightlist;
                    flightSearch.SearchId = Guid.NewGuid().ToString();
                    flightSearch.FlightWay = (FlightWay)Convert.ToByte(trip);
                    //flightSearch.From = Common.GetAirport(depart.ToString().ToString());
                    //flightSearch.To = Common.GetAirport(arrival);
                    //flightSearch.DepDate = Common.ConvertDateTime(departOn);

                    if (flightSearch.FlightWay == FlightWay.RoundTrip)
                        //flightSearch.RetDate = Common.ConvertDateTime(returnOn);
                        flightSearch.Adult = /*Convert.ToInt32(adult);1;
                    flightSearch.Child = /*Convert.ToInt32(child);*/1;
                    flightSearch.Infant = Convert.ToInt32(infant);
                    flightSearch.FlightClass = (FlightClass)Convert.ToByte(cabin);

                    flightSearch.IsDirect = Convert.ToBoolean(IsDirect);
                    flightSearch.IsFlexi = true;
                    flightSearch.Adult = Convert.ToInt32(adult);
                    int Fligtway;
                    if (flightSearch.FlightWay == FlightWay.RoundTrip)
                    {
                        Fligtway = 0;
                    }
                    else
                    {
                        Fligtway = 1;
                    }

                    flightSearch.SiteCode = utm_source;
                    flightSearch.SourceMedia = utm_source;

                    flightSearch.IsDeepLink = true;
                    Common.SourceMedia = utm_source;
                    flightSearch.IPAddress = remoteIpAddress.ToString();

                    if (SearchId != null && SearchId != "")
                    {
                        DataTable dtFlight = SqlData.Data.SqlHelperData.ExecuteDataset(Common.ConnectionString, CommandType.Text, "SELECT * FROM FlightsSearch_New Where SearchId='" + SearchId + "'").Tables[0];
                        List<FlightsResult> lstFlight = new List<FlightsResult>();
                        FlightsResult obj = new FlightsResult();
                        if (dtFlight.Rows.Count != 0)
                        {
                            var XMLString = dtFlight.Rows[0].ItemArray[1].ToString();
                            flightsResult = JsonConvert.DeserializeObject<MultiCityFlightsResult>(XMLString);
                            flightsResult.SearchId = SearchId;
                        }
                        else
                        {

                            flightsResult = Common.MultisearchSearchFlight(flightSearch);

                        }
                    }
                    else
                    {
                        flightsResult = Common.MultisearchSearchFlight(flightSearch);

                    }

                    MultiCityFlightsResult newFlightsResult = new MultiCityFlightsResult();

                    if (flightsResult != null && flightsResult.MultiFlightsList != null)
                    {
                        string[] Stops = new string[5];
                        // Stops[0] = "0";
                        string[] Airlines = new string[30];

                        var avgAmount = (flightsResult.MultiFlightsList.Select(x => x.FlightsList.Select(x => x.FlightFare.AvgCost).FirstOrDefault()));
                        // FlightsResult newFlightsResult = FillFlightPrice(flightsResult);

                        string MinPrice = null;
                        string MaxPrice = null;
                        var rrsukt = flightsResult.MultiFlightsList.ToList();
                        foreach (var item in rrsukt)
                        {
                            MinPrice = ((int)Math.Floor(item.FlightsList.Min(f => f.TotalCost))).ToString();
                            MaxPrice = ((int)Math.Floor(item.FlightsList.Max(f => f.TotalCost))).ToString();
                        }
                        //MinPrice = Convert.ToString(avgAmount);


                        List<Airline> lstAir = new List<Airline>();
                        // newFlightsResult = GetFilterFlightList(true, flightsResult, Stops, Airlines, MinPrice, MaxPrice, true, "0", "0", "0", "price", out lstAir);
                        //newFlightsResult.SearchId = flightsResult.SearchId;
                        newFlightsResult.SearchId = flightsResult.FlightsSearch.SearchId;
                        HttpContext.Session.SetObjectAsJson("flightsResult", flightsResult);
                        newFlightsResult = flightsResult;
                        var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(flightsResult);
                        HttpContext.Session.SetString("flightsResult", jsonString);

                        // newFlightsResult.ListAirline = lstAir;
                        ViewBag.Currency = LocaleCurrencyHelper.GetCurrencySymbol(
                            LocaleCurrencyHelper.GetSelectedCurrency(HttpContext));
                        string sitepath = Common.WebSitePath;
                        ViewBag.WebSitePath = sitepath;
                        //ViewBag.ResultCount = newFlightsResult.FlightsList.Count;
                        ViewBag.IsAvailable = true;
                        ViewBag.allvalue = jsonString.ToString();
                        List<string> vas = new List<string>();
                        //foreach (var item in newFlightsResult.ListAirline)
                        //{
                        //    var vals = newFlightsResult.FlightsList.Where(x => x.Airline.Code == item.Code).Count();
                        //    if (vals <= 1)
                        //    {
                        //        vas.Add(item.Code);
                        //    }
                        //}
                        //if (vas.Count > 0)
                        //{
                        //    foreach (var items in vas)
                        //    {
                        //        newFlightsResult.MultiFlightsList.RemoveAll(x => x.Airline.Code == items);
                        //        newFlightsResult.ListAirline.RemoveAll(x => x.Code == items);
                        //    }
                        //}
                        FlightsResult nfls = new FlightsResult();
                        // nfls.NewFlightsResult = newFlightsResult;
                        return PartialView("_LeftPanelResult", newFlightsResult);
                    }
                    else
                    {
                        newFlightsResult = flightsResult;
                        ViewBag.IsAvailable = false;
                        return PartialView("_LeftPanelResult", newFlightsResult);

                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

        }



        [Route("flightresult/Partial/result")]
        public PartialViewResult OnGetFlightResultPartial(string Stopsx, string Airlinex, string MinPrice, string MaxPrice, string SortPrice, string SortDuration, string SorByAirlineName, string SortResult, string aurlinesnum, int Pageindex, int PageSize, string OutDep, string OutArr, string InnDep, string InnArr, string DepSlots, string searchID)
        {
            FlightsResult flightresut = new FlightsResult();
            try
            {
                int Range = ((Pageindex * PageSize)) + PageSize;
                int skip = 0;
                if (Range != 14)
                {
                    skip = Range / 2;
                }
                //string[] Stops = Stopsx.Split(',');

                //FlightsResult newFlightsResult = new FlightsResult();
                //string[] Airlines = Airlinex.Split(',');
                string[] Stops = string.IsNullOrWhiteSpace(Stopsx) ? Array.Empty<string>() : Stopsx.Split(',').Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();

                string[] Airlines = string.IsNullOrWhiteSpace(Airlinex) ? Array.Empty<string>() : Airlinex.Split(',').Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();

                FlightsResult newFlightsResult = new FlightsResult();
                List<Airline> lstAir = new List<Airline>();

                FlightsResult savedFlightsResult = AplicationHelper.Get<FlightsResult>(searchID, "flightsResult");

                if (savedFlightsResult == null)
                {
                    return PartialView("_result", new FlightsResult
                    {
                        FlightsList = new List<Flights>(),
                        SearchId = searchID
                    });
                }

                FlightsResult flightSearch = savedFlightsResult;

                newFlightsResult = GetFilterFlightList(false, flightSearch, Stops, Airlines, MinPrice, MaxPrice, false, SortPrice, SortDuration, SorByAirlineName, SortResult, out lstAir);
                if (newFlightsResult?.FlightsList == null)
                {
                    return PartialView("_result", new FlightsResult
                    {
                        FlightsList = new List<Flights>(),
                        SearchId = searchID,
                        FlightsSearch = flightSearch.FlightsSearch
                    });
                }

                if (!(OutDep == null && OutArr == null && InnDep == null && InnArr == null))
                {
                    if (!string.IsNullOrEmpty(OutDep) && OutDep.Contains("-"))
                        newFlightsResult = OutBoundDepartureSlide(newFlightsResult, OutDep, 0);

                    if (!string.IsNullOrEmpty(OutArr) && OutArr.Contains("-"))
                        newFlightsResult = OutBoundDepartureSlide(newFlightsResult, OutArr, 1);

                    if (newFlightsResult?.FlightsList != null &&
                        newFlightsResult.FlightsList.Any() &&
                        newFlightsResult.FlightsList[0].Inbound != null &&
                        newFlightsResult.FlightsList[0].Inbound.Count > 0)
                    {
                        if (!string.IsNullOrEmpty(InnDep) && InnDep.Contains("-"))
                            newFlightsResult = InBoundDepartureSlide(newFlightsResult, InnDep, 0);

                        if (!string.IsNullOrEmpty(InnArr) && InnArr.Contains("-"))
                            newFlightsResult = InBoundDepartureSlide(newFlightsResult, InnArr, 1);
                    }
                }

                if (!string.IsNullOrWhiteSpace(DepSlots))
                {
                    var ranges = DepSlots.Split('|')
                        .Where(x => !string.IsNullOrWhiteSpace(x))
                        .Select(x => x.Split('-'))
                        .Where(x => x.Length == 2)
                        .Select(x => new
                        {
                            Start = Convert.ToInt32(x[0]),
                            End = Convert.ToInt32(x[1])
                        })
                        .ToList();

                    newFlightsResult.FlightsList = newFlightsResult.FlightsList
                        .Where(f =>
                        {
                            var depTime = f.Outbound[0].DepartureDate.TimeOfDay.TotalMinutes;
                            return ranges.Any(r => depTime >= r.Start && depTime <= r.End);
                        })
                        .ToList();
                }

                flightresut.SearchId = searchID;
                flightresut.FlightsList = newFlightsResult.FlightsList.Skip((Pageindex - 1) * PageSize).Take(PageSize).ToList();
                flightresut.FlightsSearch = newFlightsResult.FlightsSearch;

                // Session write of filtered list for load-more (single serialize)
                try
                {
                    HttpContext.Session.SetObjectAsJson("Paginationresult", newFlightsResult);
                }
                catch { /* session optional */ }

                ViewBag.Trip = flightresut.FlightsSearch.FlightWay.ToString();
                ViewBag.Depart = flightresut.FlightsSearch.DepDate.ToString();
                ViewBag.Return = flightresut.FlightsSearch.RetDate.ToString();
                ViewBag.IsAvailable = true;

            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText(
       Path.Combine(Directory.GetCurrentDirectory(), "flight_error.txt"),
       DateTime.Now + " => " + ex + Environment.NewLine
   );
            }
            if (flightresut.FlightsSearch.FlightClass.ToString() == "Business")
            {
                return PartialView("_businessresult", flightresut);
            }
            else
            {
                return PartialView("_result", flightresut);
            }

        }
        private void LogStep(string message)
        {
            // Hot-path logging disabled (was sync disk I/O on every filter click)
        }

        private async Task<string> RenderPartialToStringAsync<TModel>(string viewName, TModel model)
        {
            var engine = HttpContext.RequestServices.GetService(typeof(ICompositeViewEngine)) as ICompositeViewEngine;
            if (engine == null)
                return string.Empty;

            var viewResult = engine.FindView(ControllerContext, viewName, false);
            if (!viewResult.Success)
                viewResult = engine.GetView(null, $"~/Views/Shared/{viewName}.cshtml", false);
            if (!viewResult.Success)
                viewResult = engine.GetView(null, $"~/Views/Result/{viewName}.cshtml", false);
            if (!viewResult.Success)
                return string.Empty;

            await using var writer = new StringWriter();
            var viewData = new ViewDataDictionary(ViewData) { Model = model };
            var viewContext = new ViewContext(
                ControllerContext,
                viewResult.View,
                viewData,
                TempData,
                writer,
                new HtmlHelperOptions());
            await viewResult.View.RenderAsync(viewContext);
            return writer.ToString();
        }

        //    [Route("flightresult/Partial/result")]
        //    public PartialViewResult OnGetFlightResultPartial(
        //string Stopsx,
        //string Airlinex,
        //string MinPrice,
        //string MaxPrice,
        //string SortPrice,
        //string SortDuration,
        //string SorByAirlineName,
        //string SortResult,
        //string aurlinesnum,
        //int Pageindex,
        //int PageSize,
        //string OutDep,
        //string OutArr,
        //string InnDep,
        //string InnArr,
        //string searchID)
        //    {
        //        FlightsResult flightresut = new FlightsResult();

        //        try
        //        {
        //            int Range = ((Pageindex * PageSize)) + PageSize;
        //            int skip = 0;
        //            if (Range != 15)
        //            {
        //                skip = Range / 2;
        //            }

        //            string[] Stops = (Stopsx ?? "")
        //                .Split(',', StringSplitOptions.RemoveEmptyEntries);

        //            string[] Airlines = (Airlinex ?? "")
        //                .Split(',', StringSplitOptions.RemoveEmptyEntries);

        //            List<Airline> lstAir = new List<Airline>();

        //            // 🔹 Get session data safely
        //            FlightsResult savedFlightsResult =
        //                HttpContext.Session.GetObjectFromJson<FlightsResult>("flightsResult");

        //            if (savedFlightsResult == null)
        //            {
        //                var vals = HttpContext.Session.GetString("flightsResult");
        //                if (!string.IsNullOrEmpty(vals))
        //                {
        //                    FlightsResult fls =
        //                        Newtonsoft.Json.JsonConvert.DeserializeObject<FlightsResult>(vals);
        //                    HttpContext.Session.SetObjectAsJson("flightsResult", fls);
        //                    savedFlightsResult = fls;
        //                }
        //            }

        //            if (savedFlightsResult == null)
        //            {
        //                ViewBag.IsAvailable = false;
        //                return PartialView("_result", flightresut);
        //            }

        //            FlightsResult flightSearch = savedFlightsResult;

        //            // 🔹 FILTER LOGIC (UNCHANGED)
        //            FlightsResult newFlightsResult = GetFilterFlightList(
        //                false,
        //                flightSearch,
        //                Stops,
        //                Airlines,
        //                MinPrice,
        //                MaxPrice,
        //                false,
        //                SortPrice,
        //                SortDuration,
        //                SorByAirlineName,
        //                SortResult,
        //                out lstAir
        //            );

        //            if (newFlightsResult?.FlightsList == null ||
        //                !newFlightsResult.FlightsList.Any())
        //            {
        //                flightresut.SearchId = searchID;
        //                ViewBag.IsAvailable = false;
        //                return PartialView("_result", flightresut);
        //            }

        //            // 🔹 Time filters (FORMAT FIX : instead of "-")
        //            if (!string.IsNullOrEmpty(OutDep) && OutDep.Contains(":"))
        //            {
        //                newFlightsResult = OutBoundDepartureSlide(newFlightsResult, OutDep, 0);
        //            }

        //            if (!string.IsNullOrEmpty(OutArr) && OutArr.Contains(":"))
        //            {
        //                newFlightsResult = OutBoundDepartureSlide(newFlightsResult, OutArr, 1);
        //            }

        //            // 🔹 SAFE INBOUND CHECK
        //            if (newFlightsResult.FlightsList.Any() &&
        //                newFlightsResult.FlightsList[0].Inbound != null &&
        //                newFlightsResult.FlightsList[0].Inbound.Any())
        //            {
        //                if (!string.IsNullOrEmpty(InnDep) && InnDep.Contains(":"))
        //                {
        //                    newFlightsResult = InBoundDepartureSlide(newFlightsResult, InnDep, 0);
        //                }

        //                if (!string.IsNullOrEmpty(InnArr) && InnArr.Contains(":"))
        //                {
        //                    newFlightsResult = InBoundDepartureSlide(newFlightsResult, InnArr, 1);
        //                }
        //            }

        //            // 🔹 PAGINATION (UNCHANGED)
        //            flightresut.SearchId = searchID;
        //            flightresut.FlightsList = newFlightsResult.FlightsList
        //                .Skip((Pageindex - 1) * PageSize)
        //                .Take(PageSize)
        //                .ToList();

        //            flightresut.FlightsSearch = newFlightsResult.FlightsSearch;

        //            ViewBag.Trip = flightresut.FlightsSearch?.FlightWay.ToString();
        //            ViewBag.Depart = flightresut.FlightsSearch?.DepDate.ToString();
        //            ViewBag.Return = flightresut.FlightsSearch?.RetDate.ToString();

        //            ViewBag.IsAvailable = true;

        //            HttpContext.Session.SetObjectAsJson("FlightsResultDetail", newFlightsResult);
        //            HttpContext.Session.SetObjectAsJson("newFlightsResult", newFlightsResult);
        //            HttpContext.Session.SetObjectAsJson("Paginationresult", flightresut);
        //        }
        //        catch (Exception ex)
        //        {

        //            ViewBag.IsAvailable = false;
        //        }

        //        return PartialView("_result", flightresut);
        //    }


        [Route("flightresult/Partial/resultviewNew")]
        public PartialViewResult OnGetFlightResultPartialNew(int Pageindex, int PageSize, string Airlinex, string searchID)
        {
            // PageSize = 100;
            int pagefrom = (Pageindex * PageSize) + 1;
            int Range = ((Pageindex * PageSize)) + PageSize;
            //int Range = 10;
            string[] Airlines = Airlinex.Split(',');

            if (Airlines[0] == "")
            {
                try
                {

                    List<Flights> flt = new List<Flights>();


                    var flightList = HttpContext.Session.GetObjectFromJson<FlightsResult>("Paginationresult") as FlightsResult;


                    if (flightList == null)
                    {
                        var vals = HttpContext.Session.GetString("flightsResult");
                        var jsonString = JsonConvert.SerializeObject(vals);
                        if (vals != null)
                        {
                            FlightsResult fls = Newtonsoft.Json.JsonConvert.DeserializeObject<FlightsResult>(vals);
                            HttpContext.Session.SetObjectAsJson("flightsResult", fls);
                        }
                    }
                    ViewBag.Trip = flightList.FlightsSearch.FlightWay.ToString();
                    ViewBag.Depart = flightList.FlightsSearch.DepDate.ToString();
                    ViewBag.Return = flightList.FlightsSearch.RetDate.ToString();

                    ViewBag.IsAvailable = true;

                    return PartialView("_result", flightList);
                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }
            else
            {

                try
                {

                    //var check = HttpContext.Session.GetObjectFromJson<FlightsResult>("partialflightlist23") as FlightsResult;
                    //if (check != null)
                    //{
                    //    string SearhflightId = check.FlightsList.Select(x => x.FlightId).FirstOrDefault();

                    //    SearchandflightId = SearhflightId + "," + check.SearchId;
                    //    ViewBag.Trip = check.FlightsSearch.FlightWay.ToString();
                    //    return PartialView("_result", check);

                    //}
                    List<Flights> flt = new List<Flights>();
                    //var flightList = HttpContext.Session.GetObjectFromJson<FlightsResult>("newFlightsResult") as FlightsResult;
                    var flightList = HttpContext.Session.GetObjectFromJson<FlightsResult>("Paginationresult") as FlightsResult;

                    if (flightList == null)
                    {
                        var vals = HttpContext.Session.GetString("flightsResult");
                        var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(vals);
                        if (vals != null)
                        {
                            FlightsResult fls = Newtonsoft.Json.JsonConvert.DeserializeObject<FlightsResult>(vals);
                            HttpContext.Session.SetObjectAsJson("flightsResult", fls);
                        }
                    }

                    if (Range <= flightList.FlightsList.Count)
                    {

                    }
                    //for (int ck = pagefrom; ck < Range; ck++)
                    //{
                    //    try
                    //    {
                    //        flt.Add(flightList.FlightsList[ck - 1]);
                    //    }
                    //    catch
                    //    {
                    //        //return PartialView("_noresult", flightresut);
                    //    }
                    //}
                    //flightList.FlightsList = flt;
                    ViewBag.IsAvailable = true;


                    string SearchandflightId = flightList.FlightsList.Select(x => x.FlightId).FirstOrDefault();

                    SearchandflightId = SearchandflightId + "," + flightList.SearchId;
                    ViewBag.Trip = flightList.FlightsSearch.FlightWay.ToString();

                    return PartialView("_result", flightList);


                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        //End

        public FlightsResult OutBoundDepartureSlide(FlightsResult newFlightsResult, string OutDepArr, int Type)
        {
            FlightsResult flightresutXX = new FlightsResult();

            string[] OutbDep = OutDepArr.Split("-");
            List<Flights> flights = newFlightsResult.FlightsList;
            List<Flights> flightsnew = new List<Flights>();
            // List<FlightsSegment> fls = new List<FlightsSegment>();

            DataTable dt2 = new DataTable();
            dt2.Columns.Add("FlightID", typeof(string));
            dt2.Columns.Add("OutBoundTime", typeof(int));
            DataRow drow;
            foreach (var lst in flights)
            {
                drow = dt2.NewRow();
                drow["FlightID"] = lst.FlightId;
                if (Type == 0)
                    drow["OutBoundTime"] = Convert.ToDateTime(lst.Outbound[0].DepartureDate).TimeOfDay.TotalSeconds;
                else
                    drow["OutBoundTime"] = Convert.ToDateTime(lst.Outbound[lst.Outbound.Count - 1].ArrivalDate).TimeOfDay.TotalSeconds;
                dt2.Rows.Add(drow);
                dt2.AcceptChanges();
            }

            DataRow[] drowx = dt2.Select("OutBoundTime>" + Convert.ToDateTime(OutbDep[0]).TimeOfDay.TotalSeconds + " and " + "OutBoundTime<" + Convert.ToDateTime(OutbDep[1]).TimeOfDay.TotalSeconds);

            foreach (DataRow drx in drowx)
            {
                flightsnew.Add(newFlightsResult.FlightsList.Where(g => g.FlightId == drx["FlightID"].ToString()).FirstOrDefault());
            }
            newFlightsResult.FlightsList = flightsnew.OrderBy(m => m.TotalCost).ToList();

            return newFlightsResult;
        }
        public FlightsResult InBoundDepartureSlide(FlightsResult newFlightsResult, string InDepArr, int Type)
        {
            try
            {
                FlightsResult flightresutXX = new FlightsResult();
                var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(newFlightsResult);
                // _logger.LogInformation(jsonString.ToString());

                string[] OutbDep = InDepArr.Split("-");
                List<Flights> flights = newFlightsResult.FlightsList;
                List<Flights> flightsnew = new List<Flights>();
                // List<FlightsSegment> fls = new List<FlightsSegment>();

                DataTable dt2 = new DataTable();
                dt2.Columns.Add("FlightID", typeof(string));
                dt2.Columns.Add("OutBoundTime", typeof(int));
                DataRow drow;
                foreach (var lst in flights)
                {
                    drow = dt2.NewRow();
                    drow["FlightID"] = lst.FlightId;
                    if (Type == 0)
                        drow["OutBoundTime"] = Convert.ToDateTime(lst.Inbound[0].DepartureDate).TimeOfDay.TotalSeconds;
                    else
                        drow["OutBoundTime"] = Convert.ToDateTime(lst.Inbound[lst.Inbound.Count - 1].ArrivalDate).TimeOfDay.TotalSeconds;
                    dt2.Rows.Add(drow);
                    dt2.AcceptChanges();
                }

                DataRow[] drowx = dt2.Select("OutBoundTime>" + Convert.ToDateTime(OutbDep[0]).TimeOfDay.TotalSeconds + " and " + "OutBoundTime<" + Convert.ToDateTime(OutbDep[1]).TimeOfDay.TotalSeconds);

                foreach (DataRow drx in drowx)
                {
                    flightsnew.Add(newFlightsResult.FlightsList.Where(g => g.FlightId == drx["FlightID"].ToString()).FirstOrDefault());
                }
                newFlightsResult.FlightsList = flightsnew.OrderBy(m => m.TotalCost).ToList();

                return newFlightsResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                _logger.LogError(ex.InnerException.ToString());
                _logger.LogError(ex.StackTrace.ToString());
                return newFlightsResult;
            }
        }

        public FlightsResult GetFilterFlightList(bool isLoad, FlightsResult flight, string[] Stops, string[] Airlines, string MinPrice, string MaxPrice, bool checkmain, string SortPrice, string SortDuration, string SorByAirlineName, string SortResult, out List<Airline> lstAir)
        {
            try
            {
                double atolCharge = 0;

                var atolChargeValue = _configuration["AppSettings:AtolCharge"];

                double.TryParse(atolChargeValue, out atolCharge);

                FlightsResult checkFlightCountZero = new FlightsResult();
                FlightsResult newFlight = new FlightsResult();
                newFlight = flight;
                bool lbool = false;
                List<Airline> ListAirlines = new List<Airline>();

                // Filter clicks (!isLoad): skip merge / stop-bucket rebuild — data already prepared at search time
                if (!isLoad)
                {
                    lstAir = ListAirlines;
                    return FilterFlightsFast(flight, Stops, Airlines, MinPrice, MaxPrice, SortPrice, SortDuration, SorByAirlineName, SortResult);
                }


                //  lstAir = ListAirlines;
                if (isLoad)
                {
                    // Fast airline matrix (no DataTable / AcceptChanges)
                    foreach (var lst in flight.FlightsList)
                    {
                        if (atolCharge > 0 && lst.FlightFare != null)
                        {
                            lst.FlightFare.AvgCost = lst.FlightFare.AvgCost + atolCharge;
                            lst.FlightFare.AtolCharge = atolCharge;
                        }
                    }

                    ListAirlines = flight.FlightsList
                        .Where(f => f.Outbound != null && f.Outbound.Count > 0 && f.Outbound[0].Airline != null)
                        .GroupBy(f => f.Outbound[0].Airline.Code, StringComparer.OrdinalIgnoreCase)
                        .Select(g =>
                        {
                            var cheapest = g.OrderBy(x => x.FlightFare?.GrandTotal ?? x.TotalCost).First();
                            var code = g.Key;
                            return new Airline
                            {
                                Code = code,
                                Stops = Math.Max(0, (cheapest.Outbound?.Count ?? 1) - 1),
                                Name = Common.GetAirline(code)?.Name ?? code,
                                TotalCost = (int)Math.Floor(cheapest.FlightFare?.GrandTotal ?? cheapest.TotalCost)
                            };
                        })
                        .OrderBy(a => a.TotalCost)
                        .ToList();
                }
                lstAir = ListAirlines;



                // min max count
                newFlight.MinPrice = (int)Math.Floor(flight.FlightsList.Min(f => f.TotalCost));
                //newFlight.MaxPrice = (int)Math.Floor(flight.FlightsList.Max(f => f.TotalCost));
                newFlight.MaxPrice = (int)Math.Ceiling(flight.FlightsList.Max(f => f.TotalCost));
                List<Flights> nonStop = flight.FlightsList.Where(f => (f.Outbound.Count == 1) && (f.Inbound.Count == 1)).OrderBy(f => f.TotalCost).ToList();
                List<Flights> oneStop = flight.FlightsList.Where(f => (f.Outbound.Count == 2) || (f.Inbound.Count == 2)).OrderBy(f => f.TotalCost).ToList();
                List<Flights> twoStop = flight.FlightsList.Where(f => (f.Outbound.Count >= 3) || (f.Inbound.Count >= 3)).OrderBy(f => f.TotalCost).ToList();
                List<Flights> threeStop = flight.FlightsList.Where(f => (f.Outbound.Count >= 4) || (f.Inbound.Count >= 4)).OrderBy(f => f.TotalCost).ToList();


                if (flight.FlightsSearch.FlightWay.GetHashCode() == 1)
                {

                    nonStop = flight.FlightsList.Where(f => (Convert.ToInt32(f.Outbound.Count) <= 0 + 1)).OrderBy(f => f.TotalCost).ToList();

                    oneStop = flight.FlightsList.Where(f => (Convert.ToInt32(f.Outbound.Count) <= 1 + 1)).OrderBy(f => f.TotalCost).ToList();

                    twoStop = flight.FlightsList.Where(f => (Convert.ToInt32(f.Outbound.Count) <= 2 + 1)).OrderBy(f => f.TotalCost).ToList();

                    threeStop = flight.FlightsList.Where(f => (Convert.ToInt32(f.Outbound.Count) <= 3 + 1)).OrderBy(f => f.TotalCost).ToList();

                }
                else
                {

                    nonStop = flight.FlightsList.Where(f => ((Convert.ToInt32(f.Outbound.Count) == 0 + 1) || (Convert.ToInt32(f.Inbound.Count) == 0 + 1)) && ((Convert.ToInt32(f.Outbound.Count) == 0 + 1) && (Convert.ToInt32(f.Inbound.Count) == 0 + 1))).OrderBy(f => f.TotalCost).ToList();

                    oneStop = flight.FlightsList.Where(f => ((Convert.ToInt32(f.Outbound.Count) == 1 + 1) || (Convert.ToInt32(f.Inbound.Count) == 1 + 1)) && ((Convert.ToInt32(f.Outbound.Count) <= 1 + 1) && (Convert.ToInt32(f.Inbound.Count) <= 1 + 1))).OrderBy(f => f.TotalCost).ToList();


                    twoStop = flight.FlightsList.Where(f => ((Convert.ToInt32(f.Outbound.Count) == 2 + 1) || (Convert.ToInt32(f.Inbound.Count) == 2 + 1)) && ((Convert.ToInt32(f.Outbound.Count) <= 2 + 1) && (Convert.ToInt32(f.Inbound.Count) <= 2 + 1))).OrderBy(f => f.TotalCost).ToList();


                    threeStop = flight.FlightsList.Where(f => ((Convert.ToInt32(f.Outbound.Count) == 3 + 1) || (Convert.ToInt32(f.Inbound.Count) == 3 + 1)) && ((Convert.ToInt32(f.Outbound.Count) <= 3 + 1) && (Convert.ToInt32(f.Inbound.Count) <= 3 + 1))).OrderBy(f => f.TotalCost).ToList();

                }



                newFlight.StopExtra = new List<FlightsExtra>();

                newFlight.StopExtra.Add(new FlightsExtra() { TotalCost = (nonStop.Count == 0) ? 0 : (int)Math.Floor(nonStop.Min(f => f.TotalCost)), FlightId = (nonStop.Count == 0) ? "" : nonStop[0].FlightId, ExtraType = "nonstop" });
                newFlight.StopExtra.Add(new FlightsExtra() { TotalCost = (oneStop.Count == 0) ? 0 : (int)Math.Floor(oneStop.Min(f => f.TotalCost)), FlightId = (oneStop.Count == 0) ? "" : oneStop[0].FlightId, ExtraType = "onestop" });
                newFlight.StopExtra.Add(new FlightsExtra() { TotalCost = (twoStop.Count == 0) ? 0 : (int)Math.Floor(twoStop.Min(f => f.TotalCost)), FlightId = (twoStop.Count == 0) ? "" : twoStop[0].FlightId, ExtraType = "twostop" });
                newFlight.StopExtra.Add(new FlightsExtra() { TotalCost = (threeStop.Count == 0) ? 0 : (int)Math.Floor(threeStop.Min(f => f.TotalCost)), FlightId = (threeStop.Count == 0) ? "" : threeStop[0].FlightId, ExtraType = "threeStop" });




                Flights cheapest = flight.FlightsList.OrderBy(f => f.TotalCost).FirstOrDefault();
                Flights shortest = flight.FlightsList.OrderBy(f => f.TotalTime).FirstOrDefault();
                Flights best = flight.FlightsList.OrderBy(f => f.TotalCost).FirstOrDefault();

                newFlight.BarExtra = new List<FlightsExtra>();
                newFlight.BarExtra.Add(new FlightsExtra() { TotalCost = cheapest.TotalCost, TotalTime = cheapest.TotalTime, Airline = cheapest.Airline, TotalStop = cheapest.Stops, FlightId = cheapest.FlightId, ExtraType = "cheapest", Provider = cheapest.Provider });
                newFlight.BarExtra.Add(new FlightsExtra() { TotalCost = shortest.TotalCost, TotalTime = shortest.TotalTime, Airline = shortest.Airline, TotalStop = shortest.Stops, FlightId = shortest.FlightId, ExtraType = "shortest", Provider = shortest.Provider });
                newFlight.BarExtra.Add(new FlightsExtra() { TotalCost = best.TotalCost, TotalTime = best.TotalTime, Airline = best.Airline, TotalStop = best.Stops, FlightId = best.FlightId, ExtraType = "best", Provider = best.Provider });


                List<FlightsSegment> outboundDep = newFlight.FlightsList.Select(cc => cc.Outbound[0]).OrderBy(kk => kk.DepartureDate.TimeOfDay.TotalMinutes).ToList();
                List<FlightsSegment> outboundArr = newFlight.FlightsList.Select(cc => cc.Outbound[cc.Outbound.Count - 1]).OrderBy(kk => kk.ArrivalDate.TimeOfDay.TotalMinutes).ToList();

                List<FlightsSegment> InboundDep = new List<FlightsSegment>();
                List<FlightsSegment> InboundArr = new List<FlightsSegment>();
                if (newFlight.FlightsList[0].Inbound.Count > 0)
                {
                    InboundDep = newFlight.FlightsList
    .Where(cc => cc.Inbound != null && cc.Inbound.Count > 0)
    .Select(cc => cc.Inbound[0])
    .OrderBy(kk => kk.DepartureDate.TimeOfDay.TotalMinutes)
    .ToList();
                    InboundArr = newFlight.FlightsList
    .Select(cc => cc.Inbound?.LastOrDefault())
    .Where(x => x != null)
    .OrderBy(kk => kk.ArrivalDate.TimeOfDay.TotalMinutes)
    .ToList();
                    var inboundlistnew = newFlight.FlightsList.Where(cc => cc.Inbound == null).ToList();
                    //InboundDep = newFlight.FlightsList.Select(cc => cc.Inbound[0]).OrderBy(kk => kk.DepartureDate.TimeOfDay.TotalMinutes).ToList();
                    //InboundArr = newFlight.FlightsList.Select(cc => cc.Inbound[cc.Inbound.Count - 1]).OrderBy(kk => kk.ArrivalDate.TimeOfDay.TotalMinutes).ToList();

                }

                newFlight.OutBoundDep = Convert.ToDateTime(outboundDep.First().DepartureDate).TimeOfDay.TotalMinutes.ToString() + ":" + Convert.ToDateTime(outboundDep.Last().DepartureDate).TimeOfDay.TotalMinutes.ToString();
                newFlight.OutBoundArr = Convert.ToDateTime(outboundArr.First().ArrivalDate).TimeOfDay.TotalMinutes.ToString() + ":" + Convert.ToDateTime(outboundArr.Last().ArrivalDate).TimeOfDay.TotalMinutes.ToString();
                if (newFlight.FlightsList[0].Inbound.Count > 0)
                {
                    newFlight.InBoundDep = Convert.ToDateTime(InboundDep.First().DepartureDate).TimeOfDay.TotalMinutes.ToString() + ":" + Convert.ToDateTime(InboundDep.Last().DepartureDate).TimeOfDay.TotalMinutes.ToString();
                    newFlight.InBoundArr = Convert.ToDateTime(InboundArr.First().ArrivalDate).TimeOfDay.TotalMinutes.ToString() + ":" + Convert.ToDateTime(InboundArr.Last().ArrivalDate).TimeOfDay.TotalMinutes.ToString();


                }


                // }
                //}

                // mearge flight segment list (skip when FlightIds already unique)
                List<Flights> newFlightList;
                var distinctCount = flight.FlightsList.Select(f => f.FlightId).Distinct().Count();
                if (distinctCount == flight.FlightsList.Count)
                {
                    newFlightList = flight.FlightsList;
                    var cur = LocaleCurrencyHelper.GetCurrencySymbol(
                        LocaleCurrencyHelper.GetSelectedCurrency(HttpContext));
                    foreach (var flightItem in newFlightList)
                    {
                        flightItem.FlightClass = newFlight.FlightsSearch.FlightClass;
                        flightItem.Currency = cur;
                    }
                }
                else
                {
                    newFlightList = new List<Flights>();
                    List<List<Flights>> mergeList = newFlight.FlightsList.GroupBy(f => new { f.FlightId }).Select(g => g.ToList()).ToList();

                    foreach (List<Flights> itemList in mergeList)
                    {
                        Flights flightItem = itemList[0];
                        List<string> outboundOption = new List<string>();
                        List<string> inboundOption = new List<string>();
                        flightItem.OutboundList = new List<List<FlightsSegment>>();
                        flightItem.InboundList = new List<List<FlightsSegment>>();

                        foreach (Flights item in itemList)
                        {
                            if (!outboundOption.Contains(string.Join(",", item.Outbound.Select(a => a.SagementRef))))
                            {
                                outboundOption.Add(string.Join(",", item.Outbound.Select(a => a.SagementRef)));
                                flightItem.OutboundList.Add(item.Outbound);
                            }
                            if (flight.FlightsSearch.FlightWay == FlightWay.RoundTrip && !inboundOption.Contains(string.Join(",", item.Inbound.Select(a => a.SagementRef))))
                            {
                                inboundOption.Add(string.Join(",", item.Inbound.Select(a => a.SagementRef)));
                                flightItem.InboundList.Add(item.Inbound);
                            }
                            flightItem.FlightClass = newFlight.FlightsSearch.FlightClass;
                            flightItem.Currency = LocaleCurrencyHelper.GetCurrencySymbol(
        LocaleCurrencyHelper.GetSelectedCurrency(HttpContext));
                        }
                        newFlightList.Add(flightItem);
                    }
                }

                newFlight.FlightsList = newFlightList;
                List<Flights> xFlightList = new List<Flights>();
                //xFlightList = newFlightList;
                if (Stops.Length > 0)
                {
                    foreach (string st in Stops)
                    {
                        dynamic stopsData = null;
                        if (string.IsNullOrEmpty(st))
                        {
                            //  newFlight.FlightsList = flight.FlightsList;
                            //  xFlightList = newFlightList;
                            break;
                        }
                        if (flight.FlightsSearch.FlightWay.GetHashCode() == 1)
                        {
                            if (st == "0")
                            {
                                stopsData = nonStop.Where(f => (Convert.ToInt32(f.Outbound.Count) <= Convert.ToInt32(st) + 1)).OrderBy(f => f.TotalCost).ToList();
                            }
                            if (st == "1")
                            {
                                stopsData = oneStop.Where(f => (Convert.ToInt32(f.Outbound.Count) <= Convert.ToInt32(st) + 1)).OrderBy(f => f.TotalCost).ToList();
                            }
                            if (st == "2")
                            {
                                stopsData = twoStop.Where(f => (Convert.ToInt32(f.Outbound.Count) <= Convert.ToInt32(st) + 1)).OrderBy(f => f.TotalCost).ToList();
                            }
                            if (st == "3")
                            {
                                stopsData = threeStop.Where(f => (Convert.ToInt32(f.Outbound.Count) <= Convert.ToInt32(st) + 1)).OrderBy(f => f.TotalCost).ToList();
                            }
                        }
                        else
                        {
                            if (st == "0")
                            {
                                stopsData = nonStop.Where(f => ((Convert.ToInt32(f.Outbound.Count) == Convert.ToInt32(st) + 1) || (Convert.ToInt32(f.Inbound.Count) == Convert.ToInt32(st) + 1)) && ((Convert.ToInt32(f.Outbound.Count) == Convert.ToInt32(st) + 1) && (Convert.ToInt32(f.Inbound.Count) == Convert.ToInt32(st) + 1))).OrderBy(f => f.TotalCost).ToList();
                            }
                            if (st == "1")
                            {
                                stopsData = oneStop.Where(f => ((Convert.ToInt32(f.Outbound.Count) == Convert.ToInt32(st) + 1) || (Convert.ToInt32(f.Inbound.Count) == Convert.ToInt32(st) + 1)) && ((Convert.ToInt32(f.Outbound.Count) <= Convert.ToInt32(st) + 1) && (Convert.ToInt32(f.Inbound.Count) <= Convert.ToInt32(st) + 1))).OrderBy(f => f.TotalCost).ToList();

                                // stopsData = oneStop.Where(f => (Convert.ToInt32(f.Outbound.Count) == Convert.ToInt32(st) + 1) || (Convert.ToInt32(f.Inbound.Count) == Convert.ToInt32(st) + 1)).OrderBy(f => f.TotalCost).ToList();
                            }
                            if (st == "2")
                            {
                                stopsData = twoStop.Where(f => ((Convert.ToInt32(f.Outbound.Count) == Convert.ToInt32(st) + 1) || (Convert.ToInt32(f.Inbound.Count) == Convert.ToInt32(st) + 1)) && ((Convert.ToInt32(f.Outbound.Count) <= Convert.ToInt32(st) + 1) && (Convert.ToInt32(f.Inbound.Count) <= Convert.ToInt32(st) + 1))).OrderBy(f => f.TotalCost).ToList();

                                //stopsData = twoStop.Where(f => (Convert.ToInt32(f.Outbound.Count) == Convert.ToInt32(st) + 1) || (Convert.ToInt32(f.Inbound.Count) == Convert.ToInt32(st) + 1)).OrderBy(f => f.TotalCost).ToList();
                            }
                            if (st == "3")
                            {
                                stopsData = threeStop.Where(f => ((Convert.ToInt32(f.Outbound.Count) == Convert.ToInt32(st) + 1) || (Convert.ToInt32(f.Inbound.Count) == Convert.ToInt32(st) + 1)) && ((Convert.ToInt32(f.Outbound.Count) <= Convert.ToInt32(st) + 1) && (Convert.ToInt32(f.Inbound.Count) <= Convert.ToInt32(st) + 1))).OrderBy(f => f.TotalCost).ToList();

                                // stopsData = threeStop.Where(f => (Convert.ToInt32(f.Outbound.Count) == Convert.ToInt32(st) + 1) || (Convert.ToInt32(f.Inbound.Count) == Convert.ToInt32(st) + 1)).OrderBy(f => f.TotalCost).ToList();
                            }
                        }
                        //   stopsData = newFlightList.Where(f => Convert.ToInt32(f.Outbound.Count) == Convert.ToInt32(st) + 1).OrderBy(f => f.TotalCost).ToList();
                        foreach (var items in stopsData)
                        {
                            xFlightList.Add(items);

                        }
                        // newFlightList.Where(f => f.Stops == 0).ToList()
                    }

                    if (xFlightList.Count > 0)
                    {
                        newFlight.FlightsList = xFlightList;
                        lbool = true;
                    }


                }


                List<Flights> xFlightList1 = new List<Flights>();
                if (Airlines.Length > 0)
                {
                    foreach (string air in Airlines)
                    {
                        if (string.IsNullOrEmpty(air))
                        {
                            //newFlight.FlightsList = newFlight.FlightsList;
                            //  xFlightList1 = xFlightList;
                            break;
                        }
                        var airdata = newFlight.FlightsList.Where(f => f.Outbound[0].Airline.Code.ToUpper().Trim() == air.ToUpper().Trim()).OrderBy(f => f.TotalCost).ToList();

                        foreach (var items in airdata)
                        {
                            xFlightList1.Add(items);

                        }

                    }
                    if (xFlightList1.Count > 0)
                    {
                        newFlight.FlightsList = xFlightList1;
                        lbool = true;
                    }

                }


                //   if(SorByAirlineName.ToLower()=="price")
                if (SortResult.ToLower() == "price")
                {
                    if (string.IsNullOrWhiteSpace(SortPrice) || SortPrice == "0" || SortPrice == "undefined")
                    {
                        decimal minPrice = Convert.ToDecimal(MinPrice);
                        decimal maxPrice = Convert.ToDecimal(MaxPrice);

                        newFlight.FlightsList = newFlight.FlightsList
                            .Where(f => Convert.ToDecimal(f.TotalCost) >= minPrice)
                            .Where(f => Convert.ToDecimal(f.TotalCost) <= maxPrice)
                            .OrderBy(f => f.TotalCost)
                            .ToList();
                    }
                    else
                    {
                        newFlight.FlightsList = newFlight.FlightsList.Where(cc => cc.TotalCost >= Convert.ToDouble(MinPrice)).Where(pp => pp.TotalCost <= Convert.ToDouble(MaxPrice)).OrderByDescending(f => f.TotalCost).ToList();
                    }

                }
                if (SortResult.ToLower() == "durarion")
                {
                    if (SortDuration == "0")
                    {
                        newFlight.FlightsList = newFlight.FlightsList.Where(cc => cc.TotalCost >= Convert.ToDouble(MinPrice)).Where(pp => pp.TotalCost <= Convert.ToDouble(MaxPrice)).OrderBy(f => f.TotalTime).ToList();
                    }
                    else
                    {
                        newFlight.FlightsList = newFlight.FlightsList.Where(cc => cc.TotalCost >= Convert.ToDouble(MinPrice)).Where(pp => pp.TotalCost <= Convert.ToDouble(MaxPrice)).OrderByDescending(f => f.TotalTime).ToList();
                    }
                }
                if (SortResult.ToLower() == "airline")
                {
                    if (SorByAirlineName == "0")
                    {
                        newFlight.FlightsList = newFlight.FlightsList.Where(cc => cc.TotalCost >= Convert.ToDouble(MinPrice)).Where(pp => pp.TotalCost <= Convert.ToDouble(MaxPrice)).OrderBy(f => f.Outbound[0].Airline.Code).ToList();
                    }
                    else
                    {
                        newFlight.FlightsList = newFlight.FlightsList.Where(cc => cc.TotalCost >= Convert.ToDouble(MinPrice)).Where(pp => pp.TotalCost <= Convert.ToDouble(MaxPrice)).OrderByDescending(f => f.Outbound[0].Airline.Code).ToList();
                    }

                }



                if (newFlight.FlightsList.Count > 0)
                {
                    lbool = true;
                }
                //newFlight.FlightsList = newFlight.FlightsList.OrderBy(f => f.TotalCost).ToList();

                if (!lbool)
                {
                    return newFlight = null;
                }
                if (newFlight != null)
                {

                }
                return newFlight;

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        /// <summary>
        /// Lightweight filter for airline/stop/price clicks — no GroupBy merge or sidebar rebuild.
        /// </summary>
        private static FlightsResult FilterFlightsFast(
            FlightsResult source,
            string[] Stops,
            string[] Airlines,
            string MinPrice,
            string MaxPrice,
            string SortPrice,
            string SortDuration,
            string SorByAirlineName,
            string SortResult)
        {
            if (source?.FlightsList == null || source.FlightsList.Count == 0)
                return null;

            IEnumerable<Flights> query = source.FlightsList;

            var airlineSet = new HashSet<string>(
                (Airlines ?? Array.Empty<string>())
                    .Where(a => !string.IsNullOrWhiteSpace(a))
                    .Select(a => a.Trim()),
                StringComparer.OrdinalIgnoreCase);

            if (airlineSet.Count > 0)
            {
                query = query.Where(f =>
                    f.Outbound != null && f.Outbound.Count > 0
                    && f.Outbound[0].Airline != null
                    && airlineSet.Contains(f.Outbound[0].Airline.Code ?? ""));
            }

            var stopSet = new HashSet<string>(
                (Stops ?? Array.Empty<string>())
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .Select(s => s.Trim()),
                StringComparer.OrdinalIgnoreCase);

            if (stopSet.Count > 0)
            {
                bool isOneWay = source.FlightsSearch != null
                    && source.FlightsSearch.FlightWay.GetHashCode() == 1;

                query = query.Where(f =>
                {
                    int outStops = Math.Max(0, (f.Outbound?.Count ?? 1) - 1);
                    int inStops = Math.Max(0, (f.Inbound?.Count ?? 1) - 1);
                    if (isOneWay)
                        return stopSet.Contains(outStops.ToString());
                    return stopSet.Contains(outStops.ToString()) || stopSet.Contains(inStops.ToString());
                });
            }

            double.TryParse(MinPrice, NumberStyles.Any, CultureInfo.InvariantCulture, out var minP);
            double.TryParse(MaxPrice, NumberStyles.Any, CultureInfo.InvariantCulture, out var maxP);
            if (maxP <= 0) maxP = double.MaxValue;

            query = query.Where(f => f.TotalCost >= minP && f.TotalCost <= maxP);

            var sort = (SortResult ?? "price").ToLowerInvariant();
            if (sort == "durarion" || sort == "duration")
            {
                query = (SortDuration == "0" || string.IsNullOrWhiteSpace(SortDuration))
                    ? query.OrderBy(f => f.TotalTime)
                    : query.OrderByDescending(f => f.TotalTime);
            }
            else if (sort == "airline")
            {
                query = (SorByAirlineName == "0" || string.IsNullOrWhiteSpace(SorByAirlineName))
                    ? query.OrderBy(f => f.Outbound[0].Airline.Code)
                    : query.OrderByDescending(f => f.Outbound[0].Airline.Code);
            }
            else
            {
                query = (string.IsNullOrWhiteSpace(SortPrice) || SortPrice == "0" || SortPrice == "undefined")
                    ? query.OrderBy(f => f.TotalCost)
                    : query.OrderByDescending(f => f.TotalCost);
            }

            var list = query.ToList();
            if (list.Count == 0)
                return null;

            return new FlightsResult
            {
                SearchId = source.SearchId,
                FlightsSearch = source.FlightsSearch,
                FlightsList = list,
                ListAirline = source.ListAirline,
                StopExtra = source.StopExtra,
                BarExtra = source.BarExtra,
                MinPrice = source.MinPrice,
                MaxPrice = source.MaxPrice,
                OutBoundDep = source.OutBoundDep,
                OutBoundArr = source.OutBoundArr,
                InBoundDep = source.InBoundDep,
                InBoundArr = source.InBoundArr,
                Source = source.Source
            };
        }

        // flights/itinerary/?SourceMedia=JetCost&From=EDO&To=SAW&DepDate=2020-12-19&RetDate=2021-01-02&Adult=1&Child=0&Infant=0&FlightWay=RoundTrip&FlightClass=Economy&Airline=&IsDirect=false&IsFlexi=false&Currency=USD&SearchId=3742b225-c0ac-4661-a3c8-0278df254865&TranId=02973277-2c4c-4d50-ae65-9bd9c7dd44cf&P=kiwi
        [Route("{folder}/itinerary")]
        public IActionResult itinerary(string qs, string flightid, string provider)
        {
            try
            {
                string[] QParam = qs.Split('|');

                FlightsSearch flightSearch = new FlightsSearch();
                if (QParam.Length > 1)
                {
                    string[] QSStrArr = QParam;
                    flightSearch.SearchId = Guid.NewGuid().ToString();
                    flightSearch.FlightWay = (FlightWay)Convert.ToByte(QSStrArr[9]);
                    flightSearch.From = Common.GetAirport(QSStrArr[2].ToString().ToString());
                    flightSearch.To = Common.GetAirport(QSStrArr[3]);
                    flightSearch.DepDate = Common.ConvertDateTime(QSStrArr[4]);
                    if (flightSearch.FlightWay == FlightWay.RoundTrip)
                        flightSearch.RetDate = Common.ConvertDateTime(QSStrArr[5]);
                    flightSearch.Adult = Convert.ToInt32(QSStrArr[6]);
                    flightSearch.Child = Convert.ToInt32(QSStrArr[7]);
                    flightSearch.Infant = Convert.ToInt32(QSStrArr[8]);
                    flightSearch.FlightClass = (FlightClass)Convert.ToByte(QSStrArr[10]);
                    //flightSearch.Airline = Common.GetAirline(QSArr[11]);s
                    flightSearch.IsDirect = Convert.ToBoolean(QSStrArr[11]);
                    flightSearch.IsFlexi = true;
                    // flightSearch.Currency = QSStrArr[12];
                    flightSearch.SiteCode = QSStrArr[0];
                    flightSearch.SourceMedia = QSStrArr[1];
                    flightSearch.IsDeepLink = false;
                    Common.SourceMedia = QSStrArr[1];
                }

                //FlightsResult flightsResult = new FlightsResult();

                // FlightsResult flightsResult = Common.SearchFlight(flightSearch);
                FlightsResult flightsResult = new FlightsResult();
                DataTable dtFlight = SqlData.Data.SqlHelperData.ExecuteDataset(Common.ConnectionString, CommandType.Text, "SELECT * FROM MetaSearch Where FlightID='" + flightid + "'").Tables[0];
                List<Flights> lstFlight = new List<Flights>();
                Flights obj = new Flights();

                Flights FlightData = (Flights)XMLToObject(dtFlight.Rows[0]["FlightData"].ToString(), obj);

                lstFlight.Add(FlightData);
                flightsResult.FlightsSearch = flightSearch;
                flightsResult.FlightsList = lstFlight;
                HttpContext.Session.SetObjectAsJson("FlightsResultDetail", flightsResult);
                var value = HttpContext.Session.GetObjectFromJson<FlightsResult>("flightsResult") as FlightsResult;
                var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(value);
                // _logger.LogInformation("FlightResult--775" + jsonString);
                // return new RedirectResult("http://www.website.com");
                return new RedirectResult(Common.WebSitePath + "details?qs=" + qs + "&flightId=" + flightid);


            }
            catch (Exception e)
            {
                return Redirect("/");
            }
        }

        public JsonResult GetAirport(string pickUpLocation)
        {
            List<Airport> airportList = Common.GetAllAirport(pickUpLocation);

            return Json(airportList);
        }

        public Object XMLToObject(string XMLString, Object oObject)
        {

            XmlSerializer oXmlSerializer = new XmlSerializer(oObject.GetType());
            oObject = oXmlSerializer.Deserialize(new StringReader(XMLString));
            return oObject;
        }
        public string ToXML(Object oObject)
        {
            XmlDocument xmlDoc = new XmlDocument();
            XmlSerializer xmlSerializer = new XmlSerializer(oObject.GetType());
            using (MemoryStream xmlStream = new MemoryStream())
            {
                xmlSerializer.Serialize(xmlStream, oObject);
                xmlStream.Position = 0;
                xmlDoc.Load(xmlStream);
                return xmlDoc.InnerXml;
            }
        }


        [Route("DeleteRecord")]
        public void RecordDeleted()
        {
            DatabaseContext.FlightshallContext _DbContext = new DatabaseContext.FlightshallContext();
            var TodayDate = DateTime.Now;

            TodayDate = TodayDate.AddDays(-1);
            TodayDate = TodayDate.Date;
            try
            {
                var search = _DbContext.FlightsSearchNews.Where(a => a.DateCreated == TodayDate).ToList();
                if (search.Count > 0)
                {
                    foreach (var item in _DbContext.FlightsSearchNews)
                    {
                        _DbContext.FlightsSearchNews.Remove(item);
                    }
                    _DbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }


        }

        [Route("flights/{route}")]
        public IActionResult FlightRoute(string route)
        {
            try
            {
                if (string.IsNullOrEmpty(route))
                    return RedirectToAction("Index", "Home");

                // Example route: nyc-to-dxb
                var parts = route.Split("-to-");

                if (parts.Length != 2)
                    return RedirectToAction("Index", "Home");

                string depart = parts[0].ToUpper();   // NYC
                string arrival = parts[1].ToUpper();  // DXB



                ViewBag.Depart = depart;
                ViewBag.Arrival = arrival;

                return View("Result"); // Same Result.cshtml 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in FlightRoute");
                return RedirectToAction("Index", "Home");
            }
        }


        [HttpPost]
        public IActionResult ResultPage(FlightSearchModel model)
        {
            try
            {
                if (model == null)
                    return RedirectToAction("Index", "Home");

                TempData["FlightSearchModel"] = JsonConvert.SerializeObject(model);
                // var result = FlightService.SearchFlights(model);

                return RedirectToAction("result");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Result error");
                return RedirectToAction("Index", "Home");
            }
        }


        private static string GetClientIp(HttpContext ctx)
        {
            string ip = null;

            // Cloudflare
            ip = ctx.Request.Headers["CF-Connecting-IP"].FirstOrDefault();

            // Proxy / load balancer / IIS ARR (first IP is the real client)
            if (string.IsNullOrWhiteSpace(ip))
            {
                var xff = ctx.Request.Headers["X-Forwarded-For"].FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(xff))
                    ip = xff.Split(',')[0].Trim();
            }

            if (string.IsNullOrWhiteSpace(ip))
                ip = ctx.Request.Headers["X-Real-IP"].FirstOrDefault();

            // Direct connection (localhost or server without proxy)
            if (string.IsNullOrWhiteSpace(ip) || !IPAddress.TryParse(ip, out _))
            {
                var remote = ctx.Connection.RemoteIpAddress;
                if (remote != null)
                {
                    if (remote.IsIPv4MappedToIPv6) remote = remote.MapToIPv4();
                    ip = IPAddress.IsLoopback(remote) ? "127.0.0.1" : remote.ToString();
                }
            }

            return string.IsNullOrWhiteSpace(ip) ? "unknown" : ip;
        }

        private static string GetDeviceType(string userAgent)
        {
            if (string.IsNullOrWhiteSpace(userAgent)) return "Unknown";
            var ua = userAgent.ToLowerInvariant();

            if (ua.Contains("ipad") || ua.Contains("tablet") ||
                (ua.Contains("android") && !ua.Contains("mobile")))
                return "Tablet";

            if (ua.Contains("mobi") || ua.Contains("iphone") || ua.Contains("android"))
                return "Mobile";

            return "Desktop";
        }

    }

}