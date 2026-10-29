using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.Datamodel
{
    public partial class Group1
    {
        public long GroupId { get; set; }
        public string GroupName { get; set; }
        public long SubCategoryId { get; set; }
        public string ImageUrl { get; set; }
        public long ProductId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string EditedBy { get; set; }
        public DateTime? EditedDate { get; set; }
        public bool? DelInd { get; set; }
        public bool? IsActive { get; set; }
    }
}
