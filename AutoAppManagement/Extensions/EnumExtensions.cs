using System.ComponentModel;
using System.Reflection;

namespace AutoAppManagement.Extensions
{
    /// <summary>
    /// Extension methods for Enum types
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// Get Description attribute value from enum
        /// </summary>
        /// <param name="value">Enum value</param>
        /// <returns>Description string or enum name if no description</returns>
        public static string GetDescription(this Enum value)
        {
            if (value == null)
                return string.Empty;

            var field = value.GetType().GetField(value.ToString());
            if (field == null)
                return value.ToString();

            var attribute = field.GetCustomAttribute<DescriptionAttribute>();
            return attribute?.Description ?? value.ToString().ToLower();
        }

        /// <summary>
        /// Get enum value from description string
        /// </summary>
        /// <typeparam name="T">Enum type</typeparam>
        /// <param name="description">Description string</param>
        /// <returns>Enum value or default if not found</returns>
        public static T GetEnumFromDescription<T>(string description) where T : Enum
        {
            var type = typeof(T);
            foreach (var field in type.GetFields())
            {
                var attribute = field.GetCustomAttribute<DescriptionAttribute>();
                if (attribute?.Description == description)
                {
                    return (T)field.GetValue(null)!;
                }
            }

            // Fallback to parse by name
            if (Enum.TryParse(type, description, true, out var result))
            {
                return (T)result;
            }

            return default(T)!;
        }

        /// <summary>
        /// Get all enum values with their descriptions
        /// </summary>
        /// <typeparam name="T">Enum type</typeparam>
        /// <returns>Dictionary of enum value and description</returns>
        public static Dictionary<T, string> GetEnumDescriptions<T>() where T : Enum
        {
            var result = new Dictionary<T, string>();
            var values = Enum.GetValues(typeof(T)).Cast<T>();

            foreach (var value in values)
            {
                result[value] = value.GetDescription();
            }

            return result;
        }
    }
}
