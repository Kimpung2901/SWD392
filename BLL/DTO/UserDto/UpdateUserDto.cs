namespace BLL.DTO.UserDTO
{
    public class UpdateUserDto
    {
        public string? UserName { get; set; }
        public string? Phones { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Status { get; set; }
        public string? Role { get; set; }
        public bool? IsDeleted { get; set; }
    }
}
