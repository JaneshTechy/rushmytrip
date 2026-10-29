using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.Datamodel
{
    public partial class CurrencyMaster
    {
        public int Id { get; set; }
        public string BaseCurrency { get; set; }
        public string ChangeCurrency { get; set; }
        public double CurrencyValue { get; set; }
        public string WebsiteId { get; set; }
    }
}
