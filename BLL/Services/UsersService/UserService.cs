using BLL.DTO.UserDTO;
using BLL.IService;
using DAL.IRepo;
using DAL.Models;

namespace BLL.Services.UsersService;

public class UserService : IUserService
{
    private readonly IUserRepository _repo;
    private readonly DollDbContext _context;

    public UserService(IUserRepository repo, DollDbContext db)
    {
        _repo = repo;
        _context = db;
    }

    public async Task<List<UserDto>> GetAllAsync()
    {
        var users = await _repo.GetAllAsync();
        return users.Select(u => new UserDto
        {
            UserID = u.UserID,
            UserName = u.UserName,
            Phones = u.Phones,
            Email = u.Email,
            Status = u.Status,
            Role = u.Role,
            IsDeleted = u.IsDeleted,
            CreatedAt = u.CreatedAt
        }).ToList();
    }

    public async Task<UserDto?> GetByIdAsync(int id)
    {
        var user = await _repo.GetByIdAsync(id);
        if (user == null) return null;

        return new UserDto
        {
            UserID = user.UserID,
            UserName = user.UserName,
            Phones = user.Phones,
            Email = user.Email,
            Status = user.Status,
            Role = user.Role,
            IsDeleted = user.IsDeleted,
            CreatedAt = user.CreatedAt
        };
    }

    public async Task<UserDto> CreateAsync(CreateUserDto dto)
    {
        var entity = new User
        {
            UserName = dto.UserName,
            Phones = dto.Phones,
            Email = dto.Email,
            Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Status = "Active",
            Role = dto.Role,
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        await _repo.AddAsync(entity);
        return await GetByIdAsync(entity.UserID) ?? throw new Exception("Create failed");
    }

    public async Task<UserDto?> UpdatePartialAsync(int id, UpdateUserDto dto)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity == null) return null;

        if (dto.UserName != null) entity.UserName = dto.UserName;
        if (dto.Phones != null) entity.Phones = dto.Phones;
        if (dto.Email != null) entity.Email = dto.Email;
        if (dto.Password != null) entity.Password = BCrypt.Net.BCrypt.HashPassword(dto.Password);
        if (dto.Status != null) entity.Status = dto.Status;
        if (dto.Role != null) entity.Role = dto.Role;
        if (dto.IsDeleted.HasValue) entity.IsDeleted = dto.IsDeleted.Value;

        await _repo.UpdateAsync(entity);
        return await GetByIdAsync(entity.UserID);
    }

    public async Task<bool> SoftDeleteAsync(int id)
    {
        await _repo.SoftDeleteAsync(id);
        return true;
    }

    public async Task<bool> HardDeleteAsync(int id)
    {
        await _repo.HardDeleteAsync(id);
        return true;
    }

    // ✅ Implement methods mới cho Auth
    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _repo.GetUserByEmailAsync(email);
    }

    public async Task<bool> CheckUserExistsAsync(string username, string email)
    {
        return await _repo.CheckUserExistsAsync(username, email);
    }

    public async Task<bool> ResetPasswordAsync(string email, string newPassword)
    {
        var user = await _repo.GetUserByEmailAsync(email);
        if (user == null || user.IsDeleted) return false;

        user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
        await _repo.UpdateAsync(user);
        return true;
    }

    public async Task<RefreshToken> CreateRefreshTokenAsync(int userId, string? ipAddress)
    {
        var refresh = new RefreshToken
        {
            UserID = userId,
            Token = Guid.NewGuid().ToString("N"),
            Expires = DateTime.UtcNow.AddDays(7),
            Created = DateTime.UtcNow,
            CreatedByIp = ipAddress
        };

        _context.RefreshTokens.Add(refresh);
        await _context.SaveChangesAsync();

        return refresh;
    }
}

