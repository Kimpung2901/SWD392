namespace BLL.DTO.Common;

/// <summary>
/// Generic class cho kết quả phân trang
/// </summary>
/// <typeparam name="T">Kiểu dữ liệu của items</typeparam>
public class PagedResult<T>
{
    /// <summary>
    /// Danh sách items trong trang hiện tại
    /// </summary>
    public List<T> Items { get; set; } = new();

    /// <summary>
    /// Tổng số records trong database
    /// </summary>
    public int Total { get; set; }

    /// <summary>
    /// Trang hiện tại (bắt đầu từ 1)
    /// </summary>
    public int Page { get; set; }

    /// <summary>
    /// Số lượng items mỗi trang
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Tổng số trang
    /// </summary>
    public int TotalPages => (int)Math.Ceiling(Total / (double)PageSize);

    /// <summary>
    /// Có trang trước không?
    /// </summary>
    public bool HasPreviousPage => Page > 1;

    /// <summary>
    /// Có trang sau không?
    /// </summary>
    public bool HasNextPage => Page < TotalPages;
}