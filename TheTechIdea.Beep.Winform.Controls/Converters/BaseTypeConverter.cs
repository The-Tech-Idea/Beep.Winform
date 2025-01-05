using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace TheTechIdea.Beep.Winform.Controls.Converters
{
    public abstract class BaseTypeConverter : TypeConverter
    {
        private readonly Dictionary<string, Type> _typeMap = new();
        private readonly Type _targetType;

        protected BaseTypeConverter(Type targetType)
        {
            _targetType = targetType;
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) => true;

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context) => true;

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
                return true;

            return base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
                return true;

            return base.CanConvertTo(context, destinationType);
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            var types = AppDomain.CurrentDomain.GetAssemblies()
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
                        return new Type[0];
                    }
                })
                .Where(t => _targetType.IsAssignableFrom(t) && t.IsClass && !t.IsAbstract)
                .ToList();

            _typeMap.Clear();
            foreach (var type in types)
            {
                _typeMap[type.FullName] = type;
            }

            return new StandardValuesCollection(_typeMap.Keys.ToList());
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string typeName && _typeMap.TryGetValue(typeName, out var type))
            {
                return type;
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value is Type typeValue)
            {
                return typeValue.FullName;
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    //// Specific converter for IBeepUIComponent
    //public class BeepUIComponentTypeConverter : BaseTypeConverter
    //{
    //    public BeepUIComponentTypeConverter() : base(typeof(IBeepUIComponent))
    //    {
    //    }
    //}

    //// Specific converter for Entity
    //public class EntityTypeConverter : BaseTypeConverter
    //{
    //    public EntityTypeConverter() : base(typeof(Entity)) // Replace 'Entity' with the actual base class
    //    {
    //    }
    //}
}
