using AutoAppManagement.Models.BaseEntity;
using AutoAppManagement.Models.Common;
using AutoAppManagement.Models.DTO.Permission;
using AutoAppManagement.Models.Enum;
using AutoAppManagement.Repository.Repositories;
using AutoAppManagement.Repository.Repositories.Base;
using AutoAppManagement.Service.Services.Base;
using AutoMapper;
using static AutoAppManagement.Models.Enum.DataModelType;

namespace AutoAppManagement.Service.Services
{
    public interface IPermissionService : IBaseBusinessService<PermissionDTO>
    {
        // Permission Management
        Task<BaseResponse> CreatePermission(string resource, string action, string? displayName = null, string? description = null, string? category = null);
        Task<BaseResponse> UpdatePermission(long permissionId, string? displayName = null, string? description = null, string? category = null);
        Task<BaseResponse> DeletePermission(long permissionId);
        Task<List<Permission>> GetAllPermissions();
        Task<List<Permission>> GetPermissionsByCategory(string category);
        Task<List<Permission>> GetPermissionsByResource(string resource);
        Task<Permission?> GetPermissionByCode(string code);
        
        // Role Permission Management
        Task<BaseResponse> AssignPermissionToRole(long roleId, long permissionId, string scope = "own", int priority = 0);
        Task<BaseResponse> RemovePermissionFromRole(long roleId, long permissionId);
        Task<BaseResponse> UpdateRolePermission(long roleId, long permissionId, string? newScope = null, int? newPriority = null);
        Task<List<Permission>> GetRolePermissions(long roleId);
        Task<List<RolePermission>> GetRolePermissionsWithScope(long roleId);
        Task<BaseResponse> BulkAssignPermissionsToRole(long roleId, List<long> permissionIds, string defaultScope = "own");
        Task<BaseResponse> SyncRolePermissions(long roleId, List<(long permissionId, string scope)> permissions);
        
        // Account Permission Checking
        Task<bool> CheckAccountHasPermission(long accountId, string resource, string action, string scope = "own");
        Task<bool> CheckAccountHasPermissionCode(long accountId, string permissionCode, string scope = "own");
        Task<List<Permission>> GetAccountPermissions(long accountId);
        Task<List<RolePermission>> GetAccountPermissionsWithScope(long accountId);
        Task<List<string>> GetAccountPermissionCodes(long accountId);
        
        // Role Account Management  
        Task<BaseResponse> AssignRoleToAccount(long accountId, long roleId);
        Task<BaseResponse> RemoveRoleFromAccount(long accountId, long roleId);
        Task<List<Role>> GetAccountRoles(long accountId);
        Task<List<Account>> GetRoleAccounts(long roleId);
        Task<BaseResponse> SyncAccountRoles(long accountId, List<long> roleIds);
        Task<List<Permission>> GetPermissionsUserCanGrant(long accountId);
        
        // Utility Methods
        Task<BaseResponse> InitializeDefaultPermissions();
        Task<List<Permission>> SearchPermissions(string searchTerm);
        Task<Dictionary<string, List<Permission>>> GetPermissionsByCategory();
        
        // Composite Operations
        Task<BaseResponse> CreateRoleWithPermissions(string roleName, string roleDescription, List<(string resource, string action, string scope)> permissions);
        Task<BaseResponse> CreateRoleWithDefaultPermissions(string roleName, string roleDescription, string roleType = "user");
        Task<BaseResponse> CreateRoleAndAssignToAccount(long accountId, string roleName, string roleDescription, List<(string resource, string action, string scope)> permissions);
        Task<BaseResponse> CreateAccountWithRole(string email, string fullName, string roleName, string roleType = "user");
    }

    public class PermissionService : BaseBusinessService<Permission, PermissionDTO, IPermissionRepository>, IPermissionService
    {
        private readonly IPermissionRepository _permissionRepository;
        private readonly IRolePermissionRepository _rolePermissionRepository;
        private readonly IRoleAccountRepository _roleAccountRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IAccountsRepository _accountRepository;

        public PermissionService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _permissionRepository = UnitOfWork.GetRepository<Permission>() as IPermissionRepository ?? throw new InvalidOperationException("Permission repository not found");
            _rolePermissionRepository = UnitOfWork.GetRepository<RolePermission>() as IRolePermissionRepository ?? throw new InvalidOperationException("RolePermission repository not found");
            _roleAccountRepository = UnitOfWork.GetRepository<RoleAccount>() as IRoleAccountRepository ?? throw new InvalidOperationException("RoleAccount repository not found");
            _roleRepository = UnitOfWork.GetRepository<Role>() as IRoleRepository ?? throw new InvalidOperationException("Role repository not found");
            _accountRepository = UnitOfWork.GetRepository<Account>() as IAccountsRepository ?? throw new InvalidOperationException("Account repository not found");
        }

