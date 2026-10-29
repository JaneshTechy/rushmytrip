using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using TravelWeb.Helper;
using TravelWeb.Models.Hotel;

namespace TravelWeb.Services
{
    public class HotelSearchService
    {
        private static readonly HttpClient _httpClient;

        private readonly string _apiUrl;
        private readonly string _detailsApiUrl;

        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        static HotelSearchService()
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
                {
                    var host = message?.RequestUri?.Host;
                    if (string.Equals(host, "localhost", StringComparison.OrdinalIgnoreCase)
                        || string.Equals(host, "127.0.0.1", StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                    return errors == System.Net.Security.SslPolicyErrors.None;
                }
            };
            _httpClient = new HttpClient(handler)
            {
                Timeout = TimeSpan.FromSeconds(120)
            };
        }

        public HotelSearchService(IConfiguration configuration)
        {
            _apiUrl = configuration["MetaService:HotelsSearchUrl"]
                      ?? "https://localhost:44393/api/hotels/search";
            _detailsApiUrl = configuration["MetaService:HotelsDetailsUrl"]
                             ?? "https://localhost:44393/api/hotels/details";
        }

        public async Task<(bool Success, string SearchId, HotelsResult Result, string Error)> SearchAndSaveAsync(HotelSearchFormRequest form)
        {
            if (form == null)
                return (false, null, null, "Invalid search request.");

            if (string.IsNullOrWhiteSpace(form.Destination))
                return (false, null, null, "Please enter a destination.");

            if (!TryParseDate(form.CheckIn, out var checkIn) || !TryParseDate(form.CheckOut, out var checkOut))
                return (false, null, null, "Please select valid check-in and check-out dates.");

            if (checkOut <= checkIn)
                return (false, null, null, "Check-out must be after check-in.");

            ParseDestination(form.Destination, out var cityName, out var countryName);

            var rooms = Math.Max(1, Math.Min(form.Rooms, 6));
            var adults = Math.Max(1, Math.Min(form.Adults, 10));
            var children = Math.Max(0, Math.Min(form.Children, 8));

            var apiRequest = new HotelApiSearchRequest
            {
                CheckInDate = checkIn,
                CheckOutDate = checkOut,
                CityName = cityName,
                CountryName = countryName,
                CityId = 0,
                NoOfRooms = rooms,
                GuestNationality = string.IsNullOrWhiteSpace(form.GuestNationality) ? "US" : form.GuestNationality.Trim().ToUpperInvariant(),
                PreferredCurrencyCode = string.IsNullOrWhiteSpace(form.Currency) ? "USD" : form.Currency.Trim().ToUpperInvariant(),
                StarRating = "All",
                OrderBy = "PriceAsc",
                ResultCount = 50,
                IsNearBySearchAllowed = false,
                RoomGuests = BuildRoomGuests(rooms, adults, children)
            };

            HotelsResult apiResult;
            try
            {
                apiResult = await CallMetaHotelsApiAsync(apiRequest);
            }
            catch (Exception ex)
            {
                return (false, null, null, "Hotel search failed: " + ex.Message);
            }

            if (apiResult == null)
                return (false, null, null, "Empty response from hotel search API.");

            if (!string.IsNullOrWhiteSpace(apiResult.Error) && (apiResult.HotelsList == null || apiResult.HotelsList.Count == 0))
                return (false, apiResult.SearchId, apiResult, apiResult.Error);

            var searchId = string.IsNullOrWhiteSpace(apiResult.SearchId)
                ? Guid.NewGuid().ToString("N")
                : apiResult.SearchId;

            apiResult.SearchId = searchId;

            var payload = new HotelSearchStorePayload
            {
                Form = new HotelSearchFormRequest
                {
                    Destination = form.Destination?.Trim(),
                    CheckIn = checkIn.ToString("MM-dd-yyyy"),
                    CheckOut = checkOut.ToString("MM-dd-yyyy"),
                    Adults = adults,
                    Children = children,
                    Rooms = rooms,
                    Currency = apiRequest.PreferredCurrencyCode,
                    GuestNationality = apiRequest.GuestNationality
                },
                ApiRequest = apiRequest,
                Result = apiResult,
                SavedAt = DateTime.UtcNow
            };

            AplicationHelper.Save(searchId, "hotel_search", payload);
            AplicationHelper.Save(searchId, "hotel_result", apiResult);

            return (true, searchId, apiResult, null);
        }

