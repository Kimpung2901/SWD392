using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using DAL.Enum;

namespace BLL.DTO.UserDTO;

public class UpdateUserStatusDto
{
    [Required(ErrorMessage = "Status là bắt buộc")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public UserStatus Status { get; set; }
}