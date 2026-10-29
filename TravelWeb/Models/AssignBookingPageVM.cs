using System.Collections.Generic;
namespace TravelWeb.Models
{
    public class AssignBookingPageVM
    {
        public List<AssignBookingVM> Bookings { get; set; }
        public List<RoleVM> Roles { get; set; }

        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
    }

}
public class RoleVM
{
    public int RoleId { get; set; }
    public string RoleName { get; set; }
}
