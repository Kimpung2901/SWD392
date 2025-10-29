using DAL.IRepo;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    public class OrderItemRepository : IOrderItemRepository
    {
        private readonly DollDbContext _db;
        private readonly IUnitOfWork _unitOfWork;

        public OrderItemRepository(DollDbContext db, IUnitOfWork unitOfWork)
        {
            _db = db;
            _unitOfWork = unitOfWork;
        }

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
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task AddRangeAsync(List<OrderItem> entities)
        {
            _db.OrderItems.AddRange(entities);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateAsync(OrderItem entity)
        {
            if (_db.Entry(entity).State == EntityState.Detached)
            {
                _db.OrderItems.Attach(entity);
            }

            _db.Entry(entity).State = EntityState.Modified;
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _db.OrderItems.FindAsync(id);
            if (entity != null)
            {
                _db.OrderItems.Remove(entity);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _unitOfWork.SaveChangesAsync() > 0;
        }
    }
}
