using System.Linq;
using BLL.DTO.StatisticDto;
using BLL.IService;
using DAL.Enum;
using DAL.IRepo;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services;

public class StatisticService : IStatisticService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUserRepository _userRepository;

    public StatisticService(
        IOrderRepository orderRepository,
        IUserRepository userRepository)
    {
        _orderRepository = orderRepository;
        _userRepository = userRepository;
    }

    public async Task<MonthlyRevenueDto> GetMonthlyRevenueAsync(int month, int year)
    {
        var query = _orderRepository
            .GetQueryable()
            .Where(o => o.OrderDate.Month == month && o.OrderDate.Year == year)
            .Where(o => o.Status == OrderStatus.Completed || o.Status == OrderStatus.Shipped);

        var totalRevenue = await query.SumAsync(o => o.TotalAmount);

        return new MonthlyRevenueDto
        {
            Month = month,
            Year = year,
            TotalRevenue = totalRevenue
        };
    }

    public async Task<MonthlyCountDto> GetMonthlyUserCountAsync(int month, int year)
    {
        var totalUsers = await _userRepository
            .GetQueryable()
            .Where(u => !u.IsDeleted && u.CreatedAt.Month == month && u.CreatedAt.Year == year)
            .CountAsync();

        return new MonthlyCountDto
        {
            Month = month,
            Year = year,
            TotalCount = totalUsers,
            Metric = "users"
        };
    }

    public async Task<MonthlyCountDto> GetMonthlyOrderCountAsync(int month, int year)
    {
        var totalOrders = await _orderRepository
            .GetQueryable()
            .Where(o => o.OrderDate.Month == month && o.OrderDate.Year == year)
            .CountAsync();

        return new MonthlyCountDto
        {
            Month = month,
            Year = year,
            TotalCount = totalOrders,
            Metric = "orders"
        };
    }
}
