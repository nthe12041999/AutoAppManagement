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

        public RoleService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _roleAccountRepository = UnitOfWork.GetBaseRepository<RoleAccount>();
            _accountRepository = UnitOfWork.GetBaseRepository<Account>();
        }

        /// <summary>
        /// Validate Role trước khi submit
        /// </summary>
        public override async Task CustomBeforeSubmitData(RoleDTO dto)
        {
            if (dto.State == Models.Common.EntityState.Add)
            {
                // Đảm bảo Code không được để trống
                if (string.IsNullOrWhiteSpace(dto.Code))
                {
                    throw new ArgumentException("Code không được để trống");
                }

                // Kiểm tra Code trùng
                var existingRole = await Repository.FirstOrDefault(r => 
                    r.Code == dto.Code && r.Status == Models.Enum.StatusEnum.Active);
                if (existingRole != null)
                {
                    throw new Exception($"Code '{dto.Code}' đã tồn tại trong hệ thống");
                }
            }
            else if (dto.State == Models.Common.EntityState.Edit)
            {
                // Đảm bảo Code không được để trống
                if (string.IsNullOrWhiteSpace(dto.Code))
                {
                    throw new ArgumentException("Code không được để trống");
                }

                // Kiểm tra Code trùng (trừ chính nó)
                var existingRole = await Repository.FirstOrDefault(r => 
                    r.Code == dto.Code && 
                    r.ID != dto.ID && 
                    r.Status == Models.Enum.StatusEnum.Active);
                if (existingRole != null)
                {
                    throw new Exception($"Code '{dto.Code}' đã tồn tại trong hệ thống");
                }
            }

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
                    code = roleDto.Code,
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
                        code = p.Code,
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
