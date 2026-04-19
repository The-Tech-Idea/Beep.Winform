// BeepConverters/BeepUIComponentTypeConverter.cs
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;


namespace TheTechIdea.Beep.Vis.Modules
{
    public class BeepUIComponentTypeConverter : TypeConverter
    {
        private readonly Dictionary<string, Type> _typeMap = new();

        /// <summary>
        /// Indicates that the user cannot enter arbitrary text; they must pick from the dropdown.
        /// </summary>
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) => true;

        /// <summary>
        /// Indicates that this converter supports providing a list of standard values (dropdown).
        /// </summary>
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context) => true;

        /// <summary>
        /// Indicates that the converter can convert from a given source type.
        /// </summary>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
                return true;

            return base.CanConvertFrom(context, sourceType);
        }

        /// <summary>
        /// Indicates that the converter can convert to a given destination type.
        /// </summary>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
                return true;

            return base.CanConvertTo(context, destinationType);
        }

        /// <summary>
        /// Provides the list of types implementing IBeepUIComponent for the dropdown.
        /// </summary>
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            // Retrieve all non-abstract classes that implement IBeepUIComponent
            var componentTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly =>
                {
                    try
                    {
                        return assembly.GetTypes();
                    }
                    catch (ReflectionTypeLoadException ex)
                    {
                        return ex.Types.Where(t => t != null);
                    }
                    catch
                    {
                        // If an assembly fails to load, skip it
                        return new Type[0];
                    }
                })
                .Where(IsBeepUIComponentType)
                .ToList();

            // Build the mapping from FullName to Type
            _typeMap.Clear();
            foreach (var type in componentTypes)
            {
                _typeMap[type.FullName] = type;
            }

            // Return the list of FullNames for the PropertyGrid dropdown
            return new StandardValuesCollection(_typeMap.Keys.ToList());
        }

        /// <summary>
        /// Converts from a string (selected type name) to a Type object.
        /// </summary>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string typeName && _typeMap.TryGetValue(typeName, out var type))
            {
                return type;
            }

            return base.ConvertFrom(context, culture, value);
        }

        /// <summary>
        /// Converts a Type object to its string representation for display.
        /// </summary>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value is Type typeValue)
            {
                // Display the FullName. You can switch to typeValue.Name if preferred.
                return typeValue.FullName;
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        /// <summary>
        /// Helper method to check if a type implements IBeepUIComponent.
        /// </summary>
        private bool IsBeepUIComponentType(Type type)
        {
            return type != null
                   && type.IsClass
                   && !type.IsAbstract
                   && typeof(IBeepUIComponent).IsAssignableFrom(type);
        }
    }
}
