using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Converters
{
    /// <summary>
    /// Type converter for Theme property that provides a dropdown list of all available themes
    /// from BeepThemesManager in the designer.
    /// </summary>
    public class ThemeEnumConverter : TypeConverter
    {
        /// <summary>
        /// Initializes a new instance of the ThemeEnumConverter class.
        /// </summary>
        public ThemeEnumConverter() { }

        /// <summary>
        /// Enables the dropdown list in the designer
        /// </summary>
        public override bool GetStandardValuesSupported(ITypeDescriptorContext? context)
        {
            return true;
        }

        /// <summary>
        /// Forces the user to pick from the list (no custom values)
        /// </summary>
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext? context)
        {
            return true;
        }

        /// <summary>
        /// Returns all available theme names from BeepThemesManager
        /// </summary>
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext? context)
        {
            try
            {
                // Get all theme names from BeepThemesManager
                var themeNames = BeepThemesManager.GetThemeNames();
                if (themeNames != null && themeNames.Any())
                {
                    return new StandardValuesCollection(themeNames.ToList());
                }
                
                // Fallback to default theme if no themes are available
                return new StandardValuesCollection(new[] { "DefaultTheme" });
            }
            catch
            {
                // Fallback in case of any errors
                return new StandardValuesCollection(new[] { "DefaultTheme" });
            }
        }

        /// <summary>
        /// Allows conversion from string to theme name
        /// </summary>
        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        /// <summary>
        /// Converts from string to theme name
        /// </summary>
        public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
        {
            if (value is string stringValue)
            {
                // Validate that the theme exists
                var themeNames = BeepThemesManager.GetThemeNames();
                if (themeNames != null && themeNames.Contains(stringValue))
                {
                    return stringValue;
                }
                
                // If theme doesn't exist, return the value anyway (might be set programmatically)
                return stringValue;
            }
            return base.ConvertFrom(context, culture, value);
        }

        /// <summary>
        /// Allows conversion to string for display
        /// </summary>
        public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
        {
            return destinationType == typeof(string) || base.CanConvertTo(context, destinationType);
        }

        /// <summary>
        /// Converts theme name to string for display
        /// </summary>
        public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
        {
            if (destinationType == typeof(string) && value is string stringValue)
            {
                return stringValue; // Return the theme name as-is
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
