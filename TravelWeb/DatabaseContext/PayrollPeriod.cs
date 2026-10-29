using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.DatabaseContext
{
    public partial class PayrollPeriod
    {
        public PayrollPeriod()
        {
            EmployeeAttendances = new HashSet<EmployeeAttendance>();
            EmployeeIncentives = new HashSet<EmployeeIncentive>();
            EmployeePayrolls = new HashSet<EmployeePayroll>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Twd { get; set; }
        public int Holidays { get; set; }
        public bool Active { get; set; }

        public virtual ICollection<EmployeeAttendance> EmployeeAttendances { get; set; }
        public virtual ICollection<EmployeeIncentive> EmployeeIncentives { get; set; }
        public virtual ICollection<EmployeePayroll> EmployeePayrolls { get; set; }
    }
}
