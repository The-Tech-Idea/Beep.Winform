using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
// Make sure you reference the namespace that defines Entity:
using TheTechIdea.Beep.Editor;

namespace TheTechIdea.Beep.Winform.Controls.Converters
{
    public class EntityTypeConverter : TypeConverter
    {
        private readonly Dictionary<string, Type> _typeMap = new();

        /// <summary>
        /// Indicate that the user cannot type arbitrary text (they must pick from the dropdown).
        /// </summary>
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) => true;

        /// <summary>
        /// Indicate that this converter supports providing a list of standard values (dropdown).
        /// </summary>
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context) => true;

        /// <summary>
        /// Tells the PropertyGrid that we know how to convert from 'string' (the dropdown) to 'Type'.
        /// </summary>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
                return true;

            return base.CanConvertFrom(context, sourceType);
        }

        /// <summary>
        /// Tells the PropertyGrid that we know how to convert to 'string' (to display in the grid).
        /// </summary>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
                return true;

            return base.CanConvertTo(context, destinationType);
        }

        /// <summary>
        /// Returns the list of valid entity types that the user can pick from in the dropdown.
        /// </summary>
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            // Retrieve all types that inherit from Entity (non-abstract).
            var entityTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly =>
                {
                    // Safely handle ReflectionTypeLoadExceptions:
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
                        // If an assembly fails, skip it
                        return new Type[0];
                    }
                })
                .Where(IsEntityType)
                .ToList();

            // Build the mapping from "type.FullName" => actual Type
            _typeMap.Clear();
            foreach (var type in entityTypes)
            {
                // Decide what you want to display in the dropdown: short name or full name
                _typeMap[type.FullName] = type;
            }

            // Return the list of keys for the PropertyGrid dropdown
            return new StandardValuesCollection(_typeMap.Keys.ToList());
        }

        /// <summary>
        /// Convert the selected string from the dropdown into a System.Type object.
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
        /// Convert a System.Type object to a string for displaying in the PropertyGrid.
        /// </summary>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value is Type typeValue)
            {
                // If you stored FullName in the dictionary, return FullName:
                return typeValue.FullName;
                // If you stored short name, return typeValue.Name;
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        /// <summary>
        /// Checks if a Type is a concrete class that inherits from Entity.
        /// </summary>
        private bool IsEntityType(Type type)
        {
            return type != null
                   && type.IsClass
                   && !type.IsAbstract
                   && typeof(Entity).IsAssignableFrom(type);
        }
    }
}
