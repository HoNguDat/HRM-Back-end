using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM_Common.Models
{
    public class Absence
    {
        [Key]
        public int Id { get; set; }
        public LeaveType LeaveType { get; set; }
        public int Type { get; set; }
        public DateTime FromDateSingle { get; set; }
        public ShiftType ShiftTypeSingle { get; set; }
        public DateTime? FromDateMulti { get; set; }
        public ShiftType ShiftTypeFromDateMulti { get; set; }
        public DateTime? ToDateMulti { get; set; }
        public ShiftType ShiftTypeToDateMulti { get; set; }
        public string? Reason { get; set; }
        public string ApplicationUserId { get; set; }
        public virtual ApplicationUser? ApplicationUser { get; set; }
        public decimal HourDeducted { get; set; }

    }
    public enum ShiftType
    {
        Morning = 0,
        Afternoon = 1,
        Allday = 2,
    }
    public enum LeaveType
    {
        AnnualLeave = 0,
        WeddingLeave = 1,
        CompassionateLeave = 2,
        LeaveWithoutPay = 3,
        MaternityLeave = 4,
        PaternityLeave = 5,
        OneMoreChild = 6,
        AdoptionLeave = 7,
        SickLeave = 8,
        ChildSickLeaveUnder3YearsOld = 9,
        ChildSickLeaveUnder7YearsOld = 10,
    }
}
