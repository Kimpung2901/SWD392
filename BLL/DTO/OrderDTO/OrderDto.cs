using DAL.Enum;

namespace BLL.DTO.OrderDTO
{
    public class OrderDto
    {
        public int OrderID { get; set; }
        public int? UserID { get; set; }
        public int? PaymentID { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string ShippingAddress { get; set; } = null!;
        public OrderStatus Status { get; set; }
        public string StatusDisplay { get; }
        public List<OrderItemDto> OrderItems { get; set; } = new();
        public string UserName { get; set; } // <-- Add this property
    }
}