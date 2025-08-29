using AutoAppManagement.Models.BaseEntity;
using AutoAppManagement.Models.Common;
using AutoAppManagement.Models.DTO;
using AutoAppManagement.Models.DTO.Role;
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
        Task<PagingResultDTO<RoleDTO>> GetAllRoles(int page = 1, int pageSize = 10, string status = "", string group = "", string search = "");
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
            var roleAccounts = await _roleAccountRepository.GetByCondition(ra => ra.AccountId == accountId && !ra.IsDeleted);
            var roleIds = roleAccounts.Select(ra => ra.RoleId);
            var roles = await Repository.GetByCondition(r => roleIds.Contains(r.Id) && !r.IsDeleted);
            return Mapper.Map<List<RoleDTO>>(roles);
        }

        public async Task<PagingResultDTO<RoleDTO>> GetAllRoles(int page = 1, int pageSize = 10, string status = "", string group = "", string search = "")
        {
            // Kết hợp các search term
            var searchTerms = new List<string>();
            if (!string.IsNullOrEmpty(status)) searchTerms.Add(status);
            if (!string.IsNullOrEmpty(group)) searchTerms.Add(group);
            if (!string.IsNullOrEmpty(search)) searchTerms.Add(search);
            
            var combinedSearch = string.Join(" ", searchTerms);
            
            // Sử dụng GetPaging từ BaseBusinessService
            var result = await GetPaging(page, pageSize, combinedSearch);
            
            // Convert về PagingResultDTO
            dynamic dynamicResult = result;
            return new PagingResultDTO<RoleDTO>
            {
                Data = dynamicResult.Data,
                PageIndex = dynamicResult.CurrentPage,
                PageSize = dynamicResult.PageSize,
                TotalItems = dynamicResult.TotalCount
            };
        }

        public async Task<BaseResponse> AssignRoleToAccount(AssignRoleRequest request)
        {
            try
            {
                var account = await _accountRepository.FirstOrDefault(a => a.Id == request.AccountId && !a.IsDeleted);
                if (account == null) return BaseResponse.Error("Account không tồn tại");

                var role = await Repository.FirstOrDefault(r => r.Id == request.RoleId && !r.IsDeleted);
                if (role == null) return BaseResponse.Error("Role không tồn tại");

                var existingAssignment = await _roleAccountRepository.FirstOrDefault(ra => ra.AccountId == request.AccountId && ra.RoleId == request.RoleId && !ra.IsDeleted);
                if (existingAssignment != null) return BaseResponse.Error("Account đã có role này");

                var roleAccount = new RoleAccount
                {
                    AccountId = request.AccountId,
                    RoleId = request.RoleId,
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
                var roleAccount = await _roleAccountRepository.FirstOrDefault(ra => ra.AccountId == accountId && ra.RoleId == roleId && !ra.IsDeleted);
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
