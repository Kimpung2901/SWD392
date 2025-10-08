namespace BLL.DTO.DollVariantDTO
{
    public class UpdateDollVariantDto
    {
        public string? Name { get; set; }
        public decimal? Price { get; set; }
        public string? Color { get; set; }
        public string? Size { get; set; }
        public string? Image { get; set; }
        public bool? IsActive { get; set; }
    }
}