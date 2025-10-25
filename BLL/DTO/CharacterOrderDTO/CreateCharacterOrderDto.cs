using System.ComponentModel.DataAnnotations;
using DAL.Enum;

namespace BLL.DTO.CharacterOrderDTO
{
    public class CreateCharacterOrderDto
    {
        [Required(ErrorMessage = "PackageID là bắt buộc")]
        public int PackageID { get; set; }

        [Required(ErrorMessage = "CharacterID là bắt buộc")]
        public int CharacterID { get; set; }

    }
}