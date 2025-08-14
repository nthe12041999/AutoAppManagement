using Microsoft.AspNetCore.Mvc;

namespace AutoAppManagement.Controllers
{
    public class DemoController : Controller
    {
        /// <summary>
        /// Trang chủ demo - Tổng quan các demo components
        /// </summary>
        public IActionResult Index()
        {
            ViewData["Title"] = "Demo Components";
            return View();
        }

        /// <summary>
        /// Trang demo grid
        /// </summary>
        public IActionResult Grid()
        {
            ViewData["Title"] = "Demo DataGrid Component";
            return View();
        }

        /// <summary>
        /// Trang demo charts
        /// </summary>
        public IActionResult Charts()
        {
            ViewData["Title"] = "Demo Charts";
            return View();
        }

        /// <summary>
        /// Trang demo Custom Control Filter
        /// </summary>
        public IActionResult FilterDemo()
        {
            ViewData["Title"] = "Demo Custom Control Filter";
            return View();
        }

        /// <summary>
        /// Trang demo Data Attribute Filter
        /// </summary>
        public IActionResult DataAttributeFilter()
        {
            ViewData["Title"] = "Demo Data Attribute Filter";
            return View();
        }



        /// <summary>
        /// API endpoint để lấy demo users data
        /// </summary>
        [HttpGet]
        public IActionResult GetDemoUsers()
        {
            var users = new[]
            {
                new {
                    id = 1,
                    avatar = "NA",
                    name = "Nguyễn Văn An",
                    email = "an.nguyen@company.com",
                    phone = "0901234567",
                    role = "Admin",
                    role_badge = "danger",
                    status = "Hoạt động",
                    status_badge = "success",
                    lastlogin = "2024-08-12T14:30:00"
                },
                new {
                    id = 2,
                    avatar = "TB",
                    name = "Trần Thị Bình",
                    email = "binh.tran@company.com",
                    phone = "0912345678",
                    role = "User",
                    role_badge = "primary",
                    status = "Hoạt động",
                    status_badge = "success",
                    lastlogin = "2024-08-12T13:45:00"
                },
                new {
                    id = 3,
                    avatar = "LC",
                    name = "Lê Minh Cường",
                    email = "cuong.le@company.com",
                    phone = "0923456789",
                    role = "Moderator",
                    role_badge = "info",
                    status = "Không hoạt động",
                    status_badge = "secondary",
                    lastlogin = "2024-08-12T12:00:00"
                },
                new {
                    id = 4,
                    avatar = "PD",
                    name = "Phạm Thị Dung",
                    email = "dung.pham@company.com",
                    phone = "0934567890",
                    role = "User",
                    role_badge = "primary",
                    status = "Hoạt động",
                    status_badge = "success",
                    lastlogin = "2024-08-12T11:15:00"
                },
                new {
                    id = 5,
                    avatar = "HE",
                    name = "Hoàng Văn Em",
                    email = "em.hoang@company.com",
                    phone = "0945678901",
                    role = "User",
                    role_badge = "primary",
                    status = "Bị khóa",
                    status_badge = "danger",
                    lastlogin = "2024-08-11T16:30:00"
                }
            };

            return Json(new { data = users });
        }

        /// <summary>
        /// API endpoint để test AJAX Grid (giữ lại cho tương thích)
        /// </summary>
        [HttpGet]
        public IActionResult GetUsers()
        {
            return GetDemoUsers();
        }

        /// <summary>
        /// Trang demo Auto Filter Component
        /// </summary>
        public IActionResult AutoFilter()
        {
            ViewData["Title"] = "Demo Auto Filter Component";
            return View();
        }

        /// <summary>
        /// Trang test Filter Component
        /// </summary>
        public IActionResult FilterTest()
        {
            ViewData["Title"] = "Filter Test";
            return View();
        }

        /// <summary>
        /// Trang test đơn giản
        /// </summary>
        public IActionResult SimpleTest()
        {
            ViewData["Title"] = "Simple Test";
            return View();
        }

        /// <summary>
        /// Trang debug filter
        /// </summary>
        public IActionResult FilterDebug()
        {
            ViewData["Title"] = "Filter Debug";
            return View();
        }

        /// <summary>
        /// Trang showcase Auto Filter
        /// </summary>
        public IActionResult AutoFilterShowcase()
        {
            ViewData["Title"] = "Auto Filter Showcase";
            return View();
        }

