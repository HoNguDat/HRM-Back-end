using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HRM_Common.Models.Response
{
    public class CheckInRecordRes
    {
        public string FullName { get; set; }
        public string Id { get; set; }
        public string? Date { get; set; }
        public string? CheckInTime { get; set; }
        public string? ChecOutTime { get; set; }
        public string? GoInTime { get; set; }
        public string? GoOutTime { get; set; }
        public double? MinutesLate { get; set; }
        public double? HoursOutside { get; set; }
        public double? HoursWorking { get; set; }
        public string? EmployeeId { get; set; }

        public CheckInRecordRes() { }
        public CheckInRecordRes(CheckInRecord checkInRecord)
        {
            FullName = checkInRecord.ApplicationUser.FullName;
            Id = checkInRecord.CheckInRecordId.ToString();
            Date = checkInRecord.Date?.ToString("dd/MM/yyyy hh:mm tt");
            CheckInTime = checkInRecord.CheckInTime?.ToString("dd/MM/yyyy hh:mm tt");
            ChecOutTime = checkInRecord.CheckOutTime?.ToString("dd/MM/yyyy hh:mm tt");
            GoInTime = checkInRecord.GoInTime?.ToString("dd/MM/yyyy hh:mm tt");
            GoOutTime = checkInRecord.GoOutTime?.ToString("dd/MM/yyyy hh:mm tt");
            MinutesLate = checkInRecord.MinutesLate;
            EmployeeId = checkInRecord.EmployeeId;
            HoursOutside = checkInRecord.HoursOutside;
            HoursWorking = checkInRecord.HoursWorking;
        }
    }
}
