using System.Globalization;
using BLL.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebNameProjectOfSWD.Controllers;

[ApiController]
[Route("api/statics")]
[Authorize(Policy = "AdminOrManager")]
public class StaticsController : ControllerBase
{
    private static readonly string[] SupportedSearchFormats =
    {
        "M-yyyy", "MM-yyyy",
        "yyyy-M", "yyyy-MM",
        "M/yyyy", "MM/yyyy",
        "yyyy/M", "yyyy/MM"
    };

    private readonly IStatisticService _service;
    private readonly ILogger<StaticsController> _logger;

    public StaticsController(
        IStatisticService service,
        ILogger<StaticsController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpGet("revenue")]
    public async Task<IActionResult> GetMonthlyRevenue([FromQuery] string? search, [FromQuery] int? month, [FromQuery] int? year)
    {
        if (!TryResolveMonthYear(search, month, year, out var resolvedMonth, out var resolvedYear, out var error))
            return BadRequest(new { message = error });

        _logger.LogInformation("Fetching revenue statistic for {Month}/{Year}", resolvedMonth, resolvedYear);
        var result = await _service.GetMonthlyRevenueAsync(resolvedMonth, resolvedYear);
        return Ok(result);
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetMonthlyUsers([FromQuery] string? search, [FromQuery] int? month, [FromQuery] int? year)
    {
        if (!TryResolveMonthYear(search, month, year, out var resolvedMonth, out var resolvedYear, out var error))
            return BadRequest(new { message = error });

        _logger.LogInformation("Fetching user statistic for {Month}/{Year}", resolvedMonth, resolvedYear);
        var result = await _service.GetMonthlyUserCountAsync(resolvedMonth, resolvedYear);
        return Ok(result);
    }

    [HttpGet("orders")]
    public async Task<IActionResult> GetMonthlyOrders([FromQuery] string? search, [FromQuery] int? month, [FromQuery] int? year)
    {
        if (!TryResolveMonthYear(search, month, year, out var resolvedMonth, out var resolvedYear, out var error))
            return BadRequest(new { message = error });

        _logger.LogInformation("Fetching order statistic for {Month}/{Year}", resolvedMonth, resolvedYear);
        var result = await _service.GetMonthlyOrderCountAsync(resolvedMonth, resolvedYear);
        return Ok(result);
    }

    private static bool TryResolveMonthYear(
        string? search,
        int? month,
        int? year,
        out int resolvedMonth,
        out int resolvedYear,
        out string? errorMessage)
    {
        resolvedMonth = default;
        resolvedYear = default;
        errorMessage = null;

        if (month.HasValue && year.HasValue)
        {
            if (!IsValidMonthYear(month.Value, year.Value, out errorMessage))
                return false;

            resolvedMonth = month.Value;
            resolvedYear = year.Value;
            return true;
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            if (DateTime.TryParseExact(search.Trim(),
                    SupportedSearchFormats,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var parsed))
            {
                if (!IsValidMonthYear(parsed.Month, parsed.Year, out errorMessage))
                    return false;

                resolvedMonth = parsed.Month;
                resolvedYear = parsed.Year;
                return true;
            }

            errorMessage = $"Unable to parse search value '{search}'. Supported formats: {string.Join(", ", SupportedSearchFormats)}.";
            return false;
        }

        errorMessage = "Please provide either month/year query parameters or a search string formatted as month-year.";
        return false;
    }

    private static bool IsValidMonthYear(int month, int year, out string? errorMessage)
    {
        errorMessage = null;

        if (month is < 1 or > 12)
        {
            errorMessage = "Month must be between 1 and 12.";
            return false;
        }

        if (year < 1)
        {
            errorMessage = "Year must be greater than zero.";
            return false;
        }

        return true;
    }
}
