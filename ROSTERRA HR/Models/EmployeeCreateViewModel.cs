using System;
using System.ComponentModel.DataAnnotations;

namespace ROSTERRA_HR.Models
{
    /// <summary>
    /// View model for creating a new employee and their associated login credentials.
    /// </summary>
    public class EmployeeCreateViewModel
    {
        [Required(ErrorMessage = "First name is required.")]
        [Display(Name = "First Name")]
        [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        [Display(Name = "Last Name")]
        [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [Display(Name = "Corporate Email")]
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters.")]
        public string Email { get; set; }

        [Display(Name = "Phone Number")]
        [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters.")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Department is required.")]
        [Display(Name = "Department")]
        [StringLength(50, ErrorMessage = "Department cannot exceed 50 characters.")]
        public string Department { get; set; }

        [Required(ErrorMessage = "Position / Designation is required.")]
        [Display(Name = "Position")]
        [StringLength(50, ErrorMessage = "Position cannot exceed 50 characters.")]
        public string Position { get; set; }

        [Required(ErrorMessage = "Salary is required.")]
        [Range(0, 10000000, ErrorMessage = "Salary must be a non-negative number.")]
        [Display(Name = "Base Salary ($)")]
        [DataType(DataType.Currency)]
        public decimal Salary { get; set; }

        [Required(ErrorMessage = "Hire date is required.")]
        [Display(Name = "Hire Date")]
        [DataType(DataType.Date)]
        public DateTime HireDate { get; set; } = DateTime.Today;

        [Display(Name = "Assigned Shift")]
        public int? ShiftId { get; set; }

        [Required(ErrorMessage = "Login username is required.")]
        [Display(Name = "Employee Username")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Initial login password is required.")]
        [Display(Name = "Initial Password")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string Password { get; set; } = "Emp@2026";
    }
}
