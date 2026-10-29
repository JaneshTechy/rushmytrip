using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.Datamodel
{
    public partial class BusinessDetail
    {
        public int BusinessId { get; set; }
        public int UserId { get; set; }
        public string Gstin { get; set; }
        public string Pan { get; set; }
        public string UdyamNumber { get; set; }
        public string AadhaarNumber { get; set; }
        public string ImportExportCode { get; set; }
        public string Tan { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
