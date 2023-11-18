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
        public async Task<IActionResult> Register([FromForm] ApplicationUser model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest("Invalid payload!");
                if (!await _authService.IsUserNameUniqueAsync(model.UserName))
                {
                    return BadRequest("User already exists!");
                }
                if (!await _authService.IsUserEmailUniqueAsync(model.Email))
                {
                    return BadRequest("Email already exists!");
                }
                var data = await _authService.Registeration(model, model.role.ToString());
                return Ok(new { Message = "Add success!", Data = new ApplicationUserRes(data) });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
