using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Converters
{
    /// <summary>
    /// TypeConverter for BeepSvgPaths to enable dropdown selection in Visual Studio designer
    /// </summary>
    public class BeepSvgPathConverter : StringConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            // Get all SVG paths from BeepSvgPaths class
            var paths = new System.Collections.Generic.List<string> { "(None)" };

            try
            {
                var type = typeof(BeepSvgPaths);
                var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static);

                foreach (var field in fields)
                {
                    if (field.FieldType == typeof(string) && field.IsLiteral == false && field.IsInitOnly)
                    {
                        var value = field.GetValue(null) as string;
                        if (!string.IsNullOrEmpty(value) && value.EndsWith(".svg"))
                        {
                            paths.Add(value);
                        }
                    }
                }

                // Sort alphabetically for easier browsing
                paths.Sort();
            }
            catch (Exception)
            {
                // If there's an error, just return the basic list
            }

            return new StandardValuesCollection(paths.ToArray());
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return false; // Allow custom values
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string stringValue)
            {
                if (stringValue == "(None)")
                    return string.Empty;
                return stringValue;
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value is string stringValue)
            {
                if (string.IsNullOrEmpty(stringValue))
                    return "(None)";
                return stringValue;
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}