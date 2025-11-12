using System;
using BLL.DTO.Common;
using BLL.DTO.OrderDTO;
using BLL.Helper;
using BLL.IService;
using DAL.Enum;
using DAL.IRepo;
using DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BLL.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepo;
    private readonly IUserRepository _userRepo;
    private readonly IDollVariantRepository _variantRepo;
    private readonly DollDbContext _db; // ✅ FIX 1: Thêm field
    private readonly IOwnedDollManager _ownedDollManager;
    private readonly ILogger<OrderService> _logger;

    public OrderService(
        IOrderRepository orderRepo,
        IUserRepository userRepo,
        IDollVariantRepository variantRepo,
        DollDbContext db, // ✅ FIX 2: Thêm parameter
        IOwnedDollManager ownedDollManager,
        ILogger<OrderService> logger)
    {
        _orderRepo = orderRepo;
        _userRepo = userRepo;
        _variantRepo = variantRepo;
        _db = db; // ✅ FIX 3: Gán giá trị
        _ownedDollManager = ownedDollManager;
        _logger = logger;
    }

    public async Task<List<OrderDto>> GetAllAsync()
    {
        var orders = await _orderRepo.GetAllAsync();
        var result = new List<OrderDto>();

        foreach (var order in orders)
        {
            var userName = order.UserID.HasValue
                ? (await _userRepo.GetByIdAsync(order.UserID.Value))?.UserName
                : null;
            var variant = order.DollVariantID.HasValue
                ? await _variantRepo.GetByIdAsync(order.DollVariantID.Value)
                : null;

            result.Add(MapToDto(order, userName, variant?.Name));
        }

        return result;
    }

    public async Task<PagedResult<OrderDto>> GetAsync(
        string? search,
        string? sortBy,
        string? sortDir,
        int page,
        int pageSize)
    {
        var query = _orderRepo.GetQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchLower = search.Trim().ToLower();
            query = query.Where(o =>
                o.OrderID.ToString().Contains(searchLower) ||
                (o.ShippingAddress != null && o.ShippingAddress.ToLower().Contains(searchLower)) ||
                o.Status.ToString().ToLower().Contains(searchLower));
        }

        var total = await query.CountAsync();

        query = ApplySorting(query, sortBy, sortDir);
        query = query.ApplyPagination(page, pageSize);

        var orders = await query.ToListAsync();
        var items = new List<OrderDto>();

        foreach (var order in orders)
        {
            var userName = order.UserID.HasValue
                ? (await _userRepo.GetByIdAsync(order.UserID.Value))?.UserName
                : null;

            var variant = order.DollVariantID.HasValue
                ? await _variantRepo.GetByIdAsync(order.DollVariantID.Value)
                : null;

            items.Add(MapToDto(order, userName, variant?.Name));
        }

        return new PagedResult<OrderDto>
        {
            Items = items,
            Total = total,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<PagedResult<OrderDto>> GetOrdersByUserIdAsync(
        int userId,
        string? search,
        string? sortBy,
        string? sortDir,
        int page,
        int pageSize)
    {
        var query = _orderRepo.GetQueryable().Where(o => o.UserID == userId);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchLower = search.Trim().ToLower();
            query = query.Where(o =>
                o.OrderID.ToString().Contains(searchLower) ||
                (o.ShippingAddress != null && o.ShippingAddress.ToLower().Contains(searchLower)) ||
                o.Status.ToString().ToLower().Contains(searchLower));
        }

        var total = await query.CountAsync();
        query = ApplySorting(query, sortBy, sortDir);
        query = query.ApplyPagination(page, pageSize);

        var orders = await query.ToListAsync();
        var user = await _userRepo.GetByIdAsync(userId);
        var items = new List<OrderDto>();
        foreach (var order in orders)
        {
            var variant = order.DollVariantID.HasValue
                ? await _variantRepo.GetByIdAsync(order.DollVariantID.Value)
                : null;
            items.Add(MapToDto(order, user?.UserName, variant?.Name));
        }

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
        if (order == null)
            return null;

        var userName = order.UserID.HasValue
            ? (await _userRepo.GetByIdAsync(order.UserID.Value))?.UserName
            : null;
        var variant = order.DollVariantID.HasValue
            ? await _variantRepo.GetByIdAsync(order.DollVariantID.Value)
            : null;

        return MapToDto(order, userName, variant?.Name);
    }

    public async Task<List<OrderDto>> GetByUserIdAsync(int userId)
    {
        var orders = await _orderRepo.GetByUserIdAsync(userId);
        var user = await _userRepo.GetByIdAsync(userId);
        var result = new List<OrderDto>();

        foreach (var order in orders)
        {
            var variant = order.DollVariantID.HasValue
                ? await _variantRepo.GetByIdAsync(order.DollVariantID.Value)
                : null;
            result.Add(MapToDto(order, user?.UserName, variant?.Name));
        }

        return result;
    }


    public async Task<OrderDto> CreateAsync(CreateOrderDto dto, int userId)
    {

        var user = await _userRepo.GetByIdAsync(userId);
        if (user == null)
            throw new Exception($"User with ID {userId} does not exist.");

        var variant = await _variantRepo.GetByIdAsync(dto.DollVariantID);
        if (variant == null)
            throw new Exception($"Doll variant with ID {dto.DollVariantID} does not exist.");

        if (!variant.IsActive)
            throw new Exception($"Doll variant '{variant.Name}' is inactive.");

        // ✅ Tự động lấy giá từ variant
        var totalAmount = variant.Price;

        var order = new Order
        {
            UserID = userId,  // ✅ Dùng userId từ parameter
            PaymentID = null,
            DollVariantID = dto.DollVariantID,
            OrderDate = DateTime.UtcNow,
            TotalAmount = totalAmount,
            ShippingAddress = dto.ShippingAddress,
            Status = OrderStatus.Pending
        };

        await _orderRepo.AddAsync(order);

        return MapToDto(order, user.UserName, variant.Name);
    }

    public async Task<OrderDto?> UpdatePartialAsync(int id, UpdateOrderDto dto)
    {
        var entity = await _orderRepo.GetByIdAsync(id);
        if (entity == null)
            return null;

        var oldStatus = entity.Status;

        if (!string.IsNullOrWhiteSpace(dto.ShippingAddress))
            entity.ShippingAddress = dto.ShippingAddress.Trim();

        if (dto.Status.HasValue)
            entity.Status = dto.Status.Value;

        string? variantName = null;
        if (dto.DollVariantID.HasValue && dto.DollVariantID.Value != entity.DollVariantID)
        {
            var variant = await _variantRepo.GetByIdAsync(dto.DollVariantID.Value);
            if (variant == null)
                throw new Exception($"Doll variant with ID {dto.DollVariantID.Value} does not exist.");

            if (!variant.IsActive)
                throw new Exception($"Doll variant '{variant.Name}' is inactive.");

            entity.DollVariantID = dto.DollVariantID.Value;
            entity.TotalAmount = variant.Price;
            variantName = variant.Name;
        }

        // ✅ TẠO OwnedDoll KHI ADMIN CHUYỂN SANG COMPLETED
        if (oldStatus != OrderStatus.Completed && entity.Status == OrderStatus.Completed)
        {
            // ✅ SỬ DỤNG OwnedDollManager thay vì trực tiếp query _db
            var ownedDollCreated = await _ownedDollManager.EnsureOwnedDollForOrderAsync(
                entity, 
                "OrderService.UpdatePartialAsync");
            
            if (ownedDollCreated)
            {
                _logger.LogInformation(
                    "[Order] OwnedDoll created for Order #{OrderId} when marked as Completed",
                    entity.OrderID);
            }
        }

        // ✅ UpdateAsync sẽ save tất cả
        await _orderRepo.UpdateAsync(entity);

        var userName = entity.UserID.HasValue
            ? (await _userRepo.GetByIdAsync(entity.UserID.Value))?.UserName
            : null;
        variantName ??= entity.DollVariantID.HasValue
            ? (await _variantRepo.GetByIdAsync(entity.DollVariantID.Value))?.Name
            : null;

        return MapToDto(entity, userName, variantName);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        await _orderRepo.DeleteAsync(id);
        return true;
    }

    public async Task<bool> CancelOrderAsync(int id)
    {
        var order = await _orderRepo.GetByIdAsync(id);
        if (order == null)
            return false;

        if (order.Status == OrderStatus.Completed || order.Status == OrderStatus.Shipping)
            throw new Exception("Cannot cancel an order that has been shipped or completed.");

        order.Status = OrderStatus.Pending;
        await _orderRepo.UpdateAsync(order);

        return true;
    }

    private static OrderDto MapToDto(Order order, string? userName, string? variantName) => new()
    {
        OrderID = order.OrderID,
        UserID = order.UserID,
        UserName = userName,
        PaymentID = order.PaymentID,
        OrderDate = order.OrderDate,
        TotalAmount = order.TotalAmount,
        ShippingAddress = order.ShippingAddress,
        Status = order.Status,
        DollVariantID = order.DollVariantID,
        DollVariantName = variantName
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



