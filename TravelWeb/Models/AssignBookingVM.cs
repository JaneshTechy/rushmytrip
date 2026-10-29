using System;
namespace TravelWeb.Models
{
    public class AssignBookingVM
    {
        public long BookingId { get; set; }
        public string BookingNumber { get; set; }
        public DateTime? BookingDate { get; set; }
        public string FromDeparture { get; set; }
        public string ToDestination { get; set; }
        public string Status { get; set; }

        public int? AgentId { get; set; }
        public string AgentName { get; set; }
    }
}
