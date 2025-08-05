using AutoAppManagement.Extensions;
using AutoAppManagement.Models.Components;
using AutoAppManagement.Models.Enums;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq.Expressions;

namespace AutoAppManagement.Helpers
{
    public static class DataGridHelper
    {
        public static DataGridModel CreateGrid(string tableId = "dataGrid")
        {
            return new DataGridModel
            {
                TableId = tableId
            };
        }

        public static DataGridModel WithTitle(this DataGridModel grid, string title, string icon = "bi bi-table")
        {
            grid.Title = title;
            grid.TitleIcon = icon;
            return grid;
        }

        public static DataGridModel WithColumns(this DataGridModel grid, params DataGridColumn[] columns)
        {
            grid.Columns.AddRange(columns);
            return grid;
        }

        public static DataGridModel WithData(this DataGridModel grid, List<dynamic> data)
        {
            grid.Data = data;
            grid.TotalRecords = data.Count;
            return grid;
        }

        public static DataGridModel WithActions(this DataGridModel grid, params DataGridAction[] actions)
        {
            grid.ActionButtons.AddRange(actions);
            return grid;
        }

        public static DataGridModel WithFeatures(this DataGridModel grid, 
            bool showCheckbox = true, 
            bool showActions = true, 
            bool showPagination = true, 
            bool showExport = true, 
            bool showRefresh = true)
        {
            grid.ShowCheckbox = showCheckbox;
            grid.ShowActions = showActions;
            grid.ShowPagination = showPagination;
            grid.ShowExport = showExport;
            grid.ShowRefresh = showRefresh;
            return grid;
        }

        public static DataGridModel WithPagination(this DataGridModel grid, int currentPage, int pageSize)
        {
            grid.CurrentPage = currentPage;
            grid.PageSize = pageSize;
            return grid;
        }

        // Column builders
        public static DataGridColumn TextColumn(string title, string bindingField, int width = 0)
        {
            return new DataGridColumn
            {
                Title = title,
                BindingField = bindingField,
                Type = "text",
                Width = width
            };
        }

        public static DataGridColumn BadgeColumn(string title, string bindingField, int width = 0)
        {
            return new DataGridColumn
            {
                Title = title,
                BindingField = bindingField,
                Type = "badge",
                Width = width
            };
        }

        public static DataGridColumn CurrencyColumn(string title, string bindingField, int width = 0)
        {
            return new DataGridColumn
            {
                Title = title,
                BindingField = bindingField,
                Type = "currency",
                Width = width
            };
        }

        public static DataGridColumn DateColumn(string title, string bindingField, int width = 0)
        {
            return new DataGridColumn
            {
                Title = title,
                BindingField = bindingField,
                Type = "date",
                Width = width
            };
        }

        public static DataGridColumn DateTimeColumn(string title, string bindingField, int width = 0)
        {
            return new DataGridColumn
            {
                Title = title,
                BindingField = bindingField,
                Type = "datetime",
                Width = width
            };
        }

        public static DataGridColumn UserColumn(string title, string bindingField = "", int width = 0)
        {
            return new DataGridColumn
            {
                Title = title,
                BindingField = bindingField,
                Type = "user",
                Width = width
            };
        }

        public static DataGridColumn AvatarColumn(string title, string bindingField, int width = 0)
        {
            return new DataGridColumn
            {
                Title = title,
                BindingField = bindingField,
                Type = "avatar",
                Width = width
            };
        }

        public static DataGridColumn CustomColumn(string title, string bindingField, string template, int width = 0)
        {
            return new DataGridColumn
            {
                Title = title,
                BindingField = bindingField,
                Type = "custom",
                CustomTemplate = template,
                Width = width
            };
        }

        // Action builders
        public static DataGridAction ViewAction(string action = "viewItem")
        {
            return new DataGridAction
            {
                Title = "Xem chi tiết",
                Icon = "bi bi-eye",
                CssClass = "btn-outline-primary",
                Action = action
            };
        }

        public static DataGridAction EditAction(string action = "editItem")
        {
            return new DataGridAction
            {
                Title = "Chỉnh sửa",
                Icon = "bi bi-pencil",
                CssClass = "btn-outline-warning",
                Action = action
            };
        }

        public static DataGridAction DeleteAction(string action = "deleteItem")
        {
            return new DataGridAction
            {
                Title = "Xóa",
                Icon = "bi bi-trash",
                CssClass = "btn-outline-danger",
                Action = action
            };
        }

        public static DataGridAction CustomAction(string title, string icon, string cssClass, string action)
        {
            return new DataGridAction
            {
                Title = title,
                Icon = icon,
                CssClass = cssClass,
                Action = action
            };
        }

        // ===== ENUM SUPPORT METHODS =====

        /// <summary>
        /// Generate data-type attribute from enum
        /// </summary>
        /// <param name="columnType">Column type enum</param>
        /// <returns>data-type attribute string</returns>
        public static string GetDataType(DataGridColumnType columnType)
        {
            return columnType.GetDescription();
        }

