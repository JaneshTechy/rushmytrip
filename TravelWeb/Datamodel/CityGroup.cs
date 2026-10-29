using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.Datamodel
{
    public partial class CityGroup
    {
        public int Id { get; set; }
        public string CityGroupName { get; set; }
        public string CityCode { get; set; }
        public string SaAccountNo { get; set; }
        public DateTime SystemDate { get; set; }
        public string GroupType { get; set; }
    }
}
