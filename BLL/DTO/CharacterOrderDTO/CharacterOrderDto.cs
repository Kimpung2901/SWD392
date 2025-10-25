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
        public int UserCharacterID { get; set; }
        public int QuantityMonths { get; set; }
        public decimal UnitPrice { get; set; }
        public DateTime Start_Date { get; set; }
        public DateTime End_Date { get; set; }
        public DateTime CreatedAt { get; set; }
        public CharacterOrderStatus Status { get; set; }
        
        public string StatusDisplay => Status.ToString();
    }
}