using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TravelWeb.Models.Hotel;
using TravelWeb.Services;

namespace TravelWeb.Controllers
{
    public class HotelsController : Controller
    {
        private readonly HotelSearchService _hotelSearchService;

        public HotelsController(HotelSearchService hotelSearchService)
        {
            _hotelSearchService = hotelSearchService;
        }

        [HttpPost]
        public async Task<IActionResult> Search([FromBody] HotelSearchFormRequest request)
        {
            var (success, searchId, result, error) = await _hotelSearchService.SearchAndSaveAsync(request);

            if (!success)
            {
                return Json(new
                {
                    success = false,
                    error = error ?? "Hotel search failed.",
                    searchId
                });
            }

            HotelSearchService.TryParseDate(request?.CheckIn, out var checkIn);
            HotelSearchService.TryParseDate(request?.CheckOut, out var checkOut);
            var nights = Math.Max(1, (checkOut.Date - checkIn.Date).Days);
            if (nights < 1) nights = 1;

            var hotels = (result?.HotelsList ?? Enumerable.Empty<HotelItem>()).Select(h =>
            {
                var stars = HotelSearchService.ParseStarRating(h.Rating);
                var guestRating = HotelSearchService.ParseGuestRating(h.TripAdvisorRating);
                var nightly = h.TotalPrice / nights;
                return new
                {
                    hotelCode = h.HotelCode,
                    name = h.HotelName,
                    picture = string.IsNullOrWhiteSpace(h.HotelPicture)
                        ? h.Images?.FirstOrDefault()?.Url
                        : h.HotelPicture,
                    address = string.IsNullOrWhiteSpace(h.HotelAddress) ? (request?.Destination ?? "") : h.HotelAddress,
                    stars,
                    rating = guestRating,
                    nightly = Math.Round(nightly),
                    total = Math.Round(h.TotalPrice),
                    currency = h.Currency ?? request?.Currency ?? "USD",
                    amenities = h.Amenities ?? new System.Collections.Generic.List<string>(),
                    provider = string.IsNullOrWhiteSpace(h.Provider) ? "WIKI" : h.Provider,
                    searchId
                };
            }).ToList();

            return Json(new
            {
                success = true,
                searchId,
                count = hotels.Count,
                nights,
                minPrice = result?.MinPrice ?? 0,
                maxPrice = result?.MaxPrice ?? 0,
                destination = request?.Destination,
                checkIn = request?.CheckIn,
                checkOut = request?.CheckOut,
                adults = request?.Adults ?? 2,
                children = request?.Children ?? 0,
                rooms = request?.Rooms ?? 1,
                hotels,
                redirectUrl = Url.Action("Listing", "Hotels", new { searchId })
            });
        }

        [HttpGet]
        public IActionResult Listing(
            string searchId,
            string destination,
            string checkIn,
            string checkOut,
            int adults = 2,
            int children = 0,
            int rooms = 1)
        {
            if (!string.IsNullOrWhiteSpace(searchId))
            {
                var model = _hotelSearchService.GetListing(searchId);
                ViewData["Title"] = string.IsNullOrWhiteSpace(model.Destination)
                    ? "Hotel Listing"
                    : ("Hotels in " + model.Destination);
                return View(model);
            }

            // Immediate redirect from home — show shell + loading, JS will call Search
            var pending = new HotelListingViewModel
            {
                IsPending = true,
                Destination = destination ?? "",
                Adults = adults > 0 ? adults : 2,
                Children = Math.Max(0, children),
                Rooms = rooms > 0 ? rooms : 1,
                Currency = "USD",
                Result = new HotelsResult()
            };

            if (HotelSearchService.TryParseDate(checkIn, out var cin))
                pending.CheckIn = cin;
            if (HotelSearchService.TryParseDate(checkOut, out var cout))
                pending.CheckOut = cout;

            if (pending.CheckIn > DateTime.MinValue && pending.CheckOut > DateTime.MinValue)
                pending.Nights = Math.Max(1, (pending.CheckOut.Date - pending.CheckIn.Date).Days);

            ViewData["Title"] = string.IsNullOrWhiteSpace(pending.Destination)
                ? "Searching Hotels"
                : ("Hotels in " + pending.Destination);

            return View(pending);
        }

        /// <summary>
        /// Called when user clicks a hotel on the listing.
        /// Hits MetaService /api/hotels/details and saves under SearchStore/search_{searchId}/.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Details([FromBody] HotelDetailFormRequest request)
        {
            var (success, result, error) = await _hotelSearchService.GetDetailsAndSaveAsync(request);

            if (!success)
            {
                return Json(new
                {
                    success = false,
                    error = error ?? "Unable to load hotel details.",
                    searchId = request?.SearchId,
                    hotelCode = request?.HotelCode
                });
            }

            return Json(new
            {
                success = true,
                searchId = request?.SearchId,
                hotelCode = request?.HotelCode,
                hotelName = result?.Hotel?.HotelName,
                redirectUrl = Url.Action("Details", "Hotels", new
                {
                    searchId = request?.SearchId,
                    hotelCode = request?.HotelCode
                })
            });
        }

        [HttpGet]
        public IActionResult Details(string searchId, string hotelCode)
        {
            var model = _hotelSearchService.GetSavedDetail(searchId, hotelCode);
            ViewData["Title"] = model.Detail?.Hotel?.HotelName ?? "Hotel Details";
            return View(model);
        }
    }
}
