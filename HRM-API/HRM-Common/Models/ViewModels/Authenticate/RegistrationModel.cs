using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HRM_Common.Models.ViewModels.Authenticate
{
    public class RegistrationModel
    {
        [Required(ErrorMessage = "Full name is required")]
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string Address { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
        public double Salary { get; set; }
        public Gender? Gender { get; set; }
        public DateTime? Dob { get; set; }
        public string? PlaceOfBirth { get; set; }
        public DateTime? StartDate { get; set; }
        public EmployeeStatus? EmployeeStatus { get; set; }
        public EmploymentCategory? EmploymentCategory { get; set; }
        public EmployeeLevel? EmployeeLevel { get; set; }
        public Guid? DepartmentId { get; set; }
        public Guid? PositionId { get; set; }
    }
}