        #region Permission Management

        public async Task<BaseResponse> CreatePermission(string resource, string action, string? displayName = null, string? description = null, string? category = null)
        {
            try
            {
                var code = $"{resource.ToLower()}.{action.ToLower()}";
                
                // Check if permission already exists
                var existingPermission = await Repository.FirstOrDefault(p => p.Code == code || (p.Resource == resource && p.Action.ToString() == action));
                if (existingPermission != null)
                    return BaseResponse.Error("Permission đã tồn tại");

                var permission = new Permission
                {
                    Resource = resource,
                    Action = Enum.Parse<PermissionAction>(action, true),
                    Code = code,
                    Description = description,
                    Category = category ?? resource
                };

                permission.GenerateCode();
                permission.SetCreated(GetCurrentUserId());

                await Insert(permission);
                await UnitOfWork.SaveAsync();

                return BaseResponse.Success(Mapper.Map<PermissionDTO>(permission), "Tạo permission thành công");
            }
            catch (Exception ex)
            {
                return BaseResponse.Error($"Lỗi khi tạo permission: {ex.Message}");
            }
        }

        public async Task<BaseResponse> UpdatePermission(long permissionId, string? displayName = null, string? description = null, string? category = null)
        {
            try
            {
                var permission = await Repository.FirstOrDefault(p => p.ID == permissionId && p.Status == Models.Enum.StatusEnum.Active);
                if (permission == null)
                    return BaseResponse.Error("Permission không tồn tại");

                if (!string.IsNullOrEmpty(description))
                    permission.Description = description;
                if (!string.IsNullOrEmpty(category))
                    permission.Category = category;

                permission.SetUpdated(GetCurrentUserId());
                await UnitOfWork.SaveAsync();

                return BaseResponse.Success(Mapper.Map<PermissionDTO>(permission), "Cập nhật permission thành công");
            }
            catch (Exception ex)
            {
                return BaseResponse.Error($"Lỗi khi cập nhật permission: {ex.Message}");
            }
        }

        public async Task<BaseResponse> DeletePermission(long permissionId)
        {
            try
            {
                var permission = await Repository.FirstOrDefault(p => p.ID == permissionId && p.Status == Models.Enum.StatusEnum.Active);
                if (permission == null)
                    return BaseResponse.Error("Permission không tồn tại");

                // Check if permission is being used
                var usedRolePermissions = await _rolePermissionRepository.GetByCondition(rp => rp.PermissionId == permissionId && rp.Status == Models.Enum.StatusEnum.Active);
                if (usedRolePermissions.Any())
                    return BaseResponse.Error("Không thể xóa permission đang được sử dụng");

                permission.SetDeleted(GetCurrentUserId());
                await UnitOfWork.SaveAsync();

                return BaseResponse.Success("Xóa permission thành công");
            }
            catch (Exception ex)
            {
                return BaseResponse.Error($"Lỗi khi xóa permission: {ex.Message}");
            }
        }

        public async Task<List<Permission>> GetAllPermissions()
        {
            return (await Repository.GetByCondition(p => p.Status == Models.Enum.StatusEnum.Active)).ToList();
        }

        public async Task<List<Permission>> GetPermissionsByCategory(string category)
        {
            return (await Repository.GetByCondition(p => p.Status == Models.Enum.StatusEnum.Active && p.Category == category)).ToList();
        }

        public async Task<List<Permission>> GetPermissionsByResource(string resource)
        {
            return (await Repository.GetByCondition(p => p.Status == Models.Enum.StatusEnum.Active && p.Resource == resource)).ToList();
        }

        public async Task<Permission?> GetPermissionByCode(string code)
        {
            return await Repository.FirstOrDefault(p => p.Code == code && p.Status == Models.Enum.StatusEnum.Active);
        }

        #endregion

        #region Role Permission Management

