using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM_Common.Models
{
    public class CheckInRecord
    {
        [Key]
        public Guid CheckInRecordId { get; set; }
        public DateTime CheckInTime { get; set; }
        public DateTime CheckOutTime { get; set; }
        public DateTime Date { get; set; }
        public DateTime GoOutTime { get; set; }
        public DateTime GoInTime { get; set; }
        public string EmployeeId { get; set; }
        public int MinutesLate { get; set; }
        public virtual ApplicationUser? ApplicationUser { get; set; }
    }
}
