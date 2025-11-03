using AutoAppManagement.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AutoAppManagement.WebApp.Controllers.Base
{
    public class BaseController : Controller
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

        #region Protected Methods
        
        /// <summary>
        /// Get current user ID from claims
        /// </summary>
        /// <returns></returns>
        protected long? GetCurrentUserId()
        {
            var userIdClaim = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? 
                             User?.FindFirst("UserId")?.Value;
            
            if (long.TryParse(userIdClaim, out long userId))
                return userId;
                
            return 1; // Default for testing
        }
        
        #endregion

        #region Private Method

        #endregion
    }
}
