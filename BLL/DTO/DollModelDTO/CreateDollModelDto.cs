namespace BLL.DTO.DollModelDTO
{
    public class CreateDollModelDto
    {
        public int DollTypeID { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Image { get; set; } = null!;
    }
}
