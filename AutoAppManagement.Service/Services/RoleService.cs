using AutoAppManagement.Models.BaseEntity;
using AutoAppManagement.Models.DTO.Role;
using AutoAppManagement.Models.ViewModel;
using AutoAppManagement.Repository.Common.Repository;
using AutoAppManagement.Service.Common.Cache;
using AutoAppManagement.Service.Common.Socket;
using AutoAppManagement.Service.Services.Base;
using AutoMapper;
using Microsoft.AspNetCore.Http;

namespace AutoAppManagement.Service.Services
{
    public interface IRoleService
    {
        Task<List<RoleDTO>> GetAllRoles();
        Task<RoleDTO> GetRoleById(long id);
        Task<RestOutput> CreateRole(CreateRoleRequest request);
        Task<RestOutput> UpdateRole(UpdateRoleRequest request);
        Task<RestOutput> DeleteRole(long id);
        Task<List<RoleDTO>> GetRolesByAccountId(long accountId);
        Task<RestOutput> AssignRoleToAccount(AssignRoleRequest request);
        Task<RestOutput> RemoveRoleFromAccount(long accountId, long roleId);
        Task<bool> CheckRoleExists(string roleName);
    }

    public class RoleService : BaseService, IRoleService
    {
        public RoleService(IHttpContextAccessor httpContextAccessor, IDistributedCacheCustom cache, 
            IUnitOfWork unitOfWork, IMapper mapper, INotificationSocketHub notificationSocketHub) 
            : base(httpContextAccessor, cache, unitOfWork, mapper, notificationSocketHub)
        {
        }

        /// <summary>
        /// Lấy tất cả roles
        /// </summary>
        /// <returns></returns>
        public async Task<List<RoleDTO>> GetAllRoles()
        {
            var roles = await UnitOfWork.RoleRepository.GetAll();
            return Mapper.Map<List<RoleDTO>>(roles.ToList());
        }

        /// <summary>
        /// Lấy role theo ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<RoleDTO> GetRoleById(long id)
        {
            var role = await UnitOfWork.RoleRepository.FirstOrDefault(r => r.Id == id);
            return Mapper.Map<RoleDTO>(role);
        }

        /// <summary>
        /// Tạo role mới
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<RestOutput> CreateRole(CreateRoleRequest request)
        {
            var result = new RestOutput();

            try
            {
                // Kiểm tra role đã tồn tại chưa
                var existingRole = await UnitOfWork.RoleRepository.FirstOrDefault(r => r.RoleName == request.RoleName);
                if (existingRole != null)
                {
                    result.ErrorEventHandler("Role đã tồn tại");
                    return result;
                }

                var role = new Role
                {
                    RoleName = request.RoleName,
                    RoleDescription = request.RoleDescription
                };

                await UnitOfWork.RoleRepository.CreateAsync(role);
                await UnitOfWork.CommitAsync();

                result.SuccessEventHandler(Mapper.Map<RoleDTO>(role));
            }
            catch (Exception ex)
            {
                result.ErrorEventHandler(ex.Message);
            }

            return result;
        }

        /// <summary>
        /// Cập nhật role
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<RestOutput> UpdateRole(UpdateRoleRequest request)
        {
            var result = new RestOutput();

            try
            {
                var role = await UnitOfWork.RoleRepository.FirstOrDefault(r => r.Id == request.Id);
                if (role == null)
                {
                    result.ErrorEventHandler("Role không tồn tại");
                    return result;
                }

                // Kiểm tra tên role đã tồn tại chưa (trừ role hiện tại)
                var existingRole = await UnitOfWork.RoleRepository.FirstOrDefault(r => r.RoleName == request.RoleName && r.Id != request.Id);
                if (existingRole != null)
                {
                    result.ErrorEventHandler("Tên role đã tồn tại");
                    return result;
                }

                role.RoleName = request.RoleName;
                role.RoleDescription = request.RoleDescription;

                await UnitOfWork.CommitAsync();

                result.SuccessEventHandler(Mapper.Map<RoleDTO>(role));
            }
            catch (Exception ex)
            {
                result.ErrorEventHandler(ex.Message);
            }

            return result;
        }

        /// <summary>
        /// Xóa role
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<RestOutput> DeleteRole(long id)
        {
            var result = new RestOutput();

            try
            {
                var role = await UnitOfWork.RoleRepository.FirstOrDefault(r => r.Id == id);
                if (role == null)
                {
                    result.ErrorEventHandler("Role không tồn tại");
                    return result;
                }

                // Kiểm tra xem có account nào đang sử dụng role này không
                var roleAccounts = await UnitOfWork.RoleAccountRepository.GetByCondition(ra => ra.RoleId == id);
                if (roleAccounts.Any())
                {
                    result.ErrorEventHandler("Không thể xóa role đang được sử dụng");
                    return result;
                }

                UnitOfWork.RoleRepository.Delete(role);
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
        /// Lấy roles của account
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public async Task<List<RoleDTO>> GetRolesByAccountId(long accountId)
        {
            var roles = (from r in UnitOfWork.RoleRepository.Get()
                        join ra in UnitOfWork.RoleAccountRepository.Get() on r.Id equals ra.RoleId
                        where ra.AccountId == accountId
                        select r).ToList();

            return Mapper.Map<List<RoleDTO>>(roles);
        }

        /// <summary>
        /// Gán role cho account
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<RestOutput> AssignRoleToAccount(AssignRoleRequest request)
        {
            var result = new RestOutput();

            try
            {
                // Kiểm tra account tồn tại
                var account = await UnitOfWork.AccountsRepository.FirstOrDefault(a => a.Id == request.AccountId);
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
                var existingRoleAccount = await UnitOfWork.RoleAccountRepository.FirstOrDefault(ra => ra.AccountId == request.AccountId && ra.RoleId == request.RoleId);
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
                    CreatedBy = GetUserAuthen()?.Id
                };

                await UnitOfWork.RoleAccountRepository.CreateAsync(roleAccount);
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
                var roleAccount = await UnitOfWork.RoleAccountRepository.FirstOrDefault(ra => ra.AccountId == accountId && ra.RoleId == roleId);
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
        /// Kiểm tra role có tồn tại không
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public async Task<bool> CheckRoleExists(string roleName)
        {
            var role = await UnitOfWork.RoleRepository.FirstOrDefault(r => r.RoleName == roleName);
            return role != null;
        }
    }
}
