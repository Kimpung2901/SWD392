using System.ComponentModel.DataAnnotations;
using DAL.Enum;

namespace BLL.DTO.CharacterOrderDTO
{
    public class UpdateCharacterOrderDto
    {
        public DateTime? StartAt { get; set; }

        public DateTime? EndAt { get; set; }

        public bool? AutoRenew { get; set; }

        public CharacterOrderStatus? Status { get; set; }
    }
}