using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.Datamodel
{
    public partial class SiteAddonmaster
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string AddOnValue { get; set; }
        public bool IsActive { get; set; }
    }
}
