using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb
{
    public partial class OfferMarkupTblUpdateDetail
    {
        public int Id { get; set; }
        public DateTime? OfferTblUpdatedDate { get; set; }
        public DateTime? MarkupTblUpdatedDate { get; set; }
        public string WebsiteId { get; set; }
    }
}
