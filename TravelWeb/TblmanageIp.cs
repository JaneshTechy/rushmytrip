using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb
{
    public partial class TblmanageIp
    {
        public int Id { get; set; }
        public string Ipaddress { get; set; }
        public string WebsiteId { get; set; }
        public DateTime? SystemDate { get; set; }
    }
}
