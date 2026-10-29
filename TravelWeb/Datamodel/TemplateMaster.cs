using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.Datamodel
{
    public partial class TemplateMaster
    {
        public int Id { get; set; }
        public string Category { get; set; }
        public string Code { get; set; }
        public string PageName { get; set; }
        public string AdminDisplayName { get; set; }
        public string Description { get; set; }
    }
}
