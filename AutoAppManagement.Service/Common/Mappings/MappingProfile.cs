using AutoAppManagement.Models.BaseEntity;
using AutoAppManagement.Models.ViewModel.Account;
using AutoMapper;

namespace AutoAppManagement.Service.Common.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<AccountRegister, Account>();
        }
    }
}
