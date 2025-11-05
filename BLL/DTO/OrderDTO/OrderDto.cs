using DAL.Enum;

namespace BLL.DTO.OrderDTO
{
    public class OrderDto
    {
        public int OrderID { get; set; }
        public int? UserID { get; set; }
        public int? PaymentID { get; set; }
        public int DollVariantID { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string ShippingAddress { get; set; } = null!;
        public OrderStatus Status { get; set; }
        public string StatusDisplay { get; }
        public string UserName { get; set; }
        public string? DollVariantName { get; set; }
    }
}
