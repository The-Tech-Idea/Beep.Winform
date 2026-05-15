// BeepDocumentPanelDesigner.cs
// Design-time designer for BeepDocumentPanel.
//
// Clicking a tab header in BeepDocumentHostDesigner calls
// ISelectionService.SetSelectedComponents(panel) which triggers this designer.
// It narrows the Properties window to only the document-relevant properties
// (Title, Icon, CanClose, Category, ShowStatusBar) — identical to the
// per-tab property experience in DevExpress XtraTabbedView.
// ─────────────────────────────────────────────────────────────────────────────
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Microsoft.DotNet.DesignTools.Designers;
using Microsoft.DotNet.DesignTools.Designers.Behaviors;
using TheTechIdea.Beep.Winform.Controls.DocumentHost;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    /// <summary>
    /// Design-time designer for <see cref="BeepDocumentPanel"/>.
    /// <para>
    /// Filters the Properties window so it shows only the properties that are
    /// meaningful for configuring a single document tab at design time:
    /// <list type="bullet">
    ///   <item><c>DocumentTitle</c> — text on the tab header</item>
    ///   <item><c>IconPath</c> — optional icon on the tab</item>
    ///   <item><c>CanClose</c> — whether the × button is shown</item>
    ///   <item><c>DocumentCategory</c> — overflow-dropdown grouping label</item>
    ///   <item><c>ShowStatusBar</c> — show/hide the panel's own mini status bar</item>
    /// </list>
    /// All other inherited WinForms / BaseControl properties are suppressed.
    /// </para>
    /// </summary>
    public class BeepDocumentPanelDesigner : ParentControlDesigner
    {
        // ── Properties that SHOULD be visible in the Properties window ────────

        private static readonly HashSet<string> _visibleProperties
            = new HashSet<string>(StringComparer.Ordinal)
        {
            nameof(BeepDocumentPanel.DocumentTitle),
            nameof(BeepDocumentPanel.IconPath),
            nameof(BeepDocumentPanel.CanClose),
            nameof(BeepDocumentPanel.DocumentCategory),
            nameof(BeepDocumentPanel.ShowStatusBar),
        };

        // ── Properties grid filtering ─────────────────────────────────────────

        protected override void PreFilterProperties(IDictionary properties)
        {
            base.PreFilterProperties(properties);

            // Collect all keys that should be removed
            var toRemove = new List<string>(properties.Count);
            foreach (DictionaryEntry entry in properties)
            {
                if (entry.Key is string key && !_visibleProperties.Contains(key))
                    toRemove.Add(key);
            }

            foreach (var key in toRemove)
                properties.Remove(key);
        }

        // ── Prevent children from being moved / resized by the designer ───────

        public override SelectionRules SelectionRules
            => SelectionRules.None;                          // no resize handles

        /// <inheritdoc/>
        public override bool CanBeParentedTo(IDesigner parentDesigner)
            => parentDesigner?.Component is BeepDocumentHost;

        // ── Snap-lines: none — the panel fills whatever space the host gives it ─
        public override System.Collections.Generic.IList<SnapLine> SnapLines
            => new List<SnapLine>();

        // ── Initialize: lock all children so they do not interfere ────────────

        public override void Initialize(IComponent component)
        {
            base.Initialize(component);

            if (component is not BeepDocumentPanel panel) return;

            // Children added via toolbox-drop are routed here by BeepDocumentHostDesigner.
            // Lock any pre-existing children (e.g., design-time content placeholders).
            foreach (Control child in panel.Controls)
                LockPanelChild(child);

            panel.ControlAdded   += Panel_ControlAdded;
            panel.ControlRemoved += Panel_ControlRemoved;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && Component is BeepDocumentPanel panel)
            {
                panel.ControlAdded   -= Panel_ControlAdded;
                panel.ControlRemoved -= Panel_ControlRemoved;
            }

            base.Dispose(disposing);
        }

        private void Panel_ControlAdded(object? sender, ControlEventArgs e)
        {
            if (e.Control != null) LockPanelChild(e.Control);
        }

        private void Panel_ControlRemoved(object? sender, ControlEventArgs e) { /* nothing */ }

        private void LockPanelChild(Control child)
        {
            TypeDescriptor.AddAttributes(child,
                new ReadOnlyAttribute(true));
        }
    }
}
