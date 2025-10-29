namespace BLL.DTO.StatisticDto;

public class MonthlyCountDto
{
    public int Month { get; set; }
    public int Year { get; set; }
    public int TotalCount { get; set; }
    public string Metric { get; set; } = string.Empty;
}
