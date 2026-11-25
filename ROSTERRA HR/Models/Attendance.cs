using System;

namespace ROSTERRA_HR.Models
{
    /// <summary>
    /// Represents a daily attendance log for an employee, tracking punch in/out timestamps and shift compliance.
    /// </summary>
    public class Attendance
    {
        public int AttendanceId { get; set; }
        public int EmployeeId { get; set; }
        public int ShiftId { get; set; }
        public DateTime AttendanceDate { get; set; }
        public DateTime? PunchInTime { get; set; }
        public DateTime? PunchOutTime { get; set; }
        public string Status { get; set; } // Present, Late, Absent, Pending
        public TimeSpan ShiftStartTimeSnapshot { get; set; }
        public TimeSpan ShiftEndTimeSnapshot { get; set; }
        public decimal? TotalHours { get; set; }
        public string Remarks { get; set; }
        public int MarkedByUserId { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }

        // Joined Employee metadata
        public string EmployeeName { get; set; }
        public string Department { get; set; }
        public string Position { get; set; }
        public string ShiftName { get; set; }

        public string FormattedShiftTiming =>
            $"{DateTime.Today.Add(ShiftStartTimeSnapshot):hh:mm tt} - {DateTime.Today.Add(ShiftEndTimeSnapshot):hh:mm tt}";

        public string FormattedPunchIn => PunchInTime.HasValue ? PunchInTime.Value.ToString("hh:mm tt") : "--:--";
        public string FormattedPunchOut => PunchOutTime.HasValue ? PunchOutTime.Value.ToString("hh:mm tt") : "--:--";
    }
}
