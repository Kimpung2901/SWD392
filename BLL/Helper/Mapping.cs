using AutoMapper;
using BLL.DTO.CharacterDTO;
using BLL.DTO.CharacterOrderDTO;
using BLL.DTO.DollCharacterLinkDTO;
using BLL.DTO.DollModelDTO;
using BLL.DTO.DollTypeDTO;
using BLL.DTO.DollVariantDTO;
using BLL.DTO.OwnedDollDTO;
using BLL.DTO.UserCharacterDTO;
using DAL.Models;

namespace BLL.Helper
{
    public class Mapping : Profile
    {
        public Mapping() 
        {
            // ✅ OwnedDoll mappings
            CreateMap<OwnedDoll, OwnedDollDto>()
                .ForMember(dest => dest.DollVariantName, 
                    opt => opt.MapFrom(src => src.DollVariant != null ? src.DollVariant.Name : string.Empty));

            CreateMap<CreateOwnedDollDto, OwnedDoll>()
                .ForMember(dest => dest.OwnedDollID, opt => opt.Ignore())
                .ForMember(dest => dest.Acquired_at, opt => opt.MapFrom(src => src.Acquired_at ?? DateTime.UtcNow))
                .ForMember(dest => dest.Expired_at, opt => opt.MapFrom(src => src.Expired_at ?? DateTime.UtcNow.AddYears(1)))
                .ForMember(dest => dest.DollVariant, opt => opt.Ignore());

            // ✅ UserCharacter mappings
            CreateMap<UserCharacter, UserCharacterDto>()
                .ForMember(dest => dest.UserName, 
                    opt => opt.MapFrom(src => src.User != null ? src.User.UserName : string.Empty))
                .ForMember(dest => dest.CharacterName, 
                    opt => opt.MapFrom(src => src.Character != null ? src.Character.Name : string.Empty))
                .ForMember(dest => dest.PackageName, 
                    opt => opt.MapFrom(src => src.Package != null ? src.Package.Name : string.Empty));

            CreateMap<CreateUserCharacterDto, UserCharacter>()
                .ForMember(dest => dest.UserCharacterID, opt => opt.Ignore())
                .ForMember(dest => dest.StartAt, opt => opt.MapFrom(src => src.StartAt ?? DateTime.UtcNow))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.Character, opt => opt.Ignore())
                .ForMember(dest => dest.Package, opt => opt.Ignore());

            // ✅ CharacterOrder mappings - Enum được map tự động
            CreateMap<CharacterOrder, CharacterOrderDto>()
                .ForMember(dest => dest.PackageName,
                    opt => opt.MapFrom(src => src.Package != null ? src.Package.Name : string.Empty))
                .ForMember(dest => dest.CharacterName,
                    opt => opt.MapFrom(src => src.Character != null ? src.Character.Name : string.Empty));

            CreateMap<CreateCharacterOrderDto, CharacterOrder>()
                .ForMember(dest => dest.CharacterOrderID, opt => opt.Ignore())
                .ForMember(dest => dest.UnitPrice, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Package, opt => opt.Ignore())
                .ForMember(dest => dest.Character, opt => opt.Ignore());


            CreateMap<UpdateCharacterOrderDto, CharacterOrder>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // ✅ DollCharacterLink mappings
            CreateMap<CreateDollCharacterLinkDto, DollCharacterLink>()
                .ForMember(dest => dest.LinkID, opt => opt.Ignore())
                .ForMember(dest => dest.BoundAt, opt => opt.MapFrom(src => src.BoundAt ?? DateTime.UtcNow))
                .ForMember(dest => dest.UnBoundAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.Note, opt => opt.MapFrom(src => src.Note ?? string.Empty));

            CreateMap<UpdateDollCharacterLinkDto, DollCharacterLink>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // DollVariant mappings
            CreateMap<DollVariant, DollVariantDto>()
                .ForMember(dest => dest.DollModelName,
                    opt => opt.MapFrom(src => src.DollModel != null ? src.DollModel.Name : string.Empty));

            CreateMap<CreateDollVariantDto, DollVariant>()
                .ForMember(dest => dest.DollVariantID, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(_ => true));

            CreateMap<UpdateDollVariantDto, DollVariant>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // DollModel mappings
            CreateMap<DollModel, DollModelDto>()
                .ForMember(dest => dest.DollTypeName,
                    opt => opt.MapFrom(src => src.DollType != null ? src.DollType.Name : string.Empty));

            CreateMap<CreateDollModelDto, DollModel>()
                .ForMember(dest => dest.DollModelID, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(_ => false));

            CreateMap<UpdateDollModelDto, DollModel>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // DollType mappings
            CreateMap<DollType, DollTypeDto>();
            CreateMap<CreateDollTypeDto, DollType>()
                .ForMember(dest => dest.DollTypeID, opt => opt.Ignore())
                .ForMember(dest => dest.Create_at, opt => opt.MapFrom(_ => DateTime.UtcNow));
            CreateMap<UpdateDollTypeDto, DollType>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // Character mappings
            CreateMap<Character, CharacterDto>();
            CreateMap<CreateCharacterDto, Character>()
                .ForMember(dest => dest.CharacterId, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(_ => true))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));
            CreateMap<UpdateCharacterDto, Character>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
