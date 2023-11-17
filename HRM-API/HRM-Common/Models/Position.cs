using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM_Common.Models
{
    public class Position
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }

        public Guid DepartmentId { get; set; }
        public virtual Department? Department { get; set; }

        public virtual ICollection<ApplicationUser>? ApplicationUsers { get; set; }
        public virtual ICollection<Recruitment>? Recruitments { get; set; }
    }
}
