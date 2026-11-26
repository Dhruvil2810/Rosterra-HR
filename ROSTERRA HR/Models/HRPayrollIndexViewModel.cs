using System;
using System.Collections.Generic;

namespace ROSTERRA_HR.Models
{
    /// <summary>
    /// View model for HR Administrator to manage monthly payrolls and generate new payroll cycles.
    /// </summary>
    public class HRPayrollIndexViewModel
    {
        public int SelectedMonth { get; set; } = DateTime.Today.Month;
        public int SelectedYear { get; set; } = DateTime.Today.Year;

        public decimal TotalDisbursed { get; set; }
        public int TotalEmployeesProcessed { get; set; }
        public int PaidCount { get; set; }
        public int PendingCount { get; set; }

        public List<Payroll> PayrollRecords { get; set; } = new List<Payroll>();
    }
}
