using System;
using System.Collections.Generic;

namespace ROSTERRA_HR.Models
{
    /// <summary>
    /// View model for Employee Self-Service Attendance Log and Shift Schedule view.
    /// </summary>
    public class EmployeeAttendanceLogViewModel
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string Department { get; set; }
        public string Position { get; set; }
        public string ShiftName { get; set; }
        public string ShiftTiming { get; set; }

        public Attendance TodayAttendance { get; set; }
        public List<Attendance> History { get; set; } = new List<Attendance>();
    }
}