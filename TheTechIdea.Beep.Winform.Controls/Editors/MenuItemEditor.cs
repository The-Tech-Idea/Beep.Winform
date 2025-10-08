using System.ComponentModel.Design;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Models;
using System.ComponentModel;



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

        // Ensure the control refreshes after editing the collection at design time
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            var result = base.EditValue(context, provider, value);

            try
            {
                if (context?.Instance is TheTechIdea.Beep.Winform.Controls.BeepTree tree)
                {
                    // Trigger a refresh so newly added nodes are drawn in the designer
                    tree.RefreshTree();
                }
            }
            catch { /* ignore design-time exceptions */ }

            return result;
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
    //                    if (editorService.ShowDialog(form) == System.Windows.Forms.BeepDialogResult.OK)
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
