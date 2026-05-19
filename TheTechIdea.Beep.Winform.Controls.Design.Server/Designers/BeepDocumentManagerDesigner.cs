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
    /// document panel collection, resetting layout, and switching the active
    /// <see cref="IBeepDocumentManagerView"/>.
    /// </summary>
    public sealed partial class BeepDocumentManagerDesigner : ComponentDesigner
    {
        private BeepDocumentManager?          _manager;
        private DesignerVerbCollection?       _verbs;
        private DesignerActionListCollection? _actionLists;
        private bool                          _suppressImmediateHostSync;
        private bool                          _managerHostSyncScheduled;

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
                // Auto-create BeepDocumentHost via IDesignerHost so it serializes to designer.cs
                if (Component is BeepDocumentManager mgr && mgr.Site != null)
                {
                    var sync = SynchronizationContext.Current;
                    if (sync != null)
                        sync.Post(_ => SafeAutoCreateHost(), null);
                    else
                        SafeAutoCreateHost();
                }
            }
            catch
            {
                // Auto-creation must never break the toolbox drop operation.
            }
        }

        private void SafeAutoCreateHost()
        {
            try { AutoCreateHost(); } catch { /* design-time guard */ }
        }

        /// <summary>
        /// Auto-creates a BeepDocumentHost on the root form and assigns it to the manager.
        /// Uses IDesignerHost.CreateComponent so the host is serialized to designer.cs.
        /// </summary>
        private void AutoCreateHost()
        {
            if (_manager == null || DesignerHost == null) return;
            if (_manager.Host != null) return; // Already has a host

            // Only auto-create when the root component is a Form
            if (DesignerHost.RootComponent is not Form rootForm) return;

            using var txn = DesignerHost.CreateTransaction("Create BeepDocumentHost");
            try
            {
                // CRITICAL: Use designer host to create component so it tracks and serializes it
                var host = (BeepDocumentHost)DesignerHost.CreateComponent(typeof(BeepDocumentHost));
                
                var changeSvc = GetService(typeof(IComponentChangeService)) as IComponentChangeService;
                var controlsProp = TypeDescriptor.GetProperties(rootForm)["Controls"];
                
                // Add host to form's controls
                changeSvc?.OnComponentChanging(rootForm, controlsProp);
                rootForm.Controls.Add(host);
                changeSvc?.OnComponentChanged(rootForm, controlsProp, null, null);
                
                // Dock = Fill via property descriptor so change service notifies serializers
                var dockProp = TypeDescriptor.GetProperties(host)[nameof(Control.Dock)];
                if (dockProp != null)
                {
                    var oldDock = dockProp.GetValue(host);
                    changeSvc?.OnComponentChanging(host, dockProp);
                    dockProp.SetValue(host, DockStyle.Fill);
                    changeSvc?.OnComponentChanged(host, dockProp, oldDock, DockStyle.Fill);
                }
                
                // Send to back so it doesn't obscure docked siblings
                try { host.SendToBack(); } catch { }
                
                // Assign to manager via property descriptor so it serializes
                var hostProp = TypeDescriptor.GetProperties(_manager)[nameof(BeepDocumentManager.Host)];
                if (hostProp != null)
                {
                    var oldHost = hostProp.GetValue(_manager);
                    changeSvc?.OnComponentChanging(_manager, hostProp);
                    hostProp.SetValue(_manager, host);
                    changeSvc?.OnComponentChanged(_manager, hostProp, oldHost, host);
                }
                
                txn.Commit();
            }
            catch
            {
                txn.Cancel();
                throw;
            }
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
            if (_manager?.Host == null) return;
            HideSmartTag();

            if (TryAddDocumentViaHostDesigner())
            {
                return;
            }

            MutateDesignTimeDocuments("Add Document", docs =>
            {
                docs.Add(DesignTimeDocumentCoordinator.CreateDetachedDescriptor(
                    Guid.NewGuid().ToString("N"),
                    $"Document {Environment.TickCount & 0xFFFF}"));
            });
        }

        private void OnClearDocuments(object? sender, EventArgs e)
        {
            if (_manager == null) return;
            HideSmartTag();
            var r = MessageBox.Show(
                "Remove all document panels and close all open documents?",
                "Clear All Documents",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);
            if (r != DialogResult.Yes) return;

            MutateDesignTimeDocuments("Clear All Documents", docs =>
            {
                docs.Clear();
            });
        }

        private void OnResetLayout(object? sender, EventArgs e)
        {
            HideSmartTag();
            // ResetLayout only clears the runtime host; DesignTimeDocuments is
            // untouched, so this is a runtime-only operation that does not
            // participate in the serializable undo graph. We still create a
            // transaction so VS can group any cascading change-service
            // notifications fired by host teardown into a single Edit-menu
            // entry.
            if (_manager?.Host == null) return;
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

                _manager.BeginDesignTimeDocumentUpdate();
                mutate(_manager.DesignTimeDocuments);
                // Pass applyChanges:false — the host designer will create proper
                // designer component panels below via SyncFromManagerDocuments.
                // The runtime AddDocument path would create unsited panels that
                // do not appear in Designer.cs and cause re-open naming conflicts.
                _manager.EndDesignTimeDocumentUpdate(applyChanges: false);

                changeSvc?.OnComponentChanged(_manager, prop, null, null);
                txn?.Commit();
            }
            catch
            {
                try { _manager.EndDesignTimeDocumentUpdate(applyChanges: false); }
                catch { /* design-time; non-fatal */ }
                txn?.Cancel();
                throw;
            }
            finally
            {
                RefreshPanel();
            }

            // Delegate panel creation to the host designer so panels are created via
            // designerHost.CreateComponent() and appear in Designer.cs as real components
            // (and in DocumentPanels so the WinForms serializer emits Add() calls).
            if (!_suppressImmediateHostSync)
            {
                SyncManagerDocumentsViaHostDesigner();
            }
        }

        /// <summary>
        /// Finds the <see cref="BeepDocumentHostDesigner"/> for the manager's host and
        /// asks it to sync the manager's document list to the host's
        /// <see cref="BeepDocumentHost.DocumentPanels"/> using proper designer
        /// component creation. Falls back to the runtime path when no host designer
        /// is available.
        /// </summary>
        private void SyncManagerDocumentsViaHostDesigner()
            => SyncManagerDocumentsViaHostDesigner(createTransaction: true);

        private void SyncManagerDocumentsViaHostDesigner(bool createTransaction)
        {
            if (_manager == null) return;

            var host = _manager.Host;
            if (host == null)
            {
                // No host — runtime refresh is the only option
                _manager.RefreshDesignTimeDocuments();
                return;
            }

            if (DesignerHost?.GetDesigner(host) is BeepDocumentHostDesigner hostDesigner)
            {
                if (createTransaction)
                {
                    hostDesigner.SyncFromManagerDocuments(host, _manager.DesignTimeDocuments);
                }
                else
                {
                    hostDesigner.SyncFromManagerDocumentsInPlace(host, _manager.DesignTimeDocuments);
                }
            }
            else
            {
                if (!_managerHostSyncScheduled)
                {
                    ScheduleManagerDocumentsHostSync();
                }
            }
        }

        private bool TryAddDocumentViaHostDesigner(string? title = null, bool activate = true, bool selectSurface = true)
        {
            if (_manager == null)
            {
                return false;
            }

            var host = _manager.Host;
            if (host == null || DesignerHost?.GetDesigner(host) is not BeepDocumentHostDesigner hostDesigner)
            {
                return false;
            }

            DocumentDescriptor? created = null;
            hostDesigner.ExecuteSharedDocumentAction("Add Document", (h, docs) =>
            {
                created = new DesignTimeDocumentCoordinator(hostDesigner, h, docs)
                    .AddNewDocument(activate, selectSurface, title);
            });

            if (created == null)
            {
                return false;
            }

            return true;
        }

        private void ScheduleManagerDocumentsHostSync()
        {
            var sync = SynchronizationContext.Current;
            if (sync != null)
            {
                _managerHostSyncScheduled = true;
                sync.Post(_ =>
                {
                    _managerHostSyncScheduled = false;
                    try { SyncManagerDocumentsViaHostDesigner(); }
                    catch { /* design-time; non-fatal */ }
                }, null);
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
                    "Keyboard shortcuts are only available when a host is assigned.",
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
            var host = _manager?.Host;
            if (host == null)
            {
                MessageBox.Show(
                    "Manage Workspaces requires a BeepDocumentHost to be assigned.",
                    "Manage Workspaces",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            using var dlg = new WorkspaceManagerDialog(host);
            dlg.ShowDialog();
        }

        /// <summary>
        /// Creates and assigns a BeepDocumentHost to the manager using IDesignerHost.CreateComponent
        /// so it serializes to the form's designer.cs.
        /// </summary>
        internal BeepDocumentHost? CreateAndAssignHost(string txnName)
        {
            if (_manager == null || DesignerHost == null) return null;

            // Already has a host → reuse, don't recreate.
            if (_manager.Host != null)
                return _manager.Host;

            using var txn = DesignerHost.CreateTransaction(txnName);
            try
            {
                var host = (BeepDocumentHost)DesignerHost.CreateComponent(typeof(BeepDocumentHost));
                PropertyDescriptor? hostProperty = GetPropertyDescriptor(nameof(BeepDocumentManager.Host));
                if (hostProperty != null)
                {
                    var changeSvc = GetService(typeof(IComponentChangeService)) as IComponentChangeService;
                    object? oldValue = hostProperty.GetValue(_manager);
                    changeSvc?.OnComponentChanging(_manager, hostProperty);
                    hostProperty.SetValue(_manager, host);
                    changeSvc?.OnComponentChanged(_manager, hostProperty, oldValue, host);
                }
                txn.Commit();
                return host;
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
            HideSmartTag();

            var prop = TypeDescriptor.GetProperties(_manager)[nameof(BeepDocumentManager.DesignTimeDocuments)];
            if (prop == null) return;

            var serviceProvider = GetService(typeof(IDesignerHost)) as IServiceProvider
                                  ?? (IServiceProvider)GetService(typeof(IServiceProvider))!;

            var ctx    = new DesignTimeDocumentsEditorContext(_manager, prop, serviceProvider);
            var editor = new DesignTimeDocumentsEditor(
                typeof(Collection<DocumentDescriptor>));

            var current = prop.GetValue(_manager);
            editor.EditValue(ctx, ctx, current);

            if (GetService(typeof(DesignerActionUIService)) is DesignerActionUIService svc)
                svc.Refresh(_manager);
        }

        private void ApplyDesignTimeDocumentsToView()
        {
            // Prefer the host designer path so newly added/edited documents are
            // created as proper designer components and appear in Designer.cs.
            SyncManagerDocumentsViaHostDesigner();
        }

        internal void SyncCurrentDesignTimeDocuments()
            => ApplyDesignTimeDocumentsToView();

        internal void SyncCurrentDesignTimeDocumentsInPlace()
            => SyncManagerDocumentsViaHostDesigner(createTransaction: false);

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

        internal void SetDesignerProperty(object component, string propertyName, object? value, string? transactionName = null)
        {
            if (component == null) return;

            PropertyDescriptor? prop = TypeDescriptor.GetProperties(component)[propertyName];
            if (prop == null || prop.IsReadOnly) return;

            object? oldValue = prop.GetValue(component);
            if (Equals(oldValue, value)) return;

            IComponentChangeService? changeSvc = GetService(typeof(IComponentChangeService)) as IComponentChangeService;
            DesignerTransaction? txn = null;
            try
            {
                txn = DesignerHost?.CreateTransaction(transactionName ?? $"Set {propertyName}");
                changeSvc?.OnComponentChanging(component, prop);
                prop.SetValue(component, value);
                changeSvc?.OnComponentChanged(component, prop, oldValue, value);
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

        // Direct reference to the host for tab appearance props.
        private BeepDocumentHost? TabbedHost  => Manager?.Host;

        // ── Host ──────────────────────────────────────────────────────────────

        [Description("The BeepDocumentHost control that renders documents.")]
        public BeepDocumentHost? Host
        {
            get => Manager?.Host;
            set
            {
                var mgr = Manager;
                if (mgr == null) return;
                _designer.SetDesignerProperty(
                    mgr,
                    nameof(BeepDocumentManager.Host),
                    value,
                    "Set Document Host");
            }
        }

        // ── Tab appearance (only meaningful when Host is assigned) ──────

        [Description("Tab style used by BeepDocumentHost (Chrome / Pills / Underline / Cards / Material).")]
        public DocumentTabStyle TabStyle
        {
            get => TabbedHost?.TabStyle ?? DocumentTabStyle.Chrome;
            set
            {
                var host = TabbedHost;
                if (host == null) return;
                _designer.SetDesignerProperty(
                    host,
                    nameof(BeepDocumentHost.TabStyle),
                    value,
                    "Set Tab Style");
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
                _designer.SetDesignerProperty(
                    host,
                    nameof(BeepDocumentHost.TabPosition),
                    value,
                    "Set Tab Position");
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
                _designer.SetDesignerProperty(
                    host,
                    nameof(BeepDocumentHost.CloseMode),
                    value,
                    "Set Close Button Mode");
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

            _designer.SetDesignerProperty(
                mgr,
                nameof(BeepDocumentManager.DefaultPolicy),
                clone,
                $"Set {flagName}");
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
                _designer.SetDesignerProperty(
                    mgr,
                    nameof(BeepDocumentManager.AutoSaveLayout),
                    value,
                    "Set Auto-Save Layout");
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
                _designer.SetDesignerProperty(
                    mgr,
                    nameof(BeepDocumentManager.SessionFile),
                    value,
                    "Set Session File");
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
                _designer.SetDesignerProperty(
                    mgr,
                    nameof(BeepDocumentManager.WindowMenuText),
                    value,
                    "Set Window Menu Text");
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
        public void SaveLayoutNow()        { _designer.HideSmartTag(); Manager?.SaveLayout(); }
        public void LoadLayoutNow()        { _designer.HideSmartTag(); Manager?.LoadLayout(); }

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
                "Open the document editor and create real document panels on the design surface.",
                includeAsDesignerVerb: true));
            items.Add(new DesignerActionMethodItem(
                this, nameof(AddDocument), "Add Document",
                "Documents",
                "Add a new document to the active view.",
                includeAsDesignerVerb: true));
            items.Add(new DesignerActionMethodItem(
                this, nameof(ClearDocuments), "Clear All Documents",
                "Documents",
                "Remove all document panels from the active view.",
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
