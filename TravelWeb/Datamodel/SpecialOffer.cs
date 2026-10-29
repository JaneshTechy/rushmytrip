using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.Datamodel
{
    public partial class SpecialOffer
    {
        public SpecialOffer()
        {
            SpecialOfferMiscellaneous = new HashSet<SpecialOfferMiscellaneou>();
        }

        public int Id { get; set; }
        public string OfferType { get; set; }
        public string Title { get; set; }
        public string Images { get; set; }
        public string CabinClass { get; set; }
        public string JournyType { get; set; }
        public string HotelName { get; set; }
        public int? Rating { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string Description { get; set; }
        public decimal? Amount { get; set; }
        public bool? IsActive { get; set; }
        public string WebsiteId { get; set; }
        public string Airline { get; set; }
        public string Destination { get; set; }
        public bool? IsPayable { get; set; }
        public decimal? DepositPercent { get; set; }
        public int? CategoryId { get; set; }
        public bool? IsFeature { get; set; }
        public string FareType { get; set; }
        public string Origin { get; set; }
        public bool? IsAutomated { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string GeneralOfferUrl { get; set; }
        public string MetaDescrption { get; set; }
        public string MetaKeywords { get; set; }
        public string OffersCategory { get; set; }
        public string InclusionExclusionId { get; set; }
        public string ExclusionId { get; set; }
        public string CityCode { get; set; }
        public string CountryName { get; set; }
        public bool? IsSlider { get; set; }

        public virtual ICollection<SpecialOfferMiscellaneou> SpecialOfferMiscellaneous { get; set; }
    }
}
