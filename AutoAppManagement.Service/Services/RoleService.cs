using AutoAppManagement.Models.BaseEntity;
using AutoAppManagement.Models.Common;
using AutoAppManagement.Models.DTO;
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
    }

    public class RoleService : BaseBusinessService<Role, RoleDTO, IRoleRepository>, IRoleService
    {
        // Additional repositories for related entities
        private readonly IBaseRepository<RoleAccount> _roleAccountRepository;
        private readonly IBaseRepository<Account> _accountRepository;

        public RoleService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _roleAccountRepository = UnitOfWork.GetBaseRepository<RoleAccount>();
            _accountRepository = UnitOfWork.GetBaseRepository<Account>();
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
    }
}
