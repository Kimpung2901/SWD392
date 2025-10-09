using DAL.Models;

namespace DAL.IRepo
{
    public interface IPaymentRepository
    {
        Task<Payment?> GetByIdAsync(int id);
        Task<Payment?> GetByTransactionIdAsync(string transactionId);
        Task AddAsync(Payment payment);
        Task UpdateAsync(Payment payment);
        Task<bool> SaveChangesAsync();
    }
}