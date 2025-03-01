using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using TheTechIdea.Beep.Winform.Controls.Design.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Design
{
    public class EnumTypeEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            using (EnumSelectorForm form = new EnumSelectorForm())
            {
                form.SelectedEnumType = value as string;
                if (form.ShowDialog() == DialogResult.OK)
                {
                    return form.SelectedEnumType;
                }
            }
            return value;
        }
    }
}
