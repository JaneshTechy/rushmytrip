using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.DatabaseContext
{
    public partial class EmployeeAttendance
    {
        public int Id { get; set; }
        public Guid EmployeeId { get; set; }
        public int PayrollPeriodId { get; set; }
        public DateTime AttendanceDate { get; set; }
        public DateTime? LoginTime { get; set; }
        public DateTime? LogoutTime { get; set; }
        public string Type { get; set; }
        public bool DelInd { get; set; }

        public virtual Employee Employee { get; set; }
        public virtual PayrollPeriod PayrollPeriod { get; set; }
    }
}
