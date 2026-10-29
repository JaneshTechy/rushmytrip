using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.Datamodel
{
    public partial class Buyer
    {
        public int BuyerId { get; set; }
        public string Name { get; set; }
        public string ContactName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string WhatsApp { get; set; }
        public string WebsiteUrl { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public int? CityIdRef { get; set; }
        public string Gstin { get; set; }
        public string LogoUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Password { get; set; }
    }
}
