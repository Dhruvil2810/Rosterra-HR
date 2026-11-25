using System;
using System.Linq;
using System.Web.Mvc;
using ROSTERRA_HR.Filters;
using ROSTERRA_HR.Models;
using ROSTERRA_HR.Repositories;

namespace ROSTERRA_HR.Controllers
{
    /// <summary>
    /// Employee-facing controller for viewing personal leave status and submitting new leave requests.
    /// Strictly guarded by AuthorizeEmployee filter.
    /// </summary>
    [AuthorizeEmployee]
    public class EmployeeLeaveController : Controller
    {
        private readonly EmployeeRepository _employeeRepository;
        private readonly LeaveRepository _leaveRepository;

        public EmployeeLeaveController()
        {
            _employeeRepository = new EmployeeRepository();
            _leaveRepository = new LeaveRepository();
        }

        // GET: /EmployeeLeave
        // GET: /EmployeeLeave/Index
        [HttpGet]
        public ActionResult Index()
        {
            if (Session["UserId"] == null)
            {
                return Redirect("~/Account/Login");
            }

            int userId = Convert.ToInt32(Session["UserId"]);
            var employee = _employeeRepository.GetEmployeeByUserId(userId);

            if (employee == null)
            {
                ViewBag.ErrorMessage = "Employee profile not found.";
                return View(new EmployeeLeaveListViewModel());
            }

            var leaves = _leaveRepository.GetLeavesByEmployee(employee.EmployeeId);
            var blockedDates = _leaveRepository.GetBlockedDatesForEmployee(employee.EmployeeId);
var model = new EmployeeLeaveListViewModel
            {
                EmployeeId = employee.EmployeeId,
                EmployeeName = employee.FullName,
                Department = employee.Department,
                TotalApplied = leaves.Count,
                ApprovedCount = leaves.Count(l => l.Status == "Approved"),
                PendingCount = leaves.Count(l => l.Status == "Pending"),
                RejectedCount = leaves.Count(l => l.Status == "Rejected"),
                Leaves = leaves,
                ApplyForm = new LeaveApplicationViewModel
                {
                    StartDate = DateTime.Today,
                    EndDate = DateTime.Today,
                    BlockedDates = blockedDates
                }
            };

            return View(model);
        }

        // POST: /EmployeeLeave/Apply
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Apply(LeaveApplicationViewModel applyForm)
        {
            if (Session["UserId"] == null)
            {
                return Redirect("~/Account/Login");
            }

            int userId = Convert.ToInt32(Session["UserId"]);
            var employee = _employeeRepository.GetEmployeeByUserId(userId);

            if (employee == null)
            {
                TempData["ErrorMessage"] = "Employee record not found.";
                return RedirectToAction("Index");
            }

            // 1. Validation: Date order
            if (applyForm.EndDate < applyForm.StartDate)
            {
                TempData["ErrorMessage"] = "End Date cannot be earlier than Start Date.";
                return RedirectToAction("Index");
            }

            // 2. Validation: Sunday check
            if (applyForm.StartDate.Date == applyForm.EndDate.Date && applyForm.StartDate.DayOfWeek == DayOfWeek.Sunday)
            {
                TempData["ErrorMessage"] = "Sunday is an official weekly holiday and cannot be requested as leave.";
                return RedirectToAction("Index");
            }

            // 3. Calculate working days excluding Sundays
            int workingDays = LeaveRepository.CalculateWorkingDaysExcludingSundays(applyForm.StartDate, applyForm.EndDate);
            if (workingDays <= 0)
            {
                TempData["ErrorMessage"] = "Selected date range contains no working days (e.g. only Sunday was selected).";
                return RedirectToAction("Index");
            }

            // 4. Validation: Check overlapping active leave
            if (_leaveRepository.HasOverlappingLeave(employee.EmployeeId, applyForm.StartDate, applyForm.EndDate))
            {
                TempData["ErrorMessage"] = "You already have an active leave request covering the selected date(s).";
                return RedirectToAction("Index");
            }

            try
            {
                _leaveRepository.SubmitLeaveRequest(
                    employee.EmployeeId,
                    applyForm.LeaveType,
                    applyForm.StartDate,
                    applyForm.EndDate,
                    applyForm.Reason
                );

                TempData["SuccessMessage"] = $"Your leave request for {workingDays} working day(s) has been submitted for HR approval.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to submit leave request: " + ex.Message;
            }

            return RedirectToAction("Index");
        }
    }
}