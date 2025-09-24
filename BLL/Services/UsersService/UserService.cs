using DAL;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services;


public class UserService
 {
    private readonly DollDbContext _context; // hoặc Swd392DbContext theo tên của bạn
    public UserService(DollDbContext db) => _context = db;

    public async Task<User?> AuthenticateAsync(string username, string password)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == username ); //&& !u.IsDeleted);
        if (user is null ) return null;  //|| !user.IsActive

        // Nếu bạn lưu PasswordHash bằng BCrypt:
        // return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash) ? user : null;

        // Nếu hiện tại DB đang lưu plain-text (tạm thời để test):
        return user.Password == password ? user : null;
    }
 }
    

