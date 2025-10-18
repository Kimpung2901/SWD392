using AutoMapper;
using BLL.DTO.OwnedDollDTO;
using BLL.DTO.UserCharacterDTO;
using BLL.DTO.CharacterOrderDTO;
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

            // ✅ CharacterOrder mappings
            CreateMap<CharacterOrder, CharacterOrderDto>()
                .ForMember(dest => dest.PackageName,
                    opt => opt.MapFrom(src => src.Package != null ? src.Package.Name : string.Empty))
                .ForMember(dest => dest.CharacterName,
                    opt => opt.MapFrom(src => src.Character != null ? src.Character.Name : string.Empty));

            CreateMap<CreateCharacterOrderDto, CharacterOrder>()
                .ForMember(dest => dest.CharacterOrderID, opt => opt.Ignore())
                .ForMember(dest => dest.Start_Date, opt => opt.MapFrom(src => src.Start_Date ?? DateTime.UtcNow))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Package, opt => opt.Ignore())
                .ForMember(dest => dest.Character, opt => opt.Ignore())
                .ForMember(dest => dest.UserCharacter, opt => opt.Ignore());

            CreateMap<UpdateCharacterOrderDto, CharacterOrder>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
