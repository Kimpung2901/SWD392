using DAL.IRepo;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly DollDbContext _db;

        public PaymentRepository(DollDbContext db) => _db = db;

        public async Task<Payment?> GetByIdAsync(int id)
        {
            return await _db.Payments.FindAsync(id);
        }

        public async Task<Payment?> GetByTransactionIdAsync(string transactionId)
        {
            return await _db.Payments
                .FirstOrDefaultAsync(p => p.TransactionId == transactionId);
        }

        public async Task AddAsync(Payment payment)
        {
            _db.Payments.Add(payment);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(Payment payment)
        {
            if (_db.Entry(payment).State == EntityState.Detached)
            {
                _db.Payments.Attach(payment);
            }
            _db.Entry(payment).State = EntityState.Modified;
            await _db.SaveChangesAsync();
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _db.SaveChangesAsync() > 0;
        }
    }
}