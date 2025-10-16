namespace BLL.DTO.OrderDTO
{
    public class OrderDto
    {
        public int OrderID { get; set; }
        public int? UserID { get; set; }  // ✅ Đổi thành nullable
        public string? UserName { get; set; }
        public int? PaymentID { get; set; }  // ✅ Đổi thành nullable
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string? Currency { get; set; }  // ✅ Nullable
        public string? ShippingAddress { get; set; }  // ✅ Nullable
        public string? Status { get; set; }  // ✅ Nullable
        public List<OrderItemDto>? OrderItems { get; set; }
    }
}