using AuthorizeNet.Api.Contracts.V1;
using AuthorizeNet.Api.Controllers;
using AuthorizeNet.Api.Controllers.Bases;
using DocumentFormat.OpenXml.Spreadsheet;
using FlightServices.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Razor.Templating.Core;
using RazorLight;
using SendGrid;
using SendGrid.Helpers.Mail;
using ServiceStack;
using SqlData.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Net.Security;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using TravelWeb.Admin.Models;
using TravelWeb.Areas.Admin.Models;
using TravelWeb.DatabaseContext;
using TravelWeb.Datamodel;
using TravelWeb.Helper;
using TravelWeb.Models;
using TravelWeb.Services;
using TravelWeb.Utilities;

using Flights = TravelWeb.Models.Flights;
using FlightsResult = TravelWeb.Models.FlightsResult;
using FlightsSearch = TravelWeb.Models.FlightsSearch;
using FlightsSegment = TravelWeb.Models.FlightsSegment;

namespace TravelWeb.Controllers
{

    public class DetailsController : Controller
    {

        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly DeepLinkTrackerService _deepLinkTrackerService;
        public readonly ILogger<DetailsController> _logger;
        FlightshallContext context = new FlightshallContext();
        private readonly RazorLightEngine _engine;
        private readonly IConfiguration _configuration;
        private readonly PaymentSettings _settings;
        private readonly FlightVerifierService _flightVerifierService;


        public async Task<string> RenderAsync<T>(string viewPath, T model)
        {
            return await _engine.CompileRenderAsync(viewPath, model);
        }
        public DetailsController(ILogger<DetailsController> logger, DeepLinkTrackerService deepLinkTrackerService, IWebHostEnvironment env, IConfiguration configuration, Microsoft.Extensions.Options.IOptions<PaymentSettings> options, FlightVerifierService flightVerifierService /*,ResultController resultcontroller*/)
        {
            //_resultcontroller = resultcontroller;
            _configuration = configuration;
            _logger = logger;
            _settings = options.Value;
            _flightVerifierService = flightVerifierService;

            _deepLinkTrackerService = deepLinkTrackerService;
            // Airports/airlines are warmed at startup — don't re-parse XML on every details request
            if (Common.GetAirportDataX == null)
                Common.GetAirportDataX = Common.GetAirportData();
            if (Common.GetAirlineDataX == null)
                Common.GetAirlineDataX = Common.GetAirlineData();
            _engine = new RazorLightEngineBuilder().UseFileSystemProject(Path.Combine(env.ContentRootPath, "EmailTemplates")).UseMemoryCachingProvider()
                .Build();
        }
        [Microsoft.AspNetCore.Mvc.Route("details")]

        public async Task<IActionResult> Index(string depart, string arrival, string trip, /*string departOn*/ string adult, string child, string infant, string cabin, string flightId,/* string searchID*/int moreadultcount, int morechildcount, string returnOn, string seniors, string utm_campaign = "", string ch = "flightshall", string utm_source = "kayak", string flightshall = "", string utm_medium = "cpc", string af = "", bool acceptLivePrice = false)
        {
            AplicationHelper.Log("Index API Hit");

            ViewBag.ApiLoginId = _configuration["PaymentSettings:AcceptJsApiLoginId"];
            ViewBag.ClientKey = _configuration["PaymentSettings:AcceptJsClientKey"];
            var qs = "";
            //var guidSearchId = new Guid(searchIDs);
            string departOn = HttpContext.Request.Query["departOn[]"].ToString();

            if (departOn == "" || departOn == null)
            {
                departOn = HttpContext.Request.Query["departOn"];
            }
            var currency = Request.Cookies["site_currency"] ?? "USD";
            SetCurrentCulture(currency);
            string search = HttpContext.Request.Query["searchID"].ToString();
            string searchID = search.Substring(search.LastIndexOf(',') + 1);
            ViewBag.SearchId = searchID;
            //coment by Sunit casue of session timeout issue
            //fetch flight detail from session
            //FlightsResult newFlightsResult = HttpContext.Session.GeObjectFromJson<FlightsResult>("FlightsResultDetail");
            //End


            //fetch current flight detail from DB

            //DataTable dtFlight = SqlData.Data.SqlHelperData.ExecuteDataset(Common.ConnectionString, CommandType.Text, "SELECT * FROM FlightsSearch_New Where SearchId='" + searchID + "'").Tables[0];
            //List<FlightsResult> lstFlight = new List<FlightsResult>();
            //FlightsResult newFlightsResult = new  FlightsResult();
            //FlightsResult obj = new FlightsResult();
            //if (dtFlight.Rows.Count !=0) {
            //    var XMLString = dtFlight.Rows[0].ItemArray[1].ToString();
            //     newFlightsResult = JsonConvert.DeserializeObject<FlightsResult>(XMLString);
            //}
            //var value = Common.GetSession("search_" + searchID);
            //FlightsResult value = null;
            //string json = AplicationHelper.Get(searchID);
            //if (!string.IsNullOrEmpty(json))
            //{
            //    value = JsonConvert.DeserializeObject<FlightsResult>(json);
            //}
            FlightsResult value = null;
            FlightsResult newFlightsResult = null;
            Flights fls = null;
            FlightsSearch flsearch = null;

            // Prefer slim details context (one flight) prepared on Select — avoids loading multi‑MB search_.txt
            var detailsCtx = FlightVerifierService.LoadDetailsContext(searchID, flightId);
            if (detailsCtx?.Flight != null)
            {
                fls = detailsCtx.Flight;
                flsearch = detailsCtx.FlightsSearch;
                newFlightsResult = new FlightsResult
                {
                    FlightsList = new List<Flights> { fls },
                    FlightsSearch = flsearch
                };
            }
            else
            {
                value = AplicationHelper.Get<FlightsResult>(searchID, "search_");
                newFlightsResult = value;
                if (newFlightsResult == null)
                {
                    return Redirect(Common.WebSitePath + "Result?depart=" + depart + "&arrival=" + arrival + "&trip=" + trip + "&departOn[]=" + departOn + "&af=" + af + "&ch=" + ch + "&adult=" + adult + "&child=" + child + "&infant=" + infant + "&seniors=" + seniors + "&cabin=" + cabin + "&searchID=" + searchID + "&returnOn=" + returnOn + "&utm_source=" + utm_source + "&utm_medium=" + utm_medium + "&utm_campaign=" + utm_campaign + "" + searchID);
                }
                fls = newFlightsResult.FlightsList?.Where(x => x.FlightId == flightId).FirstOrDefault();
                flsearch = newFlightsResult.FlightsSearch;
                if (fls != null)
                    FlightVerifierService.SaveDetailsContext(searchID, fls, flsearch);
            }

            //var value = Common.GetObjectSession<FlightsResult>("search_" + searchID);

            //var value = AplicationHelper.Get("localhost", searchID);
            if (newFlightsResult != null) ;
            else
            {
                return Redirect(Common.WebSitePath + "Result?depart=" + depart + "&arrival=" + arrival + "&trip=" + trip + "&departOn[]=" + departOn + "&af=" + af + "&ch=" + ch + "&adult=" + adult + "&child=" + child + "&infant=" + infant + "&seniors=" + seniors + "&cabin=" + cabin + "&searchID=" + searchID + "&returnOn=" + returnOn + "&utm_source=" + utm_source + "&utm_medium=" + utm_medium + "&utm_campaign=" + utm_campaign + "" + searchID);

            }

            //End

            try
            {
                //string backurl = (Common.WebSitePath + "Result?depart=" + depart + "&arrival=" + arrival + "&trip=" + trip + "&departOn[]=" + departOn + "&af=" + af + "&ch=" + ch + "&adult=" + adult + "&child=" + child + "&infant=" + infant + "&seniors=" + seniors + "&cabin=" + cabin + "&returnOn=" + returnOn + "&utm_source=" + utm_source + "&utm_medium=" + utm_medium + "&utm_campaign=" + utm_campaign + "");

                string backurl = (Common.WebSitePath + "Result?depart=" + depart + "&arrival=" + arrival + "&trip=" + trip + "&departOn[]=" + departOn + "&af=" + af + "&ch=" + ch + "&adult=" + adult + "&child=" + child + "&infant=" + infant + "&seniors=" + seniors + "&cabin=" + cabin + "&searchID=" + searchID + "&returnOn=" + returnOn + "&utm_source=" + utm_source + "&utm_medium=" + utm_medium + "&utm_campaign=" + utm_campaign + "" + searchID);

                //code coment by sunit
                //redirect to result page due to session timeout
                /*if (newFlightsResult == null)
                {
                    return Redirect("/result?depart=" + depart + "&arrival=" + arrival + "&trip=" + trip + "&departOn[]=" + departOn + "&af=" + af + "&ch=" + ch + "&adult=" + adult + "&child=" + child + "&infant=" + infant + "&seniors=" + seniors + "&cabin=" + cabin + "&returnOn=" + returnOn + "&utm_source=" + utm_source + "&utm_medium=" + utm_medium + "&utm_campaign=" + utm_campaign + "");
                }*/
                //end


                // Flights fls already resolved above
                ViewBag.BackToResult = backurl;
                // FlightsSearch flsearch already resolved above
                if (fls != null && string.IsNullOrWhiteSpace(fls.SearchId))
                    fls.SearchId = searchID;

                //HttpContext.Session.SetObjectAsJson("Flights", fls);
                //HttpContext.Session.SetObjectAsJson("FlightsSearch", flsearch);

                if (fls == null || fls.Outbound == null || !fls.Outbound.Any())
                {
                    return Redirect(backurl);
                }

                bool isMulticity = fls.OutboundList != null
                    && fls.OutboundList.Count > 1
                    && (fls.Inbound == null || fls.Inbound.Count == 0)
                    && fls.OutboundList.SelectMany(x => x ?? new List<FlightsSegment>()).Count() == fls.Outbound.Count;

                ViewBag.IsMulticity = isMulticity;
                ViewBag.FlightWay = isMulticity ? "MultiCity" : flsearch.FlightWay.ToString();
                ViewBag.FlightClass = flsearch.FlightClass;

                ViewBag.OutboundClass = fls.Outbound[0].ClassOfService;

                if (fls.Inbound != null && fls.Inbound.Count > 0)
                {
                    if (fls.Inbound[0] != null)
                    {
                        ViewBag.InboundClass = fls.Inbound[0].ClassOfService;
                    }
                    ViewBag.InboundClass = fls.Inbound[0].ClassOfService;
                }

                ViewBag.MarkupType = fls.FlightFare.MarkupType;
                ViewBag.MarkupValue = fls.FlightFare.MarkupValue;
                //comment by janesh -301012023;
                if (moreadultcount != 0 && moreadultcount != null)
                {
                    flsearch.Adult = /*flsearch.Adult +*/ moreadultcount;
                    fls.FlightFare.Adult = flsearch.Adult;
                }
                if (morechildcount != null)
                {
                    flsearch.Child = morechildcount;
                    fls.FlightFare.Child = flsearch.Child;
                }
                ViewBag.Adult = flsearch.Adult;
                ViewBag.Child = flsearch.Child;
                ViewBag.Infant = flsearch.Infant;
                ViewBag.Airlines = ""; ;
                ViewBag.FromName = Common.GetAirport(flsearch.From.Code).CityName;// flsearch.From.CityName;
                ViewBag.FromCode = flsearch.From.Code;
                ViewBag.ToName = Common.GetAirport(flsearch.To.Code).CityName;// flsearch.To.CityName;
                ViewBag.ToCode = flsearch.To.Code;
                ViewBag.MainCabinAllowed = TravelWeb.Utilities.Common.MainCabinAllowed.ToString();
                //ViewBag.PaxWithoutInfant = TravelWeb.Utilities.Common.PaxWithoutInfant.ToString();
                ViewBag.SeatPrefrenceAmount = 0;//TravelWeb.Utilities.Common.SeatPrefrenceAmount.ToString();
                ViewBag.TotalPax = (Convert.ToInt32(ViewBag.Adult) + Convert.ToInt32(ViewBag.Child) /*+ Convert.ToInt32(ViewBag.Infant)*/);

                //int PaxwithoutInfant = Convert.ToInt32(ViewBag.Adult) + Convert.ToInt32(ViewBag.Child);
                //if (PaxWithoutInfant == "true")
                //{
                //    PaxwithoutInfant += Convert.ToInt32(ViewBag.Infant);
                //}
                ViewBag.TotalMainCabinPrice = 0;// Convert.ToDouble(Convert.ToDouble(ViewBag.SeatPrefrenceAmount) * ViewBag.TotalPax);
                //Comment on Bharat InfantFare subtract because infantamount not add.
                ViewBag.TotalAmountWithPreference = fls.FlightFare.GrandTotal + ViewBag.TotalMainCabinPrice - (fls.FlightFare.InfantFare * fls.FlightFare.Infant);
                //ViewBag.Qs = "result?qs=" + qs;

                ViewBag.Qs = "result?depart = " + depart + " & arrival = " + arrival + " & trip = " + trip + " & departOn[] = " + departOn + " & af = " + af + " & ch = " + ch + " & adult = " + adult + " & child = " + child + " & infant = " + infant + " & seniors = " + seniors + " & cabin = " + cabin + " & returnOn = " + returnOn + " & utm_source = " + utm_source + " & utm_medium = " + utm_medium + " & utm_campaign = " + utm_campaign + "";

                string pax = "";
                if (Convert.ToInt32(flsearch.Adult) < 2)
                {
                    pax += flsearch.Adult + " Adult ";
                }
                else if (Convert.ToInt32(flsearch.Adult) > 1)
                {
                    pax += flsearch.Adult + " Adults ";
                }
                if (Convert.ToInt32(flsearch.Child) < 2 && Convert.ToInt32(flsearch.Child) > 0)
                {
                    pax += flsearch.Child + " Child ";
                }
                else if (Convert.ToInt32(flsearch.Child) > 1)
                {
                    pax += flsearch.Child + " Childs ";
                }
                if (Convert.ToInt32(flsearch.Infant) < 2 && Convert.ToInt32(flsearch.Infant) > 0)
                {
                    pax += flsearch.Infant + " Infant ";
                }

                else if (Convert.ToInt32(flsearch.Infant) > 1)
                {
                    pax += flsearch.Infant + " Infants ";
                }

                ViewBag.Trip = (isMulticity ? "Multi-city" : flsearch.FlightWay.ToString()) + ", " + flsearch.FlightClass + ", " + pax;

                ViewBag.Depart = flsearch.DepDate;//Convert.ToDateTime(flsearch.DepDate).ToString("MMMM, dd, yyyy");
                ViewBag.Return = flsearch.RetDate;//Convert.ToDateTime(flsearch.RetDate).ToString("MMMM, dd, yyyy");
                var diplinktrackerdata = new DeepLinkTracker();
                //DataTable dtx=    SqlData.Data.SqlHelperData.ExecuteDataset(Common.ConnectionString, CommandType.Text, "SELECT * FROM FlightBooking").Tables[0];

                //DataTable inbound = ToDataTable(fls.Inbound);
                fls.SearchId = searchID;

                var Json = JsonConvert.SerializeObject(fls.FlightFare);
                var fare = fls.FlightFare;

                decimal adultTotal = (decimal)(fare.AdultFare + fare.AdultTax) * fare.Adult;
                decimal childTotal = (decimal)(fare.ChildFare + fare.ChildTax) * fare.Child;
                decimal infantTotal = (decimal)(fare.InfantFare + fare.InfantTax) * fare.Infant;

                decimal priceWithoutMarkup = adultTotal + childTotal + infantTotal;


                decimal totalMarkup = (decimal)((fare.AdultFare + (Double)fare.AdultTax) * fare.Adult +
                                     (fare.ChildFare + (Double)fare.ChildTax) * fare.Child +
                                     (fare.InfantFare + (Double)fare.InfantTax) * fare.Infant);
                totalMarkup = ((decimal)fare.GrandTotal - totalMarkup);

                //Task.Run(async () =>
                //{
                //    try
                //    {
                //        var dip = new DeepLinkTracker
                //        {
                //            Stage = "ContactDetailsPage",
                //            PriceWithoutMarkup = priceWithoutMarkup,
                //            Markup = totalMarkup,
                //            Rule = fls.FlightFare.MarkupRuleName,
                //            Airline = fls.Airline?.Name,
                //            CabinClass = fls.Outbound?[0]?.CabinClass,
                //            SubClass = fls.Outbound?[0]?.ClassOfService,
                //            ModifiedBy = fls.FlightFare.UpdatedBy,
                //            TotalAmount = (decimal)fls.FlightFare.GrandTotal,
                //        };

                //        await _deepLinkTrackerService.UpdateTrackerdata(searchID, dip);
                //    }
                //    catch (Exception ex)
                //    {
                //        _logger.LogError(ex, "Error updating tracker stage");
                //    }
                //});

                try
                {
                    var outbound = fls.Outbound?.FirstOrDefault();
                    var inbound = fls.Inbound?.FirstOrDefault();
                    var dip = new DeepLinkTracker
                    {
                        Stage = "ContactDetailsPage",
                        PriceWithoutMarkup = priceWithoutMarkup,
                        Markup = totalMarkup,
                        Rule = fls.FlightFare.MarkupRuleName,
                        Airline = fls.Airline?.Name,
                        CabinClass = outbound?.CabinClass,
                        SubClass = outbound?.ClassOfService,
                        ModifiedBy = fls.FlightFare.UpdatedBy,
                        TotalAmount = (decimal)fls.FlightFare.GrandTotal,

                        InboundClass = inbound?.ClassOfService,
                        OutboundClass = outbound?.ClassOfService,

                        InboundAirline = inbound?.Airline?.Code,
                        OutboundAirline = outbound?.Airline?.Code,

                        ReturnDate = flsearch.RetDate,
                        DepartureDate = flsearch.DepDate
                    };

                    _deepLinkTrackerService.UpdateTrackerdata(searchID, dip);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating tracker stage");
                }
                void NormalizeSegmentDates(IEnumerable<FlightsSegment> segments)
                {
                    if (segments == null) return;
                    foreach (var seg in segments)
                    {
                        if (seg == null) continue;
                        if (string.IsNullOrEmpty(seg.DepDate) && seg.DepartureDate != default)
                            seg.DepDate = seg.DepartureDate.ToString("yyyy-MM-ddTHH:mm:ss");
                        if (string.IsNullOrEmpty(seg.ArrDate) && seg.ArrivalDate != default)
                            seg.ArrDate = seg.ArrivalDate.ToString("yyyy-MM-ddTHH:mm:ss");
                        if (seg.DepartureDate == default && !string.IsNullOrEmpty(seg.DepDate)
                            && DateTime.TryParse(seg.DepDate, out var depParsed))
                            seg.DepartureDate = depParsed;
                        if (seg.ArrivalDate == default && !string.IsNullOrEmpty(seg.ArrDate)
                            && DateTime.TryParse(seg.ArrDate, out var arrParsed))
                            seg.ArrivalDate = arrParsed;
                    }
                }

                NormalizeSegmentDates(fls?.Outbound);
                NormalizeSegmentDates(fls?.Inbound);
                if (fls?.OutboundList != null)
                {
                    foreach (var leg in fls.OutboundList)
                        NormalizeSegmentDates(leg);
                }
                if (fls?.InboundList != null)
                {
                    foreach (var leg in fls.InboundList)
                        NormalizeSegmentDates(leg);
                }
                var trawickService = new TrawickService(_configuration);
                if (utm_source == "Trabber")
                    fls.FlightFare.CampId = "Trabber";

                if (utm_source == "Rush_FS")
                    fls.FlightFare.CampId = "Rush_FS";

                double configuredAtol = 0;


                if (utm_source != "Trabber" && utm_source != "Rush_FS" && utm_source!="FS")
                {
                    double.TryParse(
                        _configuration["AppSettings:AtolCharge"],
                        out configuredAtol
                    );

                    FlightVerifierService.EnsureAtolCharge(fls, configuredAtol);
                }
                if (utm_source != "FS")
                {
                    // Live fare verifier — block page until response; loader shows during Select→details wait
                    var verifierState = new VerifierPageState();
                    try
                    {
                        var offerId = fls.ApiId;
                        if (!string.IsNullOrWhiteSpace(offerId))
                        {
                            if (string.IsNullOrWhiteSpace(fls.ApiId))
                                fls.ApiId = offerId;

                            //double.TryParse(_configuration["AppSettings:AtolCharge"], out var configuredAtol);
                            //FlightVerifierService.EnsureAtolCharge(fls, configuredAtol);

                            var verifier = await _flightVerifierService.VerifyAsync(offerId, searchID, fls.FlightId);

                            ApplyVerifierResult(fls, verifier, searchID, newFlightsResult, configuredAtol,
                                acceptLivePrice, verifierState);
                        }
                        else
                        {
                            verifierState.Error = "Offer id missing for verifier.";
                            AplicationHelper.Log("Verifier skipped: missing offer id for flight " + flightId);
                        }
                    }
                    catch (Exception vex)
                    {
                        verifierState.Error = vex.Message;
                        AplicationHelper.Log("Verifier error: " + vex.Message);
                    }

                    ViewBag.Verifier = verifierState;
                    ViewBag.BookingOptions = verifierState.BookingOptions ?? new List<VerifierBookingOption>();
                    ViewBag.ShowPriceChangePopup = verifierState.ShowPriceChangePopup;
                    ViewBag.OfferedPrice = verifierState.OfferedTotal;
                    ViewBag.LivePrice = verifierState.LiveTotal;
                    ViewBag.PriceDelta = verifierState.Delta;
                    AplicationHelper.Log($"Details bookingOptions count={verifierState.BookingOptions?.Count ?? 0}");
                }
                

                

                //if (fls.FlightFare.CampId != "Rush_FS")
                //{
                //    //string token = trawickService.GetToken();
                //    //ViewBag.Token = token;
                //}

                return View(fls);
            }
            catch (WebException ex)
            {
                // Response.Write("");
                ViewBag.error = ex.Message.ToString();
                AplicationHelper.Log("Main Exception: " + ex.Message);
                return View();

            }

        }

        private void ApplyVerifierResult(
            Flights fls,
            VerifierApiResponse verifier,
            string searchID,
            FlightsResult newFlightsResult,
            double configuredAtol,
            bool acceptLivePrice,
            VerifierPageState verifierState)
        {
            var built = FlightVerifierService.BuildPageState(fls, verifier);
            verifierState.HasVerifier = built.HasVerifier;
            verifierState.Currency = built.Currency;
            verifierState.OfferId = built.OfferId;
            verifierState.BookingOptions = built.BookingOptions;
            verifierState.Available = built.Available;
            verifierState.OfferedTotal = built.OfferedTotal;
            verifierState.LiveTotal = built.LiveTotal;
            verifierState.Delta = built.Delta;
            verifierState.PriceIncreased = built.PriceIncreased;
            verifierState.ShowPriceChangePopup = built.ShowPriceChangePopup;
            verifierState.DisplayTotal = built.DisplayTotal;

            FlightVerifierService.EnsureAtolCharge(fls, configuredAtol);

            var selectedTicket = FlightVerifierService.LoadSelectedTicket(searchID, fls.FlightId);
            var opts = verifierState.BookingOptions ?? new List<VerifierBookingOption>();
            var applyIdx = selectedTicket?.BookingOptionIndex ?? (opts.Count > 0 ? 0 : (int?)null);

            if (applyIdx != null && applyIdx.Value >= 0 && applyIdx.Value < opts.Count)
            {
                var idx = applyIdx.Value;
                var opt = opts[idx];
                if (string.Equals(opt.vendorCode, "LIVE", StringComparison.OrdinalIgnoreCase))
                    FlightVerifierService.ApplyLiveOfferToFlight(fls, verifier);
                else
                    FlightVerifierService.ApplyBookingOptionToFlight(fls, opt);
                FlightVerifierService.EnsureAtolCharge(fls, configuredAtol);
                PersistFlightInSearch(newFlightsResult, fls, searchID);
                verifierState.DisplayTotal = (decimal)FlightVerifierService.GetPayableTotal(fls);
                ViewBag.SelectedTicketIndex = idx;

                FlightVerifierService.SaveSelectedTicket(searchID, fls.FlightId,
                    new FlightVerifierService.SelectedTicketState
                    {
                        BookingOptionIndex = idx,
                        Total = (double)verifierState.DisplayTotal,
                        Currency = fls.Currency ?? "USD",
                        Label = opt.DisplayName
                    });
            }
            else if (acceptLivePrice && verifierState.PriceIncreased)
            {
                FlightVerifierService.ApplyLiveOfferToFlight(fls, verifier);
                FlightVerifierService.EnsureAtolCharge(fls, configuredAtol);
                verifierState.DisplayTotal = (decimal)FlightVerifierService.GetPayableTotal(fls);
                PersistFlightInSearch(newFlightsResult, fls, searchID);
            }

            // Popup is driven only by verifier meta.mismatch (set in BuildPageState)

            if (fls.TotalCost <= 0 && fls.FlightFare != null)
                fls.TotalCost = fls.FlightFare.GrandTotal;

            var cabinPref = 0d;
            try { cabinPref = Convert.ToDouble(ViewBag.TotalMainCabinPrice); } catch { /* ignore */ }

            ViewBag.TotalAmountWithPreference = FlightVerifierService.GetPayableTotal(fls)
                + cabinPref
                - (fls.FlightFare.InfantFare * fls.FlightFare.Infant);
        }

