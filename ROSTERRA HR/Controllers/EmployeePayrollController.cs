using System;
using System.Web.Mvc;
using ROSTERRA_HR.Filters;
using ROSTERRA_HR.Models;
using ROSTERRA_HR.Repositories;

namespace ROSTERRA_HR.Controllers
{
    /// <summary>
    /// Employee-facing controller for reviewing personal salary history and digital payslips.
    /// Strictly guarded by AuthorizeEmployee filter.
    /// </summary>
    [AuthorizeEmployee]
    public class EmployeePayrollController : Controller
    {
        private readonly EmployeeRepository _employeeRepository;
        private readonly PayrollRepository _payrollRepository;

        public EmployeePayrollController()
        {
            _employeeRepository = new EmployeeRepository();
            _payrollRepository = new PayrollRepository();
        }

        // GET: /EmployeePayroll
        // GET: /EmployeePayroll/Index
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
                return View(new EmployeePayrollHistoryViewModel());
            }

            var structure = _payrollRepository.GetSalaryStructure(employee.EmployeeId);
            var history = _payrollRepository.GetPayrollHistoryForEmployee(employee.EmployeeId);

            var model = new EmployeePayrollHistoryViewModel
            {
                EmployeeId = employee.EmployeeId,
                EmployeeName = employee.FullName,
                Department = employee.Department,
                Position = employee.Position,
                SalaryStructure = structure,
                PayrollHistory = history
            };

            return View(model);
        }

        // GET: /EmployeePayroll/Payslip?id=1
        [HttpGet]
        public ActionResult Payslip(int id)
        {
            if (Session["UserId"] == null)
            {
                return Redirect("~/Account/Login");
            }

            int userId = Convert.ToInt32(Session["UserId"]);
            var employee = _employeeRepository.GetEmployeeByUserId(userId);

            if (employee == null)
            {
                return Redirect("~/Account/Login");
            }

            var payroll = _payrollRepository.GetPayrollById(id);

            // Ensure employee can ONLY view their own payslip
            if (payroll == null || payroll.EmployeeId != employee.EmployeeId)
            {
                TempData["ErrorMessage"] = "Unauthorized access or payslip not found.";
                return RedirectToAction("Index");
            }

            return View(payroll);
        }
    }
}
