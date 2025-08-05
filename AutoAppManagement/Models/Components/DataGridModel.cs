using System.Collections.Generic;

namespace AutoAppManagement.Models.Components
{
    public class DataGridModel
    {
        public string TableId { get; set; } = "dataGrid";
        public string Title { get; set; } = "";
        public string TitleIcon { get; set; } = "bi bi-table";
        public List<DataGridColumn> Columns { get; set; } = new List<DataGridColumn>();
        public List<dynamic> Data { get; set; } = new List<dynamic>();
        public List<DataGridAction> ActionButtons { get; set; } = new List<DataGridAction>();
        
        // Features
        public bool ShowCheckbox { get; set; } = true;
        public bool ShowActions { get; set; } = true;
        public bool ShowPagination { get; set; } = true;
        public bool ShowExport { get; set; } = true;
        public bool ShowRefresh { get; set; } = true;
        
        // Pagination
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalRecords { get; set; } = 0;
        public int TotalPages => (int)Math.Ceiling((double)TotalRecords / PageSize);
        public int PageStart => (CurrentPage - 1) * PageSize + 1;
        public int PageEnd => Math.Min(CurrentPage * PageSize, TotalRecords);
        
        // Actions
        public string ExportAction { get; set; } = "exportData()";
        public string RefreshAction { get; set; } = "refreshData()";
        public string PaginationAction(int page) => $"goToPage({page})";
    }

    public class DataGridColumn
    {
        public string Title { get; set; } = "";
        public string BindingField { get; set; } = "";
        public string Type { get; set; } = "text"; // text, badge, currency, date, datetime, user, avatar, custom
        public int Width { get; set; } = 0; // 0 = auto width
        public bool Sortable { get; set; } = false;
        public string CustomTemplate { get; set; } = ""; // For custom type
        public Dictionary<string, object> Properties { get; set; } = new Dictionary<string, object>();
    }

    public class DataGridAction
    {
        public string Title { get; set; } = "";
        public string Icon { get; set; } = "";
        public string CssClass { get; set; } = "btn-outline-primary";
        public string OnClick(object id) => $"{Action}({id})";
        public string Action { get; set; } = "";
    }
}
