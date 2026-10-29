using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.Datamodel
{
    public partial class Message
    {
        public long Id { get; set; }
        public long ResponseId { get; set; }
        public long FromUserId { get; set; }
        public long? ToUserId { get; set; }
        public string Text { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
