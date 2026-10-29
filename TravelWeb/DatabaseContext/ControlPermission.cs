using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.DatabaseContext
{
    public partial class ControlPermission
    {
        public int Cid { get; set; }
        public string Marketing { get; set; }
        public string ProviderName { get; set; }
        public string Platforms { get; set; }
        public string TravolieSitePhone { get; set; }
        public bool TravolieIsActive { get; set; }
        public bool? TravolieSagePayIsActive { get; set; }
    }
}
