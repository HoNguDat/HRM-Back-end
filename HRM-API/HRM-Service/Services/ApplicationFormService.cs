using HRM_Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace HRM_Service.Services
{
    public interface IApplicationFormService
    {
        Task<List<ApplicationForm>> GetAllApplicationForms();
        Task<ApplicationForm> AddApplicationForm(ApplicationForm applicationForm);


    }

    public class ApplicationFormService : IApplicationFormService
    {
        private readonly ApplicationDbContext _context;

        public ApplicationFormService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ApplicationForm>> GetAllApplicationForms()
        {

            var query = await _context.ApplicationForms.Include(p => p.Recruitment).ToListAsync();
            return query;

        }

        public async Task<ApplicationForm> AddApplicationForm(ApplicationForm applicationForm)
        {
            applicationForm.Id = Guid.NewGuid();
            var recruitment = _context.Recruitments.FirstOrDefault(p => p.Id == applicationForm.RecruitmentId);
            if (recruitment != null)
            {

                applicationForm.Recruitment = recruitment;
                _context.ApplicationForms.Add(applicationForm);
                await _context.SaveChangesAsync();
            }

            return applicationForm;
        }



    }
}
