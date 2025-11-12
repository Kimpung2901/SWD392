using AutoMapper;
using AutoMapper.QueryableExtensions;
using BLL.DTO.CharacterOrderDTO;
using BLL.IService;
using DAL.IRepo;
using DAL.Models;
using DAL.Enum;
using Microsoft.EntityFrameworkCore;
using BLL.Helper;

namespace BLL.Services
{
    public class CharacterOrderService : ICharacterOrderService
    {
        private readonly ICharacterOrderRepository _repo;
        private readonly ICharacterRepository _characterRepo;
        private readonly ICharacterPackageRepository _packageRepo;
        private readonly IUserCharacterRepository _userCharacterRepo;
        private readonly DollDbContext _db;
        private readonly IMapper _mapper;

        public CharacterOrderService(
            ICharacterOrderRepository repo,
            ICharacterRepository characterRepo,
            ICharacterPackageRepository packageRepo,
            IUserCharacterRepository userCharacterRepo,
            DollDbContext db,
            IMapper mapper)
        {
            _repo = repo;
            _characterRepo = characterRepo;
            _packageRepo = packageRepo;
            _userCharacterRepo = userCharacterRepo;
            _db = db;
            _mapper = mapper;
        }

        public async Task<List<CharacterOrderDto>> GetAllAsync()
        {
            return await _db.CharacterOrders
                .Include(co => co.Package)
                .Include(co => co.Character)
                .OrderByDescending(co => co.CreatedAt)
                .AsNoTracking()
                .ProjectTo<CharacterOrderDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<CharacterOrderDto?> GetByIdAsync(int id)
        {
            return await _db.CharacterOrders
                .Include(co => co.Package)
                .Include(co => co.Character)
                .Where(co => co.CharacterOrderID == id)
                .AsNoTracking()
                .ProjectTo<CharacterOrderDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }

        public async Task<List<CharacterOrderDto>> GetByUserIdAsync(int userId)
        {
            return await _db.CharacterOrders
                .Include(co => co.Package)
                .Include(co => co.Character)
                .Where(co => co.UserID == userId)
                .OrderByDescending(co => co.CreatedAt)
                .AsNoTracking()
                .ProjectTo<CharacterOrderDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<List<CharacterOrderDto>> GetByUserCharacterIdAsync(int userCharacterId)
        {
            return await _db.CharacterOrders
                .Include(co => co.Package)
                .Include(co => co.Character)
                .OrderByDescending(co => co.CreatedAt)
                .AsNoTracking()
                .ProjectTo<CharacterOrderDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<List<CharacterOrderDto>> GetByCharacterIdAsync(int characterId)
        {
            return await _db.CharacterOrders
                .Include(co => co.Package)
                .Where(co => co.CharacterID == characterId)
                .OrderByDescending(co => co.CreatedAt)
                .AsNoTracking()
                .ProjectTo<CharacterOrderDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<List<CharacterOrderDto>> GetByPackageIdAsync(int packageId)
        {
            return await _db.CharacterOrders
                .Include(co => co.Character)
                .Where(co => co.PackageID == packageId)
                .OrderByDescending(co => co.CreatedAt)
                .AsNoTracking()
                .ProjectTo<CharacterOrderDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<List<CharacterOrderDto>> GetPendingOrdersAsync()
        {
            return await _db.CharacterOrders
                .Include(co => co.Package)
                .Include(co => co.Character)
                .Where(co => co.Status == CharacterOrderStatus.Pending)
                .OrderBy(co => co.CreatedAt)
                .AsNoTracking()
                .ProjectTo<CharacterOrderDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<CharacterOrderDto> CreateAsync(CreateCharacterOrderDto dto, int userId)
        {
            var character = await _characterRepo.GetByIdAsync(dto.CharacterID);
            if (character == null)
                throw new InvalidOperationException($"Character với ID {dto.CharacterID} không tồn tại");

            var package = await _packageRepo.GetByIdAsync(dto.PackageID);
            if (package == null)
                throw new InvalidOperationException($"Package với ID {dto.PackageID} không tồn tại");

            if (package.CharacterId != dto.CharacterID)
                throw new InvalidOperationException("Package không thuộc về Character đã chọn");

            var vietnamNow = DateTimeHelper.GetVietnamTime();

            var entity = new CharacterOrder
            {
                UserID = userId,
                PackageID = dto.PackageID,
                CharacterID = dto.CharacterID,
                UnitPrice = package.Price,
                Status = CharacterOrderStatus.Pending,
                CreatedAt = vietnamNow
            };

            await _repo.AddAsync(entity);
            
            return await GetByIdAsync(entity.CharacterOrderID) 
                ?? throw new Exception("Không thể tạo CharacterOrder");
        }

        public async Task<CharacterOrderDto?> UpdatePartialAsync(int id, UpdateCharacterOrderDto dto)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return null;

            if (dto.Status.HasValue)
                entity.Status = dto.Status.Value;

            await _repo.UpdateAsync(entity);
            return await GetByIdAsync(entity.CharacterOrderID);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            await _repo.DeleteAsync(id);
            return true;
        }

        public async Task<bool> CompleteOrderAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return false;

            if (entity.Status == CharacterOrderStatus.Completed) 
                throw new InvalidOperationException("Order đã được hoàn thành");

            entity.Status = CharacterOrderStatus.Completed; 
            await _repo.UpdateAsync(entity);
            return true;
        }

        public async Task<bool> CancelOrderAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return false;

            if (entity.Status == CharacterOrderStatus.Completed)
                throw new InvalidOperationException("Không thể hủy order đã hoàn thành");

            if (entity.Status != CharacterOrderStatus.Pending)
            {
                entity.Status = CharacterOrderStatus.Pending; 
                await _repo.UpdateAsync(entity);
            }

            return true;
        }
    }
}
