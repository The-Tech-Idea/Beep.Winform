using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Editors
{
 // Lightweight in-assembly editor to guarantee an ImagePath picker shows up in the Properties window
 public class BeepImagePathEditorSimple : UITypeEditor
 {
 public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
 => UITypeEditorEditStyle.Modal;

 public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
 {
 try
 {
 using var ofd = new OpenFileDialog
 {
 Filter = "Images|*.svg;*.png;*.jpg;*.jpeg;*.bmp|All Files|*.*",
 Title = "Select Image"
 };
 if (value is string s && !string.IsNullOrWhiteSpace(s))
 {
 try { ofd.FileName = s; } catch { }
 }
 if (ofd.ShowDialog() == DialogResult.OK)
 {
 return ofd.FileName;
 }
 return value;
 }
 catch
 {
 return value;
 }
 }
 }
}
