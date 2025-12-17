using AutoAppManagement.Models.BaseEntity;
using AutoAppManagement.Models.Common;
using AutoAppManagement.Models.DTO;
using AutoAppManagement.Models.DTO.Permission;
using AutoAppManagement.Models.DTO.Role;
using AutoAppManagement.Models.Enum;
using AutoAppManagement.Repository.Repositories;
using AutoAppManagement.Repository.Repositories.Base;
using AutoAppManagement.Service.Services.Base;
using System.Linq.Expressions;

namespace AutoAppManagement.Service.Services
{
    public interface IRoleService : IBaseBusinessService<RoleDTO>
    {
        Task<List<RoleDTO>> GetRolesByAccountId(long accountId);
        Task<BaseResponse> AssignRoleToAccount(AssignRoleRequest request);
        Task<BaseResponse> RemoveRoleFromAccount(long accountId, long roleId);
        Task<BaseResponse> GetWithPermissions(long id);
    }

    public class RoleService : BaseBusinessService<Role, RoleDTO, IRoleRepository>, IRoleService
    {
        // Additional repositories for related entities
        private readonly IBaseRepository<RoleAccount> _roleAccountRepository;
        private readonly IBaseRepository<Account> _accountRepository;
        private readonly IBaseRepository<RolePermission> _rolePermissionRepository;

        public RoleService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _roleAccountRepository = UnitOfWork.GetBaseRepository<RoleAccount>();
            _accountRepository = UnitOfWork.GetBaseRepository<Account>();
            _rolePermissionRepository = UnitOfWork.GetBaseRepository<RolePermission>();
        }

        /// <summary>
        /// Override SubmitData để xử lý cả Role và RolePermission trong cùng transaction
        /// </summary>
        public override async Task<BaseResponse> SubmitData(RoleDTO dto)
        {
            await CustomBeforeSubmitData(dto);
            
            try
            {
                long roleId = 0;
                
                switch (dto.State)
                {
                    case Models.Common.EntityState.Add:
                        // Tạo mới Role
                        var entityToCreate = Mapper.Map<Role>(dto);
                        entityToCreate.SetCreated(GetCurrentUserId());
                        await Repository.CreateAsync(entityToCreate);
                        await UnitOfWork.SaveAsync();
                        
                        // Lấy ID của Role vừa tạo
                        var createdRole = await Repository.FirstOrDefault(r => 
                            r.Name == dto.Name && r.Status == Models.Enum.StatusEnum.Active);
                        if (createdRole != null)
                        {
                            roleId = createdRole.ID;
                        }
                        break;

                    case Models.Common.EntityState.Edit:
                        // Cập nhật Role
                        var entityToUpdate = await Repository.FirstOrDefault(e => e.ID == dto.ID);
                        if (entityToUpdate == null)
                        {
                            return BaseResponse.Error("Vai trò không tồn tại.");
                        }
                        
                        // Map properties
                        entityToUpdate.Name = dto.Name;
                        entityToUpdate.Description = dto.Description;
                        entityToUpdate.Status = dto.Status;
                        entityToUpdate.SetUpdated(GetCurrentUserId());
                        await UnitOfWork.SaveAsync();
                        
                        roleId = dto.ID;
                        break;

                    case Models.Common.EntityState.Remove:
                        var entityToDelete = await Repository.FirstOrDefault(e => e.ID == dto.ID && e.Status == Models.Enum.StatusEnum.Active);
                        if (entityToDelete == null)
                        {
                            return BaseResponse.Error("Vai trò không tồn tại.");
                        }
                        entityToDelete.SetDeleted(GetCurrentUserId());
                        await UnitOfWork.SaveAsync();
                        return BaseResponse.Success("Xóa thành công.");

                    default:
                        return BaseResponse.Error("Trạng thái không hợp lệ.");
                }
                
                // Xử lý RolePermission nếu có PermissionIds
                if (roleId > 0 && dto.PermissionIds != null)
                {
                    // Xóa tất cả RolePermission hiện tại
                    var existingRolePermissions = await _rolePermissionRepository.GetByCondition(
                        rp => rp.RoleId == roleId && rp.Status == Models.Enum.StatusEnum.Active);

                    foreach (var rp in existingRolePermissions)
                    {
                        _rolePermissionRepository.Delete(rp);
                    }

                    // Tạo mới các RolePermission
                    var currentUserId = GetCurrentUserId();
                    foreach (var permissionId in dto.PermissionIds.Distinct())
                    {
                        var rolePermission = new RolePermission
                        {
                            RoleId = roleId,
                            PermissionId = permissionId
                        };
                        rolePermission.SetCreated(currentUserId);
                        await _rolePermissionRepository.CreateAsync(rolePermission);
                    }

                    // Save tất cả thay đổi RolePermission
                    await UnitOfWork.SaveAsync();
                }
                
                // Trả về DTO đơn giản để tránh circular reference
                if (dto.State == Models.Common.EntityState.Add && roleId > 0)
                {
                    return BaseResponse.Success(new RoleDTO
                    {
                        ID = roleId,
                        Name = dto.Name,
                        Description = dto.Description,
                        Status = dto.Status,
                        PermissionIds = dto.PermissionIds
                    }, "Lưu thành công");
                }
                
                return BaseResponse.Success("Lưu thành công");
            }
            catch (Exception ex)
            {
                return BaseResponse.Error($"Lỗi: {ex.Message}");
            }
        }

