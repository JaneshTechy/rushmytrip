using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.Datamodel
{
    public partial class PaymentEvent
    {
        public int Id { get; set; }
        public int? PaymentId { get; set; }
        public string EventType { get; set; }
        public string PayloadJson { get; set; }
        public DateTime? ReceivedAt { get; set; }
    }
}
