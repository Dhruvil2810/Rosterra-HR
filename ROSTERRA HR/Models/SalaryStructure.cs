using System;
using System.ComponentModel.DataAnnotations;

namespace ROSTERRA_HR.Models
{
    /// <summary>
    /// Represents the base compensation configuration for an employee in dbo.SalaryStructures.
    /// </summary>
    public class SalaryStructure
    {
        public int SalaryStructureId { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string Department { get; set; }
        public string Position { get; set; }

        [Required]
        [Range(0, 10000000, ErrorMessage = "Basic Salary must be greater than or equal to 0.")]
        [Display(Name = "Basic Salary")]
        public decimal BasicSalary { get; set; }

        [Range(0, 10000000, ErrorMessage = "Allowances must be greater than or equal to 0.")]
        [Display(Name = "Allowances")]
        public decimal Allowances { get; set; }

        [Range(0, 10000000, ErrorMessage = "Deductions must be greater than or equal to 0.")]
        [Display(Name = "Standard Deductions")]
        public decimal Deductions { get; set; }

        public decimal GrossSalary => BasicSalary + Allowances;
        public decimal NetEstimatedSalary => Math.Max(0, GrossSalary - Deductions);

        public DateTime CreatedAtUtc { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }
    }
}
