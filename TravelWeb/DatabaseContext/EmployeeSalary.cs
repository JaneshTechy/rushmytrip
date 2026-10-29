using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.DatabaseContext
{
    public partial class EmployeeSalary
    {
        public int Id { get; set; }
        public Guid EmployeeId { get; set; }
        public decimal? Basic { get; set; }
        public decimal? Hra { get; set; }
        public decimal? SpecialAllowance { get; set; }
        public decimal? Variable { get; set; }
        public decimal? Da { get; set; }
        public decimal? Incentive { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal? GrossTotal { get; set; }
    }
}
