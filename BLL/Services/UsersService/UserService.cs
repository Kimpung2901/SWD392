using BLL.DTO.Common;
using BLL.DTO.UserDTO;
using BLL.Helper;
using BLL.IService;
using DAL.Enum;
using DAL.IRepo;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

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
        return users.Select(MapToDto).ToList();
    }

    public async Task<PagedResult<UserDto>> GetAsync(
        string? search,
        string? sortBy,
        string? sortDir,
        int page,
        int pageSize)
    {
        var query = _repo.GetQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchLower = search.ToLower().Trim();
            query = query.Where(u =>
                u.UserName.ToLower().Contains(searchLower) ||
                (u.FullName != null && u.FullName.ToLower().Contains(searchLower)) ||
                u.Email.ToLower().Contains(searchLower) ||
                (u.Phones != null && u.Phones.ToLower().Contains(searchLower)));
        }

        var total = await query.CountAsync();
        query = ApplySorting(query, sortBy, sortDir);
        query = query.ApplyPagination(page, pageSize);
        var items = await query.ToListAsync();

        return new PagedResult<UserDto>
        {
            Items = items.Select(MapToDto).ToList(),
            Total = total,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<UserDto?> GetByIdAsync(int id)
    {
        var user = await _repo.GetByIdAsync(id);
        return user == null ? null : MapToDto(user);
    }

    public async Task<UserDto> CreateAsync(CreateUserDto dto)
    {

        var existsCheck = await _repo.CheckUserExistsAsync(dto.UserName.Trim(), dto.Email.Trim());
        if (existsCheck)
        {
            throw new InvalidOperationException("Username hoặc Email đã tồn tại trong hệ thống");
        }

        var vietnamNow = DateTimeHelper.GetVietnamTime();

        var entity = new User
        {
            UserName = dto.UserName.Trim(),
            FullName = dto.FullName?.Trim(),
            Phones = dto.Phones?.Trim(),
            Email = dto.Email.Trim(),
            Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Age = dto.Age,
            Status = UserStatus.Active,
            Role = dto.Role?.Trim() ?? "customer",
            CreatedAt = vietnamNow,  
            IsDeleted = false
        };

        await _repo.AddAsync(entity);
        return await GetByIdAsync(entity.UserID) ?? throw new Exception("Tạo user thất bại");
    }

    public async Task<UserDto?> UpdatePartialAsync(int id, UpdateUserDto dto)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity == null) return null;

        if (dto.FullName != null) entity.FullName = dto.FullName.Trim();
        if (dto.Phones != null) entity.Phones = dto.Phones.Trim();
        if (dto.Email != null)
        {

            var emailExists = await _context.Users
                .AnyAsync(u => u.Email == dto.Email.Trim() && u.UserID != id);
            if (emailExists)
            {
                throw new InvalidOperationException("Email đã được sử dụng bởi user khác");
            }
            entity.Email = dto.Email.Trim();
        }
        if (dto.Age.HasValue) entity.Age = dto.Age.Value;

        await _repo.UpdateAsync(entity);
        return await GetByIdAsync(entity.UserID);
    }

    public async Task<UserDto?> UpdateStatusAsync(int id, UserStatus status)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity == null) return null;

        entity.Status = status;
        await _repo.UpdateAsync(entity);

        return MapToDto(entity);
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

        var vietnamNow = DateTimeHelper.GetVietnamTime();

        var refresh = new RefreshToken
        {
            UserID = userId,
            Token = Guid.NewGuid().ToString("N"),
            Expires = vietnamNow.AddDays(7), 
            Created = vietnamNow,             
            CreatedByIp = ipAddress
        };

        _context.RefreshTokens.Add(refresh);
        await _context.SaveChangesAsync();

        return refresh;
    }

    public async Task<bool> UpdateDeviceTokenAsync(int userId, string deviceToken)
    {
        var user = await _repo.GetByIdAsync(userId);
        if (user == null) return false;

        user.DeviceToken = deviceToken?.Trim();
        await _repo.UpdateAsync(user);
        return true;
    }


    private static UserDto MapToDto(User u) => new()
    {
        UserID = u.UserID,
        UserName = u.UserName,
        FullName = u.FullName,
        Phones = u.Phones,
        Email = u.Email,
        Age = u.Age,
        Status = u.Status,
        Role = u.Role,
        CreatedAt = u.CreatedAt,
        DeviceToken = u.DeviceToken
    };

    private static IQueryable<User> ApplySorting(
        IQueryable<User> query,
        string? sortBy,
        string? sortDir)
    {
        if (string.IsNullOrWhiteSpace(sortBy))
            return query.OrderByDescending(u => u.UserID);

        var isDescending = string.Equals(sortDir, "desc", StringComparison.OrdinalIgnoreCase);

        return sortBy.ToLower() switch
        {
            "userid" => isDescending
                ? query.OrderByDescending(u => u.UserID)
                : query.OrderBy(u => u.UserID),

            "username" => isDescending
                ? query.OrderByDescending(u => u.UserName)
                : query.OrderBy(u => u.UserName),

            "fullname" => isDescending
                ? query.OrderByDescending(u => u.FullName)
                : query.OrderBy(u => u.FullName),

            "email" => isDescending
                ? query.OrderByDescending(u => u.Email)
                : query.OrderBy(u => u.Email),

            "createdat" => isDescending
                ? query.OrderByDescending(u => u.CreatedAt)
                : query.OrderBy(u => u.CreatedAt),

            "age" => isDescending
                ? query.OrderByDescending(u => u.Age)
                : query.OrderBy(u => u.Age),

            "status" => isDescending
                ? query.OrderByDescending(u => u.Status)
                : query.OrderBy(u => u.Status),

            _ => query.ApplySort(sortBy, sortDir)
        };
    }
}

