using System;
using System.Collections.Generic;

namespace ROSTERRA_HR.Models
{
    /// <summary>
    /// View model for HR day-wise attendance monitoring dashboard.
    /// </summary>
    public class AttendanceDayMonitorViewModel
    {
        public DateTime SelectedDate { get; set; } = DateTime.Today;
        public int TotalActiveEmployees { get; set; }
        public int PresentCount { get; set; }
        public int LateCount { get; set; }
        public int AbsentCount { get; set; }
        public int PendingCount { get; set; }

        public List<AttendanceMonitorItem> Items { get; set; } = new List<AttendanceMonitorItem>();
    }

    public class AttendanceMonitorItem
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string Department { get; set; }
        public string Position { get; set; }
        public int? ShiftId { get; set; }
        public string ShiftName { get; set; }
        public TimeSpan? ShiftStartTime { get; set; }
        public TimeSpan? ShiftEndTime { get; set; }
        public string ShiftTiming => (ShiftStartTime.HasValue && ShiftEndTime.HasValue)
            ? $"{DateTime.Today.Add(ShiftStartTime.Value):hh:mm tt} - {DateTime.Today.Add(ShiftEndTime.Value):hh:mm tt}"
            : "No Shift";

        public int? AttendanceId { get; set; }
        public DateTime? PunchInTime { get; set; }
        public DateTime? PunchOutTime { get; set; }
        public string Status { get; set; } // Present, Late, Absent, Not Marked
        public decimal? TotalHours { get; set; }
        public string Remarks { get; set; }

        public string FormattedPunchIn => PunchInTime.HasValue ? PunchInTime.Value.ToString("hh:mm tt") : "--:--";
        public string FormattedPunchOut => PunchOutTime.HasValue ? PunchOutTime.Value.ToString("hh:mm tt") : "--:--";
    }
}
