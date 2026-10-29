using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb
{
    public partial class UserLogin
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string LastLoginIpAddress { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public DateTime SystemDate { get; set; }
    }
}
