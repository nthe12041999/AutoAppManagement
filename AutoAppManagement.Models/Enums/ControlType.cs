namespace AutoAppManagement.Models.Enums
{
    /// <summary>
    /// Enum định nghĩa các loại control trong form
    /// </summary>
    public enum ControlType
    {
        /// <summary>
        /// Input text thường
        /// </summary>
        Text = 1,

        /// <summary>
        /// Input email
        /// </summary>
        Email = 2,

        /// <summary>
        /// Input password
        /// </summary>
        Password = 3,

        /// <summary>
        /// Input number
        /// </summary>
        Number = 4,

        /// <summary>
        /// Input tel (phone)
        /// </summary>
        Tel = 5,

        /// <summary>
        /// Input url
        /// </summary>
        Url = 6,

        /// <summary>
        /// Textarea
        /// </summary>
        Textarea = 7,

        /// <summary>
        /// Select dropdown
        /// </summary>
        Select = 8,

        /// <summary>
        /// Multiple select
        /// </summary>
        MultiSelect = 9,

        /// <summary>
        /// Radio buttons
        /// </summary>
        Radio = 10,

        /// <summary>
        /// Checkbox
        /// </summary>
        Checkbox = 11,

        /// <summary>
        /// Checkbox group
        /// </summary>
        CheckboxGroup = 12,

        /// <summary>
        /// Date picker
        /// </summary>
        Date = 13,

        /// <summary>
        /// DateTime picker
        /// </summary>
        DateTime = 14,

        /// <summary>
        /// Time picker
        /// </summary>
        Time = 15,

        /// <summary>
        /// Date range picker
        /// </summary>
        DateRange = 16,

        /// <summary>
        /// File upload
        /// </summary>
        File = 17,

        /// <summary>
        /// Image upload
        /// </summary>
        Image = 18,

        /// <summary>
        /// Rich text editor
        /// </summary>
        RichText = 19,

        /// <summary>
        /// Color picker
        /// </summary>
        Color = 20,

        /// <summary>
        /// Range slider
        /// </summary>
        Range = 21,

        /// <summary>
        /// Switch toggle
        /// </summary>
        Switch = 22,

        /// <summary>
        /// Hidden input
        /// </summary>
        Hidden = 23,

        /// <summary>
        /// Read-only display
        /// </summary>
        Display = 24,

        /// <summary>
        /// Custom control
        /// </summary>
        Custom = 25,

        /// <summary>
        /// Modal header
        /// </summary>
        ModalHeader = 26,

        /// <summary>
        /// Modal footer
        /// </summary>
        ModalFooter = 27,

        /// <summary>
        /// Form wrapper với modal header/footer
        /// </summary>
        Form = 28
    }
}
