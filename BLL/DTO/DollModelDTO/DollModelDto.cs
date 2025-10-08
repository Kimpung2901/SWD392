namespace BLL.DTO.DollModelDTO
{
    public class DollModelDto
    {
        public int DollModelID { get; set; }
        public int DollTypeID { get; set; }
        public string? DollTypeName { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Image { get; set; } = null!;
        public bool IsActive { get; set; }
        public DateTime Create_at { get; set; }
    }
}
