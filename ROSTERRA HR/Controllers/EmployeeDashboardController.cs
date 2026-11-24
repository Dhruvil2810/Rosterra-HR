using System;
using System.Web.Mvc;
using ROSTERRA_HR.Filters;
using ROSTERRA_HR.Repositories;

namespace ROSTERRA_HR.Controllers
{
    /// <summary>
    /// Serves the Employee Self-Service Dashboard.
    /// Strictly restricted to authenticated accounts with role 'Employee'.
    /// </summary>
    [AuthorizeEmployee]
    public class EmployeeDashboardController : Controller
    {
        private readonly EmployeeRepository _employeeRepository;

        public EmployeeDashboardController()
        {
            _employeeRepository = new EmployeeRepository();
        }

        // GET: /EmployeeDashboard
        // GET: /EmployeeDashboard/Index
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
                return View();
            }

            return View(employee);
        }
    }
}