        /// <summary>
        /// Validate Role trước khi submit
        /// </summary>
        public override async Task CustomBeforeSubmitData(RoleDTO dto)
        {
            // Không cần validate Code nữa
            await base.CustomBeforeSubmitData(dto);
        }

        public async Task<List<RoleDTO>> GetRolesByAccountId(long accountId)
        {
            var roleAccounts = await _roleAccountRepository.GetByCondition(ra => ra.AccountID == accountId && ra.Status == StatusEnum.Active);
            var roleIds = roleAccounts.Select(ra => ra.RoleID);
            var roles = await Repository.GetByCondition(r => roleIds.Contains(r.ID) && r.Status == Models.Enum.StatusEnum.Active);
            return Mapper.Map<List<RoleDTO>>(roles);
        }

        public async Task<BaseResponse> AssignRoleToAccount(AssignRoleRequest request)
        {
            try
            {
                 var account = await _accountRepository.FirstOrDefault(a => a.ID == request.AccountId && a.Status == Models.Enum.StatusEnum.Active);
                if (account == null) return BaseResponse.Error("Account không tồn tại");

                var role = await Repository.FirstOrDefault(r => r.ID == request.RoleId && r.Status == Models.Enum.StatusEnum.Active);
                if (role == null) return BaseResponse.Error("Role không tồn tại");

                var existingAssignment = await _roleAccountRepository.FirstOrDefault(ra => ra.AccountID == request.AccountId && ra.RoleID == request.RoleId && ra.Status == StatusEnum.Active);
                if (existingAssignment != null) return BaseResponse.Error("Account đã có role này");

                var roleAccount = new RoleAccount
                {
                    AccountID = request.AccountId,
                    RoleID = request.RoleId,
                };
                roleAccount.SetCreated(GetCurrentUserId());

                await _roleAccountRepository.CreateAsync(roleAccount);
                await UnitOfWork.SaveAsync();
                return BaseResponse.Success("Gán role thành công");
            }
            catch (Exception ex)
            {
                return BaseResponse.Error($"Lỗi khi gán role: {ex.Message}");
            }
        }

        public async Task<BaseResponse> RemoveRoleFromAccount(long accountId, long roleId)
        {
            try
            {
                var roleAccount = await _roleAccountRepository.FirstOrDefault(ra => ra.AccountID == accountId && ra.RoleID == roleId && ra.Status == StatusEnum.Active);
                if (roleAccount == null) return BaseResponse.Error("Role assignment không tồn tại");

                _roleAccountRepository.Delete(roleAccount);
                await UnitOfWork.SaveAsync();
                return BaseResponse.Success("Gỡ role thành công");
            }
            catch (Exception ex)
            {
                return BaseResponse.Error($"Lỗi khi gỡ role: {ex.Message}");
            }
        }

        public async Task<BaseResponse> GetWithPermissions(long id)
        {
            try
            {
                var role = await Repository.FirstOrDefault(r => r.ID == id && r.Status == Models.Enum.StatusEnum.Active);
                if (role == null)
                {
                    return BaseResponse.Error("Role không tồn tại");
                }

                var roleDto = Mapper.Map<RoleDTO>(role);

                // Lấy danh sách Permission của Role
                var rolePermissions = await UnitOfWork.GetBaseRepository<RolePermission>()
                    .GetByCondition(rp => rp.RoleId == id && rp.Status == Models.Enum.StatusEnum.Active);

                var permissionIds = rolePermissions.Select(rp => rp.PermissionId).ToList();
                var permissions = new List<Permission>();
                
                if (permissionIds.Any())
                {
                    var permissionRepo = UnitOfWork.GetBaseRepository<Permission>();
                    var permissionList = await permissionRepo.GetByCondition(p => 
                        permissionIds.Contains(p.ID) && 
                        p.Status == Models.Enum.StatusEnum.Active);
                    permissions = permissionList.ToList();
                }

                // Map permissions sang DTO
                var permissionDtos = Mapper.Map<List<PermissionDTO>>(permissions);

                // Tạo response object
                var response = new
                {
                    id = roleDto.ID,
                    name = roleDto.Name,
                    description = roleDto.Description,
                    status = roleDto.Status,
                    createdDate = roleDto.CreatedDate,
                    updatedDate = roleDto.UpdatedDate,
                    createdBy = roleDto.CreatedBy,
                    updatedBy = roleDto.UpdatedBy,
                    permissions = permissionDtos.Select(p => new
                    {
                        id = p.ID,
                        name = p.Name,
                        resource = p.Resource,
                        description = p.Description,
                        category = p.Category,
                        status = p.Status
                    }).ToList(),
                    permissionIds = permissionIds
                };

                return BaseResponse.Success(response, "Lấy thông tin Role kèm Permissions thành công");
            }
            catch (Exception ex)
            {
                return BaseResponse.Error($"Lỗi khi lấy thông tin Role: {ex.Message}");
            }
        }
    }
}
