using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using TheTechIdea.Beep.Winform.Controls;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    /// <summary>
    /// Design-time editor that launches the shared image picker dialog.
    /// </summary>
    internal sealed class BeepImagePathEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
            => UITypeEditorEditStyle.Modal;

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            var owningControl = ExtractControl(context?.Instance);
            var serviceProvider = provider ?? owningControl?.Site;
            var resourceAssembly = owningControl?.GetType().Assembly ?? context?.PropertyDescriptor?.ComponentType?.Assembly;

            using var dialog = new BeepImagePickerDialog(null, embed: false, serviceProvider, resourceAssembly);

            // Don't use IUIService - just show the dialog directly
            var result = dialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                if (!string.IsNullOrWhiteSpace(dialog.SelectedResourcePath))
                {
                    return dialog.SelectedResourcePath;
                }

                if (!string.IsNullOrWhiteSpace(dialog.SelectedFilePath))
                {
                    return dialog.SelectedFilePath;
                }
            }

            return value;
        }

        private static Control ExtractControl(object instance)
        {
            return instance switch
            {
                Control control => control,
                IComponent component when component is Control controlComponent => controlComponent,
                object[] array => array.OfType<Control>().FirstOrDefault(),
                _ => null
            };
        }
    }
}
