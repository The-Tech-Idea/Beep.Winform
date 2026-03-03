using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace TheTechIdea.Beep.Winform.Controls.Steppers.Painters
{
    public sealed class StepperPainterNameConverter : StringConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context) => true;

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) => false;

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            var names = StepperPainterRegistry.GetPainterNames()
                .OrderBy(n => n, StringComparer.OrdinalIgnoreCase)
                .ToList();
            return new StandardValuesCollection(names);
        }

        public override bool IsValid(ITypeDescriptorContext context, object value)
        {
            if (value is string name && !string.IsNullOrWhiteSpace(name))
            {
                return StepperPainterRegistry.GetPainterNames()
                    .Any(n => string.Equals(n, name, StringComparison.OrdinalIgnoreCase));
            }

            return base.IsValid(context, value);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string name)
            {
                return name.Trim();
            }

            return base.ConvertFrom(context, culture, value);
        }
    }
}
