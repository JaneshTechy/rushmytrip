using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb
{
    public partial class ManageUsp
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Detail { get; set; }
        public string Image { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? SystemDate { get; set; }
        public string Websiteid { get; set; }
    }
}
