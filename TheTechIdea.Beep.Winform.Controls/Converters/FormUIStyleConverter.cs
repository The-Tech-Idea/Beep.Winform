using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.Converters
{
    /// <summary>
    /// Type converter for FormUIStyle enum to enable dropdown in designer
    /// </summary>
    public class FormUIStyleConverter : EnumConverter
    {
        public FormUIStyleConverter() : base(typeof(FormUIStyle))
        {
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(Enum.GetValues(typeof(FormUIStyle)));
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }
    }
}
