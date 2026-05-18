// BeepDocumentHostDesigner.Verbs.cs
// Phase 03 — split out of BeepDocumentHostDesigner.cs.
//
// All design-time smart-tag actions (ActionLists) and verb menu entries live
// here. Each verb dispatches into the public design-time API exposed by the
// other partial classes (AddDesignTimeDocument, CloseActiveDesignTimeDocument,
// ApplyDesignTimeLayoutPreset, ShowLayoutAssistant, …) so they remain a thin
// declarative surface.
//
// A4-compatibility rule: when a BeepDocumentManager on the same form already
// owns the "Customize Keyboard Shortcuts…" verb, we disable our own copy so
// the developer only sees a single entry-point. See HasBoundManager().
// ─────────────────────────────────────────────────────────────────────────────
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.DocumentHost;
using TheTechIdea.Beep.Winform.Controls.Design.Server.ActionLists;
using TheTechIdea.Beep.Winform.Controls.Design.Server.Dialogs;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    public partial class BeepDocumentHostDesigner
    {
        // ── Smart-tag action lists ───────────────────────────────────────────

        private DesignerActionListCollection? _actionLists;

        /// <inheritdoc/>
        public override DesignerActionListCollection ActionLists
        {
            get
            {
                _actionLists ??= new DesignerActionListCollection
                {
                    new DocumentHostActionList(this)
                };
                return _actionLists;
            }
        }

        // ── Design-time verbs ────────────────────────────────────────────────

        private DesignerVerbCollection? _verbs;

        /// <summary>
        /// Cached reference so we can toggle <see cref="DesignerVerb.Enabled"/>
        /// when a <see cref="BeepDocumentManager"/> already surfaces this verb.
        /// </summary>
        private DesignerVerb? _shortcutsVerb;

        /// <inheritdoc/>
        public override DesignerVerbCollection Verbs
        {
            get
            {
                if (_verbs == null)
                {
                    _verbs = new DesignerVerbCollection
                    {
                        new DesignerVerb("Add Document", (s, e) =>
                        {
                            AddDesignTimeDocument();
                        }),
                        new DesignerVerb("Close Active Document", (s, e) =>
                        {
                            CloseActiveDesignTimeDocument();
                        }),
                        new DesignerVerb("Split With New Document \u2194", (s, e) =>
                        {
                            CreateSplitDesignTimeDocument(horizontal: true);
                        }),
                        new DesignerVerb("Split With New Document \u2195", (s, e) =>
                        {
                            CreateSplitDesignTimeDocument(horizontal: false);
                        }),
                        new DesignerVerb("Select Active Document Surface", (s, e) =>
                        {
                            SelectActiveDocumentSurface();
                        }),
                        new DesignerVerb("Select Tab Under Cursor", (s, e) =>
                        {
                            SelectDocumentAt(Cursor.Position);
                        }),
                        new DesignerVerb("Reopen Last Closed Document", (s, e) =>
                        {
                            ReopenLastClosedDesignTimeDocument();
                        }),
                        new DesignerVerb("Layout Assistant\u2026", (s, e) =>
                        {
                            ShowLayoutAssistant();
                        }),
                        new DesignerVerb("Merge All Groups", (s, e) =>
                        {
                            MergeAllDesignTimeGroups();
                        }),
                        new DesignerVerb("Edit Documents\u2026", (s, e) =>
                        {
                            if (Component is BeepDocumentHost host)
                                EditDesignTimeDocuments(host);
                        }),
                        new DesignerVerb("Apply Layout Preset\u2026", (s, e) =>
                        {
                            if (Component is BeepDocumentHost host)
                                PickAndApplyLayoutPreset(host);
                        }),
                        // Sprint 19: Nested split verbs.
                        new DesignerVerb("Set Group Tab Position\u2026", (s, e) =>
                        {
                            if (Component is BeepDocumentHost host && host.Groups.Count > 1)
                                EditGroupTabPositions(host);
                        }),
                        new DesignerVerb("View Layout Tree\u2026", (s, e) =>
                        {
                            if (Component is BeepDocumentHost host)
                                ShowLayoutTreeDialog(host);
                        }),
                        // Phase 7 designer verbs (P7-003).
                        new DesignerVerb("Export Layout Snapshot\u2026", (s, e) =>
                        {
                            if (Component is BeepDocumentHost host)
                            {
                                var json = host.SaveLayout();
                                Clipboard.SetText(json);
                                MessageBox.Show(
                                    "Layout snapshot copied to clipboard.",
                                    "Export Layout Snapshot",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                            }
                        }),
                        new DesignerVerb("Clear All Documents", (s, e) =>
                        {
                            var r = MessageBox.Show(
                                "Remove all document panels from the host?",
                                "Clear All Documents",
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Warning);
                            if (r == DialogResult.Yes)
                                CloseAllDesignTimeDocuments();
                        }),
                        new DesignerVerb("Customize Keyboard Shortcuts\u2026", (s, e) =>
                        {
                            if (Component is BeepDocumentHost host)
                            {
                                var registry = host.CommandRegistry;
                                using var dlg = new DocumentHostShortcutEditorDialog(registry);
                                dlg.ShowDialog();
                            }
                        }),
                    };

                    _shortcutsVerb = (DesignerVerb)_verbs[_verbs.Count - 1];
                }

                // A4: suppress host-level verb when BeepDocumentManager on the
                // same form already provides "Customize Keyboard Shortcuts…".
                if (_shortcutsVerb != null)
                    _shortcutsVerb.Enabled = !HasBoundManager();

                return _verbs;
            }
        }

        /// <summary>
        /// Returns <see langword="true"/> when a <see cref="BeepDocumentManager"/>
        /// on the same design surface has its
        /// <see cref="BeepTabbedView.Host"/> set to this
        /// <see cref="BeepDocumentHost"/>.
        /// </summary>
        private bool HasBoundManager()
        {
            if (Component is not BeepDocumentHost host) return false;
            var container = host.Site?.Container;
            if (container == null) return false;

            foreach (IComponent comp in container.Components)
            {
                if (comp is BeepDocumentManager mgr &&
                    (mgr.View as BeepTabbedView)?.Host == host)
                    return true;
            }
            return false;
        }
    }
}
