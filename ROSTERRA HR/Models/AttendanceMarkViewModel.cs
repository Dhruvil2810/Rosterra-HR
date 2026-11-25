using System;
using System.ComponentModel.DataAnnotations;

namespace ROSTERRA_HR.Models
{
    /// <summary>
    /// View model for recording employee punch in or punch out events.
    /// </summary>
    public class AttendanceMarkViewModel
    {
        [Required]
        public int EmployeeId { get; set; }

        public string EmployeeName { get; set; }
        public string ShiftName { get; set; }
        public string ShiftTiming { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime AttendanceDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Punch timestamp is required.")]
        [Display(Name = "Punch Time")]
        [DataType(DataType.DateTime)]
        public DateTime PunchTime { get; set; } = DateTime.Now;

        [Display(Name = "Remarks / Notes")]
        [StringLength(250)]
        public string Remarks { get; set; }
    }
}
