using AutoAppManagement.API.Controllers.Base;
using AutoAppManagement.API.Common.Attribute;
using AutoAppManagement.Models.BaseEntity;
using AutoAppManagement.Models.Constant;
using AutoAppManagement.Models.DTO.Permission;
using AutoAppManagement.Service.Services;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AutoAppManagement.API.Controllers
{
    public class PermissionController : BaseBusinessController<IPermissionService, Permission, PermissionDTO>
    {
        private readonly IMapper _mapper;

        public PermissionController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _mapper = serviceProvider.GetRequiredService<IMapper>();
        }

        /// <summary>
        /// Lấy Permission theo Category
        /// </summary>
        [HttpGet("GetByCategory")]
        [Roles(RoleConstant.Admin)]
        public async Task<IActionResult> GetByCategory([FromQuery] string category)
        {
            try
            {
                var permissions = await Service.GetPermissionsByCategory(category);
                var permissionDtos = _mapper.Map<List<PermissionDTO>>(permissions);
                ResOutput.SuccessEventHandler(permissionDtos);
                return Ok(ResOutput);
            }
            catch (Exception ex)
            {
                ResOutput.ErrorEventHandler($"Lỗi khi lấy Permission theo Category: {ex.Message}");
                return BadRequest(ResOutput);
            }
        }
    }
}
