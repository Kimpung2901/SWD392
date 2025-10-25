using BLL.DTO.OrderDTO;
using BLL.IService;
using DAL.IRepo;

namespace BLL.Services
{
    public class OrderItemService : IOrderItemService
    {
        private readonly IOrderItemRepository _repo;
        private readonly IDollVariantRepository _variantRepo;

        public OrderItemService(IOrderItemRepository repo, IDollVariantRepository variantRepo)
        {
            _repo = repo;
            _variantRepo = variantRepo;
        }

        public async Task<List<OrderItemDto>> GetAllAsync()
        {
            var items = await _repo.GetAllAsync();
            var dtos = new List<OrderItemDto>();

            foreach (var item in items)
            {
                var variant = await _variantRepo.GetByIdAsync(item.DollVariantID);
                dtos.Add(MapToDto(item, variant?.Name));
            }

            return dtos;
        }

        public async Task<OrderItemDto?> GetByIdAsync(int id)
        {
            var item = await _repo.GetByIdAsync(id);
            if (item == null) return null;

            var variant = await _variantRepo.GetByIdAsync(item.DollVariantID);
            return MapToDto(item, variant?.Name);
        }

        public async Task<List<OrderItemDto>> GetByOrderIdAsync(int orderId)
        {
            var items = await _repo.GetByOrderIdAsync(orderId);
            var dtos = new List<OrderItemDto>();

            foreach (var item in items)
            {
                var variant = await _variantRepo.GetByIdAsync(item.DollVariantID);
                dtos.Add(MapToDto(item, variant?.Name));
            }

            return dtos;
        }

        public async Task<OrderItemDto?> UpdatePartialAsync(int id, UpdateOrderItemDto dto)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return null;

            if (dto.Quantity.HasValue && dto.Quantity.Value > 0)
            {
                entity.Quantity = dto.Quantity.Value;
                entity.LineTotal = entity.UnitPrice * entity.Quantity;
            }

            if (dto.Status.HasValue)
                entity.Status = dto.Status.Value;

            await _repo.UpdateAsync(entity);

            var variant = await _variantRepo.GetByIdAsync(entity.DollVariantID);
            return MapToDto(entity, variant?.Name);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            await _repo.DeleteAsync(id);
            return true;
        }

        private static OrderItemDto MapToDto(DAL.Models.OrderItem oi, string? variantName) => new()
        {
            OrderItemID = oi.OrderItemID,
            OrderID = oi.OrderID,
            DollVariantID = oi.DollVariantID,
            DollVariantName = variantName,
            Quantity = oi.Quantity,
            UnitPrice = oi.UnitPrice,
            LineTotal = oi.LineTotal,
            Status = oi.Status
        };
    }
}