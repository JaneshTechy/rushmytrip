namespace TravelWeb.Models
{
    public class CliqPaymentConfig
    {
        public string ApiUrl { get; set; } = "https://secure.cardflexonline.com/api/transact.php";
        public string SecurityKey { get; set; }
        // optional: add TimeoutSeconds, RetryCount etc
        public int TimeoutSeconds { get; set; } = 30;
        public int RetryCount { get; set; } = 1;
    }
}
