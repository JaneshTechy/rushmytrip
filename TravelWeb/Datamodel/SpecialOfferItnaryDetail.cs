using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.Datamodel
{
    public partial class SpecialOfferItnaryDetail
    {
        public int Id { get; set; }
        public int? PackageId { get; set; }
        public int? Days { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}
