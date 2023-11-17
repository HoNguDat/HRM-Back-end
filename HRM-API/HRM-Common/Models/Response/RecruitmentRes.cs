using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM_Common.Models.Response
{
    public class RecruitmentRes
    {
        public Guid Id { get; set; }
        public string JobDescription { get; set; }
        public string CreateAt { get; set; }
        public double? SalaryMin { get; set; }
        public double? SalaryMax { get; set; }
        public string Category { get; set; }
        public string EmployeeName { get; set; }
        public string PositionName { get; set; }
        public int? Version { get; set; }
        public Guid PositionId { get; set; }
        public RecruitmentRes() { }
        public RecruitmentRes(Recruitment recruitment)
        {
            Id = recruitment.Id;
            JobDescription = recruitment.JobDescription;
            SalaryMin = recruitment.SalaryMin;
            Category = recruitment.Category.ToString();
            SalaryMax = recruitment.SalaryMax;
            string dateTimeToString = recruitment.CreateAt.ToString("dd/MM/yyyy");
            CreateAt = dateTimeToString;
            Version = recruitment.Version;
            PositionId = recruitment.PositionId;
            PositionName = recruitment.Position?.Name;
            EmployeeName = recruitment.ApplicationUser?.FullName;
        }
    }
}
