using System.ComponentModel.DataAnnotations;

namespace TravelWeb.Models
{
    public class CliqPaymentRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [Required]
        public string Address1 { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string State { get; set; }
        [Required]
        public string Country { get; set; }

        [Required]
        public string Zip { get; set; }

        public string Email { get; set; }
        public string IpAddress { get; set; }

        [Required]
        public decimal Amount { get; set; }

        public string PaymentToken { get; set; }      
        public string GooglePayPaymentData { get; set; }
        public string ApplePayPaymentData { get; set; }

        [CreditCard]
        public string CardNumber { get; set; }         
        
        [RegularExpression(@"^(0[1-9]|1[0-2])\d{2}$",
    ErrorMessage = "Expiry must be MMYY (01–12 + YY)")]
        public string CardExpiry { get; set; }
        public string CardCvv { get; set; }   
        public string PaymentType { get; set; } = "creditcard"; 
        public string TransactionType { get; set; } = "sale"; 
        public string OrderId { get; set; }
        public string OrderDescription { get; set; }
    }
}
