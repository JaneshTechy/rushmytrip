using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.Datamodel
{
    public partial class SpecialOfferCategory
    {
        public int Id { get; set; }
        public string CategoryName { get; set; }
        public DateTime? SystemDate { get; set; }
    }
}
