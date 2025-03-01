using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.ComponentModel.TypeConverter;

namespace TheTechIdea.Beep.Winform.Controls.Converters
{
    public class EnumTypeConverter : TypeConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true; // Enable dropdown
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true; // Only allow selection from the list (no free typing)
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
        //    var assemblies = AppDomain.CurrentDomain.GetAssemblies()
        //                     .Where(a => !a.IsDynamic && a.GetName().Name.StartsWith("TheTechIdea")); // Filter by namespace
            // Get all loaded assemblies in the current AppDomain
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            // Filter for enum types across all assemblies
            var enumTypes = assemblies
                .SelectMany(asm => asm.GetTypes())
                .Where(t => t.IsEnum && t.IsPublic) // Only public enums
                .OrderBy(t => t.FullName)
                .Select(t => t.AssemblyQualifiedName) // Use AssemblyQualifiedName for uniqueness
                .ToList();

            // Add a null/empty option if desired
            enumTypes.Insert(0, string.Empty);

            return new StandardValuesCollection(enumTypes);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value is string stringValue)
            {
                return stringValue; // Return the AssemblyQualifiedName as-is
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value is string stringValue)
            {
                if (string.IsNullOrEmpty(stringValue))
                    return "(None)";

                // Extract the type name without assembly details for display
                Type type = Type.GetType(stringValue);
                return type != null ? type.Name : stringValue;
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
