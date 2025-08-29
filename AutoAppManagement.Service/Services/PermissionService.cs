using AutoAppManagement.Models.BaseEntity;
using AutoAppManagement.Models.Common;
using AutoAppManagement.Models.DTO.Account;
using AutoAppManagement.Models.DTO.Role;
using AutoAppManagement.Models.DTO.RoleAccount;
using AutoAppManagement.Repository.Repositories;
using AutoAppManagement.Repository.Repositories.Base;
using AutoAppManagement.Service.Services.Base;

namespace AutoAppManagement.Service.Services
{
    public interface IPermissionService : IBaseBusinessService<RoleAccountDTO>
    {
        Task<List<RoleAccountDTO>> GetRoleAccountsByAccountId(long accountId);
        Task<List<RoleAccountDTO>> GetRoleAccountsByRoleId(long roleId);
        Task<BaseResponse> AssignRoleToAccount(AssignRoleToAccountRequest request);
        Task<BaseResponse> RemoveRoleFromAccount(long accountId, long roleId);
        Task<BaseResponse> UpdateRoleAccount(UpdateRoleAccountRequest request);
        Task<BaseResponse> BulkAssignRoles(BulkAssignRolesRequest request);
        Task<BaseResponse> BulkRemoveRoles(BulkRemoveRolesRequest request);
        Task<List<AccountWithRolesDTO>> GetAccountsWithRoles();
        Task<List<RoleWithAccountsDTO>> GetRolesWithAccounts();
        Task<bool> CheckAccountHasRole(long accountId, long roleId);
        Task<bool> CheckAccountHasPermission(long accountId, string permission);
        Task<List<string>> GetAccountPermissions(long accountId);
        Task<BaseResponse> SyncAccountRoles(long accountId, List<long> roleIds);
    }

    public class PermissionService : BaseBusinessService<RoleAccount, RoleAccountDTO, IRoleAccountRepository>, IPermissionService
    {
        private readonly IGenericRepository<Account> _accountRepository;
        private readonly IGenericRepository<Role> _roleRepository;

        public PermissionService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _accountRepository = UnitOfWork.GetRepository<Account>();
            _roleRepository = UnitOfWork.GetRepository<Role>();
        }

        public async Task<List<RoleAccountDTO>> GetRoleAccountsByAccountId(long accountId)
        {
            var roleAccounts = await Repository.GetByCondition(ra => ra.AccountId == accountId && !ra.IsDeleted);
            return Mapper.Map<List<RoleAccountDTO>>(roleAccounts.ToList());
        }

        public async Task<List<RoleAccountDTO>> GetRoleAccountsByRoleId(long roleId)
        {
            var roleAccounts = await Repository.GetByCondition(ra => ra.RoleId == roleId && !ra.IsDeleted);
            return Mapper.Map<List<RoleAccountDTO>>(roleAccounts.ToList());
        }

        public async Task<BaseResponse> AssignRoleToAccount(AssignRoleToAccountRequest request)
        {
            try
            {
                var existingRoleAccount = await Repository.FirstOrDefault(ra => ra.AccountId == request.AccountId && ra.RoleId == request.RoleId && !ra.IsDeleted);
                if (existingRoleAccount != null) return BaseResponse.Error("Account đã có role này");

                var roleAccount = Mapper.Map<RoleAccount>(request);
                roleAccount.SetCreated(GetCurrentUserId());

                await Insert(roleAccount);
                await UnitOfWork.SaveAsync();

                return BaseResponse.Success(Mapper.Map<RoleAccountDTO>(roleAccount), "Gán role thành công");
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
                var roleAccount = await Repository.FirstOrDefault(ra => ra.AccountId == accountId && ra.RoleId == roleId && !ra.IsDeleted);
                if (roleAccount == null) return BaseResponse.Error("Role assignment không tồn tại");

                roleAccount.SetDeleted(GetCurrentUserId());
                await UnitOfWork.SaveAsync();

                return BaseResponse.Success("Gỡ role thành công");
            }
            catch (Exception ex)
            {
                return BaseResponse.Error($"Lỗi khi gỡ role: {ex.Message}");
            }
        }

        public async Task<BaseResponse> UpdateRoleAccount(UpdateRoleAccountRequest request)
        {
            try
            {
                var roleAccount = await UpdateById(request.Id);

                await UnitOfWork.SaveAsync();

                return BaseResponse.Success("UpdateRoleAccount role thành công");
            }
            catch (Exception ex)
            {
                return BaseResponse.Error($"Lỗi khi cập nhật role account: {ex.Message}");
            }
        }

        public async Task<BaseResponse> BulkAssignRoles(BulkAssignRolesRequest request)
        {
            try
            {
                var account = await _accountRepository.FirstOrDefault(a => a.Id == request.AccountId && !a.IsDeleted);
                if (account == null) return BaseResponse.Error("Account không tồn tại");

                var newAssignments = new List<RoleAccount>();
                foreach (var roleId in request.RoleIds)
                {
                    var role = await _roleRepository.FirstOrDefault(r => r.Id == roleId && !r.IsDeleted);
                    if (role == null) continue;

                    var existing = await Repository.FirstOrDefault(ra => ra.AccountId == request.AccountId && ra.RoleId == roleId && !ra.IsDeleted);
                    if (existing != null) continue;

                    newAssignments.Add(new RoleAccount
                    {
                        AccountId = request.AccountId,
                        RoleId = roleId,
                        CreatedBy = GetCurrentUserId(),
                        Notes = request.Notes
                    });
                }

                if (newAssignments.Any())
                {
                    await Insert(newAssignments);
                    await UnitOfWork.SaveAsync();
                }

                return BaseResponse.Success($"Đã gán {newAssignments.Count} role(s) cho account");
            }
            catch (Exception ex)
            {
                return BaseResponse.Error($"Lỗi khi gán role hàng loạt: {ex.Message}");
            }
        }

