using AutoMapper;
using AutoAppManagement.Models.BaseEntity;
using AutoAppManagement.Models.DTO.Account;
using AutoAppManagement.Models.DTO.AdminAccount;
using AutoAppManagement.Models.DTO.License;
using AutoAppManagement.Models.DTO.Role;
using AutoAppManagement.Models.DTO.Notification;
using AutoAppManagement.Models.DTO.Permission;
using AutoAppManagement.Models.DTO.Feature;
// Removed: using AutoAppManagement.Models.DTO.ToolFeature;

namespace AutoAppManagement.Service.Common.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Account mappings
            CreateMap<Account, AccountDTO>().ReverseMap();
            // Commented out deprecated mappings - DTOs may not exist
            // CreateMap<AccountRegister, Account>();
            // CreateMap<Account, AccountDetailDTO>();
            // CreateMap<Account, AccountGenericVM>();

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
            CreateMap<LicenseUser, LicenseUserDTO>().ReverseMap();
            CreateMap<FeatureUsageTracking, FeatureUsageTrackingDTO>().ReverseMap();
            
            // Feature request/response mappings
            CreateMap<CreateFeatureRequest, Feature>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore());
                
            CreateMap<UpdateFeatureRequest, Feature>()
                .ForMember(dest => dest.Code, opt => opt.Ignore()) // Code không được update
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore());
                
            CreateMap<Feature, FeatureListResponse>();
            CreateMap<Feature, FeatureDetailResponse>();
        }
    }

    // Simple DTOs for new entities (if not exists)
    public class LicenseUserDTO : LicenseUser, AutoAppManagement.Models.Common.IStatefulDTO
    {
        public AutoAppManagement.Models.Common.EntityState State { get; set; }
    }

    public class FeatureUsageTrackingDTO : FeatureUsageTracking, AutoAppManagement.Models.Common.IStatefulDTO
    {
        public AutoAppManagement.Models.Common.EntityState State { get; set; }
    }
}
