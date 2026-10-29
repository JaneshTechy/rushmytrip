using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.Datamodel
{
    public partial class BookQuoteMaster
    {
        public int BookId { get; set; }
        public string BookRefNo { get; set; }
        public string WebsiteId { get; set; }
        public string UserHit { get; set; }
        public DateTime DepartDate { get; set; }
        public string EmailId { get; set; }
        public bool? IsActive { get; set; }
    }
}
