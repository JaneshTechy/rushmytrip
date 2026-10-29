using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb
{
    public partial class Blog
    {
        public int BlogId { get; set; }
        public string Title { get; set; }
        public string DescriptionFile { get; set; }
        public string Websiteid { get; set; }
        public string Imagefile { get; set; }
        public DateTime? SystemDate { get; set; }
    }
}
