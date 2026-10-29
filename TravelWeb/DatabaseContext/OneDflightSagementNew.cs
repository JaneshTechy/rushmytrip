using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.DatabaseContext
{
    public partial class OneDflightSagementNew
    {
        public long Id { get; set; }
        public long? FlightId { get; set; }
        public string FlightSegIdx { get; set; }
        public string Airline { get; set; }
        public string FromAirportCode { get; set; }
        public string FromTime { get; set; }
        public int? FromDayInterval { get; set; }
        public string ToAirportCode { get; set; }
        public string ToTime { get; set; }
        public int? ToDayInterval { get; set; }
        public string FlightNo { get; set; }
        public string EquipmentType { get; set; }
        public string FlightClass { get; set; }
        public string FromTerminal { get; set; }
        public string ToTerminal { get; set; }
        public string CodeShare { get; set; }
        public string Eft { get; set; }
        public int? IsReturn { get; set; }
        public string Baggage { get; set; }
        public string Status { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
