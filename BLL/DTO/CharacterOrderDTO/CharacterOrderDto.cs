using System.ComponentModel.DataAnnotations;
using DAL.Enum;

namespace BLL.DTO.CharacterOrderDTO
{
    public class CharacterOrderDto
    {
        public int CharacterOrderID { get; set; }
        public int PackageID { get; set; }
        public string? PackageName { get; set; }
        public int CharacterID { get; set; }
        public string? CharacterName { get; set; }
        public decimal UnitPrice { get; set; }
        public DateTime CreatedAt { get; set; }
        public CharacterOrderStatus Status { get; set; }
        
        public string StatusDisplay => Status.ToString();
    }
}