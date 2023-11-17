using HRM_Common.Models;
using HRM_Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace hrm_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApplicationFormController : ControllerBase
    {

        private readonly IApplicationFormService applicationFormService;
        public ApplicationFormController(IApplicationFormService _applicationFormService)
        {
            applicationFormService = _applicationFormService;
        }
        [HttpGet]
        [Route("get-all-application")]
        public async Task<List<ApplicationForm>> GetApplication()
        {
            var data = await applicationFormService.GetAllApplicationForms();
            return data;
        }
        [HttpPost]
        [Route("add-application")]
        public async Task<IActionResult> AddApplication([FromBody] ApplicationForm application)
        {
            var data = await applicationFormService.AddApplicationForm(application);

            return Ok(data);
        }
    }
}
