using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.ComponentModel.TypeConverter;

namespace TheTechIdea.Beep.Winform.Controls.Converters
{
    public class TabPageListConverter : TypeConverter
    {
        // We want a drop-down of TabPages
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context) => true;
        // We force user to pick from the drop-down (rather than typing arbitrary text)
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) => true;

        // The property grid calls this to get a list of possible values
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            // If the property is on a control that references a TabControl
            // (e.g. "BeepTabHeaderControl" or "TabControlWithoutHeader"), 
            // we must get the actual TabPages from there:
            if (context?.Instance is BeepTabHeaderControl header && header.TargetTabControl != null)
            {
                var pages = header.TargetTabControl.TabPages.OfType<TabPage>().ToList();
                return new StandardValuesCollection(pages);
            }
            else if (context?.Instance is TabControlWithoutHeader tabControl)
            {
                var pages = tabControl.TabPages.OfType<TabPage>().ToList();
                return new StandardValuesCollection(pages);
            }

            return base.GetStandardValues(context);
        }

        // Determines if we can convert from e.g., string -> TabPage
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
                return true;
            return base.CanConvertFrom(context, sourceType);
        }

        // Convert the string (e.g. "tabPage2") back into a TabPage object
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string s)
            {
                // We must find the TabPage in whichever control has the property
                if (context?.Instance is BeepTabHeaderControl header && header.TargetTabControl != null)
                {
                    // Match by Name
                    foreach (TabPage page in header.TargetTabControl.TabPages)
                    {
                        if (page.Name == s)
                            return page;
                    }
                }
                else if (context?.Instance is TabControlWithoutHeader tabControl)
                {
                    foreach (TabPage page in tabControl.TabPages)
                    {
                        if (page.Name == s)
                            return page;
                    }
                }

                // If not found, just return null or throw an exception
                return null;
            }
            return base.ConvertFrom(context, culture, value);
        }

        // Determines if we can convert from e.g., TabPage -> string
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
                return true;
            return base.CanConvertTo(context, destinationType);
        }

        // Convert the TabPage object to its Name (string) for serialization
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value is TabPage page && destinationType == typeof(string))
            {
                // Use page.Name (or page.Text) as the string representation in .Designer.cs
                return page.Name;
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

    }
}
