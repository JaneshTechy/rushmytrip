using System.ComponentModel.DataAnnotations;

namespace TravelWeb.Models
{
    public class PaymentViewModel
    {
        [Required, Range(0.01, 1000000)]
        public decimal Amount { get; set; }
        public string? Account { get; set; }

        // Token from Accept.js
        public string? OpaqueDataDescriptor { get; set; }
        public string? OpaqueDataValue { get; set; }

        [StringLength(60)]
        public string FirstName { get; set; } = "";

        [StringLength(60)]
        public string LastName { get; set; } = "";

        [EmailAddress, StringLength(100)]
        public string Email { get; set; } = "";
        public string Phone { get; set; }  // <-- add this
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? ZipCode { get; set; }
        public string? invoice { get; set; }

    }
}
