using System.ComponentModel.Design;
using TheTechIdea.Beep.Winform.Controls.Common;



namespace TheTechIdea.Beep.Winform.Controls.Editors
{

    public class MenuItemCollectionEditor : CollectionEditor
    {
        public MenuItemCollectionEditor(Type type) : base(type) { }

        protected override Type CreateCollectionItemType()
        {
            return typeof(SimpleItem);  // Specify the type for collection rootnodeitems
        }

        protected override object CreateInstance(Type itemType)
        {
            return new SimpleItem();  // Create an instance of SimpleMenuItem
        }
    }


    //public class MenuItemEditor : UITypeEditor
    //{
    //    // Override EditValue to display the custom form
    //    public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
    //    {
    //        if (value is BindingList<SimpleItem> menuItems)
    //        {
    //            IWindowsFormsEditorService editorService = provider?.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;

    //            if (editorService != null)
    //            {
    //                using (MenuItemEditorForm form = new MenuItemEditorForm(menuItems))
    //                {
    //                    if (editorService.ShowDialog(form) == System.Windows.Forms.DialogResult.OK)
    //                    {
    //                        return form.MenuItems; // Return updated MenuItems
    //                    }
    //                }
    //            }
    //        }

    //        return base.EditValue(context, provider, value);
    //    }

    //    public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
    //    {
    //        // Indicate that this editor displays a modal dialog
    //        return UITypeEditorEditStyle.Modal;
    //    }
    //}
}
