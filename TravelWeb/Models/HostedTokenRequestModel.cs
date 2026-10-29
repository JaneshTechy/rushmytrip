namespace TravelWeb.Models
{
    public class HostedTokenRequestModel
    {
        public decimal Amount { get; set; }
        public string OrderId { get; set; }
        public string Description { get; set; }
        public string ReturnUrl { get; set; }    // e.g. https://yourdomain.com/Payments/Return
        public string CancelUrl { get; set; }    // optional
    }
}
