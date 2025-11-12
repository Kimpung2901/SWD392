using DAL.Models;

namespace BLL.IService;

public interface IOwnedDollManager
{
    /// <summary>
    /// Ensures that an OwnedDoll is provisioned for the given order.
    /// Returns true when a new record was added.
    /// </summary>
    /// <param name="order">The order for which an owned doll may need to be created.</param>
    /// <param name="contextTag">An optional tag used for logging/tracing.</param>
    Task<bool> EnsureOwnedDollForOrderAsync(Order order, string? contextTag = null);
}
