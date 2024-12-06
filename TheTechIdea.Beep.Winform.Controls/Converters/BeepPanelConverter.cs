using System;
using System.ComponentModel;
using System.Linq;

namespace TheTechIdea.Beep.Winform.Controls.Converters
{
    public class BeepPanelConverter : TypeConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true; // Enable dropdown list in Properties Window
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true; // Force selection from the list (no free typing)
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            if (context?.Instance is BeepTabs_old tabs)
            {
                // Get the list of BeepPanel instances in the TabPanels collection
              //  Console.WriteLine($"GetStandardValues : {tabs.TabPanels.ToList().Count()}");
                return new StandardValuesCollection(tabs.TabPanels.ToList());
            }
            return new StandardValuesCollection(Array.Empty<object>());
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (context?.Instance is BeepTabs_old tabs && value is string stringValue)
            {
                // Match the TitleText to a BeepPanel instance
                return tabs.TabPanels.FirstOrDefault(panel => panel.TitleText == stringValue);
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value is BeepPanel panel)
            {
                // Show TitleText of the BeepPanel in the dropdown
                return panel.TitleText;
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
