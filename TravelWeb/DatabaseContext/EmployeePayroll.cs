using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.DatabaseContext
{
    public partial class EmployeePayroll
    {
        public EmployeePayroll()
        {
            EmployeeDeductions = new HashSet<EmployeeDeduction>();
            EmployeeEarnings = new HashSet<EmployeeEarning>();
        }

        public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public int PayrollPeriodId { get; set; }
        public decimal? Earnings { get; set; }
        public decimal? Deductions { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? LastModified { get; set; }
        public int? UserId { get; set; }
        public bool Released { get; set; }
        public DateTime? ReleasedOn { get; set; }
        public int? ReleasedBy { get; set; }

        public virtual Employee Employee { get; set; }
        public virtual PayrollPeriod PayrollPeriod { get; set; }
        public virtual ICollection<EmployeeDeduction> EmployeeDeductions { get; set; }
        public virtual ICollection<EmployeeEarning> EmployeeEarnings { get; set; }
    }
}
