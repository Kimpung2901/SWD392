using BLL.IService;
using DAL.IRepo;
using DAL.Models;
using BLL.Services.MailService;  


namespace BLL.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _users;
        private readonly IAuthRepository _auth;
        private readonly IEmailSender _email; // BLL.Services.MailService

        public AuthService(IUserRepository users, IAuthRepository auth, IEmailSender email)
        { _users = users; _auth = auth; _email = email; }

        public async Task<User?> AuthenticateAsync(string username, string password)
        {
            var user = await _users.GetUserByUsernameAsync(username.Trim());
            if (user == null || user.IsDeleted || !user.Status.Equals("Active", StringComparison.OrdinalIgnoreCase))
                return null;
            return BCrypt.Net.BCrypt.Verify(password, user.Password) ? user : null;
        }

        public async Task<User> RegisterAsync(string username, string rawPassword, string role = "customer",
                                              string? email = null, string? phone = null)
        {
            var uname = username.Trim();
            if (await _users.GetUserByUsernameAsync(uname) != null)
                throw new InvalidOperationException("Username already exists");

            var user = new User
            {
                UserName = uname,
                Email = email?.Trim(),
                Phones = phone?.Trim(),
                Password = BCrypt.Net.BCrypt.HashPassword(rawPassword),
                Role = role.Trim(),
                Status = "Active",
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            await _users.AddAsync(user);
            await _users.SaveChangesAsync(); 
            return user;
        }

        public async Task<bool> ChangePasswordAsync(int userId, string oldPassword, string newPassword)
        {
            var user = await _auth.GetUserByIdAsync(userId);
            if (user == null || user.IsDeleted ||
                !user.Status.Equals("Active", StringComparison.OrdinalIgnoreCase)) return false;

            if (!BCrypt.Net.BCrypt.Verify(oldPassword, user.Password)) return false;

            user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
            await _users.UpdateAsync(user);
    
            return true;
        }


    }

}
