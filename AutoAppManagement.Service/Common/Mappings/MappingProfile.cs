using AutoMapper;
using AutoAppManagement.Models.BaseEntity;
using AutoAppManagement.Models.DTO.Account;
using AutoAppManagement.Models.DTO.AdminAccount;
using AutoAppManagement.Models.DTO.License;
using AutoAppManagement.Models.DTO.Role;
using AutoAppManagement.Models.DTO.Notification;
using AutoAppManagement.Models.DTO.Permission;
using AutoAppManagement.Models.DTO.Feature;
using AutoAppManagement.Models.DTO.AIConfig;
// Removed: using AutoAppManagement.Models.DTO.ToolFeature;

namespace AutoAppManagement.Service.Common.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Account mappings with custom conversions
            CreateMap<Account, AccountDTO>()
                .ForMember(d => d.Gender, opt => opt.MapFrom(s => s.Gender.ToString()))
                ;

            CreateMap<AccountDTO, Account>();
            // Commented out deprecated mappings - DTOs may not exist
            // CreateMap<AccountRegister, Account>();
            // CreateMap<Account, AccountDetailDTO>();
            // CreateMap<Account, AccountGenericVM>();
            CreateMap<AIConfig, AIConfigDTO>().ReverseMap();

            // Role mappings
            CreateMap<Role, RoleDTO>().ReverseMap();

            // RoleAccount mappings - commented out as DTO may not exist
            // CreateMap<RoleAccount, RoleAccountDTO>();

            // AdminAccount mappings
            CreateMap<AdminAccount, AdminAccountDTO>().ReverseMap();

            // Notification mappings
            CreateMap<Notification, NotificationDTO>().ReverseMap();

            // Permission mappings
            CreateMap<Permission, PermissionDTO>().ReverseMap();

            // License mappings
            CreateMap<License, LicenseDTO>().ReverseMap();

            // AccountDevice mappings - commented out as DTO may not exist  
            // CreateMap<AccountDevice, AccountDeviceDTO>();

            // NEW: Simple Feature Management mappings
            CreateMap<Feature, FeatureDTO>().ReverseMap();
                
            CreateMap<Feature, FeatureListResponse>();
            CreateMap<Feature, FeatureDetailResponse>();
        }
    }
}
