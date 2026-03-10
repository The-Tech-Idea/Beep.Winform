using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Images;

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
            string? currentPath = value as string;
            var ownerControl = ExtractControl(context?.Instance);
            var ownerImage = ownerControl as BeepImage;
            var resourceAssembly = ownerControl?.GetType().Assembly ?? context?.PropertyDescriptor?.ComponentType?.Assembly;
            using var dialog = new BeepImagePickerDialog(ownerImage, embed: false, provider, resourceAssembly, currentPath);
            var result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                if (!dialog.SelectionResult.IsCancelled && !string.IsNullOrWhiteSpace(dialog.SelectionResult.SelectedPath))
                {
                    return dialog.SelectionResult.SelectedPath;
                }
            }

            return value;
        }

        private static Control? ExtractControl(object? instance)
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
