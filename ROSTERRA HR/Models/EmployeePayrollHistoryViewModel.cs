using System.Collections.Generic;

namespace ROSTERRA_HR.Models
{
    /// <summary>
    /// View model for Employee portal to view monthly salary history and payslip.
    /// </summary>
    public class EmployeePayrollHistoryViewModel
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string Department { get; set; }
        public string Position { get; set; }

        public SalaryStructure SalaryStructure { get; set; }
        public List<Payroll> PayrollHistory { get; set; } = new List<Payroll>();
    }
}
