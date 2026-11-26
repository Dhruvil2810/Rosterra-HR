using System;
using System.Linq;
using System.Web.Mvc;
using ROSTERRA_HR.Filters;
using ROSTERRA_HR.Models;
using ROSTERRA_HR.Repositories;

namespace ROSTERRA_HR.Controllers
{
    /// <summary>
    /// Controller for HR Administrators to review, search, approve, and reject employee leave requests.
    /// Strictly guarded by AuthorizeHR filter.
    /// </summary>
    [AuthorizeHR]
    public class LeaveController : Controller
    {
        private readonly LeaveRepository _leaveRepository;

        public LeaveController()
        {
            _leaveRepository = new LeaveRepository();
        }

        // GET: /Leave
        // GET: /Leave/Index
        [HttpGet]
        public ActionResult Index(string status = "All", string search = null)
        {
            var leaves = _leaveRepository.GetAllLeaves(status, search);

            var model = new HRLeaveManageViewModel
            {
                FilterStatus = status ?? "All",
                SearchTerm = search,
                LeaveRequests = leaves,
                TotalRequests = leaves.Count,
                PendingRequests = leaves.Count(l => l.Status == "Pending"),
                ApprovedRequests = leaves.Count(l => l.Status == "Approved"),
                RejectedRequests = leaves.Count(l => l.Status == "Rejected")
            };

            return View(model);
        }

        // POST: /Leave/Approve
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Approve(int leaveId, string adminRemarks)
        {
            try
            {
                int hrUserId = Convert.ToInt32(Session["UserId"]);
                _leaveRepository.UpdateLeaveStatus(leaveId, "Approved", hrUserId, adminRemarks);
                TempData["SuccessMessage"] = "Leave request has been approved successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to approve leave: " + ex.Message;
            }

            return RedirectToAction("Index");
        }

        // POST: /Leave/Reject
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Reject(int leaveId, string adminRemarks)
        {
            try
            {
                int hrUserId = Convert.ToInt32(Session["UserId"]);
                _leaveRepository.UpdateLeaveStatus(leaveId, "Rejected", hrUserId, adminRemarks);
                TempData["SuccessMessage"] = "Leave request has been rejected.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to reject leave: " + ex.Message;
            }

            return RedirectToAction("Index");
        }
    }
}
