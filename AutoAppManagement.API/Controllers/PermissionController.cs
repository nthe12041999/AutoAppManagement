using AutoAppManagement.API.Controllers.Base;
using AutoAppManagement.API.Common.Attribute;
using AutoAppManagement.Models.BaseEntity;
using AutoAppManagement.Models.Constant;
using AutoAppManagement.Models.DTO.Permission;
using AutoAppManagement.Models.Common;
using AutoAppManagement.Models.Enum;
using AutoAppManagement.Service.Services;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

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
        [RequirePermission(PermissionCodes.PERMISSION_VIEW)]
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

        /// <summary>
        /// Lấy danh sách Permission với phân trang
        /// </summary>
        [HttpGet("GetPaging")]
        [RequirePermission(PermissionCodes.PERMISSION_VIEW)]
        public async Task<IActionResult> GetPaging(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? keyword = null,
            [FromQuery] string? sortBy = null,
            [FromQuery] bool sortDescending = false)
        {
            try
            {
                var allPermissions = await Service.GetAllPermissions();
                
                // Filter by keyword
                var filteredPermissions = allPermissions.AsQueryable();
                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    var lowerKeyword = keyword.ToLower();
                    filteredPermissions = filteredPermissions.Where(p => 
                        (p.Name != null && p.Name.ToLower().Contains(lowerKeyword)) ||
                        (p.Category != null && p.Category.ToLower().Contains(lowerKeyword)) ||
                        (p.Resource != null && p.Resource.ToLower().Contains(lowerKeyword))
                    );
                }

                // Total items before pagination
                var totalItems = filteredPermissions.Count();

                // Sorting
                if (!string.IsNullOrWhiteSpace(sortBy))
                {
                    filteredPermissions = sortBy.ToLower() switch
                    {
                        "name" => sortDescending 
                            ? filteredPermissions.OrderByDescending(p => p.Name)
                            : filteredPermissions.OrderBy(p => p.Name),
                        "category" => sortDescending
                            ? filteredPermissions.OrderByDescending(p => p.Category)
                            : filteredPermissions.OrderBy(p => p.Category),
                        _ => filteredPermissions.OrderBy(p => p.Category).ThenBy(p => p.Name)
                    };
                }
                else
                {
                    filteredPermissions = filteredPermissions.OrderBy(p => p.Category ?? "ZZZ").ThenBy(p => p.Name);
                }

                // Pagination
                var pagedPermissions = filteredPermissions
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var permissionDtos = _mapper.Map<List<PermissionDTO>>(pagedPermissions);

                var result = new
                {
                    Data = permissionDtos,
                    PageIndex = pageNumber,
                    PageSize = pageSize,
                    TotalItems = totalItems,
                    TotalPages = (int)Math.Ceiling((double)totalItems / pageSize)
                };

                ResOutput.SuccessEventHandler(result);
                return Ok(ResOutput);
            }
            catch (Exception ex)
            {
                ResOutput.ErrorEventHandler($"Lỗi khi lấy danh sách Permission: {ex.Message}");
                return BadRequest(ResOutput);
            }
        }

        /// <summary>
        /// Lấy danh sách Permission grouped theo Category với phân trang
        /// </summary>
        [HttpGet("GetGroupedByCategory")]
        [RequirePermission(PermissionCodes.PERMISSION_VIEW)]
        public async Task<IActionResult> GetGroupedByCategory(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? keyword = null,
            [FromQuery] string? sortBy = null,
            [FromQuery] bool sortDescending = false)
        {
            try
            {
                var allPermissions = await Service.GetAllPermissions();
                
                // Filter by keyword
                var filteredPermissions = allPermissions.AsQueryable();
                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    var lowerKeyword = keyword.ToLower();
                    filteredPermissions = filteredPermissions.Where(p => 
                        (p.Name != null && p.Name.ToLower().Contains(lowerKeyword)) ||
                        (p.Category != null && p.Category.ToLower().Contains(lowerKeyword))
                    );
                }

                // Group theo Category
                var grouped = filteredPermissions
                    .OrderBy(p => p.Category ?? "ZZZ")
                    .ThenBy(p => p.Name)
                    .GroupBy(p => p.Category ?? "Không phân loại")
                    .Select(g => new
                    {
                        Category = g.Key,
                        Name = string.Join(", ", g.Select(p => p.Name))
                    })
                    .ToList();

                // Total items before pagination
                var totalItems = grouped.Count;

                // Sorting
                var sortedGroups = grouped.AsQueryable();
                if (!string.IsNullOrWhiteSpace(sortBy))
                {
                    sortedGroups = sortBy.ToLower() switch
                    {
                        "category" => sortDescending 
                            ? sortedGroups.OrderByDescending(p => p.Category)
                            : sortedGroups.OrderBy(p => p.Category),
                        "name" => sortDescending
                            ? sortedGroups.OrderByDescending(p => p.Name)
                            : sortedGroups.OrderBy(p => p.Name),
                        _ => sortedGroups.OrderBy(p => p.Category)
                    };
                }

                // Pagination
                var pagedGroups = sortedGroups
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var result = new
                {
                    Data = pagedGroups,
                    PageIndex = pageNumber,
                    PageSize = pageSize,
                    TotalItems = totalItems,
                    TotalPages = (int)Math.Ceiling((double)totalItems / pageSize)
                };
                
                ResOutput.SuccessEventHandler(result);
                return Ok(ResOutput);
            }
            catch (Exception ex)
            {
                ResOutput.ErrorEventHandler($"Lỗi khi lấy Permission: {ex.Message}");
                return BadRequest(ResOutput);
            }
        }

        /// <summary>
        /// Khởi tạo các Permission mặc định (Customer, License, Staff, Role, Permission)
        /// </summary>
        [HttpPost("InitializeDefaultPermissions")]
        [Roles(RoleConstant.Admin)]
        [RequirePermission(PermissionCodes.PERMISSION_CREATE)]
        public async Task<IActionResult> InitializeDefaultPermissions()
        {
            try
            {
                var result = await Service.InitializeDefaultPermissions();
                if (result.IsSuccess)
                {
                    ResOutput.SuccessEventHandler(result.Data, result.Message);
                    return Ok(ResOutput);
                }
                else
                {
                    ResOutput.ErrorEventHandler(result.Message);
                    return BadRequest(ResOutput);
                }
            }
            catch (Exception ex)
            {
                ResOutput.ErrorEventHandler($"Lỗi khi khởi tạo Permissions: {ex.Message}");
                return BadRequest(ResOutput);
            }
        }

        /// <summary>
        /// Submit danh sách Permission (Create/Update nhiều Permission cùng lúc)
        /// </summary>
        [HttpPost("SubmitDataList")]
        [RequirePermission(PermissionCodes.PERMISSION_CREATE, PermissionCodes.PERMISSION_EDIT)]
        public async Task<IActionResult> SubmitDataList([FromBody] List<PermissionDTO> permissionDtos)
        {
            try
            {
                if (permissionDtos == null || !permissionDtos.Any())
                {
                    ResOutput.ErrorEventHandler("Danh sách Permission không được để trống");
                    return BadRequest(ResOutput);
                }

                var results = new List<object>();
                var errors = new List<string>();

                foreach (var dto in permissionDtos)
                {
                    try
                    {
                        // Submit từng permission qua SubmitData endpoint logic
                        var submitResult = await Service.SubmitData(dto);
                        
                        if (submitResult.IsSuccess)
                        {
                            results.Add(new { dto.Name, Status = "Success", Data = submitResult.Data });
                        }
                        else
                        {
                            errors.Add($"{dto.Name}: {submitResult.Message}");
                        }
                    }
                    catch (Exception innerEx)
                    {
                        errors.Add($"{dto.Name}: {innerEx.Message}");
                    }
                }

                if (errors.Any())
                {
                    ResOutput.ErrorEventHandler($"Hoàn thành với {errors.Count} lỗi", new { 
                        SuccessCount = results.Count,
                        ErrorCount = errors.Count,
                        Errors = errors,
                        SuccessResults = results
                    });
                }
                else
                {
                    ResOutput.SuccessEventHandler(results, $"Đã xử lý thành công {results.Count} Permission");
                }

                return Ok(ResOutput);
            }
            catch (Exception ex)
            {
                ResOutput.ErrorEventHandler($"Lỗi khi submit danh sách Permission: {ex.Message}");
                return BadRequest(ResOutput);
            }
        }
    }
}
