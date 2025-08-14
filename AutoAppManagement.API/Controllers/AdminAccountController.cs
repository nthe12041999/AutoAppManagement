using AutoAppManagement.API.Controllers.Base;
using AutoAppManagement.Models.Requests;
using AutoAppManagement.Models.ViewModel;
using AutoAppManagement.WebApp.Common.Attribute;
using AutoAppManagement.WebApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace AutoAppManagement.API.Controllers
{
    //[Route("api/[controller]")]
    //[ApiController]
    //public class AdminAccountController : BaseController
    //{
    //    private readonly IAdminAccountService _adminAccountService;
    //    private readonly ILogger<AdminAccountController> _logger;

    //    public AdminAccountController(
    //        IRestOutput res, 
    //        IHttpContextAccessor httpContextAccessor,
    //        IAdminAccountService adminAccountService,
    //        ILogger<AdminAccountController> logger
    //    ) : base(res, httpContextAccessor)
    //    {
    //        _adminAccountService = adminAccountService;
    //        _logger = logger;
    //    }

    //    /// <summary>
    //    /// Lấy danh sách tài khoản admin
    //    /// </summary>
    //    /// <returns></returns>
    //    [HttpGet]
    //    [CustomAuthorize]
    //    public async Task<IActionResult> GetAdminAccounts()
    //    {
    //        try
    //        {
    //            var result = await _adminAccountService.GetAdminAccountsAsync();
    //            if (result.IsSuccess)
    //            {
    //                _res.SuccessEventHandler(result.Data);
    //            }
    //            else
    //            {
    //                _res.ErrorEventHandler(message: result.Message);
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            _logger.LogError(ex, "Error getting admin accounts");
    //            _res.ErrorEventHandler(message: "Có lỗi xảy ra khi lấy danh sách admin");
    //        }
    //        return Json(_res);
    //    }

    //    /// <summary>
    //    /// Lấy tài khoản admin theo ID
    //    /// </summary>
    //    /// <param name="id">ID tài khoản</param>
    //    /// <returns></returns>
    //    [HttpGet("{id}")]
    //    [CustomAuthorize]
    //    public async Task<IActionResult> GetAdminAccountById(long id)
    //    {
    //        try
    //        {
    //            var result = await _adminAccountService.GetAdminAccountByIdAsync(id);
    //            if (result.IsSuccess)
    //            {
    //                _res.SuccessEventHandler(result.Data);
    //            }
    //            else
    //            {
    //                _res.ErrorEventHandler(message: result.Message);
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            _logger.LogError(ex, "Error getting admin account {Id}", id);
    //            _res.ErrorEventHandler(message: "Có lỗi xảy ra khi lấy thông tin admin");
    //        }
    //        return Json(_res);
    //    }

    //    /// <summary>
    //    /// Tạo tài khoản admin mới
    //    /// </summary>
    //    /// <param name="request">Thông tin tài khoản</param>
    //    /// <returns></returns>
    //    [HttpPost]
    //    [CustomAuthorize]
    //    public async Task<IActionResult> CreateAdminAccount([FromBody] CreateAdminAccountRequest request)
    //    {
    //        try
    //        {
    //            if (!ModelState.IsValid)
    //            {
    //                _res.ErrorEventHandler(message: "Dữ liệu không hợp lệ");
    //                return Json(_res);
    //            }

    //            var result = await _adminAccountService.CreateAdminAccountAsync(request);
    //            if (result.IsSuccess)
    //            {
    //                _res.SuccessEventHandler(result.Data, "Tạo tài khoản admin thành công");
    //            }
    //            else
    //            {
    //                _res.ErrorEventHandler(message: result.Message);
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            _logger.LogError(ex, "Error creating admin account");
    //            _res.ErrorEventHandler(message: "Có lỗi xảy ra khi tạo tài khoản admin");
    //        }
    //        return Json(_res);
    //    }

    //    /// <summary>
    //    /// Cập nhật tài khoản admin
    //    /// </summary>
    //    /// <param name="id">ID tài khoản</param>
    //    /// <param name="request">Thông tin cập nhật</param>
    //    /// <returns></returns>
    //    [HttpPut("{id}")]
    //    [CustomAuthorize]
    //    public async Task<IActionResult> UpdateAdminAccount(long id, [FromBody] UpdateAdminAccountRequest request)
    //    {
    //        try
    //        {
    //            if (!ModelState.IsValid)
    //            {
    //                _res.ErrorEventHandler(message: "Dữ liệu không hợp lệ");
    //                return Json(_res);
    //            }

    //            request.Id = id;
    //            var result = await _adminAccountService.UpdateAdminAccountAsync(id, request);
    //            if (result.IsSuccess)
    //            {
    //                _res.SuccessEventHandler(result.Data, "Cập nhật tài khoản admin thành công");
    //            }
    //            else
    //            {
    //                _res.ErrorEventHandler(message: result.Message);
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            _logger.LogError(ex, "Error updating admin account {Id}", id);
    //            _res.ErrorEventHandler(message: "Có lỗi xảy ra khi cập nhật tài khoản admin");
    //        }
    //        return Json(_res);
    //    }

    //    /// <summary>
    //    /// Xóa tài khoản admin
    //    /// </summary>
    //    /// <param name="id">ID tài khoản</param>
    //    /// <returns></returns>
    //    [HttpDelete("{id}")]
    //    [CustomAuthorize]
    //    public async Task<IActionResult> DeleteAdminAccount(long id)
    //    {
    //        try
    //        {
    //            var result = await _adminAccountService.DeleteAdminAccountAsync(id);
    //            if (result.IsSuccess)
    //            {
    //                _res.SuccessEventHandler(true, "Xóa tài khoản admin thành công");
    //            }
    //            else
    //            {
    //                _res.ErrorEventHandler(message: result.Message);
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            _logger.LogError(ex, "Error deleting admin account {Id}", id);
    //            _res.ErrorEventHandler(message: "Có lỗi xảy ra khi xóa tài khoản admin");
    //        }
    //        return Json(_res);
    //    }

    //    /// <summary>
    //    /// Tìm kiếm tài khoản admin
    //    /// </summary>
    //    /// <param name="request">Tham số tìm kiếm</param>
    //    /// <returns></returns>
    //    [HttpPost("search")]
    //    [CustomAuthorize]
    //    public async Task<IActionResult> SearchAdminAccounts([FromBody] SearchAdminAccountRequest request)
    //    {
    //        try
    //        {
    //            var result = await _adminAccountService.SearchAdminAccountsAsync(request);
    //            if (result.IsSuccess)
    //            {
    //                _res.SuccessEventHandler(result.Data);
    //            }
    //            else
    //            {
    //                _res.ErrorEventHandler(message: result.Message);
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            _logger.LogError(ex, "Error searching admin accounts");
    //            _res.ErrorEventHandler(message: "Có lỗi xảy ra khi tìm kiếm admin");
    //        }
    //        return Json(_res);
    //    }

    //    /// <summary>
    //    /// Thay đổi trạng thái tài khoản admin
    //    /// </summary>
    //    /// <param name="id">ID tài khoản</param>
    //    /// <param name="request">Thông tin thay đổi</param>
    //    /// <returns></returns>
    //    [HttpPost("{id}/status")]
    //    [CustomAuthorize]
    //    public async Task<IActionResult> ChangeAdminAccountStatus(long id, [FromBody] ChangeAdminAccountStatusRequest request)
    //    {
    //        try
    //        {
    //            var result = await _adminAccountService.ChangeAdminAccountStatusAsync(id, request);
    //            if (result.IsSuccess)
    //            {
    //                _res.SuccessEventHandler(true, "Thay đổi trạng thái thành công");
    //            }
    //            else
    //            {
    //                _res.ErrorEventHandler(message: result.Message);
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            _logger.LogError(ex, "Error changing admin account status {Id}", id);
    //            _res.ErrorEventHandler(message: "Có lỗi xảy ra khi thay đổi trạng thái");
    //        }
    //        return Json(_res);
    //    }

    //    /// <summary>
    //    /// Phân quyền cho tài khoản admin
    //    /// </summary>
    //    /// <param name="id">ID tài khoản</param>
    //    /// <param name="request">Thông tin phân quyền</param>
    //    /// <returns></returns>
    //    [HttpPost("{id}/permissions")]
    //    [CustomAuthorize]
    //    public async Task<IActionResult> AssignPermissions(long id, [FromBody] AssignPermissionsRequest request)
    //    {
    //        try
    //        {
    //            var result = await _adminAccountService.AssignPermissionsAsync(id, request);
    //            if (result.IsSuccess)
    //            {
    //                _res.SuccessEventHandler(true, "Phân quyền thành công");
    //            }
    //            else
    //            {
    //                _res.ErrorEventHandler(message: result.Message);
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            _logger.LogError(ex, "Error assigning permissions to admin {Id}", id);
    //            _res.ErrorEventHandler(message: "Có lỗi xảy ra khi phân quyền");
    //        }
    //        return Json(_res);
    //    }

    //    /// <summary>
    //    /// Lấy quyền hạn của tài khoản admin
    //    /// </summary>
    //    /// <param name="id">ID tài khoản</param>
    //    /// <returns></returns>
    //    [HttpGet("{id}/permissions")]
    //    [CustomAuthorize]
    //    public async Task<IActionResult> GetAdminPermissions(long id)
    //    {
    //        try
    //        {
    //            var result = await _adminAccountService.GetAdminPermissionsAsync(id);
    //            if (result.IsSuccess)
    //            {
    //                _res.SuccessEventHandler(result.Data);
    //            }
    //            else
    //            {
    //                _res.ErrorEventHandler(message: result.Message);
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            _logger.LogError(ex, "Error getting admin permissions {Id}", id);
    //            _res.ErrorEventHandler(message: "Có lỗi xảy ra khi lấy quyền hạn");
    //        }
    //        return Json(_res);
    //    }

    //    /// <summary>
    //    /// Lấy thống kê tài khoản admin
    //    /// </summary>
    //    /// <returns></returns>
    //    [HttpGet("statistics")]
    //    [CustomAuthorize]
    //    public async Task<IActionResult> GetAdminAccountStatistics()
    //    {
    //        try
    //        {
    //            var result = await _adminAccountService.GetAdminAccountStatisticsAsync();
    //            if (result.IsSuccess)
    //            {
    //                _res.SuccessEventHandler(result.Data);
    //            }
    //            else
    //            {
    //                _res.ErrorEventHandler(message: result.Message);
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            _logger.LogError(ex, "Error getting admin account statistics");
    //            _res.ErrorEventHandler(message: "Có lỗi xảy ra khi lấy thống kê");
    //        }
    //        return Json(_res);
    //    }

    //    /// <summary>
    //    /// Lấy danh sách admin đang online
    //    /// </summary>
    //    /// <returns></returns>
    //    [HttpGet("online")]
    //    [CustomAuthorize]
    //    public async Task<IActionResult> GetOnlineAdmins()
    //    {
    //        try
    //        {
    //            var result = await _adminAccountService.GetOnlineAdminsAsync();
    //            if (result.IsSuccess)
    //            {
    //                _res.SuccessEventHandler(result.Data);
    //            }
    //            else
    //            {
    //                _res.ErrorEventHandler(message: result.Message);
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            _logger.LogError(ex, "Error getting online admins");
    //            _res.ErrorEventHandler(message: "Có lỗi xảy ra khi lấy danh sách admin online");
    //        }
    //        return Json(_res);
    //    }

    //    /// <summary>
    //    /// Đổi mật khẩu admin
    //    /// </summary>
    //    /// <param name="id">ID admin</param>
    //    /// <param name="request">Thông tin đổi mật khẩu</param>
    //    /// <returns></returns>
    //    [HttpPost("{id}/change-password")]
    //    [CustomAuthorize]
    //    public async Task<IActionResult> ChangePassword(long id, [FromBody] ChangeAdminPasswordRequest request)
    //    {
    //        try
    //        {
    //            var result = await _adminAccountService.ChangePasswordAsync(id, request);
    //            if (result.IsSuccess)
    //            {
    //                _res.SuccessEventHandler(true, "Đổi mật khẩu thành công");
    //            }
    //            else
    //            {
    //                _res.ErrorEventHandler(message: result.Message);
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            _logger.LogError(ex, "Error changing password for admin {Id}", id);
    //            _res.ErrorEventHandler(message: "Có lỗi xảy ra khi đổi mật khẩu");
    //        }
    //        return Json(_res);
    //    }

    //    /// <summary>
    //    /// Reset mật khẩu admin
    //    /// </summary>
    //    /// <param name="id">ID admin</param>
    //    /// <param name="request">Thông tin reset</param>
    //    /// <returns></returns>
    //    [HttpPost("{id}/reset-password")]
    //    [CustomAuthorize]
    //    public async Task<IActionResult> ResetPassword(long id, [FromBody] ResetAdminPasswordRequest request)
    //    {
    //        try
    //        {
    //            var result = await _adminAccountService.ResetPasswordAsync(id, request);
    //            if (result.IsSuccess)
    //            {
    //                _res.SuccessEventHandler(true, "Reset mật khẩu thành công");
    //            }
    //            else
    //            {
    //                _res.ErrorEventHandler(message: result.Message);
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            _logger.LogError(ex, "Error resetting password for admin {Id}", id);
    //            _res.ErrorEventHandler(message: "Có lỗi xảy ra khi reset mật khẩu");
    //        }
    //        return Json(_res);
    //    }

    //    /// <summary>
    //    /// Xuất danh sách admin ra Excel
    //    /// </summary>
    //    /// <returns></returns>
    //    [HttpGet("export")]
    //    [CustomAuthorize]
    //    public async Task<IActionResult> ExportAdminAccountsToExcel()
    //    {
    //        try
    //        {
    //            var fileBytes = await _adminAccountService.ExportAdminAccountsToExcelAsync();
    //            var fileName = $"DanhSachAdmin_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
    //            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    //        }
    //        catch (Exception ex)
    //        {
    //            _logger.LogError(ex, "Error exporting admin accounts to Excel");
    //            _res.ErrorEventHandler(message: "Có lỗi xảy ra khi xuất file Excel");
    //            return Json(_res);
    //        }
    //    }
    //}
}
