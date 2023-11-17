using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace HRM_Common.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Thêm chỉ dẫn xác định rằng trường `Version` không được yêu cầu nhập giá trị
            builder.Entity<ApplicationUser>().Property(p => p.Version).ValueGeneratedOnAddOrUpdate();
        }
        public DbSet<Candidate> Candidates { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<Recruitment> Recruitments { get; set; }
        public DbSet<Absence> Absences { get; set; }
        public DbSet<HolidayConfig> HolidayConfigs { get; set; }
        public DbSet<CheckInRecord> CheckInRecords { get; set; }
        public DbSet<Payroll> Payrolls { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }


        public DbSet<ApplicationForm> ApplicationForms { get; set; }
    }
}
