using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using TheTechIdea.Beep.Winform.Controls.Design.Server.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Editors
{
    /// <summary>
    /// UITypeEditor for selecting UserControl for wizard step content
    /// Provides a dialog to browse and select UserControl types
    /// </summary>
    public class WizardStepContentEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext? context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public override object? EditValue(ITypeDescriptorContext? context, IServiceProvider provider, object? value)
        {
            if (provider == null || context == null)
                return value;

            var editorService = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
            if (editorService == null)
                return value;

            // Create a dialog to select UserControl
            using (var dialog = new OpenFileDialog
            {
                Title = "Select UserControl for Wizard Step",
                Filter = "UserControl Files (*.cs)|*.cs|All Files (*.*)|*.*",
                CheckFileExists = true
            })
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    // In a full implementation, this would:
                    // 1. Parse the file to find UserControl classes
                    // 2. Show a list of available UserControls
                    // 3. Return the selected UserControl instance or type
                    // For now, return the file path as a placeholder
                    return dialog.FileName;
                }
            }

            return value;
        }
    }
}
