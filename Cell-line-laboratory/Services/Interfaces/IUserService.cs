using Cell_line_laboratory.Entities;
using Cell_line_laboratory.Models;

namespace Cell_line_laboratory.Services.Interfaces
{
    public interface IUserService
    {
        Task<User> Authenticate(SignInViewModel model);
        Task<bool> CreateUser(User User);
        Task DeleteUser(int id);
        Task<List<User>> GetAllUsers();
        Task<User> GetUserByEmail(string email);
        Task<User> GetUser(int? id);
        Task<User> GetUserByUserId(int? userId);
        Task<User> GetUserId(int? id);
        Task UpdateUser(User User);
        bool UserExists(int id);
       
    }
}
