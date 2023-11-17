using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM_Common.Models.Response
{
    public class AbsenceRes
    {
        public int Id { get; set; }
        public string LeaveType { get; set; }
        public int Type { get; set; }
        public string FromDateSingle { get; set; }
        public string ShiftTypeSingle { get; set; }
        public DateTime? FromDateMulti { get; set; }
        public ShiftType ShiftTypeFromDateMulti { get; set; }
        public DateTime? ToDateMulti { get; set; }
        public ShiftType ShiftTypeToDateMulti { get; set; }
        public string? Reason { get; set; }
        public decimal HourDeducted { get; set; }
        public string EmployeeName { get; set; }
        public AbsenceRes() { }
        public AbsenceRes(Absence absence)
        {
            Id = absence.Id;
            LeaveType = absence.LeaveType.ToString();
            Type = absence.Type;
            string fromDateSingle = absence.FromDateSingle.ToString("dd/MM/yyyy");
            FromDateSingle = fromDateSingle;
            ShiftTypeSingle = absence.ShiftTypeSingle.ToString();
            Reason = absence.Reason;
            HourDeducted = absence.HourDeducted;
            EmployeeName = absence.ApplicationUser?.FullName;
        }
    }
}
