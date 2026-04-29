using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using TheTechIdea.Beep.Winform.Controls.GridX;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Editors
{
    /// <summary>
    /// Collection editor for BeepGridPro columns.
    /// Provides a visual dialog for adding, removing, and configuring grid columns.
    /// </summary>
    public class BeepGridColumnCollectionEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (context?.Instance is not BeepGridPro grid)
                return value;

            var editorService = (IWindowsFormsEditorService?)provider?.GetService(typeof(IWindowsFormsEditorService));
            if (editorService == null)
                return value;

            using var dialog = new BeepGridColumnEditorDialog(grid);
            var result = editorService.ShowDialog(dialog);

            if (result == DialogResult.OK)
            {
                // Force layout recalculation and repaint
                var layoutHelper = typeof(BeepGridPro).GetProperty("Layout", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)?.GetValue(grid);
                layoutHelper?.GetType().GetMethod("Recalculate", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic)?.Invoke(layoutHelper, null);
                grid.Invalidate();
                grid.Refresh();
            }

            return value;
        }
    }
}
