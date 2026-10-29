using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TravelWeb.Models
{
    public class AdminUserPageVM
    {
        public AdminUserVM FormUser { get; set; }
        public List<AdminUserVM> Users { get; set; }
    }
    public class AdminUserVM
    {
        public int UserId { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required, EmailAddress]
        public string UserEmail { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string UserPhone { get; set; }

        [Required]
        public string AllowedIPAddress { get; set; }

        [Required]
        public int RoleId { get; set; }
        public string RoleName { get; set; }

        public bool IsActive { get; set; }
        public bool IsLocked { get; set; }
        public DateTime? LastLoginAt { get; set; }
    }
}
