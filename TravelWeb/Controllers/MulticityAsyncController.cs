using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using FlightServices.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TravelWeb.Helper;
using TravelWeb.Models;
using TravelWeb.Utilities;

namespace TravelWeb.Controllers
{
    /// <summary>
    /// Separate multicity flow: 3 one-way searches in parallel, combined into one card list.
    /// Does not change ResultController / existing OneWay / RoundTrip.
    /// </summary>
    public class MulticityAsyncController : Controller
    {
        private readonly ILogger<MulticityAsyncController> _logger;

        public MulticityAsyncController(ILogger<MulticityAsyncController> logger)
        {
            _logger = logger;
            if (Common.GetAirportDataX == null)
            {
                Common.GetAirportDataX = Common.GetAirportData();
            }
            if (Common.GetAirlineDataX == null)
            {
                Common.GetAirlineDataX = Common.GetAirlineData();
            }
        }

        [HttpGet]
        [Route("multicity-async")]
        [Route("MulticityAsync")]
        public IActionResult Index()
        {
            var currency = LocaleCurrencyHelper.GetSelectedCurrency(HttpContext);
            ViewBag.Currency = currency;
            ViewBag.CurrencySymbol = LocaleCurrencyHelper.GetCurrencySymbol(currency);
            return View("~/Views/MulticityAsync/Result.cshtml");
        }

        [HttpPost]
        [Route("MulticityAsync/Search")]
        public async Task<IActionResult> Search()
        {
            try
            {
                string body;
                using (var reader = new System.IO.StreamReader(Request.Body))
                {
                    body = await reader.ReadToEndAsync();
                }

                var request = Newtonsoft.Json.JsonConvert.DeserializeObject<MulticitySearchRequest>(body);
                if (request == null)
                {
                    return Json(new { success = false, error = "Invalid request." });
                }

                if (string.IsNullOrWhiteSpace(request.Currency) ||
                    string.Equals(request.Currency, "MXN", StringComparison.OrdinalIgnoreCase))
                {
                    request.Currency = "USD";
                }

                if (string.IsNullOrWhiteSpace(request.UtmSource))
                {
                    request.UtmSource = "HOME";
                }

                var combined = await MulticityAsyncSearchHelper.SearchAsync(request);

                if (!string.IsNullOrEmpty(combined.Error) && (combined.Itineraries == null || combined.Itineraries.Count == 0))
                {
                    return Json(new { success = false, error = combined.Error, searchId = combined.SearchId });
                }

                HttpContext.Session.SetObjectAsJson("multicity_async_" + combined.SearchId, combined);

                return Json(new
                {
                    success = true,
                    searchId = combined.SearchId,
                    minPrice = combined.MinPrice,
                    maxPrice = combined.MaxPrice,
                    count = combined.Itineraries?.Count ?? 0,
                    error = combined.Error
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MulticityAsync search failed");
                return Json(new { success = false, error = ex.Message });
            }
        }

        [HttpGet]
        [Route("MulticityAsync/Results")]
        public IActionResult Results(string searchId)
        {
            if (string.IsNullOrWhiteSpace(searchId))
            {
                return Content("<div class='alert alert-warning'>Missing search id.</div>");
            }

            var combined = HttpContext.Session.GetObjectFromJson<MulticityCombinedResult>("multicity_async_" + searchId);
            if (combined == null)
            {
                return Content("<div class='alert alert-warning'>Session expired. Please search again.</div>");
            }

            ViewBag.Currency = LocaleCurrencyHelper.GetSelectedCurrency(HttpContext);
            ViewBag.CurrencySymbol = LocaleCurrencyHelper.GetCurrencySymbol(ViewBag.Currency as string ?? "USD");
            return PartialView("~/Views/Shared/_multicityCombinedResult.cshtml", combined);
        }

        /// <summary>
        /// Same flow as /result Select: store FlightsResult under search_, then go to /details.
        /// </summary>
        [HttpGet]
        [Route("MulticityAsync/Select")]
        public IActionResult Select(string searchId, string combinedId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchId) || string.IsNullOrWhiteSpace(combinedId))
                {
                    return Redirect(Common.WebSitePath);
                }

                var combined = HttpContext.Session.GetObjectFromJson<MulticityCombinedResult>("multicity_async_" + searchId);
                if (combined?.Itineraries == null)
                {
                    return Redirect(Common.WebSitePath + "multicity-async?searchID=" + Uri.EscapeDataString(searchId));
                }

                var itinerary = combined.Itineraries.FirstOrDefault(x => x.CombinedId == combinedId);
                if (itinerary == null)
                {
                    return Redirect(Common.WebSitePath + "multicity-async?searchID=" + Uri.EscapeDataString(searchId));
                }

                var bookingResult = MulticityAsyncSearchHelper.BuildBookingFlightsResult(itinerary, combined.Request);
                if (bookingResult?.FlightsList == null || bookingResult.FlightsList.Count == 0)
                {
                    return Redirect(Common.WebSitePath + "multicity-async?searchID=" + Uri.EscapeDataString(searchId));
                }

                // Same storage DetailsController expects from /result Select
                AplicationHelper.Save(bookingResult.SearchId, "search_", bookingResult);
                Common.SaveObjectSession("search_" + bookingResult.SearchId, bookingResult);
                HttpContext.Session.SetObjectAsJson("search_" + bookingResult.SearchId, bookingResult);

                var fs = bookingResult.FlightsSearch;
                var flight = bookingResult.FlightsList[0];
                var depart = fs.From?.Code ?? "";
                var arrival = fs.To?.Code ?? "";
                var departOn = fs.DepDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                var returnOn = (fs.RetDate != DateTime.MinValue && fs.RetDate.Year > 1900)
                    ? fs.RetDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
                    : "";
                var cabin = MapCabinQuery(fs.FlightClass);
                var adult = fs.Adult;
                var child = fs.Child;
                var infant = fs.Infant;

                var url =
                    Common.WebSitePath + "details?" +
                    "depart=" + Uri.EscapeDataString(depart) +
                    "&arrival=" + Uri.EscapeDataString(arrival) +
                    "&trip=one" +
                    "&departOn[]=" + Uri.EscapeDataString(departOn) +
                    "&af=" +
                    "&ch=RushMyFlight" +
                    "&adult=" + adult +
                    "&child=" + child +
                    "&infant=" + infant +
                    "&seniors=0" +
                    "&cabin=" + Uri.EscapeDataString(cabin) +
                    "&returnOn=" + Uri.EscapeDataString(returnOn) +
                    "&utm_source=" + Uri.EscapeDataString(combined.Request?.UtmSource ?? "HOME") +
                    "&utm_medium=cpc" +
                    "&utm_campaign=" +
                    "&flightId=" + Uri.EscapeDataString(flight.FlightId) +
                    "&searchID=" + Uri.EscapeDataString(bookingResult.SearchId) +
                    "&moreadultcount=" + adult +
                    "&morechildcount=" + child;

                return Redirect(url);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MulticityAsync Select failed");
                return Redirect(Common.WebSitePath);
            }
        }

        private static string MapCabinQuery(FlightClass flightClass)
        {
            switch (flightClass)
            {
                case FlightClass.First: return "F";
                case FlightClass.Business: return "B";
                case FlightClass.EconomyPremium: return "P";
                default: return "Y";
            }
        }
    }
}
