using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.Datamodel
{
    public partial class Specification
    {
        public int SpecificationId { get; set; }
        public string Name { get; set; }
        public string DataType { get; set; }
        public int ProductId { get; set; }
    }
}