        public HotelListingViewModel GetListing(string searchId)
        {
            var model = new HotelListingViewModel
            {
                SearchId = searchId,
                Result = new HotelsResult { HotelsList = new List<HotelItem>() }
            };

            if (string.IsNullOrWhiteSpace(searchId))
            {
                model.Error = "Missing search id.";
                return model;
            }

            var payload = AplicationHelper.Get<HotelSearchStorePayload>(searchId, "hotel_search");
            if (payload?.Result == null)
            {
                var resultOnly = AplicationHelper.Get<HotelsResult>(searchId, "hotel_result");
                if (resultOnly == null)
                {
                    model.Error = "Hotel search expired. Please search again.";
                    return model;
                }

                model.Result = resultOnly;
                model.Destination = "Hotels";
                return model;
            }

            model.Result = payload.Result ?? new HotelsResult { HotelsList = new List<HotelItem>() };
            model.Destination = payload.Form?.Destination ?? payload.ApiRequest?.CityName ?? "Hotels";
            model.CityName = payload.ApiRequest?.CityName;
            model.CountryName = payload.ApiRequest?.CountryName;
            model.Adults = payload.Form?.Adults ?? 2;
            model.Children = payload.Form?.Children ?? 0;
            model.Rooms = payload.Form?.Rooms ?? 1;
            model.Currency = payload.Form?.Currency ?? "USD";

            if (payload.ApiRequest != null)
            {
                model.CheckIn = payload.ApiRequest.CheckInDate;
                model.CheckOut = payload.ApiRequest.CheckOutDate;
            }
            else if (payload.Form != null)
            {
                TryParseDate(payload.Form.CheckIn, out var cin);
                TryParseDate(payload.Form.CheckOut, out var cout);
                model.CheckIn = cin;
                model.CheckOut = cout;
            }

            model.Nights = Math.Max(1, (model.CheckOut.Date - model.CheckIn.Date).Days);
            return model;
        }

        /// <summary>
        /// Calls MetaService /api/hotels/details and saves request + response
        /// under the same SearchStore/search_{searchId} folder as hotel search.
        /// </summary>
        public async Task<(bool Success, HotelDetailResult Result, string Error)> GetDetailsAndSaveAsync(HotelDetailFormRequest form)
        {
            if (form == null || string.IsNullOrWhiteSpace(form.SearchId))
                return (false, null, "Missing search id. Please search hotels again.");

            if (string.IsNullOrWhiteSpace(form.HotelCode))
                return (false, null, "Missing hotel code.");

            var listing = GetListing(form.SearchId);
            if (!string.IsNullOrWhiteSpace(listing.Error) && (listing.Result?.HotelsList == null || listing.Result.HotelsList.Count == 0))
                return (false, null, listing.Error ?? "Hotel search expired. Please search again.");

            var hotel = listing.Result?.HotelsList?
                .FirstOrDefault(h => string.Equals(h.HotelCode, form.HotelCode, StringComparison.OrdinalIgnoreCase));

            var provider = !string.IsNullOrWhiteSpace(form.Provider)
                ? form.Provider.Trim()
                : (hotel?.Provider ?? "WIKI");

            if (listing.CheckIn == default || listing.CheckOut == default)
                return (false, null, "Missing check-in/check-out for this search.");

            var apiRequest = new HotelDetailApiRequest
            {
                SearchId = form.SearchId.Trim(),
                EntityId = form.HotelCode.Trim(),
                CheckInDate = listing.CheckIn,
                CheckOutDate = listing.CheckOut,
                Adults = Math.Max(1, listing.Adults),
                Children = Math.Max(0, listing.Children),
                Provider = string.IsNullOrWhiteSpace(provider) ? "WIKI" : provider.ToUpperInvariant()
            };

            HotelDetailResult apiResult;
            try
            {
                apiResult = await CallMetaHotelsDetailsApiAsync(apiRequest);
            }
            catch (Exception ex)
            {
                return (false, null, "Hotel details failed: " + ex.Message);
            }

            if (apiResult == null)
                return (false, null, "Empty response from hotel details API.");

            if (string.IsNullOrWhiteSpace(apiResult.SearchId))
                apiResult.SearchId = form.SearchId;

            var fileKey = "hotel_detail_" + SanitizeFilePart(form.HotelCode);
            EnrichHotelFromListing(apiResult, hotel);
            var store = new HotelDetailStorePayload
            {
                Request = apiRequest,
                Result = apiResult,
                SavedAt = DateTime.UtcNow
            };

            AplicationHelper.Save(form.SearchId, fileKey + "_request", apiRequest);
            AplicationHelper.Save(form.SearchId, fileKey, store);
            AplicationHelper.Save(form.SearchId, fileKey + "_result", apiResult);

            if (!string.IsNullOrWhiteSpace(apiResult.Error) && apiResult.Hotel == null)
                return (false, apiResult, apiResult.Error);

            return (true, apiResult, null);
        }

