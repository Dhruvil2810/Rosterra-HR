using System.Collections.Generic;

namespace ROSTERRA_HR.Models
{
    /// <summary>
    /// View model for Employee portal to view their leave history and apply for new leave.
    /// </summary>
    public class EmployeeLeaveListViewModel
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string Department { get; set; }

        public int TotalApplied { get; set; }
        public int ApprovedCount { get; set; }
        public int PendingCount { get; set; }
        public int RejectedCount { get; set; }

        public LeaveApplicationViewModel ApplyForm { get; set; } = new LeaveApplicationViewModel();
        public List<Leave> Leaves { get; set; } = new List<Leave>();
    }
}