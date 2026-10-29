using System;
using System.ComponentModel.DataAnnotations;

namespace TravelWeb.Models
{
    public class FlightSearchRequest
    {
        [Required]
        [StringLength(3, MinimumLength = 3)]
        public string Origin { get; set; } = string.Empty;     // e.g. DEL

        [Required]
        [StringLength(3, MinimumLength = 3)]
        public string Destination { get; set; } = string.Empty; // e.g. BOM

        [Required]
        public DateTime DepartureDate { get; set; }

        public DateTime? ReturnDate { get; set; }

        [Range(1, 9)]
        public int Adult { get; set; } = 1;

        [Range(0, 9)]
        public int Child { get; set; } = 0;

        [Range(0, 9)]
        public int Infant { get; set; } = 0;

        [Required]
        public CabinClass CabinClass { get; set; } = CabinClass.Economy;
        // 2 = Economy
        // 3 = PremiumEconomy
        // 4 = Business
        // 5 = First

        public bool DirectFlight { get; set; } = false;

        public string? PreferredAirline { get; set; }

        public bool FlexibleDate { get; set; } = false;

        public int Youth { get; set; } = 0;
    }
    public enum CabinClass
    {
        Economy = 2,
        PremiumEconomy = 3,
        Business = 4,
        First = 5
    }
}