        /// <summary>
        /// Trang demo Grid Filter (Bootstrap Card style - Standard)
        /// </summary>
        public IActionResult GridFilter()
        {
            ViewData["Title"] = "Grid Filter Demo";
            return View();
        }

        /// <summary>
        /// Trang demo Statistics
        /// </summary>
        public IActionResult Statistics()
        {
            ViewData["Title"] = "Statistics Demo";
            return View();
        }

        /// <summary>
        /// Trang demo DataGrid
        /// </summary>
        public IActionResult DataGrid()
        {
            ViewData["Title"] = "DataGrid Demo";
            return View();
        }

        /// <summary>
        /// Trang demo forms
        /// </summary>
        public IActionResult Forms()
        {
            ViewData["Title"] = "Demo Forms";
            return View();
        }

        /// <summary>
        /// Trang demo modals
        /// </summary>
        public IActionResult Modals()
        {
            ViewData["Title"] = "Demo Modals";
            return View();
        }

        /// <summary>
        /// Trang demo UI components
        /// </summary>
        public IActionResult Components()
        {
            ViewData["Title"] = "Demo UI Components";
            return View();
        }

        /// <summary>
        /// Trang chi tiết demo components
        /// </summary>
        public IActionResult DetailDemo(int? userId = null, string? mode = null)
        {
            ViewData["Title"] = "Chi tiết Demo Components";
            
            // Pass parameters to view for demo purposes
            ViewBag.UserId = userId;
            ViewBag.Mode = mode ?? "view";
            
            return View();
        }