        public async Task<BaseResponse> AssignPermissionToRole(long roleId, long permissionId, string scope = "own", int priority = 0)
        {
            try
            {
                // Check if role exists
                var role = await _roleRepository.FirstOrDefault(r => r.ID == roleId && r.Status == Models.Enum.StatusEnum.Active);
                if (role == null)
                    return BaseResponse.Error("Role không tồn tại");

                // Check if permission exists
                var permission = await Repository.FirstOrDefault(p => p.ID == permissionId && p.Status == Models.Enum.StatusEnum.Active);
                if (permission == null)
                    return BaseResponse.Error("Permission không tồn tại");

                // Check if already assigned
                var existing = await _rolePermissionRepository.FirstOrDefault(rp => rp.RoleId == roleId && rp.PermissionId == permissionId && rp.Status == Models.Enum.StatusEnum.Active);
                if (existing != null)
                    return BaseResponse.Error("Permission đã được gán cho role này");

                var rolePermission = new RolePermission
                {
                    RoleId = roleId,
                    PermissionId = permissionId,
                    // Priority = priority // TODO: Removed in schema update
                };

                rolePermission.SetCreated(GetCurrentUserId());

                await _rolePermissionRepository.CreateAsync(rolePermission);
                await UnitOfWork.SaveAsync();

                return BaseResponse.Success("Gán permission cho role thành công");
            }
            catch (Exception ex)
            {
                return BaseResponse.Error($"Lỗi khi gán permission: {ex.Message}");
            }
        }

        public async Task<BaseResponse> RemovePermissionFromRole(long roleId, long permissionId)
        {
            try
            {
                var rolePermission = await _rolePermissionRepository.FirstOrDefault(rp => rp.RoleId == roleId && rp.PermissionId == permissionId && rp.Status == Models.Enum.StatusEnum.Active);
                if (rolePermission == null)
                    return BaseResponse.Error("Permission assignment không tồn tại");

                rolePermission.SetDeleted(GetCurrentUserId());
                await UnitOfWork.SaveAsync();

                return BaseResponse.Success("Gỡ permission khỏi role thành công");
            }
            catch (Exception ex)
            {
                return BaseResponse.Error($"Lỗi khi gỡ permission: {ex.Message}");
            }
        }

        public async Task<BaseResponse> UpdateRolePermission(long roleId, long permissionId, string? newScope = null, int? newPriority = null)
        {
            try
            {
                var rolePermission = await _rolePermissionRepository.FirstOrDefault(rp => rp.RoleId == roleId && rp.PermissionId == permissionId && rp.Status == Models.Enum.StatusEnum.Active);
                if (rolePermission == null)
                    return BaseResponse.Error("Permission assignment không tồn tại");

                if (newPriority.HasValue)
                {
                    // rolePermission.Priority = newPriority.Value; // TODO: Removed in schema update
                }

                rolePermission.SetUpdated(GetCurrentUserId());
                await UnitOfWork.SaveAsync();

                return BaseResponse.Success("Cập nhật role permission thành công");
            }
            catch (Exception ex)
            {
                return BaseResponse.Error($"Lỗi khi cập nhật role permission: {ex.Message}");
            }
        }

        public async Task<List<Permission>> GetRolePermissions(long roleId)
        {
            var rolePermissions = await _rolePermissionRepository.GetByCondition(rp => rp.RoleId == roleId && rp.Status == Models.Enum.StatusEnum.Active);
            var permissionIds = rolePermissions.Select(rp => rp.PermissionId);
            return (await Repository.GetByCondition(p => permissionIds.Contains(p.ID) && p.Status == Models.Enum.StatusEnum.Active)).ToList();
        }

        public async Task<List<RolePermission>> GetRolePermissionsWithScope(long roleId)
        {
            return (await _rolePermissionRepository.GetByCondition(rp => rp.RoleId == roleId && rp.Status == Models.Enum.StatusEnum.Active)).ToList();
        }

        public async Task<BaseResponse> BulkAssignPermissionsToRole(long roleId, List<long> permissionIds, string defaultScope = "own")
        {
            try
            {
                var role = await _roleRepository.FirstOrDefault(r => r.ID == roleId && r.Status == Models.Enum.StatusEnum.Active);
                if (role == null)
                    return BaseResponse.Error("Role không tồn tại");

                var newAssignments = new List<RolePermission>();
                foreach (var permissionId in permissionIds)
                {
                    var permission = await Repository.FirstOrDefault(p => p.ID == permissionId && p.Status == Models.Enum.StatusEnum.Active);
                    if (permission == null) continue;

                    var existing = await _rolePermissionRepository.FirstOrDefault(rp => rp.RoleId == roleId && rp.PermissionId == permissionId && rp.Status == Models.Enum.StatusEnum.Active);
                    if (existing != null) continue;

                    newAssignments.Add(new RolePermission
                    {
                        RoleId = roleId,
                        PermissionId = permissionId,
                        CreatedBy = GetCurrentUserId()
                    });
                }

                if (newAssignments.Any())
                {
                    await _rolePermissionRepository.CreateRangeAsync(newAssignments);
                    await UnitOfWork.SaveAsync();
                }

                return BaseResponse.Success($"Đã gán {newAssignments.Count} permission(s) cho role");
            }
            catch (Exception ex)
            {
                return BaseResponse.Error($"Lỗi khi gán permission hàng loạt: {ex.Message}");
            }
        }

