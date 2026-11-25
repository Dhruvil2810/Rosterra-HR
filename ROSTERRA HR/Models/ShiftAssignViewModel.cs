using System.ComponentModel.DataAnnotations;

namespace ROSTERRA_HR.Models
{
    /// <summary>
    /// View model for mapping an employee to a designated work shift.
    /// </summary>
    public class ShiftAssignViewModel
    {
        [Required]
        public int EmployeeId { get; set; }

        public string EmployeeName { get; set; }
        public string Department { get; set; }
        public string Position { get; set; }
        public string CurrentShiftName { get; set; }

        [Required(ErrorMessage = "Please select a shift.")]
        [Display(Name = "Assigned Shift")]
        public int ShiftId { get; set; }
    }
}
