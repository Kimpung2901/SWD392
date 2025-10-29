using BLL.DTO.Common;
using BLL.DTO.UserDTO;
using DAL.Enum;
using DAL.Enum;
using DAL.Models;

namespace BLL.IService;

public interface IUserService
{
    Task<List<UserDto>> GetAllAsync();
    Task<PagedResult<UserDto>> GetAsync(
        string? search, 
        string? sortBy, 
        string? sortDir, 
        int page, 
        int pageSize);

    Task<UserDto?> GetByIdAsync(int id);
    Task<UserDto> CreateAsync(CreateUserDto dto);
    Task<UserDto?> UpdatePartialAsync(int id, UpdateUserDto dto);
    Task<bool> SoftDeleteAsync(int id);
    Task<bool> HardDeleteAsync(int id);
    Task<User?> GetUserByEmailAsync(string email);
    Task<bool> CheckUserExistsAsync(string username, string email);
    Task<bool> ResetPasswordAsync(string email, string newPassword);
    Task<RefreshToken> CreateRefreshTokenAsync(int userId, string? ipAddress);
    Task<UserDto?> UpdateStatusAsync(int id, UserStatus status);
}
