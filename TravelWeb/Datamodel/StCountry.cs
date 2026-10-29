using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.Datamodel
{
    public partial class StCountry
    {
        public string CountryCode { get; set; }
        public string CurrencyCode { get; set; }
        public string RelatedCurrencyCode { get; set; }
        public string DefaultLanguageCode { get; set; }
        public string GeogLoc { get; set; }
        public string Nationality { get; set; }
        public short? ZoneCode { get; set; }
        public string IsdCode { get; set; }
    }
}
