using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.Datamodel
{
    public partial class FareinfoMaster
    {
        public string Id { get; set; }
        public DateTime? CrDt { get; set; }
        public DateTime? LastDt { get; set; }
        public string Depart { get; set; }
        public string Arrive { get; set; }
        public string Cabin { get; set; }
        public string Airline { get; set; }
        public string Ftype { get; set; }
        public decimal? Adtfare { get; set; }
        public decimal? Chdfare { get; set; }
        public decimal? Inffare { get; set; }
        public decimal? NdAdtfare { get; set; }
        public decimal? NdChdfare { get; set; }
        public decimal? NdInffare { get; set; }
        public string Currency { get; set; }
        public decimal? NdAdtfare3 { get; set; }
        public decimal? NdChdfare3 { get; set; }
        public decimal? NdInffare3 { get; set; }
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
    }
}
