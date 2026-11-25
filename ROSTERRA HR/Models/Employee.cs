using System;

namespace ROSTERRA_HR.Models
{
    /// <summary>
    /// Represents an employee record linked to a system user account.
    /// </summary>
    public class Employee
    {
        public int EmployeeId { get; set; }
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName => $"{FirstName} {LastName}".Trim();
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Department { get; set; }
        public string Position { get; set; }
        public decimal Salary { get; set; }
        public DateTime HireDate { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }

        // Shift metadata
        public int? ShiftId { get; set; }
        public string ShiftName { get; set; }
        public TimeSpan? ShiftStartTime { get; set; }
        public TimeSpan? ShiftEndTime { get; set; }
        public string ShiftTiming => (ShiftStartTime.HasValue && ShiftEndTime.HasValue)
            ? $"{DateTime.Today.Add(ShiftStartTime.Value):hh:mm tt} - {DateTime.Today.Add(ShiftEndTime.Value):hh:mm tt}"
            : "No Shift Assigned";

        // Joined account metadata
        public string Username { get; set; }
    }
}
