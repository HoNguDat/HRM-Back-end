using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
namespace HRM_Common.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? Code { get; set; }
        public string FullName { get; set; }
        public string? Address { get; set; }
        public double? Salary { get; set; }
        [MaxLength]
        public string? Avatar { get; set; }
        [NotMapped]
        public IFormFile? FormFile { get; set; }
        public Gender? Gender { get; set; }
        public DateTime? Dob { get; set; }
        public string? PlaceOfBirth { get; set; }
        public string? BankAccount { get; set; }
        public string? BankName { get; set; }
        public DateTime? StartDate { get; set; }
        public EmployeeStatus? EmployeeStatus { get; set; }
        public EmploymentCategory? EmploymentCategory { get; set; }
        public EmployeeLevel? EmployeeLevel { get; set; }
        public bool? Inactive { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public Role? role { get; set; }
        public int EmployeeNumber { get; set; }

        //password
        public string Password { get; set; }
        //Version
        [Timestamp]
        public byte[]? Version { get; set; }


        //Foregin key
        public Guid? DepartmentId { get; set; }
        public virtual Department? Department { get; set; }

        public Guid? PositionId { get; set; }
        public virtual Position? Position { get; set; }

        public virtual ICollection<Recruitment>? Recruitments { get; set; }

        public virtual ICollection<Absence>? Absences { get; set; }

        public virtual ICollection<CheckInRecord>? CheckInRecords { get; set; }

        public virtual ICollection<Payroll>? Payrolls { get; set; }

    }
    public enum Gender { Male, Female, Other }
    public enum EmployeeStatus { Working, Leaving }
    public enum EmployeeLevel
    {
        Intern = 0,
        Fresher = 1,
        Junior = 2,
        Intermediate = 3,
        Senior = 4,
        Lead = 5,
        SeniorLead = 6,
        Expert = 7
    }
    public enum EmploymentCategory
    {
        FullTime = 0,
        PartTime = 1,
        Intern = 2,
    }
    public enum Role
    {
        Admin = 0,
        Staff = 1,
        HR = 2,
    }
}
