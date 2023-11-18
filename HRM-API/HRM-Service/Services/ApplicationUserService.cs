﻿using HRM_Common.Models;
using HRM_Common.Paged;
using HRM_Common.ReqModules;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;


namespace HRM_Service.Services
{
    public interface IApplicationUserService
    {
        Task<List<ApplicationUser>> GetAllApplicationUser(string? keyword);
        Task<ApplicationUser> UpdateApplicationUser(string id, ApplicationUser applicationUser);
        Task<ApplicationUser> GetApplicationUserByFullName(string name);
        Task<ApplicationUser> GetApplicationUserById(string id);
        Task<PagedResult<ApplicationUser>> GetApplicationUserByStatus(GetApplicationUserModule req);

        void DeleteApplicationUser(string id);
        Task<PagedResult<ApplicationUser>> GetAllPaging(GetApplicationUserModule req);

    }
    public class ApplicationUserService : IApplicationUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ApplicationUserService(ApplicationDbContext applicationDbContext, IWebHostEnvironment webHostEnvironment)
        {
            _context = applicationDbContext;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<List<ApplicationUser>> GetAllApplicationUser(string? keyword)
        {

            var query = _context.ApplicationUsers.Include(p => p.Department).Include(p => p.Position).Include(p => p.Absences).Include(p => p.Payrolls).Include(p => p.CheckInRecords).AsQueryable();
            if (!string.IsNullOrEmpty(keyword))
            {
                var lowerKeyword = keyword.ToLower();
                query = query.Where(x => x.FullName.ToLower().Contains(keyword));
            }

            return await query.ToListAsync();
        }
        public async Task<PagedResult<ApplicationUser>> GetAllPaging(GetApplicationUserModule req)
        {
            var query = _context.ApplicationUsers.Include(p => p.Department).Include(p => p.Position).Include(p => p.Absences).Include(p => p.Payrolls).Include(p => p.CheckInRecords).AsQueryable();
            if (!string.IsNullOrEmpty(req.Keyword))
            {
                query = query.Where(s => s.FullName.ToLower().Contains(req.Keyword.ToLower()));
            }
            if (req.Direction.Equals("asc", StringComparison.OrdinalIgnoreCase))
            {
                query = query.OrderBy(s => s.EmployeeNumber); // Sắp xếp tăng dần
            }
            else if (req.Direction.Equals("desc", StringComparison.OrdinalIgnoreCase))
            {
                query = query.OrderByDescending(s => s.EmployeeNumber); // Sắp xếp giảm dần
            }
            var skip = (req.Page - 1) * req.PageSize;
            int total = query.Count();

            var results = query.Skip(skip).Take(req.PageSize).ToList();

            var data = new PagedResult<ApplicationUser>
            {
                Results = results,
                Total = total
            };

            return data;
        }

        public async Task<ApplicationUser> GetApplicationUserByFullName(string fullname)
        {
            var applicationUser = await _context.ApplicationUsers.Include(p => p.Department).Include(p => p.Position).Include(p => p.Absences).Include(p => p.Payrolls).Include(p => p.CheckInRecords).AsQueryable().FirstOrDefaultAsync(p => p.FullName == fullname);
            return applicationUser;
        }
        public async Task<ApplicationUser> GetApplicationUserById(string id)
        {
            var applicationUser = await _context.ApplicationUsers.Include(p => p.Department).Include(p => p.Position).Include(p => p.Absences).Include(p => p.Payrolls).Include(p => p.CheckInRecords).AsQueryable().FirstOrDefaultAsync(p => p.Id == id);
            return applicationUser;
        }
        public async Task<PagedResult<ApplicationUser>> GetApplicationUserByStatus(GetApplicationUserModule req)
        {
            EmployeeStatus? employeeStatus = Enum.TryParse<EmployeeStatus>(req.Keyword, out var result)
            ? result
            : (EmployeeStatus?)null;
            var query = _context.ApplicationUsers.Include(p => p.Department).Include(p => p.Position).Include(p => p.Absences).Include(p => p.Payrolls).Include(p => p.CheckInRecords).AsQueryable();
            if (!string.IsNullOrEmpty(req.Keyword))
            {
                query = query.Where(s => s.EmployeeStatus == employeeStatus);
            }
            if (req.Direction.Equals("asc", StringComparison.OrdinalIgnoreCase))
            {
                query = query.OrderBy(s => s.EmployeeNumber); // Sắp xếp tăng dần
            }
            else if (req.Direction.Equals("desc", StringComparison.OrdinalIgnoreCase))
            {
                query = query.OrderByDescending(s => s.EmployeeNumber); // Sắp xếp giảm dần
            }
            var skip = (req.Page - 1) * req.PageSize;
            int total = query.Count();

            var results = query.Skip(skip).Take(req.PageSize).ToList();

            var data = new PagedResult<ApplicationUser>
            {
                Results = results,
                Total = total
            };

            return data;
        }


