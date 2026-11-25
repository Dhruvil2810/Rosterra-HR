using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ROSTERRA_HR.Models
{
    /// <summary>
    /// View model for an Employee applying for a new leave request.
    /// Excludes Sundays and prevents applying on blocked/approved dates.
    /// </summary>
    public class LeaveApplicationViewModel
    {
        [Required(ErrorMessage = "Leave Type is required.")]
        [Display(Name = "Leave Type")]
        public string LeaveType { get; set; } = "Casual Leave";

        [Required(ErrorMessage = "Start Date is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "From Date")]
        public DateTime StartDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "End Date is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "To Date")]
        public DateTime EndDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Please provide a reason for the leave.")]
        [StringLength(500, MinimumLength = 5, ErrorMessage = "Reason must be between 5 and 500 characters.")]
        public string Reason { get; set; }

        public int EstimatedDays { get; set; }

        // List of ISO date strings ('yyyy-MM-dd') already approved or pending for this employee,
        // used to lock calendar pickers in JavaScript / HTML5 min-max validation.
        public List<string> BlockedDates { get; set; } = new List<string>();
    }
}