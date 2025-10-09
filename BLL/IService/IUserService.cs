using BLL.DTO.UserDTO;
using DAL.Models;

namespace BLL.IService;

public interface IUserService
{
    // CRUD
    Task<List<UserDto>> GetAllAsync();
    Task<UserDto?> GetByIdAsync(int id);
    Task<UserDto> CreateAsync(CreateUserDto dto);
    Task<UserDto?> UpdatePartialAsync(int id, UpdateUserDto dto);
    Task<bool> SoftDeleteAsync(int id);
    Task<bool> HardDeleteAsync(int id);

    // ✅ Thêm methods mới cho Auth
    Task<User?> GetUserByEmailAsync(string email);
    Task<bool> CheckUserExistsAsync(string username, string email);
    Task<bool> ResetPasswordAsync(string email, string newPassword);
    Task<RefreshToken> CreateRefreshTokenAsync(int userId, string? ipAddress);
}
