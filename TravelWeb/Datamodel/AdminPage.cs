using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.Datamodel
{
    public partial class AdminPage
    {
        public int PageId { get; set; }
        public string Pages { get; set; }
        public string Categories { get; set; }
        public string RoleId { get; set; }
        public bool? IsActive { get; set; }
    }
}
