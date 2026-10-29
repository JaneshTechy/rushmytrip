using FlightServices.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using TravelWeb.Admin.Models;
using TravelWeb.Models;
using TravelWeb.Utilities;

namespace TravelWeb.Controllers
{
    public class MulticityController : Controller
    {
        public readonly ILogger<MulticityController> _logger;
        public MulticityController(ILogger<MulticityController> logger)
        {
            _logger = logger;
            Common.GetAirportDataX = Common.GetAirportData();
            Common.GetAirlineDataX = Common.GetAirlineData();
        }
        public IActionResult Index()
        {
            return View();
        }

        [Route("Multicity")]
        [Route("{folder}/Multicity")]
        //public IActionResult Multicity( /*string trip, string adult, string child, string infant, string returnOn, string seniors, string utm_campaign = "", string flightshall = "", string af = ""*/)
        //{
        //    try
        //    {

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
        //        string Flight1from = HttpContext.Request.Query["Flight1from"].ToString();
        //        string flight_to1 = HttpContext.Request.Query["flight_to1"].ToString();
        //        string Flight2from = HttpContext.Request.Query["Flight2from"].ToString();
        //        string flight_to2 = HttpContext.Request.Query["flight_to2"].ToString();

        //        string Flight3from = HttpContext.Request.Query["Flight3from"].ToString();
        //        string flight_to3 = HttpContext.Request.Query["flight_to3"].ToString();

        //        string Flight4from = HttpContext.Request.Query["Flight4from"].ToString();
        //        string flight_to4 = HttpContext.Request.Query["flight_to4"].ToString();


        //        string fromDate1 = HttpContext.Request.Query["fromDate1"].ToString();
        //        string fromDate2 = HttpContext.Request.Query["fromDate2"].ToString();
        //        string fromDate3 = HttpContext.Request.Query["fromDate3"].ToString();
        //        string fromDate4 = HttpContext.Request.Query["fromDate4"].ToString();

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
        //        if (trip == "round")
        //        {
        //            trips = "2";
        //        }
        //        else
        //        {
        //            trips = "1";
        //        }
        //        if (cabin == "Y")
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
        //        List<MultiselectFlight> multiselectList = new List<MultiselectFlight>();

        //        if (depart != "")
        //        {
        //            MultiselectFlight multiselect = new MultiselectFlight();


        //            multiselect.From = Common.GetAirport(depart.ToString().ToString());
        //            multiselect.To = Common.GetAirport(arrival);
        //            multiselect.DepDate = Common.ConvertDateTime(departOn);
        //            multiselectList.Add(multiselect);
        //        }
        //        if (Flight1from != "")
        //        {
        //            MultiselectFlight multiselect = new MultiselectFlight();

        //            multiselect.From = Common.GetAirport(Flight1from.ToString().ToString());
        //            multiselect.To = Common.GetAirport(flight_to1);
        //            multiselect.DepDate = Common.ConvertDateTime(fromDate1);
        //            multiselectList.Add(multiselect);
        //        }
        //        if (Flight2from != "")
        //        {
        //            MultiselectFlight multiselect = new MultiselectFlight();

        //            multiselect.From = Common.GetAirport(Flight2from.ToString().ToString());
        //            multiselect.To = Common.GetAirport(flight_to2);
        //            multiselect.DepDate = Common.ConvertDateTime(fromDate2);
        //            multiselectList.Add(multiselect);
        //        }
        //        if (Flight3from != "")
        //        {
        //            MultiselectFlight multiselect = new MultiselectFlight();

        //            multiselect.From = Common.GetAirport(Flight3from.ToString().ToString());
        //            multiselect.To = Common.GetAirport(flight_to3);
        //            multiselect.DepDate = Common.ConvertDateTime(fromDate3);
        //            multiselectList.Add(multiselect);
        //        }
        //        if (Flight4from != "")
        //        {
        //            MultiselectFlight multiselect = new MultiselectFlight();

        //            multiselect.From = Common.GetAirport(Flight4from.ToString().ToString());
        //            multiselect.To = Common.GetAirport(flight_to3);
        //            multiselect.DepDate = Common.ConvertDateTime(fromDate4);
        //            multiselectList.Add(multiselect);
        //        }

