
using Cell_line_laboratory.Entities;
using Cell_line_laboratory.Exceptions;
using Cell_line_laboratory.Models;
using Cell_line_laboratory.Services.Interfaces;
using Cell_line_laboratory.Utils;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using static Cell_line_laboratory.Models.ForgotPasswordModel;

namespace ArtisanELearningSystem.Services
{
    public class LoginService : ILoginService
    {
        public const string Student = "Student";
        public const string Instructor = "Instructor";


        private readonly IUserService userService;



        public LoginService(IUserService _userService)
        {
            userService = _userService;


        }


        public async Task<ClaimsPrincipal> Authenticate(SignInViewModel model)
        {
            var user = await userService.Authenticate(model);
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Email),
            new Claim(ClaimTypes.GivenName, user.Name),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // User ID claim
            new Claim(ClaimTypes.Role, user.Role), // User roles claim
            
            // Add any other claims as needed
        };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            return principal;
        }



        public async Task<object> GetUserByEmailAsync(string email)
        {
            User user = await GetAllUserByEmailAsync(email);

            if (user == null)
            {
                return new { Status = ForgotPasswordStatus.Error, ErrorMessage = "User not found." };
            }

            var newPassword = GeneratePassword(10);
            var hashedPassword = HashPassword(newPassword);
            user.Password = hashedPassword;
            await userService.UpdateUser(user);


            await SendPasswordResetEmailAsync(email, newPassword);

            return new { Status = ForgotPasswordStatus.Success };
        }

        private async Task<User> GetAllUserByEmailAsync(string email)
        {
            // for forgot password reset
            return await userService.GetUserByEmail(email);


        }

        private string GeneratePassword(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();

            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private string HashPassword(string password)
        {
            var hasher = new PasswordHasher<object>();
            return hasher.HashPassword(null, password);
        }

        private async Task SendPasswordResetEmailAsync(string email, string newPassword)
        {        
            var emailBody = $"Your new password is: {newPassword}";
            await EmailSender.SendEmailAsync(email, "Password Reset", emailBody);
        }



    }
}


