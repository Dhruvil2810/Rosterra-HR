using System;

namespace ROSTERRA_HR.Models
{
    /// <summary>
    /// Represents a generated monthly payroll record in dbo.Payrolls.
    /// </summary>
    public class Payroll
    {
        public int PayrollId { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeCode { get; set; }
        public string Department { get; set; }
        public string Position { get; set; }
        public string Email { get; set; }

        public int Month { get; set; }
        public int Year { get; set; }

        public int TotalWorkingDays { get; set; }
        public int PresentDays { get; set; }
        public int AbsentDays { get; set; }

        public decimal BasicSalary { get; set; }
        public decimal Allowances { get; set; }
        public decimal GrossSalary { get; set; }
        public decimal StandardDeductions { get; set; }
        public decimal LossOfPay { get; set; }
        public decimal NetSalary { get; set; }

        public string Status { get; set; } // 'Generated', 'Paid'
        public DateTime? PaymentDate { get; set; }
        public int? GeneratedByUserId { get; set; }
        public string GeneratedByUsername { get; set; }
        public DateTime CreatedAtUtc { get; set; }

        public string MonthName => new DateTime(Year, Month, 1).ToString("MMMM yyyy");
        public string FormattedGross => GrossSalary.ToString("C");
        public string FormattedNet => NetSalary.ToString("C");
        public string FormattedPaymentDate => PaymentDate.HasValue ? PaymentDate.Value.ToString("MMM dd, yyyy") : "--";
    }
}
