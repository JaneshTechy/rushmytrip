using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.Datamodel
{
    public partial class MembershipPlan
    {
        public int PlanId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int MaxProductimg { get; set; }
        public int MaxProductPages { get; set; }
        public bool CanLiveChat { get; set; }
        public bool CanGmb { get; set; }
        public int MaxVideoLinks { get; set; }
        public int BannerKeywordsCount { get; set; }
        public int VerifiedBuyerInquiries { get; set; }
        public bool HasTrustedSellerStamp { get; set; }
        public bool HasPersonalLogo { get; set; }
        public bool HasBrochure { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
