using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
 
namespace HRM_Common.Models
{
    public class Candidate
    {
        [Key]
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public Gender Gender { get; set; }
        public DateTime Dob { get; set; }
        public string Nationality { get; set; }
        public string PresentAddress { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public double ExpectedSalary { get; set; }
        [MaxLength]
        public string CVUrl { get; set; }
        [NotMapped]
        public IFormFile FormFile { get; set; }
        public CandidateStatus Status { get; set; }

        public Guid RecruitmentId { get; set; }
        public virtual Recruitment? Recruitment { get; set; }

    }
    public enum CandidateStatus
    {
        Pending = 0,
        PassInterview = 1,
        FailInterview = 2,
        CandidateDeclined = 3,
        CandidateAcceptedOffer = 4,
        Blacklist = 5,
    }
}
