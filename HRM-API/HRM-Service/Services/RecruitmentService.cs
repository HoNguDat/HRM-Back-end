using HRM_Common.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM_Service.Services
{
    public interface IRecruitmentService
    {
        Task<List<Recruitment>> GetAllRecruitment(string? keyword);
        Task<Recruitment> AddRecruitment(Recruitment recruitment);
        Task<Recruitment> UpdateRecruitment(Guid id, Recruitment recruitment);
        Task<Recruitment> GetRecruitmentById(Guid id);
        void DeleteRecruitment(Guid id);
    }
    public class RecruitmentService : IRecruitmentService
    {

        private readonly ApplicationDbContext _context;

        public RecruitmentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Recruitment>> GetAllRecruitment(string? keyword)
        {
            var query = _context.Recruitments.Include(p => p.Candidates).Include(p => p.ApplicationForms).Include(p => p.Position).Include(p => p.ApplicationUser).AsQueryable();
            if (!string.IsNullOrEmpty(keyword))
            {
                var lowerKeyword = keyword.ToLower();
                query = query.Where(x => x.Position.Name.ToLower().Contains(lowerKeyword));
            }

            return await query.ToListAsync();
        }

        public async Task<Recruitment> GetRecruitmentById(Guid id)
        {
            var recruitment = await _context.Recruitments.Include(p => p.Candidates).Include(p => p.ApplicationForms).Include(p => p.Position).Include(p => p.ApplicationUser).FirstOrDefaultAsync(p => p.Id == id);
            return recruitment;
        }

        public async Task<Recruitment> AddRecruitment(Recruitment recruitment)
        {
            var position = _context.Positions.FirstOrDefault(p => p.Id == recruitment.PositionId);
            var employee = _context.Users.FirstOrDefault(p => p.Id == recruitment.ApplicationUserId);
            recruitment.Position = position;
            recruitment.ApplicationUser = employee;
            recruitment.CreateAt = DateTime.Now;
            recruitment.Version = 1;
            _context.Recruitments.Add(recruitment);
            await _context.SaveChangesAsync();

            return recruitment;
        }

        public async Task<Recruitment> UpdateRecruitment(Guid id, Recruitment recruitment)
        {
            var existingRecruitment = await _context.Recruitments.FindAsync(id);
            if (existingRecruitment == null)
            {
                throw new Exception("Not found");
            }
            var position = _context.Positions.FirstOrDefault(p => p.Id == recruitment.PositionId);
            var employee = _context.Users.FirstOrDefault(p => p.Id == recruitment.ApplicationUserId);
            existingRecruitment.Id = id;
            existingRecruitment.JobDescription = recruitment.JobDescription;
            existingRecruitment.SalaryMax = recruitment.SalaryMax;
            existingRecruitment.SalaryMin = recruitment.SalaryMin;
            existingRecruitment.Category = recruitment.Category;
            existingRecruitment.PositionId = recruitment.PositionId;
            existingRecruitment.ApplicationUserId = recruitment.ApplicationUserId;
            existingRecruitment.CreateAt = DateTime.Now;
            existingRecruitment.Version += 1;
            existingRecruitment.ApplicationUser = employee;
            existingRecruitment.Position = position;
            await _context.SaveChangesAsync();
            return existingRecruitment;
        }

        public void DeleteRecruitment(Guid id)
        {
            var recruitment = _context.Recruitments.Find(id);

            if (recruitment == null)
            {
                throw new Exception("Id không tồn tại");
            }

            _context.Entry(recruitment).State = EntityState.Deleted;
            _context.SaveChanges();
        }
    }
}