        public async Task<BaseResponse> BulkRemoveRoles(BulkRemoveRolesRequest request)
        {
            try
            {
                var assignmentsToRemove = await Repository.GetByCondition(ra => ra.AccountId == request.AccountId && request.RoleIds.Contains(ra.RoleId) && !ra.IsDeleted);
                if (!assignmentsToRemove.Any()) return BaseResponse.Success("Không có role nào để gỡ");

                foreach (var assignment in assignmentsToRemove)
                {
                    assignment.SetDeleted(GetCurrentUserId());
                }

                await UnitOfWork.SaveAsync();

                return BaseResponse.Success($"Đã gỡ {assignmentsToRemove.Count()} role(s) khỏi account");
            }
            catch (Exception ex)
            {
                return BaseResponse.Error($"Lỗi khi gỡ role hàng loạt: {ex.Message}");
            }
        }

        public async Task<List<AccountWithRolesDTO>> GetAccountsWithRoles()
        {
            var accounts = await _accountRepository.GetByCondition(a => !a.IsDeleted);
            var roleAssignments = await Repository.GetAll();
            var roles = await _roleRepository.GetAll();

            return accounts.Select(account => new AccountWithRolesDTO
            {
                Account = Mapper.Map<AccountDTO>(account),
                Roles = Mapper.Map<List<RoleDTO>>(roleAssignments
                    .Where(ra => ra.AccountId == account.Id && !ra.IsDeleted)
                    .Join(roles, ra => ra.RoleId, r => r.Id, (ra, r) => r)
                    .Where(r => !r.IsDeleted)
                    .ToList())
            }).ToList();
        }

        public async Task<List<RoleWithAccountsDTO>> GetRolesWithAccounts()
        {
            var roles = await _roleRepository.GetByCondition(r => !r.IsDeleted);
            var roleAssignments = await Repository.GetAll();
            var accounts = await _accountRepository.GetAll();

            return roles.Select(role => new RoleWithAccountsDTO
            {
                Role = Mapper.Map<RoleDTO>(role),
                Accounts = Mapper.Map<List<AccountDTO>>(roleAssignments
                    .Where(ra => ra.RoleId == role.Id && !ra.IsDeleted)
                    .Join(accounts, ra => ra.AccountId, a => a.Id, (ra, a) => a)
                    .Where(a => !a.IsDeleted)
                    .ToList())
            }).ToList();
        }

        public async Task<bool> CheckAccountHasRole(long accountId, long roleId)
        {
            return await Any(ra => ra.AccountId == accountId && ra.RoleId == roleId && !ra.IsDeleted);
        }

        public async Task<bool> CheckAccountHasPermission(long accountId, string permission)
        {
            var roles = await GetAccountRoles(accountId);
            return roles.Any(r => !string.IsNullOrEmpty(r.RoleDescription) && r.RoleDescription.Contains(permission));
        }

        public async Task<List<string>> GetAccountPermissions(long accountId)
        {
            var roles = await GetAccountRoles(accountId);
            var permissions = new List<string>();
            foreach (var role in roles)
            {
                if (!string.IsNullOrEmpty(role.RoleDescription))
                {
                    permissions.AddRange(role.RoleDescription.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim()));
                }
            }
            return permissions.Distinct().ToList();
        }

        private async Task<IEnumerable<Role>> GetAccountRoles(long accountId)
        {
            var roleAssignments = await Repository.GetByCondition(ra => ra.AccountId == accountId && !ra.IsDeleted);
            var roleIds = roleAssignments.Select(ra => ra.RoleId);
            return await _roleRepository.GetByCondition(r => roleIds.Contains(r.Id) && !r.IsDeleted);
        }

        public async Task<BaseResponse> SyncAccountRoles(long accountId, List<long> roleIds)
        {
            try
            {
                var existingAssignments = await Repository.GetByCondition(ra => ra.AccountId == accountId && !ra.IsDeleted);
                var existingRoleIds = existingAssignments.Select(ra => ra.RoleId).ToList();

                var rolesToAdd = roleIds.Except(existingRoleIds).ToList();
                var rolesToRemove = existingRoleIds.Except(roleIds).ToList();

                var assignmentsToRemove = existingAssignments.Where(ra => rolesToRemove.Contains(ra.RoleId));
                foreach (var assignment in assignmentsToRemove)
                {
                    assignment.SetDeleted(GetCurrentUserId());
                }

                var newAssignments = new List<RoleAccount>();
                foreach (var roleId in rolesToAdd)
                {
                    var role = await _roleRepository.FirstOrDefault(r => r.Id == roleId && !r.IsDeleted);
                    if (role == null) continue;

                    newAssignments.Add(new RoleAccount
                    {
                        AccountId = accountId,
                        RoleId = roleId,
                        CreatedBy = GetCurrentUserId()
                    });
                }
                if (newAssignments.Any())
                {
                    await Insert(newAssignments);
                }

                await UnitOfWork.SaveAsync();

                return BaseResponse.Success($"Đã đồng bộ {rolesToAdd.Count} role(s) thêm và {rolesToRemove.Count} role(s) xóa cho account");
            }
            catch (Exception ex)
            {
                return BaseResponse.Error($"Lỗi khi đồng bộ role: {ex.Message}");
            }
        }
    }
}
