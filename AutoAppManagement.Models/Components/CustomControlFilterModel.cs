namespace AutoAppManagement.Models.Components
{
    /// <summary>
    /// Model for CustomControlFilter View Component
    /// </summary>
    public class CustomControlFilterModel
    {
        public string ContainerId { get; set; } = "customControlFilter";
        
        // Search Input Properties
        public bool ShowSearchInput { get; set; } = true;
        public string SearchInputId { get; set; } = "searchInput";
        public string SearchLabel { get; set; } = "Tìm kiếm...";
        public string SearchPlaceholder { get; set; } = "Tìm kiếm...";
        public string SearchValue { get; set; } = string.Empty;
        public int SearchInputColSize { get; set; } = 4;
        
        // Filter Controls
        public List<FilterControl> FilterControls { get; set; } = new List<FilterControl>();
        
        // Clear Button Properties
        public bool ShowClearButton { get; set; } = true;
        public string ClearButtonId { get; set; } = "clearFilters";
        public string ClearButtonText { get; set; } = "Xóa bộ lọc";
        public int ClearButtonColSize { get; set; } = 2;
        
        // Custom Buttons
        public List<CustomButton> CustomButtons { get; set; } = new List<CustomButton>();
        
        // Configuration for JavaScript
        public FilterConfig Config { get; set; } = new FilterConfig();
    }

    /// <summary>
    /// Filter control definition
    /// </summary>
    public class FilterControl
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = "select"; // select, input, daterange
        public string Label { get; set; } = string.Empty;
        public string Placeholder { get; set; } = string.Empty;
        public string DefaultOption { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string InputType { get; set; } = "text"; // text, number, email, etc.
        public int ColSize { get; set; } = 2;
        
        // For date range
        public string FromValue { get; set; } = string.Empty;
        public string ToValue { get; set; } = string.Empty;
        
        // Options for select
        public List<FilterOption> Options { get; set; } = new List<FilterOption>();
    }

    /// <summary>
    /// Option for select filter
    /// </summary>
    public class FilterOption
    {
        public string Value { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public bool Selected { get; set; } = false;
    }

    /// <summary>
    /// Custom button definition
    /// </summary>
    public class CustomButton
    {
        public string Id { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string CssClass { get; set; } = "btn-primary";
        public string OnClick { get; set; } = string.Empty;
        public int ColSize { get; set; } = 2;
    }

    /// <summary>
    /// Configuration for JavaScript functionality
    /// </summary>
    public class FilterConfig
    {
        public string TargetTableId { get; set; } = string.Empty;
        public string ApiEndpoint { get; set; } = string.Empty;
        public int SearchDebounceMs { get; set; } = 500;
        public bool AutoApplyFilters { get; set; } = true;
        public string OnFilterChange { get; set; } = string.Empty; // JavaScript callback function name
        public Dictionary<string, object> CustomSettings { get; set; } = new Dictionary<string, object>();
    }
}
