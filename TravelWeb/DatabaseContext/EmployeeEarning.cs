using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.DatabaseContext
{
    public partial class EmployeeEarning
    {
        public int Id { get; set; }
        public Guid EmployeeId { get; set; }
        public Guid PayrollId { get; set; }
        public decimal Basic { get; set; }
        public decimal Hra { get; set; }
        public decimal Variable { get; set; }
        public decimal SpecialAllowance { get; set; }
        public decimal Da { get; set; }
        public decimal Arrears { get; set; }
        public decimal Bonus { get; set; }
        public decimal Incentive { get; set; }
        public decimal Rate { get; set; }
        public decimal? GrossTotal { get; set; }

        public virtual Employee Employee { get; set; }
        public virtual EmployeePayroll Payroll { get; set; }
    }
}
