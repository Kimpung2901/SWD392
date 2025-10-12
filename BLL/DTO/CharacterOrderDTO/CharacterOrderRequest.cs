using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTO.CharacterOrderDTO
{
    public class CharacterOrderRequest
    {
        public int CharacterOrderID { get; set; }
        public int PackageID { get; set; }
        public int CharacterID { get; set; }
        public int UserCharacterID { get; set; }
        public decimal UnitPrice { get; set; }
        public DateTime Start_Date { get; set; }
        public DateTime End_Date { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; } = null!;
    }
}