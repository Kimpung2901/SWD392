namespace BLL.DTO.UserDTO
{
    public class UserDto
    {
        public int UserID { get; set; }
        public string UserName { get; set; } = null!;
        public string Phones { get; set; } = null!;
        public string Email { get; set; } = null!;
        public int? Age { get; set; } = null!;
        public string Status { get; set; } = null!;
        public string Role { get; set; } = null!;
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
