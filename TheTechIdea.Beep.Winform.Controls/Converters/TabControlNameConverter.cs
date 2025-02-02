using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.ComponentModel.TypeConverter;

namespace TheTechIdea.Beep.Winform.Controls.Converters
{
    public class TabControlNameConverter : TypeConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context) => true;
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) => true;

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            if (context?.Instance is BeepTabHeaderControl headerControl && headerControl.Parent != null)
            {
                // Gather all TabControlWithoutHeader controls by their Name
                var allTabControls = headerControl.Parent
                    .Controls
                    .OfType<TabControlWithoutHeader>()
                    .Where(tch => !string.IsNullOrEmpty(tch.Name))
                    .Select(tch => tch.Name)
                    .ToList();

                return new StandardValuesCollection(allTabControls);
            }
            return base.GetStandardValues(context);
        }
    }
}
