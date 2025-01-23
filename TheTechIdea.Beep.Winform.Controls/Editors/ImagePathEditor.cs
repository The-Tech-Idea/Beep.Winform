using System;
using System.CodeDom;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using TheTechIdea.Beep.Winform.Controls.Design.UIEditor;

namespace TheTechIdea.Beep.Winform.Controls.Editors
{
    [CLSCompliant(false)]
    public class ImagePathEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            // Indicate that this editor uses a modal dialog
            return UITypeEditorEditStyle.Modal;
        }

        public override object? EditValue(ITypeDescriptorContext? context, IServiceProvider provider, object? value)
        {
            // Ensure the value is a string
            string path = value as string ?? string.Empty;

            // Get the editor service
            var editorService = provider?.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
            if (editorService == null)
            {
                return path; // If no editor service is available, return the original value
            }

            // Config the image selector dialog
            using (var form = new ImageSelectorImporterDialog(path))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    // Update the path if the user clicks OK
                    path = form.SelectedImagePath;
                }
            }

            // Return the updated or original path
            return path;
        }
    }

    internal class ImagePathCodeDomSerializer : CodeDomSerializer
    {
        public override object Serialize(IDesignerSerializationManager manager, object value)
        {
            if (value is string imagePath)
            {
                // Serialize the LogoImage as a string assignment
                var assignImagePath = new CodeAssignStatement(
                    new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), "LogoImage"),
                    new CodePrimitiveExpression(imagePath));

                return new CodeStatementCollection { assignImagePath };
            }

            return base.Serialize(manager, value);
        }
    }


}
