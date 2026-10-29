using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.DatabaseContext
{
    public partial class EmployeeIncentive
    {
        public int Id { get; set; }
        public Guid EmployeeId { get; set; }
        public int PayrollPeriodId { get; set; }
        public decimal Incentive { get; set; }
        public decimal? Rate { get; set; }
        public DateTime? DateCreated { get; set; }
        public bool DelInd { get; set; }

        public virtual Employee Employee { get; set; }
        public virtual PayrollPeriod PayrollPeriod { get; set; }
    }
}
