// BLL/IService/IAuthService.cs
using DAL.Models;

namespace BLL.IService
{
    public interface IAuthService
    {
        Task<User?> AuthenticateAsync(string username, string password);
        Task<User> RegisterAsync(string username, string rawPassword, string role = "customer", string? email = null);
        Task<bool> ChangePasswordAsync(int userId, string oldPassword, string newPassword);
    }
}
