using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.Datamodel
{
    public partial class AirlineDetail
    {
        public int Id { get; set; }
        public string AirlineName { get; set; }
        public string AirlineImage { get; set; }
        public string AirlineImagePath { get; set; }
        public string AirlineCode { get; set; }
        public string GeneralInformation { get; set; }
        public string Economy { get; set; }
        public string Business { get; set; }
        public string First { get; set; }
        public string PremiumEconomy { get; set; }
        public string AirlineImage1 { get; set; }
        public string AirlineImage2 { get; set; }
    }
}
