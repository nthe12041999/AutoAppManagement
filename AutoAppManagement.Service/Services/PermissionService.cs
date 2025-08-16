using AutoAppManagement.Models.BaseEntity;
using AutoAppManagement.Models.DTO.Account;
using AutoAppManagement.Models.DTO.Role;
using AutoAppManagement.Models.DTO.RoleAccount;
using AutoAppManagement.Models.ViewModel;
using AutoAppManagement.Repository.Common.Repository;
using AutoAppManagement.Service.Common.Cache;
using AutoAppManagement.Service.Common.Socket;
using AutoAppManagement.Service.Services.Base;
using AutoMapper;
using Microsoft.AspNetCore.Http;

namespace AutoAppManagement.Service.Services
{
    public interface IPermissionService
    {
        Task<List<RoleAccountDTO>> GetAllRoleAccounts();
        Task<List<RoleAccountDTO>> GetRoleAccountsByAccountId(long accountId);
        Task<List<RoleAccountDTO>> GetRoleAccountsByRoleId(long roleId);
        Task<RoleAccountDTO> GetRoleAccountById(long id);
        Task<RestOutput> AssignRoleToAccount(AssignRoleToAccountRequest request);
        Task<RestOutput> RemoveRoleFromAccount(long accountId, long roleId);
        Task<RestOutput> UpdateRoleAccount(UpdateRoleAccountRequest request);
        Task<RestOutput> BulkAssignRoles(BulkAssignRolesRequest request);
        Task<RestOutput> BulkRemoveRoles(BulkRemoveRolesRequest request);
        Task<List<AccountWithRolesDTO>> GetAccountsWithRoles();
        Task<List<RoleWithAccountsDTO>> GetRolesWithAccounts();
        Task<bool> CheckAccountHasRole(long accountId, long roleId);
        Task<bool> CheckAccountHasPermission(long accountId, string permission);
        Task<List<string>> GetAccountPermissions(long accountId);
        Task<RestOutput> SyncAccountRoles(long accountId, List<long> roleIds);
    }

    public class PermissionService : BaseService, IPermissionService
    {
        public PermissionService(IHttpContextAccessor httpContextAccessor, IDistributedCacheCustom cache, 
            IUnitOfWork unitOfWork, IMapper mapper, INotificationSocketHub notificationSocketHub) 
            : base(httpContextAccessor, cache, unitOfWork, mapper, notificationSocketHub)
        {
        }

        /// <summary>
        /// Lấy tất cả role accounts
        /// </summary>
        /// <returns></returns>
        public async Task<List<RoleAccountDTO>> GetAllRoleAccounts()
        {
            var roleAccounts = await UnitOfWork.RoleAccountRepository.GetAll();
            return Mapper.Map<List<RoleAccountDTO>>(roleAccounts.ToList());
        }

        /// <summary>
        /// Lấy role accounts theo account ID
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public async Task<List<RoleAccountDTO>> GetRoleAccountsByAccountId(long accountId)
        {
            var roleAccounts = await UnitOfWork.RoleAccountRepository.GetByCondition(ra => ra.AccountId == accountId);
            return Mapper.Map<List<RoleAccountDTO>>(roleAccounts.ToList());
        }

        /// <summary>
        /// Lấy role accounts theo role ID
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public async Task<List<RoleAccountDTO>> GetRoleAccountsByRoleId(long roleId)
        {
            var roleAccounts = await UnitOfWork.RoleAccountRepository.GetByCondition(ra => ra.RoleId == roleId);
            return Mapper.Map<List<RoleAccountDTO>>(roleAccounts.ToList());
        }

        /// <summary>
        /// Lấy role account theo ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<RoleAccountDTO> GetRoleAccountById(long id)
        {
            var roleAccount = await UnitOfWork.RoleAccountRepository.FirstOrDefault(ra => ra.Id == id);
            return Mapper.Map<RoleAccountDTO>(roleAccount);
        }

        /// <summary>
        /// Gán role cho account
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<RestOutput> AssignRoleToAccount(AssignRoleToAccountRequest request)
        {
            var result = new RestOutput();

            try
            {
                // Kiểm tra account tồn tại
                var account = await UnitOfWork.AccountsRepository.FirstOrDefault(a => a.Id == request.AccountId && !a.IsDeleted);
                if (account == null)
                {
                    result.ErrorEventHandler("Account không tồn tại");
                    return result;
                }

                // Kiểm tra role tồn tại
                var role = await UnitOfWork.RoleRepository.FirstOrDefault(r => r.Id == request.RoleId);
                if (role == null)
                {
                    result.ErrorEventHandler("Role không tồn tại");
                    return result;
                }

                // Kiểm tra đã gán role chưa
                var existingRoleAccount = await UnitOfWork.RoleAccountRepository.FirstOrDefault(ra => 
                    ra.AccountId == request.AccountId && ra.RoleId == request.RoleId);
                if (existingRoleAccount != null)
                {
                    result.ErrorEventHandler("Account đã có role này");
                    return result;
                }

                var roleAccount = new RoleAccount
                {
                    AccountId = request.AccountId,
                    RoleId = request.RoleId,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = GetUserAuthen()?.Id,
                    Notes = request.Notes
                };

                await UnitOfWork.RoleAccountRepository.CreateAsync(roleAccount);
                await UnitOfWork.CommitAsync();

                result.SuccessEventHandler(Mapper.Map<RoleAccountDTO>(roleAccount));
            }
            catch (Exception ex)
            {
                result.ErrorEventHandler(ex.Message);
            }

            return result;
        }

