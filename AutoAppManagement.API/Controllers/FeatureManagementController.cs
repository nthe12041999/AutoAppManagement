using AutoAppManagement.API.Controllers.Base;
using AutoAppManagement.Models.BaseEntity;
using AutoAppManagement.Models.DTO.Feature;
using AutoAppManagement.Service.Services;

namespace AutoAppManagement.API.Controllers
{
    /// <summary>
    /// Simple Feature Management API Controller
    /// </summary>
    public class FeatureManagementController : BaseBusinessController<IFeatureManagementService, Feature, FeatureDTO>
    {
        public FeatureManagementController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