        public HotelDetailViewModel GetSavedDetail(string searchId, string hotelCode)
        {
            var model = new HotelDetailViewModel
            {
                SearchId = searchId,
                HotelCode = hotelCode
            };

            if (string.IsNullOrWhiteSpace(searchId) || string.IsNullOrWhiteSpace(hotelCode))
            {
                model.Error = "Missing search or hotel id.";
                return model;
            }

            var listing = GetListing(searchId);
            model.Destination = listing.Destination;
            model.CheckIn = listing.CheckIn;
            model.CheckOut = listing.CheckOut;
            model.Adults = listing.Adults;
            model.Children = listing.Children;
            model.Rooms = listing.Rooms;

            var listingHotel = listing.Result?.HotelsList?
                .FirstOrDefault(h => string.Equals(h.HotelCode, hotelCode, StringComparison.OrdinalIgnoreCase));

            var fileKey = "hotel_detail_" + SanitizeFilePart(hotelCode);
            var store = AplicationHelper.Get<HotelDetailStorePayload>(searchId, fileKey);
            if (store?.Result != null)
            {
                model.Detail = store.Result;
                EnrichHotelFromListing(model.Detail, listingHotel);
                if (!string.IsNullOrWhiteSpace(store.Result.Error) && store.Result.Hotel == null)
                    model.Error = store.Result.Error;
                return model;
            }

            var resultOnly = AplicationHelper.Get<HotelDetailResult>(searchId, fileKey + "_result");
            if (resultOnly != null)
            {
                model.Detail = resultOnly;
                EnrichHotelFromListing(model.Detail, listingHotel);
                if (!string.IsNullOrWhiteSpace(resultOnly.Error) && resultOnly.Hotel == null)
                    model.Error = resultOnly.Error;
                return model;
            }

            model.Error = "Hotel details not found. Open the hotel from the listing again. ";
            return model;
        }

        /// <summary>
        /// Detail API mock often has empty images — fill from search listing card images.
        /// </summary>
        private static void EnrichHotelFromListing(HotelDetailResult detail, HotelItem listingHotel)
        {
            if (detail?.Hotel == null || listingHotel == null)
                return;

            if (string.IsNullOrWhiteSpace(detail.Hotel.HotelPicture) && !string.IsNullOrWhiteSpace(listingHotel.HotelPicture))
                detail.Hotel.HotelPicture = listingHotel.HotelPicture;

            if (string.IsNullOrWhiteSpace(detail.Hotel.HotelAddress) && !string.IsNullOrWhiteSpace(listingHotel.HotelAddress))
                detail.Hotel.HotelAddress = listingHotel.HotelAddress;

            if (detail.Hotel.Images == null || detail.Hotel.Images.Count == 0)
            {
                if (listingHotel.Images != null && listingHotel.Images.Count > 0)
                {
                    detail.Hotel.Images = listingHotel.Images
                        .Where(i => !string.IsNullOrWhiteSpace(i?.Url))
                        .Select(i => new HotelImageItem { Url = i.Url, Alt = i.Alt })
                        .ToList();
                }
            }

            if ((detail.Hotel.Images == null || detail.Hotel.Images.Count == 0)
                && !string.IsNullOrWhiteSpace(detail.Hotel.HotelPicture))
            {
                detail.Hotel.Images = new List<HotelImageItem>
                {
                    new HotelImageItem { Url = detail.Hotel.HotelPicture, Alt = detail.Hotel.HotelName }
                };
            }
        }

        private async Task<HotelsResult> CallMetaHotelsApiAsync(HotelApiSearchRequest request)
        {
            var json = JsonSerializer.Serialize(request, JsonOptions);
            using var content = new StringContent(json, Encoding.UTF8, "application/json");
            using var response = await _httpClient.PostAsync(_apiUrl, content);
            var raw = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"{(int)response.StatusCode} {response.ReasonPhrase}: {Truncate(raw, 400)}");

            return DeserializePossiblyDoubleEncoded<HotelsResult>(raw)
                   ?? new HotelsResult { HotelsList = new List<HotelItem>() };
        }

