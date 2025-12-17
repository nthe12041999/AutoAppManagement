using AutoAppManagement.API.Controllers.Base;
using AutoAppManagement.Models.BaseEntity;
using AutoAppManagement.Models.DTO.License;
using AutoAppManagement.Service.Services;

namespace AutoAppManagement.API.Controllers
{
    public class LicenseController : BaseBusinessController<ILicenseService, License, LicenseDTO>
    {
        public LicenseController(IServiceProvider serviceProvider) : base(serviceProvider) { }
    }
}
