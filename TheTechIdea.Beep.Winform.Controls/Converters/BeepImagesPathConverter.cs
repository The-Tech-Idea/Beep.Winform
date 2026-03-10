using System;
using System.ComponentModel;
using System.Globalization;
using TheTechIdea.Beep.Icons;
using System.Linq;
using System.Collections.Generic;

namespace TheTechIdea.Beep.Winform.Controls.Converters
{
    /// <summary>
    /// TypeConverter for all classes icon under namespace TheTechIdea.Beep.Icons to enable dropdown selection in Visual Studio designer
    /// </summary>
    public class BeepImagesPathConverter : StringConverter
    {
        private static readonly StringComparer Cmp = StringComparer.OrdinalIgnoreCase;
        private static Dictionary<string, string> BuildLabelToPathMap()
        {
            return IconCatalog.GetAllEntries()
                .GroupBy(e => e.Path, Cmp)
                .Select(g => g.First())
                .ToDictionary(
                    e => $"[{e.Source}/{e.Category}] {e.Name}",
                    e => e.Path,
                    Cmp);
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            var labels = new List<string> { "(None)" };
            labels.AddRange(BuildLabelToPathMap().Keys.OrderBy(k => k, StringComparer.OrdinalIgnoreCase));
            return new StandardValuesCollection(labels.ToArray());
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

                var map = BuildLabelToPathMap();
                if (map.TryGetValue(stringValue, out var path))
                {
                    return path;
                }

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

                var map = BuildLabelToPathMap();
                var label = map.FirstOrDefault(kvp => Cmp.Equals(kvp.Value, stringValue)).Key;
                return string.IsNullOrEmpty(label) ? stringValue : label;
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}