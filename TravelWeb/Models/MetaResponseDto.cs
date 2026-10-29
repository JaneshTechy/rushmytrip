using System.Collections.Generic;

namespace TravelWeb.Models
{
    public class MetaResponseDto
    {
        public class MetaFlightSearchResponse
        {
            public List<MetaFlightItem> Airlines { get; set; } = new List<MetaFlightItem>();
        }
        public class MetaFlightItem
        {
            public string Id { get; set; }
            public string CabinClass { get; set; }   // "EC","PE","BC","FC"
            public double TotalAmt { get; set; }
            public double TotalTax { get; set; }
            public int Stop { get; set; }
            public int TotalDuration { get; set; } // minutes
            public List<MetaOSeg> Segments { get; set; } = new List<MetaOSeg>();
            public string BookUrl { get; set; } // enrichment
            public double ABF { get; set; } // Adult Base Fare
            public double ATX { get; set; } // Adult Tax
            public double AYQ { get; set; } // Adult Fuel Surcharge

            public double CBF { get; set; } // Child Base Fare
            public double CTX { get; set; } // Child Tax
            public double CYQ { get; set; } // Child Fuel Surcharge

            public double IBF { get; set; } // Infant Base Fare
            public double ITX { get; set; } // Infant Tax
            public double IYQ { get; set; } // Infant Fuel Surcharge

            public double YBF { get; set; } // Youth Base Fare
            public double YTF { get; set; } // Youth Tax Fare
            public double YYQ { get; set; } 


        }
        public class MetaOSeg
        {
            public List<Metaseggg> OSeg { get; set; } 
        }

        public class Metaseggg
        {
            public List<MetaISeg> ISeg { get; set; }
        }

        public class MetaISeg
        {
            public string DDat { get; set; }
            public string ADat { get; set; }
            public string DTime { get; set; }
            public string ATime { get; set; }
            public string DApt { get; set; }
            public string AApt { get; set; }
            public string MA { get; set; }
            public string AirlineNm { get; set; }
            public string FN { get; set; }
            public string FBA { get; set; }
            public string DTer { get; set; }
            public string ATer { get; set; }
            public string BC { get; set; }
            public int SegmentDuration { get; set; }
        }

    }
}
