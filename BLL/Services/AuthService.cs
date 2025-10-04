// BLL/Services/AuthService.cs
using BLL.IService;
using DAL;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services
{
    public class AuthService : IAuthService
    {
        private readonly DollDbContext _context;

        public AuthService(DollDbContext context)
        {
            _context = context;
        }

        public async Task<User?> AuthenticateAsync(string username, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u =>
                u.UserName == username && !u.IsDeleted && u.Status == "Active");

            if (user == null) return null;

            // Kiểm tra mật khẩu
            return BCrypt.Net.BCrypt.Verify(password, user.Password) ? user : null;
        }

        public async Task<User> RegisterAsync(string username, string rawPassword, string role = "customer", string? email = null)
        {
            var hash = BCrypt.Net.BCrypt.HashPassword(rawPassword);

            var user = new User
            {
                UserName = username,
                Password = hash,
                Email = email ?? "",
                Role = role,
                Status = "Active",
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<bool> ChangePasswordAsync(int userId, string oldPassword, string newPassword)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null || user.IsDeleted) return false;

            if (!BCrypt.Net.BCrypt.Verify(oldPassword, user.Password))
                return false;

            user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
