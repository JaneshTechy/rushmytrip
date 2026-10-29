using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.Datamodel
{
    public partial class ManageMenu
    {
        public int Id { get; set; }
        public string MenuName { get; set; }
        public bool IsActive { get; set; }
        public string Websiteid { get; set; }
        public string MenuLink { get; set; }
        public int? Squence { get; set; }
        public string Icon { get; set; }
        public bool? IsExternal { get; set; }
        public bool? IsSubMenu { get; set; }
        public bool? IsNewWindow { get; set; }
    }
}
