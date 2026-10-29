using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.DatabaseContext
{
    public partial class User
    {
        public User()
        {
            BookingTransactionAssignAgents = new HashSet<BookingTransaction>();
            BookingTransactionUsers = new HashSet<BookingTransaction>();
            SupplierPayments = new HashSet<SupplierPayment>();
        }

        public int UserId { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string UserType { get; set; }
        public bool? IsActive { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime CreatedDate { get; set; }
        public string PageAccess { get; set; }
        public int? RoleId { get; set; }
        public string MobileNo { get; set; }

        public virtual Role Role { get; set; }
        public virtual ICollection<BookingTransaction> BookingTransactionAssignAgents { get; set; }
        public virtual ICollection<BookingTransaction> BookingTransactionUsers { get; set; }
        public virtual ICollection<SupplierPayment> SupplierPayments { get; set; }
    }
}
