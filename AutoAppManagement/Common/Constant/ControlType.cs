using System.ComponentModel;

namespace AutoAppManagement.WebApp.Common.Constant
{
    public enum ControlType: int
    {
        // Basic Input Types
        Text = 1,
        Email = 2,
        Password = 3,
        Number = 4,
        Tel = 5,
        Url = 6,

        // Text Areas
        Textarea = 10,

        // Selection Controls
        Select = 20,
        MultiSelect = 21,
        Radio = 22,
        Checkbox = 23,
        CheckboxGroup = 24,

        // Date/Time Controls
        Date = 30,
        DateTime = 31,
        Time = 32,

        // File Controls
        File = 40,
        Image = 41,

        // Other Controls
        Color = 50,
        Range = 51,
        Switch = 52,
        Toggle = 53,
        Hidden = 54,

        // Display Controls
        Display = 60,

        // Form Structure
        Form = 70,
        ModalHeader = 71,
        ModalFooter = 72,

        // Legacy (keep for compatibility)
        Combox = 100,
        Toogle = 101
    }
}
