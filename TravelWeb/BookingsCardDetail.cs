using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb
{
    public partial class BookingsCardDetail
    {
        public int Id { get; set; }
        public string Website { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CardNumber { get; set; }
        public string CardType { get; set; }
        public string ExpiryDate { get; set; }
        public string ExpiryYear { get; set; }
        public string Cvc { get; set; }
        public string WebsiteId { get; set; }
        public string Url { get; set; }
        public DateTime? Date { get; set; }
        public string Phone { get; set; }
        public int? BookingsId { get; set; }
    }
}
