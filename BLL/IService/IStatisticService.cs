using BLL.DTO.StatisticDto;

namespace BLL.IService;

public interface IStatisticService
{
    Task<MonthlyRevenueDto> GetMonthlyRevenueAsync(int month, int year);
    Task<MonthlyCountDto> GetMonthlyUserCountAsync(int month, int year);
    Task<MonthlyCountDto> GetMonthlyOrderCountAsync(int month, int year);
}
