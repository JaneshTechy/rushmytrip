using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.Datamodel
{
    public partial class FaqMaster
    {
        public int FaqId { get; set; }
        public string ReferenceType { get; set; }
        public int ReferenceId { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public int? SortOrder { get; set; }
        public bool? IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool Delind { get; set; }
    }
}
