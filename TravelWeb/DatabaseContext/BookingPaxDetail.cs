using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.DatabaseContext
{
    public partial class BookingPaxDetail
    {
        public long PaxId { get; set; }
        public long? BookingId { get; set; }
        public string PaxTitle { get; set; }
        public string PaxType { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Dob { get; set; }
        public string MiddleName { get; set; }
        public string Gender { get; set; }
        public bool? Delind { get; set; }
    }
}