        private static void PersistFlightInSearch(FlightsResult result, Flights updated, string searchId)
        {
            if (updated == null || string.IsNullOrWhiteSpace(searchId))
                return;

            // Slim one-flight context — avoid rewriting multi-MB search_.txt
            FlightVerifierService.SaveDetailsContext(searchId, updated, result?.FlightsSearch);

            if (result?.FlightsList != null && result.FlightsList.Count <= 3)
            {
                var idx = result.FlightsList.FindIndex(f => f.FlightId == updated.FlightId);
                if (idx >= 0)
                    result.FlightsList[idx] = updated;
                else
                    result.FlightsList.Add(updated);
                AplicationHelper.Save(searchId, "search_details_", result);
            }
        }

        [HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("details/prepare")]
        public async Task<IActionResult> PrepareDetails(string searchId, string flightId)
        {
            if (string.IsNullOrWhiteSpace(searchId) || string.IsNullOrWhiteSpace(flightId))
                return Json(new { ok = false, error = "Missing ids." });

            try
            {
                Flights flight = null;
                FlightsSearch search = null;

                var existing = FlightVerifierService.LoadDetailsContext(searchId, flightId);
                if (existing?.Flight != null)
                {
                    flight = existing.Flight;
                    search = existing.FlightsSearch;
                }
                else
                {
                    var result = AplicationHelper.Get<FlightsResult>(searchId, "search_");
                    flight = result?.FlightsList?.FirstOrDefault(f => f.FlightId == flightId);
                    if (flight == null)
                        return Json(new { ok = false, error = "Flight not found." });

                    if (string.IsNullOrWhiteSpace(flight.SearchId))
                        flight.SearchId = searchId;

                    search = result.FlightsSearch;
                    FlightVerifierService.SaveDetailsContext(searchId, flight, search);
                }

                // Run verifier here while result-page loader is visible; details loads with response ready
                var offerId = FlightVerifierService.ResolveOfferId(flight);
                if (!string.IsNullOrWhiteSpace(offerId))
                {
                    if (string.IsNullOrWhiteSpace(flight.ApiId))
                        flight.ApiId = offerId;

                    if (FlightVerifierService.TryLoadFreshVerifier(searchId, flight.FlightId, offerId, TimeSpan.FromMinutes(10)) == null)
                        await _flightVerifierService.VerifyAsync(offerId, searchId, flight.FlightId);
                }

                return Json(new { ok = true, cached = existing?.Flight != null });
            }
            catch (Exception ex)
            {
                AplicationHelper.Log("PrepareDetails error: " + ex.Message);
                return Json(new { ok = false, error = ex.Message });
            }
        }

        [HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("details/accept-live-price")]
        public IActionResult AcceptLivePrice(string searchId, string flightId, int? bookingOptionIndex = null)
        {
            if (string.IsNullOrWhiteSpace(searchId) || string.IsNullOrWhiteSpace(flightId))
                return Json(new { ok = false, error = "Missing search or flight id." });

            var ctx = FlightVerifierService.LoadDetailsContext(searchId, flightId);
            Flights flight = ctx?.Flight;
            FlightsResult result = null;

            if (flight == null)
            {
                result = AplicationHelper.Get<FlightsResult>(searchId, "search_");
                flight = result?.FlightsList?.FirstOrDefault(f => f.FlightId == flightId);
            }
            else
            {
                result = new FlightsResult
                {
                    FlightsList = new List<Flights> { flight },
                    FlightsSearch = ctx?.FlightsSearch
                };
            }

            if (flight == null)
                return Json(new { ok = false, error = "Flight not found." });

            var verifier = FlightVerifierService.LoadVerifierFromStore(searchId, flight.FlightId, flight.ApiId);
            if (verifier == null)
                return Json(new { ok = false, error = "Verifier response not found. Reload the page." });

            double.TryParse(_configuration["AppSettings:AtolCharge"], out var configuredAtol);
            FlightVerifierService.EnsureAtolCharge(flight, configuredAtol);

            var options = FlightVerifierService.NormalizeBookingOptions(verifier);
            string label = null;

            if (bookingOptionIndex.HasValue
                && bookingOptionIndex.Value >= 0
                && bookingOptionIndex.Value < options.Count)
            {
                var opt = options[bookingOptionIndex.Value];
                label = opt.DisplayName;
                if (string.Equals(opt.vendorCode, "LIVE", StringComparison.OrdinalIgnoreCase))
                    FlightVerifierService.ApplyLiveOfferToFlight(flight, verifier);
                else
                    FlightVerifierService.ApplyBookingOptionToFlight(flight, opt);
                FlightVerifierService.EnsureAtolCharge(flight, configuredAtol);
            }
            else
            {
                FlightVerifierService.ApplyLiveOfferToFlight(flight, verifier);
                FlightVerifierService.EnsureAtolCharge(flight, configuredAtol);
                label = "Live fare";
            }

            PersistFlightInSearch(result, flight, searchId);

            var payable = FlightVerifierService.GetPayableTotal(flight);
            var ff = flight.FlightFare;
            FlightVerifierService.SaveSelectedTicket(searchId, flightId, new FlightVerifierService.SelectedTicketState
            {
                BookingOptionIndex = bookingOptionIndex,
                Total = payable,
                Currency = flight.Currency ?? "USD",
                Label = label
            });

            return Json(new
            {
                ok = true,
                total = Math.Round(ff?.GrandTotal ?? flight.TotalCost, 2),
                payable,
                currency = flight.Currency ?? "USD",
                adultFare = ff?.AdultFare ?? 0,
                adultTax = ff?.AdultTax ?? 0,
                adultMarkup = ff?.AdultMarkup ?? 0,
                childFare = ff?.ChildFare ?? 0,
                childTax = ff?.ChildTax ?? 0,
                childMarkup = ff?.ChildMarkup ?? 0,
                infantFare = ff?.InfantFare ?? 0,
                infantTax = ff?.InfantTax ?? 0,
                infantMarkup = ff?.InfantMarkup ?? 0,
                adult = ff?.Adult ?? 0,
                child = ff?.Child ?? 0,
                infant = ff?.Infant ?? 0,
                atolCharge = ff?.AtolCharge ?? 0,
                label
            });
        }

        private static string SanitizeFilePart(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return "unknown";
            foreach (var c in Path.GetInvalidFileNameChars())
                value = value.Replace(c, '_');
            return value.Length > 80 ? value.Substring(0, 80) : value;
        }

        //public IActionResult Index(string depart, string arrival, string trip, /*string departOn*/ string adult, string child, string infant, string cabin, string flightId,/* string searchID*/int moreadultcount, int morechildcount, string returnOn, string seniors, string utm_campaign = "", string ch = "flightshall", string utm_source = "kayak", string flightshall = "", string utm_medium = "cpc", string af = "")
        //{
        //    AplicationHelper.Log("Index API Hit");

        //    var qs = "";
        //    string departOn = HttpContext.Request.Query["departOn[]"].ToString();

        //    if (departOn == "" || departOn == null)
        //    {
        //        departOn = HttpContext.Request.Query["departOn"];
        //    }

        //    string search = HttpContext.Request.Query["searchID"].ToString();
        //    string searchID = search.Substring(search.LastIndexOf(',') + 1);

        //    AplicationHelper.Log("SearchID: " + searchID);

        //    ViewBag.SearchId = searchID;

        //    FlightsResult value = AplicationHelper.Get<FlightsResult>(searchID, "search_");
        //    FlightsResult newFlightsResult = value;

        //    if (newFlightsResult != null) ;
        //    else
        //    {
        //        AplicationHelper.Log("FlightsResult NULL - Redirecting");

        //        return Redirect(Common.WebSitePath + "Result?depart=" + depart + "&arrival=" + arrival + "&trip=" + trip + "&departOn[]=" + departOn + "&af=" + af + "&ch=" + ch + "&adult=" + adult + "&child=" + child + "&infant=" + infant + "&seniors=" + seniors + "&cabin=" + cabin + "&searchID=" + searchID + "&returnOn=" + returnOn + "&utm_source=" + utm_source + "&utm_medium=" + utm_medium + "&utm_campaign=" + utm_campaign + "" + searchID);
        //    }

        //    try
        //    {
        //        string backurl = (Common.WebSitePath + "Result?depart=" + depart + "&arrival=" + arrival + "&trip=" + trip + "&departOn[]=" + departOn + "&af=" + af + "&ch=" + ch + "&adult=" + adult + "&child=" + child + "&infant=" + infant + "&seniors=" + seniors + "&cabin=" + cabin + "&searchID=" + searchID + "&returnOn=" + returnOn + "&utm_source=" + utm_source + "&utm_medium=" + utm_medium + "&utm_campaign=" + utm_campaign + "" + searchID);

        //        Flights fls = newFlightsResult.FlightsList.Where(x => x.FlightId == flightId).FirstOrDefault();

        //        if (fls == null)
        //        {
        //            AplicationHelper.Log("Flight NULL for FlightId: " + flightId);
        //        }

        //        ViewBag.BackToResult = backurl;
        //        FlightsSearch flsearch = newFlightsResult.FlightsSearch;

        //        ViewBag.FlightWay = flsearch.FlightWay.ToString();
        //        ViewBag.FlightClass = flsearch.FlightClass;

        //        ViewBag.OutboundClass = fls.Outbound[0].ClassOfService;

        //        if (fls.Inbound != null && fls.Inbound.Count > 0)
        //        {
        //            if (fls.Inbound[0] != null)
        //            {
        //                ViewBag.InboundClass = fls.Inbound[0].ClassOfService;
        //            }
        //            ViewBag.InboundClass = fls.Inbound[0].ClassOfService;
        //        }

        //        ViewBag.MarkupType = fls.FlightFare.MarkupType;
        //        ViewBag.MarkupValue = fls.FlightFare.MarkupValue;

        //        if (moreadultcount != 0 && moreadultcount != null)
        //        {
        //            flsearch.Adult = moreadultcount;
        //            fls.FlightFare.Adult = flsearch.Adult;
        //        }
        //        if (morechildcount != null)
        //        {
        //            flsearch.Child = morechildcount;
        //            fls.FlightFare.Child = flsearch.Child;
        //        }

        //        ViewBag.Adult = flsearch.Adult;
        //        ViewBag.Child = flsearch.Child;
        //        ViewBag.Infant = flsearch.Infant;
        //        ViewBag.Airlines = "";
        //        ViewBag.FromName = Common.GetAirport(flsearch.From.Code).CityName;
        //        ViewBag.FromCode = flsearch.From.Code;
        //        ViewBag.ToName = Common.GetAirport(flsearch.To.Code).CityName;
        //        ViewBag.ToCode = flsearch.To.Code;
        //        ViewBag.MainCabinAllowed = TravelWeb.Utilities.Common.MainCabinAllowed.ToString();
        //        ViewBag.SeatPrefrenceAmount = 0;
        //        ViewBag.TotalPax = (Convert.ToInt32(ViewBag.Adult) + Convert.ToInt32(ViewBag.Child));

        //        ViewBag.TotalMainCabinPrice = 0;
        //        ViewBag.TotalAmountWithPreference = fls.FlightFare.GrandTotal + ViewBag.TotalMainCabinPrice - (fls.FlightFare.InfantFare * fls.FlightFare.Infant);

        //        ViewBag.Qs = "result?depart = " + depart + " & arrival = " + arrival + " & trip = " + trip + " & departOn[] = " + departOn + " & af = " + af + " & ch = " + ch + " & adult = " + adult + " & child = " + child + " & infant = " + infant + " & seniors = " + seniors + " & cabin = " + cabin + " & returnOn = " + returnOn + " & utm_source = " + utm_source + " & utm_medium = " + utm_medium + " & utm_campaign = " + utm_campaign + "";

        //        string pax = "";
        //        if (Convert.ToInt32(flsearch.Adult) < 2)
        //        {
        //            pax += flsearch.Adult + " Adult ";
        //        }
        //        else if (Convert.ToInt32(flsearch.Adult) > 1)
        //        {
        //            pax += flsearch.Adult + " Adults ";
        //        }

        //        if (Convert.ToInt32(flsearch.Child) < 2 && Convert.ToInt32(flsearch.Child) > 0)
        //        {
        //            pax += flsearch.Child + " Child ";
        //        }
        //        else if (Convert.ToInt32(flsearch.Child) > 1)
        //        {
        //            pax += flsearch.Child + " Childs ";
        //        }

        //        if (Convert.ToInt32(flsearch.Infant) < 2 && Convert.ToInt32(flsearch.Infant) > 0)
        //        {
        //            pax += flsearch.Infant + " Infant ";
        //        }
        //        else if (Convert.ToInt32(flsearch.Infant) > 1)
        //        {
        //            pax += flsearch.Infant + " Infants ";
        //        }

        //        ViewBag.Trip = flsearch.FlightWay + ", " + flsearch.FlightClass + ", " + pax;

        //        ViewBag.Depart = flsearch.DepDate;
        //        ViewBag.Return = flsearch.RetDate;

        //        fls.SearchId = searchID;

        //        decimal adultTotal = (decimal)(fls.FlightFare.AdultFare + fls.FlightFare.AdultTax) * fls.FlightFare.Adult;
        //        decimal childTotal = (decimal)(fls.FlightFare.ChildFare + fls.FlightFare.ChildTax) * fls.FlightFare.Child;
        //        decimal infantTotal = (decimal)(fls.FlightFare.InfantFare + fls.FlightFare.InfantTax) * fls.FlightFare.Infant;

        //        decimal priceWithoutMarkup = adultTotal + childTotal + infantTotal;

        //        decimal totalMarkup = (decimal)((fls.FlightFare.GrandTotal));

        //        try
        //        {
        //            AplicationHelper.Log("Tracker Update Start: " + searchID);

        //            var dip = new DeepLinkTracker
        //            {
        //                Stage = "ContactDetailsPage",
        //                PriceWithoutMarkup = priceWithoutMarkup,
        //                Markup = totalMarkup,
        //                Rule = fls.FlightFare.MarkupRuleName,
        //                Airline = fls.Airline?.Name,
        //                CabinClass = fls.Outbound?[0]?.CabinClass,
        //                SubClass = fls.Outbound?[0]?.ClassOfService,
        //                ModifiedBy = fls.FlightFare.UpdatedBy,
        //                TotalAmount = (decimal)fls.FlightFare.GrandTotal,
        //                InboundClass = fls.Inbound[0].ClassOfService,
        //                OutboundClass = fls.Outbound[0].ClassOfService,
        //                InboundAirline = fls.Inbound[0].Airline.Code,
        //                OutboundAirline = fls.Outbound[0].Airline.Code,
        //                ReturnDate = flsearch.RetDate,
        //                DepartureDate = flsearch.DepDate
        //            };

        //            _deepLinkTrackerService.UpdateTrackerdata(searchID, dip);
        //        }
        //        catch (Exception ex)
        //        {
        //            AplicationHelper.Log("Tracker Error: " + ex.Message);
        //            _logger.LogError(ex, "Error updating tracker stage");
        //        }

        //        return View(fls);
        //    }
        //    catch (Exception ex)
        //    {
        //        AplicationHelper.Log("Main Exception: " + ex.Message);

        //        ViewBag.error = ex.Message.ToString();
        //        return View();
        //    }
        //}
        [HttpGet]
        public IActionResult RedirectToClient(string guid, string flightId)
        {
            if (string.IsNullOrEmpty(guid) || string.IsNullOrEmpty(flightId))
            {
                return BadRequest("Missing required parameters: guid or flightId.");
            }

            // RushMyFlight deep link
            string redirectUrl = $"https://rushmyflight.com/Flight/FlightBooking?Guid={guid}&FId={flightId}&Source=FS";
            return Redirect(redirectUrl);
        }

        public Object XMLToObject(string XMLString, Object oObject)
        {

            XmlSerializer oXmlSerializer = new XmlSerializer(oObject.GetType());
            oObject = oXmlSerializer.Deserialize(new StringReader(XMLString));
            return oObject;
        }
        //public IActionResult Index(string qs, string flightId)

        //{

        //    if (HttpContext.Session.GetObjectFromJson<string>("IsValid") == "FALSE")
        //    {
        //        return Redirect("/result?qs=" + qs);

        //    }
        //        FlightsResult newFlightsResult = HttpContext.Session.GetObjectFromJson<FlightsResult>("FlightsResultDetail");

        //    if (newFlightsResult == null)
        //    {

        //        return Redirect("result?qs=" + qs);
        //    }
        //    try
        //    {

        //  Flights fls = newFlightsResult.FlightsList.Where(x => x.FlightId == flightId).FirstOrDefault();


        //        ViewBag.BackToResult = TravelWeb.Utilities.Common.WebSitePath + "result?qs=" + qs.Replace(qs.Split('|')[1], "HOME");


        //        FlightsSearch flsearch = newFlightsResult.FlightsSearch;


        //    HttpContext.Session.SetObjectAsJson("Flights", fls);
        //    HttpContext.Session.SetObjectAsJson("FlightsSearch", flsearch);

        //    ViewBag.FlightWay = flsearch.FlightWay.ToString();
        //    ViewBag.FlightClass = flsearch.FlightClass;
        //    ViewBag.OutboundClass = fls.Outbound[0].FareClasses;

        //    if (fls.Inbound != null && fls.Inbound.Count > 0)
        //    {
        //       if (fls.Inbound[0] != null)
        //       {
        //        ViewBag.InboundClass = fls.Inbound[0].FareClasses;
        //       }
        //        ViewBag.InboundClass = fls.Inbound[0].FareClasses;
        //    }

        //    ViewBag.MarkupType = fls.FlightFare.MarkupType;
        //    ViewBag.MarkupValue = fls.FlightFare.MarkupValue;
        //    ViewBag.Adult = flsearch.Adult;
        //    ViewBag.Child = flsearch.Child;
        //    ViewBag.Infant = flsearch.Infant;
        //    ViewBag.Airlines = ""; ;
        //    ViewBag.FromName = Common.GetAirport(flsearch.From.Code).CityName ;// flsearch.From.CityName;
        //    ViewBag.FromCode = flsearch.From.Code;
        //    ViewBag.ToName = Common.GetAirport(flsearch.To.Code).CityName;// flsearch.To.CityName;
        //    ViewBag.ToCode = flsearch.To.Code;
        //    ViewBag.MainCabinAllowed = TravelWeb.Utilities.Common.MainCabinAllowed.ToString();
        //    //ViewBag.PaxWithoutInfant = TravelWeb.Utilities.Common.PaxWithoutInfant.ToString();
        //    ViewBag.SeatPrefrenceAmount = TravelWeb.Utilities.Common.SeatPrefrenceAmount.ToString();
        //    ViewBag.TotalPax = (Convert.ToInt32(ViewBag.Adult) + Convert.ToInt32(ViewBag.Child) /*+ Convert.ToInt32(ViewBag.Infant)*/);

        //    //int PaxwithoutInfant = Convert.ToInt32(ViewBag.Adult) + Convert.ToInt32(ViewBag.Child);
        //        //if (PaxWithoutInfant == "true")
        //        //{
        //        //    PaxwithoutInfant += Convert.ToInt32(ViewBag.Infant);
        //        //}
        //    ViewBag.TotalMainCabinPrice = Convert.ToDouble(Convert.ToDouble(ViewBag.SeatPrefrenceAmount) * ViewBag.TotalPax);
        //    ViewBag.TotalAmountWithPreference = fls.FlightFare.GrandTotal + ViewBag.TotalMainCabinPrice;
        //    ViewBag.Qs = "result?qs=" + qs;
        //    string pax = "";
        //    if (Convert.ToInt32(flsearch.Adult) < 2)
        //    {
        //        pax += flsearch.Adult + " Adult ";
        //    }
        //    else if (Convert.ToInt32(flsearch.Adult) > 1)
        //    {
        //        pax += flsearch.Adult + " Adults ";
        //    }
        //    if (Convert.ToInt32(flsearch.Child) < 2 && Convert.ToInt32(flsearch.Child) > 0)
        //    {
        //        pax += flsearch.Child + " Child ";
        //    }
        //    else if (Convert.ToInt32(flsearch.Child) > 1)
        //    {
        //        pax += flsearch.Child + " Childs ";
        //    }
        //    if (Convert.ToInt32(flsearch.Infant) < 2 && Convert.ToInt32(flsearch.Infant) > 0)
        //    {
        //        pax += flsearch.Infant + " Infant ";
        //    }
        //    else if (Convert.ToInt32(flsearch.Infant) > 1)
        //    {
        //        pax += flsearch.Infant + " Infants ";
        //    }

        //    ViewBag.Trip = flsearch.FlightWay + ", " + flsearch.FlightClass + ", " + pax;

        //    ViewBag.Depart = flsearch.DepDate;//Convert.ToDateTime(flsearch.DepDate).ToString("MMMM, dd, yyyy");
        //    ViewBag.Return = flsearch.RetDate;//Convert.ToDateTime(flsearch.RetDate).ToString("MMMM, dd, yyyy");

        //    //DataTable dtx=    SqlData.Data.SqlHelperData.ExecuteDataset(Common.ConnectionString, CommandType.Text, "SELECT * FROM FlightBooking").Tables[0];

        //    //DataTable inbound = ToDataTable(fls.Inbound);

        //    return View(fls);
        //    }
        //    catch (WebException ex)
        //    {
        //        // Response.Write("");
        //        ViewBag.error = ex.Message.ToString();
        //        return View();



        //    }

        //}

