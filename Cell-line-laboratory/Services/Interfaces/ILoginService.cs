using Cell_line_laboratory.Models;
using System.Security.Claims;

namespace Cell_line_laboratory.Services.Interfaces
{
    public interface ILoginService
    {
        Task<ClaimsPrincipal> Authenticate(SignInViewModel model);
        Task<object> GetUserByEmailAsync(string email);
    }
}
