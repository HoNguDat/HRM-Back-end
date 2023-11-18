using HRM_Common.Models;
using HRM_Common.Models.Response;
using HRM_Common.Paged;
using HRM_Common.ReqModules;
using HRM_Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace hrm_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //Authorize(Roles = "Admin")]
    public class ApplicationUserController : ControllerBase
    {
        private readonly IApplicationUserService applicationUserService;
        private readonly ILogger<ApplicationUserController> _logger;


        public ApplicationUserController(IApplicationUserService _applicationService, ILogger<ApplicationUserController> logger)
        {
            applicationUserService = _applicationService;
            _logger = logger;
        }

        [HttpGet]
        [Route("get-all-employee")]
        public async Task<List<ApplicationUserRes>> GetApplicationUser(string? keyword)
        {
            var data = await applicationUserService.GetAllApplicationUser(keyword);
            var result = data.Select(p => new ApplicationUserRes(p)).ToList();
            return result;
        }


        [HttpGet]
        [Route("get-all-paging-employee")]
        public async Task<IActionResult> GetAllPaging([FromQuery] GetApplicationUserModule req)
        {
            var data = await applicationUserService.GetAllPaging(req);
            var result = new PagedResult<ApplicationUserRes>
            {
                Results = data.Results.Select(p => new ApplicationUserRes(p)).ToList(),
                Total = data.Total
            };
            return Ok(new { Message = "Get data success!", Data = result });
        }

        [HttpGet]
        [Route("get-employee-status")]
        public async Task<IActionResult> GetApplicationUserByStatus([FromQuery] GetApplicationUserModule req)
        {
            var data = await applicationUserService.GetApplicationUserByStatus(req);
            var result = new PagedResult<ApplicationUserRes>
            {
                Results = data.Results.Select(p => new ApplicationUserRes(p)).ToList(),
                Total = data.Total
            };
            return Ok(new { Message = "Get data success!", Data = result });
        }

        [HttpGet]
        [Route("get-employee/{id?}")]
        public async Task<IActionResult> GetApplicationUserById(string id)
        
        {
            try
            {
                var data = await applicationUserService.GetApplicationUserById(id);

                if (data == null)
                {
                    return NotFound( new { Message = "User does not exist!" });
                }
                var result = new ApplicationUserRes(data);
                return Ok(new {Message = "Get data success!", Data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }

        [HttpDelete]
        [Route("delete-employee/{id}")]
        public async Task<IActionResult> DeleteApplicationUser(string id)
        {
            var existingapplicationUser = await applicationUserService.GetApplicationUserById(id);
            if (existingapplicationUser == null)
            {
                return BadRequest();
            }
            applicationUserService.DeleteApplicationUser(id);
            return Ok();
        }

        [HttpPut]
        [Route("update-employee/{id}")]
        public async Task<IActionResult> UpdateApplicationUser(string id, [FromForm] ApplicationUser applicationUser)
        {
            var data = await applicationUserService.UpdateApplicationUser(id, applicationUser);
            return Ok(data);
        }
    }
}
