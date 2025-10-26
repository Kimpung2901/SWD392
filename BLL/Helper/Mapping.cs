using AutoMapper;
using BLL.DTO.OwnedDollDTO;
using BLL.DTO.UserCharacterDTO;
using BLL.DTO.CharacterOrderDTO;
using BLL.DTO.DollCharacterLinkDTO;
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
        }
    }
}
