using DAL.Models;

namespace BLL.IService;

public interface IPaymentService
{
    Task<Payment> StartAsync(string provider, decimal amount, string targetType, int targetId, int? orderId, int? characterOrderId);
    Task<bool> HandleIpnAsync(string provider, IDictionary<string, string> query);
}
