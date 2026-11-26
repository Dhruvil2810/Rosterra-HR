using System;

namespace ROSTERRA_HR.Models
{
    /// <summary>
    /// Represents an employee leave application record in dbo.Leaves.
    /// </summary>
    public class Leave
    {
        public int LeaveId { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeCode { get; set; }
        public string Department { get; set; }
        public string Position { get; set; }

        public string LeaveType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalDays { get; set; }
        public string Reason { get; set; }

        public string Status { get; set; } // 'Pending', 'Approved', 'Rejected'
        public string AdminRemarks { get; set; }
        public int? DecidedByUserId { get; set; }
        public string DecidedByUsername { get; set; }
        public DateTime? DecidedAtUtc { get; set; }
        public DateTime CreatedAtUtc { get; set; }

        public string FormattedStartDate => StartDate.ToString("MMM dd, yyyy");
        public string FormattedEndDate => EndDate.ToString("MMM dd, yyyy");
        public string FormattedCreatedAt => CreatedAtUtc.ToLocalTime().ToString("MMM dd, yyyy hh:mm tt");
        public string FormattedDecidedAt => DecidedAtUtc.HasValue ? DecidedAtUtc.Value.ToLocalTime().ToString("MMM dd, yyyy hh:mm tt") : "--";
    }
}
