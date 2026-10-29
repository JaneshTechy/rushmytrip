using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.Datamodel
{
    public partial class TblCampaign
    {
        public int Id { get; set; }
        public bool? Status { get; set; }
        public int? PaxCount { get; set; }
        public string CampId { get; set; }
        public string Company { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public decimal ExtraMarkup { get; set; }
    }
}