        /// <summary>
        /// Gỡ role khỏi account
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public async Task<RestOutput> RemoveRoleFromAccount(long accountId, long roleId)
        {
            var result = new RestOutput();

            try
            {
                var roleAccount = await UnitOfWork.RoleAccountRepository.FirstOrDefault(ra => 
                    ra.AccountId == accountId && ra.RoleId == roleId);
                if (roleAccount == null)
                {
                    result.ErrorEventHandler("Role assignment không tồn tại");
                    return result;
                }

                UnitOfWork.RoleAccountRepository.Delete(roleAccount);
                await UnitOfWork.CommitAsync();

                result.SuccessEventHandler(true);
            }
            catch (Exception ex)
            {
                result.ErrorEventHandler(ex.Message);
            }

            return result;
        }

        /// <summary>
        /// Cập nhật role account
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<RestOutput> UpdateRoleAccount(UpdateRoleAccountRequest request)
        {
            var result = new RestOutput();

            try
            {
                var roleAccount = await UnitOfWork.RoleAccountRepository.FirstOrDefault(ra => ra.Id == request.Id);
                if (roleAccount == null)
                {
                    result.ErrorEventHandler("Role account không tồn tại");
                    return result;
                }

                roleAccount.Notes = request.Notes;
                roleAccount.UpdatedDate = DateTime.UtcNow;
                roleAccount.UpdatedBy = GetUserAuthen()?.Id;

                await UnitOfWork.CommitAsync();

                result.SuccessEventHandler(Mapper.Map<RoleAccountDTO>(roleAccount));
            }
            catch (Exception ex)
            {
                result.ErrorEventHandler(ex.Message);
            }

            return result;
        }

        /// <summary>
        /// Gán nhiều role cho account
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<RestOutput> BulkAssignRoles(BulkAssignRolesRequest request)
        {
            var result = new RestOutput();

            try
            {
                // Kiểm tra account tồn tại
                var account = await UnitOfWork.AccountsRepository.FirstOrDefault(a => a.Id == request.AccountId && !a.IsDeleted);
                if (account == null)
                {
                    result.ErrorEventHandler("Account không tồn tại");
                    return result;
                }

                var assignedCount = 0;
                foreach (var roleId in request.RoleIds)
                {
                    // Kiểm tra role tồn tại
                    var role = await UnitOfWork.RoleRepository.FirstOrDefault(r => r.Id == roleId);
                    if (role == null) continue;

                    // Kiểm tra đã gán role chưa
                    var existingRoleAccount = await UnitOfWork.RoleAccountRepository.FirstOrDefault(ra => 
                        ra.AccountId == request.AccountId && ra.RoleId == roleId);
                    if (existingRoleAccount != null) continue;

                    var roleAccount = new RoleAccount
                    {
                        AccountId = request.AccountId,
                        RoleId = roleId,
                        CreatedDate = DateTime.UtcNow,
                        CreatedBy = GetUserAuthen()?.Id,
                        Notes = request.Notes
                    };

                    await UnitOfWork.RoleAccountRepository.CreateAsync(roleAccount);
                    assignedCount++;
                }

                await UnitOfWork.CommitAsync();

                result.SuccessEventHandler($"Đã gán {assignedCount} role cho account");
            }
            catch (Exception ex)
            {
                result.ErrorEventHandler(ex.Message);
            }

            return result;
        }

        /// <summary>
        /// Gỡ nhiều role khỏi account
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<RestOutput> BulkRemoveRoles(BulkRemoveRolesRequest request)
        {
            var result = new RestOutput();

            try
            {
                var removedCount = 0;
                foreach (var roleId in request.RoleIds)
                {
                    var roleAccount = await UnitOfWork.RoleAccountRepository.FirstOrDefault(ra => 
                        ra.AccountId == request.AccountId && ra.RoleId == roleId);
                    if (roleAccount != null)
                    {
                        UnitOfWork.RoleAccountRepository.Delete(roleAccount);
                        removedCount++;
                    }
                }

                await UnitOfWork.CommitAsync();

                result.SuccessEventHandler($"Đã gỡ {removedCount} role khỏi account");
            }
            catch (Exception ex)
            {
                result.ErrorEventHandler(ex.Message);
            }

            return result;
        }

