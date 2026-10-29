using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.Datamodel
{
    public partial class Taxinfo
    {
        public string Id { get; set; }
        public DateTime? CrDt { get; set; }
        public DateTime? LastDt { get; set; }
        public string Depart { get; set; }
        public string Arrive { get; set; }
        public string Cabin { get; set; }
        public string Airline { get; set; }
        public string Ftype { get; set; }
        public decimal? Adttax { get; set; }
        public decimal? YthTax { get; set; }
        public decimal? Chdtax { get; set; }
        public decimal? Inftax { get; set; }
        public decimal? NdAdttax { get; set; }
        public decimal? NdYthTax { get; set; }
        public decimal? NdChdtax { get; set; }
        public decimal? NdInftax { get; set; }
        public string Currency { get; set; }
        public decimal? NdAdttax3 { get; set; }
        public decimal? NdYthTax3 { get; set; }
        public decimal? NdChdtax3 { get; set; }
        public decimal? NdInftax3 { get; set; }
        public decimal? AdtYq { get; set; }
        public decimal? ChdYq { get; set; }
        public decimal? InfYq { get; set; }
        public decimal? NdAdtYq { get; set; }
        public decimal? NdChdYq { get; set; }
        public decimal? NdInfYq { get; set; }
        public decimal? NdAdtYq3 { get; set; }
        public decimal? NdChdYq3 { get; set; }
        public decimal? NdInfYq3 { get; set; }
        public string Class { get; set; }
        public string WebsiteId { get; set; }
        public DateTime? DepartDate { get; set; }
        public DateTime? ReturnDate { get; set; }
    }
}
