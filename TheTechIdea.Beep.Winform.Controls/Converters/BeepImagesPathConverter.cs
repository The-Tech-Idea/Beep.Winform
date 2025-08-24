using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using TheTechIdea.Beep.Icons;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using System.Linq;

namespace TheTechIdea.Beep.Winform.Controls.Converters
{
    /// <summary>
    /// TypeConverter for all classes icon under namespace TheTechIdea.Beep.Icons to enable dropdown selection in Visual Studio designer
    /// </summary>
    public class BeepImagesPathConverter : StringConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            var paths = new System.Collections.Generic.List<string> { "(None)" };
            string[] exts = new[] { ".svg", ".png", ".jpg", ".jpeg", ".gif", ".bmp", ".ico", ".webp" };
            try
            {
                // Search all loaded assemblies for icon/image classes
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                var iconTypes = assemblies
                    .SelectMany(a => {
                        try { return a.GetTypes(); } catch { return Array.Empty<Type>(); }
                    })
                    .Where(t => t.IsClass && t.IsPublic && t.IsAbstract && t.IsSealed && t.Namespace == "TheTechIdea.Beep.Icons");

                foreach (var type in iconTypes)
                {
                    var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static);
                    foreach (var field in fields)
                    {
                        if (field.FieldType == typeof(string) && !field.IsLiteral && field.IsInitOnly)
                        {
                            var value = field.GetValue(null) as string;
                            if (!string.IsNullOrEmpty(value) && exts.Any(ext => value.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
                            {
                                paths.Add(value);
                            }
                        }
                    }
                }
                // Remove duplicates and sort
                paths = paths.Distinct().ToList();
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