using System.Reflection;

namespace WebNameProjectOfSWD.Extensions;

public static class QueryExtensions
{
    public static List<T> ApplyQuery<T>(this IEnumerable<T> source, string? search, string? sortBy, string? sortDirection)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));

        IEnumerable<T> query = source;

        var normalizedSearch = string.IsNullOrWhiteSpace(search)
            ? null
            : search.Trim();

        if (!string.IsNullOrEmpty(normalizedSearch))
        {
            var stringProperties = typeof(T)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.PropertyType == typeof(string));

            query = query.Where(item =>
                stringProperties.Any(prop =>
                {
                    var value = prop.GetValue(item) as string;
                    return !string.IsNullOrEmpty(value) &&
                           value.Contains(normalizedSearch, StringComparison.OrdinalIgnoreCase);
                }));
        }

        if (!string.IsNullOrWhiteSpace(sortBy))
        {
            var property = typeof(T)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .FirstOrDefault(p => string.Equals(p.Name, sortBy, StringComparison.OrdinalIgnoreCase));

            if (property != null)
            {
                query = string.Equals(sortDirection, "desc", StringComparison.OrdinalIgnoreCase)
                    ? query.OrderByDescending(item => property.GetValue(item))
                    : query.OrderBy(item => property.GetValue(item));
            }
        }

        return query.ToList();
    }
}