        /// <summary>
        /// Generate complete th element with data attributes using enum
        /// </summary>
        /// <param name="columnName">Column identifier</param>
        /// <param name="fieldName">Data field name (optional)</param>
        /// <param name="columnType">Column type enum</param>
        /// <param name="displayText">Display text</param>
        /// <param name="sortable">Is sortable</param>
        /// <param name="width">Column width (optional)</param>
        /// <returns>HTML string for th element</returns>
        public static IHtmlContent GenerateTableHeader(
            string columnName,
            string? fieldName,
            DataGridColumnType columnType,
            string displayText,
            bool sortable = true,
            string? width = null)
        {
            var tagBuilder = new TagBuilder("th");
            tagBuilder.Attributes["scope"] = "col";
            tagBuilder.Attributes["data-column"] = columnName;
            tagBuilder.Attributes["data-type"] = GetDataType(columnType);
            tagBuilder.Attributes["data-sortable"] = sortable.ToString().ToLower();

            if (!string.IsNullOrEmpty(fieldName))
            {
                tagBuilder.Attributes["data-field"] = fieldName;
            }

            if (!string.IsNullOrEmpty(width))
            {
                tagBuilder.Attributes["width"] = width;
            }

            tagBuilder.InnerHtml.SetContent(displayText);
            return tagBuilder;
        }

        /// <summary>
        /// Generate checkbox column header using enum
        /// </summary>
        /// <param name="selectAllId">ID for select all checkbox</param>
        /// <returns>HTML content for checkbox header</returns>
        public static IHtmlContent GenerateCheckboxHeader(string selectAllId = "selectAll")
        {
            var th = new TagBuilder("th");
            th.Attributes["scope"] = "col";
            th.Attributes["width"] = "50";
            th.Attributes["data-column"] = "checkbox";
            th.Attributes["data-type"] = GetDataType(DataGridColumnType.Checkbox);
            th.Attributes["data-sortable"] = "false";

            var checkbox = new TagBuilder("input");
            checkbox.Attributes["type"] = "checkbox";
            checkbox.Attributes["class"] = "form-check-input";
            checkbox.Attributes["id"] = selectAllId;
            checkbox.Attributes["data-action"] = "select-all";
            checkbox.Attributes["data-target"] = "all-rows";

            th.InnerHtml.SetHtmlContent(checkbox);
            return th;
        }

        /// <summary>
        /// Generate actions column header using enum
        /// </summary>
        /// <param name="displayText">Display text</param>
        /// <param name="width">Column width</param>
        /// <returns>HTML content for actions header</returns>
        public static IHtmlContent GenerateActionsHeader(string displayText = "Actions", string width = "150")
        {
            return GenerateTableHeader(
                columnName: "actions",
                fieldName: null,
                columnType: DataGridColumnType.Actions,
                displayText: displayText,
                sortable: false,
                width: width
            );
        }

        /// <summary>
        /// Get all available column types for dropdown/selection
        /// </summary>
        /// <returns>SelectList of column types</returns>
        public static SelectList GetColumnTypeSelectList(DataGridColumnType? selectedValue = null)
        {
            var types = EnumExtensions.GetEnumDescriptions<DataGridColumnType>()
                .Select(kvp => new SelectListItem
                {
                    Value = kvp.Value, // Description value
                    Text = $"{kvp.Key} ({kvp.Value})", // Enum name + description
                    Selected = selectedValue.HasValue && kvp.Key == selectedValue.Value
                })
                .ToList();

            return new SelectList(types, "Value", "Text", selectedValue?.GetDescription());
        }

        /// <summary>
        /// Validate if column type is valid
        /// </summary>
        /// <param name="columnType">Column type string</param>
        /// <returns>True if valid</returns>
        public static bool IsValidColumnType(string columnType)
        {
            if (string.IsNullOrEmpty(columnType))
                return false;

            var descriptions = EnumExtensions.GetEnumDescriptions<DataGridColumnType>();
            return descriptions.Values.Contains(columnType.ToLower());
        }

        /// <summary>
        /// Get column type enum from string
        /// </summary>
        /// <param name="columnType">Column type string</param>
        /// <returns>DataGridColumnType enum</returns>
        public static DataGridColumnType GetColumnTypeEnum(string columnType)
        {
            return EnumExtensions.GetEnumFromDescription<DataGridColumnType>(columnType);
        }

        /// <summary>
        /// Get property name for data-field attribute using lambda expression
        /// </summary>
        /// <typeparam name="T">Model type</typeparam>
        /// <typeparam name="TProperty">Property type</typeparam>
        /// <param name="expression">Lambda expression for property</param>
        /// <returns>Property name as string</returns>
        public static string GetPropNameField<T, TProperty>(Expression<Func<T, TProperty>> expression)
        {
            if (expression.Body is MemberExpression memberExpression)
            {
                return memberExpression.Member.Name;
            }

            if (expression.Body is UnaryExpression unaryExpression &&
                unaryExpression.Operand is MemberExpression memberExpr)
            {
                return memberExpr.Member.Name;
            }

            throw new ArgumentException("Expression must be a member expression", nameof(expression));
        }
    }
}
