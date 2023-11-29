using HRM_Common.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM_Service.Services
{
    public interface ICandidateService
    {
        Task<List<Candidate>> GetAllCandidates(string? keyword);
    }
    public class CandidateService:ICandidateService
    {
        private readonly ApplicationDbContext _context;

        public CandidateService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Candidate>> GetAllCandidates(string? keyword)
        {
            var query = _context.Candidates.Include(p => p.Recruitment).AsQueryable();
            if (!string.IsNullOrEmpty(keyword))
            {
                var lowerKeyword = keyword.ToLower();
                query = query.Where(x => x.FullName.ToLower().Contains(lowerKeyword));
            }

            return await query.ToListAsync();
        }
    }
}
