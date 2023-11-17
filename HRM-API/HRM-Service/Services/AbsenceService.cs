using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HRM_Common.Models;
using Microsoft.EntityFrameworkCore;

namespace HRM_Service.Services
{
    public interface IAbsenceService
    {
        Task<List<Absence>> GetAllAbsences();
        Task<List<Absence>> GetAllAbsenceById(int id);
        Task<Absence> BookingAbsence(Absence absence);
    }
    public class AbsenceService : IAbsenceService
    {

        private readonly ApplicationDbContext _context;

        public AbsenceService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Absence>> GetAllAbsences()
        {
            var quey = await _context.Absences.Include(p => p.ApplicationUser).ToListAsync();
            return quey;
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
                var checkAbsence = await _context.Absences.FirstOrDefaultAsync(x => x.FromDateSingle == absence.FromDateSingle);
                var employee = _context.Users.FirstOrDefault(p => p.Id == absence.ApplicationUserId);
                if (checkAbsence != null)
                {
                    if (absence.ShiftTypeSingle == ShiftType.Morning || absence.ShiftTypeSingle == ShiftType.Afternoon)
                    {
                        if (checkAbsence.ShiftTypeSingle == ShiftType.Allday)
                        {
                            throw new Exception("Can not book beacause this day all day");
                        }
                        else if (absence.ShiftTypeSingle == checkAbsence.ShiftTypeSingle)
                        {
                            throw new Exception("Overlap");
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
                            throw new Exception("Have Morning or Afternoon");
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
            return absence;
        }

        public decimal CalculateHourDeducted(ShiftType shiftType)
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
    }
}