        public async Task<BaseResponse> SyncRolePermissions(long roleId, List<(long permissionId, string scope)> permissions)
        {
            try
            {
                var role = await _roleRepository.FirstOrDefault(r => r.ID == roleId && r.Status == Models.Enum.StatusEnum.Active);
                if (role == null)
                    return BaseResponse.Error("Role không tồn tại");

                var existingAssignments = await _rolePermissionRepository.GetByCondition(rp => rp.RoleId == roleId && rp.Status == Models.Enum.StatusEnum.Active);
                var existingPermissionIds = existingAssignments.Select(rp => rp.PermissionId).ToList();

                var newPermissionIds = permissions.Select(p => p.permissionId).ToList();
                var permissionsToAdd = permissions.Where(p => !existingPermissionIds.Contains(p.permissionId)).ToList();
                var permissionsToRemove = existingAssignments.Where(rp => !newPermissionIds.Contains(rp.PermissionId));

                // Remove permissions
                foreach (var assignment in permissionsToRemove)
                {
                    assignment.SetDeleted(GetCurrentUserId());
                }

                // Add new permissions
                var newAssignments = new List<RolePermission>();
                foreach (var (permissionId, scope) in permissionsToAdd)
                {
                    var permission = await Repository.FirstOrDefault(p => p.ID == permissionId && p.Status == Models.Enum.StatusEnum.Active);
                    if (permission == null) continue;

                    newAssignments.Add(new RolePermission
                    {
                        RoleId = roleId,
                        PermissionId = permissionId,
                        CreatedBy = GetCurrentUserId()
                    });
                }

                if (newAssignments.Any())
                {
                    await _rolePermissionRepository.CreateRangeAsync(newAssignments);
                }

                await UnitOfWork.SaveAsync();

                return BaseResponse.Success($"Đồng bộ {newAssignments.Count} permission(s) thêm và {permissionsToRemove.Count()} permission(s) xóa cho role");
            }
            catch (Exception ex)
            {
                return BaseResponse.Error($"Lỗi khi đồng bộ role permissions: {ex.Message}");
            }
        }

        #endregion

        #region Account Permission Checking

        public async Task<bool> CheckAccountHasPermission(long accountId, string resource, string action, string scope = "own")
        {
            var accountPermissions = await GetAccountPermissionsWithScope(accountId);
            return accountPermissions.Any(ap => 
                ap.Permission != null && 
                ap.Permission.Matches(resource, action));
        }

        public async Task<bool> CheckAccountHasPermissionCode(long accountId, string permissionCode, string scope = "own")
        {
            var accountPermissions = await GetAccountPermissionsWithScope(accountId);
            return accountPermissions.Any(ap => 
                ap.Permission != null && 
                ap.Permission.Code == permissionCode);
        }

        public async Task<List<Permission>> GetAccountPermissions(long accountId)
        {
            var accountRoles = await GetAccountRoles(accountId);
            var roleIds = accountRoles.Select(r => r.ID);
            
            var rolePermissions = await _rolePermissionRepository.GetByCondition(rp => 
                roleIds.Contains(rp.RoleId) && rp.Status == Models.Enum.StatusEnum.Active);
            
            var permissionIds = rolePermissions.Select(rp => rp.PermissionId).Distinct();
            return (await Repository.GetByCondition(p => permissionIds.Contains(p.ID) && p.Status == Models.Enum.StatusEnum.Active)).ToList();
        }

        public async Task<List<RolePermission>> GetAccountPermissionsWithScope(long accountId)
        {
            var accountRoles = await GetAccountRoles(accountId);
            var roleIds = accountRoles.Select(r => r.ID);
            
            return (await _rolePermissionRepository.GetByCondition(rp => 
                roleIds.Contains(rp.RoleId) && rp.Status == Models.Enum.StatusEnum.Active)).ToList();
        }

        public async Task<List<string>> GetAccountPermissionCodes(long accountId)
        {
            var permissions = await GetAccountPermissions(accountId);
            return permissions.Select(p => p.Code).ToList();
        }

        #endregion

        #region Role Account Management

