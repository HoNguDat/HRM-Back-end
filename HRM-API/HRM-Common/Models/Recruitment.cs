using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM_Common.Models
{
    public class Recruitment
    {
        [Key]
        public Guid Id { get; set; }
        [MaxLength]
        public string JobDescription { get; set; }
        public DateTime CreateAt { get; set; }
        public double? SalaryMin { get; set; }
        public double? SalaryMax { get; set; }
        public EmploymentCategory Category { get; set; }
        public int? Version { get; set; }
        public Guid PositionId { get; set; }
        public virtual Position? Position { get; set; }

        public string ApplicationUserId { get; set; }
        public virtual ApplicationUser? ApplicationUser { get; set; }

        public ICollection<Candidate>? Candidates { get; set; }
        public ICollection<ApplicationForm>? ApplicationForms { get; set; }
    }
}
