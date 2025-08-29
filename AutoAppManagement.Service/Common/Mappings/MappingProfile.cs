using AutoAppManagement.Models.BaseEntity;
using AutoAppManagement.Models.ViewModel.Account;
using AutoAppManagement.Models.DTO.Account;
using AutoAppManagement.Models.DTO.Role;
using AutoAppManagement.Models.DTO.RoleAccount;
using AutoAppManagement.Models.DTO.Notification;
using AutoAppManagement.Models.DTO.License;
using AutoAppManagement.Models.DTO.AdminAccount;
using AutoAppManagement.Models.DTO.AccountDevice;
using AutoMapper;

namespace AutoAppManagement.Service.Common.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Account mappings
            CreateMap<AccountRegister, Account>();
            CreateMap<Account, AccountDTO>();
            CreateMap<Account, AccountGenericVM>();
            CreateMap<AccountRegister, Account>();
            
            // Role mappings
            CreateMap<Role, RoleDTO>();
            
            // RoleAccount mappings
            CreateMap<RoleAccount, RoleAccountDTO>();
            
            // Notification mappings
            CreateMap<Notification, NotificationDTO>();
            
            // License mappings
            CreateMap<License, LicenseDTO>();
            
            // AdminAccount mappings
            CreateMap<AdminAccount, AdminAccountDTO>();
            
            // AccountDevice mappings
            CreateMap<AccountDevice, AccountDeviceDTO>();
        }
    }
}
