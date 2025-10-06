using BLL.Helper;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services.UsersService;

public class UserService
{
    private readonly DollDbContext _context;
    public UserService(DollDbContext db) => _context = db;

    public async Task<User?> AuthenticateAsync(string username, string password)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.UserName == username /* && (u.IsDeleted == false || u.IsDeleted == null) */);

        if (user is null) return null;

        var stored = user.Password;

        if (PasswordHelper.LooksLikeBCrypt(stored))
        {
            return BCrypt.Net.BCrypt.Verify(password, stored) ? user : null;
        }

        if (stored == password)
        {
            var newHash = BCrypt.Net.BCrypt.HashPassword(password);
            user.Password = newHash;
            await _context.SaveChangesAsync();
            return user;
        }

        return null;
    }

    public async Task<User> RegisterAsync(string username, string rawPassword, string role = "User")
    {
        var hash = BCrypt.Net.BCrypt.HashPassword(rawPassword);
        var user = new User
        {
            UserName = username,
            Password = hash,
            Role = role
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }
    public async Task<bool> ChangePasswordAsync(int userId, string oldPassword, string newPassword)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user is null) return false;

        if (!PasswordHelper.LooksLikeBCrypt(user.Password) ||
            !BCrypt.Net.BCrypt.Verify(oldPassword, user.Password))
            return false;

        user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
        await _context.SaveChangesAsync();
        return true;
    }

}