        public async Task<BaseResponse> AssignRoleToAccount(long accountId, long roleId)
        {
            try
            {
                var account = await _accountRepository.FirstOrDefault(a => a.ID == accountId && a.Status == Models.Enum.StatusEnum.Active);
                if (account == null)
                    return BaseResponse.Error("Account không tồn tại");

                var role = await _roleRepository.FirstOrDefault(r => r.ID == roleId && r.Status == Models.Enum.StatusEnum.Active);
                if (role == null)
                    return BaseResponse.Error("Role không tồn tại");

                var existing = await _roleAccountRepository.FirstOrDefault(ra => ra.AccountID == accountId && ra.RoleID == roleId && ra.Status == Models.Enum.StatusEnum.Active);
                if (existing != null)
                    return BaseResponse.Error("Account đã có role này");

                var roleAccount = new RoleAccount
                {
                    AccountID = accountId,
                    RoleID = roleId,
                    CreatedBy = GetCurrentUserId()
                };

                await _roleAccountRepository.CreateAsync(roleAccount);
                await UnitOfWork.SaveAsync();

                return BaseResponse.Success("Gán role cho account thành công");
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
                if (roleAccount == null)
                    return BaseResponse.Error("Role assignment không tồn tại");

                roleAccount.SetDeleted(GetCurrentUserId());
                await UnitOfWork.SaveAsync();

                return BaseResponse.Success("Gỡ role khỏi account thành công");
            }
            catch (Exception ex)
            {
                return BaseResponse.Error($"Lỗi khi gỡ role: {ex.Message}");
            }
        }

        public async Task<List<Role>> GetAccountRoles(long accountId)
        {
            var roleAccounts = await _roleAccountRepository.GetByCondition(ra => ra.AccountID == accountId && ra.Status == StatusEnum.Active);
            var roleIds = roleAccounts.Select(ra => ra.RoleID);
            return (await _roleRepository.GetByCondition(r => roleIds.Contains(r.ID) && r.Status == Models.Enum.StatusEnum.Active)).ToList();
        }

        public async Task<List<Account>> GetRoleAccounts(long roleId)
        {
            var roleAccounts = await _roleAccountRepository.GetByCondition(ra => ra.RoleID == roleId && ra.Status == StatusEnum.Active);
            var accountIds = roleAccounts.Select(ra => ra.AccountID);
            return (await _accountRepository.GetByCondition(a => accountIds.Contains(a.ID) && a.Status == Models.Enum.StatusEnum.Active)).ToList();
        }

        public async Task<BaseResponse> SyncAccountRoles(long accountId, List<long> roleIds)
        {
            try
            {
                var account = await _accountRepository.FirstOrDefault(a => a.ID == accountId && a.Status == Models.Enum.StatusEnum.Active);
                if (account == null)
                    return BaseResponse.Error("Account không tồn tại");

                var existingAssignments = await _roleAccountRepository.GetByCondition(ra => ra.AccountID == accountId && ra.Status == StatusEnum.Active);
                var existingRoleIds = existingAssignments.Select(ra => ra.RoleID).ToList();

                var rolesToAdd = roleIds.Except(existingRoleIds).ToList();
                var rolesToRemove = existingAssignments.Where(ra => !roleIds.Contains(ra.RoleID));

                // Remove roles
                foreach (var assignment in rolesToRemove)
                {
                    assignment.SetDeleted(GetCurrentUserId());
                }

                // Add new roles
                var newAssignments = new List<RoleAccount>();
                foreach (var roleId in rolesToAdd)
                {
                    var role = await _roleRepository.FirstOrDefault(r => r.ID == roleId && r.Status == Models.Enum.StatusEnum.Active);
                    if (role == null) continue;

                    newAssignments.Add(new RoleAccount
                    {
                        AccountID = accountId,
                        RoleID = roleId,
                        CreatedBy = GetCurrentUserId()
                    });
                }

                if (newAssignments.Any())
                {
                    await _roleAccountRepository.CreateRangeAsync(newAssignments);
                }

                await UnitOfWork.SaveAsync();

                return BaseResponse.Success($"Đồng bộ {newAssignments.Count} role(s) thêm và {rolesToRemove.Count()} role(s) xóa cho account");
            }
            catch (Exception ex)
            {
                return BaseResponse.Error($"Lỗi khi đồng bộ account roles: {ex.Message}");
            }
        }

        #endregion

        #region Advanced Permission Checking

