using System.Linq.Expressions;

namespace BLL.Helper;

/// <summary>
/// Extension methods cho IQueryable để hỗ trợ dynamic sorting
/// </summary>
public static class QueryableExtensions
{
    /// <summary>
    /// Apply dynamic sorting cho IQueryable
    /// </summary>
    public static IQueryable<T> ApplySort<T>(
        this IQueryable<T> query, 
        string? sortBy, 
        string? sortDir)
    {
        if (string.IsNullOrWhiteSpace(sortBy))
            return query; // Không sort nếu không có sortBy

        var isDescending = string.Equals(sortDir, "desc", StringComparison.OrdinalIgnoreCase);

        // ✅ Tìm property theo tên (case-insensitive)
        var propertyInfo = typeof(T).GetProperty(
            sortBy, 
            System.Reflection.BindingFlags.IgnoreCase | 
            System.Reflection.BindingFlags.Public | 
            System.Reflection.BindingFlags.Instance);

        if (propertyInfo == null)
            return query; // Property không tồn tại, giữ nguyên

        // ✅ Build expression: x => x.PropertyName
        var parameter = Expression.Parameter(typeof(T), "x");
        var property = Expression.Property(parameter, propertyInfo);
        var lambda = Expression.Lambda(property, parameter);

        // ✅ Call OrderBy hoặc OrderByDescending
        var methodName = isDescending ? "OrderByDescending" : "OrderBy";
        var resultExpression = Expression.Call(
            typeof(Queryable),
            methodName,
            new Type[] { typeof(T), propertyInfo.PropertyType },
            query.Expression,
            Expression.Quote(lambda));

        return query.Provider.CreateQuery<T>(resultExpression);
    }

    /// <summary>
    /// Apply pagination cho IQueryable
    /// </summary>
    public static IQueryable<T> ApplyPagination<T>(
        this IQueryable<T> query, 
        int page, 
        int pageSize)
    {
        // ✅ Validate parameters
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;
        if (pageSize > 100) pageSize = 100; // Max 100 items per page

        return query
            .Skip((page - 1) * pageSize)
            .Take(pageSize);
    }
}