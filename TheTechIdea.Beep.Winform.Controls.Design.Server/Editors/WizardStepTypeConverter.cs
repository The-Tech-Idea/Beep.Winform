using System;
using System.ComponentModel;
using TheTechIdea.Beep.Winform.Controls.Wizards;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Editors
{
    /// <summary>
    /// Type converter for WizardStep to enable expandable properties in designer
    /// </summary>
    public class WizardStepTypeConverter : ExpandableObjectConverter
    {
        public override bool GetPropertiesSupported(ITypeDescriptorContext? context)
        {
            return true;
        }

        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext? context, object value, Attribute[]? attributes)
        {
            return TypeDescriptor.GetProperties(typeof(WizardStep), attributes);
        }
    }
}
