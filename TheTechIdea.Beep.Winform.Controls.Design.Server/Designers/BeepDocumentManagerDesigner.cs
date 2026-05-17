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
using System.Threading;
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
    public sealed partial class BeepDocumentManagerDesigner : ComponentDesigner
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

        /// <summary>
        /// Phase 07 — runs ONCE when the user drops a BeepDocumentManager from
        /// the toolbox (not on every form-load). Launches the setup wizard so
        /// the new component is fully wired before the user has to hunt for a
        /// smart-tag. Designed to mirror commercial MDI platforms where
        /// dropping a single component opens a guided setup.
        /// </summary>
        public override void InitializeNewComponent(System.Collections.IDictionary? defaultValues)
        {
            base.InitializeNewComponent(defaultValues);
            try
            {
                // Honor the developer's "Don't show again" preference (HKCU).
                if (!ShouldAutoOpenWizard()) return;

                // Defer to the next message loop tick so the component is fully
                // sited by the designer host before the wizard tries to read
                // services from it.
                if (Component is BeepDocumentManager mgr && mgr.Site != null)
                {
                    var sync = SynchronizationContext.Current;
                    if (sync != null)
                    {
                        sync.Post(_ => SafeShowSetupWizard(), null);
                    }
                    else
                    {
                        SafeShowSetupWizard();
                    }
                }
            }
            catch
            {
                // Wizard must never break the toolbox drop operation.
            }
        }

        private void SafeShowSetupWizard()
        {
            try { ShowSetupWizard(); } catch { /* design-time guard */ }
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
                        // Phase 07 — the wizard is the primary entry point.
                        new DesignerVerb("\u2728 Setup Wizard\u2026",         OnChooseViewMode),
                        new DesignerVerb("Edit Documents\u2026",                OnEditDocuments),
                        new DesignerVerb("Add Document",                       OnAddDocument),
                        new DesignerVerb("Clear All Documents",                OnClearDocuments),
                        new DesignerVerb("Reset Layout",                       OnResetLayout),
                        new DesignerVerb("Use Tabbed Documents",                OnUseTabbedView),
                        new DesignerVerb("Use Browser Tabs",                    OnUseBrowserTabs),
                        new DesignerVerb("Use Native MDI",                      OnUseNativeMdiView),
                        new DesignerVerb("Customize Keyboard Shortcuts\u2026", OnCustomizeShortcuts),
                        new DesignerVerb("Manage Workspaces\u2026",            OnManageWorkspaces),
                        new DesignerVerb("Reset Setup Wizard Preference",      OnResetWizardPreference),
                    };
                }
                return _verbs;
            }
        }

        private void OnEditDocuments(object? sender, EventArgs e)
        {
            if (_manager == null) return;
            HideSmartTag();
            OpenDesignTimeDocumentsEditor();
        }

        private void OnAddDocument(object? sender, EventArgs e)
        {
            if (_manager?.View == null) return;

            var title      = $"Document {Environment.TickCount & 0xFFFF}";
            var descriptor = new DocumentDescriptor
            {
                Id             = Guid.NewGuid().ToString("N"),
                Title          = title,
                InitialContent = DocumentInitialContent.Empty
            };

            MutateDesignTimeDocuments("Add Document", docs =>
            {
                docs.Add(descriptor);
                _manager.View?.AddDocument(descriptor);
            });
        }

        private void OnClearDocuments(object? sender, EventArgs e)
        {
            if (_manager == null) return;
            HideSmartTag();
            var r = MessageBox.Show(
                "Remove all design-time documents and close all open documents?",
                "Clear All Documents",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);
            if (r != DialogResult.Yes) return;

            MutateDesignTimeDocuments("Clear All Documents", docs =>
            {
                docs.Clear();
                _manager.CloseAllDocuments();
            });
        }

        private void OnResetLayout(object? sender, EventArgs e)
        {
            // ResetLayout only clears the runtime view; DesignTimeDocuments is
            // untouched, so this is a runtime-only operation that does not
            // participate in the serializable undo graph. We still create a
            // transaction so VS can group any cascading change-service
            // notifications fired by view teardown into a single Edit-menu
            // entry.
            if (_manager?.View == null) return;
            using var txn = DesignerHost?.CreateTransaction("Reset Layout");
            try
            {
                _manager.CloseAllDocuments();
                txn?.Commit();
            }
            catch
            {
                txn?.Cancel();
                throw;
            }
        }

        /// <summary>
        /// Wraps a mutation of <see cref="BeepDocumentManager.DesignTimeDocuments"/>
        /// inside a designer transaction with full
        /// <see cref="IComponentChangeService"/> notifications so the change
        /// is visible to the undo stack and downstream serializers.
        /// </summary>
        internal void MutateDesignTimeDocuments(string transactionName, Action<Collection<DocumentDescriptor>> mutate)
        {
            if (_manager == null) return;

            var prop      = GetPropertyDescriptor(nameof(BeepDocumentManager.DesignTimeDocuments));
            var changeSvc = GetService(typeof(IComponentChangeService)) as IComponentChangeService;

            DesignerTransaction? txn = null;
            try
            {
                txn = DesignerHost?.CreateTransaction(transactionName);
                changeSvc?.OnComponentChanging(_manager, prop);

                mutate(_manager.DesignTimeDocuments);

                changeSvc?.OnComponentChanged(_manager, prop, null, null);
                txn?.Commit();
            }
            catch
            {
                txn?.Cancel();
                throw;
            }
            finally
            {
                RefreshPanel();
            }
        }

        private void OnUseTabbedView(object? sender, EventArgs e)
            => ApplyTabbedViewMode(showInfo: true);

        private void OnUseNativeMdiView(object? sender, EventArgs e)
            => ApplyNativeMdiViewMode(showInfo: true);

        private void OnUseBrowserTabs(object? sender, EventArgs e)
            => ApplyBrowserTabsMode(showInfo: true);

        private void OnChooseViewMode(object? sender, EventArgs e)
            => ShowSetupWizard();

        private void OnCustomizeShortcuts(object? sender, EventArgs e)
        {
            HideSmartTag();
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
            HideSmartTag();
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

        // Kept internal so the new view-mode partial (and BeepDocumentManagerActionList)
        // can request a view type without re-implementing the transaction wrapper.
        //
        // Hot-fix 2026-05-17: made idempotent. The previous implementation
        // unconditionally called DesignerHost.CreateComponent(TView), which
        // meant every wizard re-run leaked a fresh component into the tray
        // (beepTabbedView1, beepTabbedView2, beepTabbedView3, …) instead of
        // reusing the assigned view. We now:
        //   1. Return the existing view as-is when it is already the
        //      requested TView type (no transaction, no leaked components).
        //   2. When switching view types, destroy the previous view via
        //      DesignerHost.DestroyComponent so the tray stays clean.
        //   3. Only create+assign a fresh component when no view of the
        //      requested type exists.
        internal TView? CreateAndAssignView<TView>(string txnName)
            where TView : Component, IBeepDocumentManagerView, new()
        {
            if (_manager == null || DesignerHost == null) return null;

            // Already correct type → reuse, don't recreate.
            if (_manager.View is TView existing)
                return existing;

            using var txn = DesignerHost.CreateTransaction(txnName);
            try
            {
                // Tear down any previous view of a different type so the
                // component tray doesn't accumulate orphans.
                if (_manager.View is Component oldView)
                {
                    try { DesignerHost.DestroyComponent(oldView); }
                    catch { /* design-time; non-fatal */ }
                }

                var view = (TView)DesignerHost.CreateComponent(typeof(TView));
                GetPropertyDescriptor(nameof(BeepDocumentManager.View))?.SetValue(_manager, view);
                txn.Commit();
                return view;
            }
            catch
            {
                txn.Cancel();
                return null;
            }
        }

        /// <summary>
        /// Actively dismisses the BeepDocumentManager smart-tag action panel.
        /// Hot-fix 2026-05-17: <see cref="RefreshPanel"/> only re-renders
        /// content; it does not close the panel. After modal flows
        /// (wizard, MessageBox, collection editor) we want the panel gone
        /// so the user sees their just-applied configuration on the
        /// surface, not an obscuring popup.
        /// </summary>
        internal void HideSmartTag()
        {
            try
            {
                if (Component != null
                    && GetService(typeof(DesignerActionUIService)) is DesignerActionUIService svc)
                {
                    svc.HideUI(Component);
                }
            }
            catch { /* design-time; non-fatal */ }
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
        //
        // DefaultPolicy is a reference-type sealed class (BeepDocumentPolicy).
        // Mutating it in place would be invisible to IComponentChangeService
        // (the DefaultPolicy property reference itself never changes), so
        // Ctrl+Z would not reverse the flag flip. Phase 04: clone the policy,
        // toggle the requested flag on the clone, then publish via the
        // DefaultPolicy PropertyDescriptor.SetValue which raises Component-
        // Changing/Changed and registers a proper undo unit.

        [Description("Allow documents to be dragged out as floating windows.")]
        public bool AllowFloat
        {
            get => Manager?.DefaultPolicy.AllowFloat ?? true;
            set => SetPolicyFlag(nameof(AllowFloat), value, (p, v) => p.AllowFloat = v);
        }

        [Description("Allow documents to be pinned to the auto-hide strip.")]
        public bool AllowPin
        {
            get => Manager?.DefaultPolicy.AllowPin ?? true;
            set => SetPolicyFlag(nameof(AllowPin), value, (p, v) => p.AllowPin = v);
        }

        [Description("Allow the document area to be split into side-by-side groups.")]
        public bool AllowSplit
        {
            get => Manager?.DefaultPolicy.AllowSplit ?? true;
            set => SetPolicyFlag(nameof(AllowSplit), value, (p, v) => p.AllowSplit = v);
        }

        /// <summary>
        /// Clones <see cref="BeepDocumentManager.DefaultPolicy"/>, applies the
        /// supplied flag mutation on the clone, then writes the clone back
        /// through the property descriptor so the change participates in the
        /// designer's undo/redo stack.
        /// </summary>
        private void SetPolicyFlag(string flagName, bool value, Action<BeepDocumentPolicy, bool> applyFlag)
        {
            var mgr = Manager;
            if (mgr == null) return;

            var current = mgr.DefaultPolicy ?? new BeepDocumentPolicy();
            var clone = new BeepDocumentPolicy
            {
                AllowFloat    = current.AllowFloat,
                AllowPin      = current.AllowPin,
                AllowSplit    = current.AllowSplit,
                MaxSplitDepth = current.MaxSplitDepth
            };
            applyFlag(clone, value);

            BeepDocumentManagerDesigner.GetPropertyDescriptor(
                nameof(BeepDocumentManager.DefaultPolicy))?.SetValue(mgr, clone);

            // The setter on BeepDocumentManager.DefaultPolicy already pushes the
            // new policy to the active view, so no explicit PushPolicy call is
            // required here (and skipping it avoids a double-push on undo).
            RefreshPanel();
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
        public void SetupWizard()          => _designer.ShowSetupWizard();
        public void UseTabbedDocuments()   => _designer.ApplyTabbedViewMode(showInfo: true);
        public void UseBrowserTabs()       => _designer.ApplyBrowserTabsMode(showInfo: true);
        public void UseNativeMdi()         => _designer.ApplyNativeMdiViewMode(showInfo: true);
        public void SaveLayoutNow()        => Manager?.SaveLayout();
        public void LoadLayoutNow()        => Manager?.LoadLayout();

        // Smart-tag status banner (plain English, first item in the panel).
        [Description("Current configuration of this BeepDocumentManager.")]
        public string Status => _designer.DescribeViewMode();

        private void InvokeVerb(string name)
        {
            foreach (DesignerVerb v in _designer.Verbs)
                if (v.Text == name) { v.Invoke(); break; }
        }

        // ── Build sorted items ────────────────────────────────────────────────

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            var items = new DesignerActionItemCollection();

            // ── Status banner (Phase 07): always the first item the user sees
            items.Add(new DesignerActionHeaderItem("Status"));
            items.Add(new DesignerActionTextItem(
                _designer.DescribeViewMode(),
                "Status"));

            // ── View setup
            items.Add(new DesignerActionHeaderItem("View"));
            items.Add(new DesignerActionMethodItem(
                this, nameof(SetupWizard), "\u2728 Setup Wizard\u2026",
                "View",
                "Open the visual setup wizard: pick a mode, optionally seed sample tabs, auto-wire everything.",
                includeAsDesignerVerb: true));
            items.Add(new DesignerActionPropertyItem(
                nameof(View), "View",
                "View",
                "The IBeepDocumentManagerView that renders documents (Tabbed / NativeMdi / ...)."));
            items.Add(new DesignerActionMethodItem(
                this, nameof(UseTabbedDocuments), "Use Tabbed Documents",
                "View",
                "IDE-style tabs with splits, docking, floating windows.",
                includeAsDesignerVerb: true));
            items.Add(new DesignerActionMethodItem(
                this, nameof(UseBrowserTabs), "Use Browser Tabs",
                "View",
                "Chrome-style tabs with new-tab button and always-visible close buttons.",
                includeAsDesignerVerb: true));
            items.Add(new DesignerActionMethodItem(
                this, nameof(UseNativeMdi), "Use Native MDI",
                "View",
                "Classic WinForms MDI children. Sets the root form's IsMdiContainer = true and wires ParentForm.",
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
