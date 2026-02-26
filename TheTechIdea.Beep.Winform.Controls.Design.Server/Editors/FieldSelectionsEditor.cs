using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Winform.Controls.Design.Server.Designers;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Editors
{
    internal sealed class FieldSelectionsEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext? context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public override object? EditValue(ITypeDescriptorContext? context, IServiceProvider? provider, object? value)
        {
            if (context?.Instance is not BeepDataBlock dataBlock)
            {
                return value;
            }

            var changeService = provider?.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
            using var editorForm = new BeepDataBlockFieldEditorForm(dataBlock);
            if (editorForm.ShowDialog() != System.Windows.Forms.DialogResult.OK)
            {
                return value;
            }

            var applied = editorForm.ApplySelections(changeService);
            if (applied)
            {
                try
                {
                    dataBlock.RefreshEntityMetadata(regenerateSurface: true);
                }
                catch
                {
                    // Keep property-grid editing resilient in design-time hosts.
                }
            }

            return value;
        }
    }
}
