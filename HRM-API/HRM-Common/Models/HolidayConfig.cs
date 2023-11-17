using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM_Common.Models
{
    public class HolidayConfig
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int AnnualLeave { get; set; }
        public int WeddingLeave { get; set; }
        public int CompassionateLeave { get; set; }
        public int AbsenceFormMustSendBefore { get; set; }

        public int MaternityLeave { get; set; }
        public int PaternityLeave { get; set; }
        public int OneMoreChild { get; set; }
        public int AdoptionLeave { get; set; }

        public int SickLeave { get; set; }
        public int ChildSickLeaveUnder3YearsOld { get; set; }
        public int ChildSickLeaveUnder7YearsOld { get; set; }
        public int Year { get;set; }
    }
}
