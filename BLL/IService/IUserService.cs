using BLL.DTO.UserDTO;
using DAL.Models;

namespace BLL.IService
{
    public interface IUserService
    {
        // CRUD
        Task<List<UserDto>> GetAllAsync();
        Task<UserDto?> GetByIdAsync(int id);
        Task<UserDto> CreateAsync(CreateUserDto dto);
        Task<UserDto?> UpdatePartialAsync(int id, UpdateUserDto dto);
        Task<bool> SoftDeleteAsync(int id);
        Task<bool> HardDeleteAsync(int id);

        // Auth
        //Task<User?> AuthenticateAsync(string username, string password);
        //Task<User> RegisterAsync(string username, string rawPassword, string role = "User");
        //Task<bool> ChangePasswordAsync(int userId, string oldPassword, string newPassword);
    }
}
