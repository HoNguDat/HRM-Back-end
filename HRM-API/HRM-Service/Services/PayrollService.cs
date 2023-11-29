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
        Task<PagedResult<Payroll>> GetAllPaging(GetApplicationUserModule req);
        Task<Payroll> AddPayRoll(Payroll payroll);
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
        public async Task<PagedResult<Payroll>> GetAllPaging(GetApplicationUserModule req)
        {
            var query = _context.Payrolls.Include(p => p.ApplicationUser).AsQueryable();
            if (!string.IsNullOrEmpty(req.Keyword))
            {
                query = query.Where(s => s.ApplicationUser.FullName.ToLower().Contains(req.Keyword.ToLower()));
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
            ApplicationUser? applicationUser = null;
            if (payroll != null)
            {
                applicationUser = _context.ApplicationUsers.FirstOrDefault(p => p.Id == payroll.EmployeeId);
                if(applicationUser == null) {
                    throw new Exception("User does not exist");
                }
            }

            List<CheckInRecord> checkInRecords = _context.CheckInRecords
             .Where(record => record.EmployeeId == payroll.EmployeeId &&
                             record.Date.HasValue &&  
                             record.Date.Value.Year == payroll.Date.Value.Year &&
                             record.Date.Value.Month == payroll.Date.Value.Month)
             .ToList();

            double totalMinutesLate = checkInRecords.Sum(record => record.MinutesLate ?? 0);
            double totalHoursWorking = checkInRecords.Sum(record => record.HoursWorking ?? 0);
            double salaryPerHour = (applicationUser.Salary ?? 0) / (Working_hours_1_day * Working_day_1_month);
            double salaryPerMinute = salaryPerHour / 60;

            double totalSalary = (applicationUser.Salary ?? 0) - (salaryPerMinute * totalMinutesLate);
            // Thêm lương vào database
            Payroll addPayRoll = new Payroll
            {
                HoursWorking = totalHoursWorking,
                MinutesLate = totalMinutesLate,
                Date = payroll.Date,
                Salary = applicationUser.Salary,
                EmployeeId = payroll.EmployeeId,
                ApplicationUser = applicationUser,
                Total = payroll.Total


            };
            _context.Payrolls.Add(addPayRoll);
            await _context.SaveChangesAsync();

            return payroll;
        }

        public void CalculateAndSavePayroll()
        {

        }
    }
}
