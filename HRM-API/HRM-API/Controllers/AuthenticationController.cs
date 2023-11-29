using HRM_Common.Models;
using HRM_Common.Models.Response;
using HRM_Common.Models.ViewModels.Authenticate;
using HRM_Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;

namespace hrm_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthenticationController> _logger;
        public AuthenticationController(IAuthService authService, ILogger<AuthenticationController> logger, UserManager<ApplicationUser> userManager)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest("Invalid payload");
                var result = await _authService.Login(model);
                if (result.StatusCode == 0)
                    return BadRequest(result.StatusMessage);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        [Route("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId != null)
            {
                var result = await _authService.LogoutAsync(userId);
                if (result)
                {
                    return Ok("Logged out successfully.");
                }
            }

            return BadRequest("Unable to log out.");
        }

        [HttpPost]
        [Route("registeration")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Register([FromForm] ApplicationUser model)
        {
            try
            {
                string[] allowedExtensions = { ".jpg", ".png", ".gif" };
                if (!_authService.IsValidImageFile(model.FormFile, allowedExtensions))
                {
                    return BadRequest(new { Message = "The file is not an image file!", Status = "400", Description = "Bad Request" });
                }
                if (!_authService.IsValidNull(model))
                {
                    return BadRequest(new { Message = "Position does not exists!", Status = "400", Description = "Bad Request" });
                }
                if (!ModelState.IsValid)
                    return BadRequest(new { Message = "Invalid payload!" });
                if (!await _authService.IsUserNameUniqueAsync(model.UserName))
                {
                    return BadRequest(new { Message = "User already exists!", Status = "400", Description = "Bad Request" });
                }
                if (!await _authService.IsUserEmailUniqueAsync(model.Email))
                {
                    return BadRequest(new { Message = "Email already exists!", Status = "400", Description = "Bad Request" });
                }
                if (!_authService.IsValidEmail(model.Email))
                {
                    return BadRequest(new { Message = "Email invalidate!", Status = "400", Description = "Bad Request" });
                }
                if (!_authService.IsValidPhoneNumber(model.PhoneNumber))
                {
                    return BadRequest(new { Message = "Phone number invalidate!", Status = "400", Description = "Bad Request" });
                }
                if (!_authService.IsValidDate(model.Dob.ToString()))
                {
                    return BadRequest(new { Message = "Dob invalidate!", Status = "400", Description = "Bad Request" });
                }
                if (!_authService.IsValidStartDate(model.StartDate.ToString()))
                {
                    return BadRequest(new { Message = "StartDate invalidate!", Status = "400", Description = "Bad Request" });
                }
                if (!_authService.IsValidInput(model.FullName))
                {
                    return BadRequest(new { Message = "FullName invalidate!", Status = "400", Description = "Bad Request" });
                }
                if (!_authService.IsValidInput(model.Email))
                {
                    return BadRequest(new { Message = "Email invalidate!", Status = "400", Description = "Bad Request" });
                }
                if (!_authService.IsValidInput(model.PhoneNumber))
                {
                    return BadRequest(new { Message = "PhoneNumber invalidate!", Status = "400", Description = "Bad Request" });
                }
                if (!_authService.IsValidInput(model.Address))
                {
                    return BadRequest(new { Message = "Address invalidate!", Status = "400", Description = "Bad Request" });
                }
                if (!_authService.IsValidInput(model.PlaceOfBirth))
                {
                    return BadRequest(new { Message = "PlaceOfBirth invalidate!", Status = "400", Description = "Bad Request" });
                }
                if (!_authService.IsValidInput(model.BankAccount))
                {
                    return BadRequest(new { Message = "BankAccount invalidate!", Status = "400", Description = "Bad Request" });
                }
                if (!_authService.IsValidInput(model.BankName))
                {
                    return BadRequest(new { Message = "BankName invalidate!", Status = "400", Description = "Bad Request" });
                }
                if (!_authService.IsValidInput(model.UserName))
                {
                    return BadRequest(new { Message = "UserName invalidate!", Status = "400", Description = "Bad Request" });
                }
                if (!_authService.IsValidInput(model.Password))
                {
                    return BadRequest(new { Message = "Password invalidate!", Status = "400", Description = "Bad Request" });
                }

                if (!_authService.IsValidLengthAndCharacters(model.FullName))
                {
                    return BadRequest(new { Message = "FullName invalidate!", Status = "400", Description = "Bad Request" });
                }

                if (!_authService.IsValidLengthAndCharacters(model.PhoneNumber))
                {
                    return BadRequest(new { Message = "PhoneNumber invalidate!", Status = "400", Description = "Bad Request" });
                }
                if (!_authService.IsValidLengthAndCharactersAddress(model.Address))
                {
                    return BadRequest(new { Message = "Address invalidate!", Status = "400", Description = "Bad Request" });
                }
                if (!_authService.IsValidLengthAndCharactersAddress(model.PlaceOfBirth))
                {
                    return BadRequest(new { Message = "PlaceOfBirth invalidate!", Status = "400", Description = "Bad Request" });
                }
                if (!_authService.IsValidBankAccountNumber(model.BankAccount))
                {
                    return BadRequest(new { Message = "BankAccount invalidate!", Status = "400", Description = "Bad Request" });
                }
                if (!_authService.IsValidLengthAndCharacters(model.BankName))
                {
                    return BadRequest(new { Message = "BankName invalidate!", Status = "400", Description = "Bad Request" });
                }
                if (!_authService.IsValidLengthAndCharacters(model.UserName))
                {
                    return BadRequest(new { Message = "UserName invalidate!", Status = "400", Description = "Bad Request" });
                }
                if (!_authService.IsValidPassword(model.Password))
                {
                    return BadRequest(new { Message = "Password invalidate!", Status = "400", Description = "Bad Request" });
                }
                var data = await _authService.Registeration(model, model.role.ToString());
                return Ok(new { Message = "Add success!", Data = new ApplicationUserRes(data), Status = "200", Description = "Success" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
