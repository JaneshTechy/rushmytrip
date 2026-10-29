using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.Datamodel
{
    public partial class AirlineTable
    {
        public string AirlineCode { get; set; }
        public string AirlineName { get; set; }
        public string Imagepath { get; set; }
        public string AirlineImageName { get; set; }
        public bool? Visible { get; set; }
        public string VideoPath { get; set; }
    }
}
