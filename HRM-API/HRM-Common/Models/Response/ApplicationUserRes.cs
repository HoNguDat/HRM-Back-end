using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace HRM_Common.Models.Response
{
    public class ApplicationUserRes
    {
        public string Id { get; set; }
        public string Code { get; set; }
        public string FullName { get; set; }
        public string? Address { get; set; }
        public double? Salary { get; set; }
        public string? Avatar { get; set; }
        public IFormFile FormFile { get; set; }
        public string? Gender { get; set; }
        public DateTime? Dob { get; set; }
        public string? PlaceOfBirth { get; set; }
        public string? BankAccount { get; set; }
        public string? BankName { get; set; }
        public DateTime? StartDate { get; set; }
        public string? EmployeeStatus { get; set; }
        public string? EmploymentCategory { get; set; }
        public string? EmployeeLevel { get; set; }
        public Guid? DepartmentId { get; set; }
        public Guid? PositionId { get; set; }
        public string PositionName { get; set; }
        public string DepartmentName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string PassWord { get; set; }
        public string Role { get; set; }
        public byte[]? Version { get; set; }
        public ApplicationUserRes() { }
        public ApplicationUserRes(ApplicationUser applicationUser)
        {
            Id = applicationUser.Id;
            Code = applicationUser.Code;
            FullName = applicationUser.FullName;
            Address = applicationUser.Address;
            Salary = applicationUser.Salary;
            Avatar = applicationUser.Avatar;
            FormFile = applicationUser.FormFile;
            Gender = applicationUser.Gender.ToString();
            Dob = applicationUser.Dob;
            PlaceOfBirth = applicationUser.PlaceOfBirth;
            BankAccount = applicationUser.BankAccount;
            BankName = applicationUser.BankName;
            StartDate = applicationUser.StartDate;
            EmployeeLevel = applicationUser.EmployeeLevel.ToString();
            EmployeeStatus = applicationUser.EmployeeStatus.ToString();
            EmploymentCategory = applicationUser.EmploymentCategory.ToString();
            DepartmentId = applicationUser.DepartmentId;
            PositionId = applicationUser.PositionId;
            PositionName = applicationUser.Position?.Name;
            DepartmentName = applicationUser.Department?.Name;
            PhoneNumber = applicationUser.PhoneNumber;
            Email = applicationUser.Email;
            UserName = applicationUser.UserName;
            PassWord = applicationUser.Password;
            Role = applicationUser.role.ToString();
            Version = applicationUser.Version;
        }
    }
}
