using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HRM_Common.Models;
using HRM_Common.Models.Response;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace HRM_Service.Services
{
    public interface IAbsenceService
    {
        Task<List<Absence>> GetAllAbsences(string? keyword);
        Task<List<Absence>> GetAllAbsenceById(int id);
        Task<Absence> BookingAbsence(Absence absence);
        bool IsBookingDateValid(DateTime fromDate);
        bool IsMorningOrAfternoon(ShiftType shiftType);
        bool IsAllDay(ShiftType shiftType);
        void RemoveAbsence(int id);
    }
    public class AbsenceService : IAbsenceService
    {

        private readonly ApplicationDbContext _context;

        public AbsenceService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Absence>> GetAllAbsences(string? keyword)
        {
            var query = _context.Absences.Include(p => p.ApplicationUser).AsQueryable();
            if (!string.IsNullOrEmpty(keyword))
            {
                var lowerKeyword = keyword.ToLower();
                query = query.Where(x => x.ApplicationUser.FullName.ToLower().Contains(lowerKeyword));
            }

            return await query.ToListAsync();
        }

        public async Task<List<Absence>> GetAllAbsenceById(int id)
        {
            var data = await _context.Absences.Where(x => x.Id == id).ToListAsync();
            return data;
        }

        public async Task<Absence> BookingAbsence(Absence absence)
        {
            if (absence.Type == 1)
            {
                if (IsBookingDateValid(absence.FromDateSingle))
                {
                    var checkAbsenceMulti = await _context.Absences.FirstOrDefaultAsync(x => x.FromDateMulti <= absence.FromDateSingle && absence.FromDateSingle <= x.ToDateMulti && x.Type == 2);
                    if (checkAbsenceMulti != null)
                    {
                        throw new Exception("Overlap");
                    }
                    var checkAbsence = await _context.Absences.FirstOrDefaultAsync(x => x.FromDateSingle == absence.FromDateSingle);
                    var employee = _context.Users.FirstOrDefault(p => p.Id == absence.ApplicationUserId);
                    if (checkAbsence != null)
                    {
                        if (IsMorningOrAfternoon(absence.ShiftTypeSingle))
                        {
                            if (IsAllDay(checkAbsence.ShiftTypeSingle))
                            {
                                throw new Exception("Can not book beacause this day all day");
                            }
                            else if (absence.ShiftTypeSingle == checkAbsence.ShiftTypeSingle)
                            {
                                throw new Exception("Can not booking because overlap");
                            }
                            absence.HourDeducted = CalculateHourDeducted(absence.ShiftTypeSingle);
                            absence.ApplicationUser = employee;
                            _context.Absences.Add(absence);
                            await _context.SaveChangesAsync();
                            return absence;
                        }
                        else if (absence.ShiftTypeSingle == ShiftType.Allday)
                        {
                            if (checkAbsence.ShiftTypeSingle == ShiftType.Allday)
                            {
                                throw new Exception("Can not book beacause this day all day");
                            }
                            else if (checkAbsence.ShiftTypeSingle == ShiftType.Morning || checkAbsence.ShiftTypeSingle == ShiftType.Afternoon)
                            {
                                throw new Exception("Can not booking all day beacuse this day have morning or afternoon !");
                            }
                            absence.HourDeducted = CalculateHourDeducted(absence.ShiftTypeSingle);
                            absence.ApplicationUser = employee;
                            _context.Absences.Add(absence);
                            await _context.SaveChangesAsync();
                            return absence;
                        }
                    }

                    absence.HourDeducted = CalculateHourDeducted(absence.ShiftTypeSingle);
                    absence.ApplicationUser = employee;
                    _context.Absences.Add(absence);
                    await _context.SaveChangesAsync();
                    return absence;
                }
                else
                {
                    throw new Exception("From date single < date now");
                }
            }
            else
            {
                if (absence.FromDateMulti > DateTime.Now)
                {
                    var checkAbsence = await _context.Absences.FirstOrDefaultAsync(x => x.FromDateMulti == absence.FromDateMulti || x.ToDateMulti > absence.FromDateMulti);
                    var employee = _context.Users.FirstOrDefault(p => p.Id == absence.ApplicationUserId);
                    if (checkAbsence != null)
                    {
                        throw new Exception("Can not booking absence because overlap");
                    }
                    var checkAbsenceSingle = await _context.Absences.FirstOrDefaultAsync(x => x.FromDateSingle == absence.FromDateMulti);
                    if (checkAbsenceSingle != null)
                    {
                        if (checkAbsenceSingle.ShiftTypeSingle == ShiftType.Allday)
                        {
                            throw new Exception("Can not booking absence because overlap");
                        }
                        else
                        {
                            if (checkAbsenceSingle.ShiftTypeSingle == absence.ShiftTypeFromDateMulti)
                            {
                                throw new Exception("Can not booking absence because overlap");
                            }
                            else if (checkAbsenceSingle.ShiftTypeSingle == ShiftType.Afternoon && absence.ShiftTypeFromDateMulti == ShiftType.Morning)
                            {
                                throw new Exception("Can not booking absence because overlap");
                            }
                            else if (checkAbsenceSingle.ShiftTypeSingle == ShiftType.Morning && absence.ShiftTypeFromDateMulti == ShiftType.Afternoon)
                            {
                                absence.HourDeducted = CalculateHourDeducted(absence);
                                absence.ApplicationUser = employee;
                                _context.Absences.Add(absence);
                                await _context.SaveChangesAsync();
                                return absence;
                            }

                        }
                        throw new Exception("Can not booking absence because overlap");
                    }
                    var checkAbsenceFromDateMultiLessThanToDateMulti = await _context.Absences.FirstOrDefaultAsync(x => x.ToDateMulti == absence.FromDateMulti);
                    if (checkAbsenceFromDateMultiLessThanToDateMulti != null)
                    {
                        if (absence.ShiftTypeFromDateMulti == ShiftType.Allday)
                        {
                            throw new Exception("Can not booking absence overlap");
                        }
                        else if (checkAbsenceFromDateMultiLessThanToDateMulti.ShiftTypeToDateMulti == absence.ShiftTypeFromDateMulti)
                        {
                            throw new Exception("Can not booking absence because overlap");
                        }
                        else if (checkAbsenceFromDateMultiLessThanToDateMulti.ShiftTypeToDateMulti == ShiftType.Morning && absence.ShiftTypeFromDateMulti == ShiftType.Afternoon)
                        {
                            absence.HourDeducted = CalculateHourDeducted(absence);
                            absence.ApplicationUser = employee;
                            _context.Absences.Add(absence);
                            await _context.SaveChangesAsync();
                            return absence;
                        }
                        else if (checkAbsenceFromDateMultiLessThanToDateMulti.ShiftTypeToDateMulti == ShiftType.Afternoon && absence.ShiftTypeFromDateMulti == ShiftType.Morning)
                        {
                            throw new Exception("Can not booking absence because overlap");
                        }
                        else if (checkAbsenceFromDateMultiLessThanToDateMulti.ShiftTypeToDateMulti == ShiftType.Allday)
                        {
                            throw new Exception("Can not booking absence because overlap");
                        }
                    }

                    absence.HourDeducted = CalculateHourDeducted(absence);
                    absence.ApplicationUser = employee;
                    _context.Absences.Add(absence);
                    await _context.SaveChangesAsync();
                    return absence;
                }
                else
                {
                    throw new Exception("From date single < date now    ");
                }
            }
        }

        public decimal CalculateHourDeducted(ShiftType? shiftType)
        {
            decimal hourDeducted = 0;
            switch (shiftType)
            {
                case ShiftType.Morning:
                    hourDeducted = 4;
                    break;
                case ShiftType.Afternoon:
                    hourDeducted = 4;
                    break;
                case ShiftType.Allday:
                    hourDeducted = 8;
                    break;
            }
            return hourDeducted;
        }

        public decimal CalculateHourDeducted(Absence absence)
        {
            decimal totalVacationHours;
            decimal hourDeductedFromDate;
            if (absence.ShiftTypeFromDateMulti == ShiftType.Morning)
            {
                hourDeductedFromDate = 8;
            }
            else
            {
                hourDeductedFromDate = CalculateHourDeducted(absence.ShiftTypeFromDateMulti);
            }
            int totalDay = CalculateDaysDifference(absence.FromDateMulti, absence.ToDateMulti);
            decimal hourDeductedTotalDate = totalDay * 8;
            decimal hourDeductedToDate;
            if (absence.ShiftTypeToDateMulti == ShiftType.Afternoon)
            {
                hourDeductedToDate = 8;
            }
            else
            {
                hourDeductedToDate = CalculateHourDeducted(absence.ShiftTypeToDateMulti);
            }
            totalVacationHours = hourDeductedFromDate + hourDeductedTotalDate + hourDeductedToDate;
            return totalVacationHours;
        }

        public int CalculateDaysDifference(DateTime fromDate, DateTime toDate)
        {
            int daysDifference = 0;

            DateTime currentDate = fromDate.AddDays(1);

            while (currentDate < toDate)
            {
                if (currentDate.DayOfWeek != DayOfWeek.Saturday && currentDate.DayOfWeek != DayOfWeek.Sunday)
                {
                    daysDifference++;
                }

                currentDate = currentDate.AddDays(1);
            }
            return daysDifference;
        }

        public bool IsBookingDateValid(DateTime fromDate)
        {
            if (fromDate > DateTime.Now)
            {
                return true;
            }
            return false;
        }
        public bool IsMorningOrAfternoon(ShiftType shiftType)
        {
            if (shiftType == ShiftType.Morning || shiftType == ShiftType.Afternoon)
            {
                return true;
            }
            return false;
        }
        public bool IsAllDay(ShiftType shiftType)
        {
            if (shiftType == ShiftType.Allday)
            {
                return true;
            }
            return false;
        }
        public void RemoveAbsence(int id)
        {
            var checkAbsence = _context.Absences.Find(id);
            if (checkAbsence == null)
            {
                throw new Exception("Absence do not exist");
            }
            if (checkAbsence.Type == 1)
            {
                if (checkAbsence.FromDateSingle <= DateTime.Now)
                {
                    throw new Exception("Can not remove absence because from date less than or equals date now");
                }
            }
            else
            {
                if (checkAbsence.FromDateMulti <= DateTime.Now)
                {
                    throw new Exception("Can not remove absence because from date less than or equals date now");
                }
            }

            _context.Entry(checkAbsence).State = EntityState.Deleted;
            _context.SaveChanges();
        }
    }
}
