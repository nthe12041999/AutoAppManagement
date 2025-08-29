using AutoAppManagement.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace AutoAppManagement.API.Controllers.Base
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        protected readonly IServiceProvider _serviceProvider;

        private IRestOutput? _res;
        protected IRestOutput ResOutput
            => _res ??= _serviceProvider.GetRequiredService<IRestOutput>();

        private IHttpContextAccessor? _httpContextAccessor;
        private IHttpContextAccessor HttpContextAccessor
            => _httpContextAccessor ??= _serviceProvider.GetRequiredService<IHttpContextAccessor>();
        public BaseController(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        #region Protected Method

        #endregion
    }
}
