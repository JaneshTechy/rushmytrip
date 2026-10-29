using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.Datamodel
{
    public partial class HomeSection
    {
        public int SectionId { get; set; }
        public string KeyName { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string SectionType { get; set; }
        public string LayoutJson { get; set; }
        public int SortOrder { get; set; }
        public bool? IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
