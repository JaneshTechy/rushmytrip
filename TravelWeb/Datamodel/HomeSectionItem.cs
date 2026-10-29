using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.Datamodel
{
    public partial class HomeSectionItem
    {
        public int SectionItemId { get; set; }
        public int SectionIdRef { get; set; }
        public string EntityType { get; set; }
        public int? EntityId { get; set; }
        public string TitleOverride { get; set; }
        public string SubTitleOverride { get; set; }
        public string ImageUrlOverride { get; set; }
        public string LinkUrl { get; set; }
        public string IconName { get; set; }
        public int SortOrder { get; set; }
        public string MetaJson { get; set; }
        public bool? IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
