using AngleSharp.Io;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using TravelWeb.Datamodel;
using TravelWeb.Models;
using static TravelWeb.Models.MetaRequestDto;

namespace TravelWeb.Utilities
{
    public class DeepLinkTrackerService
    {
        private readonly RSFLY _db;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DeepLinkTrackerService(RSFLY db, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<DeepLinkTracker> CreateOrGetTrackerAsync(
    MetaFlightSearchRequest req,
    string page,
    string parentSearchId = null,
    string ip = "127.0.0.1",
    string userAgent = "unknown")
        {
            var tracker = new DeepLinkTracker();
            try
            {
                var searchId = req.GeneralInfo?.GUID;

                string rootSource = null;

                if (!string.IsNullOrEmpty(parentSearchId))
                {
                    var parent = await _db.DeepLinkTrackers
                        .FirstOrDefaultAsync(t => t.SearchId == parentSearchId);

                    if (parent != null)
                        rootSource = parent.RootSource ?? parent.Source;
                }

                string immediateSource = string.IsNullOrEmpty(req.Source) ? "Internal" : req.Source;
                rootSource ??= immediateSource;

                var from = req.Segments?.FirstOrDefault()?.DepartApt ?? "";
                var to = req.Segments?.FirstOrDefault()?.ArrivalApt ?? "";

                DateTime? dep = null, ret = null;

                if (req.Segments?.Any() == true)
                {
                    if (DateTime.TryParseExact(req.Segments.First().DepartDate, "yyyy-MM-dd",
                        CultureInfo.InvariantCulture, DateTimeStyles.None, out var d))
                        dep = d;

                    if (req.Segments.Count > 1 &&
                        DateTime.TryParseExact(req.Segments.Last().DepartDate, "yyyy-MM-dd",
                        CultureInfo.InvariantCulture, DateTimeStyles.None, out var r))
                        ret = r;
                }

                tracker = new DeepLinkTracker
                {
                    SearchId = searchId,
                    ParentSearchId = parentSearchId,

                    Source = immediateSource,
                    RootSource = rootSource,
                    Stage = page,
                    PreviousStage = null,
                    FromLocation = from,
                    ToLocation = to,
                    DepartureDate = dep,
                    ReturnDate = ret,
                    TripType = req.GeneralInfo?.TripType ?? "default",
                    Adults = req.PaxDetails?.Adult ?? 1,
                    Children = req.PaxDetails?.Child ?? 0,
                    Infants = req.PaxDetails?.Infant ?? 0,
                    CabinClass = req.GeneralInfo?.Cabin ?? "default",
                    Ipaddress = ip,
                    UserAgent = userAgent,
                    CreatedAt = DateTime.Now
                };

                _db.DeepLinkTrackers.Add(tracker);
                _db.SaveChanges();

            }
            catch (Exception ex)
            {

                throw ex;
            }

            return tracker;
        }

        public async Task UpdateStageAsync(string searchId, string newStage)
        {
            var tracker = await _db.DeepLinkTrackers
                .FirstOrDefaultAsync(t => t.SearchId == searchId);

            if (tracker != null)
            {
                tracker.PreviousStage = tracker.Stage;
                tracker.Stage = newStage;
                tracker.UpdatedAt = DateTime.Now;
                await _db.SaveChangesAsync();
            }
        }
        public  void UpdateTrackerdata(string searchId, DeepLinkTracker dip)
        {
            var tracker =  _db.DeepLinkTrackers.Where(t => t.SearchId == searchId).FirstOrDefault();

            if (tracker != null)
            {
                tracker.PreviousStage = tracker.Stage;
                tracker.Stage = dip.Stage;

                tracker.PriceWithoutMarkup = dip.PriceWithoutMarkup;
                tracker.Markup = dip.Markup;
                tracker.TotalAmount = dip.TotalAmount;
                tracker.Rule = dip.Rule;
                tracker.Airline = dip.Airline;
                tracker.CabinClass = dip.CabinClass;
                tracker.SubClass = dip.SubClass;
                tracker.ModifiedBy = dip.ModifiedBy;
                tracker.InboundAirline = dip.InboundAirline;
                tracker.OutboundAirline = dip.OutboundAirline;
                    tracker.InboundClass = dip.InboundClass;
                    tracker.OutboundClass = dip.OutboundClass;
                tracker.OutboundClass = dip.OutboundAirline;
                tracker.UpdatedAt = DateTime.Now;
                tracker.ReturnDate = dip.ReturnDate;
                tracker.DepartureDate = dip.DepartureDate;
                 _db.SaveChangesAsync();
            }
        }

        public async Task UpdateSubDetailStageAsync(string searchId, string subStage)
        {
            var tracker = await _db.DeepLinkTrackers
                .FirstOrDefaultAsync(t => t.SearchId == searchId);

            if (tracker != null)
            {
                //tracker.SubDetailStage = subStage;
                tracker.UpdatedAt = DateTime.Now;
                await _db.SaveChangesAsync();
            }
        }

        public async Task CompleteBookingAsync(string searchId, string bookingRef)
        {
            var tracker = await _db.DeepLinkTrackers
                .FirstOrDefaultAsync(t => t.SearchId == searchId);

            if (tracker != null)
            {
                tracker.BookingReference = bookingRef;
                tracker.UpdatedAt = DateTime.Now;
                await _db.SaveChangesAsync();
            }
        }
        public async Task SaveFareCompareTrafficAsync(
    FlightsSearch flightSearch,
    DateTime departDate,
    DateTime? returnDate,
    string utm_source,
    string utm_campaign,
    string subId,
    string ip = null,
    string userAgent = null)
        {
            try
            {
                var tracker = new DeepLinkTracker
                {
                    SearchId = flightSearch.SearchId,
                    ParentSearchId = null,

                    Source = flightSearch.SourceMedia,
                    RootSource = flightSearch.SiteCode,

                    Stage = "Listing",
                    PreviousStage = null,

                    FromLocation = flightSearch.From?.Code?.ToUpper(),
                    ToLocation = flightSearch.To?.Code?.ToUpper(),

                    DepartureDate = departDate,
                    ReturnDate = returnDate,

                    TripType = flightSearch.FlightWay == FlightWay.RoundTrip
                        ? "R"
                        : "O",

                    Adults = flightSearch.Adult,
                    Children = flightSearch.Child,
                    Infants = flightSearch.Infant,

                    CabinClass = GetCabinClassForTracker(flightSearch.FlightClass),

                    Ipaddress = !string.IsNullOrWhiteSpace(ip) ? ip
                      : !string.IsNullOrWhiteSpace(flightSearch.IPAddress) ? flightSearch.IPAddress
                      : "unknown",
                    UserAgent = string.IsNullOrWhiteSpace(userAgent) ? "unknown" : userAgent,

                    BookingReference = null,

                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,

                    PriceWithoutMarkup = null,
                    Markup = null,

                    Rule = null,
                    Airline = null,
                    SubClass = null,
                    ModifiedBy = null,

                    SubDetailStage = null,

                    TotalAmount = 0,

                    InboundAirline = null,
                    OutboundAirline = null,

                    Stops = null,

                    InboundClass = null,
                    OutboundClass = null
                };

                _db.DeepLinkTrackers.Add(tracker);

                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                

            }
        }

        private string GetCabinClassForTracker(FlightClass flightClass)
        {
            switch (flightClass)
            {
                case FlightClass.Economy:
                    return "Y";

                case FlightClass.Business:
                    return "B";

                case FlightClass.EconomyPremium:
                    return "P";

                case FlightClass.First:
                    return "F";

                default:
                    return "Y";
            }
        }


    }
}
