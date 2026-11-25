using System;
using System.Web.Mvc;
using ROSTERRA_HR.Filters;
using ROSTERRA_HR.Models;
using ROSTERRA_HR.Repositories;
using ROSTERRA_HR.Security;

namespace ROSTERRA_HR.Controllers
{
    /// <summary>
    /// Controller managing Employee CRUD operations for HR administrators.
    /// Fully guarded by server-side AuthorizeHR filter.
    /// </summary>
    [AuthorizeHR]
    public class EmployeeController : Controller
    {
        private readonly EmployeeRepository _employeeRepository;
        private readonly ShiftRepository _shiftRepository;

        public EmployeeController()
        {
            _employeeRepository = new EmployeeRepository();
            _shiftRepository = new ShiftRepository();
        }

        // GET: /Employee
        // GET: /Employee/Index
        [HttpGet]
        public ActionResult Index(string search = null, string department = null, bool? status = null)
        {
            ViewBag.Search = search;
            ViewBag.Department = department;
            ViewBag.Status = status;

            var employees = _employeeRepository.GetAllEmployees(search, department, status);
            return View(employees);
        }

        // GET: /Employee/Details/5
        [HttpGet]
        public ActionResult Details(int id)
        {
            var employee = _employeeRepository.GetEmployeeById(id);
            if (employee == null)
            {
                return HttpNotFound("Employee not found.");
            }

            return View(employee);
        }

        // GET: /Employee/Create
        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.Shifts = new SelectList(_shiftRepository.GetAllShifts(activeOnly: true), "ShiftId", "ShiftName");
            var model = new EmployeeCreateViewModel
            {
                HireDate = DateTime.Today,
                Password = "Emp@" + DateTime.Today.Year
            };
            return View(model);
        }

        // POST: /Employee/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(EmployeeCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Shifts = new SelectList(_shiftRepository.GetAllShifts(activeOnly: true), "ShiftId", "ShiftName", model.ShiftId);
                return View(model);
            }

            try
            {
                // Validate duplicate email
                if (_employeeRepository.IsEmailTaken(model.Email))
                {
                    ModelState.AddModelError("Email", "An employee with this email address already exists.");
                    ViewBag.Shifts = new SelectList(_shiftRepository.GetAllShifts(activeOnly: true), "ShiftId", "ShiftName", model.ShiftId);
                    return View(model);
                }

                // Validate duplicate username
                if (_employeeRepository.IsUsernameTaken(model.Username))
                {
                    ModelState.AddModelError("Username", "This username is already in use. Please select a different username.");
                    ViewBag.Shifts = new SelectList(_shiftRepository.GetAllShifts(activeOnly: true), "ShiftId", "ShiftName", model.ShiftId);
                    return View(model);
                }

                // Generate secure PBKDF2 SHA-256 hash and salt for employee login
                PasswordHasher.CreatePasswordHash(model.Password, out string passwordHash, out string passwordSalt);

                var employee = new Employee
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    Department = model.Department,
                    Position = model.Position,
                    Salary = model.Salary,
                    HireDate = model.HireDate,
                    ShiftId = model.ShiftId
                };

                int newEmployeeId = _employeeRepository.CreateEmployeeWithUser(
                    employee,
                    model.Username,
                    passwordHash,
                    passwordSalt
                );

                TempData["SuccessMessage"] = $"Employee '{employee.FullName}' registered successfully! Generated login username: '{model.Username}'.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while creating the employee: " + ex.Message);
                ViewBag.Shifts = new SelectList(_shiftRepository.GetAllShifts(activeOnly: true), "ShiftId", "ShiftName", model.ShiftId);
                return View(model);
            }
        }

        // GET: /Employee/Edit/5
        [HttpGet]
        public ActionResult Edit(int id)
        {
            var employee = _employeeRepository.GetEmployeeById(id);
            if (employee == null)
            {
                return HttpNotFound("Employee not found.");
            }

            ViewBag.Shifts = new SelectList(_shiftRepository.GetAllShifts(activeOnly: true), "ShiftId", "ShiftName", employee.ShiftId);

            var model = new EmployeeEditViewModel
            {
                EmployeeId = employee.EmployeeId,
                UserId = employee.UserId,
                Username = employee.Username,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Email = employee.Email,
                PhoneNumber = employee.PhoneNumber,
                Department = employee.Department,
                Position = employee.Position,
                Salary = employee.Salary,
                HireDate = employee.HireDate,
                ShiftId = employee.ShiftId,
                IsActive = employee.IsActive
            };

            return View(model);
        }

        // POST: /Employee/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EmployeeEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Shifts = new SelectList(_shiftRepository.GetAllShifts(activeOnly: true), "ShiftId", "ShiftName", model.ShiftId);
                return View(model);
            }

            try
            {
                // Validate duplicate email excluding current employee
                if (_employeeRepository.IsEmailTaken(model.Email, model.EmployeeId))
                {
                    ModelState.AddModelError("Email", "This email address is already assigned to another employee.");
                    ViewBag.Shifts = new SelectList(_shiftRepository.GetAllShifts(activeOnly: true), "ShiftId", "ShiftName", model.ShiftId);
                    return View(model);
                }

                var employee = new Employee
                {
                    EmployeeId = model.EmployeeId,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    Department = model.Department,
                    Position = model.Position,
                    Salary = model.Salary,
                    HireDate = model.HireDate,
                    ShiftId = model.ShiftId
                };

                _employeeRepository.UpdateEmployee(employee);

                TempData["SuccessMessage"] = $"Employee '{employee.FullName}' updated successfully.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while updating the employee: " + ex.Message);
                ViewBag.Shifts = new SelectList(_shiftRepository.GetAllShifts(activeOnly: true), "ShiftId", "ShiftName", model.ShiftId);
                return View(model);
            }
        }

        // POST: /Employee/ToggleStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ToggleStatus(int id)
        {
            var employee = _employeeRepository.GetEmployeeById(id);
            if (employee == null)
            {
                return HttpNotFound("Employee not found.");
            }

            bool newStatus = !employee.IsActive;
            _employeeRepository.SetActiveStatus(id, newStatus);

            TempData["SuccessMessage"] = $"Employee '{employee.FullName}' has been {(newStatus ? "activated" : "deactivated")}.";
            return RedirectToAction("Index");
        }
    }
}
