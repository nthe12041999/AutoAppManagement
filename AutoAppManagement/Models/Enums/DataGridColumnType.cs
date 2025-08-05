namespace AutoAppManagement.Models.Enums
{
    /// <summary>
    /// Enum định nghĩa các loại column trong DataGrid
    /// Sử dụng int values cho data-type attribute
    /// </summary>
    public enum DataGridColumnType
    {
        /// <summary>
        /// Plain text display
        /// </summary>
        Text = 0,

        /// <summary>
        /// Formatted number (right-aligned)
        /// </summary>
        Number = 1,

        /// <summary>
        /// Selection checkbox
        /// </summary>
        Checkbox = 2,

        /// <summary>
        /// Radio button selection
        /// </summary>
        Radio = 3,

        /// <summary>
        /// Date and time display (full datetime)
        /// </summary>
        DateTime = 4,

        /// <summary>
        /// Date only display
        /// </summary>
        Date = 5,

        /// <summary>
        /// Time only display
        /// </summary>
        Time = 6,

        /// <summary>
        /// Currency format (VND)
        /// </summary>
        Currency = 7,

        /// <summary>
        /// Colored badge display
        /// </summary>
        Badge = 8,

        /// <summary>
        /// User profile with avatar and details
        /// </summary>
        User = 9,

        /// <summary>
        /// User avatar only
        /// </summary>
        Avatar = 10,

        /// <summary>
        /// Image display
        /// </summary>
        Image = 11,

        /// <summary>
        /// Web link (opens in new tab)
        /// </summary>
        Link = 12,

        /// <summary>
        /// Email link (mailto)
        /// </summary>
        Email = 13,

        /// <summary>
        /// Phone link (tel)
        /// </summary>
        Phone = 14,

        /// <summary>
        /// Boolean display (Yes/No with icons)
        /// </summary>
        Boolean = 15,

        /// <summary>
        /// Action buttons
        /// </summary>
        Actions = 16
    }
}
