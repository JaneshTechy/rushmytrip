namespace TravelWeb.Models
{
    public class AuthorizeNetPaymentResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string TransactionId { get; set; }
        public int? PaymentId { get; set; }
    }
}