        //        string json = JsonConvert.SerializeObject(multiselectList);
        //        ViewBag.multiselect = json;
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
        //        ViewBag.searchID = searchID;
        //        flightSearch.SearchId = Guid.NewGuid().ToString();
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
        public IActionResult Multicity( /*string trip, string adult, string child, string infant, string returnOn, string seniors, string utm_campaign = "", string flightshall = "", string af = ""*/)
        {
            try
            {

                string depart = HttpContext.Request.Query["depart"].ToString();
                string arrival = HttpContext.Request.Query["arrival"].ToString();
                string departOn = HttpContext.Request.Query["departOn[]"].ToString();
                string trip = HttpContext.Request.Query["trip"].ToString();

                string adult = HttpContext.Request.Query["adult"].ToString();

                string Moreadult = HttpContext.Request.Query["adult"].ToString();
                string MoreChild = HttpContext.Request.Query["child"].ToString();

                string child = HttpContext.Request.Query["child"].ToString();

                string infant = HttpContext.Request.Query["infant"].ToString();

                string returnOn = HttpContext.Request.Query["returnOn"].ToString();

                string seniors = HttpContext.Request.Query["seniors"].ToString();

                string utm_campaign = HttpContext.Request.Query["child"].ToString();

                string flightshall = HttpContext.Request.Query["infant"].ToString();

                string af = HttpContext.Request.Query["returnOn"].ToString();
                string cabin = HttpContext.Request.Query["cabin"].ToString();
                string utm_source = HttpContext.Request.Query["utm_source"].ToString();

                string utm_medium = HttpContext.Request.Query["utm_medium"].ToString();

                string searchID = HttpContext.Request.Query["searchID"].ToString();
                string Flight1from = HttpContext.Request.Query["Flight1from"].ToString();
                string flight_to1 = HttpContext.Request.Query["flight_to1"].ToString();
                string Flight2from = HttpContext.Request.Query["Flight2from"].ToString();
                string flight_to2 = HttpContext.Request.Query["flight_to2"].ToString();

                string Flight3from = HttpContext.Request.Query["Flight3from"].ToString();
                string flight_to3 = HttpContext.Request.Query["flight_to3"].ToString();

                string Flight4from = HttpContext.Request.Query["Flight4from"].ToString();
                string flight_to4 = HttpContext.Request.Query["flight_to4"].ToString();


                string fromDate1 = HttpContext.Request.Query["fromDate1"].ToString();
                string fromDate2 = HttpContext.Request.Query["fromDate2"].ToString();
                string fromDate3 = HttpContext.Request.Query["fromDate3"].ToString();
                string fromDate4 = HttpContext.Request.Query["fromDate4"].ToString();

                _logger.LogInformation(depart.ToString());
                if (HttpContext.Session.GetObjectFromJson<string>("CommonData") == null)
                {
                    Common.GetAirportDataX = Common.GetAirportData();
                    Common.GetAirlineDataX = Common.GetAirlineData();
                    HttpContext.Session.SetObjectAsJson("CommonData", "TRUE");
                }
                bool IsLocal = Common.IsLocalIpAddress("localhost");
                MultiFlightsSearch flightSearch = new MultiFlightsSearch();
                string folderName = "";
                string URLStr = Request.Path;
                if (URLStr.Split('/').Length == 3)
                {
                    folderName = URLStr.Split('/')[1];

                }
                Common.FolderName = folderName;

                var trips = "";
                if (trip == "round")
                {
                    trips = "2";
                }
                else
                {
                    trips = "1";
                }
                if (cabin == "Y")
                {
                    cabin = "4";
                }
                else if (cabin == "B")
                {
                    cabin = "2";
                }
                else if (cabin == "P")
                {
                    cabin = "3";
                }
                else if (cabin == "F")
                {
                    cabin = "1";
                }
                string utm_sources = "";

                if (utm_source == "kayak")
                {
                    utm_sources = utm_source;
                }

                else
                {
                    utm_sources = "HOME"; 
                }
                List<FlightLeg> multiselectList = new List<FlightLeg>();

                if (depart != "")
                {
                    FlightLeg multiselect = new FlightLeg();


                    multiselect.From = Common.GetAirport(depart.ToString().ToString());
                    multiselect.To = Common.GetAirport(arrival);
                    multiselect.DepDate = Common.ConvertDateTime(departOn);
                    multiselectList.Add(multiselect);
                }
                if (Flight1from != "")
                {
                    FlightLeg multiselect = new FlightLeg();

                    multiselect.From = Common.GetAirport(Flight1from.ToString().ToString());
                    multiselect.To = Common.GetAirport(flight_to1);
                    multiselect.DepDate = Common.ConvertDateTime(fromDate1);
                    multiselectList.Add(multiselect);
                }
                if (Flight2from != "")
                {
                    FlightLeg multiselect = new FlightLeg();

                    multiselect.From = Common.GetAirport(Flight2from.ToString().ToString());
                    multiselect.To = Common.GetAirport(flight_to2);
                    multiselect.DepDate = Common.ConvertDateTime(fromDate2);
                    multiselectList.Add(multiselect);
                }
                if (Flight3from != "")
                {
                    FlightLeg multiselect = new FlightLeg();

                    multiselect.From = Common.GetAirport(Flight3from.ToString().ToString());
                    multiselect.To = Common.GetAirport(flight_to3);
                    multiselect.DepDate = Common.ConvertDateTime(fromDate3);
                    multiselectList.Add(multiselect);
                }
                if (Flight4from != "")
                {
                    FlightLeg multiselect = new FlightLeg();

                    multiselect.From = Common.GetAirport(Flight4from.ToString().ToString());
                    multiselect.To = Common.GetAirport(flight_to3);
                    multiselect.DepDate = Common.ConvertDateTime(fromDate4);
                    multiselectList.Add(multiselect);
                }

                string json = JsonConvert.SerializeObject(multiselectList);
                ViewBag.multiselect = json;
                ViewBag.depart = depart;
                ViewBag.arrival = arrival;
                ViewBag.trip = trips;
                ViewBag.departOn = departOn;
                ViewBag.adult = adult;
                ViewBag.child = child;
                ViewBag.infant = infant;
                ViewBag.returnOn = returnOn;
                ViewBag.MorderAdult = Moreadult;
                ViewBag.MoreChild = MoreChild;
                // ViewBag.Home = "Home";
                ViewBag.IsDirect = false;
                ViewBag.seniors = seniors;
                ViewBag.cabin = cabin;
                ViewBag.searchID = searchID;
                flightSearch.SearchId = Guid.NewGuid().ToString();
                flightSearch.FlightWay = (Models.FlightWay)Convert.ToByte(trips);
                //flightSearch.From = Common.GetAirport(depart.ToString().ToString());
                //flightSearch.To = Common.GetAirport(arrival);
                //if (ViewBag.RoundTrip == null)
                //{
                //    ViewBag.RoundTrip = "1";
                //}
                ViewBag.RoundTrip = trips;
                //flightSearch.DepDate = Common.ConvertDateTime(departOn);
                if (flightSearch.FlightWay == Models.FlightWay.RoundTrip)
                {
                    ViewBag.RoundTrip = "2";
                    //flightSearch.RetDate = Common.ConvertDateTime(returnOn);
                }
                flightSearch.Adult = Convert.ToInt32(adult);
                flightSearch.Child = Convert.ToInt32(child);
                flightSearch.Infant = Convert.ToInt32(infant);
                flightSearch.FlightClass = (Models.FlightClass)Convert.ToByte(cabin);
                //flightSearch.Airline = Common.GetAirline(QSArr[11]);
                flightSearch.IsDirect = Convert.ToBoolean(false);
                flightSearch.IsFlexi = true;
                // flightSearch.Currency = QSStrArr[12];
                flightSearch.SiteCode = utm_sources;
                flightSearch.SourceMedia = utm_sources;
                flightSearch.IsDeepLink = true;
                Common.SourceMedia = utm_sources;
                ViewBag.utm_source = Common.SourceMedia;
                ViewBag.RoundTrip = flightSearch.FlightWay.GetHashCode();

                //ViewBag.DepCode = flightSearch.From.Code.ToUpper();
                //ViewBag.DestCode = flightSearch.To.Code.ToUpper();
                ViewBag.adult = flightSearch.Adult;
                ViewBag.child = flightSearch.Child;
                ViewBag.infant = flightSearch.Infant;
                ViewBag.flightClass = flightSearch.FlightClass;
                //ViewBag.departure = flightSearch.From.Code + "-" + flightSearch.From.CityName + "," + flightSearch.From.CountryName;
                //ViewBag.destination = flightSearch.To.Code + "-" + flightSearch.To.CityName + "," + flightSearch.To.CountryName;
                //ViewBag.DepartDate = flightSearch.DepDate.ToString("yyyy-MM-dd");
                //ViewBag.ReturnDate = flightSearch.RetDate.ToString("yyyy-MM-dd");
                ViewBag.FlyClass = "4";
                ViewBag.FlightClass = flightSearch.FlightClass;
                //ViewBag.ToCity = flightSearch.To.CityName;


                return View();
            }
            catch (Exception ex)
            {
                _logger.LogInformation("something went wrong");
                throw ex;
            }
        }

    }
}
