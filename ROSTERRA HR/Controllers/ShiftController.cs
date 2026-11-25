using System;
using System.Web.Mvc;
using ROSTERRA_HR.Filters;
using ROSTERRA_HR.Models;
using ROSTERRA_HR.Repositories;

namespace ROSTERRA_HR.Controllers
{
    /// <summary>
    /// Controller managing work shift definitions and employee shift assignments for HR.
    /// Strictly guarded by AuthorizeHR filter.
    /// </summary>
    [AuthorizeHR]
    public class ShiftController : Controller
    {
        private readonly ShiftRepository _shiftRepository;
        private readonly EmployeeRepository _employeeRepository;

        public ShiftController()
        {
            _shiftRepository = new ShiftRepository();
            _employeeRepository = new EmployeeRepository();
        }

        // GET: /Shift
        // GET: /Shift/Index
        [HttpGet]
        public ActionResult Index()
        {
            var shifts = _shiftRepository.GetAllShifts(activeOnly: false);
            return View(shifts);
        }

        // GET: /Shift/Create
        [HttpGet]
        public ActionResult Create()
        {
            var model = new ShiftViewModel
            {
                StartTime = new TimeSpan(8, 0, 0),
                EndTime = new TimeSpan(15, 0, 0),
                IsActive = true
            };
            return View(model);
        }

        // POST: /Shift/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ShiftViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (_shiftRepository.IsShiftNameTaken(model.ShiftName))
            {
                ModelState.AddModelError("ShiftName", "A shift with this name already exists.");
                return View(model);
            }

            try
            {
                var shift = new Shift
                {
                    ShiftName = model.ShiftName.Trim(),
                    StartTime = model.StartTime,
                    EndTime = model.EndTime,
                    Description = model.Description?.Trim(),
                    IsActive = model.IsActive
                };

                _shiftRepository.CreateShift(shift);

                TempData["SuccessMessage"] = $"Shift '{shift.ShiftName}' ({shift.FormattedTiming}) created successfully.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error creating shift: " + ex.Message);
                return View(model);
            }
        }

        // GET: /Shift/Edit/5
        [HttpGet]
        public ActionResult Edit(int id)
        {
            var shift = _shiftRepository.GetShiftById(id);
            if (shift == null)
            {
                return HttpNotFound("Shift not found.");
            }

            var model = new ShiftViewModel
            {
                ShiftId = shift.ShiftId,
                ShiftName = shift.ShiftName,
                StartTime = shift.StartTime,
                EndTime = shift.EndTime,
                Description = shift.Description,
                IsActive = shift.IsActive
            };

            return View(model);
        }

        // POST: /Shift/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ShiftViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (_shiftRepository.IsShiftNameTaken(model.ShiftName, model.ShiftId))
            {
                ModelState.AddModelError("ShiftName", "A shift with this name already exists.");
                return View(model);
            }

            try
            {
                var shift = new Shift
                {
                    ShiftId = model.ShiftId,
                    ShiftName = model.ShiftName.Trim(),
                    StartTime = model.StartTime,
                    EndTime = model.EndTime,
                    Description = model.Description?.Trim(),
                    IsActive = model.IsActive
                };

                _shiftRepository.UpdateShift(shift);

                TempData["SuccessMessage"] = $"Shift '{shift.ShiftName}' updated successfully.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error updating shift: " + ex.Message);
                return View(model);
            }
        }

        // GET: /Shift/Assign?employeeId=5
        [HttpGet]
        public ActionResult Assign(int? employeeId)
        {
            ViewBag.Shifts = new SelectList(_shiftRepository.GetAllShifts(activeOnly: true), "ShiftId", "ShiftName");
            ViewBag.Employees = new SelectList(_employeeRepository.GetAllEmployees(activeOnly: true), "EmployeeId", "FullName", employeeId);

            var model = new ShiftAssignViewModel();

            if (employeeId.HasValue)
            {
                var emp = _employeeRepository.GetEmployeeById(employeeId.Value);
                if (emp != null)
                {
                    model.EmployeeId = emp.EmployeeId;
                    model.EmployeeName = emp.FullName;
                    model.Department = emp.Department;
                    model.Position = emp.Position;
                    model.CurrentShiftName = emp.ShiftName ?? "Unassigned";
                    if (emp.ShiftId.HasValue)
                    {
                        model.ShiftId = emp.ShiftId.Value;
                    }
                }
            }

            return View(model);
        }

        // POST: /Shift/Assign
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Assign(ShiftAssignViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Shifts = new SelectList(_shiftRepository.GetAllShifts(activeOnly: true), "ShiftId", "ShiftName", model.ShiftId);
                ViewBag.Employees = new SelectList(_employeeRepository.GetAllEmployees(activeOnly: true), "EmployeeId", "FullName", model.EmployeeId);
                return View(model);
            }

            try
            {
                _shiftRepository.AssignShiftToEmployee(model.EmployeeId, model.ShiftId);
                var shift = _shiftRepository.GetShiftById(model.ShiftId);
                var emp = _employeeRepository.GetEmployeeById(model.EmployeeId);

                TempData["SuccessMessage"] = $"Assigned shift '{shift?.ShiftName}' ({shift?.FormattedTiming}) to employee '{emp?.FullName}'.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error assigning shift: " + ex.Message);
                ViewBag.Shifts = new SelectList(_shiftRepository.GetAllShifts(activeOnly: true), "ShiftId", "ShiftName", model.ShiftId);
                ViewBag.Employees = new SelectList(_employeeRepository.GetAllEmployees(activeOnly: true), "EmployeeId", "FullName", model.EmployeeId);
                return View(model);
            }
        }
    }
}
