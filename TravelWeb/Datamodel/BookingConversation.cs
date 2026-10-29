using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.Datamodel
{
    public partial class BookingConversation
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public string Message { get; set; }
        public int? SenderId { get; set; }
        public string SenderName { get; set; }
        public string SenderRole { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsRead { get; set; }
    }
}
