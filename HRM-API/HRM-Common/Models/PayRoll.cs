using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM_Common.Models
{
    public class Payroll
    {
        [Key]
        public Guid Id { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy hh:mm tt}", ApplyFormatInEditMode = true)]
        public DateTime? Date { get; set; }
        public double? Total { get; set; }
        public double? MinutesLate { get; set; }
        public double? HoursWorking { get; set; }
        public double? Salary { get; set; }
        public string? EmployeeId { get; set; }
        public virtual ApplicationUser? ApplicationUser { get; set; }
    }
}
