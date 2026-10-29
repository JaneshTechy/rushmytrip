using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TravelWeb.Models.Hotel
{
    public class HotelSearchFormRequest
    {
        public string Destination { get; set; }
        public string CheckIn { get; set; }
        public string CheckOut { get; set; }
        public int Adults { get; set; } = 2;
        public int Children { get; set; } = 0;
        public int Rooms { get; set; } = 1;
        public string Currency { get; set; } = "USD";
        public string GuestNationality { get; set; } = "US";
    }

    public class HotelApiSearchRequest
    {
        [Required]
        public DateTime CheckInDate { get; set; }

        [Required]
        public DateTime CheckOutDate { get; set; }

        [Required]
        public string CountryName { get; set; } = string.Empty;

        [Required]
        public string CityName { get; set; } = string.Empty;

        public int CityId { get; set; }

        public int NoOfRooms { get; set; } = 1;

        public string GuestNationality { get; set; } = "US";

        public List<HotelRoomGuest> RoomGuests { get; set; } = new List<HotelRoomGuest>();

        public string PreferredCurrencyCode { get; set; } = "USD";

        public string StarRating { get; set; } = "All";

        public string OrderBy { get; set; } = "PriceAsc";

        public int ResultCount { get; set; } = 50;

        public bool IsNearBySearchAllowed { get; set; } = false;
    }

    public class HotelRoomGuest
    {
        public int AdultCount { get; set; } = 1;
        public int ChildCount { get; set; } = 0;
        public List<int> ChildAges { get; set; } = new List<int>();
    }

    public class HotelsResult
    {
        public string Error { get; set; }
        public string SearchId { get; set; } = string.Empty;
        public string SessionId { get; set; }
        public int MinPrice { get; set; }
        public int MaxPrice { get; set; }
        public List<HotelItem> HotelsList { get; set; } = new List<HotelItem>();
    }

    public class HotelItem
    {
        public string Provider { get; set; } = string.Empty;
        public int ResultIndex { get; set; }
        public string HotelCode { get; set; } = string.Empty;
        public string HotelName { get; set; } = string.Empty;
        public string HotelPicture { get; set; } = string.Empty;
        public string HotelDescription { get; set; } = string.Empty;
        public string HotelAddress { get; set; } = string.Empty;
        public string Rating { get; set; } = string.Empty;
        public string Currency { get; set; } = string.Empty;
        public double TotalPrice { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string TripAdvisorRating { get; set; }
        public string TripAdvisorReviewUrl { get; set; }
        public string SearchId { get; set; } = string.Empty;
        public string SessionId { get; set; }
        public List<string> Amenities { get; set; }
        public List<HotelImageItem> Images { get; set; }
    }

    public class HotelImageItem
    {
        public string Url { get; set; }
        public string Alt { get; set; }
    }

    public class HotelListingViewModel
    {
        public string SearchId { get; set; }
        public string Destination { get; set; }
        public string CityName { get; set; }
        public string CountryName { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public int Adults { get; set; } = 2;
        public int Children { get; set; }
        public int Rooms { get; set; } = 1;
        public int Nights { get; set; } = 1;
        public string Currency { get; set; } = "USD";
        public HotelsResult Result { get; set; }
        public string Error { get; set; }
        /// <summary>True when redirected from home before API results are ready.</summary>
        public bool IsPending { get; set; }
    }

    public class HotelSearchStorePayload
    {
        public HotelSearchFormRequest Form { get; set; }
        public HotelApiSearchRequest ApiRequest { get; set; }
        public HotelsResult Result { get; set; }
        public DateTime SavedAt { get; set; } = DateTime.UtcNow;
    }

    /// <summary>Request sent to MetaService POST /api/hotels/details</summary>
    public class HotelDetailApiRequest
    {
        public string SearchId { get; set; } = string.Empty;
        public string EntityId { get; set; } = string.Empty;
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int Adults { get; set; } = 2;
        public int Children { get; set; } = 0;
        public string Provider { get; set; } = "WIKI";
    }

    public class HotelDetailFormRequest
    {
        public string SearchId { get; set; }
        public string HotelCode { get; set; }
        public string Provider { get; set; } = "WIKI";
    }

    public class HotelDetailResult
    {
        public string Error { get; set; }
        public string SearchId { get; set; } = string.Empty;
        public string Provider { get; set; } = string.Empty;
        public HotelDetail Hotel { get; set; }
    }

    public class HotelDetail
    {
        public string EntityId { get; set; } = string.Empty;
        public string HotelName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Rating { get; set; } = string.Empty;
        public double? GuestRating { get; set; }
        public string ReviewCountLabel { get; set; }
        public bool EcoCertified { get; set; }
        public string Deal { get; set; }
        public string About { get; set; }
        public string CheckInTime { get; set; }
        public string CheckOutTime { get; set; }
        public string HotelAddress { get; set; }
        public string Phone { get; set; }
        public string Website { get; set; }
        public string HotelPicture { get; set; }
        public string Currency { get; set; } = string.Empty;
        public int Nights { get; set; }
        public double NightlyPrice { get; set; }
        public double RoomSubtotal { get; set; }
        public double TaxesAndFees { get; set; }
        public double TotalPrice { get; set; }
        public List<string> Amenities { get; set; }
        public List<HotelImageItem> Images { get; set; }
    }

    public class HotelDetailViewModel
    {
        public string SearchId { get; set; }
        public string HotelCode { get; set; }
        public string Destination { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public int Adults { get; set; } = 2;
        public int Children { get; set; }
        public int Rooms { get; set; } = 1;
        public HotelDetailResult Detail { get; set; }
        public string Error { get; set; }
        public bool IsPending { get; set; }
    }

    public class HotelDetailStorePayload
    {
        public HotelDetailApiRequest Request { get; set; }
        public HotelDetailResult Result { get; set; }
        public DateTime SavedAt { get; set; } = DateTime.UtcNow;
    }
}
