using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.DatabaseContext
{
    public partial class ApiKey
    {
        public int KeyId { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public DateTime? ExpiryDate { get; set; }
    }
}
