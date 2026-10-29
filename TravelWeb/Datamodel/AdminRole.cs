using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.Datamodel
{
    public partial class AdminRole
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public bool? DelInd { get; set; }
        public bool? IsActive { get; set; }
    }
}
