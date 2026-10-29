using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.DatabaseContext
{
    public partial class Employee
    {
        public Employee()
        {
            EmployeeAddresses = new HashSet<EmployeeAddress>();
            EmployeeAttendances = new HashSet<EmployeeAttendance>();
            EmployeeDeductions = new HashSet<EmployeeDeduction>();
            EmployeeEarnings = new HashSet<EmployeeEarning>();
            EmployeeIncentives = new HashSet<EmployeeIncentive>();
            EmployeePayrolls = new HashSet<EmployeePayroll>();
        }

        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Code { get; set; }
        public string PseudoName { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public bool? Active { get; set; }
        public DateTime? Doj { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? Lwd { get; set; }
        public string AadharCard { get; set; }
        public string Pan { get; set; }
        public Guid? Image { get; set; }
        public int? UserId { get; set; }

        public virtual ICollection<EmployeeAddress> EmployeeAddresses { get; set; }
        public virtual ICollection<EmployeeAttendance> EmployeeAttendances { get; set; }
        public virtual ICollection<EmployeeDeduction> EmployeeDeductions { get; set; }
        public virtual ICollection<EmployeeEarning> EmployeeEarnings { get; set; }
        public virtual ICollection<EmployeeIncentive> EmployeeIncentives { get; set; }
        public virtual ICollection<EmployeePayroll> EmployeePayrolls { get; set; }
    }
}
