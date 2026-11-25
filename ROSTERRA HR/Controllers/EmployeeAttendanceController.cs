using System;
using System.Web.Mvc;
using ROSTERRA_HR.Filters;
using ROSTERRA_HR.Models;
using ROSTERRA_HR.Repositories;

namespace ROSTERRA_HR.Controllers
{
    /// <summary>
    /// Employee-facing controller for reviewing personal shift timing and daily punch-in/out logs.
    /// Strictly guarded by AuthorizeEmployee filter.
    /// </summary>
    [AuthorizeEmployee]
    public class EmployeeAttendanceController : Controller
    {
        private readonly EmployeeRepository _employeeRepository;
        private readonly AttendanceRepository _attendanceRepository;

        public EmployeeAttendanceController()
        {
            _employeeRepository = new EmployeeRepository();
            _attendanceRepository = new AttendanceRepository();
        }

        // GET: /EmployeeAttendance
        // GET: /EmployeeAttendance/Index
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
                return View(new EmployeeAttendanceLogViewModel());
            }

            var todayAttendance = _attendanceRepository.GetAttendance(employee.EmployeeId, DateTime.Today);
            var history = _attendanceRepository.GetEmployeeAttendanceHistory(employee.EmployeeId);

            var model = new EmployeeAttendanceLogViewModel
            {
                EmployeeId = employee.EmployeeId,
                EmployeeName = employee.FullName,
                Department = employee.Department,
                Position = employee.Position,
                ShiftName = employee.ShiftName ?? "Standard Shift",
                ShiftTiming = employee.ShiftTiming,
                TodayAttendance = todayAttendance,
                History = history
            };

            return View(model);
        }
    }
}
