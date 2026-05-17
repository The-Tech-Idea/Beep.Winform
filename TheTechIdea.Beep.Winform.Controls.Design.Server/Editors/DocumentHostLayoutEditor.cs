// DocumentHostLayoutEditor.cs
// Phase 06B — UITypeEditor that opens the read-only LayoutTreeDialog so the
// user can inspect a host's serialised layout (groups → splits → documents)
// directly from the Properties grid via the [...] button, in addition to the
// existing smart-tag verb path.
//
// The editor is intentionally read-only for now; the dialog itself is a
// passive viewer. A future iteration can swap in a drag-to-rearrange tree
// without changing the editor contract.
// ─────────────────────────────────────────────────────────────────────────────
using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using TheTechIdea.Beep.Winform.Controls.Design.Server.Dialogs;
using TheTechIdea.Beep.Winform.Controls.DocumentHost;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Editors
{
    /// <summary>
    /// <see cref="UITypeEditor"/> for <c>BeepDocumentHost.DesignTimeLayoutJson</c>
    /// and any other string property that represents a host's serialised
    /// layout. Opens <see cref="LayoutTreeDialog"/> with the host that owns
    /// the property so the user can inspect the live layout tree rather than
    /// the raw JSON.
    /// </summary>
    public sealed class DocumentHostLayoutEditor : UITypeEditor
    {
        public DocumentHostLayoutEditor() { }

        // Kept for [Editor] attribute compatibility with frameworks that pass
        // the property type as a constructor argument.
        public DocumentHostLayoutEditor(Type _) { }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext? context)
            => UITypeEditorEditStyle.Modal;

        public override object? EditValue(
            ITypeDescriptorContext? context,
            IServiceProvider?       provider,
            object?                 value)
        {
            // The Properties grid passes the host as context.Instance. Without
            // a live BeepDocumentHost we cannot render a meaningful tree, so
            // we fall back to the raw value untouched.
            if (context?.Instance is not BeepDocumentHost host)
                return value;

            var wfSvc = provider?.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;

            using var dlg = new LayoutTreeDialog(host);
            if (wfSvc != null)
                wfSvc.ShowDialog(dlg);
            else
                dlg.ShowDialog();

            // The dialog is read-only; the underlying value is unchanged.
            return value;
        }
    }
}
