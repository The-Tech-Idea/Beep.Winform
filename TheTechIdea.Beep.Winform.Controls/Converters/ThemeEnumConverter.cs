using System;
using System.ComponentModel;
using System.Globalization;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Converters
{
    public class ThemeEnumConverter : EnumConverter
    {
        public ThemeEnumConverter() : base(typeof(EnumBeepThemes)) { }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string stringValue)
            {
                return Enum.TryParse(typeof(EnumBeepThemes), stringValue, out var enumValue)
                    ? enumValue
                    : throw new ArgumentException($"Invalid theme: {stringValue}");
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value is EnumBeepThemes enumValue)
            {
                return enumValue.ToString(); // Return the name of the enum as a string
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
