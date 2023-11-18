using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using HRM_Common;
using HRM_Common.Models;
using HRM_Common.Models.ViewModels.Authenticate;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace HRM_Service.Services
{
    public interface IAuthService
    {
        Task<ApplicationUser> Registeration(ApplicationUser model, string role);
        Task<TokenViewModel> Login(LoginModel model);
        Task<TokenViewModel> GetRefreshToken(GetRefreshTokenViewModel model);
        Task<bool> LogoutAsync(string userId);
        Task<bool> IsUserNameUniqueAsync(string userName);
        Task<bool> IsUserEmailUniqueAsync(string userName);

    }
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public AuthService(UserManager<ApplicationUser> userManager, IWebHostEnvironment webHostEnvironment, RoleManager<IdentityRole> roleManager, IConfiguration configuration, ApplicationDbContext context)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            _configuration = configuration;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<bool> IsUserNameUniqueAsync(string userName)
        {
            var userExists = await userManager.FindByNameAsync(userName);
            return userExists == null;
        }
        public async Task<bool> IsUserEmailUniqueAsync(string email)
        {
            var userExists = await userManager.FindByEmailAsync(email);
            return userExists == null;
        }
        public async Task<ApplicationUser> Registeration(ApplicationUser model, string role)
        {
            Position? position = null;
            Department? department = null;
            if (model.PositionId != null)
            {
                position = _context.Positions.FirstOrDefault(p => p.Id == model.PositionId);
                department = _context.Departments.FirstOrDefault(p => p.Id == position.DepartmentId);
            }

            var latestEmployee = _context.ApplicationUsers.OrderByDescending(e => e.EmployeeNumber).FirstOrDefault();
            int latestEmployeeNumber = latestEmployee?.EmployeeNumber ?? 0;
            int nextEmployeeNumber = latestEmployeeNumber + 1;
            string employeeCode = $"NV-{nextEmployeeNumber}";
            string uniqueFileName = UploadedFile(model);
            ApplicationUser user = new()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                FullName = model.FullName,
                UserName = model.UserName,
                Address = model.Address,
                Code = employeeCode,
                Password = model.Password,
                role = model.role,
                Version = model.Version,
                Salary = model.Salary,
                Dob = model.Dob,
                Gender = model.Gender,
                PlaceOfBirth = model.PlaceOfBirth,
                StartDate = model.StartDate,
                EmployeeStatus = EmployeeStatus.Working,
                EmploymentCategory = model.EmploymentCategory,
                EmployeeLevel = model.EmployeeLevel,
                Inactive = false,
                DepartmentId = position?.DepartmentId,
                Department = department,
                Position = position,
                PhoneNumber = model.PhoneNumber,
                BankAccount = model.BankAccount,
                BankName = model.BankName,
                FormFile = model.FormFile,
                Avatar = uniqueFileName,
                EmployeeNumber = nextEmployeeNumber
            };
            try
            {
                var createUserResult = await userManager.CreateAsync(user, model.Password);
                if (!createUserResult.Succeeded)
                    throw new("User creation failed! Please check user details and try again.");

                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));

                if (await roleManager.RoleExistsAsync(role))
                    await userManager.AddToRoleAsync(user, role);

            }
            catch (DbUpdateException ex)
            {
                var innerException = ex.InnerException;
                while (innerException != null)
                {
                    Console.WriteLine(innerException.Message);
                    innerException = innerException.InnerException;
                }
            }
            return user;
        }
        private string UploadedFile(ApplicationUser model)
        {
            string uniqueFileName = string.Empty;
            if (model.FormFile != null)
            {
                string uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "assets/avatars");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.FormFile.FileName;
                string filePath = Path.Combine(uploadFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.FormFile.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }
        public async Task<TokenViewModel> Login(LoginModel model)
        {
            var user = await userManager.FindByNameAsync(model.UserName);
            TokenViewModel _TokenViewModel = new TokenViewModel();

            if (user == null)
            {
                _TokenViewModel.StatusCode = 0;
                _TokenViewModel.StatusMessage = "Invalid username";
                return _TokenViewModel;
            }
            if (!await userManager.CheckPasswordAsync(user, model.Password))
            {
                _TokenViewModel.StatusCode = 0;
                _TokenViewModel.StatusMessage = "Invalid password";

                return _TokenViewModel;
            }

            var userRoles = await userManager.GetRolesAsync(user);
            var authClaims = new List<Claim>
            {
               new Claim(ClaimTypes.Name, user.UserName),
               new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }
            _TokenViewModel.UserId = user.Id;
            _TokenViewModel.FullName = user.FullName;
            _TokenViewModel.Email = user.Email;
            _TokenViewModel.AccessToken = GenerateToken(authClaims);
            _TokenViewModel.RefreshToken = GenerateRefreshToken();
            _TokenViewModel.StatusCode = 1;
            _TokenViewModel.StatusMessage = "Success";

            var _RefreshTokenValidityInDays = Convert.ToInt64(_configuration["JWTKey:RefreshTokenValidityInDays"]);
            user.RefreshToken = _TokenViewModel.RefreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(_RefreshTokenValidityInDays);
            await userManager.UpdateAsync(user);


            return _TokenViewModel;
        }

        public async Task<bool> LogoutAsync(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user != null)
            {
                user.RefreshToken = null;
                user.RefreshTokenExpiryTime = null;
                var result = await userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    // Đăng xuất thành công
                    return true;
                }
            }

            return false;
        }

        public async Task<TokenViewModel> GetRefreshToken(GetRefreshTokenViewModel model)
        {
            TokenViewModel _TokenViewModel = new();
            var principal = GetPrincipalFromExpiredToken(model.AccessToken);
            string username = principal.Identity.Name;
            var user = await userManager.FindByNameAsync(username);

            if (user == null || user.RefreshToken != model.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
            {
                _TokenViewModel.StatusCode = 0;
                _TokenViewModel.StatusMessage = "Invalid access token or refresh token";
                return _TokenViewModel;
            }

            var authClaims = new List<Claim>
            {
               new Claim(ClaimTypes.Name, user.UserName),
               new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };
            var newAccessToken = GenerateToken(authClaims);
            var newRefreshToken = GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            await userManager.UpdateAsync(user);

            _TokenViewModel.StatusCode = 1;
            _TokenViewModel.StatusMessage = "Success";
            _TokenViewModel.AccessToken = newAccessToken;
            _TokenViewModel.RefreshToken = newRefreshToken;
            return _TokenViewModel;
        }

        private string GenerateToken(IEnumerable<Claim> claims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWTKey:Secret"]));
            var _TokenExpiryTimeInHour = Convert.ToInt64(_configuration["JWTKey:TokenExpiryTimeInHour"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _configuration["JWTKey:ValidIssuer"],
                Audience = _configuration["JWTKey:ValidAudience"],
                //Expires = DateTime.UtcNow.AddHours(_TokenExpiryTimeInHour),
                Expires = DateTime.UtcNow.AddMinutes(30),
                SigningCredentials = new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256),
                Subject = new ClaimsIdentity(claims)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private static string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string? token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWTKey:Secret"])),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;
        }
    }
}
