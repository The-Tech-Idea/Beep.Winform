using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Converters
{
    public class DataBlockConverter : TypeConverter
    {
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) => true;

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context) => true;

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            if (context?.Container == null) return new StandardValuesCollection(Array.Empty<object>());

            var blocks = context.Container.Components
                .OfType<BeepDataBlock>()
                .Where(block => block != context.Instance) // Exclude the current instance
                .ToList();

            blocks.Insert(0, null); // Add null as the first option

            return new StandardValuesCollection(blocks);
        }


        private IContainer GetContainerFromContext(ITypeDescriptorContext context)
        {
            if (context.Container != null) return context.Container;

            // Try to resolve the container from the instance's parent
            if (context.Instance is Control control && control.TopLevelControl != null)
            {
                return control.TopLevelControl.Site?.Container;
            }

            return null;
        }

        // Override ConvertTo to display the BeepDataBlock as a string (e.g., its Name property)
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
                return true;
            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value is BeepDataBlock dataBlock)
            {
                // Return the Name of the data block, or any other identifying property
                return dataBlock.Name;
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        // Override ConvertFrom to convert the selected string back to the BeepDataBlock instance
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
                return true;
            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value is null || value.ToString().Trim() == "")
            {
                // Allow null to be explicitly set
                return null;
            }

            if (value is string name)
            {
                var container = context?.Container;
                if (container != null)
                {
                    var block = container.Components
                        .OfType<BeepDataBlock>()
                        .FirstOrDefault(b => b.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));

                    if (block == null)
                    {
                        throw new ArgumentException($"BeepDataBlock with Name '{name}' not found.");
                    }

                    return block;
                }
            }

            return base.ConvertFrom(context, culture, value);
        }

    }
}
