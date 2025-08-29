using AutoAppManagement.Models.BaseEntity;
using AutoAppManagement.WebApp.Services;
using AutoMapper;

namespace AutoAppManagement.WebApp.Common.Mappings
{
    public class WebAppMappingProfile : Profile
    {
        public WebAppMappingProfile()
        {
            // Role mappings
            CreateMap<Role, RoleViewModel>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.RoleName))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.RoleDescription))
                .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.RoleName.ToLower().Replace(" ", "_")))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.UserCount, opt => opt.MapFrom(src => src.RoleAccounts != null ? src.RoleAccounts.Count : 0))
                .ForMember(dest => dest.Permissions, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => "System"))
                .ForMember(dest => dest.UpdatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore());
                
            CreateMap<CreateRoleViewModel, Role>()
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.RoleDescription, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.RoleAccounts, opt => opt.Ignore());
                
            CreateMap<UpdateRoleViewModel, Role>()
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.RoleDescription, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.RoleAccounts, opt => opt.Ignore());
        }
    }
}
