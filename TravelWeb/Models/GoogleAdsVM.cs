using System.Collections.Generic;

namespace TravelWeb.Models
{
    public class GoogleAdsVM
    {
        public long TotalClicks { get; set; }
        public long TotalImpressions { get; set; }
        public double TotalCost { get; set; }

        public List<GoogleAdsCampaignStatVM> CampaignStats { get; set; } = new List<GoogleAdsCampaignStatVM>();
        public List<GoogleAdsRegionStatVM> RegionStats { get; set; } = new List<GoogleAdsRegionStatVM>();
        public List<GoogleAdsDeviceStatVM> DeviceStats { get; set; } = new List<GoogleAdsDeviceStatVM>();
    }

    public class GoogleAdsCampaignStatVM
    {
        public string CampaignName { get; set; } = "";
        public string Date { get; set; } = "";
        public long Clicks { get; set; }
        public long Impressions { get; set; }
        public double Ctr { get; set; }
        public double AverageCpc { get; set; }
        public double Cost { get; set; }
    }

    public class GoogleAdsRegionStatVM
    {
        public string CampaignName { get; set; } = "";
        public string Region { get; set; } = "";
        public long Clicks { get; set; }
        public long Impressions { get; set; }
        public double Cost { get; set; }
    }

    public class GoogleAdsDeviceStatVM
    {
        public string Device { get; set; } = "";
        public long Clicks { get; set; }
        public long Impressions { get; set; }
        public double Ctr { get; set; }
        public double Cost { get; set; }
    }
}
