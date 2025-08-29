using AutoAppManagement.Models.Enums;

namespace AutoAppManagement.Models.Components
{
    /// <summary>
    /// Model cho form control
    /// </summary>
    public class FormControlModel
    {
        /// <summary>
        /// ID của control
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Name attribute của control
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Label hiển thị
        /// </summary>
        public string Label { get; set; } = string.Empty;

        /// <summary>
        /// Placeholder text
        /// </summary>
        public string Placeholder { get; set; } = string.Empty;

        /// <summary>
        /// Giá trị mặc định
        /// </summary>
        public string? Value { get; set; }

        /// <summary>
        /// Loại control
        /// </summary>
        public ControlType Type { get; set; } = ControlType.Text;

        /// <summary>
        /// Có bắt buộc không
        /// </summary>
        public bool Required { get; set; } = false;

        /// <summary>
        /// Có disabled không
        /// </summary>
        public bool Disabled { get; set; } = false;

        /// <summary>
        /// Có readonly không
        /// </summary>
        public bool ReadOnly { get; set; } = false;

        /// <summary>
        /// CSS class thêm
        /// </summary>
        public string CssClass { get; set; } = string.Empty;

        /// <summary>
        /// Kích thước cột (1-12 cho Bootstrap grid)
        /// </summary>
        public int ColSize { get; set; } = 12;

        /// <summary>
        /// Thông báo validation
        /// </summary>
        public string ValidationMessage { get; set; } = string.Empty;

        /// <summary>
        /// Help text
        /// </summary>
        public string HelpText { get; set; } = string.Empty;

        /// <summary>
        /// Icon hiển thị
        /// </summary>
        public string Icon { get; set; } = string.Empty;

        /// <summary>
        /// Danh sách options cho Select, Radio, Checkbox
        /// </summary>
        public List<FormControlOption> Options { get; set; } = new List<FormControlOption>();

        /// <summary>
        /// Attributes tùy chỉnh
        /// </summary>
        public Dictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Số hàng cho textarea
        /// </summary>
        public int Rows { get; set; } = 3;

        /// <summary>
        /// Giá trị min (cho number, date, range)
        /// </summary>
        public string? Min { get; set; }

        /// <summary>
        /// Giá trị max (cho number, date, range)
        /// </summary>
        public string? Max { get; set; }

        /// <summary>
        /// Step (cho number, range)
        /// </summary>
        public string? Step { get; set; }

        /// <summary>
        /// Pattern validation (regex)
        /// </summary>
        public string? Pattern { get; set; }

        /// <summary>
        /// Độ dài tối đa
        /// </summary>
        public int? MaxLength { get; set; }

        /// <summary>
        /// Độ dài tối thiểu
        /// </summary>
        public int? MinLength { get; set; }

        /// <summary>
        /// Accept types cho file upload
        /// </summary>
        public string? Accept { get; set; }

        /// <summary>
        /// Multiple selection cho file upload
        /// </summary>
        public bool Multiple { get; set; } = false;

        /// <summary>
        /// Data source URL cho dynamic options
        /// </summary>
        public string? DataSource { get; set; }

        /// <summary>
        /// Dependency controls (controls phụ thuộc)
        /// </summary>
        public List<string> Dependencies { get; set; } = new List<string>();
    }

    /// <summary>
    /// Option cho Select, Radio, Checkbox
    /// </summary>
    public class FormControlOption
    {
        /// <summary>
        /// Giá trị
        /// </summary>
        public string Value { get; set; } = string.Empty;

        /// <summary>
        /// Text hiển thị
        /// </summary>
        public string Text { get; set; } = string.Empty;

        /// <summary>
        /// Có được chọn mặc định không
        /// </summary>
        public bool Selected { get; set; } = false;

        /// <summary>
        /// Có disabled không
        /// </summary>
        public bool Disabled { get; set; } = false;

        /// <summary>
        /// Group (cho optgroup)
        /// </summary>
        public string? Group { get; set; }

        /// <summary>
        /// CSS class
        /// </summary>
        public string CssClass { get; set; } = string.Empty;

        /// <summary>
        /// Icon
        /// </summary>
        public string Icon { get; set; } = string.Empty;

        /// <summary>
        /// Data attributes
        /// </summary>
        public Dictionary<string, string> DataAttributes { get; set; } = new Dictionary<string, string>();
    }
}
