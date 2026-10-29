using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.DatabaseContext
{
    public partial class Supplier
    {
        public Supplier()
        {
            SupplierPayments = new HashSet<SupplierPayment>();
        }

        public int SupplierId { get; set; }
        public string Name { get; set; }
        public string ContactPerson { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public bool DelInd { get; set; }
        public int? SupplierFee { get; set; }

        public virtual ICollection<SupplierPayment> SupplierPayments { get; set; }
    }
}
