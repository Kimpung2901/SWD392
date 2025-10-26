using BLL.DTO.OrderDTO;
using BLL.IService;
using DAL.Enum;
using DAL.IRepo;
using DAL.Models;

namespace BLL.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepo;
        private readonly IOrderItemRepository _orderItemRepo;
        private readonly IUserRepository _userRepo;
        private readonly IDollVariantRepository _variantRepo;
        private readonly IPaymentService _paymentService;

        public OrderService(
            IOrderRepository orderRepo,
            IOrderItemRepository orderItemRepo,
            IUserRepository userRepo,
            IDollVariantRepository variantRepo,
            IPaymentService paymentService)
        {
            _orderRepo = orderRepo;
            _orderItemRepo = orderItemRepo;
            _userRepo = userRepo;
            _variantRepo = variantRepo;
            _paymentService = paymentService;
        }

        public async Task<List<OrderDto>> GetAllAsync()
        {
            var orders = await _orderRepo.GetAllAsync();
            var dtos = new List<OrderDto>();

            foreach (var order in orders)
            {
                var user = order.UserID.HasValue ? await _userRepo.GetByIdAsync(order.UserID.Value) : null;  
                dtos.Add(MapToDto(order, user?.UserName));
            }

            return dtos;
        }

        public async Task<OrderDto?> GetByIdAsync(int id)
        {
            var order = await _orderRepo.GetByIdAsync(id);
            if (order == null) return null;

            var user = await _userRepo.GetByIdAsync((int)order.UserID);
            return MapToDto(order, user?.UserName); 
        }

        public async Task<List<OrderDto>> GetByUserIdAsync(int userId)
        {
            var orders = await _orderRepo.GetByUserIdAsync(userId);
            var user = await _userRepo.GetByIdAsync(userId);
            var dtos = new List<OrderDto>();

            foreach (var order in orders)
            {
                dtos.Add(MapToDto(order, user?.UserName));
            }

            return dtos;
        }

        public async Task<OrderDto?> GetByIdWithItemsAsync(int id)
        {
            var order = await _orderRepo.GetByIdAsync(id);
            if (order == null) return null;

            var user = await _userRepo.GetByIdAsync((int)order.UserID);
            var orderItems = await _orderItemRepo.GetByOrderIdAsync(id);

            var dto = MapToDto(order, user?.UserName);
            dto.OrderItems = new List<OrderItemDto>();

            foreach (var item in orderItems)
            {
                var variant = await _variantRepo.GetByIdAsync(item.DollVariantID);
                dto.OrderItems.Add(new OrderItemDto
                {
                    OrderItemID = item.OrderItemID,
                    OrderID = item.OrderID,
                    DollVariantID = item.DollVariantID,
                    DollVariantName = variant?.Name,
                    UnitPrice = item.UnitPrice,  
                    LineTotal = item.LineTotal,
                    Status = item.Status
                });
            }

            return dto;
        }

        public async Task<OrderDto> CreateAsync(CreateOrderDto dto)
        {
            // Validate user
            var user = await _userRepo.GetByIdAsync(dto.UserID);
            if (user == null)
                throw new Exception($"User với ID {dto.UserID} không tồn tại");

            if (dto.OrderItems == null || !dto.OrderItems.Any())
                throw new Exception("Order phải có ít nhất 1 sản phẩm");

            // Calculate total
            decimal totalAmount = 0;
            var orderItems = new List<OrderItem>();

            foreach (var itemDto in dto.OrderItems)
            {
                var variant = await _variantRepo.GetByIdAsync(itemDto.DollVariantID);
                if (variant == null)
                    throw new Exception($"DollVariant với ID {itemDto.DollVariantID} không tồn tại");

                if (!variant.IsActive)
                    throw new Exception($"DollVariant '{variant.Name}' không còn khả dụng");

                // ✅ THAY ĐỔI: Không nhân với Quantity nữa
                var lineTotal = variant.Price; // Mỗi item = 1 sản phẩm
                totalAmount += lineTotal;

                orderItems.Add(new OrderItem
                {
                    DollVariantID = itemDto.DollVariantID,
                    // ❌ XÓA: Quantity = itemDto.Quantity,
                    UnitPrice = variant.Price,
                    LineTotal = lineTotal, // ✅ LineTotal = UnitPrice (không nhân Quantity)
                    Status = OrderItemStatus.Pending 
                });
            }

            var order = new Order
            {
                UserID = dto.UserID,
                PaymentID = null,
                OrderDate = DateTime.UtcNow,
                TotalAmount = totalAmount,
                ShippingAddress = dto.ShippingAddress,
                Status = OrderStatus.Pending 
            };

            await _orderRepo.AddAsync(order);

            // Add order items
            foreach (var item in orderItems)
            {
                item.OrderID = order.OrderID;
            }
            await _orderItemRepo.AddRangeAsync(orderItems);

            return MapToDto(order, user.UserName); 
        }

        public async Task<OrderDto?> UpdatePartialAsync(int id, UpdateOrderDto dto)
        {
            var entity = await _orderRepo.GetByIdAsync(id);
            if (entity == null) return null;

            if (!string.IsNullOrWhiteSpace(dto.ShippingAddress))
                entity.ShippingAddress = dto.ShippingAddress.Trim();

            if (dto.Status.HasValue)
                entity.Status = dto.Status.Value;

            await _orderRepo.UpdateAsync(entity);

            var user = await _userRepo.GetByIdAsync((int)entity.UserID);
            return MapToDto(entity, user?.UserName); 
        }

        public async Task<bool> DeleteAsync(int id)
        {
            await _orderRepo.DeleteAsync(id);
            return true;
        }

        public async Task<bool> CancelOrderAsync(int id)
        {
            var order = await _orderRepo.GetByIdAsync(id);
            if (order == null) return false;

            if (order.Status == OrderStatus.Completed || order.Status == OrderStatus.Shipped) // ✅ ĐỔI Delivered → Completed
                throw new Exception("Không thể hủy đơn hàng đã giao hoặc đang giao");

            order.Status = OrderStatus.Cancelled;
            await _orderRepo.UpdateAsync(order);

            // Update order items status
            var items = await _orderItemRepo.GetByOrderIdAsync(id);
            foreach (var item in items)
            {
                item.Status = OrderItemStatus.Cancelled;
                await _orderItemRepo.UpdateAsync(item);
            }

            return true;
        }

        private OrderDto MapToDto(Order o, string? userName)
        {
            return new OrderDto
            {
                OrderID = o.OrderID,
                UserID = o.UserID, 
                UserName = userName,
                PaymentID = o.PaymentID,  
                OrderDate = o.OrderDate,
                TotalAmount = o.TotalAmount,
                ShippingAddress = o.ShippingAddress,
                Status = o.Status 
            };
        }
    }
}