using DAL.Models;

namespace DAL.IRepo;

public interface IPaymentRepository
{
    Task<Payment?> GetByIdAsync(int id);
    Task AddAsync(Payment payment);
    Task UpdateAsync(Payment payment);
}
