using HRM_Common.Models;
using HRM_Common.Models.Response;
using HRM_Common.Paged;
using HRM_Common.ReqModules;
using HRM_Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace hrm_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
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
            string[] allowedExtensions = { ".jpg", ".png", ".gif" };
            if (!applicationUserService.IsValidImageFile(applicationUser.FormFile, allowedExtensions))
            {
                return BadRequest(new { Message = "The file is not an image file!"  ,Status = "400", Description = "Bad Request" });
            }
            if (!applicationUserService.IsValidNull(applicationUser))
            {
                return BadRequest(new { Message = "Position does not exists!", Status = "400", Description = "Bad Request" });
            }
            if (!ModelState.IsValid)
                return BadRequest("Invalid payload!");
            if (!applicationUserService.IsValidEmail(applicationUser.Email))
            {
                return BadRequest(new { Message = "Email invalidate!", Status = "400", Description = "Bad Request" });
            }
            if (!applicationUserService.IsValidPhoneNumber(applicationUser.PhoneNumber))
            {
                return BadRequest(new { Message = "Phone number invalidate!", Status = "400", Description = "Bad Request" });
            }
            if (!applicationUserService.IsValidDate(applicationUser.Dob.ToString()))
            {
                return BadRequest(new { Message = "Dob invalidate!", Status = "400", Description = "Bad Request" });
            }
            if (!applicationUserService.IsValidStartDate(applicationUser.StartDate.ToString()))
            {
                return BadRequest(new { Message = "StartDate invalidate!", Status = "400", Description = "Bad Request" });
            }
            if (!applicationUserService.IsValidInput(applicationUser.FullName))
            {
                return BadRequest(new { Message = "FullName invalidate!", Status = "400", Description = "Bad Request" });
            }
            if (!applicationUserService.IsValidInput(applicationUser.Email))
            {
                return BadRequest(new { Message = "Email invalidate!", Status = "400", Description = "Bad Request" });
            }
            if (!applicationUserService.IsValidInput(applicationUser.PhoneNumber))
            {
                return BadRequest(new { Message = "PhoneNumber invalidate!", Status = "400", Description = "Bad Request" });
            }
            if (!applicationUserService.IsValidInput(applicationUser.Address))
            {
                return BadRequest(new { Message = "Address invalidate!", Status = "400", Description = "Bad Request" });
            }
            if (!applicationUserService.IsValidInput(applicationUser.PlaceOfBirth))
            {
                return BadRequest(new { Message = "PlaceOfBirth invalidate!", Status = "400", Description = "Bad Request" });
            }
            if (!applicationUserService.IsValidInput(applicationUser.BankAccount))
            {
                return BadRequest(new { Message = "BankAccount invalidate!", Status = "400", Description = "Bad Request" });
            }
            if (!applicationUserService.IsValidInput(applicationUser.BankName))
            {
                return BadRequest(new { Message = "BankName invalidate!", Status = "400", Description = "Bad Request" });
            }
            if (!applicationUserService.IsValidInput(applicationUser.UserName))
            {
                return BadRequest(new { Message = "UserName invalidate!", Status = "400", Description = "Bad Request" });
            }
            if (!applicationUserService.IsValidInput(applicationUser.Password))
            {
                return BadRequest(new { Message = "Password invalidate!", Status = "400", Description = "Bad Request" });
            }

            if (!applicationUserService.IsValidLengthAndCharacters(applicationUser.FullName))
            {
                return BadRequest(new { Message = "FullName invalidate!", Status = "400", Description = "Bad Request" });
            }
            if (!applicationUserService.IsValidLengthAndCharacters(applicationUser.Email))
            {
                return BadRequest(new { Message = "Email invalidate!", Status = "400", Description = "Bad Request" });
            }
            if (!applicationUserService.IsValidLengthAndCharacters(applicationUser.PhoneNumber))
            {
                return BadRequest(new { Message = "PhoneNumber invalidate!", Status = "400", Description = "Bad Request" });
            }
            if (!applicationUserService.IsValidLengthAndCharactersAddress(applicationUser.Address))
            {
                return BadRequest(new { Message = "Address invalidate!", Status = "400", Description = "Bad Request" });
            }
            if (!applicationUserService.IsValidLengthAndCharactersAddress(applicationUser.PlaceOfBirth))
            {
                return BadRequest(new { Message = "PlaceOfBirth invalidate!", Status = "400", Description = "Bad Request" });
            }
            if (!applicationUserService.IsValidBankAccountNumber(applicationUser.BankAccount))
            {
                return BadRequest(new { Message = "BankAccount invalidate!", Status = "400", Description = "Bad Request" });
            }
            if (!applicationUserService.IsValidLengthAndCharacters(applicationUser.BankName))
            {
                return BadRequest(new { Message = "BankName invalidate!", Status = "400", Description = "Bad Request" });
            }
            if (!applicationUserService.IsValidLengthAndCharacters(applicationUser.UserName))
            {
                return BadRequest(new { Message = "UserName invalidate!", Status = "400", Description = "Bad Request" });
            }
            if (!applicationUserService.IsValidPassword(applicationUser.Password))
            {
                return BadRequest(new { Message = "Password invalidate!", Status = "400", Description = "Bad Request" });
            }
            var userExists = await applicationUserService.GetApplicationUserById(id);
            if(userExists.UserName == applicationUser.UserName)
            {
                return BadRequest(new { Message = "User name exist!", Status = "400", Description = "Bad Request" });
            }
            if (userExists.Email == applicationUser.Email)
            {
                return BadRequest( new { Message = "Email exist!", Status = "400", Description = "Bad Request" });
            }

            if (userExists == null)
            {
                return NotFound(new { Message = "User does not exist!", Status = "400", Description = "Bad Request" });
            }
            if (!applicationUserService.IsVersionValid(applicationUser, userExists))
            {
                return Conflict(new { Messeage = "Data conflict" , Status = "409", Description = "Conflict" });
            }
            var data = await applicationUserService.UpdateApplicationUser(id, applicationUser);
            return Ok(new { Message = "Get data success!", Data = data, Status = "200", Description = "Success" });
        }
    }
}