        /// <summary>
        /// API lấy dữ liệu demo cho DataGrid component
        /// </summary>
        [HttpPost]
        public IActionResult GetDemoData([FromBody] DataGridRequest request)
        {
            try
            {
                // Tạo dữ liệu mẫu
                var allData = GenerateDemoData();

                // Áp dụng tìm kiếm
                if (!string.IsNullOrEmpty(request.SearchText))
                {
                    allData = allData
                        .Where(x =>
                            x.Name.Contains(request.SearchText, StringComparison.OrdinalIgnoreCase)
                            || x.Email.Contains(
                                request.SearchText,
                                StringComparison.OrdinalIgnoreCase
                            )
                            || x.Department.Contains(
                                request.SearchText,
                                StringComparison.OrdinalIgnoreCase
                            )
                        )
                        .ToList();
                }

                // Áp dụng bộ lọc từ filters object
                if (request.Filters != null)
                {
                    if (!string.IsNullOrEmpty(request.Filters.StatusFilter))
                    {
                        allData = allData
                            .Where(x => x.Status == request.Filters.StatusFilter)
                            .ToList();
                    }

                    if (!string.IsNullOrEmpty(request.Filters.DepartmentFilter))
                    {
                        allData = allData
                            .Where(x => x.Department == request.Filters.DepartmentFilter)
                            .ToList();
                    }
                }

                // Sắp xếp
                if (!string.IsNullOrEmpty(request.SortBy))
                {
                    allData = ApplySorting(allData, request.SortBy, request.SortDirection);
                }

                // Tính toán phân trang
                var totalRecords = allData.Count;
                var totalPages = (int)Math.Ceiling((double)totalRecords / request.PageSize);

                var pagedData = allData
                    .Skip((request.Page - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .Select(x => new
                    {
                        id = x.Id,
                        name = x.Name,
                        email = x.Email,
                        phone = x.Phone,
                        department = x.Department,
                        position = x.Position,
                        status = x.Status,
                        joinDate = x.JoinDate.ToString("yyyy-MM-dd"),
                        joinDateFormatted = x.JoinDate.ToString("dd/MM/yyyy"),
                        salary = x.Salary,
                        salaryFormatted = x.Salary.ToString("N0") + " VNĐ",
                        score = x.Score,
                        isActive = x.IsActive,
                        // Additional fields for display
                        avatar = $"/images/avatars/default-{(x.Id % 5) + 1}.png",
                        statusBadge = GetStatusBadgeClass(x.Status),
                        departmentBadge = GetDepartmentBadgeClass(x.Department),
                    })
                    .ToList();

                return Json(
                    new
                    {
                        success = true,
                        data = pagedData,
                        totalRecords = totalRecords,
                        totalPages = totalPages,
                        currentPage = request.Page,
                        pageSize = request.PageSize,
                    }
                );
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }

        /// <summary>
        /// Áp dụng sắp xếp
        /// </summary>
        private List<DemoItem> ApplySorting(
            List<DemoItem> data,
            string sortBy,
            string sortDirection
        )
        {
            var isDescending = sortDirection?.ToLower() == "desc";

            return sortBy?.ToLower() switch
            {
                "id" => isDescending
                    ? data.OrderByDescending(x => x.Id).ToList()
                    : data.OrderBy(x => x.Id).ToList(),
                "name" => isDescending
                    ? data.OrderByDescending(x => x.Name).ToList()
                    : data.OrderBy(x => x.Name).ToList(),
                "email" => isDescending
                    ? data.OrderByDescending(x => x.Email).ToList()
                    : data.OrderBy(x => x.Email).ToList(),
                "department" => isDescending
                    ? data.OrderByDescending(x => x.Department).ToList()
                    : data.OrderBy(x => x.Department).ToList(),
                "status" => isDescending
                    ? data.OrderByDescending(x => x.Status).ToList()
                    : data.OrderBy(x => x.Status).ToList(),
                "joindate" => isDescending
                    ? data.OrderByDescending(x => x.JoinDate).ToList()
                    : data.OrderBy(x => x.JoinDate).ToList(),
                "salary" => isDescending
                    ? data.OrderByDescending(x => x.Salary).ToList()
                    : data.OrderBy(x => x.Salary).ToList(),
                "score" => isDescending
                    ? data.OrderByDescending(x => x.Score).ToList()
                    : data.OrderBy(x => x.Score).ToList(),
                _ => data.OrderBy(x => x.Id).ToList(),
            };
        }

        /// <summary>
        /// Lấy class badge cho trạng thái
        /// </summary>
        private string GetStatusBadgeClass(string status)
        {
            return status switch
            {
                "Active" => "success",
                "Inactive" => "secondary",
                "Pending" => "warning",
                "Suspended" => "danger",
                _ => "light",
            };
        }

        /// <summary>
        /// Lấy class badge cho phòng ban
        /// </summary>
        private string GetDepartmentBadgeClass(string department)
        {
            return department switch
            {
                "IT" => "primary",
                "Marketing" => "success",
                "Sales" => "warning",
                "HR" => "info",
                "Finance" => "danger",
                "Operations" => "secondary",
                _ => "light",
            };
        }

        /// <summary>
        /// Tạo dữ liệu mẫu
        /// </summary>
        private List<DemoItem> GenerateDemoData()
        {
            var random = new Random();
            var departments = new[] { "IT", "Marketing", "Sales", "HR", "Finance", "Operations" };
            var statuses = new[] { "Active", "Inactive", "Pending", "Suspended" };
            var positions = new[]
            {
                "Manager",
                "Developer",
                "Analyst",
                "Specialist",
                "Coordinator",
                "Assistant",
            };

            var data = new List<DemoItem>();

            for (int i = 1; i <= 150; i++)
            {
                data.Add(
                    new DemoItem
                    {
                        Id = i,
                        Name = $"Người dùng {i:D3}",
                        Email = $"user{i:D3}@company.com",
                        Phone = $"090{random.Next(1000000, 9999999)}",
                        Department = departments[random.Next(departments.Length)],
                        Position = positions[random.Next(positions.Length)],
                        Status = statuses[random.Next(statuses.Length)],
                        JoinDate = DateTime.Now.AddDays(-random.Next(1, 1000)),
                        Salary = random.Next(10, 50) * 1000000,
                        IsActive = random.Next(0, 2) == 1,
                        Score = random.Next(60, 100),
                    }
                );
            }

            return data;
        }
    }

    /// <summary>
    /// Model cho DataGrid request
    /// </summary>
    public class DataGridRequest
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SearchText { get; set; } = "";
        public string SortBy { get; set; } = "Id";
        public string SortDirection { get; set; } = "asc";
        public DataGridFilters? Filters { get; set; }
    }

    /// <summary>
    /// Model cho filters
    /// </summary>
    public class DataGridFilters
    {
        public string StatusFilter { get; set; } = "";
        public string DepartmentFilter { get; set; } = "";
        public string RoleFilter { get; set; } = "";
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
    }

    /// <summary>
    /// Model cho dữ liệu demo
    /// </summary>
    public class DemoItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Department { get; set; } = "";
        public string Position { get; set; } = "";
        public string Status { get; set; } = "";
        public DateTime JoinDate { get; set; }
        public decimal Salary { get; set; }
        public bool IsActive { get; set; }
        public int Score { get; set; }
    }
}