        public async Task<List<Permission>> GetPermissionsUserCanGrant(long accountId)
        {
            var accountPermissions = await GetAccountPermissions(accountId);
            // TODO: Implement logic for what permissions a user can grant based on their permissions
            return accountPermissions.Where(p => p.Resource == "role" && p.Action == PermissionAction.Manage).ToList();
        }

        #endregion

        #region Utility Methods

        public async Task<BaseResponse> InitializeDefaultPermissions()
        {
            try
            {
                var defaultPermissions = new[]
                {
                    // Account permissions
                    ("account", "view", "Xem tài khoản", "Xem thông tin tài khoản", "Account"),
                    ("account", "create", "Tạo tài khoản", "Tạo tài khoản mới", "Account"),
                    ("account", "update", "Cập nhật tài khoản", "Cập nhật thông tin tài khoản", "Account"),
                    ("account", "delete", "Xóa tài khoản", "Xóa tài khoản", "Account"),
                    
                    // License permissions
                    ("license", "view", "Xem license", "Xem thông tin license", "License"),
                    ("license", "create", "Tạo license", "Tạo license mới", "License"),
                    ("license", "update", "Cập nhật license", "Cập nhật thông tin license", "License"),
                    ("license", "delete", "Xóa license", "Xóa license", "License"),
                    
                    // Role permissions
                    ("role", "view", "Xem role", "Xem thông tin role", "Role"),
                    ("role", "create", "Tạo role", "Tạo role mới", "Role"),
                    ("role", "update", "Cập nhật role", "Cập nhật thông tin role", "Role"),
                    ("role", "delete", "Xóa role", "Xóa role", "Role"),
                    ("role", "assign", "Gán role", "Gán role cho user", "Role"),
                    
                    // Permission permissions
                    ("permission", "view", "Xem permission", "Xem thông tin permission", "Permission"),
                    ("permission", "create", "Tạo permission", "Tạo permission mới", "Permission"),
                    ("permission", "update", "Cập nhật permission", "Cập nhật thông tin permission", "Permission"),
                    ("permission", "delete", "Xóa permission", "Xóa permission", "Permission"),
                    
                    // Device permissions
                    ("device", "view", "Xem thiết bị", "Xem thông tin thiết bị", "Device"),
                    ("device", "create", "Tạo thiết bị", "Đăng ký thiết bị mới", "Device"),
                    ("device", "update", "Cập nhật thiết bị", "Cập nhật thông tin thiết bị", "Device"),
                    ("device", "delete", "Xóa thiết bị", "Xóa thiết bị", "Device"),
                };

                var createdCount = 0;
                foreach (var (resource, action, displayName, description, category) in defaultPermissions)
                {
                    var code = $"{resource}.{action}";
                    var existing = await Repository.FirstOrDefault(p => p.Code == code);
                    if (existing == null)
                    {
                        var permission = new Permission
                        {
                            Resource = resource,
                            Action = Enum.Parse<PermissionAction>(action, true),
                            Code = code,
                            Description = description,
                            Category = category
                        };
                        permission.SetCreated(GetCurrentUserId());
                        await Insert(permission);
                        createdCount++;
                    }
                }

                await UnitOfWork.SaveAsync();
                return BaseResponse.Success($"Đã khởi tạo {createdCount} permission(s) mặc định");
            }
            catch (Exception ex)
            {
                return BaseResponse.Error($"Lỗi khi khởi tạo permissions: {ex.Message}");
            }
        }

        public async Task<List<Permission>> SearchPermissions(string searchTerm)
        {
            return (await Repository.GetByCondition(p => p.Status == Models.Enum.StatusEnum.Active && 
                (p.Code.Contains(searchTerm) || 
                 p.Description!.Contains(searchTerm) ||
                 p.Resource.Contains(searchTerm) ||
                 p.Action.ToString().Contains(searchTerm)))).ToList();
        }

        public async Task<Dictionary<string, List<Permission>>> GetPermissionsByCategory()
        {
            var permissions = await GetAllPermissions();
            return permissions.GroupBy(p => p.Category ?? "Other")
                           .ToDictionary(g => g.Key, g => g.ToList());
        }

        #endregion

        #region Composite Operations

