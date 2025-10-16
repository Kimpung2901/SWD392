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

        public async Task<List<CharacterOrderDto>> GetAllAsync()
        {
            var orders = await _repo.GetAllAsync();
            return orders.Select(Map).ToList();
        }

        public async Task<CharacterOrderDto?> GetByIdAsync(int id)
        {
            var order = await _repo.GetByIdAsync(id);
            return order == null ? null : Map(order);
        }

        public async Task<List<CharacterOrderDto>> GetByCharacterIdAsync(int characterId)
        {
            var orders = await _repo.GetByCharacterIdAsync(characterId);
            return orders.Select(Map).ToList();
        }

        public async Task<List<CharacterOrderDto>> GetByPackageIdAsync(int packageId)
        {
            var orders = await _repo.GetByPackageIdAsync(packageId);
            return orders.Select(Map).ToList();
        }

        public async Task<List<CharacterOrderDto>> GetByUserCharacterIdAsync(int userCharacterId)
        {
            var orders = await _repo.GetByUserCharacterIdAsync(userCharacterId);
            return orders.Select(Map).ToList();
        }

        public async Task<CharacterOrderDto> CreateAsync(CreateCharacterOrderDto dto)
        {
            var entity = new CharacterOrder
            {
                PackageID = dto.PackageID,
                CharacterID = dto.CharacterID,
                UserCharacterID = dto.UserCharacterID,
                UnitPrice = dto.UnitPrice,
                QuantityMonths = dto.QuantityMonths, 
                Start_Date = dto.Start_Date,
                End_Date = dto.End_Date,
                Status = "Active",
                CreatedAt = DateTime.UtcNow
            };

            await _repo.AddAsync(entity);
            return Map(entity);
        }

        public async Task<CharacterOrderDto?> UpdatePartialAsync(int id, UpdateCharacterOrderDto dto)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return null;

            if (dto.PackageID.HasValue)
                entity.PackageID = dto.PackageID.Value;

            if (dto.CharacterID.HasValue)
                entity.CharacterID = dto.CharacterID.Value;

            if (dto.UserCharacterID.HasValue)
                entity.UserCharacterID = dto.UserCharacterID.Value;

            if (dto.UnitPrice.HasValue)
                entity.UnitPrice = dto.UnitPrice.Value;

            if (dto.QuantityMonths.HasValue) 
                entity.QuantityMonths = dto.QuantityMonths.Value;

            if (dto.Start_Date.HasValue)
                entity.Start_Date = dto.Start_Date.Value;

            if (dto.End_Date.HasValue)
                entity.End_Date = dto.End_Date.Value;

            if (!string.IsNullOrWhiteSpace(dto.Status))
                entity.Status = dto.Status.Trim();

            await _repo.UpdateAsync(entity);
            return Map(entity);
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            return await _repo.SoftDeleteAsync(id);
        }

        public async Task<bool> HardDeleteAsync(int id)
        {
            return await _repo.HardDeleteAsync(id);
        }

        private static CharacterOrderDto Map(CharacterOrder o) => new()
        {
            CharacterOrderID = o.CharacterOrderID,
            PackageID = o.PackageID,
            CharacterID = o.CharacterID,
            UserCharacterID = o.UserCharacterID,
            UnitPrice = o.UnitPrice,
            QuantityMonths = o.QuantityMonths, 
            Start_Date = o.Start_Date,
            End_Date = o.End_Date,
            Status = o.Status,
            CreatedAt = o.CreatedAt
        };
    }
}