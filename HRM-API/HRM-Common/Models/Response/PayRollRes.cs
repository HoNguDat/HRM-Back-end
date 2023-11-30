using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM_Common.Models.Response
{
    public class PayRollRes
    {
        public Guid? Id { get; set; }
        public string FullName { get; set; }
        public string? Date { get; set; }
        public double? MinutesLate { get; set; }
        public double? HoursWorking { get; set; }
        public double? Salary { get; set; }
        public double? Total { get; set; }


        public string? EmployeeId { get; set; }

        public PayRollRes() { }
        public PayRollRes(Payroll payroll)
        {
            Id = payroll.Id;
            FullName = payroll.ApplicationUser.FullName;
            Date = payroll.Date?.ToString("dd/MM/yyyy hh:mm tt");
            MinutesLate = payroll.MinutesLate;
            HoursWorking = payroll.HoursWorking;
            Salary = payroll.Salary;
            Total = payroll.Total;

        }
    }
}
