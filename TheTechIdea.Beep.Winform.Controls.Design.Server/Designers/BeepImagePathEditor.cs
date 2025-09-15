using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using TheTechIdea.Beep.Winform.Controls;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    /// <summary>
    /// UITypeEditor that opens BeepImagePickerDialog to select an image path or embedded resource.
    /// </summary>
    public class BeepImagePathEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
            => UITypeEditorEditStyle.Modal;

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            try
            {
                // Try to get a BeepImage from the owning component if available
                BeepImage targetImage = null;

                // Attempt to use the owning component as a source for assembly scanning if it's a BeepImage or contains one
                if (context?.Instance is BeepImage img)
                {
                    targetImage = img;
                }

                using var dlg = new BeepImagePickerDialog(targetImage, embed: false, sp: provider);
                var result = dlg.ShowDialog();
                if (result == DialogResult.OK)
                {
                    // Prefer embedded resource path if chosen, else use file path
                    if (!string.IsNullOrWhiteSpace(dlg.SelectedResourcePath))
                        return dlg.SelectedResourcePath;
                    if (!string.IsNullOrWhiteSpace(dlg.SelectedFilePath))
                        return dlg.SelectedFilePath;
                }
                return value;
            }
            catch
            {
                // Fallback to default file dialog if the picker fails for any reason
                using var ofd = new OpenFileDialog { Filter = "Images|*.svg;*.png;*.jpg;*.jpeg;*.bmp|All Files|*.*" };
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    return ofd.FileName;
                }
                return value;
            }
        }
    }
}
