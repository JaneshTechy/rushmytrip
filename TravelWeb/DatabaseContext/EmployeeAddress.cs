using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.DatabaseContext
{
    public partial class EmployeeAddress
    {
        public int AddressId { get; set; }
        public Guid EmployeeId { get; set; }
        public string AddressType { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostCode { get; set; }
        public string Country { get; set; }
        public Guid? DocumentId { get; set; }

        public virtual Employee Employee { get; set; }
    }
}
