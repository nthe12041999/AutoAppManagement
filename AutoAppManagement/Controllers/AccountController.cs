using AutoAppManagement.Models.DTO.Account;
using AutoAppManagement.WebApp.Controllers.Base;
using AutoAppManagement.WebApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace AutoAppManagement.WebApp.Controllers
{
    public class AccountController : BaseBusinessController<IAccountService, AccountDTO>
    {
        public AccountController(IServiceProvider serviceProvider) : base(serviceProvider) { }

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
                Logger.LogError(ex, "Error loading customer accounts page");
                return View("Error");
            }
        }

        /// <summary>
        /// Modal form để thêm/sửa (được gọi từ DataGrid)
        /// </summary>
        /// <returns></returns>
        public IActionResult CustomerForms(string mode = "add", string entity = "Customer")
        {
            ViewBag.Mode = mode;
            ViewBag.Entity = entity;
            return PartialView();
        }

        /// <summary>
        /// API: Lấy thông tin chi tiết tài khoản theo ID (for data binding)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetById(long id)
        {
            try
            {
                // Mock data for testing - replace with actual service call later
                var mockData = new
                {
                    id = id,
                    name = id switch
                    {
                        1 => "Nguyễn Văn An",
                        2 => "Trần Thị Bình",
                        3 => "Lê Hoàng Cường",
                        4 => "Phạm Minh Dũng",
                        5 => "Hoàng Thị Hương",
                        _ => $"Khách hàng {id}"
                    },
                    email = $"customer{id}@gmail.com",
                    phone = id switch
                    {
                        1 => "0912 345 678",
                        2 => "0987 654 321", 
                        3 => "0901 234 567",
                        4 => "0933 888 999",
                        5 => "0966 777 555",
                        _ => $"090{id:D7}"
                    },
                    dateOfBirth = id switch
                    {
                        1 => "1990-05-15",
                        2 => "1985-08-22",
                        3 => "1992-12-30",
                        4 => "1988-03-10",
                        5 => "1995-11-05",
                        _ => "1990-01-01"
                    },
                    gender = id switch
                    {
                        1 => "1", // Nam
                        2 => "2", // Nữ  
                        3 => "1", // Nam
                        4 => "1", // Nam
                        5 => "2", // Nữ
                        _ => "0"  // Khác
                    },
                    address = id switch
                    {
                        1 => "123 Nguyễn Huệ, Quận 1, TP.HCM",
                        2 => "456 Lý Thường Kiệt, Quận 10, TP.HCM", 
                        3 => "789 Võ Văn Tần, Quận 3, TP.HCM",
                        4 => "321 Lê Lợi, Quận 1, TP.HCM",
                        5 => "654 Trần Hưng Đạo, Quận 5, TP.HCM",
                        _ => $"{id} Đường số {id}, Quận {id % 12 + 1}, TP.HCM"
                    },
                    notes = id switch
                    {
                        1 => "Khách hàng VIP, đã mua hàng nhiều lần, rất tin tưởng công ty",
                        2 => "Khách hàng thân thiết, thích mua hàng online, hay giới thiệu bạn bè",
                        3 => "Khách hàng mới, cần chăm sóc đặc biệt, tiềm năng phát triển cao",
                        4 => "Khách hàng doanh nghiệp, có nhu cầu mua số lượng lớn",
                        5 => "Khách hàng cá nhân, thích sản phẩm chất lượng cao",
                        _ => $"Ghi chú cho khách hàng {id} - Cần theo dõi và chăm sóc"
                    },
                    imgAvatar = id switch
                    {
                        1 => "https://i.pravatar.cc/150?img=1",
                        2 => "https://i.pravatar.cc/150?img=2", 
                        3 => "https://i.pravatar.cc/150?img=3",
                        4 => "https://i.pravatar.cc/150?img=4",
                        5 => "https://i.pravatar.cc/150?img=5",
                        _ => $"https://i.pravatar.cc/150?img={id % 20 + 1}"
                    },
                    isVerified = id % 2 == 1,
                    status = id switch
                    {
                        1 => "active",
                        2 => "active", 
                        3 => "pending",
                        4 => "active",
                        5 => "inactive",
                        _ => (id % 3) switch { 0 => "inactive", 1 => "active", _ => "pending" }
                    },
                    role = id switch
                    {
                        1 => "vip",
                        2 => "premium",
                        3 => "customer", 
                        4 => "premium",
                        5 => "trial",
                        _ => (id % 4) switch { 0 => "trial", 1 => "customer", 2 => "premium", _ => "vip" }
                    },
                    lastLogin = DateTime.Now.AddHours(-id),
                    createdDate = DateTime.Now.AddDays(-id * 30),
                    level = id % 3 + 1,
                    maxAccountFb = id * 5,
                    registeredDate = DateTime.Now.AddDays(-id * 30),
                    expiredDate = DateTime.Now.AddDays(365 - id * 10)
                };

                return Json(new { success = true, data = mockData });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error getting account by ID: {Id}", id);
                return Json(new { success = false, message = "Có lỗi xảy ra khi lấy dữ liệu" });
            }
        }

        /// <summary>
        /// API: Lấy danh sách tài khoản với phân trang (for DataGrid)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetPaging(int page = 1, int pageSize = 10, string filter = "")
        {
            try
            {
                // Mock data for now - replace with actual service call later
                var totalCount = 50;
                var data = new List<object>();

                // Generate mock data
                var startIndex = (page - 1) * pageSize + 1;
                var endIndex = Math.Min(startIndex + pageSize - 1, totalCount);

                for (int i = startIndex; i <= endIndex; i++)
                {
                    data.Add(new
                    {
                        id = i,
                        avatar = $"<img src='https://i.pravatar.cc/40?img={i}' class='rounded-circle' width='40'>",
                        name = i switch
                        {
                            1 => "Nguyễn Văn An",
                            2 => "Trần Thị Bình", 
                            3 => "Lê Hoàng Cường",
                            _ => $"Khách hàng {i}"
                        },
                        email = $"customer{i}@gmail.com",
                        phone = $"090{i:D7}",
                        role = (i % 4) switch
                        {
                            0 => "trial",
                            1 => "customer", 
                            2 => "premium",
                            _ => "vip"
                        },
                        status = (i % 3) switch
                        {
                            0 => "inactive",
                            1 => "active",
                            _ => "pending"
                        },
                        lastLogin = DateTime.Now.AddHours(-i * 2),
                        createdDate = DateTime.Now.AddDays(-i * 5),
                        isVerified = i % 2 == 1
                    });
                }

                // Apply filter if provided
                if (!string.IsNullOrEmpty(filter))
                {
                    var filterLower = filter.ToLower();
                    data = data.Where(d =>
                    {
                        var item = (dynamic)d;
                        return item.name.ToString().ToLower().Contains(filterLower) ||
                               item.email.ToString().ToLower().Contains(filterLower) ||
                               item.phone.ToString().Contains(filter);
                    }).ToList();
                }

                return Json(new
                {
                    success = true,
                    data = new
                    {
                        data = data,
                        currentPage = page,
                        pageSize = pageSize,
                        totalCount = totalCount,
                        totalPages = (int)Math.Ceiling((double)totalCount / pageSize)
                    }
                });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error getting paged accounts");
                return Json(new { success = false, message = "Có lỗi xảy ra khi lấy dữ liệu" });
            }
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
                    ResOutput.SuccessEventHandler(true, "Thay đổi trạng thái thành công");
                }
                else
                {
                    ResOutput.ErrorEventHandler(message: result.Message);
                }
                */

                // TEMPORARY: Mock status toggle
                // Simulate customer not found
                if (id == 999)
                {
                    ResOutput.ErrorEventHandler(message: "Không tìm thấy khách hàng");
                    return Json(ResOutput);
                }

                // Mock successful status toggle
                var newStatus = id == 5 ? "Active" : "Locked"; // Toggle status
                var message = newStatus == "Locked" ? "Đã khóa tài khoản khách hàng" : "Đã mở khóa tài khoản khách hàng";

                ResOutput.SuccessEventHandler(new { Id = id, Status = newStatus }, message);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error toggling customer status {Id}", id);
                ResOutput.ErrorEventHandler(message: "Có lỗi xảy ra khi thay đổi trạng thái");
            }
            return Json(ResOutput);
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
                ResOutput.SuccessEventHandler(statistics);
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

                ResOutput.SuccessEventHandler(statistics);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error getting customer account statistics");
                ResOutput.ErrorEventHandler(message: "Có lỗi xảy ra khi tải thống kê");
            }
            return Json(ResOutput);
        }
    }
}
