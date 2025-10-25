using DAL.Enum;

namespace BLL.DTO.OrderDTO
{
    public class OrderItemDto
    {
        public int OrderItemID { get; set; }
        public int OrderID { get; set; }
        public int DollVariantID { get; set; }
        public string? DollVariantName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal { get; set; }
        public OrderItemStatus Status { get; set; }
        public string StatusDisplay => Status.ToString();
    }
}