        [HttpPost]
        public JsonResult SaveData(string AdultData, string ChildtData, string InfantData, string phoneNumber, string Emailval, string billAddress1, string billAddress2, string billPostalCode, string billCity, string billState, string billCountry, string billPCardName, string CardNumber, string CardValMonth, string CardvalYear, string CardSecrutyCode, string CardType, string TotalFinalCost, bool isMainCabinChecked, string SeatPrefrenceAmount, string totalSeatPrefrenceAmount)
        {
            try
            {


                dynamic AdultDetails = JsonConvert.DeserializeObject(AdultData);
                dynamic ChildDetails = JsonConvert.DeserializeObject(ChildtData);
                dynamic InfantDetails = JsonConvert.DeserializeObject(InfantData);

                //FlightsSearch flsearch = AplicationHelper.Get<FlightsSearch>(searchId, "flightsSearch");


                Flights newFlights = HttpContext.Session.GetObjectFromJson<Flights>("Flights");
                FlightsSearch newFlightsSearch = HttpContext.Session.GetObjectFromJson<FlightsSearch>("FlightsSearch");


                List<FlightsSegment> outbound = newFlights.Outbound;
                List<FlightsSegment> inbound = newFlights.Inbound;

                DataTable dtPax = new DataTable();
                dtPax.Columns.Add("FirstName", typeof(string));
                dtPax.Columns.Add("LastName", typeof(string));
                dtPax.Columns.Add("DateOfBirth", typeof(string));
                dtPax.Columns.Add("Gender", typeof(string));
                dtPax.Columns.Add("PaxType", typeof(string));

                foreach (var adlt in AdultDetails)
                {
                    DataRow drad = dtPax.NewRow();
                    drad["DateOfBirth"] = adlt["DateOfBirth"].ToString();
                    drad["Gender"] = adlt["Gender"].ToString();
                    drad["FirstName"] = adlt["FirstName"].ToString();
                    drad["LastName"] = adlt["LastName"].ToString();
                    drad["PaxType"] = "Adult";
                    dtPax.Rows.Add(drad);
                }
                foreach (var chld in ChildDetails)
                {
                    DataRow drch = dtPax.NewRow();
                    drch["DateOfBirth"] = chld["DateOfBirth"].ToString();
                    drch["Gender"] = chld["Gender"].ToString();
                    drch["FirstName"] = chld["FirstName"].ToString();
                    drch["LastName"] = chld["LastName"].ToString();
                    drch["PaxType"] = "Child";
                    dtPax.Rows.Add(drch);
                }
                foreach (var infnt in InfantDetails)
                {
                    DataRow drinf = dtPax.NewRow();
                    drinf["DateOfBirth"] = infnt["DateOfBirth"].ToString();
                    drinf["Gender"] = infnt["Gender"].ToString();
                    drinf["FirstName"] = infnt["FirstName"].ToString();
                    drinf["LastName"] = infnt["LastName"].ToString();
                    drinf["PaxType"] = "Infant";
                    dtPax.Rows.Add(drinf);
                }
                List<Passenger> lps = new List<Passenger>();

                foreach (DataRow drp in dtPax.Rows)
                {
                    Passenger psg = new Passenger();
                    psg.FirstName = drp["FirstName"].ToString();
                    psg.LastName = drp["LastName"].ToString();
                    psg.DateOfBirth = drp["DateOfBirth"].ToString();
                    psg.Gender = drp["Gender"].ToString();
                    psg.PaxType = drp["PaxType"].ToString();
                    lps.Add(psg);
                }

                string paxfullname = dtPax.Rows[0]["FirstName"].ToString() + " " + dtPax.Rows[0]["LastName"].ToString();

                string bookingno = "B" + DateTime.Now.ToString("yyMMddHHmmssff");

                SqlParameter[] param = new SqlParameter[29];
                param[0] = new SqlParameter("@BookingNumber", bookingno);
                param[1] = new SqlParameter("@BookingDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                param[2] = new SqlParameter("@FromDeparture", newFlightsSearch.From.CityCode);
                param[3] = new SqlParameter("@ToDestination", newFlightsSearch.To.CityCode);
                param[4] = new SqlParameter("@DepartDate", Convert.ToDateTime(newFlightsSearch.DepDate.ToString()).ToString("yyyy-MM-dd HH:mm:ss"));
                param[5] = new SqlParameter("@ReturnDate", Convert.ToDateTime(newFlightsSearch.RetDate.ToString()).ToString("yyyy-MM-dd HH:mm:ss"));
                param[6] = new SqlParameter("@Adult", newFlightsSearch.Adult);
                param[7] = new SqlParameter("@Child", newFlightsSearch.Child);
                param[8] = new SqlParameter("@Infant", newFlightsSearch.Infant);
                param[9] = new SqlParameter("@Nett", Convert.ToInt32(TotalFinalCost));
                param[10] = new SqlParameter("@Status", "PENDING");
                param[11] = new SqlParameter("@PassangerData", ToXML(lps));
                param[12] = new SqlParameter("@FlightDetailsOutbound", ToXML(outbound));
                param[13] = new SqlParameter("@FlightDetailsInbound", ToXML(inbound));
                param[14] = new SqlParameter("@ContactPerson", phoneNumber);
                param[15] = new SqlParameter("@ContactEmail", Emailval);
                param[16] = new SqlParameter("@PaxFullName", paxfullname);
                param[17] = new SqlParameter("@BillAddress1", billAddress1);
                param[18] = new SqlParameter("@BillAddress2", billAddress2);
                param[19] = new SqlParameter("@BillPostalCode", billPostalCode);
                param[20] = new SqlParameter("@BillCity", billCity);
                param[21] = new SqlParameter("@BillState", billState);
                param[22] = new SqlParameter("@BillCountry", billCountry);
                param[23] = new SqlParameter("@CardName", billPCardName);
                param[24] = new SqlParameter("@CardType", CardType);
                param[25] = new SqlParameter("@CardNumber", CardNumber);
                param[26] = new SqlParameter("@CardMonth", CardValMonth);
                param[27] = new SqlParameter("@CardValYear", CardvalYear);
                param[28] = new SqlParameter("@CardSecCode", CardSecrutyCode);
                // param[28].Direction = ParameterDirection.InputOutput;
                //  SqlData.Data.SqlHelperData.ExecuteNonQuery(TravelWeb.Utilities.Common.ConnectionString, CommandType.StoredProcedure, "HSP_InsertBooking", param);



                AcknowledgeData ack = new AcknowledgeData();
                ack.Reference = bookingno;
                ack.PassengerName = paxfullname;
                ack.PaidTo = "RushMyFlight Travel";
                ack.PaymentMethod = CardType;
                ack.TravelDate = Convert.ToDateTime(newFlightsSearch.DepDate.ToString()).ToString("MMM-dd-yyyy HH:mm");
                string subject = newFlightsSearch.From.CityCode + " " + newFlightsSearch.To.CityCode;
                if (newFlightsSearch.FlightWay.ToString() == "RoundTrip")
                {
                    subject += " RETURN " + Convert.ToDateTime(newFlightsSearch.DepDate.ToString()).ToString("MMM-dd-yyyy") + " " + Convert.ToDateTime(newFlightsSearch.RetDate.ToString()).ToString("MMM-dd-yyyy");
                }
                else
                {
                    subject += " ONEWAY " + Convert.ToDateTime(newFlightsSearch.DepDate.ToString()).ToString("MMM-dd-yyyy");
                }
                if (Convert.ToInt32(newFlightsSearch.Adult) > 0)
                {
                    subject += "  " + newFlightsSearch.Adult + " ADULT ";
                }
                if (Convert.ToInt32(newFlightsSearch.Child) > 0)
                {
                    subject += newFlightsSearch.Child + " CHILD ";
                }
                if (Convert.ToInt32(newFlightsSearch.Infant) > 0)
                {
                    subject += newFlightsSearch.Infant + " INFANT";
                }
                ack.Subject = subject;
                //ack.AmountPaid = newFlights.TotalCost.ToString("f2");
                ack.AmountPaid = TotalFinalCost.ToString();


                HttpContext.Session.SetObjectAsJson("AcknowledgeData", ack);

                // string bookingno = "B" + DateTime.Now.ToString("yyMMddHHmmssff");
                string SqlQuery = "INSERT INTO FlightBookings (BookingNumber,BookingDate,FromDeparture,ToDestination,DepartDate,ReturnDate,ContactPerson,ContactEmail,BillAddress1,BillAddress2,BillPostalCode,BillCity,BillState,BillCountry,nett,CardCharge,Gross,Comments,FlightClass,Airline,CardNumber) ";
                SqlQuery += " VALUES('" + bookingno + "',getdate(),'" + newFlightsSearch.From.Code + "','" + newFlightsSearch.To.Code + "','" + Convert.ToDateTime(newFlightsSearch.DepDate.ToString()).ToString("dd-MMM") + "','" + Convert.ToDateTime(newFlightsSearch.RetDate.ToString()).ToString("dd-MMM") + "','" + phoneNumber + "','" + Emailval + "','" + billAddress1 + "','" + billAddress2 + "','" + billCity + "','" + billPostalCode + "','" + billState + "','" + billCountry + "'," + newFlights.TotalCost + ",0," + newFlights.TotalCost + ",''," + newFlights.FlightClass + ")";

                Int64 BookingID = SqlData.Data.SqlHelperData.AddToTableReturnID(TravelWeb.Utilities.Common.ConnectionString, SqlQuery);

                foreach (FlightsSegment seg in outbound)
                {
                    string SQLQuery = "INSERT INTO[dbo].[BookingFlghtDetails]([BookingID],[DepartDate],[ArrivalDate],[FlightNo],[Airline],[DepartOn],[Dcode],[Gcode],[ArriveOn])";
                    SQLQuery += "VALUES(" + BookingID + ",'" + seg.DepartureDate.ToString("dd-MMM") + "','" + seg.ArrivalDate.ToString("dd-MMM") + "','" + seg.FlightNo + "'";
                    SQLQuery += ",'" + seg.Airline.Code + "','" + seg.DepartureDate.ToString("HH:mm") + "','" + seg.FromAirport.Code + "','" + seg.ToAirport.Code + "','" + seg.ArrivalDate.ToString("HH:mm") + "')";

                    int ck = SqlData.Data.SqlHelperData.ExecuteNonQuery(TravelWeb.Utilities.Common.ConnectionString, CommandType.Text, SQLQuery);
                }
                if (inbound.Count > 0)
                {
                    foreach (FlightsSegment seg in inbound)
                    {
                        string SQLQuery = "INSERT INTO[dbo].[BookingFlghtDetails]([BookingID],[DepartDate],[ArrivalDate],[FlightNo],[Airline],[DepartOn],[Dcode],[Gcode],[ArriveOn])";
                        SQLQuery += "VALUES(" + BookingID + ",'" + seg.DepartureDate.ToString("dd-MMM") + "','" + seg.ArrivalDate.ToString("dd-MMM") + "','" + seg.FlightNo + "'";
                        SQLQuery += ",'" + seg.Airline.Code + "','" + seg.DepartureDate.ToString("HH:mm") + "','" + seg.FromAirport.Code + "','" + seg.ToAirport.Code + "','" + seg.ArrivalDate.ToString("HH:mm") + "')";

                        int ck = SqlData.Data.SqlHelperData.ExecuteNonQuery(TravelWeb.Utilities.Common.ConnectionString, CommandType.Text, SQLQuery);
                    }
                }
                foreach (Passenger pax in lps)
                {
                    string SQLQuery = "INSERT INTO[dbo].[BookingPaxDetails] ([BookingID],[PaxTitle],[PaxType],[FirstName],[LastName],[DOB])";
                    SQLQuery += "VALUES(" + BookingID + ", '" + pax.Title + "', '" + pax.PaxType + "', '" + pax.FirstName + "', '" + pax.LastName + "', '" + pax.DateOfBirth + "')";
                    int ck = SqlData.Data.SqlHelperData.ExecuteNonQuery(TravelWeb.Utilities.Common.ConnectionString, CommandType.Text, SQLQuery);
                }


                //string sqlquery = "Insert into FlightBookings(BookingNumber,[BookingDate],[FromDeparture],[ToDestination],[DepartDate],[ReturnDate],[Adult],[Child],[Infant],[Nett],[Status],[PassangerData],[FlightDetailsOutbound],[FlightDetailsInbound],[ContactPerson],[ContactEmail],[PaxFullName],[BillAddress1],[BillAddress2],[BillPostalCode],[BillCity],[BillState],[BillCountry],[CardName],[CardType],[CardNumber],[CardValMonth],[CardValYear], [CardSecCode])";
                //sqlquery += " values('" + bookingno + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + newFlightsSearch.From.CityCode + "','" + newFlightsSearch.To.CityCode + "','" + Convert.ToDateTime(newFlightsSearch.DepDate.ToString()).ToString("yyyy-MM-dd HH:mm:ss") + "','" + Convert.ToDateTime(newFlightsSearch.RetDate.ToString()).ToString("yyyy-MM-dd HH:mm:ss") + "'," + newFlightsSearch.Adult + "," + newFlightsSearch.Child + "," + newFlightsSearch.Infant + ",'" + newFlights.TotalCost + "','PENDING','" + ToXML(lps) + "','" + ToXML(outbound) + "','" + ToXML(inbound) + "',";
                //sqlquery += " '" + phoneNumber + "','" + Emailval + "','" + paxfullname + "','" + billAddress1 + "','" + billAddress2 + "','" + billPostalCode + "','" + billCity + "','" + billState + "','" + billCountry + "','" + billPCardName + "','" + CardType + "','" + CardNumber + "','" + CardValMonth + "','" + CardvalYear + "','" + CardSecrutyCode + "')";

                //int i = SqlData.Data.SqlHelperData.ExecuteNonQuery(TravelWeb.Utilities.Common.ConnectionString, CommandType.Text, sqlquery);



            }
            catch (Exception ex)
            {

            }


            // Here you can store datatable value in database


            return Json("");
        }

        private string GetBookingNumber()
        {
            string bookingNumber = "";
            string todayDate = "B" + DateTime.Now.ToString("yyMMdd");
            var context = new Datamodel.RSFLY();

            //var latestBookingNumber = context.FlightBookings.Where(x => x.BookingNumber.Contains(todayDate)).Select(x => x.BookingNumber).OrderByDescending(x => x.).Take(1);
            //var latestBooking = context.BookingsB2cs.Where(x => !string.IsNullOrEmpty(x.RandomNo)).OrderByDescending(x => x.BookingId).FirstOrDefault(); if (latestBooking != null)
            var latestBooking = context.FlightBookings.OrderByDescending(x => x.BookingId).FirstOrDefault();
            if (latestBooking != null)
            {
                if (latestBooking.BookingNumber.Contains(todayDate))
                {
                    int currentNo = Convert.ToInt32(latestBooking.BookingNumber.Substring(7)) + 1;
                    bookingNumber = todayDate + currentNo.ToString("D3");
                }
                else
                    bookingNumber = todayDate + "001";
            }
            else
                bookingNumber = todayDate + "001";
            return bookingNumber;
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


        public JsonResult SendEmailToBusiness(string FlightClass, string FlightWay, string SupportPlan, string suppurtPlanAmt, string BaggagePlan, string totBagPrice, string PriceSummury, string AdultData, string ChildtData, string InfantData, string phoneNumber, string Emailval, string TotalFinalCost, string ModelAirlineName, string FromCode, string ToCode, string DepartDate, string ReturnDate, string CustomerIpAddress, string BookingID)
        {
            try
            {


                dynamic AdultDetails = JsonConvert.DeserializeObject(AdultData);
                dynamic ChildDetails = JsonConvert.DeserializeObject(ChildtData);
                dynamic InfantDetails = JsonConvert.DeserializeObject(InfantData);
                FlightsSearch newFlightsSearch = HttpContext.Session.GetObjectFromJson<FlightsSearch>("FlightsSearch");
                dynamic PriceSummuryDetails = JsonConvert.DeserializeObject(PriceSummury);


                DataTable dtPax = new DataTable();
                dtPax.Columns.Add("FirstName", typeof(string));
                dtPax.Columns.Add("LastName", typeof(string));
                dtPax.Columns.Add("MiddleName", typeof(string));
                dtPax.Columns.Add("DateOfBirth", typeof(string));
                dtPax.Columns.Add("Gender", typeof(string));
                dtPax.Columns.Add("PaxType", typeof(string));

                foreach (var adlt in AdultDetails)
                {
                    DataRow drad = dtPax.NewRow();
                    drad["DateOfBirth"] = adlt["DateOfBirth"].ToString();
                    drad["Gender"] = adlt["Gender"].ToString();
                    drad["FirstName"] = adlt["FirstName"].ToString();
                    drad["LastName"] = adlt["LastName"].ToString();
                    drad["MiddleName"] = adlt["MiddleName"].ToString();
                    drad["PaxType"] = "Adult";
                    dtPax.Rows.Add(drad);
                }
                foreach (var chld in ChildDetails)
                {
                    DataRow drch = dtPax.NewRow();
                    drch["DateOfBirth"] = chld["DateOfBirth"].ToString();
                    drch["Gender"] = chld["Gender"].ToString();
                    drch["FirstName"] = chld["FirstName"].ToString();
                    drch["LastName"] = chld["LastName"].ToString();
                    drch["MiddleName"] = chld["MiddleName"].ToString();
                    drch["PaxType"] = "Child";
                    dtPax.Rows.Add(drch);
                }
                foreach (var infnt in InfantDetails)
                {
                    DataRow drinf = dtPax.NewRow();
                    drinf["DateOfBirth"] = infnt["DateOfBirth"].ToString();
                    drinf["Gender"] = infnt["Gender"].ToString();
                    drinf["FirstName"] = infnt["FirstName"].ToString();
                    drinf["LastName"] = infnt["LastName"].ToString();
                    drinf["MiddleName"] = infnt["MiddleName"].ToString();
                    drinf["PaxType"] = "Infant";
                    dtPax.Rows.Add(drinf);
                }
                List<Passenger> lps = new List<Passenger>();

                foreach (DataRow drp in dtPax.Rows)
                {
                    Passenger psg = new Passenger();
                    psg.FirstName = drp["FirstName"].ToString();
                    psg.LastName = drp["LastName"].ToString();
                    psg.DateOfBirth = drp["DateOfBirth"].ToString();
                    psg.Gender = drp["Gender"].ToString();
                    psg.PaxType = drp["PaxType"].ToString();
                    psg.MiddleName = drp["MiddleName"].ToString();
                    lps.Add(psg);
                }
                string paxfullname = dtPax.Rows[0]["FirstName"].ToString() + " " + dtPax.Rows[0]["LastName"].ToString();
                sendConfirmationEmailtoBusiness(lps, FlightWay, phoneNumber, Emailval, TotalFinalCost, ModelAirlineName, FromCode, ToCode, DepartDate, ReturnDate, CustomerIpAddress, BookingID);
            }

            catch (Exception ex)
            {
                throw ex;
            }
            return Json("");
        }

        public void sendConfirmationEmailtoBusiness(List<Passenger> lps, string FlightWay, string phoneNumber, string Emailval, string TotalFinalCost, string ModelAirlineName, string FromCode, string ToCode, string DepartDate, string ReturnDate, string CustomerIpAddress, string BookingID)
        {
            try
            {
                var context = new FlightshallContext();
                long BId = Convert.ToInt64(BookingID);
                var BookingNumber = context.FlightBookings.Where(x => x.BookingId == BId).Select(x => x.BookingNumber).FirstOrDefault();
                // var GetBookingDetails = context.FlightBookings.Where(x => x.BookingId == BookingID).FirstOrDefault();
                var ContactNumber = " +1 800 231 7119";
                //var ContactNumber = "+1 800 315 9001";
                if (FlightWay == "OneWay")
                {
                    ReturnDate = "";
                }
                StringBuilder stringBuilder = new StringBuilder();
                StringBuilder stringBuilder2 = new StringBuilder();
                StringBuilder stringBuilder3 = new StringBuilder();
                stringBuilder.Append("<br/><br/><table width='800'style='border:solid 1px black; font-family:Verdana;' >");
                stringBuilder.Append("<br/>Dear Sir/Madam, <br/><br/>");
                stringBuilder.Append(" You Got One New Enquiry</strong> <br/><br/>");
                stringBuilder.Append("From : <strong>" + FromCode + "</strong><br/>");
                stringBuilder.Append("To : <strong>" + ToCode + "</strong><br/>");
                stringBuilder.Append("Flight Way : <strong>" + FlightWay + "</strong><br/>");
                stringBuilder.Append("Airline Name : <strong>" + ModelAirlineName + "</strong><br/>");
                stringBuilder.Append("Departure Date : <strong>" + DepartDate + "</strong>");
                stringBuilder.Append("  " + "  " + " " + "Return Date : <strong>" + ReturnDate + "</strong><br/><br/>");
                stringBuilder.Append("Email : <strong>" + Emailval + "</strong><br/>");
                stringBuilder.Append("Contact Number : <strong>" + phoneNumber + "</strong><br/>");
                stringBuilder.Append("Customer Ip Address : <strong>" + CustomerIpAddress + "</strong><br/>");
                stringBuilder.Append("BookingID : <strong>" + BookingNumber + "</strong><br/>");
                stringBuilder.Append("Total Cost : <strong>" + TotalFinalCost + "</strong><br/><br/>");
                stringBuilder.Append("<strong>Passengers Detail</strong></br>");
                stringBuilder3.Append("<br/><table width='800'style='border:solid 0px black; font-family:Verdana;' >");
                stringBuilder3.Append("<tr style='background-color:#CAF6D8;'><th width='400'>Name</th><th width='200'>Date Of Birth</th><th width='200'>PaxType</th><th width='200'>Gender</th></tr>");
                foreach (var item in lps)
                {
                    stringBuilder3.Append("<tr ><td style='border:solid 1px black;  text-align:center;'>" + item.FirstName + " " + item.MiddleName + " " + item.LastName + " </td><td style='border:solid 1px black;  text-align:center;'>" + item.DateOfBirth + "</td><td style='border:solid 1px black;  text-align:center;'>" + item.PaxType + "</td><td style='border:solid 1px black;  text-align:center;'>" + item.Gender + "</td></tr>");

                }
                stringBuilder2.Append("<br/>");
                stringBuilder2.Append("For more offers visit <a href=\"https://www.RushmyFlight.com/\">www.RushmyFlights.com</a><br/>");
                stringBuilder2.Append("Customer services RushmyFlight <br/>");
                stringBuilder2.Append("Tel: <a href=\"tel: " + ContactNumber + "\">" + ContactNumber + "</a><br />Website: <a href=\"https://www..RushmyFlight.com/\">www..RushmyFlight.com</a><br/>");
                stringBuilder2.Append("For full Terms and conditions visit <a href=\"https://www.RushmyFlight.com/terms-conditions\"><b>Terms and conditions</b></a></br>");

                var message = new System.Net.Mail.MailMessage();
                message.Subject = "New Enquiry";
                message.Body = stringBuilder.ToString() + stringBuilder3.ToString() + stringBuilder2.ToString();
                var EmailId = TravelWeb.Utilities.Common.EmailAddress;
                message.From = new MailAddress(EmailId);

                // message.Bcc=


                foreach (var address in Common.BusinessMailTo.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    message.To.Add(address);
                }
                //message.To.Add(TravelWeb.Utilities.Common.BusinessMailTo);

                message.IsBodyHtml = true;
                SmtpClient smtp =
                smtp = new SmtpClient
                {
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    EnableSsl = true,
                    Host = TravelWeb.Utilities.Common.Host,
                    Port = 587,
                    UseDefaultCredentials = false
                };
                smtp.Credentials = new NetworkCredential(TravelWeb.Utilities.Common.EmailAddress, TravelWeb.Utilities.Common.Password);
                //Task.Factory.StartNew(() => smtp.Send(message));
                smtp.Send(message);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void sendConfirmationEmailtoCustomer(int BookingID)
        {
            try
            {

                // var value = RenderInviteView();
                //var Body = value.Result.Value.ToString();
                var context = new TravelWeb.Datamodel.RSFLY();
                var GetBookingDetails = context.FlightBookings.Where(x => x.BookingId == BookingID).FirstOrDefault();
                var Flights = context.BookingFlghtDetails.Where(x => x.BookingId == BookingID /*&& x.Delind == false */).OrderBy(x => x.BookingId).ToList();

                List<FlightsSegment> InBoundFlights = new List<FlightsSegment>();
                List<FlightsSegment> OutBoundFlights = new List<FlightsSegment>();
                foreach (var item in Flights)
                {
                    FlightsSegment fs = new FlightsSegment();
                    fs.Airline = Common.GetAirline(item.Airline);
                    if (item.DepDate != null)
                    {
                        fs.DepartureDate = (DateTime)item.DepDate;
                    }
                    if (item.ArrDate != null)
                    {
                        fs.ArrivalDate = (DateTime)item.ArrDate;
                    }
                    fs.FromAirport = Common.GetAirport(item.Dcode);
                    fs.ToAirport = Common.GetAirport(item.Gcode);
                    if (item.DepDate != null)
                    {
                        fs.DepDate = item.DepartOn + "," + fs.DepartureDate.DayOfWeek + " " + fs.DepartureDate.ToString("dd MMM yyyy");
                    }
                    if (item.ArrDate != null)
                    {
                        fs.ArrDate = item.ArriveOn + "," + fs.ArrivalDate.DayOfWeek + " " + fs.ArrivalDate.ToString("dd MMM yyyy");
                    }
                    fs.FlightNo = item.FlightNo;
                    if (item.ArrDate != null)
                    {
                        TimeSpan ts = (DateTime)item.ArrDate - (DateTime)item.DepDate;
                        fs.TotalTime = new DateTime(ts.Ticks).ToString("HH:mm");
                    }
                    if (item.FlightType == "InBound")
                        InBoundFlights.Add(fs);
                    else
                        OutBoundFlights.Add(fs);
                }
                var GetPaxDetails = context.BookingPaxDetails.Where(x => x.BookingId == BookingID /* && x.Delind == false */).OrderBy(x => x.PaxType).ToList();
                //BookingEmail be = new BookingEmail();
                EmailbookingModel be = new EmailbookingModel();
                be.InBoundFlights = InBoundFlights;
                be.OutBoundFlights = OutBoundFlights;
                be.FlightBooking = GetBookingDetails;
                be.BookingPaxDetail = GetPaxDetails;
                be.FromLocation = Common.GetAirport(GetBookingDetails.FromDeparture);
                be.ToLocation = Common.GetAirport(GetBookingDetails.ToDestination);

                //if (InBoundFlights.Count > 0)
                //{

                //    TimeSpan totalTimeSpan = new TimeSpan();
                //    for (int i = 0; i < InBoundFlights.Count; i++)
                //    {
                //        TimeSpan A = new TimeSpan();
                //        TimeSpan B = new TimeSpan();
                //        A = InBoundFlights[i].ArrivalDate - InBoundFlights[i].DepartureDate;
                //        if (InBoundFlights.Count > (i+1))
                //        {
                //            B = InBoundFlights[i + 1].DepartureDate - InBoundFlights[i].ArrivalDate;
                //            totalTimeSpan = A + B + totalTimeSpan;

                //        }
                //        totalTimeSpan = A + B + totalTimeSpan;
                //    }
                //    be.FromTime = new DateTime(totalTimeSpan.Ticks).ToString("HH:mm");



                //}
                //if(OutBoundFlights.Count > 0)
                //{
                //    for (int i = 0; i < OutBoundFlights.Count; i++)
                //    {
                //        TimeSpan tsem = OutBoundFlights[i].ArrivalDate - OutBoundFlights[i].DepartureDate;
                //        TimeSpan NewValues = tsem;
                //        if (OutBoundFlights.Count > (i + 1))
                //        {
                //            TimeSpan tsems = OutBoundFlights[i + 1].DepartureDate - OutBoundFlights[i].ArrivalDate;
                //            NewValues = NewValues + tsems;
                //        }

                //    }


                //}
                if (InBoundFlights.Count > 0)
                {
                    DateTime Departure = InBoundFlights.Select(x => x.DepartureDate).First();
                    DateTime Arrival = InBoundFlights.Select(x => x.ArrivalDate).Last();
                    TimeSpan tse = Arrival - Departure;
                    be.ToTime = new DateTime(tse.Ticks).ToString("HH:mm");
                }
                if (OutBoundFlights.Count > 0)
                {

                    DateTime Departure = OutBoundFlights.Select(x => x.DepartureDate).First();
                    DateTime Arrival = OutBoundFlights.Select(x => x.ArrivalDate).Last();
                    TimeSpan tsemo = Arrival - Departure;
                    be.FromTime = new DateTime(tsemo.Ticks).ToString("HH:mm");
                }
                var value = RenderInviteView(be);
                var Body = value.Result.Value.ToString();

                var tos = new List<EmailAddress>();
                tos.Add(new EmailAddress(TravelWeb.Utilities.Common.BccMail.ToString()));
                tos.Add(new EmailAddress(GetBookingDetails.ContactEmail));
                //var apiKey = Environment.GetEnvironmentVariable("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");
                //var client = new SendGridClient("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");
                var from = new EmailAddress("care@rushmyflight.com", "RushMyFlight");
                var subject = "Booking Details";
                //var plainTextContent = "and easy to do anywhere, even with C#";
                var htmlContent = Body;

                //code coment by janesh(07112023)
                //var msg = MailHelper.CreateSingleEmailToMultipleRecipients(from, tos, subject, "", htmlContent);
                //var response = client.SendEmailAsync(msg);

                var message = new System.Net.Mail.MailMessage();
                message.Subject = "Booking Details";
                message.Body = Body;
                var EmailId = TravelWeb.Utilities.Common.EmailAddress;
                message.From = new MailAddress(EmailId, "RushMyFlight");
                message.Bcc.Add(TravelWeb.Utilities.Common.BccMail.ToString());
                message.To.Add(GetBookingDetails.ContactEmail);
                message.IsBodyHtml = true;
                SmtpClient smtp = smtp = new SmtpClient
                {
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    EnableSsl = true,
                    Host = TravelWeb.Utilities.Common.Host,
                    Port = 587,
                    UseDefaultCredentials = true
                };
                //client.UseDefaultCredentials = true;
                smtp.Credentials = new NetworkCredential(TravelWeb.Utilities.Common.EmailAddress, TravelWeb.Utilities.Common.Password);
                smtp.Send(message);

                //if (response.Status.ToString() == "Accepted")
                //{
                //    _logger.LogInformation("Email Send Succcesfully to Booking Id" + BookingID);
                //}
                //else
                //{
                //    _logger.LogInformation("Something went wrong when sending Email");
                //}

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString() + ex.StackTrace.ToString());
                throw ex;
            }

        }
        public async Task<JsonResult> RenderInviteView(EmailbookingModel be)
        {
            var jsion = JsonConvert.SerializeObject(be);
            //var html = await RazorTemplateEngine.RenderAsync("/Views/BookingMail.cshtml", be);
            var htmls = await RenderAsync("BookingMail.cshtml", be);
            //string projectRootPath = _hostingEnvironment.ContentRootPath;
            // var emailView = View("BookingEmail/BookingMail");
            //var result = await this.RenderViewAsync(projectRootPath + "\\Views\\BookingEmail\\BookingMail", be);
            return Json(htmls);
        }

        //public JsonResult SaveLeads(string FlightClass, string MarkupType, string MarkupValue, string InboundClass, string OutboundClass, string FlightWay, string SupportPlan, string suppurtPlanAmt, string BaggagePlan, string totBagPrice, string PriceSummury, string AdultData, string ChildtData, string InfantData, string phoneNumber, string Emailval, string billAddress1, string billAddress2, string billPostalCode, string billCity, string billState, string billCountry, string billPCardName, string CardNumber, string CardValMonth, string CardvalYear, string CardSecrutyCode, string CardType, string TotalFinalCost, string IPAddress, bool isMainCabinChecked, string SeatPrefrenceAmount, string totalSeatPrefrenceAmount)
        //{
        //    //Saving Leads in Booking
        //    try
        //    {

        //        dynamic AdultDetails = JsonConvert.DeserializeObject(AdultData);
        //        dynamic ChildDetails = JsonConvert.DeserializeObject(ChildtData);
        //        dynamic InfantDetails = JsonConvert.DeserializeObject(InfantData);

        //        //HttpContext.Session.SetObjectAsJson("Flights", fls);

        //        Flights newFlights = HttpContext.Session.GetObjectFromJson<Flights>("Flights");
        //        FlightsSearch newFlightsSearch = HttpContext.Session.GetObjectFromJson<FlightsSearch>("FlightsSearch");
        //        dynamic PriceSummuryDetails = JsonConvert.DeserializeObject(PriceSummury);

        //        List<FlightsSegment> outbound = newFlights.Outbound;
        //        List<FlightsSegment> inbound = newFlights.Inbound;

        //        DataTable dtPax = new DataTable();
        //        dtPax.Columns.Add("FirstName", typeof(string));
        //        dtPax.Columns.Add("LastName", typeof(string));
        //        dtPax.Columns.Add("MiddleName", typeof(string));
        //        dtPax.Columns.Add("DateOfBirth", typeof(string));
        //        dtPax.Columns.Add("Gender", typeof(string));
        //        dtPax.Columns.Add("PaxType", typeof(string));

        //        foreach (var adlt in AdultDetails)
        //        {
        //            DataRow drad = dtPax.NewRow();
        //            drad["DateOfBirth"] = adlt["DateOfBirth"].ToString();
        //            drad["Gender"] = adlt["Gender"].ToString();
        //            drad["FirstName"] = adlt["FirstName"].ToString();
        //            drad["LastName"] = adlt["LastName"].ToString();
        //            drad["MiddleName"] = adlt["MiddleName"].ToString();
        //            drad["PaxType"] = "Adult";
        //            dtPax.Rows.Add(drad);
        //        }
        //        foreach (var chld in ChildDetails)
        //        {
        //            DataRow drch = dtPax.NewRow();
        //            drch["DateOfBirth"] = chld["DateOfBirth"].ToString();
        //            drch["Gender"] = chld["Gender"].ToString();
        //            drch["FirstName"] = chld["FirstName"].ToString();
        //            drch["LastName"] = chld["LastName"].ToString();
        //            drch["MiddleName"] = chld["MiddleName"].ToString();
        //            drch["PaxType"] = "Child";
        //            dtPax.Rows.Add(drch);
        //        }
        //        foreach (var infnt in InfantDetails)
        //        {
        //            DataRow drinf = dtPax.NewRow();
        //            drinf["DateOfBirth"] = infnt["DateOfBirth"].ToString();
        //            drinf["Gender"] = infnt["Gender"].ToString();
        //            drinf["FirstName"] = infnt["FirstName"].ToString();
        //            drinf["LastName"] = infnt["LastName"].ToString();
        //            drinf["MiddleName"] = infnt["MiddleName"].ToString();
        //            drinf["PaxType"] = "Infant";
        //            dtPax.Rows.Add(drinf);
        //        }
        //        List<Passenger> lps = new List<Passenger>();

        //        foreach (DataRow drp in dtPax.Rows)
        //        {
        //            Passenger psg = new Passenger();
        //            psg.FirstName = drp["FirstName"].ToString();
        //            psg.LastName = drp["LastName"].ToString();
        //            psg.MiddleName = drp["MiddleName"].ToString();
        //            psg.DateOfBirth = drp["DateOfBirth"].ToString();
        //            psg.Gender = drp["Gender"].ToString();
        //            psg.PaxType = drp["PaxType"].ToString();
        //            lps.Add(psg);
        //        }
        //        string paxfullname = dtPax.Rows[0]["FirstName"].ToString() + " " + dtPax.Rows[0]["MiddleName"] + "" + dtPax.Rows[0]["LastName"].ToString();
        //        string bookingno = GetBookingNumber();
        //        //string bookingno = "B" + DateTime.Now.ToString("yyMMddHHmmssff");


        //        string subject = newFlightsSearch.From.CityCode + " " + newFlightsSearch.To.CityCode;
        //        if (newFlightsSearch.FlightWay.ToString() == "RoundTrip")
        //        {
        //            subject += " RETURN " + Convert.ToDateTime(newFlightsSearch.DepDate.ToString()).ToString("MMM-dd-yyyy") + " " + Convert.ToDateTime(newFlightsSearch.RetDate.ToString()).ToString("MMM-dd-yyyy");
        //        }
        //        else
        //        {
        //            subject += " ONEWAY " + Convert.ToDateTime(newFlightsSearch.DepDate.ToString()).ToString("MMM-dd-yyyy");
        //        }
        //        if (Convert.ToInt32(newFlightsSearch.Adult) > 0)
        //        {
        //            subject += "  " + newFlightsSearch.Adult + " ADULT ";
        //        }
        //        if (Convert.ToInt32(newFlightsSearch.Child) > 0)
        //        {
        //            subject += newFlightsSearch.Child + " CHILD ";
        //        }
        //        if (Convert.ToInt32(newFlightsSearch.Infant) > 0)
        //        {
        //            subject += newFlightsSearch.Infant + " INFANT";
        //        }

        //        string depdate = (newFlightsSearch.DepDate.ToString().Contains("0001")) ? "" : Convert.ToDateTime(newFlightsSearch.DepDate.ToString()).ToString("dd-MMM");
        //        string retDate = (newFlightsSearch.RetDate.ToString().Contains("0001")) ? "" : Convert.ToDateTime(newFlightsSearch.RetDate.ToString()).ToString("dd-MMM");

        //        //string CArdinfo = "CardName :" + billPCardName + "\n";
        //        //CArdinfo += "CardType : " + CardType + "\n";
        //        //CArdinfo += "CardNumber : " + CardNumber + "\n";
        //        //CArdinfo += "CardValMonth : " + CardValMonth + " / " + CardvalYear + "\n";
        //        //CArdinfo += "CardSecCode : " + CardSecrutyCode + "\n";

        //        string CArdinfo = "";
        //        var paymentStatus = "Not Added";

        //        if (CardNumber != null)
        //        {
        //            paymentStatus = "Done";
        //        }
        //        // string bookingno = "B" + DateTime.Now.ToString("yyMMddHHmmssff");

        //        //comment by janesh _13oct25
        //        //string SqlQuery = "INSERT INTO FlightBookings (BookingNumber,BookingDate,FromDeparture,ToDestination,DepartDate,ReturnDate,ContactPerson,ContactEmail,BillAddress1,BillAddress2,BillPostalCode,BillCity,BillState,BillCountry,nett,CardCharge,Gross,Comments,[CardName],[CardType],[CardNumber],[CardValMonth],[CardValYear], [CardSecCode],[IPAddress],[StatusId], MarkupType, MarkupValue) ";
        //        //SqlQuery += " VALUES('" + bookingno + "',getdate(),'" + newFlightsSearch.From.Code + "','" + newFlightsSearch.To.Code + "','" + depdate + "','" + retDate + "','" + phoneNumber + "','" + Emailval + "','" + billAddress1 + "','" + billAddress2 + "','" + billPostalCode + "','" + billCity + "','" + billState + "','" + billCountry + "'," + TotalFinalCost + ",0," + TotalFinalCost + ",'" + CArdinfo + "','" + billPCardName + "','" + CardType + "','" + CardNumber + "','" + CardValMonth + "','" + 2022 + "','" + CardSecrutyCode + "','" + IPAddress + "',10,'" + MarkupType + "','" + MarkupValue + "')";
        //        string SqlQuery = "INSERT INTO FlightBooking (BookingNumber,BookingDate,FromDeparture,ToDestination,DepartDate,ReturnDate,ContactPerson,ContactEmail,BillAddress1,BillAddress2,BillPostalCode,BillCity,BillState,BillCountry,nett,CardCharge,Gross,Comments,[CardName],[CardType],[CardNumber],[CardValMonth],[CardValYear], [CardSecCode],[IPAddress],[StatusId], MarkupType, MarkupValue,PaymentType) ";
        //        SqlQuery += " VALUES('" + bookingno + "',getdate(),'" + newFlightsSearch.From.Code + "','" + newFlightsSearch.To.Code + "','" + depdate + "','" + retDate + "','" + phoneNumber + "','" + Emailval + "','" + billAddress1 + "','" + billAddress2 + "','" + billPostalCode + "','" + billCity + "','" + billState + "','" + billCountry + "'," + TotalFinalCost + ",0," + TotalFinalCost + ",'" + CArdinfo + "','" + billPCardName + "','" + CardType + "','" + CardNumber + "','" + CardValMonth + "','" + 2022 + "','" + CardSecrutyCode + "','" + IPAddress + "',10,'" + MarkupType + "','" + MarkupValue + "','" + paymentStatus + "')";

        //        Int64 BookingID = SqlData.Data.SqlHelperData.AddToTableReturnID(TravelWeb.Utilities.Common.ConnectionString, SqlQuery);



        //        string Adult = PriceSummuryDetails[0].AdultDetails.ToString().Split('|')[0];
        //        string Child = PriceSummuryDetails[0].ChildDetails.ToString().Split('|')[0];
        //        string Infant = PriceSummuryDetails[0].InfantDetails.ToString().Split('|')[0];

        //        string Adultfare = PriceSummuryDetails[0].AdultDetails.ToString().Split('|')[1];
        //        string Childfare = PriceSummuryDetails[0].ChildDetails.ToString().Split('|')[1];
        //        string Infantfare = PriceSummuryDetails[0].InfantDetails.ToString().Split('|')[1];

        //        string AdultfareTax = PriceSummuryDetails[0].AdultDetails.ToString().Split('|')[2];
        //        string ChildfareTax = PriceSummuryDetails[0].ChildDetails.ToString().Split('|')[2];
        //        string InfantfareTax = PriceSummuryDetails[0].InfantDetails.ToString().Split('|')[2];

        //        string AdultfareMarkup = PriceSummuryDetails[0].AdultDetails.ToString().Split('|')[3];
        //        string ChildfareMarkup = PriceSummuryDetails[0].ChildDetails.ToString().Split('|')[3];
        //        string InfantfareMarkup = PriceSummuryDetails[0].InfantDetails.ToString().Split('|')[3];

        //        string SqlQuery1 = "UPDATE FlightBooking SET FlightClass='" + FlightClass + "',FlightWay='" + FlightWay + "',SupportPlan='" + SupportPlan + "',SupportCost=" + suppurtPlanAmt + ",BaggageCost=" + totBagPrice + ",BaggageSize='" + BaggagePlan + "', Adult=" + Adult + ",Child=" + Child + ",Infant=" + Infant + ",AdultFare=" + Adultfare + ",AdutlTax=" + AdultfareTax + ",AdultMarkup=" + AdultfareMarkup + ",ChildFare=" + Childfare + ",ChildTax=" + ChildfareTax + ",ChildMarkup=" + ChildfareMarkup + ",InfantFare=" + Infantfare + ",InfantTax=" + InfantfareTax + ",InfantMarkup=" + InfantfareMarkup + " WHERE BookingID=" + BookingID;



        //        int cd = SqlData.Data.SqlHelperData.ExecuteNonQuery(Common.ConnectionString, CommandType.Text, SqlQuery1);
        //        //            string SqlQuery = "INSERT INTO bookings " +
        //        //"(name, phone, email_id, website, source_station, destination, departure_date, return_date, " +
        //        //"cabin_class, adult, children, infrants, airline, total_fare, request_url, add_card) " +
        //        //"VALUES ('" + billPCardName + "','" + phoneNumber + "','" + Emailval + "','" + "rsmyfly" + "','" +
        //        //newFlightsSearch.From.Code + "','" + newFlightsSearch.To.Code + "','" + depdate + "','" + retDate + "','" +
        //        //FlightClass + "'," + Adult + "," + Child + "," + Infant + ",'" + "No_Name currently " + "'," +
        //        //TotalFinalCost + ",'" + "" + "','" + CArdinfo + "')";
        //        //            Int64 BookingID = SqlData.Data.SqlHelperData.AddToTableReturnID(TravelWeb.Utilities.Common.ConnectionString, SqlQuery);


        //        //string SqlQuery1 = "UPDATE FlightBookings SET FlightClass='" + FlightClass + "',FlightWay='" + FlightWay + "',SupportPlan='" + SupportPlan + "',SupportCost=" + suppurtPlanAmt + ",BaggageCost=" + totBagPrice + ",BaggageSize='" + BaggagePlan + "', Adult=" + Adult + ",Child=" + Child + ",Infant=" + Infant + ",AdultFare=" + Adultfare + ",AdutlTax=" + AdultfareTax + ",AdultMarkup=" + AdultfareMarkup + ",ChildFare=" + Childfare + ",ChildTax=" + ChildfareTax + ",ChildMarkup=" + ChildfareMarkup + ",InfantFare=" + Infantfare + ",InfantTax=" + InfantfareTax + ",InfantMarkup=" + InfantfareMarkup + " WHERE BookingID=" + BookingID;



        //        //int cd = SqlData.Data.SqlHelperData.ExecuteNonQuery(Common.ConnectionString, CommandType.Text, SqlQuery1);

        //        //string statusQuery = "INSERT INTO BookingStatus (BookingID,Status,UserID,DateCreated) ";
        //        //statusQuery += " VALUES(" + BookingID + ",'Pending',0,getdate()";
        //        // string bookingno = "B" + DateTime.Now.ToString("yyMMddHHmmssff");
        //        //string SqlQuery = "INSERT INTO FlightBookings (BookingNumber,BookingDate,FromDeparture,ToDestination,DepartDate,ReturnDate,ContactPerson,ContactEmail,BillAddress1,BillAddress2,BillPostalCode,BillCity,BillState,BillCountry,nett,CardCharge,Gross,Comments) ";
        //        //SqlQuery += " VALUES('" + bookingno + "',getdate(),'" + newFlightsSearch.From.Code + "','" + newFlightsSearch.To.Code + "','" + Convert.ToDateTime(newFlightsSearch.DepDate.ToString()).ToString("dd-MMM") + "','" + Convert.ToDateTime(newFlightsSearch.RetDate.ToString()).ToString("dd-MMM") + "','" + phoneNumber + "','" + Emailval + "','" + billAddress1 + "','" + billAddress2 + "','" + billCity + "','" + billPostalCode + "','" + billState + "','" + billCountry + "'," + newFlights.TotalCost + ",0," + newFlights.TotalCost + ",'')";

        //        //Int64 BookingID = SqlData.Data.SqlHelperData.AddToTableReturnID(TravelWeb.Utilities.Common.ConnectionString, SqlQuery);
        //        //foreach (FlightsSegment seg in outbound)
        //        //{
        //        //    string type = "OutBound";
        //        //    string SQLQuery = "INSERT INTO[dbo].[BookingFlghtDetails]([BookingID],[DepartDate],[ArrivalDate],[FlightNo],[Airline],[DepartOn],[Dcode],[Gcode],[ArriveOn],[FlightType],[DepDate],[ArrDate],[BoundClass])";
        //        //    SQLQuery += "VALUES(" + BookingID + ",'" + seg.DepartureDate.ToString("dd-MMM") + "','" + seg.ArrivalDate.ToString("dd-MMM") + "','" + seg.FlightNo + "'";
        //        //    SQLQuery += ",'" + seg.Airline.Code + "','" + seg.DepartureDate.ToString("HH:mm") + "','" + seg.FromAirport.Code + "','" + seg.ToAirport.Code + "','" + seg.ArrivalDate.ToString("HH:mm") + "','" + type + "','" + seg.DepartureDate + "','" + seg.ArrivalDate + "','" + OutboundClass + "')";

        //        //    int ck = SqlData.Data.SqlHelperData.ExecuteNonQuery(TravelWeb.Utilities.Common.ConnectionString, CommandType.Text, SQLQuery);
        //        //}
        //        //if (inbound.Count > 0)
        //        //{
        //        //    foreach (FlightsSegment seg in inbound)
        //        //    {
        //        //        string type = "InBound";
        //        //        string SQLQuery = "INSERT INTO[dbo].[BookingFlghtDetails]([BookingID],[DepartDate],[ArrivalDate],[FlightNo],[Airline],[DepartOn],[Dcode],[Gcode],[ArriveOn],[FlightType],[DepDate],[ArrDate],[BoundClass])";
        //        //        SQLQuery += "VALUES(" + BookingID + ",'" + seg.DepartureDate.ToString("dd-MMM") + "','" + seg.ArrivalDate.ToString("dd-MMM") + "','" + seg.FlightNo + "'";
        //        //        SQLQuery += ",'" + seg.Airline.Code + "','" + seg.DepartureDate.ToString("HH:mm") + "','" + seg.FromAirport.Code + "','" + seg.ToAirport.Code + "','" + seg.ArrivalDate.ToString("HH:mm") + "','" + type + "','" + seg.DepartureDate + "','" + seg.ArrivalDate + "','" + InboundClass + "')";

        //        //        int ck = SqlData.Data.SqlHelperData.ExecuteNonQuery(TravelWeb.Utilities.Common.ConnectionString, CommandType.Text, SQLQuery);
        //        //    }
        //        //}
        //        foreach (Passenger pax in lps)
        //        {
        //            string SQLQuery = "INSERT INTO[dbo].[BookingPaxDetails] ([BookingID],[PaxTitle],[PaxType],[FirstName],[LastName],[DOB],[MiddleName])";
        //            SQLQuery += "VALUES(" + BookingID + ", '" + pax.Title + "', '" + pax.PaxType + "', '" + pax.FirstName + "', '" + pax.LastName + "', '" + pax.DateOfBirth + "', '" + pax.MiddleName + "')";
        //            int ck = SqlData.Data.SqlHelperData.ExecuteNonQuery(TravelWeb.Utilities.Common.ConnectionString, CommandType.Text, SQLQuery);
        //        }

        //        return Json(BookingID);

        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }

        //    // Here you can store datatable value in database

        //}

        //public JsonResult ManageLeads(string FlightClass, string MarkupType, string MarkupValue, string InboundClass, string OutboundClass, string FlightWay, string SupportPlan, string suppurtPlanAmt, string BaggagePlan, string totBagPrice, string PriceSummury, string AdultData, string ChildtData, string InfantData, string phoneNumber, string Emailval, string billAddress1, string billAddress2, string billPostalCode, string billCity, string billState, string billCountry, string billPCardName, string CardNumber, string CardValMonth, string CardvalYear, string CardSecrutyCode, string CardType, string TotalFinalCost, string IPAddress, bool isMainCabinChecked, string SeatPrefrenceAmount, string totalSeatPrefrenceAmount)
        //{
        //    //Saving Leads in Booking
        //    try
        //    {

        //        dynamic AdultDetails = JsonConvert.DeserializeObject(AdultData);
        //        dynamic ChildDetails = JsonConvert.DeserializeObject(ChildtData);
        //        dynamic InfantDetails = JsonConvert.DeserializeObject(InfantData);

        //        //HttpContext.Session.SetObjectAsJson("Flights", fls);

        //        Flights newFlights = HttpContext.Session.GetObjectFromJson<Flights>("Flights");
        //        FlightsSearch newFlightsSearch = HttpContext.Session.GetObjectFromJson<FlightsSearch>("FlightsSearch");
        //        dynamic PriceSummuryDetails = JsonConvert.DeserializeObject(PriceSummury);

        //        List<FlightsSegment> outbound = newFlights.Outbound;
        //        List<FlightsSegment> inbound = newFlights.Inbound;

        //        DataTable dtPax = new DataTable();
        //        dtPax.Columns.Add("FirstName", typeof(string));
        //        dtPax.Columns.Add("LastName", typeof(string));
        //        dtPax.Columns.Add("MiddleName", typeof(string));
        //        dtPax.Columns.Add("DateOfBirth", typeof(string));
        //        dtPax.Columns.Add("Gender", typeof(string));
        //        dtPax.Columns.Add("PaxType", typeof(string));

        //        foreach (var adlt in AdultDetails)
        //        {
        //            DataRow drad = dtPax.NewRow();
        //            drad["DateOfBirth"] = adlt["DateOfBirth"].ToString();
        //            drad["Gender"] = adlt["Gender"].ToString();
        //            drad["FirstName"] = adlt["FirstName"].ToString();
        //            drad["LastName"] = adlt["LastName"].ToString();
        //            drad["MiddleName"] = adlt["MiddleName"].ToString();
        //            drad["PaxType"] = "Adult";
        //            dtPax.Rows.Add(drad);
        //        }
        //        foreach (var chld in ChildDetails)
        //        {
        //            DataRow drch = dtPax.NewRow();
        //            drch["DateOfBirth"] = chld["DateOfBirth"].ToString();
        //            drch["Gender"] = chld["Gender"].ToString();
        //            drch["FirstName"] = chld["FirstName"].ToString();
        //            drch["LastName"] = chld["LastName"].ToString();
        //            drch["MiddleName"] = chld["MiddleName"].ToString();
        //            drch["PaxType"] = "Child";
        //            dtPax.Rows.Add(drch);
        //        }
        //        foreach (var infnt in InfantDetails)
        //        {
        //            DataRow drinf = dtPax.NewRow();
        //            drinf["DateOfBirth"] = infnt["DateOfBirth"].ToString();
        //            drinf["Gender"] = infnt["Gender"].ToString();
        //            drinf["FirstName"] = infnt["FirstName"].ToString();
        //            drinf["LastName"] = infnt["LastName"].ToString();
        //            drinf["MiddleName"] = infnt["MiddleName"].ToString();
        //            drinf["PaxType"] = "Infant";
        //            dtPax.Rows.Add(drinf);
        //        }
        //        List<Passenger> lps = new List<Passenger>();

        //        foreach (DataRow drp in dtPax.Rows)
        //        {
        //            Passenger psg = new Passenger();
        //            psg.FirstName = drp["FirstName"].ToString();
        //            psg.LastName = drp["LastName"].ToString();
        //            psg.MiddleName = drp["MiddleName"].ToString();
        //            psg.DateOfBirth = drp["DateOfBirth"].ToString();
        //            psg.Gender = drp["Gender"].ToString();
        //            psg.PaxType = drp["PaxType"].ToString();
        //            lps.Add(psg);
        //        }
        //        string paxfullname = dtPax.Rows[0]["FirstName"].ToString() + " " + dtPax.Rows[0]["MiddleName"] + "" + dtPax.Rows[0]["LastName"].ToString();
        //        string bookingno = GetBookingNumber();
        //        //string bookingno = "B" + DateTime.Now.ToString("yyMMddHHmmssff");


        //        string subject = newFlightsSearch.From.CityCode + " " + newFlightsSearch.To.CityCode;
        //        if (newFlightsSearch.FlightWay.ToString() == "RoundTrip")
        //        {
        //            subject += " RETURN " + Convert.ToDateTime(newFlightsSearch.DepDate.ToString()).ToString("MMM-dd-yyyy") + " " + Convert.ToDateTime(newFlightsSearch.RetDate.ToString()).ToString("MMM-dd-yyyy");
        //        }
        //        else
        //        {
        //            subject += " ONEWAY " + Convert.ToDateTime(newFlightsSearch.DepDate.ToString()).ToString("MMM-dd-yyyy");
        //        }
        //        if (Convert.ToInt32(newFlightsSearch.Adult) > 0)
        //        {
        //            subject += "  " + newFlightsSearch.Adult + " ADULT ";
        //        }
        //        if (Convert.ToInt32(newFlightsSearch.Child) > 0)
        //        {
        //            subject += newFlightsSearch.Child + " CHILD ";
        //        }
        //        if (Convert.ToInt32(newFlightsSearch.Infant) > 0)
        //        {
        //            subject += newFlightsSearch.Infant + " INFANT";
        //        }

        //        string depdate = (newFlightsSearch.DepDate.ToString().Contains("0001")) ? "" : Convert.ToDateTime(newFlightsSearch.DepDate.ToString()).ToString("dd-MMM");
        //        string retDate = (newFlightsSearch.RetDate.ToString().Contains("0001")) ? "" : Convert.ToDateTime(newFlightsSearch.RetDate.ToString()).ToString("dd-MMM");

        //        //string CArdinfo = "CardName :" + billPCardName + "\n";
        //        //CArdinfo += "CardType : " + CardType + "\n";
        //        //CArdinfo += "CardNumber : " + CardNumber + "\n";
        //        //CArdinfo += "CardValMonth : " + CardValMonth + " / " + CardvalYear + "\n";
        //        //CArdinfo += "CardSecCode : " + CardSecrutyCode + "\n";

        //        string CArdinfo = "";
        //        var paymentStatus = "Not Added";

        //        if (CardNumber != null)
        //        {
        //            paymentStatus = "Done";
        //        }
        //        // string bookingno = "B" + DateTime.Now.ToString("yyMMddHHmmssff");

        //        //comment by janesh _13oct25
        //        //string SqlQuery = "INSERT INTO FlightBookings (BookingNumber,BookingDate,FromDeparture,ToDestination,DepartDate,ReturnDate,ContactPerson,ContactEmail,BillAddress1,BillAddress2,BillPostalCode,BillCity,BillState,BillCountry,nett,CardCharge,Gross,Comments,[CardName],[CardType],[CardNumber],[CardValMonth],[CardValYear], [CardSecCode],[IPAddress],[StatusId], MarkupType, MarkupValue) ";
        //        //SqlQuery += " VALUES('" + bookingno + "',getdate(),'" + newFlightsSearch.From.Code + "','" + newFlightsSearch.To.Code + "','" + depdate + "','" + retDate + "','" + phoneNumber + "','" + Emailval + "','" + billAddress1 + "','" + billAddress2 + "','" + billPostalCode + "','" + billCity + "','" + billState + "','" + billCountry + "'," + TotalFinalCost + ",0," + TotalFinalCost + ",'" + CArdinfo + "','" + billPCardName + "','" + CardType + "','" + CardNumber + "','" + CardValMonth + "','" + 2022 + "','" + CardSecrutyCode + "','" + IPAddress + "',10,'" + MarkupType + "','" + MarkupValue + "')";
        //        string SqlQuery = "INSERT INTO FlightBooking (BookingNumber,BookingDate,FromDeparture,ToDestination,DepartDate,ReturnDate,ContactPerson,ContactEmail,BillAddress1,BillAddress2,BillPostalCode,BillCity,BillState,BillCountry,nett,CardCharge,Gross,Comments,[CardName],[CardType],[CardNumber],[CardValMonth],[CardValYear], [CardSecCode],[IPAddress],[StatusId], MarkupType, MarkupValue,PaymentType) ";
        //        SqlQuery += " VALUES('" + bookingno + "',getdate(),'" + newFlightsSearch.From.Code + "','" + newFlightsSearch.To.Code + "','" + depdate + "','" + retDate + "','" + phoneNumber + "','" + Emailval + "','" + billAddress1 + "','" + billAddress2 + "','" + billPostalCode + "','" + billCity + "','" + billState + "','" + billCountry + "'," + TotalFinalCost + ",0," + TotalFinalCost + ",'" + CArdinfo + "','" + billPCardName + "','" + CardType + "','" + CardNumber + "','" + CardValMonth + "','" + 2022 + "','" + CardSecrutyCode + "','" + IPAddress + "',10,'" + MarkupType + "','" + MarkupValue + "','" + paymentStatus + "')";

        //        Int64 BookingID = SqlData.Data.SqlHelperData.AddToTableReturnID(TravelWeb.Utilities.Common.ConnectionString, SqlQuery);



        //        string Adult = PriceSummuryDetails[0].AdultDetails.ToString().Split('|')[0];
        //        string Child = PriceSummuryDetails[0].ChildDetails.ToString().Split('|')[0];
        //        string Infant = PriceSummuryDetails[0].InfantDetails.ToString().Split('|')[0];

        //        string Adultfare = PriceSummuryDetails[0].AdultDetails.ToString().Split('|')[1];
        //        string Childfare = PriceSummuryDetails[0].ChildDetails.ToString().Split('|')[1];
        //        string Infantfare = PriceSummuryDetails[0].InfantDetails.ToString().Split('|')[1];

        //        string AdultfareTax = PriceSummuryDetails[0].AdultDetails.ToString().Split('|')[2];
        //        string ChildfareTax = PriceSummuryDetails[0].ChildDetails.ToString().Split('|')[2];
        //        string InfantfareTax = PriceSummuryDetails[0].InfantDetails.ToString().Split('|')[2];

        //        string AdultfareMarkup = PriceSummuryDetails[0].AdultDetails.ToString().Split('|')[3];
        //        string ChildfareMarkup = PriceSummuryDetails[0].ChildDetails.ToString().Split('|')[3];
        //        string InfantfareMarkup = PriceSummuryDetails[0].InfantDetails.ToString().Split('|')[3];

        //        string SqlQuery1 = "UPDATE FlightBooking SET FlightClass='" + FlightClass + "',FlightWay='" + FlightWay + "',SupportPlan='" + SupportPlan + "',SupportCost=" + suppurtPlanAmt + ",BaggageCost=" + totBagPrice + ",BaggageSize='" + BaggagePlan + "', Adult=" + Adult + ",Child=" + Child + ",Infant=" + Infant + ",AdultFare=" + Adultfare + ",AdutlTax=" + AdultfareTax + ",AdultMarkup=" + AdultfareMarkup + ",ChildFare=" + Childfare + ",ChildTax=" + ChildfareTax + ",ChildMarkup=" + ChildfareMarkup + ",InfantFare=" + Infantfare + ",InfantTax=" + InfantfareTax + ",InfantMarkup=" + InfantfareMarkup + " WHERE BookingID=" + BookingID;



        //        int cd = SqlData.Data.SqlHelperData.ExecuteNonQuery(Common.ConnectionString, CommandType.Text, SqlQuery1);
        //        //            string SqlQuery = "INSERT INTO bookings " +
        //        //"(name, phone, email_id, website, source_station, destination, departure_date, return_date, " +
        //        //"cabin_class, adult, children, infrants, airline, total_fare, request_url, add_card) " +
        //        //"VALUES ('" + billPCardName + "','" + phoneNumber + "','" + Emailval + "','" + "rsmyfly" + "','" +
        //        //newFlightsSearch.From.Code + "','" + newFlightsSearch.To.Code + "','" + depdate + "','" + retDate + "','" +
        //        //FlightClass + "'," + Adult + "," + Child + "," + Infant + ",'" + "No_Name currently " + "'," +
        //        //TotalFinalCost + ",'" + "" + "','" + CArdinfo + "')";
        //        //            Int64 BookingID = SqlData.Data.SqlHelperData.AddToTableReturnID(TravelWeb.Utilities.Common.ConnectionString, SqlQuery);


        //        //string SqlQuery1 = "UPDATE FlightBookings SET FlightClass='" + FlightClass + "',FlightWay='" + FlightWay + "',SupportPlan='" + SupportPlan + "',SupportCost=" + suppurtPlanAmt + ",BaggageCost=" + totBagPrice + ",BaggageSize='" + BaggagePlan + "', Adult=" + Adult + ",Child=" + Child + ",Infant=" + Infant + ",AdultFare=" + Adultfare + ",AdutlTax=" + AdultfareTax + ",AdultMarkup=" + AdultfareMarkup + ",ChildFare=" + Childfare + ",ChildTax=" + ChildfareTax + ",ChildMarkup=" + ChildfareMarkup + ",InfantFare=" + Infantfare + ",InfantTax=" + InfantfareTax + ",InfantMarkup=" + InfantfareMarkup + " WHERE BookingID=" + BookingID;



        //        //int cd = SqlData.Data.SqlHelperData.ExecuteNonQuery(Common.ConnectionString, CommandType.Text, SqlQuery1);

        //        //string statusQuery = "INSERT INTO BookingStatus (BookingID,Status,UserID,DateCreated) ";
        //        //statusQuery += " VALUES(" + BookingID + ",'Pending',0,getdate()";
        //        // string bookingno = "B" + DateTime.Now.ToString("yyMMddHHmmssff");
        //        //string SqlQuery = "INSERT INTO FlightBookings (BookingNumber,BookingDate,FromDeparture,ToDestination,DepartDate,ReturnDate,ContactPerson,ContactEmail,BillAddress1,BillAddress2,BillPostalCode,BillCity,BillState,BillCountry,nett,CardCharge,Gross,Comments) ";
        //        //SqlQuery += " VALUES('" + bookingno + "',getdate(),'" + newFlightsSearch.From.Code + "','" + newFlightsSearch.To.Code + "','" + Convert.ToDateTime(newFlightsSearch.DepDate.ToString()).ToString("dd-MMM") + "','" + Convert.ToDateTime(newFlightsSearch.RetDate.ToString()).ToString("dd-MMM") + "','" + phoneNumber + "','" + Emailval + "','" + billAddress1 + "','" + billAddress2 + "','" + billCity + "','" + billPostalCode + "','" + billState + "','" + billCountry + "'," + newFlights.TotalCost + ",0," + newFlights.TotalCost + ",'')";

        //        //Int64 BookingID = SqlData.Data.SqlHelperData.AddToTableReturnID(TravelWeb.Utilities.Common.ConnectionString, SqlQuery);
        //        //foreach (FlightsSegment seg in outbound)
        //        //{
        //        //    string type = "OutBound";
        //        //    string SQLQuery = "INSERT INTO[dbo].[BookingFlghtDetails]([BookingID],[DepartDate],[ArrivalDate],[FlightNo],[Airline],[DepartOn],[Dcode],[Gcode],[ArriveOn],[FlightType],[DepDate],[ArrDate],[BoundClass])";
        //        //    SQLQuery += "VALUES(" + BookingID + ",'" + seg.DepartureDate.ToString("dd-MMM") + "','" + seg.ArrivalDate.ToString("dd-MMM") + "','" + seg.FlightNo + "'";
        //        //    SQLQuery += ",'" + seg.Airline.Code + "','" + seg.DepartureDate.ToString("HH:mm") + "','" + seg.FromAirport.Code + "','" + seg.ToAirport.Code + "','" + seg.ArrivalDate.ToString("HH:mm") + "','" + type + "','" + seg.DepartureDate + "','" + seg.ArrivalDate + "','" + OutboundClass + "')";

        //        //    int ck = SqlData.Data.SqlHelperData.ExecuteNonQuery(TravelWeb.Utilities.Common.ConnectionString, CommandType.Text, SQLQuery);
        //        //}
        //        //if (inbound.Count > 0)
        //        //{
        //        //    foreach (FlightsSegment seg in inbound)
        //        //    {
        //        //        string type = "InBound";
        //        //        string SQLQuery = "INSERT INTO[dbo].[BookingFlghtDetails]([BookingID],[DepartDate],[ArrivalDate],[FlightNo],[Airline],[DepartOn],[Dcode],[Gcode],[ArriveOn],[FlightType],[DepDate],[ArrDate],[BoundClass])";
        //        //        SQLQuery += "VALUES(" + BookingID + ",'" + seg.DepartureDate.ToString("dd-MMM") + "','" + seg.ArrivalDate.ToString("dd-MMM") + "','" + seg.FlightNo + "'";
        //        //        SQLQuery += ",'" + seg.Airline.Code + "','" + seg.DepartureDate.ToString("HH:mm") + "','" + seg.FromAirport.Code + "','" + seg.ToAirport.Code + "','" + seg.ArrivalDate.ToString("HH:mm") + "','" + type + "','" + seg.DepartureDate + "','" + seg.ArrivalDate + "','" + InboundClass + "')";

        //        //        int ck = SqlData.Data.SqlHelperData.ExecuteNonQuery(TravelWeb.Utilities.Common.ConnectionString, CommandType.Text, SQLQuery);
        //        //    }
        //        //}
        //        foreach (Passenger pax in lps)
        //        {
        //            string SQLQuery = "INSERT INTO[dbo].[BookingPaxDetails] ([BookingID],[PaxTitle],[PaxType],[FirstName],[LastName],[DOB],[MiddleName])";
        //            SQLQuery += "VALUES(" + BookingID + ", '" + pax.Title + "', '" + pax.PaxType + "', '" + pax.FirstName + "', '" + pax.LastName + "', '" + pax.DateOfBirth + "', '" + pax.MiddleName + "')";
        //            int ck = SqlData.Data.SqlHelperData.ExecuteNonQuery(TravelWeb.Utilities.Common.ConnectionString, CommandType.Text, SQLQuery);
        //        }

        //        return Json(BookingID);

        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }

        //    // Here you can store datatable value in database

        //}

        //        [HttpPost]
        //        public JsonResult V1SaveData(string FlightClass, string MarkupType, string MarkupValue, string InboundClass, string OutboundClass, string FlightWay, string SupportPlan, string suppurtPlanAmt, string BaggagePlan, string totBagPrice, string PriceSummury, string AdultData, string ChildtData, string InfantData, string phoneNumber, string Emailval, string billAddress1, string billAddress2, string billPostalCode, string billCity, string billState, string billCountry, string billPCardName, string CardNumber, string CardValMonth, string CardvalYear, string CardSecrutyCode, string CardType, string TotalFinalCost, string IPAddress, string BookingId, string Qs, bool isMainCabinChecked, string SeatPrefrenceAmount, string totalSeatPrefrenceAmount, string TotalMainCabinPrice, string SearchId, string FlightId)
        //        {
        //            var context = new RSFLY();
        //            var PaymentType = "Not_Filled";
        //            if (CardNumber != null)
        //            {
        //                PaymentType = "Filled";
        //            }
        //            //fetch current flight detail from DB
        //            //DataTable dtFlight = SqlData.Data.SqlHelperData.ExecuteDataset(Common.ConnectionString, CommandType.Text, "SELECT * FROM FlightsSearch_New Where SearchId='" + SearchId + "'").Tables[0];
        //            //List<FlightsResult> lstFlight = new List<FlightsResult>();
        //            //FlightsResult obj = new FlightsResult();
        //            //var XMLString = dtFlight.Rows[0].ItemArray[1].ToString();
        //            //FlightsResult flightSearch = JsonConvert.DeserializeObject<FlightsResult>(XMLString);
        //            ////End


        //            var val = Common.GetObjectSession<FlightsResult>("search_" + SearchId);
        //            //var val = AplicationHelper.Get("localhost", SearchId);
        //            FlightsResult flightSearch = val;

        //            Flights newFlights = flightSearch.FlightsList.Where(x => x.FlightId == FlightId).FirstOrDefault();
        //            FlightsSearch newFlightsSearch = flightSearch.FlightsSearch;


        //            try
        //            {
        //                if (Convert.ToInt32(BookingId) > 0)
        //                {

        //                    dynamic AdultDetails = JsonConvert.DeserializeObject(AdultData);
        //                    dynamic ChildDetails = JsonConvert.DeserializeObject(ChildtData);
        //                    dynamic InfantDetails = JsonConvert.DeserializeObject(InfantData);


        //                    //Code comented by Sunit
        //                    //Resolve session timeout issue now fetching data from based on searchID

        //                    /*Flights newFlights = HttpContext.Session.GetObjectFromJson<Flights>("Flights");

        //                     if (newFlights == null)
        //                     {
        //                         return Json(Qs);
        //                     }
        //                    FlightsSearch newFlightsSearch = HttpContext.Session.GetObjectFromJson<FlightsSearch>("FlightsSearch");

        //                    if (newFlightsSearch == null)
        //                     {
        //                         return Json(Qs);
        //                     }*/

        //                    //End


        //                    dynamic PriceSummuryDetails = JsonConvert.DeserializeObject(PriceSummury);

        //                    List<FlightsSegment> outbound = newFlights.Outbound;
        //                    List<FlightsSegment> inbound = newFlights.Inbound;

        //                    DataTable dtPax = new DataTable();
        //                    dtPax.Columns.Add("FirstName", typeof(string));
        //                    dtPax.Columns.Add("LastName", typeof(string));
        //                    dtPax.Columns.Add("MiddleName", typeof(string));
        //                    dtPax.Columns.Add("DateOfBirth", typeof(string));
        //                    dtPax.Columns.Add("Gender", typeof(string));
        //                    dtPax.Columns.Add("PaxType", typeof(string));

        //                    foreach (var adlt in AdultDetails)
        //                    {
        //                        DataRow drad = dtPax.NewRow();
        //                        drad["DateOfBirth"] = adlt["DateOfBirth"].ToString();
        //                        drad["Gender"] = adlt["Gender"].ToString();
        //                        drad["FirstName"] = adlt["FirstName"].ToString();
        //                        drad["LastName"] = adlt["LastName"].ToString();
        //                        drad["MiddleName"] = adlt["MiddleName"].ToString();
        //                        drad["PaxType"] = "Adult";
        //                        dtPax.Rows.Add(drad);
        //                    }
        //                    foreach (var chld in ChildDetails)
        //                    {
        //                        DataRow drch = dtPax.NewRow();
        //                        drch["DateOfBirth"] = chld["DateOfBirth"].ToString();
        //                        drch["Gender"] = chld["Gender"].ToString();
        //                        drch["FirstName"] = chld["FirstName"].ToString();
        //                        drch["LastName"] = chld["LastName"].ToString();
        //                        drch["MiddleName"] = chld["MiddleName"].ToString();
        //                        drch["PaxType"] = "Child";
        //                        dtPax.Rows.Add(drch);
        //                    }
        //                    foreach (var infnt in InfantDetails)
        //                    {
        //                        DataRow drinf = dtPax.NewRow();
        //                        drinf["DateOfBirth"] = infnt["DateOfBirth"].ToString();
        //                        drinf["Gender"] = infnt["Gender"].ToString();
        //                        drinf["FirstName"] = infnt["FirstName"].ToString();
        //                        drinf["LastName"] = infnt["LastName"].ToString();
        //                        drinf["MiddleName"] = infnt["MiddleName"].ToString();
        //                        drinf["PaxType"] = "Infant";
        //                        dtPax.Rows.Add(drinf);
        //                    }
        //                    List<Passenger> lps = new List<Passenger>();

        //                    foreach (DataRow drp in dtPax.Rows)
        //                    {
        //                        Passenger psg = new Passenger();
        //                        psg.FirstName = drp["FirstName"].ToString();
        //                        psg.LastName = drp["LastName"].ToString();
        //                        psg.MiddleName = drp["MiddleName"].ToString();
        //                        psg.DateOfBirth = drp["DateOfBirth"].ToString();
        //                        psg.Gender = drp["Gender"].ToString();
        //                        psg.PaxType = drp["PaxType"].ToString();
        //                        lps.Add(psg);
        //                    }

        //                    string paxfullname = dtPax.Rows[0]["FirstName"].ToString() + " " + dtPax.Rows[0]["MiddleName"] + " " + dtPax.Rows[0]["LastName"].ToString();
        //                    long bId = Convert.ToInt64(BookingId);
        //                    string bookingno = context.BookingsB2cs.Where(x => x.BookingId == bId).Select(x => x.RandomNo).FirstOrDefault();

        //                    //string bookingno = context.FlightBookings.Where(x => x.BookingId == bId).Select(x => x.BookingNumber).FirstOrDefault();
        //                    //string bookingno = "B" + DateTime.Now.ToString("yyMMddHHmmssff");


        //                    AcknowledgeData ack = new AcknowledgeData();
        //                    ack.Reference = bookingno;
        //                    ack.PassengerName = paxfullname;
        //                    //ack.PaidTo = "RushMyFlight Travel";
        //                    ack.PaidTo = "Pending";

        //                    ack.PaymentMethod = CardType;
        //                    ack.TravelDate = Convert.ToDateTime(newFlightsSearch.DepDate.ToString()).ToString("MMM-dd-yyyy HH:mm");
        //                    string subject = newFlightsSearch.From.CityCode + " " + newFlightsSearch.To.CityCode;
        //                    if (newFlightsSearch.FlightWay.ToString() == "RoundTrip")
        //                    {
        //                        subject += " RETURN " + Convert.ToDateTime(newFlightsSearch.DepDate.ToString()).ToString("MMM-dd-yyyy") + " " + Convert.ToDateTime(newFlightsSearch.RetDate.ToString()).ToString("MMM-dd-yyyy");
        //                    }
        //                    else
        //                    {
        //                        subject += " ONEWAY " + Convert.ToDateTime(newFlightsSearch.DepDate.ToString()).ToString("MMM-dd-yyyy");
        //                    }
        //                    if (Convert.ToInt32(newFlightsSearch.Adult) > 0)
        //                    {
        //                        subject += "  " + newFlightsSearch.Adult + " ADULT ";
        //                    }
        //                    if (Convert.ToInt32(newFlightsSearch.Child) > 0)
        //                    {
        //                        subject += newFlightsSearch.Child + " CHILD ";
        //                    }
        //                    if (Convert.ToInt32(newFlightsSearch.Infant) > 0)
        //                    {
        //                        subject += newFlightsSearch.Infant + " INFANT";
        //                    }

        //                    ack.Subject = subject;
        //                    //ack.AmountPaid = newFlights.TotalCost.ToString("f2");
        //                    ack.AmountPaid = TotalFinalCost.ToString();


        //                    HttpContext.Session.SetObjectAsJson("AcknowledgeData", ack);



        //                    string depdate = (newFlightsSearch.DepDate.ToString().Contains("0001")) ? "" : Convert.ToDateTime(newFlightsSearch.DepDate.ToString()).ToString("dd-MMM");
        //                    string retDate = (newFlightsSearch.RetDate.ToString().Contains("0001")) ? "" : Convert.ToDateTime(newFlightsSearch.RetDate.ToString()).ToString("dd-MMM");
        //                    //Comment it now showing Cardinfo and Comment simply

        //                    //string CArdinfo = "CardName :" + billPCardName + "\n";
        //                    //CArdinfo += "CardType : " + CardType + "\n";
        //                    //CArdinfo += "CardNumber : " + CardNumber + "\n";
        //                    //CArdinfo += "CardValMonth : " + CardValMonth + " / " + CardvalYear + "\n";
        //                    //CArdinfo += "CardSecCode : " + CardSecrutyCode + "\n";

        //                    string CArdinfo = "";


        //                    // string bookingno = "B" + DateTime.Now.ToString("yyMMddHHmmssff");


        //                    //string SqlQuery = "INSERT INTO FlightBookings (BookingNumber,BookingDate,FromDeparture,ToDestination,DepartDate,ReturnDate,ContactPerson,ContactEmail,BillAddress1,BillAddress2,BillPostalCode,BillCity,BillState,BillCountry,nett,CardCharge,Gross,Comments,[CardName],[CardType],[CardNumber],[CardValMonth],[CardValYear], [CardSecCode],[IPAddress],[StatusId]) ";
        //                    //SqlQuery += " VALUES('" + bookingno + "',getdate(),'" + newFlightsSearch.From.Code + "','" + newFlightsSearch.To.Code + "','" + depdate + "','" + retDate + "','" + phoneNumber + "','" + Emailval + "','" + billAddress1 + "','" + billAddress2 + "','" + billPostalCode + "','" + billCity + "','" + billState + "','" + billCountry + "'," + TotalFinalCost + ",0," + TotalFinalCost + ",'" + CArdinfo + "','" + billPCardName + "','" + CardType + "','" + CardNumber + "','" + CardValMonth + "','" + CardvalYear + "','" + CardSecrutyCode + "','" + IPAddress + "',1)";

        //                    string SqlQuery = "UPDATE FlightBookings SET BookingNumber='" + bookingno + "',FromDeparture='" + newFlightsSearch.From.Code + "',ToDestination='" + newFlightsSearch.To.Code + "',DepartDate='" + depdate + "',ReturnDate='" + retDate + "', ContactPerson='" + phoneNumber + "',ContactEmail='" + Emailval +
        //                    "',BillAddress1='" + billAddress1 + "',BillAddress2='" + billAddress2 + "',BillPostalCode='" + billPostalCode + "',BillCity='" + billCity + "',BillState='" + billState + "',BillCountry='" + billCountry + "',nett='" + TotalFinalCost + "',CardCharge='" + 0 + "',Gross='" + TotalFinalCost + "',Comments='" + CArdinfo + "',CardName='" + billPCardName + "',CardType='" + CardType + "',CardNumber='" + CardNumber + "',CardValMonth='" + CardValMonth + "',CardValYear='" + CardvalYear + "',CardSecCode='" + CardSecrutyCode + "',PaymentType='" + PaymentType + "',StatusId='" + 1 + "' WHERE BookingID=" + bId;



        //                    int cd = SqlData.Data.SqlHelperData.ExecuteNonQuery(Common.ConnectionString, CommandType.Text, SqlQuery);
        //                    //Int64 BookingID = SqlData.Data.SqlHelperData.AddToTableReturnID(TravelWeb.Utilities.Common.ConnectionString, SqlQuery);

        //                    string Adult = PriceSummuryDetails[0].AdultDetails.ToString().Split('|')[0];
        //                    string Child = PriceSummuryDetails[0].ChildDetails.ToString().Split('|')[0];
        //                    string Infant = PriceSummuryDetails[0].InfantDetails.ToString().Split('|')[0];

        //                    string Adultfare = PriceSummuryDetails[0].AdultDetails.ToString().Split('|')[1];
        //                    string Childfare = PriceSummuryDetails[0].ChildDetails.ToString().Split('|')[1];
        //                    string Infantfare = PriceSummuryDetails[0].InfantDetails.ToString().Split('|')[1];

        //                    string AdultfareTax = PriceSummuryDetails[0].AdultDetails.ToString().Split('|')[2];
        //                    string ChildfareTax = PriceSummuryDetails[0].ChildDetails.ToString().Split('|')[2];
        //                    string InfantfareTax = PriceSummuryDetails[0].InfantDetails.ToString().Split('|')[2];

        //                    string AdultfareMarkup = PriceSummuryDetails[0].AdultDetails.ToString().Split('|')[3];
        //                    string ChildfareMarkup = PriceSummuryDetails[0].ChildDetails.ToString().Split('|')[3];
        //                    string InfantfareMarkup = PriceSummuryDetails[0].InfantDetails.ToString().Split('|')[3];



        //                    if (isMainCabinChecked == true)
        //                    {
        //                        TotalMainCabinPrice = TotalMainCabinPrice;
        //                    }
        //                    else
        //                    {
        //                        TotalMainCabinPrice = "0";
        //                    }


        //                    string SqlQuery1 = "UPDATE FlightBookings SET  SeatPrefAmount='" + TotalMainCabinPrice + "',FlightClass='" + FlightClass + "',FlightWay='" + FlightWay + "',SupportPlan='" + SupportPlan + "',SupportCost=" + suppurtPlanAmt + ",BaggageCost=" + totBagPrice + ",BaggageSize='" + BaggagePlan + "', " +
        //                        //"Adult=" + Adult + ",Child=" + Child + ",Infant=" + Infant + "," +
        //                        "AdultFare=" + Adultfare + ",AdutlTax=" + AdultfareTax + ",AdultMarkup=" + AdultfareMarkup + ",ChildFare=" + Childfare + ",ChildTax=" + ChildfareTax + ",ChildMarkup=" + ChildfareMarkup + ",InfantFare=" + Infantfare + ",InfantTax=" + InfantfareTax + ",InfantMarkup=" + InfantfareMarkup + " WHERE BookingID=" + bId;



        //                    int cds = SqlData.Data.SqlHelperData.ExecuteNonQuery(Common.ConnectionString, CommandType.Text, SqlQuery1);

        //                    //string statusQuery = "INSERT INTO BookingStatus (BookingID,Status,UserID,DateCreated) ";
        //                    //statusQuery += " VALUES(" + BookingID + ",'Pending',0,getdate()";
        //                    // string bookingno = "B" + DateTime.Now.ToString("yyMMddHHmmssff");
        //                    //string SqlQuery = "INSERT INTO FlightBookings (BookingNumber,BookingDate,FromDeparture,ToDestination,DepartDate,ReturnDate,ContactPerson,ContactEmail,BillAddress1,BillAddress2,BillPostalCode,BillCity,BillState,BillCountry,nett,CardCharge,Gross,Comments) ";
        //                    //SqlQuery += " VALUES('" + bookingno + "',getdate(),'" + newFlightsSearch.From.Code + "','" + newFlightsSearch.To.Code + "','" + Convert.ToDateTime(newFlightsSearch.DepDate.ToString()).ToString("dd-MMM") + "','" + Convert.ToDateTime(newFlightsSearch.RetDate.ToString()).ToString("dd-MMM") + "','" + phoneNumber + "','" + Emailval + "','" + billAddress1 + "','" + billAddress2 + "','" + billCity + "','" + billPostalCode + "','" + billState + "','" + billCountry + "'," + newFlights.TotalCost + ",0," + newFlights.TotalCost + ",'')";

        //                    //Int64 BookingID = SqlData.Data.SqlHelperData.AddToTableReturnID(TravelWeb.Utilities.Common.ConnectionString, SqlQuery);
        //                    //var value = context.BookingFlghtDetails.Where(x => x.BookingId == bId).ToList();
        //                    var value = context.Bookings.Where(x => x.Id == bId).ToList();

        //                    if (value.Count == 0 || value == null)
        //                    {
        //                        foreach (FlightsSegment seg in outbound)
        //                        {
        //                            string type = "OutBound";
        //                            string SQLQuery = "INSERT INTO [dbo].[BookingFlghtDetails]([BookingID],[DepartDate],[ArrivalDate],[FlightNo],[Airline],[DepartOn],[Dcode],[Gcode],[ArriveOn],[FlightType],[DepDate],[ArrDate],[BoundClass],[Delind])";
        //                            SQLQuery += " VALUES(" + bId + ",'" + seg.DepartureDate.ToString("dd-MMM") + "','" + seg.ArrivalDate.ToString("dd-MMM") + "','" + seg.FlightNo + "'";
        //                            SQLQuery += ",'" + seg.Airline.Code + "','" + seg.DepartureDate.ToString("HH:mm") + "','" + seg.FromAirport.Code + "','" + seg.ToAirport.Code + "','" + seg.ArrivalDate.ToString("HH:mm") + "','" + type + "','" + seg.DepartureDate + "','" + seg.ArrivalDate + "','" + OutboundClass + "',0)";


        //                            int ck = SqlData.Data.SqlHelperData.ExecuteNonQuery(TravelWeb.Utilities.Common.ConnectionString, CommandType.Text, SQLQuery);
        //                        }
        //                    }
        //                    var valueInbound = context.Bookings.Where(x => x.Id == bId).ToList();
        //                    if (valueInbound.Count == 0 || valueInbound == null)
        //                    {
        //                        if (inbound.Count > 0)
        //                        {
        //                            foreach (FlightsSegment seg in inbound)
        //                            {
        //                                string type = "InBound";
        //                                string SQLQuery = "INSERT INTO [dbo].[BookingFlghtDetails]([BookingID],[DepartDate],[ArrivalDate],[FlightNo],[Airline],[DepartOn],[Dcode],[Gcode],[ArriveOn],[FlightType],[DepDate],[ArrDate],[BoundClass],[Delind])";
        //                                SQLQuery += " VALUES(" + bId + ",'" + seg.DepartureDate.ToString("dd-MMM") + "','" + seg.ArrivalDate.ToString("dd-MMM") + "','" + seg.FlightNo + "'";
        //                                SQLQuery += ",'" + seg.Airline.Code + "','" + seg.DepartureDate.ToString("HH:mm") + "','" + seg.FromAirport.Code + "','" + seg.ToAirport.Code + "','" + seg.ArrivalDate.ToString("HH:mm") + "','" + type + "','" + seg.DepartureDate + "','" + seg.ArrivalDate + "','" + InboundClass + "',0)";

        //                                int ck = SqlData.Data.SqlHelperData.ExecuteNonQuery(TravelWeb.Utilities.Common.ConnectionString, CommandType.Text, SQLQuery);
        //                            }
        //                        }
        //                    }
        //                    var PassengerLength = context.Bookings.Where(x => x.Id == bId).ToList();
        //                    {

        //                        //if (PassengerLength.Count != lps.Count || PassengerLength == null)
        //                        //{
        //                        //    if(PassengerLength != null)
        //                        //    {
        //                        //        foreach (var item in PassengerLength)
        //                        //        {
        //                        //            BookingPaxDetail bpx = new BookingPaxDetail();
        //                        //            bpx.PaxId = item.PaxId;
        //                        //            context.BookingPaxDetails.Attach(bpx);
        //                        //            context.Entry(bpx).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
        //                        //            context.SaveChanges();
        //                        //        }
        //                        //    }
        //                        string SQLQuerys = "Delete From [dbo].[BookingPaxDetails] Where BookingID=" + bId;
        //                        int cks = SqlData.Data.SqlHelperData.ExecuteNonQuery(TravelWeb.Utilities.Common.ConnectionString, CommandType.Text, SQLQuerys);
        //                        foreach (Passenger pax in lps)
        //                        {
        //                            string SQLQuery = "INSERT INTO[dbo].[BookingPaxDetails] ([BookingID],[PaxTitle],[PaxType],[FirstName],[LastName],[DOB],[MiddleName],[Gender])";
        //                            SQLQuery += "VALUES(" + bId + ", '" + pax.Title + "', '" + pax.PaxType + "', '" + pax.FirstName + "', '" + pax.LastName + "', '" + pax.DateOfBirth + "', '" + pax.MiddleName + "', '" + pax.Gender + "')";
        //                            int ck = SqlData.Data.SqlHelperData.ExecuteNonQuery(TravelWeb.Utilities.Common.ConnectionString, CommandType.Text, SQLQuery);
        //                        }
        //                        //}

        //                    }


        //                    //string sqlquery = "Insert into FlightBookings(BookingNumber,[BookingDate],[FromDeparture],[ToDestination],[DepartDate],[ReturnDate],[Adult],[Child],[Infant],[Nett],[Status],[PassangerData],[FlightDetailsOutbound],[FlightDetailsInbound],[ContactPerson],[ContactEmail],[PaxFullName],[BillAddress1],[BillAddress2],[BillPostalCode],[BillCity],[BillState],[BillCountry],[CardName],[CardType],[CardNumber],[CardValMonth],[CardValYear], [CardSecCode])";
        //                    //sqlquery += " values('" + bookingno + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + newFlightsSearch.From.CityCode + "','" + newFlightsSearch.To.CityCode + "','" + Convert.ToDateTime(newFlightsSearch.DepDate.ToString()).ToString("yyyy-MM-dd HH:mm:ss") + "','" + Convert.ToDateTime(newFlightsSearch.RetDate.ToString()).ToString("yyyy-MM-dd HH:mm:ss") + "'," + newFlightsSearch.Adult + "," + newFlightsSearch.Child + "," + newFlightsSearch.Infant + ",'" + newFlights.TotalCost + "','PENDING','" + ToXML(lps) + "','" + ToXML(outbound) + "','" + ToXML(inbound) + "',";
        //                    //sqlquery += " '" + phoneNumber + "','" + Emailval + "','" + paxfullname + "','" + billAddress1 + "','" + billAddress2 + "','" + billPostalCode + "','" + billCity + "','" + billState + "','" + billCountry + "','" + billPCardName + "','" + CardType + "','" + CardNumber + "','" + CardValMonth + "','" + CardvalYear + "','" + CardSecrutyCode + "')";

        //                    //int i = SqlData.Data.SqlHelperData.ExecuteNonQuery(TravelWeb.Utilities.Common.ConnectionString, CommandType.Text, sqlquery);

        //                    //sendConfirmationEmailtoCustomer(Convert.ToInt32(bId));

        //                }
        //                else
        //                {
        //                    dynamic AdultDetails = JsonConvert.DeserializeObject(AdultData);
        //                    dynamic ChildDetails = JsonConvert.DeserializeObject(ChildtData);
        //                    dynamic InfantDetails = JsonConvert.DeserializeObject(InfantData);


        //                    //comented by Sunit - 29082023

        //                    /*Flights newFlights = HttpContext.Session.GetObjectFromJson<Flights>("Flights");
        //                    FlightsSearch newFlightsSearch = HttpContext.Session.GetObjectFromJson<FlightsSearch>("FlightsSearch");*/

        //                    //End

        //                    //FlightsSearch newFlightsSearch = flightSearch.FlightsSearch;

        //                    dynamic PriceSummuryDetails = JsonConvert.DeserializeObject(PriceSummury);

        //                    List<FlightsSegment> outbound = newFlights.Outbound;
        //                    List<FlightsSegment> inbound = newFlights.Inbound;

        //                    DataTable dtPax = new DataTable();
        //                    dtPax.Columns.Add("FirstName", typeof(string));
        //                    dtPax.Columns.Add("LastName", typeof(string));
        //                    dtPax.Columns.Add("MiddleName", typeof(string));
        //                    dtPax.Columns.Add("DateOfBirth", typeof(string));
        //                    dtPax.Columns.Add("Gender", typeof(string));
        //                    dtPax.Columns.Add("PaxType", typeof(string));

        //                    foreach (var adlt in AdultDetails)
        //                    {
        //                        DataRow drad = dtPax.NewRow();
        //                        drad["DateOfBirth"] = adlt["DateOfBirth"].ToString();
        //                        drad["Gender"] = adlt["Gender"].ToString();
        //                        drad["FirstName"] = adlt["FirstName"].ToString();
        //                        drad["LastName"] = adlt["LastName"].ToString();
        //                        drad["MiddleName"] = adlt["MiddleName"].ToString();
        //                        drad["PaxType"] = "Adult";
        //                        dtPax.Rows.Add(drad);
        //                    }
        //                    foreach (var chld in ChildDetails)
        //                    {
        //                        DataRow drch = dtPax.NewRow();
        //                        drch["DateOfBirth"] = chld["DateOfBirth"].ToString();
        //                        drch["Gender"] = chld["Gender"].ToString();
        //                        drch["FirstName"] = chld["FirstName"].ToString();
        //                        drch["LastName"] = chld["LastName"].ToString();
        //                        drch["MiddleName"] = chld["MiddleName"].ToString();
        //                        drch["PaxType"] = "Child";
        //                        dtPax.Rows.Add(drch);
        //                    }
        //                    foreach (var infnt in InfantDetails)
        //                    {
        //                        DataRow drinf = dtPax.NewRow();
        //                        drinf["DateOfBirth"] = infnt["DateOfBirth"].ToString();
        //                        drinf["Gender"] = infnt["Gender"].ToString();
        //                        drinf["FirstName"] = infnt["FirstName"].ToString();
        //                        drinf["LastName"] = infnt["LastName"].ToString();
        //                        drinf["MiddleName"] = infnt["MiddleName"].ToString();
        //                        drinf["PaxType"] = "Infant";
        //                        dtPax.Rows.Add(drinf);
        //                    }
        //                    List<Passenger> lps = new List<Passenger>();

        //                    foreach (DataRow drp in dtPax.Rows)
        //                    {
        //                        Passenger psg = new Passenger();
        //                        psg.FirstName = drp["FirstName"].ToString();
        //                        psg.LastName = drp["LastName"].ToString();
        //                        psg.MiddleName = drp["MiddleName"].ToString();
        //                        psg.DateOfBirth = drp["DateOfBirth"].ToString();
        //                        psg.Gender = drp["Gender"].ToString();
        //                        psg.PaxType = drp["PaxType"].ToString();
        //                        lps.Add(psg);
        //                    }

        //                    string paxfullname = dtPax.Rows[0]["FirstName"].ToString() + " " + dtPax.Rows[0]["MiddleName"] + " " + dtPax.Rows[0]["LastName"].ToString();

        //                    string bookingno = GetBookingNumber();
        //                    //string bookingno = "B" + DateTime.Now.ToString("yyMMddHHmmssff");

        //                    SqlParameter[] param = new SqlParameter[29];
        //                    param[0] = new SqlParameter("@BookingNumber", bookingno);
        //                    param[1] = new SqlParameter("@BookingDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        //                    param[2] = new SqlParameter("@FromDeparture", newFlightsSearch.From.CityCode);
        //                    param[3] = new SqlParameter("@ToDestination", newFlightsSearch.To.CityCode);
        //                    param[4] = new SqlParameter("@DepartDate", Convert.ToDateTime(newFlightsSearch.DepDate.ToString()).ToString("yyyy-MM-dd HH:mm:ss"));
        //                    param[5] = new SqlParameter("@ReturnDate", Convert.ToDateTime(newFlightsSearch.RetDate.ToString()).ToString("yyyy-MM-dd HH:mm:ss"));
        //                    param[6] = new SqlParameter("@Adult", newFlightsSearch.Adult);
        //                    param[7] = new SqlParameter("@Child", newFlightsSearch.Child);
        //                    param[8] = new SqlParameter("@Infant", newFlightsSearch.Infant);
        //                    //param[9] = new SqlParameter("@Nett", Convert.ToInt64(TotalFinalCost));
        //                    param[9] = new SqlParameter("@Nett", Convert.ToInt64(Math.Floor(Convert.ToDouble(TotalFinalCost))));
        //                    param[10] = new SqlParameter("@Status", "PENDING");
        //                    param[11] = new SqlParameter("@PassangerData", ToXML(lps));
        //                    param[12] = new SqlParameter("@FlightDetailsOutbound", ToXML(outbound));
        //                    param[13] = new SqlParameter("@FlightDetailsInbound", ToXML(inbound));
        //                    param[14] = new SqlParameter("@ContactPerson", phoneNumber);
        //                    param[15] = new SqlParameter("@ContactEmail", Emailval);
        //                    param[16] = new SqlParameter("@PaxFullName", paxfullname);
        //                    param[17] = new SqlParameter("@BillAddress1", billAddress1);
        //                    param[18] = new SqlParameter("@BillAddress2", billAddress2);
        //                    param[19] = new SqlParameter("@BillPostalCode", billPostalCode);
        //                    param[20] = new SqlParameter("@BillCity", billCity);
        //                    param[21] = new SqlParameter("@BillState", billState);
        //                    param[22] = new SqlParameter("@BillCountry", billCountry);
        //                    param[23] = new SqlParameter("@CardName", billPCardName);
        //                    param[24] = new SqlParameter("@CardType", CardType);
        //                    param[25] = new SqlParameter("@CardNumber", CardNumber);
        //                    param[26] = new SqlParameter("@CardMonth", CardValMonth);
        //                    param[27] = new SqlParameter("@CardValYear", CardvalYear);
        //                    param[28] = new SqlParameter("@CardSecCode", CardSecrutyCode);
        //                    // param[28].Direction = ParameterDirection.InputOutput;
        //                    //  SqlData.Data.SqlHelperData.ExecuteNonQuery(TravelWeb.Utilities.Common.ConnectionString, CommandType.StoredProcedure, "HSP_InsertBooking", param);



        //                    AcknowledgeData ack = new AcknowledgeData();
        //                    ack.Reference = bookingno;
        //                    ack.PassengerName = paxfullname;
        //                    ack.PaidTo = "RushMyFlight Travel";
        //                    ack.PaymentMethod = CardType;
        //                    ack.TravelDate = Convert.ToDateTime(newFlightsSearch.DepDate.ToString()).ToString("MMM-dd-yyyy HH:mm");
        //                    string subject = newFlightsSearch.From.CityCode + " " + newFlightsSearch.To.CityCode;
        //                    if (newFlightsSearch.FlightWay.ToString() == "RoundTrip")
        //                    {
        //                        subject += " RETURN " + Convert.ToDateTime(newFlightsSearch.DepDate.ToString()).ToString("MMM-dd-yyyy") + " " + Convert.ToDateTime(newFlightsSearch.RetDate.ToString()).ToString("MMM-dd-yyyy");
        //                    }
        //                    else
        //                    {
        //                        subject += " ONEWAY " + Convert.ToDateTime(newFlightsSearch.DepDate.ToString()).ToString("MMM-dd-yyyy");
        //                    }
        //                    if (Convert.ToInt32(newFlightsSearch.Adult) > 0)
        //                    {
        //                        subject += "  " + newFlightsSearch.Adult + " ADULT ";
        //                    }
        //                    if (Convert.ToInt32(newFlightsSearch.Child) > 0)
        //                    {
        //                        subject += newFlightsSearch.Child + " CHILD ";
        //                    }
        //                    if (Convert.ToInt32(newFlightsSearch.Infant) > 0)
        //                    {
        //                        subject += newFlightsSearch.Infant + " INFANT";
        //                    }
        //                    ack.Subject = subject;
        //                    //ack.AmountPaid = newFlights.TotalCost.ToString("f2");
        //                    ack.AmountPaid = TotalFinalCost.ToString();


        //                    HttpContext.Session.SetObjectAsJson("AcknowledgeData", ack);



        //                    string depdate = (newFlightsSearch.DepDate.ToString().Contains("0001")) ? "" : Convert.ToDateTime(newFlightsSearch.DepDate.ToString()).ToString("dd-MMM");
        //                    string retDate = (newFlightsSearch.RetDate.ToString().Contains("0001")) ? "" : Convert.ToDateTime(newFlightsSearch.RetDate.ToString()).ToString("dd-MMM");

        //                    //string CArdinfo = "CardName :" + billPCardName + "\n";
        //                    //CArdinfo += "CardType : " + CardType + "\n";
        //                    //CArdinfo += "CardNumber : " + CardNumber + "\n";
        //                    //CArdinfo += "CardValMonth : " + CardValMonth + " / " + CardvalYear + "\n";
        //                    //CArdinfo += "CardSecCode : " + CardSecrutyCode + "\n";

        //                    string CArdinfo = "";
        //                    // string bookingno = "B" + DateTime.Now.ToString("yyMMddHHmmssff");


        //                    //string SqlQuery = "INSERT INTO FlightBookings (BookingNumber,BookingDate,FromDeparture,ToDestination,DepartDate,ReturnDate,ContactPerson,ContactEmail,BillAddress1,BillAddress2,BillPostalCode,BillCity,BillState,BillCountry,nett,CardCharge,Gross,Comments,[CardName],[CardType],[CardNumber],[CardValMonth],[CardValYear], [CardSecCode],[IPAddress],[StatusId], MarkupType, MarkupValue) ";
        //                    //SqlQuery += " VALUES('" + bookingno + "',getdate(),'" + newFlightsSearch.From.Code + "','" + newFlightsSearch.To.Code + "','" + depdate + "','" + retDate + "','" + phoneNumber + "','" + Emailval + "','" + billAddress1 + "','" + billAddress2 + "','" + billPostalCode + "','" + billCity + "','" + billState + "','" + billCountry + "'," + TotalFinalCost + ",0," + TotalFinalCost + ",'" + CArdinfo + "','" + billPCardName + "','" + CardType + "','" + CardNumber + "','" + CardValMonth + "','" + CardvalYear + "','" + CardSecrutyCode + "','" + IPAddress + "',1,'" + MarkupType + "','" + MarkupValue + "')";

        //                    //Int64 BookingID = SqlData.Data.SqlHelperData.AddToTableReturnID(TravelWeb.Utilities.Common.ConnectionString, SqlQuery);
        //                    string SqlQuery = $@"
        //INSERT INTO [FlightBooking]
        //(
        //    BookingNumber,
        //    BookingDate,
        //    FromDeparture,
        //    ToDestination,
        //    DepartDate,
        //    ReturnDate,
        //    ContactPerson,
        //    ContactEmail,
        //    BillAddress1,
        //    BillAddress2,
        //    BillPostalCode,
        //    BillCity,
        //    BillState,
        //    BillCountry,
        //    Nett,
        //    CardCharge,
        //    Gross,
        //    Comments,
        //    [CardName],
        //    [CardType],
        //    [CardNumber],
        //    [CardValMonth],
        //    [CardValYear],
        //    [CardSecCode],
        //    [IPAddress],
        //    [StatusId],
        //    MarkupType,
        //    MarkupValue
        //)
        //VALUES
        //(
        //    '{bookingno.Replace("'", "''")}',            -- BookingNumber
        //    GETDATE(),                                   -- BookingDate
        //    '{newFlightsSearch.From.Code}',              -- FromDeparture
        //    '{newFlightsSearch.To.Code}',                -- ToDestination
        //    '{depdate}',                                 -- DepartDate
        //    '{retDate}',                                 -- ReturnDate
        //    '{phoneNumber.Replace("'", "''")}',          -- ContactPerson
        //    '{Emailval.Replace("'", "''")}',             -- ContactEmail
        //    '{billAddress1.Replace("'", "''")}',         -- BillAddress1
        //    '{billAddress2.Replace("'", "''")}',         -- BillAddress2
        //    '{billPostalCode.Replace("'", "''")}',       -- BillPostalCode
        //    '{billCity.Replace("'", "''")}',             -- BillCity
        //    '{billState.Replace("'", "''")}',            -- BillState
        //    '{billCountry.Replace("'", "''")}',          -- BillCountry
        //    {TotalFinalCost},                            -- Nett
        //    0,                                           -- CardCharge
        //    {TotalFinalCost},                            -- Gross
        //    '{CArdinfo.Replace("'", "''")}',             -- Comments
        //    '{billPCardName.Replace("'", "''")}',        -- CardName
        //    '{CardType}',                                -- CardType
        //    '{CardNumber}',                              -- CardNumber
        //    '{CardValMonth}',                            -- CardValMonth
        //    '{CardvalYear}',                             -- CardValYear
        //    '{CardSecrutyCode}',                         -- CardSecCode
        //    '{IPAddress}',                               -- IPAddress
        //    1,                                           -- StatusId
        //    '{MarkupType}',                              -- MarkupType
        //    '{MarkupValue}'                              -- MarkupValue
        //);
        //SELECT CAST(SCOPE_IDENTITY() AS BIGINT);
        //";

        //                    long BookingID = SqlData.Data.SqlHelperData.AddToTableReturnIDValue(
        //                        TravelWeb.Utilities.Common.ConnectionString,
        //                        SqlQuery
        //                    );

        //                    string Adult = PriceSummuryDetails[0].AdultDetails.ToString().Split('|')[0];
        //                    string Child = PriceSummuryDetails[0].ChildDetails.ToString().Split('|')[0];
        //                    string Infant = PriceSummuryDetails[0].InfantDetails.ToString().Split('|')[0];

        //                    string Adultfare = PriceSummuryDetails[0].AdultDetails.ToString().Split('|')[1];
        //                    string Childfare = PriceSummuryDetails[0].ChildDetails.ToString().Split('|')[1];
        //                    string Infantfare = PriceSummuryDetails[0].InfantDetails.ToString().Split('|')[1];

        //                    string AdultfareTax = PriceSummuryDetails[0].AdultDetails.ToString().Split('|')[2];
        //                    string ChildfareTax = PriceSummuryDetails[0].ChildDetails.ToString().Split('|')[2];
        //                    string InfantfareTax = PriceSummuryDetails[0].InfantDetails.ToString().Split('|')[2];

        //                    string AdultfareMarkup = PriceSummuryDetails[0].AdultDetails.ToString().Split('|')[3];
        //                    string ChildfareMarkup = PriceSummuryDetails[0].ChildDetails.ToString().Split('|')[3];
        //                    string InfantfareMarkup = PriceSummuryDetails[0].InfantDetails.ToString().Split('|')[3];

        //                    string SqlQuery1 = "UPDATE FlightBooking SET FlightClass='" + FlightClass + "',FlightWay='" + FlightWay + "',SupportPlan='" + SupportPlan + "',SupportCost=" + suppurtPlanAmt + ",BaggageCost=" + totBagPrice + ",BaggageSize='" + BaggagePlan + "', Adult=" + Adult + ",Child=" + Child + ",Infant=" + Infant + ",AdultFare=" + Adultfare + ",AdutlTax=" + AdultfareTax + ",AdultMarkup=" + AdultfareMarkup + ",ChildFare=" + Childfare + ",ChildTax=" + ChildfareTax + ",ChildMarkup=" + ChildfareMarkup + ",InfantFare=" + Infantfare + ",InfantTax=" + InfantfareTax + ",InfantMarkup=" + InfantfareMarkup + "',PaymentType=" + PaymentType+ " WHERE BookingID=" + BookingID;



        //                    int cd = SqlData.Data.SqlHelperData.ExecuteNonQuery(Common.ConnectionString, CommandType.Text, SqlQuery1);

        //                    int.TryParse(Adult, out int adultCount);
        //                    int.TryParse(Child, out int childCount);
        //                    int.TryParse(Infant, out int infantCount);


        //                    string totalPax = (adultCount + childCount + infantCount).ToString();


        //                    string insertB2CQuery = $@"
        //INSERT INTO [Bookings_B2C]
        //(
        //    PnrNo,
        //    Bookingstatus,
        //    WebsiteID,
        //    SystemDate,
        //    TravelDateFrom,
        //    TravelDateTo,
        //    Origin,
        //    Destination,
        //    TotalPax,
        //    RandomNo,
        //    Emailid,
        //    PaylaterUrl
        //)
        //VALUES
        //(
        //    NULL,                                    -- PnrNo (set later)
        //    'Pending',                               -- Bookingstatus
        //    'rsmyfly',                               -- WebsiteID
        //    GETDATE(),                               -- SystemDate
        //    '{newFlightsSearch.DepDate}',                             -- TravelDateFrom
        //    '{newFlightsSearch.RetDate}',                             -- TravelDateTo
        //    '{newFlightsSearch.From.Code}',          -- Origin
        //    '{newFlightsSearch.To.Code}',            -- Destination
        //    {totalPax}, -- TotalPax
        //    '{bookingno.Replace("'", "''")}',        -- RandomNo (BookingNumber)
        //    '{Emailval.Replace("'", "''")}',         -- Emailid
        //    NULL                                     -- PaylaterUrl
        //);
        //";

        //                    SqlHelperData.ExecuteNonQuery(
        //    TravelWeb.Utilities.Common.ConnectionString,
        //    CommandType.Text,
        //    insertB2CQuery

        //);

        //                    //string statusQuery = "INSERT INTO BookingStatus (BookingID,Status,UserID,DateCreated) ";
        //                    //statusQuery += " VALUES(" + BookingID + ",'Pending',0,getdate()";
        //                    // string bookingno = "B" + DateTime.Now.ToString("yyMMddHHmmssff");
        //                    //string SqlQuery = "INSERT INTO FlightBookings (BookingNumber,BookingDate,FromDeparture,ToDestination,DepartDate,ReturnDate,ContactPerson,ContactEmail,BillAddress1,BillAddress2,BillPostalCode,BillCity,BillState,BillCountry,nett,CardCharge,Gross,Comments) ";
        //                    //SqlQuery += " VALUES('" + bookingno + "',getdate(),'" + newFlightsSearch.From.Code + "','" + newFlightsSearch.To.Code + "','" + Convert.ToDateTime(newFlightsSearch.DepDate.ToString()).ToString("dd-MMM") + "','" + Convert.ToDateTime(newFlightsSearch.RetDate.ToString()).ToString("dd-MMM") + "','" + phoneNumber + "','" + Emailval + "','" + billAddress1 + "','" + billAddress2 + "','" + billCity + "','" + billPostalCode + "','" + billState + "','" + billCountry + "'," + newFlights.TotalCost + ",0," + newFlights.TotalCost + ",'')";

        //                    //Int64 BookingID = SqlData.Data.SqlHelperData.AddToTableReturnID(TravelWeb.Utilities.Common.ConnectionString, SqlQuery);
        //                    foreach (FlightsSegment seg in outbound)
        //                    {
        //                        string type = "OutBound";
        //                        string SQLQuery = "INSERT INTO[dbo].[BookingFlghtDetail]([BookingID],[DepartDate],[ArrivalDate],[FlightNo],[Airline],[DepartOn],[Dcode],[Gcode],[ArriveOn],[FlightType],[DepDate],[ArrDate],[BoundClass])";
        //                        SQLQuery += "VALUES(" + BookingID + ",'" + seg.DepartureDate.ToString("dd-MMM") + "','" + seg.ArrivalDate.ToString("dd-MMM") + "','" + seg.FlightNo + "'";
        //                        SQLQuery += ",'" + seg.Airline.Code + "','" + seg.DepartureDate.ToString("HH:mm") + "','" + seg.FromAirport.Code + "','" + seg.ToAirport.Code + "','" + seg.ArrivalDate.ToString("HH:mm") + "','" + type + "','" + seg.DepartureDate + "','" + seg.ArrivalDate + "','" + OutboundClass + "')";

        //                        int ck = SqlData.Data.SqlHelperData.ExecuteNonQuery(TravelWeb.Utilities.Common.ConnectionString, CommandType.Text, SQLQuery);
        //                    }
        //                    if (inbound.Count > 0)
        //                    {
        //                        foreach (FlightsSegment seg in inbound)
        //                        {
        //                            string type = "InBound";
        //                            string SQLQuery = "INSERT INTO[dbo].[BookingFlghtDetail]([BookingID],[DepartDate],[ArrivalDate],[FlightNo],[Airline],[DepartOn],[Dcode],[Gcode],[ArriveOn],[FlightType],[DepDate],[ArrDate],[BoundClass])";
        //                            SQLQuery += "VALUES(" + BookingID + ",'" + seg.DepartureDate.ToString("dd-MMM") + "','" + seg.ArrivalDate.ToString("dd-MMM") + "','" + seg.FlightNo + "'";
        //                            SQLQuery += ",'" + seg.Airline.Code + "','" + seg.DepartureDate.ToString("HH:mm") + "','" + seg.FromAirport.Code + "','" + seg.ToAirport.Code + "','" + seg.ArrivalDate.ToString("HH:mm") + "','" + type + "','" + seg.DepartureDate + "','" + seg.ArrivalDate + "','" + InboundClass + "')";

        //                            int ck = SqlData.Data.SqlHelperData.ExecuteNonQuery(TravelWeb.Utilities.Common.ConnectionString, CommandType.Text, SQLQuery);
        //                        }
        //                    }
        //                    foreach (Passenger pax in lps)
        //                    {
        //                        string SQLQuery = "INSERT INTO[dbo].[BookingPaxDetail] ([BookingID],[PaxTitle],[PaxType],[FirstName],[LastName],[DOB] ,[MiddleName],[Gender])";
        //                        SQLQuery += "VALUES(" + BookingID + ", '" + pax.Title + "', '" + pax.PaxType + "', '" + pax.FirstName + "', '" + pax.LastName + "', '" + pax.DateOfBirth + "', '" + pax.MiddleName + "', '" + pax.Gender + "')";
        //                        int ck = SqlData.Data.SqlHelperData.ExecuteNonQuery(TravelWeb.Utilities.Common.ConnectionString, CommandType.Text, SQLQuery);
        //                    }


        //                    //string sqlquery = "Insert into FlightBookings(BookingNumber,[BookingDate],[FromDeparture],[ToDestination],[DepartDate],[ReturnDate],[Adult],[Child],[Infant],[Nett],[Status],[PassangerData],[FlightDetailsOutbound],[FlightDetailsInbound],[ContactPerson],[ContactEmail],[PaxFullName],[BillAddress1],[BillAddress2],[BillPostalCode],[BillCity],[BillState],[BillCountry],[CardName],[CardType],[CardNumber],[CardValMonth],[CardValYear], [CardSecCode])";
        //                    //sqlquery += " values('" + bookingno + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + newFlightsSearch.From.CityCode + "','" + newFlightsSearch.To.CityCode + "','" + Convert.ToDateTime(newFlightsSearch.DepDate.ToString()).ToString("yyyy-MM-dd HH:mm:ss") + "','" + Convert.ToDateTime(newFlightsSearch.RetDate.ToString()).ToString("yyyy-MM-dd HH:mm:ss") + "'," + newFlightsSearch.Adult + "," + newFlightsSearch.Child + "," + newFlightsSearch.Infant + ",'" + newFlights.TotalCost + "','PENDING','" + ToXML(lps) + "','" + ToXML(outbound) + "','" + ToXML(inbound) + "',";
        //                    //sqlquery += " '" + phoneNumber + "','" + Emailval + "','" + paxfullname + "','" + billAddress1 + "','" + billAddress2 + "','" + billPostalCode + "','" + billCity + "','" + billState + "','" + billCountry + "','" + billPCardName + "','" + CardType + "','" + CardNumber + "','" + CardValMonth + "','" + CardvalYear + "','" + CardSecrutyCode + "')";

        //                    //int i = SqlData.Data.SqlHelperData.ExecuteNonQuery(TravelWeb.Utilities.Common.ConnectionString, CommandType.Text, sqlquery);

        //                    //sendConfirmationEmailtoCustomer(Convert.ToInt32(BookingID));
        //                }

        //                return Json("");
        //            }

        //            catch (Exception ex)
        //            {
        //                return Json("");
        //            }

        //            // Here you can store datatable value in database

        //            return Json("");
        //        }

        [HttpPost]
        public JsonResult V1SaveData(
                   string FlightClass,
                   string MarkupType,
                   string MarkupValue,
                   string InboundClass,
                   string OutboundClass,
                   string FlightWay,
                   string SupportPlan,
                   string suppurtPlanAmt,
                   string BaggagePlan,
                   string totBagPrice,
                   string PriceSummury,
                   string AdultData,
                   string ChildtData,
                   string InfantData,
                   string phoneNumber,
                    string PhoneCountry,
                   string Emailval,
                   string billAddress1,
                   string billAddress2,
                   string billPostalCode,
                   string billCity,
                   string billState,
                   string billCountry,
                   string billPCardName,
                   string CardNumber,
                   string CardValMonth,
                   string CardvalYear,
                   string CardSecrutyCode,
                   string CardType,
                   string TotalFinalCost,
                   string IPAddress,
                   string BookingId,
                   string Qs,
                   bool isMainCabinChecked,
                   string SeatPrefrenceAmount,
                   string totalSeatPrefrenceAmount,
                   string TotalMainCabinPrice,
                   string SearchId,
                   string FlightId, string Token,string CampId)
        {

            //var val = Common.GetObjectSession<FlightsResult>("search_" + SearchId);
            //string json = AplicationHelper.Get(SearchId, "_search");
            FlightsResult json = AplicationHelper.Get<FlightsResult>(SearchId, "search_");
            FlightsResult val = json;

            FlightsResult flightSearch = val;

            Flights newFlights = null;
            FlightsSearch newFlightsSearch = null;

            var detailsCtx = FlightVerifierService.LoadDetailsContext(SearchId, FlightId);
            if (detailsCtx?.Flight != null)
            {
                newFlights = detailsCtx.Flight;
                newFlightsSearch = detailsCtx.FlightsSearch ?? flightSearch?.FlightsSearch;
            }
            else
            {
                newFlights = flightSearch?.FlightsList?.Where(x => x.FlightId == FlightId).FirstOrDefault();
                newFlightsSearch = flightSearch?.FlightsSearch;
            }

            if (newFlights == null || newFlightsSearch == null)
                return Json(new { success = false, message = "Flight session expired. Please search again." });

            // Prefer persisted selected/live ticket total from SearchStore so Choose-your-ticket carries forward
            // Payable = GrandTotal (fare+markup) + AtolCharge + CardCharge
            var storedTotal = FlightVerifierService.GetPayableTotal(newFlights);
            if (storedTotal <= 0)
                storedTotal = newFlights.TotalCost;
            if (storedTotal > 0)
            {
                if (!decimal.TryParse(TotalFinalCost, System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.InvariantCulture, out var posted)
                    || posted <= 0
                    || Math.Abs((double)posted - storedTotal) > 1)
                {
                    TotalFinalCost = storedTotal.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);
                }
            }

            try
            {
                using (var db = new Datamodel.RSFLY())
                {
                    var booking = new Datamodel.FlightBooking();

                    List<FlightsSegment> outbound = newFlights.Outbound;
                    List<FlightsSegment> inbound = newFlights.Inbound;

                    string prefix = "RMF";

                    string lastBookingNumber = db.FlightBookings
                        .Where(x => x.BookingNumber.StartsWith(prefix))
                        .OrderByDescending(x => x.BookingId)
                        .Select(x => x.BookingNumber)
                        .FirstOrDefault();

                    int nextNumber = 1;

                    //if (!string.IsNullOrEmpty(lastBookingNumber))
                    //{
                    //    string numericPart = lastBookingNumber.Replace(prefix, "");
                    //    nextNumber = int.Parse(numericPart) + 1;
                    //}
                    if (!string.IsNullOrEmpty(lastBookingNumber))
                    {
                        string numericPart = Regex.Match(lastBookingNumber.Replace(prefix, ""), @"^\d+").Value;
                        if (!string.IsNullOrEmpty(numericPart))
                            nextNumber = int.Parse(numericPart) + 1;
                    }

                    string suffix = CampId switch
                    {
                        "Trabber" => "TRB",
                        "Rush_FS" => "FS",
                        _ => ""   
                    };
                    //booking.BookingNumber = prefix + nextNumber.ToString("D6");
                    booking.BookingNumber = prefix + nextNumber.ToString("D6") + suffix;
                    booking.Agent = CampId;
                    booking.BookingDate = DateTime.Now;
                    booking.FlightWay = Safe(FlightWay, "RoundTrip");
                    booking.FlightClass = Safe(FlightClass, "Economy");
                    booking.StatusId = 10;
                    booking.CreatedDate = DateTime.Now;
                    booking.Ipaddress = Safe(IPAddress, "");
                    booking.Stop = newFlights.Stops;
                    string cleanQs = System.Net.WebUtility.HtmlDecode(Qs);

                    var qsData = ParseQueryString(cleanQs);

                    string fromDeparture = qsData.GetValueOrDefault("depart");

                    booking.FromDeparture = fromDeparture;
                    booking.ToDestination = qsData.GetValueOrDefault("arrival");
                    booking.DepartDate = qsData.GetValueOrDefault("departOn");
                    booking.ReturnDate = qsData.GetValueOrDefault("returnOn");

                    var priceSummary = Newtonsoft.Json.JsonConvert.DeserializeObject<List<PriceSummaryDto>>(PriceSummury)?.FirstOrDefault();

                    var adult = ParsePassengerData(priceSummary.AdultDetails);
                    var child = ParsePassengerData(priceSummary.ChildDetails);
                    var infant = ParsePassengerData(priceSummary.InfantDetails);
                    booking.AdultFare = adult.fare;
                    booking.AdutlTax = adult.tax;
                    booking.AdultMarkup = adult.markup;
                    booking.SearchId = SearchId.ToString();
                    booking.ChildFare = child.fare;
                    booking.ChildTax = child.tax;
                    booking.ChildMarkup = child.markup;

                    booking.InfantFare = infant.fare;
                    booking.InfantTax = infant.tax;
                    booking.InfantMarkup = infant.markup;
                    /* Passenger */
                    booking.Adult = adult.count;
                    booking.Child = child.count;
                    booking.Infant = infant.count;
                    booking.SeatPrefAmount = SeatPrefrenceAmount.ToInt();

                    booking.IsStatus = "Pending";

                    /* Pricing */
                    booking.Gross = SafeDecimal(TotalFinalCost, 0);
                    booking.Nett = SafeDecimal(TotalFinalCost, 0);
                    booking.TotalAmount = SafeDecimal(TotalFinalCost, 0);
                    booking.MarkupValue = (adult.markup + child.markup + infant.markup).ToString();

                    /* Support */
                    booking.SupportPlan = Safe(SupportPlan, "Basic");
                    booking.SupportCost = SafeDecimal(suppurtPlanAmt, 0);

                    /* Baggage */
                    booking.BaggageSize = Safe(BaggagePlan, "No Baggage Available");
                    booking.BaggageCost = SafeDecimal(totBagPrice, 0);

                    //var phoneResult = AplicationHelper.DetectCountry(phoneNumber);

                    booking.ContactPerson = Safe(phoneNumber);
                    booking.PhoneCountry = PhoneCountry;
                    booking.ContactEmail = Safe(Emailval);

                    /* Billing */
                    booking.BillAddress1 = Safe(billAddress1);
                    booking.BillAddress2 = Safe(billAddress2);
                    booking.BillPostalCode = Safe(billPostalCode);
                    booking.BillCity = Safe(billCity);
                    booking.BillState = Safe(billState);
                    booking.BillCountry = Safe(billCountry, "US");

                    /* Card */
                    booking.CardName = Safe(billPCardName);
                    booking.CardType = Safe(CardType);
                    booking.CardNumber = Safe(CardNumber);
                    booking.CardValMonth = Safe(CardValMonth);
                    //booking.CardValYear = Safe(CardvalYear).ToInt();
                    //booking.CardSecCode = 123.ToString();

                    /* Flight Data */
                    booking.FlightDetailsOutbound = Safe("Airline = " + outbound[0].Airline.Name.ToString() + ", " + "CabinSubClass = " + outbound[0].ClassOfService + "," + "Time of Depart = " + outbound[0].DepartureDate.TimeOfDay + "_To " + outbound[0].ArrivalDate.TimeOfDay + "," + "Flight_No" + outbound[0].FlightNo);
                    if (inbound.Any())
                    {
                        booking.FlightDetailsInbound = Safe("Airline = " + inbound[0].Airline.Name.ToString() + ", " + "CabinSubClass = " + inbound[0].ClassOfService + "," + "Time of Depart = " + inbound[0].DepartureDate.TimeOfDay + "_To " + inbound[0].ArrivalDate.TimeOfDay + "," + "Flight_No" + inbound[0].FlightNo);
                    }
                    else
                    {
                        booking.FlightDetailsInbound = Safe(InboundClass);
                    }

                    /* Misc */
                    booking.PaymentType = "Card";

                    var adultPassengers = Newtonsoft.Json.JsonConvert.DeserializeObject<List<PassengerDto>>(AdultData);

                    string paxFullName = "";

                    if (adultPassengers != null && adultPassengers.Count > 0)
                    {
                        var pax = adultPassengers[0];

                        paxFullName = $"{pax.FirstName} {pax.MiddleName} {pax.LastName}".Replace("  ", " ").Trim();
                        booking.PassengerTitle = pax.Title;

                    }

                    booking.PaxFullName = paxFullName;
                    booking.PassangerData = AdultData;
                    //booking.FlightNo = 
                    db.FlightBookings.Add(booking);
                    db.SaveChanges();

                    var outboundFlights = outbound.Select(seg => new Datamodel.BookingFlghtDetail
                    {
                        BookingId = booking.BookingId,
                        DepartDate = seg.DepartureDate.ToString("dd-MMM"),
                        ArrivalDate = seg.ArrivalDate.ToString("dd-MMM"),
                        FlightNo = seg.FlightNo,
                        Airline = seg.Airline.Code,
                        DepartOn = seg.DepartureDate.ToString("HH:mm"),
                        Dcode = seg.FromAirport.Code,
                        Gcode = seg.ToAirport.Code,
                        ArriveOn = seg.ArrivalDate.ToString("HH:mm"),
                        FlightType = "OutBound",
                        DepDate = seg.DepartureDate,
                        ArrDate = seg.ArrivalDate,
                        BoundClass = OutboundClass,
                        Delind = false
                    }).ToList();

                    db.BookingFlghtDetails.AddRange(outboundFlights);


                    if (inbound.Any())
                    {
                        var inboundFlights = inbound.Select(seg => new Datamodel.BookingFlghtDetail
                        {
                            BookingId = booking.BookingId,
                            DepartDate = seg.DepartureDate.ToString("dd-MMM"),
                            ArrivalDate = seg.ArrivalDate.ToString("dd-MMM"),
                            FlightNo = seg.FlightNo,
                            Airline = seg.Airline.Code,
                            DepartOn = seg.DepartureDate.ToString("HH:mm"),
                            Dcode = seg.FromAirport.Code,
                            Gcode = seg.ToAirport.Code,
                            ArriveOn = seg.ArrivalDate.ToString("HH:mm"),
                            FlightType = "InBound",
                            DepDate = seg.DepartureDate,
                            ArrDate = seg.ArrivalDate,
                            BoundClass = InboundClass,
                            Delind = false
                        }).ToList();

                        db.BookingFlghtDetails.AddRange(inboundFlights);
                    }

                    db.SaveChanges();

                    foreach (var pax in adultPassengers)
                    {
                        var passenger = new Datamodel.BookingPaxDetail
                        {
                            BookingId = booking.BookingId,
                            PaxTitle = pax.Title,
                            FirstName = pax.FirstName,
                            MiddleName = pax.MiddleName,
                            LastName = pax.LastName,
                            Gender = pax.Gender,
                            Dob = pax.DateOfBirth,
                            PaxType = "Adult"
                        };

                        db.BookingPaxDetails.Add(passenger);
                    }

                    var childPassengers = Newtonsoft.Json.JsonConvert.DeserializeObject<List<PassengerDto>>(ChildtData);
                    var infantPassengers = Newtonsoft.Json.JsonConvert.DeserializeObject<List<PassengerDto>>(InfantData);

                    if (childPassengers.Count != 0)
                    {
                        foreach (var pax in childPassengers)
                        {
                            var passenger = new Datamodel.BookingPaxDetail
                            {
                                BookingId = booking.BookingId,
                                PaxTitle = pax.Title,

                                FirstName = pax.FirstName,
                                MiddleName = pax.MiddleName,
                                LastName = pax.LastName,
                                Gender = pax.Gender,
                                Dob = pax.DateOfBirth,
                                PaxType = "Child"
                            };

                            db.BookingPaxDetails.Add(passenger);
                        }
                    }

                    if (infantPassengers.Count != 0)
                    {

                        foreach (var pax in infantPassengers)
                        {
                            var passenger = new Datamodel.BookingPaxDetail
                            {
                                BookingId = booking.BookingId,
                                FirstName = pax.FirstName,
                                MiddleName = pax.MiddleName,
                                LastName = pax.LastName,
                                Gender = pax.Gender,
                                Dob = pax.DateOfBirth,
                                PaxType = "Infant"
                            };

                            db.BookingPaxDetails.Add(passenger);
                        }
                    }
                    db.SaveChanges();

                    TrawickService trawickService = new TrawickService(_configuration);

                    var travelers = new List<object>();
                    adultPassengers = JsonConvert.DeserializeObject<List<PassengerDto>>(AdultData);
                    foreach (var pax in adultPassengers)
                    {
                        DateTime dob = Convert.ToDateTime(pax.DateOfBirth);

                        travelers.Add(new
                        {
                            dateOfBirth = dob.ToString("MM-dd-yyyy")
                        });
                    }

                    childPassengers = JsonConvert.DeserializeObject<List<PassengerDto>>(ChildtData);

                    foreach (var pax in childPassengers)
                    {
                        DateTime dob = Convert.ToDateTime(pax.DateOfBirth);

                        travelers.Add(new
                        {
                            dateOfBirth = dob.ToString("MM-dd-yyyy")
                        });
                    }

                    infantPassengers = JsonConvert.DeserializeObject<List<PassengerDto>>(InfantData);

                    foreach (var pax in infantPassengers)
                    {
                        DateTime dob = Convert.ToDateTime(pax.DateOfBirth);

                        travelers.Add(new
                        {
                            dateOfBirth = dob.ToString("MM-dd-yyyy")
                        });
                    }
                    string destinationCountry = "US";
                    string homeCountry = "IN";
                    var quoteResponse = new List<QuoteResponse>();
                    if (!string.IsNullOrWhiteSpace(Token))
                    {
                        quoteResponse = trawickService.GetQuote(Token, travelers, Convert.ToDateTime(booking.DepartDate), Convert.ToDateTime(booking.ReturnDate), destinationCountry, homeCountry);

                    }

                    return Json(new
                    {
                        success = true,
                        message = "Booking saved successfully",
                        bookingId = booking.BookingNumber,
                        quoteResponse = quoteResponse
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpPost]
        public JsonResult UpdateDetails(
                   string billAddress1,
                   string billAddress2,
                   string billPostalCode,
                   string billCity,
                   string billState,
                   string billCountry,
                   string billPCardName,
                   string CardNumber,
                   string CardValMonth,
                   string CardvalYear,
                   string CardSecrutyCode,
                   string CardType,
                   string OpaqueDataDescriptor,
                   string OpaqueDataValue,
                   string BookingId,
                   string SearchId,
                   string CampId
               )
        {
            try
            {
                using (var db = new Datamodel.RSFLY())
                {
                    var booking = db.FlightBookings
                                    .FirstOrDefault(x => x.BookingNumber == BookingId);

                    if (booking == null)
                    {
                        return Json(new
                        {
                            success = false,
                            message = "Booking not found"
                        });
                    }
                    var paymentRequest = new AuthorizeNetPaymentRequest
                    {
                        BillAddress1 = billAddress1,
                        BillAddress2 = billAddress2,
                        BillPostalCode = billPostalCode,
                        BillCity = billCity,
                        BillState = billState,
                        BillCountry = billCountry,

                        CardHolderName = billPCardName,
                        CardNumber = CardNumber,
                        CardValMonth = CardValMonth,
                        CardValYear = CardvalYear,
                        CardSecurityCode = CardSecrutyCode,
                        CardType = CardType,

                        OpaqueDataDescriptor = OpaqueDataDescriptor,
                        OpaqueDataValue = OpaqueDataValue,

                        BookingId = BookingId,
                        SearchId = SearchId,

                        Booking = booking
                    };
                    if (CampId != "Rush_FS"&& CampId!= null /*&& CampId != "Trabber"*/)
                    {
                        var paymentResult = ProcessAuthorizeNetPayment(db, paymentRequest);

                        var json = JsonConvert.SerializeObject(paymentResult.Value);
                        dynamic result = JsonConvert.DeserializeObject(json);

                        if (result.success == false)
                        {
                            return paymentResult;
                        }
                    }

                    /* UPDATE ONLY CARD DETAILS */
                    booking.CardName = Safe(billPCardName);
                    booking.CardType = Safe(CardType);
                    booking.CardNumber = Safe(CardNumber);
                    booking.CardValMonth = Safe(CardValMonth);
                    booking.CardValYear = Safe(CardvalYear).ToInt();
                    booking.CardSecCode = Safe(CardSecrutyCode);
                    booking.IsStatus = "Confirmed";
                    booking.BillAddress1 = Safe(billAddress1);
                    booking.BillAddress2 = Safe(billAddress2);
                    booking.BillPostalCode = Safe(billPostalCode);
                    booking.BillCity = Safe(billCity);
                    booking.BillState = Safe(billState);
                    booking.BillCountry = Safe(billCountry, "US");
                    db.SaveChanges();

                    TravelWeb.Datamodel.BookingsCardDetail bookingcarddetails = new TravelWeb.Datamodel.BookingsCardDetail();
                    bookingcarddetails.CardNumber = booking.CardNumber;
                    bookingcarddetails.CardType = booking.CardType;
                    bookingcarddetails.Cvc = booking.CardSecCode;
                    bookingcarddetails.FirstName = booking.CardName;
                    bookingcarddetails.ExpiryDate = booking.CardValMonth;
                    bookingcarddetails.ExpiryYear = Convert.ToString(booking.CardValYear);
                    bookingcarddetails.BookingsId = Convert.ToInt32(booking.BookingId);
                    db.BookingsCardDetails.Add(bookingcarddetails);
                    db.SaveChanges();

                    sendConfirmationEmailtoCustomer(Convert.ToInt32(booking.BookingId));

                    AcknowledgeData ack = new AcknowledgeData();
                    ack.Reference = booking.BookingId.ToString();
                    ack.PassengerName = booking.PaxFullName;
                    ack.PaidTo = "RushMyFlight Travel";
                    ack.PaymentMethod = booking.PaymentType;
                    ack.TravelDate = Convert.ToDateTime(booking.DepartDate.ToString()).ToString("MMM-dd-yyyy HH:mm");
                    ack.AmountPaid = booking.TotalAmount.ToString();

                    AplicationHelper.Save(SearchId, "AcknowledgeData", ack);

                    //HttpContext.Session.SetObjectAsJson("AcknowledgeData", ack);

                    return Json(new
                    {
                        success = true,
                        bookingId = BookingId,
                        searchId = SearchId
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }


        public JsonResult sendEmail(int BookingId)
        {
            sendConfirmationEmailtoCustomer(BookingId);
            return Json("success");
        }

        private (int count, decimal fare, decimal tax, decimal markup) ParsePassengerData(string data)
        {
            if (string.IsNullOrEmpty(data))
                return (0, 0, 0, 0);

            var parts = data.Split('|');

            return (
                parts.Length > 0 ? Convert.ToInt32(parts[0]) : 0,
                parts.Length > 1 ? Convert.ToDecimal(parts[1]) : 0,
                parts.Length > 2 ? Convert.ToDecimal(parts[2]) : 0,
                parts.Length > 3 ? Convert.ToDecimal(parts[3]) : 0
            );
        }

        private Dictionary<string, string> ParseQueryString(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return new Dictionary<string, string>();

            query = System.Net.WebUtility.HtmlDecode(query);

            // Remove "result?"
            if (query.Contains("?"))
                query = query.Substring(query.IndexOf("?") + 1);

            return query
                .Split('&', StringSplitOptions.RemoveEmptyEntries)
                .Select(p => p.Split('=', 2))
                .Select(p => new
                {
                    Key = Clean(p[0]),
                    Value = p.Length > 1 ? Clean(p[1]) : ""
                })
                .Where(x => !string.IsNullOrEmpty(x.Key))
                .ToDictionary(x => x.Key, x => x.Value, StringComparer.OrdinalIgnoreCase);
        }

        private string Clean(string input)
        {
            if (string.IsNullOrEmpty(input))
                return "";

            return input
                .Replace("{", "")
                .Replace("}", "")
                .Replace("[", "")
                .Replace("]", "")
                .Trim();
        }

        public JsonResult cardValidation(string cardValue)
        {
            var apiURL = "https://api.bincodes.com/cc/json/8282e65654451be6ce0693eb6aee2da7/";

            cardValidation classes = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiURL);
                //HTTP GET
                var responseTask = client.GetAsync(apiURL + cardValue);
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {

                    //var Response = result.Content.ReadAsStringAsync();
                    var readTask = result.Content.ReadAsAsync<cardValidation>();
                    readTask.Wait();
                    classes = readTask.Result;
                    return Json(classes);

                }
                else //web api sent error response
                {
                    //log response status here..

                    classes = new cardValidation();

                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
                return null;
            }

        }

        private string Safe(string value, string defaultVal = "")
        {
            return string.IsNullOrWhiteSpace(value) ? defaultVal : value;
        }

        private decimal SafeDecimal(string value, decimal defaultVal = 0)
        {
            return decimal.TryParse(value, out var result) ? result : defaultVal;
        }

        private int SafeInt(string value, int defaultVal = 0)
        {
            return int.TryParse(value, out var result) ? result : defaultVal;
        }

        private DateTime SafeDate(string value, DateTime? defaultVal = null)
        {
            return DateTime.TryParse(value, out var result)
                ? result
                : defaultVal ?? DateTime.Now;
        }

        private static string MapStatus(string responseCode, bool ok, bool heldForReview)
        {
            // Authorize.Net responseCode: "1"=Approved, "2"=Declined, "3"=Error, "4"=Held for review
            if (!ok)
            {
                return responseCode switch
                {
                    "2" => "Declined",
                    "3" => "Error",
                    "4" => "Review",
                    _ => "Error"
                };
            }
            if (heldForReview) return "Review";
            return "Approved";
        }

        private JsonResult ProcessAuthorizeNetPayment(
      Datamodel.RSFLY db,
      AuthorizeNetPaymentRequest Athrequest)
        {
            

                var env = _configuration.GetValue<bool>("PaymentSettings:UseSandbox")
                          ? "Sandbox"
                          : "Production";

            ApiOperationBase<ANetApiRequest, ANetApiResponse>.RunEnvironment =
                env == "Sandbox"
                    ? AuthorizeNet.Environment.SANDBOX
                    : AuthorizeNet.Environment.PRODUCTION;

            var merchantAuthentication = new merchantAuthenticationType
            {
                name = _configuration["PaymentSettings:ApiLoginId"],
                ItemElementName = ItemChoiceType.transactionKey,
                Item = _configuration["PaymentSettings:TransactionKey"]
            };
            ApiOperationBase<ANetApiRequest, ANetApiResponse>.MerchantAuthentication = merchantAuthentication;

            var opaqueData = new opaqueDataType
            {
                dataDescriptor = Athrequest.OpaqueDataDescriptor,
                dataValue = Athrequest.OpaqueDataValue
            };

            var paymentType = new paymentType
            {
                Item = opaqueData
            };



            decimal amount = Convert.ToDecimal(Athrequest.Booking.TotalAmount);


            var paymentEntity = new Payment
            {
                OrderId = Athrequest.BookingId,
                Amount = amount,
                Currency = "USD",
                Status = "Pending",
                CustomerEmail = Athrequest.Booking.ContactEmail,
                FirstName = Athrequest.Booking.PaxFullName,
                LastName = null,
                InvoiceNumber = Athrequest.BookingId,
                GatewayEnv = env,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            db.Payments.Add(paymentEntity);
            db.SaveChanges();


            db.PaymentEvents.Add(new PaymentEvent
            {
                PaymentId = paymentEntity.Id,
                EventType = "charge:init",
                PayloadJson = JsonConvert.SerializeObject(new
                {
                    amount = amount,
                    Athrequest.BookingId,
                    env
                }),
                ReceivedAt = DateTime.Now
            });
            db.SaveChanges();



            var transactionRequest = new transactionRequestType
            {
                transactionType = transactionTypeEnum.authOnlyTransaction.ToString(),
                amount = amount,
                payment = paymentType,
                order = new orderType
                {
                    invoiceNumber = Athrequest.BookingId,
                    description = "Travel booking payment"
                },
                billTo = new customerAddressType
                {
                    firstName = Athrequest.Booking.PaxFullName,
                    email = Athrequest.Booking.ContactEmail,
                    country = Athrequest.BillCountry,
                    lastName = "",
                    address = Athrequest.BillAddress1,
                    city = Athrequest.BillCity,
                    state = Athrequest.BillState,
                    zip = Athrequest.BillPostalCode,
                    phoneNumber = Athrequest.Booking.ContactPerson
                },
                customer = new customerDataType
                {
                    email = string.IsNullOrWhiteSpace(Athrequest.Booking.ContactEmail) ? null : Athrequest.Booking.ContactEmail,
                    id = Athrequest.BookingId
                },
                transactionSettings = new[]
        {
            new settingType
            {
                settingName = "duplicateWindow",
                settingValue = _settings.DuplicateWindowSeconds.ToString(System.Globalization.CultureInfo.InvariantCulture)
            }
        },
            };

            var request = new createTransactionRequest
            {
                //merchantAuthentication = merchantAuthentication,
                transactionRequest = transactionRequest
            };

            var controller = new createTransactionController(request);
            createTransactionResponse response = null;
            ANetApiResponse errorResponse = null;
            try
            {
                controller.Execute();
                response = controller.GetApiResponse();
                errorResponse = controller.GetErrorResponse();
            }
            catch (Exception ex)
            {
                // Transport/SDK error → mark as Error
                paymentEntity.Status = "Error";
                paymentEntity.RawResponseJson = JsonConvert.SerializeObject(new { exception = ex.Message, Athrequest.BookingId, env });
                paymentEntity.UpdatedAt = DateTime.UtcNow;
                db.SaveChanges();

                db.PaymentEvents.Add(new PaymentEvent
                {
                    PaymentId = paymentEntity.Id,
                    EventType = "charge:exception",
                    PayloadJson = paymentEntity.RawResponseJson,
                    ReceivedAt = DateTime.UtcNow
                });
                db.SaveChanges();

                
                var searchid = Athrequest.SearchId;
                return Json(new
                {
                    success = false,
                    bookingId = Athrequest.BookingId,
                    searchId = searchid,
                    message = "Payment processing error. Please try again."
                });
            }

            var rawJson = JsonConvert.SerializeObject(new { api = "createTransaction", response, error = errorResponse });
            var tr = response?.transactionResponse;
            var ok = response?.messages?.resultCode == messageTypeEnum.Ok;
            var held = tr?.responseCode == "4";
            var status = MapStatus(tr?.responseCode, ok, held);

            paymentEntity.Status = status;
            paymentEntity.AnetTransactionId = tr?.transId ?? paymentEntity.AnetTransactionId;
            paymentEntity.AuthCode = tr?.authCode ?? paymentEntity.AuthCode;
            paymentEntity.AvsResult = tr?.avsResultCode ?? paymentEntity.AvsResult;
            paymentEntity.CvvResult = tr?.cvvResultCode ?? paymentEntity.CvvResult;
            paymentEntity.RawResponseJson = rawJson;
            paymentEntity.UpdatedAt = DateTime.UtcNow;
            db.SaveChanges();

            db.PaymentEvents.Add(new PaymentEvent
            {
                PaymentId = paymentEntity.Id,
                EventType = "charge:response",
                PayloadJson = rawJson,
                ReceivedAt = DateTime.UtcNow
            });
            db.SaveChanges();
            if (response == null)
            {
                ViewBag.IsSuccess = false;
                ViewBag.Message = errorResponse?.messages?.message?.FirstOrDefault()?.text
                                  ?? "Null response from payment gateway.";
                ViewBag.Invoice = Athrequest.BookingId;
                ViewBag.PaymentId = paymentEntity.Id;
                return Json(new
                {
                    success = false,
                    bookingId = Athrequest.BookingId,
                    searchId = errorResponse.messages.message?.FirstOrDefault()?.text
                });
            }
            if (ok && tr?.responseCode == "1")
            {
                return Json(new
                {
                    success = true,
                    bookingId = Athrequest.BookingId,
                    searchId = Athrequest.SearchId,
                    transactionId = tr.transId
                });
            }

            string message = "Payment failed.";

            if (tr?.errors != null && tr.errors.Length > 0)
            {
                message = tr.errors[0].errorText;
            }

            return Json(new
            {
                success = false,
                bookingId = Athrequest.BookingId,
                searchId = Athrequest.SearchId,
                message = message
            });
        }


        public static void SetCurrentCulture(string currencyCode)
        {
            var culture = currencyCode?.ToUpper() switch
            {
                "USD" => new CultureInfo("en-US"),

                "GBP" => new CultureInfo("en-GB"),

                "CAD" => new CultureInfo("en-CA"),

                "EUR" => new CultureInfo("es-ES"), // Spain"MXN" => new CultureInfo("es-MX"),

                "BRL" => new CultureInfo("pt-BR"),

                "ARS" => new CultureInfo("es-AR"),

                "COP" => new CultureInfo("es-CO"),

                "CLP" => new CultureInfo("es-CL"),

                "PEN" => new CultureInfo("es-PE"),

                "DOP" => new CultureInfo("es-DO"),

                "CRC" => new CultureInfo("es-CR"),

                "GTQ" => new CultureInfo("es-GT"),

                _ => new CultureInfo("en-US")
            };


            CultureInfo.CurrentCulture = culture;

            CultureInfo.CurrentUICulture = culture;
        }

        [HttpGet]
        public IActionResult InsurancePlans(string bookingId, string flightId, string Token, string AdultData, string ChildtData, string InfantData, string Qs)
        {



            var adultPassengers = Newtonsoft.Json.JsonConvert.DeserializeObject<List<PassengerDto>>(AdultData);

            string paxFullName = "";


            TrawickService trawickService = new TrawickService(_configuration);
            var travelers = new List<object>();
            adultPassengers = JsonConvert.DeserializeObject<List<PassengerDto>>(AdultData);
            var childPassengers = Newtonsoft.Json.JsonConvert.DeserializeObject<List<PassengerDto>>(ChildtData);
            var infantPassengers = Newtonsoft.Json.JsonConvert.DeserializeObject<List<PassengerDto>>(InfantData);
            foreach (var pax in adultPassengers)
            {
                DateTime dob = Convert.ToDateTime(pax.DateOfBirth);

                travelers.Add(new
                {
                    dateOfBirth = dob.ToString("MM-dd-yyyy")
                });
            }

            childPassengers = JsonConvert.DeserializeObject<List<PassengerDto>>(ChildtData);

            foreach (var pax in childPassengers)
            {
                DateTime dob = Convert.ToDateTime(pax.DateOfBirth);

                travelers.Add(new
                {
                    dateOfBirth = dob.ToString("MM-dd-yyyy")
                });
            }

            infantPassengers = JsonConvert.DeserializeObject<List<PassengerDto>>(InfantData);

            foreach (var pax in infantPassengers)
            {
                DateTime dob = Convert.ToDateTime(pax.DateOfBirth);

                travelers.Add(new
                {
                    dateOfBirth = dob.ToString("MM-dd-yyyy")
                });
            }
            string destinationCountry = "US";
            string homeCountry = "IN";
            var booking = new Datamodel.FlightBooking();
            string cleanQs = System.Net.WebUtility.HtmlDecode(Qs);

            var qsData = ParseQueryString(cleanQs);

            string fromDeparture = qsData.GetValueOrDefault("depart");

            booking.FromDeparture = fromDeparture;
            booking.ToDestination = qsData.GetValueOrDefault("arrival");
            booking.DepartDate = qsData.GetValueOrDefault("departOn");
            booking.ReturnDate = qsData.GetValueOrDefault("returnOn");

            var quoteResponse = trawickService.GetQuote(Token, travelers, Convert.ToDateTime(booking.DepartDate), Convert.ToDateTime(booking.ReturnDate), destinationCountry, homeCountry);
            var jsonresponse = JsonConvert.SerializeObject(quoteResponse);
            return PartialView("_InsurancePlans", quoteResponse);
        }

        public List<ProductOptionResponse> GetProductOptions(string token, int productId)
        {
            TrawickService trawickService = new TrawickService(_configuration);


            return trawickService.GetProductOptions(token, productId);

        }


        [HttpPost]
        public JsonResult GetQuoteById(string token, int orderId)
        {
            TrawickService trawickService = new TrawickService(_configuration);

            try
            {
                var result = trawickService.GetQuoteById(token, orderId);

                return Json(new
                {
                    success = true,
                    data = result
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }
    }
}