using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.Datamodel
{
    public partial class LikedFareBasis
    {
        public int Id { get; set; }
        public string AirCode { get; set; }
        public string FareBasis { get; set; }
        public int? HitCount { get; set; }
    }
}
