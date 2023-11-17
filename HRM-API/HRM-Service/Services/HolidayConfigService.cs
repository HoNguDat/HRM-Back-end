using HRM_Common.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM_Service.Services
{
    public interface IHolidayConfigService
    {
        Task<List<HolidayConfig>> GetAllHolidayConfig();
        Task<HolidayConfig> AddHolidayConfig(HolidayConfig holidayConfig);
    }
    public class HolidayConfigService : IHolidayConfigService
    {
        private readonly ApplicationDbContext _context;

        public HolidayConfigService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<HolidayConfig>> GetAllHolidayConfig()
        {
            var query = await _context.HolidayConfigs.ToListAsync();
            return query;
        }

        public async Task<HolidayConfig> AddHolidayConfig(HolidayConfig holidayConfig)
        {
            holidayConfig.Year = DateTime.Now.Year;
            _context.HolidayConfigs.Add(holidayConfig);
            await _context.SaveChangesAsync();
            return holidayConfig;
        }
    }
}
