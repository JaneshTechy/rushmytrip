namespace TravelWeb.Models
{
    public class AuthorizeNetPaymentRequest
    {
        public string BillAddress1 { get; set; }
        public string BillAddress2 { get; set; }
        public string BillPostalCode { get; set; }
        public string BillCity { get; set; }
        public string BillState { get; set; }
        public string BillCountry { get; set; }

        public string CardHolderName { get; set; }
        public string CardNumber { get; set; }
        public string CardValMonth { get; set; }
        public string CardValYear { get; set; }
        public string CardSecurityCode { get; set; }
        public string CardType { get; set; }

        public string OpaqueDataDescriptor { get; set; }
        public string OpaqueDataValue { get; set; }

        public string BookingId { get; set; }
        public string SearchId { get; set; }

        public Datamodel.FlightBooking Booking { get; set; }
    }
}
