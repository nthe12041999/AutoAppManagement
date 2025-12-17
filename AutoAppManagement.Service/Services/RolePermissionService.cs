using AutoAppManagement.Models.BaseEntity;
using AutoAppManagement.Models.Common;
using AutoAppManagement.Models.DTO.RolePermission;
using AutoAppManagement.Repository.Repositories.Base;
using AutoAppManagement.Service.Services.Base;

namespace AutoAppManagement.Service.Services
{
    public interface IRolePermissionService : IBaseBusinessService<RolePermissionDTO>
    {
        Task<List<RolePermission>> GetByRoleIdAsync(long roleId);
        Task<BaseResponse> CreateAsync(RolePermission rolePermission);
        Task<BaseResponse> DeleteAsync(long id);
    }

    public class RolePermissionService : BaseBusinessService<RolePermission, RolePermissionDTO, IBaseRepository<RolePermission>>, IRolePermissionService
    {
        public RolePermissionService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<List<RolePermission>> GetByRoleIdAsync(long roleId)
        {
            var rolePermissions = await Repository.FindBy(
                rp => rp.RoleId == roleId && rp.Status == Models.Enum.StatusEnum.Active
            );
            return rolePermissions.ToList();
        }

        public async Task<BaseResponse> CreateAsync(RolePermission rolePermission)
        {
            try
            {
                rolePermission.SetCreated(GetCurrentUserId());
                await Repository.CreateAsync(rolePermission);
                await UnitOfWork.SaveAsync();

                return new BaseResponse
                {
                    IsSuccess = true,
                    Message = "Tạo RolePermission thành công",
                    Data = rolePermission
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse
                {
                    IsSuccess = false,
                    Message = $"Lỗi khi tạo RolePermission: {ex.Message}"
                };
            }
        }

        public async Task<BaseResponse> DeleteAsync(long id)
        {
            try
            {
                var rolePermission = await Repository.FirstOrDefault(rp => rp.ID == id);
                if (rolePermission == null)
                {
                    return new BaseResponse
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy RolePermission"
                    };
                }

                rolePermission.SetDeleted(GetCurrentUserId());
                await UnitOfWork.SaveAsync();

                return new BaseResponse
                {
                    IsSuccess = true,
                    Message = "Xóa RolePermission thành công"
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse
                {
                    IsSuccess = false,
                    Message = $"Lỗi khi xóa RolePermission: {ex.Message}"
                };
            }
        }
    }
}
