using System.ComponentModel.DataAnnotations;
using DAL.Enum;

namespace BLL.DTO.CharacterOrderDTO
{
    public class UpdateCharacterOrderDto
    {

        public bool? AutoRenew { get; set; }

        public CharacterOrderStatus? Status { get; set; }
    }
}