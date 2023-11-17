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
        public int Month { get; set; }
        public int Year { get; set; }
        public double HourDeducted { get; set; }
        public double HourViolating { get; set; }
        public double Total { get; set; }
        public string EmployeeId { get; set; }
        public virtual ApplicationUser? ApplicationUser { get; set; }
    }
}
