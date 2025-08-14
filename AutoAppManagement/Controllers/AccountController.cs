using AutoAppManagement.Models.ViewModel;
using AutoAppManagement.WebApp.Controllers.Base;
using AutoAppManagement.WebApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace AutoAppManagement.WebApp.Controllers
{
    public class AccountController : BaseController
    {
        private readonly ILogger<AccountController> _logger;
        private readonly IAdminService _adminService;

        public AccountController(
            RestOutput res,
            IAdminService adminService,
            ILogger<AccountController> logger,
            IHttpContextAccessor httpContextAccessor
        )
            : base(res)
        {
            _adminService = adminService;
            _logger = logger;
        }

        /// <summary>
        /// Trang danh sách tài khoản khách hàng
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
        {
            try
            {
                ViewData["Title"] = "Danh sách tài khoản khách hàng";
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading customer accounts page");
                return View("Error");
            }
        }

        /// <summary>
        /// Modal form để thêm/sửa (được gọi từ DataGrid)
        /// </summary>
        /// <returns></returns>
        public IActionResult CustomerForms()
        {
            return View();
        }

        /// <summary>
        /// API: Lấy danh sách tài khoản khách hàng
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetCustomerAccounts()
        {
            try
            {
                // Mock data từ bảng Account cho Customer Accounts
                var mockAccounts = new
                {
                    data = new[]
                    {
                        new
                        {
                            Id = 1,
                            UserName = "customer01",
                            Name = "Nguyễn Văn Khách",
                            Email = "customer01@gmail.com",
                            Phone = "0901234567",
                            Level = 1,
                            Gender = "Male",
                            DateOfBirth = new DateTime(1990, 5, 15),
                            RegisterDate = DateTime.Now.AddDays(-60),
                            ExpiredDate = DateTime.Now.AddDays(30),
                            CreatedDate = DateTime.Now.AddDays(-60),
                            Language = "vi",
                            IsLocked = false,
                            ImgAvatar = "/images/avatars/customer01.jpg",
                            MaxAccountFb = 10,
                            Status = "Active",
                            Role = "Customer",
                            OnlineStatus = "Online",
                            LastLogin = DateTime.Now.AddHours(-2),
                            TotalLicenses = 3,
                            ActiveLicenses = 2,
                            TotalDevices = 2
                        },
                        new
                        {
                            Id = 2,
                            UserName = "premium_user",
                            Name = "Trần Thị Premium",
                            Email = "premium@gmail.com",
                            Phone = "0912345678",
                            Level = 2,
                            Gender = "Female",
                            DateOfBirth = new DateTime(1985, 8, 20),
                            RegisterDate = DateTime.Now.AddDays(-45),
                            ExpiredDate = DateTime.Now.AddDays(60),
                            CreatedDate = DateTime.Now.AddDays(-45),
                            Language = "vi",
                            IsLocked = false,
                            ImgAvatar = "/images/avatars/premium.jpg",
                            MaxAccountFb = 25,
                            Status = "Active",
                            Role = "Premium",
                            OnlineStatus = "Away",
                            LastLogin = DateTime.Now.AddHours(-5),
                            TotalLicenses = 8,
                            ActiveLicenses = 7,
                            TotalDevices = 4
                        },
                        new
                        {
                            Id = 3,
                            UserName = "vip_customer",
                            Name = "Lê Văn VIP",
                            Email = "vip@gmail.com",
                            Phone = "0923456789",
                            Level = 3,
                            Gender = "Male",
                            DateOfBirth = new DateTime(1988, 12, 10),
                            RegisterDate = DateTime.Now.AddDays(-30),
                            ExpiredDate = DateTime.Now.AddDays(90),
                            CreatedDate = DateTime.Now.AddDays(-30),
                            Language = "en",
                            IsLocked = false,
                            ImgAvatar = "/images/avatars/vip.jpg",
                            MaxAccountFb = 50,
                            Status = "Active",
                            Role = "VIP",
                            OnlineStatus = "Online",
                            LastLogin = DateTime.Now.AddMinutes(-30),
                            TotalLicenses = 15,
                            ActiveLicenses = 12,
                            TotalDevices = 8
                        },
                        new
                        {
                            Id = 4,
                            UserName = "inactive_user",
                            Name = "Phạm Thị Inactive",
                            Email = "inactive@gmail.com",
                            Phone = "0934567890",
                            Level = 1,
                            Gender = "Female",
                            DateOfBirth = new DateTime(1992, 3, 25),
                            RegisterDate = DateTime.Now.AddDays(-90),
                            ExpiredDate = DateTime.Now.AddDays(-10),
                            CreatedDate = DateTime.Now.AddDays(-90),
                            Language = "vi",
                            IsLocked = false,
                            ImgAvatar = "/images/avatars/inactive.jpg",
                            MaxAccountFb = 5,
                            Status = "Expired",
                            Role = "Customer",
                            OnlineStatus = "Offline",
                            LastLogin = DateTime.Now.AddDays(-15),
                            TotalLicenses = 1,
                            ActiveLicenses = 0,
                            TotalDevices = 1
                        },
                        new
                        {
                            Id = 5,
                            UserName = "locked_user",
                            Name = "Hoàng Văn Locked",
                            Email = "locked@gmail.com",
                            Phone = "0945678901",
                            Level = 1,
                            Gender = "Male",
                            DateOfBirth = new DateTime(1995, 7, 8),
                            RegisterDate = DateTime.Now.AddDays(-20),
                            ExpiredDate = DateTime.Now.AddDays(40),
                            CreatedDate = DateTime.Now.AddDays(-20),
                            Language = "vi",
                            IsLocked = true,
                            ImgAvatar = "/images/avatars/locked.jpg",
                            MaxAccountFb = 10,
                            Status = "Locked",
                            Role = "Customer",
                            OnlineStatus = "Offline",
                            LastLogin = DateTime.Now.AddDays(-5),
                            TotalLicenses = 2,
                            ActiveLicenses = 0,
                            TotalDevices = 1
                        }
                    },
                    total = 5,
                    page = 1,
                    pageSize = 10
                };

                _res.SuccessEventHandler(mockAccounts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting customer accounts");
                _res.ErrorEventHandler(message: "Có lỗi xảy ra khi tải dữ liệu");
            }
            return Json(_res);
        }

        /// <summary>
        /// API: Lấy tài khoản khách hàng theo ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetCustomerAccount(long id)
        {
            try
            {
                // TODO: Uncomment when service is ready
                /*
                var account = await _customerAccountService.GetCustomerAccountByIdAsync(id);
                if (account == null)
                {
                    _res.ErrorEventHandler(message: "Không tìm thấy tài khoản");
                }
                else
                {
                    _res.SuccessEventHandler(account);
                }
                */

                // TEMPORARY: Mock single customer data based on ID
                var customer = new
                {
                    Id = id,
                    UserName = $"customer{id:D2}",
                    Name = id switch
                    {
                        1 => "Nguyễn Văn Khách",
                        2 => "Trần Thị Premium",
                        3 => "Lê Văn VIP",
                        4 => "Phạm Thị Inactive",
                        5 => "Hoàng Văn Locked",
                        _ => $"Khách hàng {id}"
                    },
                    Email = $"customer{id:D2}@gmail.com",
                    Phone = $"090123456{id}",
                    Level = id switch
                    {
                        1 => 1,
                        2 => 2,
                        3 => 3,
                        4 => 1,
                        5 => 1,
                        _ => 1
                    },
                    Gender = id % 2 == 0 ? "Female" : "Male",
                    DateOfBirth = DateTime.Now.AddYears(-25 - (int)id),
                    RegisterDate = DateTime.Now.AddDays(-60 + (int)id * 5),
                    ExpiredDate = DateTime.Now.AddDays(30 + (int)id * 10),
                    CreatedDate = DateTime.Now.AddDays(-60 + (int)id * 5),
                    Language = id == 3 ? "en" : "vi",
                    IsLocked = id == 5,
                    ImgAvatar = $"/images/avatars/customer{id:D2}.jpg",
                    MaxAccountFb = id switch
                    {
                        1 => 10,
                        2 => 25,
                        3 => 50,
                        4 => 5,
                        5 => 10,
                        _ => 10
                    },
                    Status = id switch
                    {
                        1 => "Active",
                        2 => "Active",
                        3 => "Active",
                        4 => "Expired",
                        5 => "Locked",
                        _ => "Active"
                    },
                    Role = id switch
                    {
                        1 => "Customer",
                        2 => "Premium",
                        3 => "VIP",
                        4 => "Customer",
                        5 => "Customer",
                        _ => "Customer"
                    },
                    OnlineStatus = id <= 2 ? "Online" : id == 3 ? "Away" : "Offline",
                    LastLogin = DateTime.Now.AddHours(-(int)id),
                    TotalLicenses = (int)id * 2 + 1,
                    ActiveLicenses = id <= 3 ? (int)id * 2 : 0,
                    TotalDevices = id <= 3 ? (int)id + 1 : 1
                };

                if (id > 5 && id != 999) // 999 is used for testing "not found"
                {
                    _res.ErrorEventHandler(message: "Không tìm thấy tài khoản");
                }
                else
                {
                    _res.SuccessEventHandler(customer);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting customer account {Id}", id);
                _res.ErrorEventHandler(message: "Có lỗi xảy ra khi tải dữ liệu");
            }
            return Json(_res);
        }

        /// <summary>
        /// API: Tạo tài khoản khách hàng mới
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateCustomerAccount([FromBody] dynamic model)
        {
            try
            {
                // TODO: Uncomment when service is ready
                /*
                if (!ModelState.IsValid)
                {
                    _res.ErrorEventHandler(message: "Dữ liệu không hợp lệ");
                    return Json(_res);
                }

                var result = await _customerAccountService.CreateCustomerAccountAsync(model);
                if (result.IsSuccess)
                {
                    _res.SuccessEventHandler(result.Data, "Tạo tài khoản thành công");
                }
                else
                {
                    _res.ErrorEventHandler(message: result.Message);
                }
                */

                // TEMPORARY: Mock data creation
                // Extract data from dynamic model
                string name = model?.Name?.ToString() ?? "";
                string email = model?.Email?.ToString() ?? "";
                string userName = model?.UserName?.ToString() ?? "";
                string password = model?.Password?.ToString() ?? "";
                string phone = model?.Phone?.ToString() ?? "";
                int level = model?.Level != null ? (int)model.Level : 1;
                string gender = model?.Gender?.ToString() ?? "Male";
                string language = model?.Language?.ToString() ?? "vi";
                bool isLocked = model?.IsLocked != null ? (bool)model.IsLocked : false;
                int maxAccountFb = model?.MaxAccountFb != null ? (int)model.MaxAccountFb : 10;

                // Validate required fields
                if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email) ||
                    string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
                {
                    _res.ErrorEventHandler(message: "Vui lòng điền đầy đủ thông tin bắt buộc");
                    return Json(_res);
                }

                // Simulate email/username uniqueness check
                if (email.Contains("existing"))
                {
                    _res.ErrorEventHandler(message: "Email đã tồn tại trong hệ thống");
                    return Json(_res);
                }

                if (userName.Contains("existing"))
                {
                    _res.ErrorEventHandler(message: "Tên đăng nhập đã tồn tại trong hệ thống");
                    return Json(_res);
                }

                // Mock successful creation
                var newCustomer = new
                {
                    Id = new Random().Next(100, 999),
                    Name = name,
                    UserName = userName,
                    Email = email,
                    Phone = phone,
                    Level = level,
                    Gender = gender,
                    DateOfBirth = model?.DateOfBirth,
                    RegisterDate = model?.RegisterDate ?? DateTime.Now,
                    ExpiredDate = model?.ExpiredDate ?? DateTime.Now.AddYears(1),
                    Language = language,
                    IsLocked = isLocked,
                    MaxAccountFb = maxAccountFb,
                    Status = isLocked ? "Locked" : "Active",
                    Role = level switch
                    {
                        1 => "Customer",
                        2 => "Premium",
                        3 => "VIP",
                        _ => "Customer"
                    },
                    CreatedDate = DateTime.Now,
                    OnlineStatus = "Offline",
                    TotalLicenses = 0,
                    ActiveLicenses = 0,
                    TotalDevices = 0
                };

                _res.SuccessEventHandler(newCustomer, "Tạo khách hàng thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating customer account");
                _res.ErrorEventHandler(message: "Có lỗi xảy ra khi tạo khách hàng");
            }
            return Json(_res);
        }

        /// <summary>
        /// API: Cập nhật tài khoản khách hàng
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateCustomerAccount([FromBody] dynamic model)
        {
            try
            {
                // TODO: Uncomment when service is ready
                /*
                if (!ModelState.IsValid)
                {
                    _res.ErrorEventHandler(message: "Dữ liệu không hợp lệ");
                    return Json(_res);
                }

                var result = await _customerAccountService.UpdateCustomerAccountAsync(id, model);
                if (result.IsSuccess)
                {
                    _res.SuccessEventHandler(result.Data, "Cập nhật tài khoản thành công");
                }
                else
                {
                    _res.ErrorEventHandler(message: result.Message);
                }
                */

                // TEMPORARY: Mock data update
                // Extract data from dynamic model
                long id = model?.Id != null ? (long)model.Id : 0;
                string name = model?.Name?.ToString() ?? "";
                string email = model?.Email?.ToString() ?? "";
                string userName = model?.UserName?.ToString() ?? "";
                string phone = model?.Phone?.ToString() ?? "";
                int level = model?.Level != null ? (int)model.Level : 1;
                string gender = model?.Gender?.ToString() ?? "Male";
                string language = model?.Language?.ToString() ?? "vi";
                bool isLocked = model?.IsLocked != null ? (bool)model.IsLocked : false;
                int maxAccountFb = model?.MaxAccountFb != null ? (int)model.MaxAccountFb : 10;

                // Validate required fields
                if (id <= 0 || string.IsNullOrEmpty(name) ||
                    string.IsNullOrEmpty(email) || string.IsNullOrEmpty(userName))
                {
                    _res.ErrorEventHandler(message: "Thông tin không hợp lệ");
                    return Json(_res);
                }

                // Simulate customer not found
                if (id == 999)
                {
                    _res.ErrorEventHandler(message: "Không tìm thấy khách hàng");
                    return Json(_res);
                }

                // Mock successful update
                var updatedCustomer = new
                {
                    Id = id,
                    Name = name,
                    UserName = userName,
                    Email = email,
                    Phone = phone,
                    Level = level,
                    Gender = gender,
                    DateOfBirth = model?.DateOfBirth,
                    RegisterDate = model?.RegisterDate,
                    ExpiredDate = model?.ExpiredDate,
                    Language = language,
                    IsLocked = isLocked,
                    MaxAccountFb = maxAccountFb,
                    Status = isLocked ? "Locked" : "Active",
                    Role = level switch
                    {
                        1 => "Customer",
                        2 => "Premium",
                        3 => "VIP",
                        _ => "Customer"
                    },
                    UpdatedDate = DateTime.Now,
                    OnlineStatus = isLocked ? "Offline" : "Online"
                };

                _res.SuccessEventHandler(updatedCustomer, "Cập nhật khách hàng thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating customer account");
                _res.ErrorEventHandler(message: "Có lỗi xảy ra khi cập nhật khách hàng");
            }
            return Json(_res);
        }

        /// <summary>
        /// API: Xóa tài khoản khách hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> DeleteCustomerAccount(long id)
        {
            try
            {
                // TODO: Uncomment when service is ready
                /*
                var result = await _customerAccountService.DeleteCustomerAccountAsync(id);
                if (result.IsSuccess)
                {
                    _res.SuccessEventHandler(true, "Xóa tài khoản thành công");
                }
                else
                {
                    _res.ErrorEventHandler(message: result.Message);
                }
                */

                // TEMPORARY: Mock deletion
                // Simulate customer not found
                if (id == 999)
                {
                    _res.ErrorEventHandler(message: "Không tìm thấy khách hàng");
                    return Json(_res);
                }

                // Simulate cannot delete admin account
                if (id == 1)
                {
                    _res.ErrorEventHandler(message: "Không thể xóa tài khoản admin chính");
                    return Json(_res);
                }

                // Mock successful deletion
                _res.SuccessEventHandler(true, "Xóa khách hàng thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting customer account {Id}", id);
                _res.ErrorEventHandler(message: "Có lỗi xảy ra khi xóa khách hàng");
            }
            return Json(_res);
        }

        /// <summary>
        /// API: Tìm kiếm tài khoản khách hàng
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="status"></param>
        /// <param name="role"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> SearchCustomerAccounts(string keyword = "", string status = "", string role = "", int pageIndex = 1, int pageSize = 10)
        {
            try
            {
                // TODO: Uncomment when service is ready
                /*
                var result = await _customerAccountService.SearchCustomerAccountsAsync(keyword, status, role, pageIndex, pageSize);
                _res.SuccessEventHandler(result);
                */

                // TEMPORARY: Mock search results
                // This would normally filter the GetCustomerAccounts data based on search criteria
                // For now, just return the same mock data as GetCustomerAccounts
                await Task.Delay(100); // Simulate search delay

                var searchResults = new
                {
                    data = new[]
                    {
                        new
                        {
                            Id = 1,
                            UserName = "customer01",
                            Name = "Nguyễn Văn Khách",
                            Email = "customer01@gmail.com",
                            Phone = "0901234567",
                            Level = 1,
                            Role = "Customer",
                            Status = "Active",
                            OnlineStatus = "Online"
                        }
                    },
                    totalCount = 1,
                    pageIndex = pageIndex,
                    pageSize = pageSize,
                    keyword = keyword,
                    status = status,
                    role = role
                };

                _res.SuccessEventHandler(searchResults);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching customer accounts");
                _res.ErrorEventHandler(message: "Có lỗi xảy ra khi tìm kiếm");
            }
            return Json(_res);
        }

        /// <summary>
        /// API: Thay đổi trạng thái tài khoản khách hàng
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ToggleCustomerStatus(long id)
        {
            try
            {
                // TODO: Uncomment when service is ready
                /*
                var result = await _customerAccountService.ChangeCustomerAccountStatusAsync(id, status);
                if (result.IsSuccess)
                {
                    _res.SuccessEventHandler(true, "Thay đổi trạng thái thành công");
                }
                else
                {
                    _res.ErrorEventHandler(message: result.Message);
                }
                */

                // TEMPORARY: Mock status toggle
                // Simulate customer not found
                if (id == 999)
                {
                    _res.ErrorEventHandler(message: "Không tìm thấy khách hàng");
                    return Json(_res);
                }

                // Mock successful status toggle
                var newStatus = id == 5 ? "Active" : "Locked"; // Toggle status
                var message = newStatus == "Locked" ? "Đã khóa tài khoản khách hàng" : "Đã mở khóa tài khoản khách hàng";

                _res.SuccessEventHandler(new { Id = id, Status = newStatus }, message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling customer status {Id}", id);
                _res.ErrorEventHandler(message: "Có lỗi xảy ra khi thay đổi trạng thái");
            }
            return Json(_res);
        }

        /// <summary>
        /// API: Lấy thống kê tài khoản khách hàng
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetCustomerAccountStatistics()
        {
            try
            {
                // TODO: Uncomment when service is ready
                /*
                var statistics = await _customerAccountService.GetCustomerAccountStatisticsAsync();
                _res.SuccessEventHandler(statistics);
                */

                // TEMPORARY: Mock statistics data
                var statistics = new
                {
                    TotalCustomers = 5,
                    ActiveCustomers = 3,
                    PremiumCustomers = 2,
                    OnlineCustomers = 2,
                    LockedCustomers = 1,
                    ExpiredCustomers = 1,
                    NewCustomersThisMonth = 2,
                    TotalRevenue = 15000000, // 15 triệu VND
                    AverageLoginPerDay = 25,
                    TopCustomersByLevel = new[]
                    {
                        new { Level = "VIP", Count = 1, Percentage = 20 },
                        new { Level = "Premium", Count = 1, Percentage = 20 },
                        new { Level = "Customer", Count = 3, Percentage = 60 }
                    },
                    RecentActivities = new[]
                    {
                        new { Action = "Login", CustomerName = "Nguyễn Văn Khách", Time = DateTime.Now.AddMinutes(-15) },
                        new { Action = "Register", CustomerName = "Trần Thị Premium", Time = DateTime.Now.AddHours(-2) },
                        new { Action = "Upgrade", CustomerName = "Lê Văn VIP", Time = DateTime.Now.AddHours(-5) }
                    }
                };

                _res.SuccessEventHandler(statistics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting customer account statistics");
                _res.ErrorEventHandler(message: "Có lỗi xảy ra khi tải thống kê");
            }
            return Json(_res);
        }

        /// <summary>
        /// API: Xuất danh sách tài khoản khách hàng ra Excel
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> ExportCustomerAccountsToExcel()
        {
            try
            {
                // TODO: Uncomment when service is ready
                /*
                var fileBytes = await _customerAccountService.ExportCustomerAccountsToExcelAsync();
                var fileName = $"DanhSachTaiKhoanKhachHang_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                */

                // TEMPORARY: Mock Excel export - In real implementation, generate actual Excel file
                var fileName = $"DanhSachTaiKhoanKhachHang_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                // Simulate file generation delay
                await Task.Delay(1000);

                // Create a simple CSV content as mock
                var csvContent = "ID,Tên,Email,Số điện thoại,Vai trò,Trạng thái,Ngày tạo\n" +
                               "1,Nguyễn Văn Khách,customer01@gmail.com,0901234567,Customer,Active,2024-01-01\n" +
                               "2,Trần Thị Premium,premium@gmail.com,0912345678,Premium,Active,2024-01-02\n" +
                               "3,Lê Văn VIP,vip@gmail.com,0923456789,VIP,Active,2024-01-03\n" +
                               "4,Phạm Thị Inactive,inactive@gmail.com,0934567890,Customer,Expired,2024-01-04\n" +
                               "5,Hoàng Văn Locked,locked@gmail.com,0945678901,Customer,Locked,2024-01-05";

                var bytes = System.Text.Encoding.UTF8.GetBytes(csvContent);

                // Return as CSV file (mock Excel)
                return File(bytes, "text/csv", fileName.Replace(".xlsx", ".csv"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting customer accounts to Excel");
                _res.ErrorEventHandler(message: "Có lỗi xảy ra khi xuất file Excel");
                return Json(_res);
            }
        }
    }
}
