using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.DatabaseContext
{
    public partial class RoleStatus
    {
        public int RoleStatusId { get; set; }
        public int RoleId { get; set; }
        public int StatusId { get; set; }
        public bool Delind { get; set; }

        public virtual Status Status { get; set; }
    }
}
