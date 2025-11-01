using System.Text.Json;
using static AutoAppManagement.Models.Enum.DataModelType;

namespace AutoAppManagement.Helpers
{
    public static class FormOptionsHelper
    {
        /// <summary>
        /// Tạo options cho role status
        /// </summary>
        public static string GetRoleStatusOptions(string selectedValue = "active")
        {
            var options = new object[]
            {
                new { value = "", text = "-- Chọn trạng thái --" },
                new { value = "active", text = "Hoạt động", selected = selectedValue == "active" },
                new { value = "inactive", text = "Không hoạt động", selected = selectedValue == "inactive" },
                new { value = "pending", text = "Chờ phê duyệt", selected = selectedValue == "pending" },
                new { value = "suspended", text = "Tạm khóa", selected = selectedValue == "suspended" }
            };

            return JsonSerializer.Serialize(options);
        }

        /// <summary>
        /// Tạo options cho role priority
        /// </summary>
        public static string GetRolePriorityOptions(string selectedValue = "2")
        {
            var options = new object[]
            {
                new { value = "", text = "-- Chọn độ ưu tiên --" },
                new { value = "1", text = "Cao", selected = selectedValue == "1" },
                new { value = "2", text = "Trung bình", selected = selectedValue == "2" },
                new { value = "3", text = "Thấp", selected = selectedValue == "3" }
            };

            return JsonSerializer.Serialize(options);
        }

        /// <summary>
        /// Tạo options cho user roles
        /// </summary>
        public static string GetUserRoleOptions(string selectedValue = "")
        {
            var options = new object[]
            {
                new { value = "", text = "-- Chọn vai trò --" },
                new { value = "admin", text = "Quản trị viên", selected = selectedValue == "admin" },
                new { value = "manager", text = "Quản lý", selected = selectedValue == "manager" },
                new { value = "user", text = "Người dùng", selected = selectedValue == "user" },
                new { value = "guest", text = "Khách", selected = selectedValue == "guest" }
            };

            return JsonSerializer.Serialize(options);
        }

        /// <summary>
        /// Tạo options cho account status từ StatusEnum
        /// </summary>
        public static string GetAccountStatusOptions(int? selectedValue = null)
        {
            var options = new object[]
            {
                new { value = "", text = "-- Chọn trạng thái --" },
                new { value = 1, text = "Hoạt động", selected = selectedValue == 1 },
                new { value = 2, text = "Không hoạt động", selected = selectedValue == 2 },
                new { value = 3, text = "Bị khóa", selected = selectedValue == 3 }
            };

            return JsonSerializer.Serialize(options);
        }

        /// <summary>
        /// Tạo options cho gender
        /// </summary>
        public static string GetGenderOptions(Gender selectedValue = Gender.Male)
        {
            var options = new object[]
            {
                new { value = "", text = "-- Chọn giới tính --" },
                new { value = Gender.Male, text = "Nam", selected = selectedValue == Gender.Male },
                new { value = Gender.Femal, text = "Nữ", selected = selectedValue == Gender.Femal },
                new { value = Gender.Other, text = "Khác", selected = selectedValue ==  Gender.Other }
            };

            return JsonSerializer.Serialize(options);
        }

        /// <summary>
        /// Tạo options cho customer types
        /// </summary>
        public static string GetCustomerTypeOptions(string selectedValue = "")
        {
            var options = new object[]
            {
                new { value = "", text = "-- Chọn loại khách hàng --" },
                new { value = "customer", text = "Khách hàng thường", selected = selectedValue == "customer" },
                new { value = "premium", text = "Khách hàng Premium", selected = selectedValue == "premium" },
                new { value = "vip", text = "Khách hàng VIP", selected = selectedValue == "vip" },
                new { value = "trial", text = "Dùng thử", selected = selectedValue == "trial" }
            };

            return JsonSerializer.Serialize(options);
        }

        /// <summary>
        /// Tạo options từ enum
        /// </summary>
        public static string GetEnumOptions<T>(T? selectedValue = null) where T : struct, Enum
        {
            var options = new List<object>
            {
                new { value = "", text = $"-- Chọn {typeof(T).Name} --" }
            };

            foreach (T enumValue in Enum.GetValues<T>())
            {
                options.Add(new
                {
                    value = enumValue.ToString(),
                    text = enumValue.ToString(),
                    selected = selectedValue?.Equals(enumValue) == true
                });
            }

            return JsonSerializer.Serialize(options);
        }

        /// <summary>
        /// Tạo options từ Dictionary (có thể từ DB)
        /// </summary>
        public static string GetOptionsFromDictionary(Dictionary<string, string> data, string selectedValue = "", string emptyText = "-- Chọn --")
        {
            var options = new List<object>
            {
                new { value = "", text = emptyText }
            };

            foreach (var item in data)
            {
                options.Add(new
                {
                    value = item.Key,
                    text = item.Value,
                    selected = selectedValue == item.Key
                });
            }

            return JsonSerializer.Serialize(options);
        }

        /// <summary>
        /// Tạo options từ List objects (từ DB)
        /// </summary>
        public static string GetOptionsFromList<T>(
            IEnumerable<T> data,
            Func<T, string> valueSelector,
            Func<T, string> textSelector,
            string selectedValue = "",
            string emptyText = "-- Chọn --")
        {
            var options = new List<object>
            {
                new { value = "", text = emptyText }
            };

            foreach (var item in data)
            {
                var value = valueSelector(item);
                options.Add(new
                {
                    value = value,
                    text = textSelector(item),
                    selected = selectedValue == value
                });
            }

            return JsonSerializer.Serialize(options);
        }

        /// <summary>
        /// Tạo options cho Yes/No
        /// </summary>
        public static string GetYesNoOptions(bool? selectedValue = null)
        {
            var options = new object[]
            {
                new { value = "", text = "-- Chọn --" },
                new { value = "true", text = "Có", selected = selectedValue == true },
                new { value = "false", text = "Không", selected = selectedValue == false }
            };

            return JsonSerializer.Serialize(options);
        }

        /// <summary>
        /// Tạo options cho months
        /// </summary>
        public static string GetMonthOptions(int? selectedValue = null)
        {
            var options = new List<object>
            {
                new { value = "", text = "-- Chọn tháng --" }
            };

            for (int i = 1; i <= 12; i++)
            {
                options.Add(new
                {
                    value = i.ToString(),
                    text = $"Tháng {i}",
                    selected = selectedValue == i
                });
            }

            return JsonSerializer.Serialize(options);
        }

        /// <summary>
        /// Tạo options cho years
        /// </summary>
        public static string GetYearOptions(int startYear, int endYear, int? selectedValue = null)
        {
            var options = new List<object>
            {
                new { value = "", text = "-- Chọn năm --" }
            };

            for (int year = startYear; year <= endYear; year++)
            {
                options.Add(new
                {
                    value = year.ToString(),
                    text = year.ToString(),
                    selected = selectedValue == year
                });
            }

            return JsonSerializer.Serialize(options);
        }
    }
}
