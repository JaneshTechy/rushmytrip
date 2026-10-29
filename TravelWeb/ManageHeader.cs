using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb
{
    public partial class ManageHeader
    {
        public int Id { get; set; }
        public string LogoName { get; set; }
        public string Websiteid { get; set; }
        public string Link { get; set; }
        public string Keyword { get; set; }
        public bool? IsActive { get; set; }
    }
}
