using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb
{
    public partial class AddOnServiceMaster
    {
        public int Id { get; set; }
        public string Service { get; set; }
        public string Code { get; set; }
        public decimal? Amount { get; set; }
        public string PaymentUrl { get; set; }
        public bool? IsActive { get; set; }
    }
}
