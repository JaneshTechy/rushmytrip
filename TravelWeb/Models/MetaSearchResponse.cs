using System.Collections.Generic;

namespace TravelWeb.Models
{
    public class MetaSearchResponse
    {
        public string searchId { get; set; }
        public MetaResult result { get; set; }
    }

    public class MetaResult
    {
        public List<MetaAirlineResult> airlines { get; set; }
    }

    public class MetaAirlineResult
    {
        public string id { get; set; }
        public string cabinClass { get; set; }
        public decimal totalAmt { get; set; }
        public int stop { get; set; }
        public int totalDuration { get; set; }
        public List<MetaSegmentContainer> segments { get; set; }
    }

    public class MetaSegmentContainer
    {
        public List<MetaOSeg> oSeg { get; set; }
    }

    public class MetaOSeg
    {
        public List<MetaISeg> iSeg { get; set; }
    }

    public class MetaISeg
    {
        public string dDat { get; set; }
        public string aDat { get; set; }
        public string dTime { get; set; }
        public string aTime { get; set; }
        public string dApt { get; set; }
        public string aApt { get; set; }
        public string ma { get; set; }
        public string airlineNm { get; set; }
        public string fn { get; set; }
        public int segmentDuration { get; set; }
    }

}
