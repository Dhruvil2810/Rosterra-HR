using System;
using System.Collections.Generic;

namespace ROSTERRA_HR.Models
{
    /// <summary>
    /// View model for HR Admin to monitor, search, and approve/reject employee leave requests.
    /// </summary>
    public class HRLeaveManageViewModel
    {
        public string FilterStatus { get; set; } = "All"; // 'All', 'Pending', 'Approved', 'Rejected'
        public string SearchTerm { get; set; }

        public int TotalRequests { get; set; }
        public int PendingRequests { get; set; }
        public int ApprovedRequests { get; set; }
        public int RejectedRequests { get; set; }

        public List<Leave> LeaveRequests { get; set; } = new List<Leave>();
    }
}
