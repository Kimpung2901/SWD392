using System.ComponentModel.DataAnnotations;

namespace BLL.DTO.OrderDTO
{
    // Ensure only one definition of CreateOrderItemDto exists in this namespace
    public class CreateOrderItemDto
    {
        [Required]
        public int DollVariantID { get; set; }

    }
}