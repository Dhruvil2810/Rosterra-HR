using System;
using System.Web.Mvc;
using ROSTERRA_HR.Filters;
using ROSTERRA_HR.Repositories;

namespace ROSTERRA_HR.Controllers
{
    /// <summary>
    /// Controller managing employee daily attendance monitoring, punch-in, punch-out, and shift compliance for HR.
    /// Strictly guarded by AuthorizeHR filter.
    /// </summary>
    [AuthorizeHR]
    public class AttendanceController : Controller
    {
        private readonly AttendanceRepository _attendanceRepository;
        private readonly EmployeeRepository _employeeRepository;

        public AttendanceController()
        {
            _attendanceRepository = new AttendanceRepository();
            _employeeRepository = new EmployeeRepository();
        }

        // GET: /Attendance
        // GET: /Attendance/Index?date=2026-11-25
        [HttpGet]
        public ActionResult Index(DateTime? date = null)
        {
            DateTime targetDate = date ?? DateTime.Today;
            var model = _attendanceRepository.GetDayMonitor(targetDate);
            return View(model);
        }

        // POST: /Attendance/PunchIn
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PunchIn(int employeeId, DateTime? punchTime = null, string customRemarks = null, string returnDate = null)
        {
            try
            {
                int hrUserId = Session["UserId"] != null ? Convert.ToInt32(Session["UserId"]) : 1;
                DateTime timestamp = punchTime ?? DateTime.Now;

                _attendanceRepository.PunchIn(employeeId, timestamp, hrUserId, customRemarks);

                var emp = _employeeRepository.GetEmployeeById(employeeId);
                TempData["SuccessMessage"] = $"Punch-In recorded for {emp?.FullName} at {timestamp:hh:mm tt}.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error recording Punch-In: " + ex.Message;
            }

            return RedirectToAction("Index", new { date = returnDate });
        }

        // POST: /Attendance/PunchOut
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PunchOut(int employeeId, DateTime? punchTime = null, string customRemarks = null, string returnDate = null)
        {
            try
            {
                int hrUserId = Session["UserId"] != null ? Convert.ToInt32(Session["UserId"]) : 1;
                DateTime timestamp = punchTime ?? DateTime.Now;

                _attendanceRepository.PunchOut(employeeId, timestamp, hrUserId, customRemarks);

                var emp = _employeeRepository.GetEmployeeById(employeeId);
                TempData["SuccessMessage"] = $"Punch-Out recorded for {emp?.FullName} at {timestamp:hh:mm tt}.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error recording Punch-Out: " + ex.Message;
            }

            return RedirectToAction("Index", new { date = returnDate });
        }

        // POST: /Attendance/MarkAbsent
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MarkAbsent(int employeeId, DateTime date, string customRemarks = null, string returnDate = null)
        {
            try
            {
                int hrUserId = Session["UserId"] != null ? Convert.ToInt32(Session["UserId"]) : 1;
                _attendanceRepository.MarkAbsent(employeeId, date, hrUserId, customRemarks);

                var emp = _employeeRepository.GetEmployeeById(employeeId);
                TempData["SuccessMessage"] = $"{emp?.FullName} marked as Absent for {date:MMM dd, yyyy}.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error marking Absent: " + ex.Message;
            }

            return RedirectToAction("Index", new { date = returnDate });
        }
    }
}
