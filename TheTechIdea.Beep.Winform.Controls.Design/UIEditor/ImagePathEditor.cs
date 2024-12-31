using System;
using System.CodeDom;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using Microsoft.DotNet.DesignTools.Editors;
using System.Windows.Forms.Design;
using System.Drawing.Design;
using Microsoft.DotNet.DesignTools.Serialization;

namespace TheTechIdea.Beep.Winform.Controls.Design.UIEditor
{
    [CLSCompliant(false)]
    public class ImagePathEditor : UITypeEditor
    {
      
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
      

        public override object? EditValue(ITypeDescriptorContext? context, IServiceProvider provider, object? value)
        {
            
            string path = value as string;
            var editorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
            if (editorService != null)
            {
                using (var form = new BeepImageSelectorDialog())
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        path = form.SelectedImagePath;
                    }
                }
            }
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
