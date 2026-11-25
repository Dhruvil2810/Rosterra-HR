using System;
using System.ComponentModel.DataAnnotations;

namespace ROSTERRA_HR.Models
{
    /// <summary>
    /// View model for creating or modifying a shift.
    /// </summary>
    public class ShiftViewModel
    {
        public int ShiftId { get; set; }

        [Required(ErrorMessage = "Shift name is required.")]
        [Display(Name = "Shift Name")]
        [StringLength(50, ErrorMessage = "Shift name cannot exceed 50 characters.")]
        public string ShiftName { get; set; }

        [Required(ErrorMessage = "Start time is required.")]
        [Display(Name = "Start Time (HH:mm)")]
        [DataType(DataType.Time)]
        public TimeSpan StartTime { get; set; } = new TimeSpan(8, 0, 0);

        [Required(ErrorMessage = "End time is required.")]
        [Display(Name = "End Time (HH:mm)")]
        [DataType(DataType.Time)]
        public TimeSpan EndTime { get; set; } = new TimeSpan(15, 0, 0);

        [Display(Name = "Description / Operational Notes")]
        [StringLength(250, ErrorMessage = "Description cannot exceed 250 characters.")]
        public string Description { get; set; }

        [Display(Name = "Active Status")]
        public bool IsActive { get; set; } = true;
    }
}