        public async Task<BaseResponse> CreateRoleWithPermissions(string roleName, string roleDescription, List<(string resource, string action, string scope)> permissions)
        {
            try
            {
                // Kiểm tra role đã tồn tại chưa
                var existingRole = await _roleRepository.FirstOrDefault(r => r.RoleName == roleName && r.Status == Models.Enum.StatusEnum.Active);
                if (existingRole != null)
                    return BaseResponse.Error($"Role '{roleName}' đã tồn tại");

                // Tạo role mới
                var role = new Role
                {
                    RoleName = roleName,
                    RoleDescription = roleDescription,
                    Status = Models.Enum.StatusEnum.Active
                };
                role.SetCreated(GetCurrentUserId());

                await _roleRepository.CreateAsync(role);
                await UnitOfWork.SaveAsync();

                var assignedPermissions = new List<Permission>();
                var rolePermissions = new List<RolePermission>();

                // Tạo hoặc lấy permissions và gán cho role
                foreach (var (resource, action, scope) in permissions)
                {
                    // Tạo hoặc lấy permission
                    var permissionCode = $"{resource.ToLower()}.{action.ToLower()}";
                    var permission = await Repository.FirstOrDefault(p => p.Code == permissionCode && p.Status != Models.Enum.StatusEnum.Active);
                    
                    if (permission == null)
                    {
                        permission = new Permission
                        {
                            Resource = resource,
                            Action = Enum.Parse<PermissionAction>(action, true),
                            Code = permissionCode,
                            Description = $"Quyền {action} cho {resource}",
                            Category = resource
                        };
                        permission.GenerateCode();
                        permission.SetCreated(GetCurrentUserId());
                        
                        await Repository.CreateAsync(permission);
                    }

                    assignedPermissions.Add(permission);

                    // Tạo role permission
                    var rolePermission = new RolePermission
                    {
                        RoleId = role.ID,
                        PermissionId = permission.ID,
                        // Priority = 0 // TODO: Removed in schema update
                    };
                    rolePermission.SetCreated(GetCurrentUserId());
                    
                    rolePermissions.Add(rolePermission);
                }

                // Lưu tất cả permissions mới
                await UnitOfWork.SaveAsync();

                // Gán permissions cho role
                if (rolePermissions.Any())
                {
                    await _rolePermissionRepository.CreateRangeAsync(rolePermissions);
                    await UnitOfWork.SaveAsync();
                }

                return BaseResponse.Success(new
                {
                    Role = new { role.ID, role.RoleName, role.RoleDescription },
                    PermissionsCreated = assignedPermissions.Count(p => p.ID == 0), // permissions mới tạo
                    PermissionsAssigned = assignedPermissions.Count,
                    AssignedPermissions = assignedPermissions.Select(p => new { p.Code, p.Category })
                }, $"Tạo role '{roleName}' thành công với {assignedPermissions.Count} permission(s)");
            }
            catch (Exception ex)
            {
                return BaseResponse.Error($"Lỗi khi tạo role với permissions: {ex.Message}");
            }
        }

        public async Task<BaseResponse> CreateRoleWithDefaultPermissions(string roleName, string roleDescription, string roleType = "user")
        {
            try
            {
                var defaultPermissions = GetDefaultPermissionsByRoleType(roleType);
                return await CreateRoleWithPermissions(roleName, roleDescription, defaultPermissions);
            }
            catch (Exception ex)
            {
                return BaseResponse.Error($"Lỗi khi tạo role với permissions mặc định: {ex.Message}");
            }
        }

        private List<(string resource, string action, string scope)> GetDefaultPermissionsByRoleType(string roleType)
        {
            return roleType.ToLower() switch
            {
                "admin" => new List<(string, string, string)>
                {
                    ("account", "view", "all"),
                    ("account", "create", "all"),
                    ("account", "update", "all"),
                    ("account", "delete", "all"),
                    ("role", "view", "all"),
                    ("role", "create", "all"),
                    ("role", "update", "all"),
                    ("role", "delete", "all"),
                    ("role", "assign", "all"),
                    ("permission", "view", "all"),
                    ("permission", "create", "all"),
                    ("permission", "update", "all"),
                    ("permission", "delete", "all"),
                    ("license", "view", "all"),
                    ("license", "create", "all"),
                    ("license", "update", "all"),
                    ("license", "delete", "all"),
                    ("device", "view", "all"),
                    ("device", "create", "all"),
                    ("device", "update", "all"),
                    ("device", "delete", "all")
                },
                "manager" => new List<(string, string, string)>
                {
                    ("account", "view", "org"),
                    ("account", "create", "team"),
                    ("account", "update", "team"),
                    ("role", "view", "org"),
                    ("role", "assign", "team"),
                    ("license", "view", "org"),
                    ("license", "create", "team"),
                    ("license", "update", "team"),
                    ("device", "view", "org"),
                    ("device", "create", "team"),
                    ("device", "update", "team")
                },
                "user" => new List<(string, string, string)>
                {
                    ("account", "view", "own"),
                    ("account", "update", "own"),
                    ("license", "view", "own"),
                    ("device", "view", "own"),
                    ("device", "create", "own"),
                    ("device", "update", "own")
                },
                "viewer" => new List<(string, string, string)>
                {
                    ("account", "view", "own"),
                    ("license", "view", "own"),
                    ("device", "view", "own")
                },
                _ => new List<(string, string, string)>
                {
                    ("account", "view", "own"),
                    ("license", "view", "own"),
                    ("device", "view", "own")
                }
            };
        }