        private async Task<HotelDetailResult> CallMetaHotelsDetailsApiAsync(HotelDetailApiRequest request)
        {
            var json = JsonSerializer.Serialize(request, JsonOptions);
            using var content = new StringContent(json, Encoding.UTF8, "application/json");
            using var response = await _httpClient.PostAsync(_detailsApiUrl, content);
            var raw = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"{(int)response.StatusCode} {response.ReasonPhrase}: {Truncate(raw, 400)}");

            return DeserializePossiblyDoubleEncoded<HotelDetailResult>(raw)
                   ?? new HotelDetailResult { SearchId = request.SearchId, Error = "Empty details response." };
        }

        private T DeserializePossiblyDoubleEncoded<T>(string raw) where T : class
        {
            if (string.IsNullOrWhiteSpace(raw))
                return null;

            var trimmed = raw.TrimStart();
            if (trimmed.StartsWith("\""))
            {
                var inner = JsonSerializer.Deserialize<string>(raw);
                if (string.IsNullOrWhiteSpace(inner))
                    return null;
                return JsonSerializer.Deserialize<T>(inner, JsonOptions);
            }

            return JsonSerializer.Deserialize<T>(raw, JsonOptions);
        }

        private static string SanitizeFilePart(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return "unknown";
            var chars = value.Trim().Select(c =>
                char.IsLetterOrDigit(c) || c == '-' || c == '_' ? c : '_').ToArray();
            var s = new string(chars);
            return s.Length > 80 ? s.Substring(0, 80) : s;
        }

        public static void ParseDestination(string destination, out string cityName, out string countryName)
        {
            var value = (destination ?? string.Empty).Trim();
            var comma = value.IndexOf(',');
            if (comma > 0 && comma < value.Length - 1)
            {
                cityName = value.Substring(0, comma).Trim();
                countryName = value.Substring(comma + 1).Trim();
            }
            else
            {
                cityName = value;
                countryName = "United States";
            }

            if (string.IsNullOrWhiteSpace(cityName))
                cityName = value;
            if (string.IsNullOrWhiteSpace(countryName))
                countryName = "United States";
        }

        public static List<HotelRoomGuest> BuildRoomGuests(int rooms, int adults, int children)
        {
            var list = new List<HotelRoomGuest>();
            int adultsLeft = adults;
            int childrenLeft = children;

            for (int i = 0; i < rooms; i++)
            {
                int remainingRooms = rooms - i;
                int a = Math.Max(1, adultsLeft / remainingRooms);
                int c = childrenLeft / remainingRooms;
                list.Add(new HotelRoomGuest
                {
                    AdultCount = a,
                    ChildCount = c,
                    ChildAges = new List<int>()
                });
                adultsLeft -= a;
                childrenLeft -= c;
            }

            return list;
        }

        public static bool TryParseDate(string value, out DateTime date)
        {
            date = default;
            if (string.IsNullOrWhiteSpace(value))
                return false;

            string[] formats =
            {
                "MM-dd-yy", "MM-dd-yyyy", "M-d-yy", "M-d-yyyy",
                "dd MMM yyyy", "d MMM yyyy", "yyyy-MM-dd",
                "MM/dd/yyyy", "M/d/yyyy"
            };

            return DateTime.TryParseExact(value.Trim(), formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out date)
                   || DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
        }

        public static int ParseStarRating(string rating)
        {
            if (string.IsNullOrWhiteSpace(rating))
                return 0;

            var r = rating.Trim().ToLowerInvariant();
            if (r.Contains("five") || r == "5") return 5;
            if (r.Contains("four") || r == "4") return 4;
            if (r.Contains("three") || r == "3") return 3;
            if (r.Contains("two") || r == "2") return 2;
            if (r.Contains("one") || r == "1") return 1;

            if (double.TryParse(rating, NumberStyles.Any, CultureInfo.InvariantCulture, out var n))
                return (int)Math.Round(n);

            return 0;
        }

        public static double ParseGuestRating(string tripAdvisorRating)
        {
            if (string.IsNullOrWhiteSpace(tripAdvisorRating))
                return 0;
            return double.TryParse(tripAdvisorRating, NumberStyles.Any, CultureInfo.InvariantCulture, out var n)
                ? n
                : 0;
        }

        private static string Truncate(string value, int max)
        {
            if (string.IsNullOrEmpty(value) || value.Length <= max)
                return value;
            return value.Substring(0, max) + "...";
        }
    }
}