        /// <summary>
        /// Lấy accounts với roles
        /// </summary>
        /// <returns></returns>
        public async Task<List<AccountWithRolesDTO>> GetAccountsWithRoles()
        {
            var accounts = await UnitOfWork.AccountsRepository.GetByCondition(a => !a.IsDeleted);
            var result = new List<AccountWithRolesDTO>();

            foreach (var account in accounts)
            {
                var roles = (from r in UnitOfWork.RoleRepository.Get()
                           join ra in UnitOfWork.RoleAccountRepository.Get() on r.Id equals ra.RoleId
                           where ra.AccountId == account.Id
                           select r).ToList();

                result.Add(new AccountWithRolesDTO
                {
                    Account = Mapper.Map<AccountDTO>(account),
                    Roles = Mapper.Map<List<RoleDTO>>(roles)
                });
            }

            return result;
        }

        /// <summary>
        /// Lấy roles với accounts
        /// </summary>
        /// <returns></returns>
        public async Task<List<RoleWithAccountsDTO>> GetRolesWithAccounts()
        {
            var roles = await UnitOfWork.RoleRepository.GetAll();
            var result = new List<RoleWithAccountsDTO>();

            foreach (var role in roles)
            {
                var accounts = (from a in UnitOfWork.AccountsRepository.Get()
                              join ra in UnitOfWork.RoleAccountRepository.Get() on a.Id equals ra.AccountId
                              where ra.RoleId == role.Id && !a.IsDeleted
                              select a).ToList();

                result.Add(new RoleWithAccountsDTO
                {
                    Role = Mapper.Map<RoleDTO>(role),
                    Accounts = Mapper.Map<List<AccountDTO>>(accounts)
                });
            }

            return result;
        }

        /// <summary>
        /// Kiểm tra account có role không
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public async Task<bool> CheckAccountHasRole(long accountId, long roleId)
        {
            var roleAccount = await UnitOfWork.RoleAccountRepository.FirstOrDefault(ra => 
                ra.AccountId == accountId && ra.RoleId == roleId);
            return roleAccount != null;
        }

        /// <summary>
        /// Kiểm tra account có permission không
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="permission"></param>
        /// <returns></returns>
        public async Task<bool> CheckAccountHasPermission(long accountId, string permission)
        {
            // Logic kiểm tra permission dựa trên roles của account
            var roleAccounts = await UnitOfWork.RoleAccountRepository.GetByCondition(ra => ra.AccountId == accountId);
            
            foreach (var roleAccount in roleAccounts)
            {
                var role = await UnitOfWork.RoleRepository.FirstOrDefault(r => r.Id == roleAccount.RoleId);
                if (role != null && !string.IsNullOrEmpty(role.RoleDescription) && 
                    role.RoleDescription.Contains(permission))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Lấy tất cả permissions của account
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public async Task<List<string>> GetAccountPermissions(long accountId)
        {
            var permissions = new List<string>();
            var roleAccounts = await UnitOfWork.RoleAccountRepository.GetByCondition(ra => ra.AccountId == accountId);
            
            foreach (var roleAccount in roleAccounts)
            {
                var role = await UnitOfWork.RoleRepository.FirstOrDefault(r => r.Id == roleAccount.RoleId);
                if (role != null && !string.IsNullOrEmpty(role.RoleDescription))
                {
                    // Giả sử permissions được lưu dưới dạng comma-separated trong RoleDescription
                    var rolePermissions = role.RoleDescription.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    permissions.AddRange(rolePermissions.Select(p => p.Trim()));
                }
            }

            return permissions.Distinct().ToList();
        }

        /// <summary>
        /// Đồng bộ roles của account
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="roleIds"></param>
        /// <returns></returns>
        public async Task<RestOutput> SyncAccountRoles(long accountId, List<long> roleIds)
        {
            var result = new RestOutput();

            try
            {
                // Kiểm tra account tồn tại
                var account = await UnitOfWork.AccountsRepository.FirstOrDefault(a => a.Id == accountId && !a.IsDeleted);
                if (account == null)
                {
                    result.ErrorEventHandler("Account không tồn tại");
                    return result;
                }

                // Xóa tất cả role hiện tại
                var existingRoleAccounts = await UnitOfWork.RoleAccountRepository.GetByCondition(ra => ra.AccountId == accountId);
                foreach (var roleAccount in existingRoleAccounts)
                {
                    UnitOfWork.RoleAccountRepository.Delete(roleAccount);
                }

                // Thêm roles mới
                var addedCount = 0;
                foreach (var roleId in roleIds)
                {
                    // Kiểm tra role tồn tại
                    var role = await UnitOfWork.RoleRepository.FirstOrDefault(r => r.Id == roleId);
                    if (role == null) continue;

                    var newRoleAccount = new RoleAccount
                    {
                        AccountId = accountId,
                        RoleId = roleId,
                        CreatedDate = DateTime.UtcNow,
                        CreatedBy = GetUserAuthen()?.Id
                    };

                    await UnitOfWork.RoleAccountRepository.CreateAsync(newRoleAccount);
                    addedCount++;
                }

                await UnitOfWork.CommitAsync();

                result.SuccessEventHandler($"Đã đồng bộ {addedCount} role cho account");
            }
            catch (Exception ex)
            {
                result.ErrorEventHandler(ex.Message);
            }

            return result;
        }
    }
}
