using BLL.IService;
using DAL.IRepo;
using DAL.Models;
using BLL.Services.MailService;
using DAL.Enum;

namespace BLL.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _users;
        private readonly IAuthRepository _auth;
        private readonly IEmailSender _email;

        public AuthService(IUserRepository users, IAuthRepository auth, IEmailSender email)
        { 
            _users = users; 
            _auth = auth; 
            _email = email; 
        }

        public async Task<User?> AuthenticateAsync(string username, string password)
        {
            var user = await _users.GetUserByUsernameAsync(username.Trim());
            if (user == null || user.IsDeleted || user.Status != UserStatus.Active)
                return null;
            return BCrypt.Net.BCrypt.Verify(password, user.Password) ? user : null;
        }

        public async Task<User> RegisterAsync(
            string username, 
            string rawPassword, 
            string role = "Customer", 
            string? email = null, 
            string? phone = null,
            int? age = null) 
        {
            var uname = username.Trim();
            if (await _users.GetUserByUsernameAsync(uname) != null)
                throw new InvalidOperationException("Username already exists");

          
            var validRoles = new[] { "Admin", "Manager", "Customer" };
            var normalizedRole = role.Trim();
            if (!validRoles.Contains(normalizedRole, StringComparer.OrdinalIgnoreCase))
            {
                normalizedRole = "Customer"; 
            }

            var user = new User
            {
                UserName = uname,
                Email = email?.Trim(),
                Phones = phone?.Trim(),
                Password = BCrypt.Net.BCrypt.HashPassword(rawPassword),
                Age = age, 
                Role = normalizedRole,
                Status = UserStatus.Active, 
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
                user.Status != UserStatus.Active) 
                return false;

            if (!BCrypt.Net.BCrypt.Verify(oldPassword, user.Password)) 
                return false;

            user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
            await _users.UpdateAsync(user);
            await _users.SaveChangesAsync();
            return true;
        }

        public async Task<User?> AuthenticateWithGoogleAsync(string idToken, string? ipAddress)
        {
            return await Task.FromResult<User?>(null);
        }

        public async Task<bool> CanSendOtpAsync(string email)
        {
            var user = await _users.GetUserByEmailAsync(email.Trim());
           
            return user != null && !user.IsDeleted && user.Status == UserStatus.Active; 
        }
    }
}
