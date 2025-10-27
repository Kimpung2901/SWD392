using BLL.DTO.Common;
using BLL.DTO.Common;
using BLL.DTO.OrderDTO;
using BLL.Helper;
using BLL.IService;
using DAL.Enum;
using DAL.IRepo;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepo;
    private readonly IOrderItemRepository _orderItemRepo;
    private readonly IUserRepository _userRepo;
    private readonly IDollVariantRepository _variantRepo;

    public OrderService(
        IOrderRepository orderRepo,
        IOrderItemRepository orderItemRepo,
        IUserRepository userRepo,
        IDollVariantRepository variantRepo)
    {
        _orderRepo = orderRepo;
        _orderItemRepo = orderItemRepo;
        _userRepo = userRepo;
        _variantRepo = variantRepo;
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

    // ✅ THÊM: Search/Sort/Pagination
    public async Task<PagedResult<OrderDto>> GetAsync(
        string? search,
        string? sortBy,
        string? sortDir,
        int page,
        int pageSize)
    {
        var query = _orderRepo.GetQueryable();

        // ✅ Search: Tìm theo OrderID, ShippingAddress
        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchLower = search.ToLower().Trim();
            query = query.Where(o =>
                o.OrderID.ToString().Contains(searchLower) ||
                o.ShippingAddress.ToLower().Contains(searchLower) ||
                o.Status.ToString().ToLower().Contains(searchLower));
        }

        // ✅ Count total
        var total = await query.CountAsync();

        // ✅ Apply sorting
        query = ApplySorting(query, sortBy, sortDir);

        // ✅ Apply pagination
        query = query.ApplyPagination(page, pageSize);

        // ✅ Execute và map sang DTO
        var orders = await query.ToListAsync();
        var items = new List<OrderDto>();

        foreach (var order in orders)
        {
            var user = order.UserID.HasValue ? await _userRepo.GetByIdAsync(order.UserID.Value) : null;
            items.Add(MapToDto(order, user?.UserName));
        }

        return new PagedResult<OrderDto>
        {
            Items = items,
            Total = total,
            Page = page,
            PageSize = pageSize
        };
    }

    // ✅ THÊM: Get orders by userId với pagination
    public async Task<PagedResult<OrderDto>> GetOrdersByUserIdAsync(
        int userId,
        string? search,
        string? sortBy,
        string? sortDir,
        int page,
        int pageSize)
    {
        var query = _orderRepo.GetQueryable().Where(o => o.UserID == userId);

        // Search
        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchLower = search.ToLower().Trim();
            query = query.Where(o =>
                o.OrderID.ToString().Contains(searchLower) ||
                o.ShippingAddress.ToLower().Contains(searchLower) ||
                o.Status.ToString().ToLower().Contains(searchLower));
        }

        var total = await query.CountAsync();
        query = ApplySorting(query, sortBy, sortDir);
        query = query.ApplyPagination(page, pageSize);

        var orders = await query.ToListAsync();
        var user = await _userRepo.GetByIdAsync(userId);
        var items = orders.Select(o => MapToDto(o, user?.UserName)).ToList();

        return new PagedResult<OrderDto>
        {
            Items = items,
            Total = total,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<OrderDto?> GetByIdAsync(int id)
    {
        var order = await _orderRepo.GetByIdAsync(id);
        if (order == null) return null;

        var user = order.UserID.HasValue ? await _userRepo.GetByIdAsync(order.UserID.Value) : null;
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

        var user = order.UserID.HasValue ? await _userRepo.GetByIdAsync(order.UserID.Value) : null;
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
        var user = await _userRepo.GetByIdAsync(dto.UserID);
        if (user == null)
            throw new Exception($"User với ID {dto.UserID} không tồn tại");

        if (dto.OrderItems == null || !dto.OrderItems.Any())
            throw new Exception("Order phải có ít nhất 1 sản phẩm");

        decimal totalAmount = 0;
        var orderItems = new List<OrderItem>();

        foreach (var itemDto in dto.OrderItems)
        {
            var variant = await _variantRepo.GetByIdAsync(itemDto.DollVariantID);
            if (variant == null)
                throw new Exception($"DollVariant với ID {itemDto.DollVariantID} không tồn tại");

            if (!variant.IsActive)
                throw new Exception($"DollVariant '{variant.Name}' không còn khả dụng");

            var lineTotal = variant.Price;
            totalAmount += lineTotal;

            orderItems.Add(new OrderItem
            {
                DollVariantID = itemDto.DollVariantID,
                UnitPrice = variant.Price,
                LineTotal = lineTotal,
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

        var user = entity.UserID.HasValue ? await _userRepo.GetByIdAsync(entity.UserID.Value) : null;
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

        if (order.Status == OrderStatus.Completed || order.Status == OrderStatus.Shipped)
            throw new Exception("Không thể hủy đơn hàng đã giao hoặc đang giao");

        order.Status = OrderStatus.Cancelled;
        await _orderRepo.UpdateAsync(order);

        var items = await _orderItemRepo.GetByOrderIdAsync(id);
        foreach (var item in items)
        {
            item.Status = OrderItemStatus.Cancelled;
            await _orderItemRepo.UpdateAsync(item);
        }

        return true;
    }

    // ✅ PRIVATE HELPERS
    private static OrderDto MapToDto(Order o, string? userName) => new()
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

    private static IQueryable<Order> ApplySorting(
        IQueryable<Order> query,
        string? sortBy,
        string? sortDir)
    {
        if (string.IsNullOrWhiteSpace(sortBy))
            return query.OrderByDescending(o => o.OrderDate);

        var isDescending = string.Equals(sortDir, "desc", StringComparison.OrdinalIgnoreCase);

        return sortBy.ToLower() switch
        {
            "orderid" => isDescending
                ? query.OrderByDescending(o => o.OrderID)
                : query.OrderBy(o => o.OrderID),

            "orderdate" => isDescending
                ? query.OrderByDescending(o => o.OrderDate)
                : query.OrderBy(o => o.OrderDate),

            "totalamount" => isDescending
                ? query.OrderByDescending(o => o.TotalAmount)
                : query.OrderBy(o => o.TotalAmount),

            "status" => isDescending
                ? query.OrderByDescending(o => o.Status)
                : query.OrderBy(o => o.Status),

            _ => query.ApplySort(sortBy, sortDir)
        };
    }
}