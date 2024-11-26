using TheTechIdea.Beep.Vis.Modules;
using System;
using System.ComponentModel;
using System.Globalization;

namespace TheTechIdea.Beep.Winform.Controls.Converters
{
    public class ThemeConverter : TypeConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true; // Enable a dropdown list
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true; // Force the user to pick from the list
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            // Return the available theme names (the names of EnumBeepThemes)
            var themeNames = BeepThemesManager.GetThemesNames(); // List of all enum theme names
            return new StandardValuesCollection(themeNames);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            // Allow conversion from string and EnumBeepThemes to BeepTheme
            return sourceType == typeof(string) || sourceType == typeof(EnumBeepThemes) || base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string stringValue)
            {
                // If the value is a string, we assume it's the name of the theme (Enum)
                return Enum.TryParse(typeof(EnumBeepThemes), stringValue, out var enumResult)
                    ? enumResult
                    : throw new ArgumentException($"Cannot convert {stringValue} to EnumBeepThemes.");
            }

            if (value is EnumBeepThemes enumValue)
            {
                // If it's already an EnumBeepThemes, return it as is
                return enumValue;
            }

            throw new NotSupportedException($"Cannot convert {value?.GetType()} to EnumBeepThemes");
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            // Allow conversion to string or EnumBeepThemes
            return destinationType == typeof(string) || destinationType == typeof(EnumBeepThemes) || base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                if (value is EnumBeepThemes enumValue)
                {
                    // Convert EnumBeepThemes to its string name for the dropdown
                    return enumValue.ToString();
                }
                else if (value is BeepTheme theme)
                {
                    // Convert BeepTheme to its corresponding string name
                    return BeepThemesManager.GetTheme(theme);
                }
                else if (value is string)
                {
                    // If the value is already a string, just return it (no need for conversion)
                    return value;
                }
            }

            if (destinationType == typeof(EnumBeepThemes) && value is BeepTheme themeValue)
            {
                // Convert BeepTheme to its corresponding EnumBeepThemes value
                string themeName = BeepThemesManager.GetTheme(themeValue);
                if (Enum.TryParse(typeof(EnumBeepThemes), themeName, out var result))
                {
                    return result;
                }
                throw new ArgumentException($"Cannot convert {themeName} to EnumBeepThemes.");
            }

            if (destinationType == typeof(BeepTheme) && value is string stringValue)
            {
                // Convert string back to BeepTheme via Enum parsing and then fetching the theme
                if (Enum.TryParse(typeof(EnumBeepThemes), stringValue, out var enumResult))
                {
                    return BeepThemesManager.GetTheme((EnumBeepThemes)enumResult);
                }
                throw new ArgumentException($"Cannot convert {stringValue} to BeepTheme.");
            }

            return base.ConvertTo(context, culture, value, destinationType); // Handle unexpected types with base implementation
        }


    }
}
