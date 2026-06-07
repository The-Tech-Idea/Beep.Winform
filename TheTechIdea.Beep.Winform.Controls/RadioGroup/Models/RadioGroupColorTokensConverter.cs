using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;

namespace TheTechIdea.Beep.Winform.Controls.RadioGroup.Models
{
    /// <summary>
    /// Property-grid type converter for <see cref="RadioGroupColorTokens"/>.
    /// Exposes every token as a <see cref="PropertyDescriptor"/> so the property grid
    /// (and any <see cref="ExpandableObjectConverter"/> host) can edit individual roles.
    /// </summary>
    public sealed class RadioGroupColorTokensConverter : ExpandableObjectConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
            => destinationType == typeof(string) || base.CanConvertTo(context, destinationType);

        public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
        {
            if (destinationType == typeof(string) && value is RadioGroupColorTokens)
                return "Color Tokens";
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext? context, object? value, Attribute[]? attributes)
        {
            // Always expose the public Color token properties in alphabetical order.
            var props = TypeDescriptor.GetProperties(value, attributes)
                .Cast<PropertyDescriptor>()
                .Where(p => p.PropertyType == typeof(System.Drawing.Color))
                .OrderBy(p => p.Name)
                .ToArray();
            return new PropertyDescriptorCollection(props);
        }

        public override bool GetPropertiesSupported(ITypeDescriptorContext? context) => true;
    }
}
