using System;
using System.Linq;
using System.Web.Mvc;
using ROSTERRA_HR.Filters;
using ROSTERRA_HR.Models;
using ROSTERRA_HR.Repositories;

namespace ROSTERRA_HR.Controllers
{
    /// <summary>
    /// Controller for HR Administrators to manage salary structures, generate monthly payrolls,
    /// and issue employee payslips. Strictly guarded by AuthorizeHR filter.
    /// </summary>
    [AuthorizeHR]
    public class PayrollController : Controller
    {
        private readonly PayrollRepository _payrollRepository;

        public PayrollController()
        {
            _payrollRepository = new PayrollRepository();
        }

        // GET: /Payroll
        // GET: /Payroll/Index?month=11&year=2026
        [HttpGet]
        public ActionResult Index(int? month, int? year)
        {
            int selectedMonth = month ?? DateTime.Today.Month;
            int selectedYear = year ?? DateTime.Today.Year;

            var records = _payrollRepository.GetMonthlyPayrolls(selectedMonth, selectedYear);

            var model = new HRPayrollIndexViewModel
            {
                SelectedMonth = selectedMonth,
                SelectedYear = selectedYear,
                PayrollRecords = records,
                TotalEmployeesProcessed = records.Count,
                TotalDisbursed = records.Sum(r => r.NetSalary),
                PaidCount = records.Count(r => r.Status == "Paid"),
                PendingCount = records.Count(r => r.Status == "Generated")
            };

            return View(model);
        }

        // POST: /Payroll/Generate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Generate(int month, int year)
        {
            try
            {
                int hrUserId = Convert.ToInt32(Session["UserId"]);
                _payrollRepository.GenerateMonthlyPayroll(month, year, hrUserId);

                TempData["SuccessMessage"] = $"Payroll cycle for {new DateTime(year, month, 1):MMMM yyyy} generated successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to generate payroll: " + ex.Message;
            }

            return RedirectToAction("Index", new { month = month, year = year });
        }

        // POST: /Payroll/MarkPaid
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MarkPaid(int payrollId, int month, int year)
        {
            try
            {
                _payrollRepository.MarkAsPaid(payrollId);
                TempData["SuccessMessage"] = "Payment status successfully updated to Paid.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to update payment status: " + ex.Message;
            }

            return RedirectToAction("Index", new { month = month, year = year });
        }

        // GET: /Payroll/SalaryStructures
        [HttpGet]
        public ActionResult SalaryStructures()
        {
            var structures = _payrollRepository.GetAllSalaryStructures();
            return View(structures);
        }

        // GET: /Payroll/EditStructure?employeeId=1
        [HttpGet]
        public ActionResult EditStructure(int employeeId)
        {
            var structure = _payrollRepository.GetSalaryStructure(employeeId);
            if (structure == null)
            {
                TempData["ErrorMessage"] = "Employee not found.";
                return RedirectToAction("SalaryStructures");
            }
            return View(structure);
        }

        // POST: /Payroll/EditStructure
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditStructure(SalaryStructure model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                _payrollRepository.SaveSalaryStructure(model.EmployeeId, model.BasicSalary, model.Allowances, model.Deductions);
                TempData["SuccessMessage"] = $"Salary structure for {model.EmployeeName} updated successfully.";
                return RedirectToAction("SalaryStructures");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error saving salary structure: " + ex.Message);
                return View(model);
            }
        }

        // GET: /Payroll/Payslip?id=1
        [HttpGet]
        public ActionResult Payslip(int id)
        {
            var payroll = _payrollRepository.GetPayrollById(id);
            if (payroll == null)
            {
                TempData["ErrorMessage"] = "Payslip record not found.";
                return RedirectToAction("Index");
            }

            return View(payroll);
        }
    }
}
