using HRM_Common.Models;
using HRM_Common.Paged;
using HRM_Common.ReqModules;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HRM_Service.Services
{
    public interface ICheckInRecordService
    {
        Task<CheckInRecord> CheckIn(CheckInRecord checkIn);
        Task<CheckInRecord> CheckOut(CheckInRecord checkIn);
        Task<CheckInRecord> GoOut(CheckInRecord checkIn);
        Task<CheckInRecord> GoIn(CheckInRecord checkIn);
        Task<PagedResult<CheckInRecord>> GetAllPaging(GetApplicationUserModule req);
        Task<CheckInRecord> GetCheckInRecordById(Guid id);
        Task<PagedResult<CheckInRecord>> GetCheckInRecordByIdUser(GetApplicationUserModule req);
        Task<CheckInRecord> GetCheckInRecordByDate(DateTime? date, string employeeId);
        void DeleteCheckInRecord(Guid id);
    }
    public class CheckInRecordService : ICheckInRecordService
    {
        private TimeSpan designatedTime = new TimeSpan(8, 30, 0);
        private readonly ApplicationDbContext _context;
        public CheckInRecordService(ApplicationDbContext applicationDbContext)
        {
            _context = applicationDbContext;
        }
        public async Task<PagedResult<CheckInRecord>> GetAllPaging(GetApplicationUserModule req)
        {
            var query = _context.CheckInRecords.Include(p => p.ApplicationUser).AsQueryable();
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

            var data = new PagedResult<CheckInRecord>
            {
                Results = results,
                Total = total
            };

            return data;
        }
        public async Task<PagedResult<CheckInRecord>> GetCheckInRecordByIdUser(GetApplicationUserModule req)
        {
            var query = _context.CheckInRecords.Include(p => p.ApplicationUser).AsQueryable();
            if (!string.IsNullOrEmpty(req.Keyword))
            {
                query = query.Where(s => s.ApplicationUser.Id.ToLower().Contains(req.Keyword.ToLower()));
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

            var data = new PagedResult<CheckInRecord>
            {
                Results = results,
                Total = total
            };

            return data;
        }
        public async Task<CheckInRecord> GetCheckInRecordById(Guid id)
        {
            var checkInRecord = await _context.CheckInRecords.Include(p => p.ApplicationUser).AsQueryable().FirstOrDefaultAsync(p => p.CheckInRecordId == id);
            if (checkInRecord == null)
            {
                throw new Exception("Time does not exist");
            }
            return checkInRecord;
        }
       
        public async Task<CheckInRecord> GetCheckInRecordByDate(DateTime? date, string employeeId)
        {
            var checkInRecord = await _context.CheckInRecords.Include(p => p.ApplicationUser).AsQueryable().FirstOrDefaultAsync(c => c.Date == date && c.EmployeeId == employeeId);
            if (checkInRecord == null)
            {
                return null;
            }
            return checkInRecord;
        }
        public async Task<CheckInRecord> CheckIn(CheckInRecord checkIn)
        {
            if (IsCheckInSameDayAndTime(checkIn.Date) == false)
            {
                throw new Exception("Date is different from the current date");
            }
            if (IsCheckInSameDayAndTimeByDate(checkIn.CheckInTime, checkIn.Date) == false)
            {
                throw new Exception("Check-in is different from the current date");
            }

            if (IsValidDate(checkIn.CheckInTime.ToString()) == false)
            {
                throw new Exception("Check-in time invalidate");
            }
            if (IsValidDate(checkIn.Date.ToString()) == false)
            {
                throw new Exception("Date time invalidate");
            }

            ApplicationUser? applicationUser = null;
            if (checkIn != null)
            {
                applicationUser = _context.ApplicationUsers.FirstOrDefault(p => p.Id == checkIn.EmployeeId);
                if (applicationUser == null)
                {
                    throw new Exception("User does not exist");
                }
            }

            var checkInExists = await _context.CheckInRecords.Include(p => p.ApplicationUser).AsQueryable().FirstOrDefaultAsync(c => c.Date.Value.Date == checkIn.Date.Value.Date && c.EmployeeId == checkIn.EmployeeId);
            if (checkInExists != null)
            {
                //check-in rồi không được check-in nửa
                if (checkIn.CheckInTime != null && checkInExists.CheckInTime != null)
                {
                    throw new Exception("This date has already been check-in");
                }

            }
            else if (checkIn?.CheckInTime != null)
            {

                if (CalculateLateMinutes(checkIn.CheckInTime, designatedTime) <= 0)
                {
                    throw new Exception("Check-in is invalid");
                }

                double? minutesLate = CalculateLateMinutes(checkIn.CheckInTime, designatedTime);
                CheckInRecord checkInRecord = new CheckInRecord
                {
                    CheckInTime = checkIn.CheckInTime,
                    CheckOutTime = null,
                    GoInTime = null,
                    GoOutTime = null,
                    Date = checkIn.Date,
                    MinutesLate = minutesLate,
                    HoursOutside = null,
                    HoursWorking = null,
                    EmployeeId = checkIn.EmployeeId,
                    ApplicationUser = applicationUser,
                };
                _context.CheckInRecords.Add(checkInRecord);
                await _context.SaveChangesAsync();


            }
            return checkIn;
        }
        public async Task<CheckInRecord> CheckOut(CheckInRecord chechOut)
        {
            if (IsCheckInSameDayAndTime(chechOut.Date) == false)
            {
                throw new Exception("Date is different from the current date");
            }
            if (IsCheckInSameDayAndTimeByDate(chechOut.CheckOutTime, chechOut.Date) == false)
            {
                throw new Exception("Check-out is different from the current date");
            }
            if (IsValidDate(chechOut.Date.ToString()) == false)
            {
                throw new Exception("Date time invalidate");
            }
            if (IsValidDate(chechOut.CheckOutTime.ToString()) == false)
            {
                throw new Exception("Check-out time invalidate");
            }
            ApplicationUser? applicationUser = null;
            if (chechOut != null)
            {
                applicationUser = _context.ApplicationUsers.FirstOrDefault(p => p.Id == chechOut.EmployeeId);
                if (applicationUser == null)
                {
                    throw new Exception("User does not exist");
                }
            }
            var checkInExists = await _context.CheckInRecords.Include(p => p.ApplicationUser).AsQueryable().FirstOrDefaultAsync(c => c.Date.Value.Date == chechOut.Date.Value.Date && c.EmployeeId == chechOut.EmployeeId);
            if (IsCheckInBeforeCheckOut(checkInExists?.CheckInTime, chechOut.CheckOutTime) == false)
            {
                throw new Exception("Check-out must be greater than check-in");
            }
            if (checkInExists.GoInTime != null || checkInExists.GoOutTime != null)
            {
                if (checkInExists?.GoInTime != null && checkInExists?.GoOutTime != null)
                {
                    if (IsCheckInBeforeCheckOut(checkInExists?.GoInTime, chechOut.CheckOutTime) == false || IsCheckInBeforeCheckOut(checkInExists?.GoOutTime, chechOut.CheckOutTime) == false)
                    {
                        throw new Exception("Check-out must be greater than go-out and go-in");
                    }
                }
            }
            if (checkInExists != null)
            {
                if (checkInExists?.CheckOutTime == null)
                {
                    if (CalculateLateHours(checkInExists.CheckInTime, chechOut.CheckOutTime) <= 0)
                    {
                        throw new Exception("Check-out is invalid");
                    }
                    double? hoursWorking = CalculateLateHours(checkInExists.CheckInTime, chechOut.CheckOutTime);
                    checkInExists.HoursWorking = hoursWorking;
                    checkInExists.CheckOutTime = chechOut?.CheckOutTime;
                    _context.Update(checkInExists);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    throw new Exception("This date has been check-out");
                }
            }
            else
            {
                throw new Exception("There is no date for check-out");
            }
            return chechOut;
        }
        public async Task<CheckInRecord> GoOut(CheckInRecord goOut)
        {
            if (IsCheckInSameDayAndTime(goOut.Date) == false)
            {
                throw new Exception("Date is different from the current date");
            }
            if (IsCheckInSameDayAndTimeByDate(goOut.GoOutTime, goOut.Date) == false)
            {
                throw new Exception("Go-out is different from the current date");
            }
            if (IsValidDate(goOut.GoOutTime.ToString()) == false)
            {
                throw new Exception("Go-out time invalidate");
            }
            if (IsValidDate(goOut.Date.ToString()) == false)
            {
                throw new Exception("Date time invalidate");
            }
            ApplicationUser? applicationUser = null;
            if (goOut != null)
            {
                applicationUser = _context.ApplicationUsers.FirstOrDefault(p => p.Id == goOut.EmployeeId);
                if (applicationUser == null)
                {
                    throw new Exception("User does not exist");
                }
            }
            var checkInExists = await _context.CheckInRecords.Include(p => p.ApplicationUser).AsQueryable().FirstOrDefaultAsync(c => c.Date.Value.Date == goOut.Date.Value.Date && c.EmployeeId == goOut.EmployeeId);
            if (IsCheckInBeforeCheckOut(checkInExists?.CheckInTime, goOut.GoOutTime) == false)
            {
                throw new Exception("Go-out must be greater than check-in");
            }
            if (checkInExists != null)
            {
                if (checkInExists.CheckOutTime != null)
                {
                    throw new Exception("This date has been check-out and not allowed to go-out");
                }
                else if (checkInExists?.GoOutTime == null && checkInExists?.GoInTime == null)
                {
                    checkInExists.GoOutTime = goOut.GoOutTime;
                    _context.Update(checkInExists);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    if (IsCheckInBeforeCheckOut(checkInExists?.GoOutTime, goOut.GoOutTime) == false)
                    {
                        throw new Exception("The current go-out must be larger than the existing go-out");
                    }
                    else
                    {
                        checkInExists.GoOutTime = goOut.GoOutTime;
                        _context.Update(checkInExists);
                        await _context.SaveChangesAsync();
                    }
                }
            }
            else
            {
                throw new Exception("There is no date for go-out");
            }
            return goOut;
        }
        public async Task<CheckInRecord> GoIn(CheckInRecord goIn)
        {
            if (IsCheckInSameDayAndTime(goIn.Date) == false)
            {
                throw new Exception("Date is different from the current date");
            }
            if (IsCheckInSameDayAndTimeByDate(goIn.GoInTime, goIn.Date) == false)
            {
                throw new Exception("Go-in is different from the current date");
            }
            if (IsValidDate(goIn.GoInTime.ToString()) == false)
            {
                throw new Exception("Go-in time invalidate");
            }
            if (IsValidDate(goIn.Date.ToString()) == false)
            {
                throw new Exception("Date time invalidate");
            }
            ApplicationUser? applicationUser = null;
            if (goIn != null)
            {
                applicationUser = _context.ApplicationUsers.FirstOrDefault(p => p.Id == goIn.EmployeeId);
                if (applicationUser == null)
                {
                    throw new Exception("User does not exist");
                }
            }
            var checkInExists = await _context.CheckInRecords.Include(p => p.ApplicationUser).AsQueryable().FirstOrDefaultAsync(c => c.Date.Value.Date == goIn.Date.Value.Date && c.EmployeeId == goIn.EmployeeId);
            if (IsCheckInBeforeCheckOut(checkInExists?.CheckInTime, goIn.GoInTime) == false)
            {
                throw new Exception("Go-in must be greater than check-in");
            }
            if (checkInExists != null)
            {
                if (checkInExists.CheckOutTime != null)
                {
                    throw new Exception("This date has been check-out and not allowed to go-in");
                }
                else if (checkInExists?.GoInTime == null && checkInExists?.GoOutTime != null)
                {
                    if (IsCheckInBeforeCheckOut(checkInExists?.GoOutTime, goIn.GoInTime) == false)
                    {
                        throw new Exception("Go-in must be greater than go-out");
                    }
                    if (CalculateLateHours(checkInExists.GoOutTime, goIn.GoInTime) <= 0)
                    {
                        throw new Exception("Go-in is invalid");
                    }
                    double? hoursOutside = CalculateLateHours(checkInExists.GoOutTime, goIn.GoInTime);

                    if (checkInExists.HoursOutside == null)
                    {
                        checkInExists.HoursOutside = hoursOutside;
                    }
                    else
                    {
                        checkInExists.HoursOutside += hoursOutside;
                    }
                   
                    checkInExists.GoInTime = goIn?.GoInTime;
                    _context.Update(checkInExists);
                    await _context.SaveChangesAsync();
                }
                else if (checkInExists?.GoOutTime != null && checkInExists?.GoInTime != null)
                {
                    if (IsCheckInBeforeCheckOut(checkInExists?.GoInTime, goIn.GoInTime) == false && IsCheckInBeforeCheckOut(checkInExists?.GoOutTime, goIn.GoInTime) == false)
                    {
                        throw new Exception("The current go-in must be larger than the existing go-out and existing go-in");
                    }
                    else
                    {
                        if (CalculateLateHours(checkInExists.GoOutTime, goIn.GoInTime) <= 0)
                        {
                            throw new Exception("Go-in is invalid");
                        }
                        double? hoursOutside = CalculateLateHours(checkInExists.GoOutTime, goIn.GoInTime);

                        if (checkInExists.HoursOutside == null)
                        {
                            checkInExists.HoursOutside = hoursOutside;
                        }
                        else
                        {
                            checkInExists.HoursOutside += hoursOutside;
                        }
                        checkInExists.GoInTime = goIn.GoInTime;
                        _context.Update(checkInExists);
                        await _context.SaveChangesAsync();
                    }
                }
            }
            else
            {
                throw new Exception("There is no date for go-out");
            }
            return goIn;
        }

        public bool IsValidDate(string dateStr)
        {
            int minYear = 1800;
            int maxYear = DateTime.Now.Year;

            if (DateTime.TryParseExact(dateStr, "MM/dd/yyyy h:mm:ss tt", null, System.Globalization.DateTimeStyles.None, out DateTime dateTime))
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
        public static double? CalculateLateHours(DateTime? goOutTime, DateTime? goInTime)
        {
            if (goOutTime > goInTime)
            {
                return 0;
            }
            if (goOutTime.HasValue && goInTime.HasValue)
            {
                TimeSpan lateTimeSpan = goInTime.Value.Subtract(goOutTime.Value);
                return lateTimeSpan.TotalHours;
            }

            return null;
        }

        public static double? CalculateLateMinutes(DateTime? checkInTime, TimeSpan designatedTime)
        {

            if (checkInTime.Value.Hour < designatedTime.Hours)
            {
                return 0;
            }
            if (checkInTime.Value.Hour == designatedTime.Hours && checkInTime.Value.Minute < designatedTime.Minutes)
            {
                return 0;
            }
            if (checkInTime.HasValue)
            {
                DateTime designatedDateTime = checkInTime.Value.Date + designatedTime;

                TimeSpan lateTimeSpan = checkInTime.Value.Subtract(designatedDateTime);

                return (double)lateTimeSpan.TotalMinutes;
            }

            return null;
        }
        public static bool IsCheckInSameDayAndTime(DateTime? checkInTime)
        {
            if (checkInTime.HasValue)
            {
                DateTime checkInDate = checkInTime.Value;

                DateTime currentDate = DateTime.Now;
                DateTime truncatedCheckInDate = new DateTime(checkInDate.Year, checkInDate.Month, checkInDate.Day);
                DateTime truncatedCurrentDate = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day);
                if (truncatedCheckInDate == truncatedCurrentDate)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsCheckInSameDayAndTimeByDate(DateTime? checkInTime, DateTime? date)
        {
            if (checkInTime.HasValue && date.HasValue)
            {
                DateTime checkInDate = checkInTime.Value;

                DateTime currentDate = date.Value;

                DateTime truncatedCheckInDate = new DateTime(checkInDate.Year, checkInDate.Month, checkInDate.Day);
                DateTime truncatedCurrentDate = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day);
                if (truncatedCheckInDate == truncatedCurrentDate)
                {
                    return true;
                }
            }

            return false;
        }
        public static bool IsCheckInBeforeCheckOut(DateTime? checkInDate, DateTime? checkOutDate)
        {
            return checkInDate < checkOutDate;
        }
        public void DeleteCheckInRecord(Guid id)
        {
            var checkInRecord = _context.CheckInRecords.Find(id);

            if (checkInRecord == null)
            {
                throw new Exception("Id không tồn tại");
            }
            _context.Entry(checkInRecord).State = EntityState.Deleted;
            _context.SaveChanges();
        }
        
    }
}
