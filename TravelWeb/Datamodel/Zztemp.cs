using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.Datamodel
{
    public partial class Zztemp
    {
        public int SiteId { get; set; }
        public string SiteUrl { get; set; }
        public string CompanyName { get; set; }
        public string SeAppDomain { get; set; }
        public string AdminEmailId { get; set; }
        public string ContactNo { get; set; }
        public string SaAccountNo { get; set; }
        public string SkyresAgentId { get; set; }
        public string SkyresAgentPwd { get; set; }
        public string SkyresAccountNo { get; set; }
        public string SePaymentHandlerGateway { get; set; }
        public string SearchType { get; set; }
        public string BookedOffers { get; set; }
        public string TermUrl { get; set; }
        public string CertPath { get; set; }
        public string SiteReference { get; set; }
        public decimal? DefaultMarkup { get; set; }
        public string SaBrid { get; set; }
        public string SaId { get; set; }
        public string WebSiteId { get; set; }
        public string GdsUrl { get; set; }
        public string TargetBranch { get; set; }
        public string PrefferedSearchEngine { get; set; }
        public bool? IsTransparent { get; set; }
        public string ThemeColor { get; set; }
        public string AllowedHotelRatings { get; set; }
        public bool? IsLivePaymentGateway { get; set; }
        public DateTime? SystemDate { get; set; }
        public string PaymentGateway { get; set; }
        public bool? IsAutoSpecialOffer { get; set; }
        public bool? IsBookable { get; set; }
        public bool? IsRounded { get; set; }
        public string SearchEngineBg { get; set; }
        public string Language { get; set; }
        public string Currency { get; set; }
        public bool? PackageEnquiryOnly { get; set; }
        public bool? IsFlightPlusHotel { get; set; }
        public bool? IsPackage { get; set; }
        public bool? CallOnly { get; set; }
        public bool? IsAirlineShow { get; set; }
        public bool? IsCountryShow { get; set; }
        public string CurrencySymbol { get; set; }
        public string Template { get; set; }
        public string TemplateCustom { get; set; }
        public string MasterTemplate { get; set; }
    }
}
