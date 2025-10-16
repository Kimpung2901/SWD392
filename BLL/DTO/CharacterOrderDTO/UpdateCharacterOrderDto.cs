namespace BLL.DTO.CharacterOrderDTO
{
    public class UpdateCharacterOrderDto
    {
        public int? PackageID { get; set; }
        public int? CharacterID { get; set; }
        public int? UserCharacterID { get; set; }
        public decimal? UnitPrice { get; set; }
        public int? QuantityMonths { get; set; } 
        public DateTime? Start_Date { get; set; }
        public DateTime? End_Date { get; set; }
        public string? Status { get; set; }
    }
}