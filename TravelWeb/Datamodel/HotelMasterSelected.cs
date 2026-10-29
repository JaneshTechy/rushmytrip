using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.Datamodel
{
    public partial class HotelMasterSelected
    {
        public int Id { get; set; }
        public int? HotelId { get; set; }
        public string SupplierCode { get; set; }
        public string HotelName { get; set; }
        public string RegionId { get; set; }
        public string WebsiteId { get; set; }
        public string Type { get; set; }
        public bool? IsBlackList { get; set; }
    }
}
