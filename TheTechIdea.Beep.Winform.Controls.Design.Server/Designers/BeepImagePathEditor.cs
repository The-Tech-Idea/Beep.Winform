using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using TheTechIdea.Beep.Winform.Controls.Images;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    /// <summary>
    /// Universal design-time editor for any control with an ImagePath property.
    /// Launches the shared image picker dialog with browse, preview, and embed-to-resources support.
    /// Works with BeepImage, BeepLabel, BeepButton, BeepTextBox, BeepCard, BeepCheckBox, etc.
    /// </summary>
    internal sealed class BeepImagePathEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
            => UITypeEditorEditStyle.Modal;

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            string currentPath = value as string ?? string.Empty;
            var ownerControl = ExtractControl(context?.Instance);
            var ownerImage = ownerControl as BeepImage;
            var resourceAssembly = ResolveResourceAssembly(context, ownerControl);

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

        /// <summary>
        /// Extracts the Control instance from the property grid context.
        /// Handles single controls, multi-selection arrays, and component instances.
        /// </summary>
        private static Control ExtractControl(object instance)
        {
            return instance switch
            {
                Control control => control,
                IComponent component when component is Control c => c,
                object[] array => array.OfType<Control>().FirstOrDefault(),
                _ => null
            };
        }

        /// <summary>
        /// Resolves the best assembly to search for embedded resources.
        /// Prefers the host project assembly, falls back to the control's assembly.
        /// </summary>
        private static Assembly ResolveResourceAssembly(ITypeDescriptorContext context, Control ownerControl)
        {
            // Try to get the host project assembly from the designer
            if (context?.PropertyDescriptor?.ComponentType?.Assembly != null)
                return context.PropertyDescriptor.ComponentType.Assembly;

            // Fall back to the owner control's assembly
            if (ownerControl?.GetType().Assembly != null)
                return ownerControl.GetType().Assembly;

            return null;
        }
    }
}
