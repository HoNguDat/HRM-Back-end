using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM_Common.Models
{
    public class Department
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<ApplicationUser>? ApplicationUsers { get; set; }
        public virtual ICollection<Position>? Positions { get; set; }
    }
}
