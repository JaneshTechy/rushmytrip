using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.Datamodel
{
    public partial class SpecialOfferMiscellaneou
    {
        public int Id { get; set; }
        public int SpecialOfferId { get; set; }
        public string Heading { get; set; }
        public string Value { get; set; }
        public int? Sequence { get; set; }

        public virtual SpecialOffer SpecialOffer { get; set; }
    }
}
