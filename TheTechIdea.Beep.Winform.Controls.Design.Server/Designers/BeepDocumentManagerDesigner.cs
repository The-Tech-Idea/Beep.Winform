// BeepDocumentManagerDesigner.cs
// ComponentDesigner for BeepDocumentManager — smart-tag and designer verbs.
// Phase 03: View-centric (IBeepDocumentManagerView dropdown + quick-switch verbs).
// Phase 04: Documents collection editor, Tab style/position/close-mode pickers,
//           policy flag checkboxes.
// ─────────────────────────────────────────────────────────────────────────────────────────
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using TheTechIdea.Beep.Winform.Controls.DocumentHost;
using TheTechIdea.Beep.Winform.Controls.Design.Server.Dialogs;
using TheTechIdea.Beep.Winform.Controls.Design.Server.Editors;
using Microsoft.DotNet.DesignTools.Designers;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    /// <summary>
    /// VS designer for <see cref="BeepDocumentManager"/> — lives in the component tray.
    /// Provides smart-tag action list and verbs for adding documents, editing the
    /// design-time documents collection, resetting layout, and switching the active
    /// <see cref="IBeepDocumentManagerView"/>.
    /// </summary>
    public sealed class BeepDocumentManagerDesigner : ComponentDesigner
    {
        private BeepDocumentManager?          _manager;
        private DesignerVerbCollection?       _verbs;
        private DesignerActionListCollection? _actionLists;

        // ── Initialize / Dispose ──────────────────────────────────────────────

        public override void Initialize(IComponent component)
        {
            base.Initialize(component);
            _manager = component as BeepDocumentManager;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _manager = null;
            base.Dispose(disposing);
        }

        // ── Designer verbs ────────────────────────────────────────────────────

        public override DesignerVerbCollection Verbs
        {
            get
            {
                if (_verbs == null)
                {
                    _verbs = new DesignerVerbCollection
                    {
                        new DesignerVerb("Edit Documents\u2026",             OnEditDocuments),
                        new DesignerVerb("Add Document",                   OnAddDocument),
                        new DesignerVerb("Clear All Documents",            OnClearDocuments),
                        new DesignerVerb("Reset Layout",                   OnResetLayout),
                        new DesignerVerb("Use Tabbed View",                OnUseTabbedView),
                        new DesignerVerb("Use Native MDI View",            OnUseNativeMdiView),
                        new DesignerVerb("Customize Keyboard Shortcuts\u2026", OnCustomizeShortcuts),
                        new DesignerVerb("Manage Workspaces\u2026",            OnManageWorkspaces),
                    };
                }
                return _verbs;
            }
        }

        private void OnEditDocuments(object? sender, EventArgs e)
        {
            if (_manager == null) return;
            OpenDesignTimeDocumentsEditor();
        }

        private void OnAddDocument(object? sender, EventArgs e)
        {
            if (_manager?.View == null) return;
            using var txn = DesignerHost?.CreateTransaction("Add Document");
            try
            {
                _manager.AddDocument($"Document {Environment.TickCount & 0xFFFF}");
                txn?.Commit();
            }
            catch { txn?.Cancel(); }
        }

        private void OnClearDocuments(object? sender, EventArgs e)
        {
            if (_manager == null) return;
            var r = MessageBox.Show(
                "Remove all design-time documents and close all open documents?",
                "Clear All Documents",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);
            if (r != DialogResult.Yes) return;
            using var txn = DesignerHost?.CreateTransaction("Clear All Documents");
            try
            {
                _manager.DesignTimeDocuments.Clear();
                _manager.CloseAllDocuments();
                txn?.Commit();
            }
            catch { txn?.Cancel(); }
        }

        private void OnResetLayout(object? sender, EventArgs e)
        {
            if (_manager?.View == null) return;
            using var txn = DesignerHost?.CreateTransaction("Reset Layout");
            try
            {
                _manager.CloseAllDocuments();
                txn?.Commit();
            }
            catch { txn?.Cancel(); }
        }

        private void OnUseTabbedView(object? sender, EventArgs e)
            => SetViewType<BeepTabbedView>("Use Tabbed View");

        private void OnUseNativeMdiView(object? sender, EventArgs e)
            => SetViewType<BeepNativeMdiView>("Use Native MDI View");

        private void OnCustomizeShortcuts(object? sender, EventArgs e)
        {
            var registry = _manager?.CommandRegistry;
            if (registry == null)
            {
                MessageBox.Show(
                    "Keyboard shortcuts are only available when the view is BeepTabbedView.",
                    "Customize Keyboard Shortcuts",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            using var dlg = new DocumentHostShortcutEditorDialog(registry);
            dlg.ShowDialog();
        }

        private void OnManageWorkspaces(object? sender, EventArgs e)
        {
            var host = (_manager?.View as BeepTabbedView)?.Host;
            if (host == null)
            {
                MessageBox.Show(
                    "Manage Workspaces requires a BeepTabbedView with a Host assigned.",
                    "Manage Workspaces",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            using var dlg = new WorkspaceManagerDialog(host);
            dlg.ShowDialog();
        }

        private void SetViewType<TView>(string txnName)
            where TView : Component, IBeepDocumentManagerView, new()
        {
            if (_manager == null || DesignerHost == null) return;
            using var txn = DesignerHost.CreateTransaction(txnName);
            try
            {
                var view = (TView)DesignerHost.CreateComponent(typeof(TView));
                GetPropertyDescriptor(nameof(BeepDocumentManager.View))?.SetValue(_manager, view);
                txn.Commit();
            }
            catch { txn.Cancel(); }
        }

        // ── Document collection editor ────────────────────────────────────────

        internal void OpenDesignTimeDocumentsEditor()
        {
            if (_manager == null) return;

            var prop = TypeDescriptor.GetProperties(_manager)[nameof(BeepDocumentManager.DesignTimeDocuments)];
            if (prop == null) return;

            var serviceProvider = GetService(typeof(IDesignerHost)) as IServiceProvider
                                  ?? (IServiceProvider)GetService(typeof(IServiceProvider))!;

            var ctx    = new DesignTimeDocumentsEditorContext(_manager, prop, serviceProvider);
            var editor = new DesignTimeDocumentsEditor(
                typeof(Collection<DocumentDescriptor>));

            var current = prop.GetValue(_manager);
            editor.EditValue(ctx, ctx, current);

            // Re-apply seeded documents to the view (editor modifies the collection in-place)
            try
            {
                _manager.CloseAllDocuments();
                ApplyDesignTimeDocumentsToView();
            }
            catch { /* non-fatal */ }

            if (GetService(typeof(DesignerActionUIService)) is DesignerActionUIService svc)
                svc.Refresh(_manager);
        }

        private void ApplyDesignTimeDocumentsToView()
        {
            if (_manager?.View == null) return;
            var docs = _manager.DesignTimeDocuments;
            if (docs.Count == 0) return;
            _manager.View.BeginBatchAddDocuments();
            try
            {
                foreach (var desc in docs)
                    _manager.View.AddDocument(desc);
            }
            finally
            {
                _manager.View.EndBatchAddDocuments();
            }
        }

        // ── Smart-tag action list ─────────────────────────────────────────────

        public override DesignerActionListCollection ActionLists
        {
            get
            {
                _actionLists ??= new DesignerActionListCollection
                {
                    new BeepDocumentManagerActionList(this)
                };
                return _actionLists;
            }
        }

        // ── Helpers ───────────────────────────────────────────────────────────

        internal IDesignerHost? DesignerHost =>
            GetService(typeof(IDesignerHost)) as IDesignerHost;

        internal static PropertyDescriptor? GetPropertyDescriptor(string name) =>
            TypeDescriptor.GetProperties(typeof(BeepDocumentManager))[name];

        internal static PropertyDescriptor? GetHostPropertyDescriptor(
            BeepDocumentHost host, string name) =>
            TypeDescriptor.GetProperties(host)[name];
    }

    // ═════════════════════════════════════════════════════════════════════════
    // Smart-tag action list
    // ═════════════════════════════════════════════════════════════════════════

    internal sealed class BeepDocumentManagerActionList : DesignerActionList
    {
        private readonly BeepDocumentManagerDesigner _designer;

        public BeepDocumentManagerActionList(BeepDocumentManagerDesigner designer)
            : base(designer.Component)
        {
            _designer = designer;
            AutoShow  = true;
        }

        private BeepDocumentManager? Manager  => _designer.Component as BeepDocumentManager;

        // Proxy into the tabbed-view host for tab appearance props.
        private BeepDocumentHost? TabbedHost  => (Manager?.View as BeepTabbedView)?.Host;

        // ── View ──────────────────────────────────────────────────────────────

        [Description("The view component (BeepTabbedView, BeepNativeMdiView, ...) that renders documents.")]
        public IBeepDocumentManagerView? View
        {
            get => Manager?.View;
            set
            {
                var mgr = Manager;
                if (mgr == null) return;
                BeepDocumentManagerDesigner.GetPropertyDescriptor(
                    nameof(BeepDocumentManager.View))?.SetValue(mgr, value);
                RefreshPanel();
            }
        }

        // ── Tab appearance (only meaningful when View is BeepTabbedView) ──────

        [Description("Tab style used by BeepTabbedView (Chrome / Pills / Underline / Cards / Material).")]
        public DocumentTabStyle TabStyle
        {
            get => TabbedHost?.TabStyle ?? DocumentTabStyle.Chrome;
            set
            {
                var host = TabbedHost;
                if (host == null) return;
                BeepDocumentManagerDesigner.GetHostPropertyDescriptor(
                    host, nameof(BeepDocumentHost.TabStyle))?.SetValue(host, value);
                RefreshPanel();
            }
        }

        [Description("Position of the tab strip relative to the document area.")]
        public TabStripPosition TabPosition
        {
            get => TabbedHost?.TabPosition ?? TabStripPosition.Top;
            set
            {
                var host = TabbedHost;
                if (host == null) return;
                BeepDocumentManagerDesigner.GetHostPropertyDescriptor(
                    host, nameof(BeepDocumentHost.TabPosition))?.SetValue(host, value);
                RefreshPanel();
            }
        }

        [Description("When the close button is shown on individual tab headers.")]
        public TabCloseMode CloseMode
        {
            get => TabbedHost?.CloseMode ?? TabCloseMode.OnHover;
            set
            {
                var host = TabbedHost;
                if (host == null) return;
                BeepDocumentManagerDesigner.GetHostPropertyDescriptor(
                    host, nameof(BeepDocumentHost.CloseMode))?.SetValue(host, value);
                RefreshPanel();
            }
        }

        // ── Document policy flags (from DefaultPolicy) ────────────────────────

        [Description("Allow documents to be dragged out as floating windows.")]
        public bool AllowFloat
        {
            get => Manager?.DefaultPolicy.AllowFloat ?? true;
            set
            {
                var mgr = Manager;
                if (mgr == null) return;
                mgr.DefaultPolicy.AllowFloat = value;
                mgr.View?.PushPolicy(mgr.DefaultPolicy);
                RefreshPanel();
            }
        }

        [Description("Allow documents to be pinned to the auto-hide strip.")]
        public bool AllowPin
        {
            get => Manager?.DefaultPolicy.AllowPin ?? true;
            set
            {
                var mgr = Manager;
                if (mgr == null) return;
                mgr.DefaultPolicy.AllowPin = value;
                mgr.View?.PushPolicy(mgr.DefaultPolicy);
                RefreshPanel();
            }
        }

        [Description("Allow the document area to be split into side-by-side groups.")]
        public bool AllowSplit
        {
            get => Manager?.DefaultPolicy.AllowSplit ?? true;
            set
            {
                var mgr = Manager;
                if (mgr == null) return;
                mgr.DefaultPolicy.AllowSplit = value;
                mgr.View?.PushPolicy(mgr.DefaultPolicy);
                RefreshPanel();
            }
        }

        // ── Persistence ───────────────────────────────────────────────────────

        [Description("Automatically save and restore the tab layout.")]
        public bool AutoSaveLayout
        {
            get => Manager?.AutoSaveLayout ?? false;
            set
            {
                var mgr = Manager;
                if (mgr == null) return;
                BeepDocumentManagerDesigner.GetPropertyDescriptor(
                    nameof(BeepDocumentManager.AutoSaveLayout))?.SetValue(mgr, value);
                RefreshPanel();
            }
        }

        [Description("File path for layout persistence.")]
        public string SessionFile
        {
            get => Manager?.SessionFile ?? string.Empty;
            set
            {
                var mgr = Manager;
                if (mgr == null) return;
                BeepDocumentManagerDesigner.GetPropertyDescriptor(
                    nameof(BeepDocumentManager.SessionFile))?.SetValue(mgr, value);
                RefreshPanel();
            }
        }

        [Description("Text of the auto-maintained Window menu item.")]
        public string WindowMenuText
        {
            get => Manager?.WindowMenuText ?? "&Window";
            set
            {
                var mgr = Manager;
                if (mgr == null) return;
                BeepDocumentManagerDesigner.GetPropertyDescriptor(
                    nameof(BeepDocumentManager.WindowMenuText))?.SetValue(mgr, value);
                RefreshPanel();
            }
        }

        // ── Action methods (wired below via DesignerActionMethodItem) ─────────

        public void EditDocuments()        => _designer.OpenDesignTimeDocumentsEditor();
        public void AddDocument()          => InvokeVerb("Add Document");
        public void ClearDocuments()       => InvokeVerb("Clear All Documents");
        public void CustomizeShortcuts()   => InvokeVerb("Customize Keyboard Shortcuts\u2026");
        public void ManageWorkspaces()      => InvokeVerb("Manage Workspaces\u2026");
        public void ResetLayout()          => InvokeVerb("Reset Layout");
        public void UseTabbedView()        => InvokeVerb("Use Tabbed View");
        public void UseNativeMdiView()     => InvokeVerb("Use Native MDI View");
        public void SaveLayoutNow()        => Manager?.SaveLayout();
        public void LoadLayoutNow()        => Manager?.LoadLayout();

        private void InvokeVerb(string name)
        {
            foreach (DesignerVerb v in _designer.Verbs)
                if (v.Text == name) { v.Invoke(); break; }
        }

        // ── Build sorted items ────────────────────────────────────────────────

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            var items = new DesignerActionItemCollection();

            // ── View
            items.Add(new DesignerActionHeaderItem("View"));
            items.Add(new DesignerActionPropertyItem(
                nameof(View), "View",
                "View",
                "The IBeepDocumentManagerView that renders documents (Tabbed / NativeMdi / ...)."));
            items.Add(new DesignerActionMethodItem(
                this, nameof(UseTabbedView), "Use Tabbed View",
                "View",
                "Create a BeepTabbedView and assign it to View.",
                includeAsDesignerVerb: true));
            items.Add(new DesignerActionMethodItem(
                this, nameof(UseNativeMdiView), "Use Native MDI View",
                "View",
                "Create a BeepNativeMdiView and assign it to View.",
                includeAsDesignerVerb: true));

            // ── Documents
            items.Add(new DesignerActionHeaderItem("Documents"));
            items.Add(new DesignerActionMethodItem(
                this, nameof(EditDocuments), "Edit Documents\u2026",
                "Documents",
                "Open the design-time document collection editor.",
                includeAsDesignerVerb: true));
            items.Add(new DesignerActionMethodItem(
                this, nameof(AddDocument), "Add Document",
                "Documents",
                "Add a new document to the active view.",
                includeAsDesignerVerb: true));
            items.Add(new DesignerActionMethodItem(
                this, nameof(ClearDocuments), "Clear All Documents",
                "Documents",
                "Remove all design-time documents and close any open documents.",
                includeAsDesignerVerb: true));

            // ── Commands
            items.Add(new DesignerActionHeaderItem("Commands"));
            items.Add(new DesignerActionMethodItem(
                this, nameof(CustomizeShortcuts), "Customize Keyboard Shortcuts\u2026",
                "Commands",
                "Open the keyboard-shortcut editor for the active BeepDocumentHost.",
                includeAsDesignerVerb: true));
            items.Add(new DesignerActionMethodItem(
                this, nameof(ManageWorkspaces), "Manage Workspaces\u2026",
                "Commands",
                "List, rename and delete named workspace snapshots for this document host.",
                includeAsDesignerVerb: true));

            // ── Tab appearance (only visible when view is BeepTabbedView)
            if (TabbedHost != null)
            {
                items.Add(new DesignerActionHeaderItem("Tab appearance"));
                items.Add(new DesignerActionPropertyItem(
                    nameof(TabStyle), "Tab Style",
                    "Tab appearance",
                    "Visual style for the tab strip (Chrome / Pills / Underline / Cards / Material)."));
                items.Add(new DesignerActionPropertyItem(
                    nameof(TabPosition), "Tab Position",
                    "Tab appearance",
                    "Position of the tab strip relative to the document area."));
                items.Add(new DesignerActionPropertyItem(
                    nameof(CloseMode), "Close Button Mode",
                    "Tab appearance",
                    "Controls when the per-tab close button is visible."));
            }

            // ── Policy
            items.Add(new DesignerActionHeaderItem("Policy"));
            items.Add(new DesignerActionPropertyItem(
                nameof(AllowFloat), "Allow Float",
                "Policy",
                "Allow documents to be dragged out as floating windows."));
            items.Add(new DesignerActionPropertyItem(
                nameof(AllowPin), "Allow Pin",
                "Policy",
                "Allow documents to be pinned to the auto-hide strip."));
            items.Add(new DesignerActionPropertyItem(
                nameof(AllowSplit), "Allow Split",
                "Policy",
                "Allow the document area to be split into side-by-side groups."));

            // ── Layout
            items.Add(new DesignerActionHeaderItem("Layout"));
            items.Add(new DesignerActionPropertyItem(
                nameof(AutoSaveLayout), "Auto-Save Layout",
                "Layout",
                "Automatically save and restore the layout on close/open."));
            items.Add(new DesignerActionPropertyItem(
                nameof(SessionFile), "Session File",
                "Layout",
                "File path for automatic layout persistence. Supports %AppData% and %LocalAppData%."));
            items.Add(new DesignerActionMethodItem(
                this, nameof(SaveLayoutNow), "Save Layout Now",
                "Layout",
                "Save current layout to SessionFile."));
            items.Add(new DesignerActionMethodItem(
                this, nameof(LoadLayoutNow), "Load Layout Now",
                "Layout",
                "Restore layout from SessionFile."));
            items.Add(new DesignerActionMethodItem(
                this, nameof(ResetLayout), "Reset Layout",
                "Layout",
                "Close all documents and clear layout customisations.",
                includeAsDesignerVerb: true));

            // ── Window Menu
            items.Add(new DesignerActionHeaderItem("Window menu"));
            items.Add(new DesignerActionPropertyItem(
                nameof(WindowMenuText), "Window Menu Text",
                "Window menu",
                "Text of the auto-maintained Window menu item (WindowMenuOwner must be set)."));

            return items;
        }

        // ── Helper ────────────────────────────────────────────────────────────

        private void RefreshPanel()
        {
            if (GetService(typeof(DesignerActionUIService)) is DesignerActionUIService svc)
                svc.Refresh(Component!);
        }
    }
}
