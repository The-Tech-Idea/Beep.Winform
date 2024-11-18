using System;
using System.CodeDom;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace TheTechIdea.Beep.Winform.Controls.UIEditor
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
                using (var form = new ImageSelectorImporterForm(path))
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

    internal class ImagePathActionList : DesignerActionList
    {
        private readonly BeepImage _control;

        public ImagePathActionList(BeepImage control) : base(control)
        {
            _control = control;
        }

        public string ImagePath
        {
            get => _control.ImagePath;
            set => TypeDescriptor.GetProperties(_control)["LogoImage"].SetValue(_control, value);
        }

        public void EditImagePath()
        {
            var uiService = (IUIService)GetService(typeof(IUIService));
            if (uiService != null)
            {
                using (var form = new ImageSelectorImporterForm(_control.ImagePath))
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        _control.ImagePath = form.SelectedImagePath;
                    }
                }
            }
        }

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            var items = new DesignerActionItemCollection
            {
                new DesignerActionPropertyItem("LogoImage", "Image Path", "Properties", "Set the image path"),
                new DesignerActionMethodItem(this, nameof(EditImagePath), "Edit Image Path...", "Actions", "Open image path editor")
            };
            return items;
        }
    }
}
