using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.Datamodel
{
    public partial class Supplier
    {
        public int SupplierId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string WhatsApp { get; set; }
        public string WebsiteUrl { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public int? CityIdRef { get; set; }
        public bool IsVerified { get; set; }
        public decimal? RatingAverage { get; set; }
        public string LogoUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Password { get; set; }
        public int? Userid { get; set; }
        public string SalesEmail { get; set; }
        public string Landline { get; set; }
    }
}
