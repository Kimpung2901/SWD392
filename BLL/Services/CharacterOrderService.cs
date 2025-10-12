using BLL.DTO.CharacterOrderDTO;
using BLL.IService;
using DAL.IRepo;
using DAL.Models;

namespace BLL.Services
{
    public class CharacterOrderService : ICharacterOrderService
    {
        private readonly ICharacterOrderRepository _repo;

        public CharacterOrderService(ICharacterOrderRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<CharacterOrderRequest>> GetAllAsync()
        {
            var orders = await _repo.GetAllAsync();
            return orders.Select(Map).ToList();
        }

        public async Task<CharacterOrderRequest?> GetByIdAsync(int id)
        {
            var order = await _repo.GetByIdAsync(id);
            return order == null ? null : Map(order);
        }

        public async Task<CharacterOrderRequest> CreateAsync(CharacterOrderRequest dto)
        {
            var entity = new CharacterOrder
            {
                PackageID = dto.PackageID,
                CharacterID = dto.CharacterID,
                UserCharacterID = dto.UserCharacterID,
                UnitPrice = dto.UnitPrice,
                Start_Date = dto.Start_Date,
                End_Date = dto.End_Date,
                CreatedAt = DateTime.UtcNow,
                Status = dto.Status
            };

            var created = await _repo.CreateAsync(entity);
            return Map(created);
        }

        public async Task<CharacterOrderRequest?> UpdateAsync(int id, CharacterOrderRequest dto)
        {
            var entity = new CharacterOrder
            {
                PackageID = dto.PackageID,
                CharacterID = dto.CharacterID,
                UserCharacterID = dto.UserCharacterID,
                UnitPrice = dto.UnitPrice,
                Start_Date = dto.Start_Date,
                End_Date = dto.End_Date,
                Status = dto.Status
            };

            var updated = await _repo.UpdateAsync(id, entity);
            return updated == null ? null : Map(updated);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repo.DeleteAsync(id);
        }

        private static CharacterOrderRequest Map(CharacterOrder o) => new()
        {
            CharacterOrderID = o.CharacterOrderID,
            PackageID = o.PackageID,
            CharacterID = o.CharacterID,
            UserCharacterID = o.UserCharacterID,
            UnitPrice = o.UnitPrice,
            Start_Date = o.Start_Date,
            End_Date = o.End_Date,
            CreatedAt = o.CreatedAt,
            Status = o.Status
        };
    }
}