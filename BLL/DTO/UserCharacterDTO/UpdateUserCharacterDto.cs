using System.ComponentModel.DataAnnotations;
using DAL.Enum;

namespace BLL.DTO.UserCharacterDTO
{
    public class UpdateUserCharacterDto
    {
        public DateTime? StartAt { get; set; }

        public DateTime? EndAt { get; set; }

        public bool? AutoRenew { get; set; }

        public UserCharacterStatus? Status { get; set; }
    }
}