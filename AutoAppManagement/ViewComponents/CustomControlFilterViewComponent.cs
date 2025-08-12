using AutoAppManagement.Models.Components;
using Microsoft.AspNetCore.Mvc;

namespace AutoAppManagement.ViewComponents
{
    /// <summary>
    /// View Component for reusable filter controls
    /// </summary>
    public class CustomControlFilterViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(CustomControlFilterModel model)
        {
            // Set default values if not provided
            if (model == null)
            {
                model = new CustomControlFilterModel();
            }

            // Ensure required properties have default values
            if (string.IsNullOrEmpty(model.ContainerId))
            {
                model.ContainerId = "customControlFilter";
            }

            if (string.IsNullOrEmpty(model.SearchInputId))
            {
                model.SearchInputId = "searchInput";
            }

            if (string.IsNullOrEmpty(model.ClearButtonId))
            {
                model.ClearButtonId = "clearFilters";
            }

            return View(model);
        }

        /// <summary>
        /// Create a basic search filter
        /// </summary>
        public static CustomControlFilterModel CreateBasicSearchFilter(
            string searchLabel = "Tìm kiếm...",
            string searchPlaceholder = "Tìm kiếm...")
        {
            return new CustomControlFilterModel
            {
                SearchLabel = searchLabel,
                SearchPlaceholder = searchPlaceholder,
                ShowSearchInput = true,
                ShowClearButton = true
            };
        }

        /// <summary>
        /// Create a filter with search and status dropdown
        /// </summary>
        public static CustomControlFilterModel CreateSearchWithStatusFilter(
            string searchLabel = "Tìm kiếm theo tên, email...",
            List<FilterOption>? statusOptions = null)
        {
            var model = new CustomControlFilterModel
            {
                SearchLabel = searchLabel,
                ShowSearchInput = true,
                ShowClearButton = true
            };

            // Add status filter
            var statusFilter = new FilterControl
            {
                Id = "statusFilter",
                Name = "status",
                Type = "select",
                DefaultOption = "Tất cả trạng thái",
                ColSize = 2,
                Options = statusOptions ?? new List<FilterOption>
                {
                    new FilterOption { Value = "Active", Text = "Hoạt động" },
                    new FilterOption { Value = "Inactive", Text = "Không hoạt động" },
                    new FilterOption { Value = "Pending", Text = "Chờ duyệt" },
                    new FilterOption { Value = "Suspended", Text = "Tạm ngưng" }
                }
            };

            model.FilterControls.Add(statusFilter);
            return model;
        }

        /// <summary>
        /// Create a comprehensive filter for license management
        /// </summary>
        public static CustomControlFilterModel CreateLicenseFilter()
        {
            var model = new CustomControlFilterModel
            {
                SearchLabel = "Tìm kiếm License",
                SearchInputColSize = 3,
                ShowSearchInput = true,
                ShowClearButton = true
            };

            // License type filter
            var typeFilter = new FilterControl
            {
                Id = "typeFilter",
                Name = "type",
                Type = "select",
                DefaultOption = "Tất cả loại",
                ColSize = 2,
                Options = new List<FilterOption>
                {
                    new FilterOption { Value = "basic", Text = "Basic" },
                    new FilterOption { Value = "premium", Text = "Premium" },
                    new FilterOption { Value = "enterprise", Text = "Enterprise" }
                }
            };

            // Status filter
            var statusFilter = new FilterControl
            {
                Id = "statusFilter",
                Name = "status",
                Type = "select",
                DefaultOption = "Tất cả trạng thái",
                ColSize = 2,
                Options = new List<FilterOption>
                {
                    new FilterOption { Value = "active", Text = "Hoạt động" },
                    new FilterOption { Value = "expired", Text = "Hết hạn" },
                    new FilterOption { Value = "suspended", Text = "Tạm ngưng" }
                }
            };

            // Date range filter
            var dateFilter = new FilterControl
            {
                Id = "dateRange",
                Name = "dateRange",
                Type = "daterange",
                ColSize = 3
            };

            model.FilterControls.AddRange(new[] { typeFilter, statusFilter, dateFilter });
            return model;
        }

        /// <summary>
        /// Create a filter for customer account management
        /// </summary>
        public static CustomControlFilterModel CreateCustomerAccountFilter()
        {
            var model = new CustomControlFilterModel
            {
                SearchLabel = "Tìm kiếm theo tên, email...",
                ShowSearchInput = true,
                ShowClearButton = true
            };

            // Department filter
            var departmentFilter = new FilterControl
            {
                Id = "departmentFilter",
                Name = "department",
                Type = "select",
                DefaultOption = "Tất cả phòng ban",
                ColSize = 2,
                Options = new List<FilterOption>
                {
                    new FilterOption { Value = "IT", Text = "IT" },
                    new FilterOption { Value = "Sales", Text = "Sales" },
                    new FilterOption { Value = "Marketing", Text = "Marketing" },
                    new FilterOption { Value = "HR", Text = "HR" }
                }
            };

            // Status filter
            var statusFilter = new FilterControl
            {
                Id = "statusFilter",
                Name = "status",
                Type = "select",
                DefaultOption = "Tất cả trạng thái",
                ColSize = 2,
                Options = new List<FilterOption>
                {
                    new FilterOption { Value = "Active", Text = "Hoạt động" },
                    new FilterOption { Value = "Inactive", Text = "Không hoạt động" },
                    new FilterOption { Value = "Pending", Text = "Chờ duyệt" },
                    new FilterOption { Value = "Suspended", Text = "Tạm ngưng" }
                }
            };

            model.FilterControls.AddRange(new[] { departmentFilter, statusFilter });
            return model;
        }
    }
}
