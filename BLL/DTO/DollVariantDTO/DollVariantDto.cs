namespace BLL.DTO.DollVariantDTO
{
    public class DollVariantDto
    {
        public int DollVariantID { get; set; }
        public int DollModelID { get; set; }
        public string DollModelName { get; set; } = string.Empty;
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public string Color { get; set; } = null!;
        public string Size { get; set; } = null!;
        public string Image { get; set; } = null!;
        public bool IsActive { get; set; }
    }
}