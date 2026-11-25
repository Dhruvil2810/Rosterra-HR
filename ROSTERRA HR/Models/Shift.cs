using System;

namespace ROSTERRA_HR.Models
{
    /// <summary>
    /// Represents a work shift with designated daily operating hours.
    /// </summary>
    public class Shift
    {
        public int ShiftId { get; set; }
        public string ShiftName { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAtUtc { get; set; }

        public string FormattedStartTime => DateTime.Today.Add(StartTime).ToString("hh:mm tt");
        public string FormattedEndTime => DateTime.Today.Add(EndTime).ToString("hh:mm tt");
        public string FormattedTiming => $"{FormattedStartTime} - {FormattedEndTime}";
    }
}
