using DAL.IRepo;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    public class OrderItemRepository : IOrderItemRepository
    {
        private readonly DollDbContext _db;

        public OrderItemRepository(DollDbContext db) => _db = db;

        public async Task<List<OrderItem>> GetAllAsync()
        {
            return await _db.OrderItems
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<OrderItem?> GetByIdAsync(int id)
        {
            return await _db.OrderItems
                .FirstOrDefaultAsync(oi => oi.OrderItemID == id);
        }

        public async Task<List<OrderItem>> GetByOrderIdAsync(int orderId)
        {
            return await _db.OrderItems
                .Where(oi => oi.OrderID == orderId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task AddAsync(OrderItem entity)
        {
            _db.OrderItems.Add(entity);
            await _db.SaveChangesAsync();
        }

        public async Task AddRangeAsync(List<OrderItem> entities)
        {
            _db.OrderItems.AddRange(entities);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(OrderItem entity)
        {
            if (_db.Entry(entity).State == EntityState.Detached)
            {
                _db.OrderItems.Attach(entity);
            }

            _db.Entry(entity).State = EntityState.Modified;
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _db.OrderItems.FindAsync(id);
            if (entity != null)
            {
                _db.OrderItems.Remove(entity);
                await _db.SaveChangesAsync();
            }
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _db.SaveChangesAsync() > 0;
        }
    }
}