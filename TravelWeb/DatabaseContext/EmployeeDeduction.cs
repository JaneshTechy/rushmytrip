using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.DatabaseContext
{
    public partial class EmployeeDeduction
    {
        public int Id { get; set; }
        public Guid EmployeeId { get; set; }
        public Guid PayrollId { get; set; }
        public decimal Lwp { get; set; }
        public decimal Pf { get; set; }
        public decimal Esi { get; set; }
        public decimal Tds { get; set; }
        public decimal Misc { get; set; }

        public virtual Employee Employee { get; set; }
        public virtual EmployeePayroll Payroll { get; set; }
    }
}
