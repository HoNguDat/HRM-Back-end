using HRM_Common.Models;
using HRM_Common.Models.Response;
using HRM_Common.Paged;
using HRM_Common.ReqModules;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HRM_Service.Services
{
    public interface IPayrollService
    {
        Task<PagedResult<Payroll>> GetAllPaging(GetApplicationUserModule req, int? mounth, int? year);
        Task<Payroll> AddPayRoll(Payroll payroll);
        void DeletePayRoll(Guid id);
    }
    public class PayrollService : IPayrollService
    {
        private readonly ApplicationDbContext _context;
        private double Working_hours_1_day = 8;
        private double Working_hours_1_week = 40;
        private double Working_day_1_month = 40;

        public PayrollService(ApplicationDbContext applicationDbContext)
        {
            _context = applicationDbContext;
        }
        public async Task<PagedResult<Payroll>> GetAllPaging(GetApplicationUserModule req, int? mounth, int? year)
        {
            var query = _context.Payrolls.Include(p => p.ApplicationUser).AsQueryable();
            if (!string.IsNullOrEmpty(req.Keyword))
            {
                query = query.Where(s => s.ApplicationUser.FullName.ToLower().Contains(req.Keyword.ToLower()));
            }
            if (!string.IsNullOrEmpty(mounth.ToString()) || (!string.IsNullOrEmpty(mounth.ToString()) && !string.IsNullOrEmpty(year.ToString())))
            {
                if (string.IsNullOrEmpty(year.ToString()))
                {
                    year = DateTime.Now.Year;
                }
                query = query.Where(s => s.Date.Value.Month == mounth  && s.Date.Value.Year == year);
            }
            if (req.Direction.Equals("asc", StringComparison.OrdinalIgnoreCase))
            {
                query = query.OrderBy(s => s.Date); // Sắp xếp tăng dần
            }
            else if (req.Direction.Equals("desc", StringComparison.OrdinalIgnoreCase))
            {
                query = query.OrderByDescending(s => s.Date); // Sắp xếp giảm dần
            }
            var skip = (req.Page - 1) * req.PageSize;
            int total = query.Count();

            var results = query.Skip(skip).Take(req.PageSize).ToList();

            var data = new PagedResult<Payroll>
            {
                Results = results,
                Total = total
            };

            return data;
        }

        public async Task<Payroll> AddPayRoll(Payroll payroll)
        {
            if(IsValidDate(payroll.Date.Value.ToString()) == false)
            {
                throw new Exception("Date is validate");
            }
            List<ApplicationUser> workingUsers = _context.ApplicationUsers
            .Where(u => u.EmployeeStatus == 0)
            .ToList();
            var payrollByDate = _context.Payrolls.FirstOrDefault(p => p.Date.Value.Month == payroll.Date.Value.Month && p.Date.Value.Year == payroll.Date.Value.Year);
            if(IsLastDayOfMonth(payroll.Date.Value) == false)
            {
                throw new Exception("Phải là ngày cuối của tháng mới được tính");
            }
            if(payrollByDate != null)
            {
                throw new Exception("Tháng này đã được tính");
            }
            foreach (var applicationUser in workingUsers)
            {
                var payrollById = _context.Payrolls.FirstOrDefault(p => p.EmployeeId == applicationUser.Id);
                if (payrollById == null)
                {
                    //tính giờ đi làm và giờ đi trễ
                    List<CheckInRecord> checkInRecords = _context.CheckInRecords
                 .Where(record => record.EmployeeId == applicationUser.Id &&
                                 record.Date.HasValue &&
                                 record.Date.Value.Year == payroll.Date.Value.Year &&
                                 record.Date.Value.Month == payroll.Date.Value.Month)
                 .ToList();

                    double totalMinutesLate = checkInRecords.Sum(record => record.MinutesLate ?? 0);
                    double totalHoursWorking = checkInRecords.Sum(record => record.HoursWorking ?? 0);
                    double totalHoursOutSide = checkInRecords.Sum(record => record.HoursOutside ?? 0);


                    //Tính giờ nghĩ phép
                    List<Absence> abencesSingle = _context.Absences
                    .Where(abence => abence.ApplicationUserId == applicationUser.Id &&
                                    abence.FromDateSingle.Year == payroll.Date.Value.Year &&
                                    abence.FromDateSingle.Month == payroll.Date.Value.Month)
                    .ToList();
                    List<Absence> abencesMulti = _context.Absences
                    .Where(abence => abence.ApplicationUserId == applicationUser.Id &&
                             payroll.Date.Value.Year >= abence.FromDateMulti.Year &&
                             payroll.Date.Value.Year <= abence.ToDateMulti.Year &&
                             payroll.Date.Value.Month >= abence.FromDateMulti.Month &&
                             payroll.Date.Value.Month <= abence.ToDateMulti.Month)
                    .ToList();
                    decimal totalHourDeductedSingle = abencesMulti.Sum(record => record.HourDeducted);
                    decimal totalHourDeductedMulti = abencesMulti.Sum(record => record.HourDeducted);
                    decimal totalHourDeducted = totalHourDeductedSingle + totalHourDeductedMulti;

                    //Tính lương
                    double salaryPerHour = (applicationUser.Salary ?? 0) / (Working_hours_1_day * Working_day_1_month);
                    double salaryPerMinute = salaryPerHour / 60;

                    double totalSalary = 0;
                    if (totalHoursOutSide * 60 > 70)
                    {
                        totalSalary = (applicationUser.Salary ?? 0) - (salaryPerMinute * (totalMinutesLate + (double)(totalHourDeducted * 60) + (totalHoursOutSide * 60 - 60)));
                    }
                    else
                    {
                        totalSalary = (applicationUser.Salary ?? 0) - (salaryPerMinute * (totalMinutesLate + (double)(totalHourDeducted * 60)));
                    }
                    // Thêm lương vào database
                    Payroll addPayRoll = new Payroll
                    {
                        HoursWorking = Math.Round(totalHoursWorking - totalHoursOutSide),
                        MinutesLate = totalMinutesLate,
                        Date = payroll.Date,
                        Salary = Math.Round(applicationUser.Salary.Value),
                        EmployeeId = applicationUser.Id,
                        ApplicationUser = applicationUser,
                        Total = Math.Round(totalSalary)
                    };
                    _context.Payrolls.Add(addPayRoll);
                }
            }
            await _context.SaveChangesAsync();

            return payroll;
        }

        static bool IsLastDayOfMonth(DateTime date)
        {
           
            DateTime lastDayOfMonth = new DateTime(date.Year, date.Month, 1).AddMonths(1).AddDays(-1);

            return date.Date == lastDayOfMonth.Date;
        }

        public void DeletePayRoll(Guid id)
        {
            var payroll = _context.Payrolls.Find(id);

            if (payroll == null)
            {
                throw new Exception("Id không tồn tại");
            }
            _context.Entry(payroll).State = EntityState.Deleted;
            _context.SaveChanges();
        }

        public bool IsValidDate(string dateStr)
        {
            int minYear = 1800;
            int maxYear = DateTime.Now.Year;

            if (DateTime.TryParseExact(dateStr, "MM/dd/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dateTime))
            {
                if (dateTime.Year < minYear || dateTime.Year > maxYear)
                {
                    return false;
                }

                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