        public async Task<ApplicationUser> UpdateApplicationUser(string id, ApplicationUser applicationUser)
        {
            Position? position = null;
            Department? department = null;
            var data = await _context.ApplicationUsers.FindAsync(id);
            if (data == null)
            {
                // Xử lý trường hợp người dùng không tồn tại
                throw new Exception("Người dùng không tồn tại.");
            }
            string uniqueFileName = "";
            if (applicationUser != null)
            {
                position = _context.Positions.FirstOrDefault(p => p.Id == applicationUser.PositionId);
                if (position != null)
                {
                    department = _context.Departments.FirstOrDefault(p => p.Id == position.DepartmentId);
                }
                uniqueFileName = UploadedFile(applicationUser);
            }
            var userExists = await _context.ApplicationUsers.FindAsync(applicationUser?.UserName);
            if (userExists != null && userExists != data)
            {
                throw new("User already exists");

            }
            // Kiểm tra phiên bản của người dùng hiện tại
            if (!IsVersionValid(applicationUser, data))
            {
                // Xử lý trường hợp xung đột
                throw new DbUpdateConcurrencyException("Xung đột dữ liệu. Dữ liệu đã bị sửa đổi bởi người khác.");
            }
            data.Version = applicationUser.Version;
            data.Id = applicationUser.Id;
            data.Code = applicationUser.Code;
            data.FullName = applicationUser.FullName;
            data.Address = applicationUser.Address;
            data.Salary = applicationUser.Salary;
            data.Avatar = uniqueFileName;
            data.Gender = applicationUser.Gender;
            data.Dob = applicationUser.Dob;
            data.PlaceOfBirth = applicationUser.PlaceOfBirth;
            data.BankAccount = applicationUser.BankAccount;
            data.BankName = applicationUser.BankName;
            data.StartDate = applicationUser.StartDate;
            data.EmployeeLevel = applicationUser.EmployeeLevel;
            data.EmployeeStatus = applicationUser.EmployeeStatus;
            data.EmploymentCategory = applicationUser.EmploymentCategory;
            data.DepartmentId = position?.DepartmentId;
            data.PositionId = position?.Id;
            data.Position = position;
            data.Department = department;
            data.PhoneNumber = applicationUser.PhoneNumber;
            data.Email = applicationUser.Email;
            data.UserName = applicationUser.UserName;
            data.Password = applicationUser.Password;
            data.role = applicationUser.role;
            _context.Update(data);
            await _context.SaveChangesAsync();

            return data;
        }
        private bool IsVersionValid(ApplicationUser updatedUser, ApplicationUser existingUser)
        {
            if (updatedUser.Version == null || existingUser.Version == null)
            {
                return false;
            }

            // So sánh phiên bản của người dùng cập nhật với phiên bản hiện tại
            return updatedUser.Version.SequenceEqual(existingUser.Version);
        }
        private string UploadedFile(ApplicationUser model)
        {
            string uniqueFileName = string.Empty;
            if (!string.IsNullOrEmpty(model.Avatar))
            {
                string oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "assets/avatars", model.Avatar);
                if (File.Exists(oldFilePath))
                {
                    File.Delete(oldFilePath);
                }
            }


            if (model.FormFile != null)
            {
                string uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "assets/avatars");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.FormFile.FileName;
                string filePath = Path.Combine(uploadFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.FormFile.CopyTo(fileStream);
                }
            }

            return uniqueFileName;
        }

        public void DeleteApplicationUser(string id)
        {
            var applicationUser = _context.ApplicationUsers.Find(id);

            if (applicationUser == null)
            {
                throw new Exception("Id không tồn tại");
            }
            _context.Entry(applicationUser).State = EntityState.Deleted;
            _context.SaveChanges();
        }

    }
}
