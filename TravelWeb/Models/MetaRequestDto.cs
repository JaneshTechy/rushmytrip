using System.Collections.Generic;

namespace TravelWeb.Models
{
    public class MetaRequestDto
    {
        public class MetaFlightSearchRequest
        {
            public MetaAdvanceSearch AdvanceSearch { get; set; }
            public MetaGeneralInfo GeneralInfo { get; set; }
            public MetaPaxDetails PaxDetails { get; set; }
            public List<MetaSegment> Segments { get; set; }
            public string Source { get; set; }
            public string Device { get; set; }
        }
        public class MetaAdvanceSearch { public bool DirectFlight { get; set; } }
        public class MetaGeneralInfo
        {
            public string Cabin { get; set; }   // "EC","PE","BC","FC"
            public string Currency { get; set; }
            public string GUID { get; set; }    // ignore: aap generate karoge (optional)
            public string JourneyType { get; set; } // "DOM","INT"
            public string TripType { get; set; } // "O","R","M"
            public string Username { get; set; } // ignore
            public string Password { get; set; } // ignore
            public string IPAddress { get; set; }
        }
        public class MetaPaxDetails { public int Adult { get; set; } public int Child { get; set; } public int Infant { get; set; } public int Youth { get; set; } }
        public class MetaSegment { public string ArrivalApt { get; set; } public string DepartApt { get; set; } public string DepartDate { get; set; } } // yyyy-MM-dd

    }
}