        public async Task<BaseResponse> CreateRoleAndAssignToAccount(long accountId, string roleName, string roleDescription, List<(string resource, string action, string scope)> permissions)
        {
            try
            {
                // Kiểm tra account có tồn tại không
                var account = await _accountRepository.FirstOrDefault(a => a.ID == accountId && a.Status == Models.Enum.StatusEnum.Active);
                if (account == null)
                    return BaseResponse.Error("Account không tồn tại");

                // Tạo role với permissions
                var createRoleResult = await CreateRoleWithPermissions(roleName, roleDescription, permissions);
                if (!createRoleResult.IsSuccess)
                    return createRoleResult;

                // Lấy role vừa tạo
                var role = await _roleRepository.FirstOrDefault(r => r.RoleName == roleName && r.Status == Models.Enum.StatusEnum.Active);
                if (role == null)
                    return BaseResponse.Error("Không tìm thấy role vừa tạo");

                // Gán role cho account
                var assignResult = await AssignRoleToAccount(accountId, role.ID);
                if (!assignResult.IsSuccess)
                    return BaseResponse.Error($"Tạo role thành công nhưng gán cho account thất bại: {assignResult.Message}");

                return BaseResponse.Success(new
                {
                    AccountId = accountId,
                    AccountEmail = account.Email,
                    Role = new { role.ID, role.RoleName, role.RoleDescription },
                    PermissionsCount = permissions.Count
                }, $"Tạo role '{roleName}' và gán cho account '{account.Email}' thành công");
            }
            catch (Exception ex)
            {
                return BaseResponse.Error($"Lỗi khi tạo role và gán cho account: {ex.Message}");
            }
        }

        public async Task<BaseResponse> CreateAccountWithRole(string email, string fullName, string roleName, string roleType = "user")
        {
            try
            {
                // Kiểm tra email đã tồn tại chưa
                var existingAccount = await _accountRepository.FirstOrDefault(a => a.Email == email && a.Status == Models.Enum.StatusEnum.Active);
                if (existingAccount != null)
                    return BaseResponse.Error($"Email '{email}' đã tồn tại");

                // Tạo account mới
                var account = new Account
                {
                    Email = email,
                    Name = fullName,
                    UserName = email,
                    Status = Models.Enum.StatusEnum.Active,
                    // Status = Models.Enum.StatusEnum.Active // Removed duplicate initializer
                };
                account.SetCreated(GetCurrentUserId());

                await _accountRepository.CreateAsync(account);
                await UnitOfWork.SaveAsync();

                // Kiểm tra role có tồn tại không, nếu không thì tạo mới
                var role = await _roleRepository.FirstOrDefault(r => r.RoleName == roleName && r.Status == Models.Enum.StatusEnum.Active);
                if (role == null)
                {
                    var createRoleResult = await CreateRoleWithDefaultPermissions(roleName, $"Role {roleName} được tạo tự động", roleType);
                    if (!createRoleResult.IsSuccess)
                        return BaseResponse.Error($"Tạo account thành công nhưng tạo role thất bại: {createRoleResult.Message}");
                    
                    role = await _roleRepository.FirstOrDefault(r => r.RoleName == roleName && r.Status == Models.Enum.StatusEnum.Active);
                }

                // Gán role cho account
                if (role != null)
                {
                    var assignResult = await AssignRoleToAccount(account.ID, role.ID);
                    if (!assignResult.IsSuccess)
                        return BaseResponse.Error($"Tạo account thành công nhưng gán role thất bại: {assignResult.Message}");
                }

                return BaseResponse.Success(new
                {
                    Account = new { account.ID, account.Email, account.Name },
                    Role = role != null ? new { role.ID, role.RoleName, role.RoleDescription } : null
                }, $"Tạo account '{email}' với role '{roleName}' thành công");
            }
            catch (Exception ex)
            {
                return BaseResponse.Error($"Lỗi khi tạo account với role: {ex.Message}");
            }
        }

        #endregion
    }
}
