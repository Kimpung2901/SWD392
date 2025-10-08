﻿namespace BLL.DTO.UserDTO
{
    public class CreateUserDto
    {
        public string UserName { get; set; } = null!;
        public string Phones { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Role { get; set; } = "customer";
    }
}
