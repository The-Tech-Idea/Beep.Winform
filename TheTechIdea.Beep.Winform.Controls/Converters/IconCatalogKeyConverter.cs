using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using TheTechIdea.Beep.Icons;

namespace TheTechIdea.Beep.Winform.Controls.Converters
{
    public class IconCatalogKeyConverter : StringConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context) => true;
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) => false;

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            var keys = IconCatalog.GetAllEntries()
                .Select(e => e.Key)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(k => k, StringComparer.OrdinalIgnoreCase)
                .ToList();

            keys.Insert(0, "(None)");
            return new StandardValuesCollection(keys);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string str && string.Equals(str, "(None)", StringComparison.OrdinalIgnoreCase))
            {
                return string.Empty;
            }

            return base.ConvertFrom(context, culture, value);
        }
    }
}
