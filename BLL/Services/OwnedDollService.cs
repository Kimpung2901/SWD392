using AutoMapper;
using BLL.DTO.OwnedDollDTO;
using BLL.IService;
using DAL.IRepo;
using DAL.Models;
using DAL.Enum;
using Microsoft.EntityFrameworkCore;
using BLL.Helper;

namespace BLL.Services
{
    public class OwnedDollService : IOwnedDollService
    {
        private readonly IOwnedDollRepository _repo;
        private readonly DollDbContext _db;
        private readonly IMapper _mapper;

        public OwnedDollService(IOwnedDollRepository repo, DollDbContext db, IMapper mapper)
        {
            _repo = repo;
            _db = db;
            _mapper = mapper;
        }

        public async Task<List<OwnedDollDto>> GetAllAsync()
        {
            var ownedDolls = await _db.OwnedDolls
                .Include(o => o.DollVariant)
                .AsNoTracking()
                .ToListAsync();

            return _mapper.Map<List<OwnedDollDto>>(ownedDolls);
        }

        public async Task<OwnedDollDto?> GetByIdAsync(int id)
        {
            var ownedDoll = await _db.OwnedDolls
                .Include(o => o.DollVariant)
                .FirstOrDefaultAsync(o => o.OwnedDollID == id);

            return ownedDoll == null ? null : _mapper.Map<OwnedDollDto>(ownedDoll);
        }

        public async Task<List<OwnedDollDto>> GetByUserIdAsync(int userId)
        {
            var ownedDolls = await _db.OwnedDolls
                .Include(o => o.DollVariant)
                .Where(o => o.UserID == userId)
                .OrderByDescending(o => o.Acquired_at)
                .AsNoTracking()
                .ToListAsync();

            return _mapper.Map<List<OwnedDollDto>>(ownedDolls);
        }

        public async Task<List<OwnedDollDto>> GetByDollVariantIdAsync(int dollVariantId)
        {
            var ownedDolls = await _db.OwnedDolls
                .Include(o => o.DollVariant)
                .Where(o => o.DollVariantID == dollVariantId)
                .OrderByDescending(o => o.Acquired_at)
                .AsNoTracking()
                .ToListAsync();

            return _mapper.Map<List<OwnedDollDto>>(ownedDolls);
        }

        public async Task<OwnedDollDto?> GetBySerialCodeAsync(string serialCode)
        {
            var ownedDoll = await _db.OwnedDolls
                .Include(o => o.DollVariant)
                .FirstOrDefaultAsync(o => o.SerialCode == serialCode);

            return ownedDoll == null ? null : _mapper.Map<OwnedDollDto>(ownedDoll);
        }

        public async Task<OwnedDollDto> CreateAsync(CreateOwnedDollDto dto)
        {
            var existing = await _repo.GetBySerialCodeAsync(dto.SerialCode);
            if (existing != null)
                throw new InvalidOperationException($"SerialCode '{dto.SerialCode}' đã tồn tại");

            var vietnamNow = DateTimeHelper.GetVietnamTime();  

            var entity = new OwnedDoll
            {
                UserID = dto.UserID,
                DollVariantID = dto.DollVariantID,
                SerialCode = dto.SerialCode,
                Status = OwnedDollStatus.Active,
                Acquired_at = dto.Acquired_at ?? vietnamNow, 
                Expired_at = dto.Expired_at ?? vietnamNow.AddYears(1)  
            };

            await _repo.AddAsync(entity);
            return await GetByIdAsync(entity.OwnedDollID) 
                ?? throw new Exception("Không thể tạo OwnedDoll");
        }

        public async Task<OwnedDollDto?> UpdatePartialAsync(int id, UpdateOwnedDollDto dto)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return null;

            if (!string.IsNullOrWhiteSpace(dto.SerialCode))
            {
                var existing = await _repo.GetBySerialCodeAsync(dto.SerialCode);
                if (existing != null && existing.OwnedDollID != id)
                    throw new InvalidOperationException($"SerialCode '{dto.SerialCode}' đã được sử dụng");

                entity.SerialCode = dto.SerialCode;
            }

            if (dto.Status.HasValue) 
                entity.Status = dto.Status.Value;

            if (dto.Expired_at.HasValue)
                entity.Expired_at = dto.Expired_at.Value;

            await _repo.UpdateAsync(entity);
            return await GetByIdAsync(entity.OwnedDollID);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            await _repo.DeleteAsync(id);
            return true;
        }
    }
}