using HRM_Common.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM_Service.Services
{
    public interface IDepartmentService
    {
        Task<List<Department>> GetAllDepartments(string? keyword);
        Task<Department> AddDepartment(Department department);
        Task<Department> UpdateDepartment(Guid id, Department department);
        Task<Department> GetDepartmentByName(string name);
        Task<Department> GetDepartmentById(Guid id);
        void DeleteDepartment(Guid id);
    }
    public class DepartmentService : IDepartmentService
    {
        private readonly ApplicationDbContext _context;
        public DepartmentService(ApplicationDbContext applicationDbContext)
        {
            _context = applicationDbContext;
        }

        public async Task<List<Department>> GetAllDepartments(string? keyword)
        {
            var query = _context.Departments.Include(p => p.Positions).Include(p => p.ApplicationUsers).AsQueryable();
            if (!string.IsNullOrEmpty(keyword))
            {
                var lowerKeyword = keyword.ToLower();
                query = query.Where(x => x.Name.ToLower().Contains(keyword));
            }

            return await query.ToListAsync();
        }

        public async Task<Department> GetDepartmentByName(string name)
        {
            var department = await _context.Departments.FirstOrDefaultAsync(p => p.Name == name);
            return department;
        }

        public async Task<Department> GetDepartmentById(Guid id)
        {
            var department = await _context.Departments.FirstOrDefaultAsync(p => p.Id == id);
            return department;
        }

        public async Task<Department> AddDepartment(Department department)
        {
            _context.Departments.Add(department);
            await _context.SaveChangesAsync();

            return department;
        }

        public async Task<Department> UpdateDepartment(Guid id, Department department)
        {
            department.Id = id;
            _context.Entry(department).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return department;
        }

        public void DeleteDepartment(Guid id)
        {
            var department = _context.Departments.Find(id);

            if (department == null)
            {
                throw new Exception("Id không tồn tại");
            }

            _context.Entry(department).State = EntityState.Deleted;
            _context.SaveChanges();
        }
    }
}
