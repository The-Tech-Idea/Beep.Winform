using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.ComponentModel.TypeConverter;
using TheTechIdea.Beep.Desktop.Common;

namespace TheTechIdea.Beep.Winform.Controls.Converters
{
    public class TabItemConverter : TypeConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true; // Supports a list of standard values
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true; // Values are exclusive (drop-down only)
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            if (context?.Instance is BeepDynamicTabControl control)
            {
                // Return the list of SimpleItem objects from the Tabs property
                return new StandardValuesCollection(control.Tabs?.ToList());
            }

            return new StandardValuesCollection(Array.Empty<SimpleItem>());
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string name && context?.Instance is BeepDynamicTabControl control)
            {
                // Find the tab by its Text property
                return control.Tabs?.FirstOrDefault(item => item.Text == name);
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value is SimpleItem item)
            {
                return item.Text; // Display the tab name in the designer
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
