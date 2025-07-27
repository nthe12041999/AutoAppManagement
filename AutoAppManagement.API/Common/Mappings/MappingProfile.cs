using AutoAppManagement.Models.BaseEntity;
using AutoAppManagement.Models.ViewModel.Account;
using AutoMapper;

namespace AutoAppManagement.API.Common.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<AccountRegister, Account>();
            CreateMap<Account, AccountGenericVM>();
        }
    }
}
