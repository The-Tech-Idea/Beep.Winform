using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace TheTechIdea.Beep.Winform.Controls.Forms.Caption.Design
{
    public class CaptionRendererKindEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
            => UITypeEditorEditStyle.DropDown;

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (provider.GetService(typeof(IWindowsFormsEditorService)) is not IWindowsFormsEditorService edSvc)
                return value;

            var listBox = new ListBox { BorderStyle = BorderStyle.None, IntegralHeight = true };            
            listBox.Items.AddRange(new object[]
            {
                (CaptionRendererKind.Windows, "Windows (Default)"),
                (CaptionRendererKind.MacLike, "macOS-like"),
                (CaptionRendererKind.Gnome,   "GNOME / Adwaita"),
                (CaptionRendererKind.Kde,     "KDE / Breeze"),
            });
            listBox.DisplayMember = "Item2";
            listBox.ValueMember = "Item1";

            listBox.Click += (s, e) => edSvc.CloseDropDown();

            edSvc.DropDownControl(listBox);

            if (listBox.SelectedItem is ValueTuple<CaptionRendererKind, string> selected)
                return selected.Item1;

            return value;
        }
    }
}
