using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.Datamodel
{
    public partial class AdminUser
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string PasswordHash { get; set; }
        public string UserPhone { get; set; }
        public int RoleId { get; set; }
        public bool? IsActive { get; set; }
        public bool IsLocked { get; set; }
        public int? UserCreatedBy { get; set; }
        public DateTime UserCreatedAt { get; set; }
        public DateTime UserUpdatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public string AllowedIpaddress { get; set; }
    }
}
