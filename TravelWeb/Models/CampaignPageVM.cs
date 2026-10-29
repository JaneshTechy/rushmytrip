using System;
using System.Collections.Generic;

namespace TravelWeb.Models
{
    public class CampaignPageVM
    {
        public CampaignDto Form { get; set; }
        public List<CampaignDto> Campaigns { get; set; }
    }
    public class CampaignDto
    {
        public int Id { get; set; }
        public bool Status { get; set; }
        public int PaxCount { get; set; }
        public string CampID { get; set; }
        public string Company { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public decimal ExtraMarkup { get; set; }
    }
}
