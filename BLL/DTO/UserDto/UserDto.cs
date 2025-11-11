using DAL.Enum;

namespace BLL.DTO.UserDTO
{
    public class UserDto
    {
        public int UserID { get; set; }
        public string UserName { get; set; } = null!;
        public string? FullName { get; set; }
        public string Phones { get; set; } = null!;
        public string Email { get; set; } = null!;
        public int? Age { get; set; } = null!;
        public UserStatus Status { get; set; } = UserStatus.Active;
        public string Role { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public string? DeviceToken { get; set; }
    }
}
