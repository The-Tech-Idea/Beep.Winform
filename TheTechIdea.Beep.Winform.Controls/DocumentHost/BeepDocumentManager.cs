// BeepDocumentManager.cs
// Non-visual orchestrator component — the DevExpress-style component-tray bridge
// between an IBeepDocumentManagerView (BeepTabbedView / BeepNativeMdiView / future
// BeepWindowsUIView) and the host Form's menu / status / theme.
// ─────────────────────────────────────────────────────────────────────────────────────────
// Usage (designer):
//   1. Drag BeepDocumentManager from Toolbox onto the form's component tray.
//   2. Drag a view component (BeepTabbedView or BeepNativeMdiView) onto the tray.
//   3. Set BeepDocumentManager.View to the view component.
//   4. Optionally set WindowMenuOwner, SessionFile, DefaultPolicy, etc.
//   5. Wire ActiveDocumentChanged / DocumentClosing events in the Properties pane.
//
// Usage (code-behind):
//   var view = manager.ChangeView<BeepTabbedView>();
//   view.Host = beepDocumentHost1;
// ─────────────────────────────────────────────────────────────────────────────────────────
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.DocumentHost.Features;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using static TheTechIdea.Beep.Winform.Controls.DocumentHost.Layout.LayoutMigrationService;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost
{
    // ═══════════════════════════════════════════════════════════════════════════
    // Supporting types
    // ═══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Composite design-time record that groups the float / pin / split policies
    /// into a single expandable property on <see cref="BeepDocumentManager"/>.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public sealed class BeepDocumentPolicy
    {
        [DefaultValue(true)]
        [Description("Allow documents to be dragged out as floating windows.")]
        public bool AllowFloat    { get; set; } = true;

        [DefaultValue(true)]
        [Description("Allow documents to be pinned to the auto-hide strip.")]
        public bool AllowPin      { get; set; } = true;

        [DefaultValue(true)]
        [Description("Allow the document area to be split into side-by-side groups.")]
        public bool AllowSplit    { get; set; } = true;

        [DefaultValue(4)]
        [Description("Maximum nesting depth for split groups.")]
        public int  MaxSplitDepth { get; set; } = 4;

        public override string ToString() =>
            $"Float={AllowFloat}, Pin={AllowPin}, Split={AllowSplit}, Depth={MaxSplitDepth}";
    }

    /// <summary>Event args raised by <see cref="BeepDocumentManager.LayoutChanged"/>.</summary>
    public sealed class ManagerLayoutChangedEventArgs : EventArgs
    {
        /// <summary>JSON snapshot produced by the active view.</summary>
        public string? LayoutJson { get; }
        internal ManagerLayoutChangedEventArgs(string? json) => LayoutJson = json;
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // BeepDocumentManager
    // ═══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Non-visual component that orchestrates document rendering through an
    /// <see cref="IBeepDocumentManagerView"/> from the form's component tray,
    /// mirroring the DevExpress / Telerik / Syncfusion pattern.
    /// </summary>
    /// <remarks>
    /// Drop onto the tray, assign <see cref="View"/>, then configure
    /// <see cref="WindowMenuOwner"/>, <see cref="DefaultPolicy"/>, and
    /// <see cref="AutoSaveLayout"/> without touching the renderer directly.
    /// </remarks>
    [ToolboxItem(true)]
    [DesignerCategory("Component")]
    [Designer(
        "TheTechIdea.Beep.Winform.Controls.Design.Server.Designers.BeepDocumentManagerDesigner, " +
        "TheTechIdea.Beep.Winform.Controls.Design.Server")]
    [ProvideProperty("DocumentTitle",    typeof(Control))]
    [ProvideProperty("DocumentIconPath", typeof(Control))]
    [DefaultEvent(nameof(ActiveDocumentChanged))]
    [DefaultProperty(nameof(View))]
    public sealed partial class BeepDocumentManager : Component, IExtenderProvider, ISupportInitialize
    {
        // ── Private state ─────────────────────────────────────────────────────

        private IBeepDocumentManagerView? _view;
        private string                    _themeName      = string.Empty;
        private bool                      _autoSaveLayout = false;
        private string                    _sessionFile    = string.Empty;
        private BeepCloudSyncSettings?    _cloudSyncSettings;
        private MenuStrip?                _windowMenuOwner;
        private string                    _windowMenuText = "&Window";
        private StatusStrip?              _statusStripOwner;
        private bool                      _autoPopulateStatusStrip = true;
        // Phase 05 G – status strip live integration
        private ToolStripStatusLabel?     _statusDocTitle;
        private ToolStripStatusLabel?     _statusModified;
        private ToolStripStatusLabel?     _statusCursor;
        private ToolStripDropDownButton?  _statusWorkspace;
        private BeepDocumentPanel?        _activeTrackedPanel;
        private IDocumentStatusInfoProvider? _trackedStatusProvider;
        private TextBoxBase?              _trackedTextBox;
        private BeepDocumentHost?         _trackedWorkspaceHost;
        private BeepDocumentPolicy        _defaultPolicy  = new();
        private bool                      _autoWireForm   = true;
        private bool                      _inInit         = false;
        // Phase 06 D1 — form-level shortcut forwarding
        private Form?                     _parentForm;
        private bool                      _hostHandleHooked;

        // Attached-property data (IExtenderProvider) — ConditionalWeakTable so
        // entries are collected when the extended Control is GC'd.
        private readonly ConditionalWeakTable<Control, AttachedDocInfo> _attachedInfo    = new();
        private readonly HashSet<Control>                                _extendedControls = new();

        // Design-time document seed collection.
        private readonly Collection<DocumentDescriptor> _designTimeDocuments = new();

        // User-registered document-type factories (Phase 07).
        private readonly Dictionary<string, Func<DocumentDescriptor, Control>> _templates =
            new(StringComparer.OrdinalIgnoreCase);

        // ── Constructors ──────────────────────────────────────────────────────

        public BeepDocumentManager() { }

        /// <summary>Designer-friendly overload that adds this component to a container.</summary>
        public BeepDocumentManager(IContainer container) { container?.Add(this); }

        // ══════════════════════════════════════════════════════════════════════
        // IExtenderProvider
        // ══════════════════════════════════════════════════════════════════════

        /// <summary>Extends every <see cref="Control"/> on the same form except itself.</summary>
        public bool CanExtend(object extendee) =>
            extendee is Control && !ReferenceEquals(extendee, this);

        // ----- DocumentTitle --------------------------------------------------

        [DisplayName("DocumentTitle")]
        [Description("Title shown in the tab header when this control is opened as a document.")]
        [DefaultValue(null)]
        [Category("Document Manager")]
        public string? GetDocumentTitle(Control c) =>
            _attachedInfo.TryGetValue(c, out var info) ? info.Title : null;

        public void SetDocumentTitle(Control c, string? value)
        {
            if (_attachedInfo.TryGetValue(c, out var info))
                info.Title = value;
            else if (value != null)
            {
                _attachedInfo.Add(c, new AttachedDocInfo { Title = value });
                _extendedControls.Add(c);
            }
        }

        // ----- DocumentIconPath -----------------------------------------------

        [DisplayName("DocumentIconPath")]
        [Description("Image path for the tab icon when this control is opened as a document.")]
        [DefaultValue(null)]
        [Category("Document Manager")]
        public string? GetDocumentIconPath(Control c) =>
            _attachedInfo.TryGetValue(c, out var info) ? info.IconPath : null;

        public void SetDocumentIconPath(Control c, string? value)
        {
            if (_attachedInfo.TryGetValue(c, out var info))
                info.IconPath = value;
            else if (value != null)
            {
                _attachedInfo.Add(c, new AttachedDocInfo { IconPath = value });
                _extendedControls.Add(c);
            }
        }

        // ══════════════════════════════════════════════════════════════════════
        // ISupportInitialize — defers wiring until designer InitializeComponent finishes
        // ══════════════════════════════════════════════════════════════════════

        public void BeginInit() => _inInit = true;

        public void EndInit()
        {
            _inInit = false;
            if (_autoWireForm)
                ApplyViewSettings();
            // G – wire status strip items deferred until after view is ready
            AttachStatusStrip();
        }

        // ══════════════════════════════════════════════════════════════════════
        // Properties
        // ══════════════════════════════════════════════════════════════════════

        /// <summary>
        /// The active <see cref="IBeepDocumentManagerView"/> that renders documents.
        /// Assignable in the designer to any view component on the form
        /// (e.g. <see cref="BeepTabbedView"/> or <see cref="BeepNativeMdiView"/>).
        /// </summary>
        [Category("Document Manager")]
        [Description("The view component (BeepTabbedView, BeepNativeMdiView, ...) that renders documents.")]
        [DefaultValue(null)]
        public IBeepDocumentManagerView? View
        {
            get => _view;
            set
            {
                if (ReferenceEquals(_view, value)) return;
                DetachView();
                _view = value;
                if (!_inInit)
                    ApplyViewSettings();
            }
        }

        /// <summary>Beep theme name — fan-out to <see cref="View"/> when set.</summary>
        [Category("Document Manager")]
        [Description("Beep theme name propagated to the view and all document panels.")]
        [DefaultValue("")]
        public string ThemeName
        {
            get => _themeName;
            set
            {
                _themeName = value ?? string.Empty;
                _view?.PushTheme(_themeName);
            }
        }

        /// <summary>Automatically save and restore the tab layout on application exit / start.</summary>
        [Category("Document Manager")]
        [Description("Automatically save the tab layout on close and restore it on next open.")]
        [DefaultValue(false)]
        public bool AutoSaveLayout
        {
            get => _autoSaveLayout;
            set
            {
                _autoSaveLayout = value;
                _view?.PushPersistence(_autoSaveLayout, ExpandSessionPath(_sessionFile));
            }
        }

        /// <summary>
        /// File path for layout persistence.  Supports <c>%AppData%</c> and
        /// <c>%LocalAppData%</c> tokens as well as standard Windows environment variables.
        /// </summary>
        [Category("Document Manager")]
        [Description("File path for layout persistence. Supports %AppData% and %LocalAppData% tokens.")]
        [DefaultValue("")]
        public string SessionFile
        {
            get => _sessionFile;
            set
            {
                _sessionFile = value ?? string.Empty;
                _view?.PushPersistence(_autoSaveLayout, ExpandSessionPath(_sessionFile));
            }
        }

        /// <summary>
        /// Cloud sync settings owned by the manager. Hidden from the designer;
        /// use <see cref="ConfigureCloudSync(BeepCloudSyncSettings?)"/> at runtime.
        /// The settings are replayed whenever the active view changes so sync remains
        /// attached to the manager rather than to a specific host instance.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public BeepCloudSyncSettings? CloudSyncSettings
        {
            get => _cloudSyncSettings;
            set
            {
                _cloudSyncSettings = value;
                ApplyCloudSyncSettings();
            }
        }

        /// <summary>
        /// <see cref="MenuStrip"/> on the host form.  The manager forwards it to the
        /// active view so a "Window" sub-menu is maintained automatically.
        /// </summary>
        [Category("Document Manager")]
        [Description("MenuStrip that receives an auto-maintained Window menu.")]
        [DefaultValue(null)]
        public MenuStrip? WindowMenuOwner
        {
            get => _windowMenuOwner;
            set
            {
                _windowMenuOwner = value;
                if (!_inInit && _view != null && value != null)
                    _view.AttachWindowMenu(value, _windowMenuText);
            }
        }

        /// <summary>Text of the Window menu item. Default <c>&amp;Window</c>.</summary>
        [Category("Document Manager")]
        [Description("Text of the Window menu item added to WindowMenuOwner.")]
        [DefaultValue("&Window")]
        public string WindowMenuText
        {
            get => _windowMenuText;
            set => _windowMenuText = string.IsNullOrEmpty(value) ? "&Window" : value;
        }

        /// <summary>
        /// <see cref="StatusStrip"/> that the manager populates with a
        /// document-path breadcrumb and dirty indicator (Phase 05 enhancement).
        /// </summary>
        [Category("Document Manager")]
        [Description("StatusStrip that receives breadcrumb and dirty-indicator items.")]
        [DefaultValue(null)]
        public StatusStrip? StatusStripOwner
        {
            get => _statusStripOwner;
            set
            {
                if (ReferenceEquals(_statusStripOwner, value)) return;
                DetachStatusStrip();
                _statusStripOwner = value;
                if (!_inInit) AttachStatusStrip();
            }
        }

        /// <summary>
        /// When <see langword="true"/> (default), <see cref="StatusStripOwner"/>
        /// receives the manager's default document + workspace items automatically.
        /// </summary>
        [Category("Document Manager")]
        [Description("When true, StatusStripOwner is populated automatically with document and workspace items.")]
        [DefaultValue(true)]
        public bool AutoPopulateStatusStrip
        {
            get => _autoPopulateStatusStrip;
            set
            {
                if (_autoPopulateStatusStrip == value) return;
                _autoPopulateStatusStrip = value;
                if (_inInit) return;

                if (_autoPopulateStatusStrip)
                    AttachStatusStrip();
                else
                    DetachStatusStrip();
            }
        }

        /// <summary>
        /// Default float / pin / split policy applied to <see cref="View"/> during
        /// <see cref="ISupportInitialize.EndInit"/>.
        /// </summary>
        [Category("Document Manager")]
        [Description("Default document float / pin / split policy pushed to the view.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public BeepDocumentPolicy DefaultPolicy
        {
            get => _defaultPolicy;
            set
            {
                _defaultPolicy = value ?? new BeepDocumentPolicy();
                if (!_inInit)
                    _view?.PushPolicy(_defaultPolicy);
            }
        }

        /// <summary>
        /// When <see langword="true"/> (default) <see cref="ISupportInitialize.EndInit"/>
        /// automatically pushes all manager settings onto <see cref="View"/>.
        /// </summary>
        [Category("Document Manager")]
        [Description("When true, EndInit applies all manager settings to the view automatically.")]
        [DefaultValue(true)]
        public bool AutoWireForm
        {
            get => _autoWireForm;
            set => _autoWireForm = value;
        }

        /// <summary>
        /// Documents to open automatically when the view is first attached.
        /// Configure at design time in the Properties grid or via the
        /// "Edit Documents\u2026" smart-tag action.
        /// At runtime, if <see cref="AutoSaveLayout"/> is true and a session file
        /// exists, the restored layout takes precedence over this collection.
        /// </summary>
        [Category("Document Manager")]
        [Description("Documents opened when the view is first attached. Configure at design time.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Editor(
            "TheTechIdea.Beep.Winform.Controls.Design.Server.Editors.DesignTimeDocumentsEditor, TheTechIdea.Beep.Winform.Controls.Design.Server",
            "System.Drawing.Design.UITypeEditor, System.Drawing")]
        public Collection<DocumentDescriptor> DesignTimeDocuments => _designTimeDocuments;

        // ══════════════════════════════════════════════════════════════════════
        // Events (mirror the view's surface so form code-behind goes here, not on the view)
        // ══════════════════════════════════════════════════════════════════════

        /// <summary>Fires after a document is added to <see cref="View"/>.</summary>
        [Category("Document Manager")]
        [Description("Fires when a new document is added.")]
        public event EventHandler<DocumentAddedEventArgs>? DocumentAdded;

        /// <summary>Fires after a document is closed on <see cref="View"/>.</summary>
        [Category("Document Manager")]
        [Description("Fires when a document is closed.")]
        public event EventHandler<DocumentEventArgs>? DocumentRemoved;

        /// <summary>Fires when the active (foreground) document changes.</summary>
        [Category("Document Manager")]
        [Description("Fires when the active document changes.")]
        public event EventHandler<DocumentEventArgs>? ActiveDocumentChanged;

        /// <summary>
        /// Fires before a document is closed.  Set <see cref="TabClosingEventArgs.Cancel"/>
        /// to <see langword="true"/> to prevent closing.
        /// </summary>
        [Category("Document Manager")]
        [Description("Fires before a document is closed. Cancel = true prevents closing.")]
        public event EventHandler<TabClosingEventArgs>? DocumentClosing;

        /// <summary>Fires after <see cref="SaveLayout"/> completes.</summary>
        [Category("Document Manager")]
        [Description("Fires after a layout save completes.")]
        public event EventHandler<ManagerLayoutChangedEventArgs>? LayoutChanged;

        /// <summary>Fires after the active <see cref="View"/> saves a workspace.</summary>
        [Category("Document Manager")]
        [Description("Fires after a workspace is saved (created or updated).")]
        public event EventHandler<WorkspaceEventArgs>? WorkspaceSaved;

        /// <summary>Fires after the active <see cref="View"/> deletes a workspace.</summary>
        [Category("Document Manager")]
        [Description("Fires after a workspace is deleted.")]
        public event EventHandler<WorkspaceEventArgs>? WorkspaceDeleted;

        /// <summary>Fires after the active <see cref="View"/> switches to a different workspace.</summary>
        [Category("Document Manager")]
        [Description("Fires after the active workspace is switched.")]
        public event EventHandler<WorkspaceEventArgs>? WorkspaceSwitched;

        // ══════════════════════════════════════════════════════════════════════
        // Workspace façade — routes through the active view
        // ══════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Saves the current layout as a named workspace on the active <see cref="View"/>.
        /// </summary>
        public WorkspaceDefinition? SaveWorkspace(string name, string description = "")
            => GetTabbedHost()?.SaveWorkspace(name, description);

        /// <summary>
        /// Switches to the named workspace on the active <see cref="View"/>.
        /// </summary>
        public LayoutRestoreReport? SwitchWorkspace(string name)
            => GetTabbedHost()?.SwitchWorkspace(name);

        /// <summary>
        /// Deletes the named workspace from the active <see cref="View"/>.
        /// </summary>
        public bool DeleteWorkspace(string name)
            => GetTabbedHost()?.DeleteWorkspace(name) ?? false;

        /// <summary>
        /// Returns all saved workspaces from the active <see cref="View"/>.
        /// Returns an empty list when no view is attached.
        /// </summary>
        public IReadOnlyList<WorkspaceDefinition> GetAllWorkspaces()
            => GetTabbedHost()?.GetAllWorkspaces() ?? Array.Empty<WorkspaceDefinition>();

        /// <summary>Name of the currently active workspace, or <c>null</c>.</summary>
        public string? ActiveWorkspaceName => GetTabbedHost()?.ActiveWorkspaceName;

        // ══════════════════════════════════════════════════════════════════════
        // Façade methods — all route through the active view
        // ══════════════════════════════════════════════════════════════════════

        /// <summary>Adds a new document to the active <see cref="View"/>.</summary>
        public BeepDocumentPanel? AddDocument(
            string title, string? iconPath = null, bool activate = true)
            => _view?.AddDocument(title, iconPath ?? string.Empty, activate);

        /// <summary>Closes the document identified by <paramref name="id"/>.</summary>
        public bool RemoveDocument(string id, bool force = false)
        {
            if (_view == null || string.IsNullOrEmpty(id)) return false;
            return _view.RemoveDocument(id);
        }

        /// <summary>Brings the document with <paramref name="id"/> to the foreground.</summary>
        public void ActivateDocument(string id)
        {
            if (_view == null || string.IsNullOrEmpty(id)) return;
            _view.ActivateDocument(id);
        }

        /// <summary>
        /// Suspends layout while multiple documents are added — call
        /// <see cref="EndBatchAddDocuments"/> to flush.
        /// </summary>
        public void BeginBatchAddDocuments() => _view?.BeginBatchAddDocuments();

        /// <summary>Ends the batch and performs a single layout pass.</summary>
        public void EndBatchAddDocuments()   => _view?.EndBatchAddDocuments();

        /// <summary>Closes every open document on the active view.</summary>
        public bool CloseAllDocuments()      => _view?.CloseAllDocuments() ?? false;

        /// <summary>
        /// Returns the <see cref="BeepDocumentPanel"/> for the given document id,
        /// or <see langword="null"/> when not found or when the view does not use panels
        /// (e.g. native MDI mode).
        /// </summary>
        public BeepDocumentPanel? GetPanel(string documentId)
            => string.IsNullOrEmpty(documentId) ? null : _view?.GetPanel(documentId);

        /// <summary>
        /// Replaces the active view with a fresh instance of <typeparamref name="TView"/>.
        /// The new view is attached, configured with current manager settings, and the
        /// old one is detached + disposed.
        /// </summary>
        public TView ChangeView<TView>() where TView : IBeepDocumentManagerView, new()
        {
            var fresh = new TView();
            View = fresh;        // setter handles Detach + Attach + ApplyViewSettings
            return fresh;
        }

        /// <summary>
        /// Saves the current layout to <see cref="SessionFile"/> and raises
        /// <see cref="LayoutChanged"/>.
        /// </summary>
        public void SaveLayout()
        {
            if (_view == null) return;
            string? json = null;
            try
            {
                var path = ExpandSessionPath(_sessionFile);
                if (!string.IsNullOrEmpty(path))
                    _view.SaveLayoutToFile(path);
                json = _view.SaveLayout();
            }
            catch { /* best-effort */ }
            LayoutChanged?.Invoke(this, new ManagerLayoutChangedEventArgs(json));
        }

        /// <summary>
        /// Activates cloud sync through the currently bound tabbed host while storing
        /// the settings on the manager so the same sync configuration survives view swaps.
        /// Pass <see langword="null"/> to disable sync.
        /// </summary>
        public void ConfigureCloudSync(BeepCloudSyncSettings? settings)
            => CloudSyncSettings = settings;

        /// <summary>
        /// Triggers an on-demand cloud sync through the currently bound tabbed host.
        /// Returns immediately when no tabbed host is attached.
        /// </summary>
        public System.Threading.Tasks.Task SyncToCloudAsync(
            System.Threading.CancellationToken ct = default)
        {
            var host = GetTabbedHost();
            return host != null
                ? host.SyncToCloudAsync(ct)
                : System.Threading.Tasks.Task.CompletedTask;
        }

        /// <summary>Restores the layout from <see cref="SessionFile"/>.</summary>
        public void LoadLayout()
        {
            if (_view == null || string.IsNullOrEmpty(_sessionFile)) return;
            try
            {
                var path = ExpandSessionPath(_sessionFile);
                if (!string.IsNullOrEmpty(path) && File.Exists(path))
                    _view.TryRestoreLayout(File.ReadAllText(path));
            }
            catch { /* non-fatal */ }
        }

        // ── Phase 06: command surface ─────────────────────────────────────────

        /// <summary>
        /// Returns the <see cref="BeepCommandRegistry"/> for the underlying
        /// <see cref="BeepDocumentHost"/>, or <see langword="null"/> when the
        /// active view does not expose a host (e.g. <see cref="BeepNativeMdiView"/>).
        /// </summary>
        [Browsable(false)]
        public BeepCommandRegistry? CommandRegistry =>
            (_view as BeepTabbedView)?.Host?.CommandRegistry;

        /// <summary>
        /// Returns the number of documents currently open in the active view.
        /// Returns 0 when no view is attached.
        /// </summary>
        [Browsable(false)]
        public int DocumentCount => (_view as BeepTabbedView)?.Host?.DocumentCount ?? 0;

        /// <summary>
        /// Registers a named command with an optional keyboard shortcut in the
        /// host's <see cref="BeepCommandRegistry"/>.  No-ops when no registry is available.
        /// </summary>
        /// <param name="id">Unique command identifier, e.g. <c>"myapp.openSettings"</c>.</param>
        /// <param name="title">Human-readable label shown in the command palette.</param>
        /// <param name="callback">Action executed when the command is invoked.</param>
        /// <param name="shortcut">Optional keyboard shortcut key(s).</param>
        public void RegisterCommand(string id, string title, Action callback, Keys shortcut = Keys.None)
        {
            var registry = CommandRegistry;
            if (registry == null) return;
            var entry = new BeepCommandEntry
            {
                Id       = id,
                Title    = title,
                Category = "Custom",
                Execute  = callback,
                Shortcut = shortcut == Keys.None ? null : shortcut.ToString()
            };
            registry.Register(entry);
        }

        /// <summary>
        /// Registers a factory that creates a <see cref="Control"/> for documents
        /// whose <see cref="DocumentDescriptor.Id"/> starts with <paramref name="typeKey"/>.
        /// Used by Phase 07 layout restore.
        /// </summary>
        public void RegisterTemplate(string typeKey, Func<DocumentDescriptor, Control> factory)
        {
            if (!string.IsNullOrEmpty(typeKey) && factory != null)
                _templates[typeKey] = factory;
        }

        // ══════════════════════════════════════════════════════════════════════
        // Private helpers
        // ══════════════════════════════════════════════════════════════════════

        private void ApplyViewSettings()
        {
            if (_view == null) return;

            // Wire events first so any side-effects of settings propagation reach subscribers
            _view.DocumentAdded         += View_DocumentAdded;
            _view.DocumentRemoved       += View_DocumentRemoved;
            _view.ActiveDocumentChanged += View_ActiveDocumentChanged;
            _view.DocumentClosing       += View_DocumentClosing;

            // Notify the view it now belongs to this manager
            _view.Attach(this);

            // Push scalar settings
            if (!string.IsNullOrEmpty(_themeName))   _view.PushTheme(_themeName);
            _view.PushPersistence(_autoSaveLayout, ExpandSessionPath(_sessionFile));
            _view.PushPolicy(_defaultPolicy);
            ApplyCloudSyncSettings();

            // Wire the Window menu
            if (_windowMenuOwner != null)
                _view.AttachWindowMenu(_windowMenuOwner, _windowMenuText);

            // Auto-add controls that were extended in InitializeComponent
            AutoAddExtendedControls();

            // Open any design-time-seeded documents
            ApplyDesignTimeDocuments();

            // Keep status-strip host bindings aligned when the view changes.
            RefreshStatusStripState();

            // Phase 06 D1 — wire form-level shortcut forwarding
            HookOwnerForm();
        }

        private void DetachView()
        {
            if (_view == null) return;
            try { GetTabbedHost()?.ConfigureCloudSync(null); } catch { }
            try { _view.DocumentAdded         -= View_DocumentAdded;         } catch { }
            try { _view.DocumentRemoved       -= View_DocumentRemoved;       } catch { }
            try { _view.ActiveDocumentChanged -= View_ActiveDocumentChanged; } catch { }
            try { _view.DocumentClosing       -= View_DocumentClosing;       } catch { }
            try { _view.Detach(); } catch { }
            UnwireWorkspaceHost();
            UnhookOwnerForm();
        }

        private void View_DocumentAdded(object? s, DocumentAddedEventArgs e)
            => DocumentAdded?.Invoke(this, e);

        private void View_DocumentRemoved(object? s, DocumentEventArgs e)
            => DocumentRemoved?.Invoke(this, e);

        private void View_ActiveDocumentChanged(object? s, DocumentEventArgs e)
            => ActiveDocumentChanged?.Invoke(this, e);

        private void View_DocumentClosing(object? s, TabClosingEventArgs e)
            => DocumentClosing?.Invoke(this, e);

        private void AutoAddExtendedControls()
        {
            if (_view == null) return;
            foreach (var ctrl in _extendedControls)
            {
                if (!_attachedInfo.TryGetValue(ctrl, out var info)) continue;
                if (string.IsNullOrEmpty(info.Title)) continue;

                try
                {
                    var panel = _view.AddDocument(info.Title, info.IconPath ?? string.Empty, false);
                    if (panel != null && !panel.Controls.Contains(ctrl))
                    {
                        ctrl.Dock = DockStyle.Fill;
                        panel.Controls.Add(ctrl);
                    }
                }
                catch { /* non-fatal at design-time */ }
            }
        }

        private void ApplyDesignTimeDocuments()
        {
            if (_view == null || _designTimeDocuments.Count == 0) return;
            _view.BeginBatchAddDocuments();
            try
            {
                foreach (var desc in _designTimeDocuments)
                    _view.AddDocument(desc);
            }
            finally
            {
                _view.EndBatchAddDocuments();
            }
        }

        private static string ExpandSessionPath(string path)
        {
            if (string.IsNullOrEmpty(path)) return path;
            return Environment.ExpandEnvironmentVariables(
                path.Replace("%AppData%",      Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData))
                    .Replace("%LocalAppData%", Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)));
        }

        private void ApplyCloudSyncSettings()
        {
            var host = GetTabbedHost();
            if (host == null) return;
            host.ConfigureCloudSync(_cloudSyncSettings);
        }

        // ── Phase 05 G: Status strip integration ─────────────────────────────

        private void AttachStatusStrip()
        {
            if (!_autoPopulateStatusStrip || _statusStripOwner == null || _statusDocTitle != null) return;

            _statusDocTitle = new ToolStripStatusLabel
            {
                Text      = "No document",
                Spring    = true,
                TextAlign = ContentAlignment.MiddleLeft,
                AutoSize  = false,
            };
            _statusModified = new ToolStripStatusLabel
            {
                Text      = string.Empty,
                TextAlign = ContentAlignment.MiddleRight,
                AutoSize  = true,
            };
            _statusCursor = new ToolStripStatusLabel
            {
                Text      = string.Empty,
                TextAlign = ContentAlignment.MiddleRight,
                AutoSize  = true,
                Available = false,
            };
            _statusWorkspace = new ToolStripDropDownButton
            {
                Text         = "Workspace",
                DisplayStyle = ToolStripItemDisplayStyle.Text,
                AutoToolTip  = false,
            };
            _statusWorkspace.DropDownOpening += StatusWorkspace_DropDownOpening;
            _statusStripOwner.Items.Add(_statusDocTitle);
            _statusStripOwner.Items.Add(_statusModified);
            _statusStripOwner.Items.Add(_statusCursor);
            _statusStripOwner.Items.Add(_statusWorkspace);
            ActiveDocumentChanged += StatusStrip_ActiveDocumentChanged;
            RefreshStatusStripState();
        }

        private void DetachStatusStrip()
        {
            ActiveDocumentChanged -= StatusStrip_ActiveDocumentChanged;
            UnwireTrackedPanel();
            UnwireWorkspaceHost();
            if (_statusStripOwner != null)
            {
                if (_statusDocTitle != null)
                {
                    _statusStripOwner.Items.Remove(_statusDocTitle);
                    _statusDocTitle.Dispose();
                    _statusDocTitle = null;
                }
                if (_statusModified != null)
                {
                    _statusStripOwner.Items.Remove(_statusModified);
                    _statusModified.Dispose();
                    _statusModified = null;
                }
                if (_statusCursor != null)
                {
                    _statusStripOwner.Items.Remove(_statusCursor);
                    _statusCursor.Dispose();
                    _statusCursor = null;
                }
                if (_statusWorkspace != null)
                {
                    _statusWorkspace.DropDownOpening -= StatusWorkspace_DropDownOpening;
                    _statusStripOwner.Items.Remove(_statusWorkspace);
                    _statusWorkspace.Dispose();
                    _statusWorkspace = null;
                }
            }
        }

        private void StatusStrip_ActiveDocumentChanged(object? sender, DocumentEventArgs e)
        {
            var panel = string.IsNullOrEmpty(e.DocumentId) ? null : GetPanel(e.DocumentId);
            UpdateTrackedPanel(panel);
        }

        private void StatusStrip_ModifiedChanged(object? sender, EventArgs e)
            => UpdateModifiedLabel((sender as BeepDocumentPanel)?.IsModified == true);

        private void UpdateModifiedLabel(bool isModified)
        {
            if (_statusModified == null) return;
            _statusModified.Text = isModified ? "\u25cf" : string.Empty;
        }

        private void UnwireTrackedPanel()
        {
            if (_activeTrackedPanel == null) return;
            _activeTrackedPanel.ModifiedChanged -= StatusStrip_ModifiedChanged;
            _activeTrackedPanel.ControlAdded -= ActiveTrackedPanel_ControlChanged;
            _activeTrackedPanel.ControlRemoved -= ActiveTrackedPanel_ControlChanged;
            _activeTrackedPanel = null;
            UnwireCursorSource();
        }

        // ── Phase 06 D1: form-level shortcut forwarding ───────────────────────

        /// <summary>
        /// Finds the parent <see cref="Form"/> that hosts the <see cref="BeepDocumentHost"/>
        /// and installs a <c>KeyPreview</c> handler so Ctrl+Shift+P / Ctrl+Tab / Ctrl+P
        /// reach the host even when another control on the form has keyboard focus.
        /// </summary>
        private void HookOwnerForm()
        {
            var host = GetTabbedHost();
            if (host == null) return;

            // If the host is already parented to a form, wire immediately.
            var form = host.FindForm();
            if (form != null)
            {
                WireParentForm(form);
                return;
            }

            // Otherwise defer: subscribe to ParentChanged so we catch the moment
            // the host is added to the form's control tree.
            if (!_hostHandleHooked)
            {
                _hostHandleHooked = true;
                host.ParentChanged += Host_ParentChanged;
            }
        }

        private void Host_ParentChanged(object? sender, EventArgs e)
        {
            if (sender is not BeepDocumentHost host) return;
            var form = host.FindForm();
            if (form == null) return;

            // Unsubscribe immediately — we only need this once.
            host.ParentChanged -= Host_ParentChanged;
            _hostHandleHooked = false;
            WireParentForm(form);
        }

        private void WireParentForm(Form form)
        {
            if (ReferenceEquals(_parentForm, form)) return;
            UnhookOwnerForm();
            _parentForm = form;
            _parentForm.KeyPreview = true;
            _parentForm.KeyDown += OwnerForm_KeyDown;
            _parentForm.FormClosed += OwnerForm_FormClosed;
        }

        private void UnhookOwnerForm()
        {
            // Unsubscribe pending ParentChanged if still hooked.
            if (_hostHandleHooked)
            {
                var host = GetTabbedHost();
                if (host != null) host.ParentChanged -= Host_ParentChanged;
                _hostHandleHooked = false;
            }

            if (_parentForm == null) return;
            _parentForm.KeyDown -= OwnerForm_KeyDown;
            _parentForm.FormClosed -= OwnerForm_FormClosed;
            _parentForm = null;
        }

        private void OwnerForm_FormClosed(object? sender, FormClosedEventArgs e)
            => UnhookOwnerForm();

        private void OwnerForm_KeyDown(object? sender, KeyEventArgs e)
        {
            var host = GetTabbedHost();
            if (host == null) return;

            // When focus is already inside the host, ProcessCmdKey handles it.
            // Only intercept when focus is elsewhere on the form.
            if (host.ContainsFocus) return;

            if (e.KeyData == (Keys.Control | Keys.Shift | Keys.P))
            {
                host.ShowCommandPalette(CommandPaletteMode.Commands);
                e.Handled = e.SuppressKeyPress = true;
            }
            else if (e.KeyData == (Keys.Control | Keys.P))
            {
                host.ShowCommandPalette(CommandPaletteMode.GoToFile);
                e.Handled = e.SuppressKeyPress = true;
            }
            else if (e.KeyData == (Keys.Control | Keys.Tab))
            {
                host.ShowQuickSwitch();
                e.Handled = e.SuppressKeyPress = true;
            }
        }

        // ─────────────────────────────────────────────────────────────────────

        private void RefreshStatusStripState()
        {
            if (_statusDocTitle == null || !_autoPopulateStatusStrip) return;

            RefreshWorkspaceHostBinding();
            var host = GetTabbedHost();
            UpdateTrackedPanel(host?.ActivePanel);
            UpdateWorkspaceLabel();
        }

        private void UpdateTrackedPanel(BeepDocumentPanel? panel)
        {
            UnwireTrackedPanel();

            if (_statusDocTitle == null)
                return;

            if (panel == null)
            {
                _statusDocTitle.Text = "No document";
                UpdateModifiedLabel(false);
                UpdateCursorLabel(null);
                return;
            }

            _statusDocTitle.Text = string.IsNullOrEmpty(panel.DocumentTitle)
                ? panel.DocumentId
                : panel.DocumentTitle;

            _activeTrackedPanel = panel;
            _activeTrackedPanel.ModifiedChanged += StatusStrip_ModifiedChanged;
            _activeTrackedPanel.ControlAdded += ActiveTrackedPanel_ControlChanged;
            _activeTrackedPanel.ControlRemoved += ActiveTrackedPanel_ControlChanged;
            UpdateModifiedLabel(panel.IsModified);
            RefreshCursorSource();
        }

        private void ActiveTrackedPanel_ControlChanged(object? sender, ControlEventArgs e)
            => RefreshCursorSource();

        private void RefreshWorkspaceHostBinding()
        {
            var host = GetTabbedHost();
            if (ReferenceEquals(_trackedWorkspaceHost, host))
                return;

            UnwireWorkspaceHost();
            _trackedWorkspaceHost = host;
            if (_trackedWorkspaceHost == null)
                return;

            _trackedWorkspaceHost.WorkspaceSaved    += StatusWorkspace_HostChanged;
            _trackedWorkspaceHost.WorkspaceDeleted  += StatusWorkspace_HostChanged;
            _trackedWorkspaceHost.WorkspaceSwitched += StatusWorkspace_HostChanged;

            // Forward host workspace events to manager-level events so that
            // extensions (e.g. BeepCloudSyncManager) bound to the manager
            // continue to receive notifications after a view switch.
            _trackedWorkspaceHost.WorkspaceSaved    += OnHostWorkspaceSaved;
            _trackedWorkspaceHost.WorkspaceDeleted  += OnHostWorkspaceDeleted;
            _trackedWorkspaceHost.WorkspaceSwitched += OnHostWorkspaceSwitched;
        }

        private void UnwireWorkspaceHost()
        {
            if (_trackedWorkspaceHost == null)
                return;

            _trackedWorkspaceHost.WorkspaceSaved    -= StatusWorkspace_HostChanged;
            _trackedWorkspaceHost.WorkspaceDeleted  -= StatusWorkspace_HostChanged;
            _trackedWorkspaceHost.WorkspaceSwitched -= StatusWorkspace_HostChanged;

            _trackedWorkspaceHost.WorkspaceSaved    -= OnHostWorkspaceSaved;
            _trackedWorkspaceHost.WorkspaceDeleted  -= OnHostWorkspaceDeleted;
            _trackedWorkspaceHost.WorkspaceSwitched -= OnHostWorkspaceSwitched;
            _trackedWorkspaceHost = null;
        }

        private void OnHostWorkspaceSaved(object? sender, WorkspaceEventArgs e)
            => WorkspaceSaved?.Invoke(this, e);

        private void OnHostWorkspaceDeleted(object? sender, WorkspaceEventArgs e)
            => WorkspaceDeleted?.Invoke(this, e);

        private void OnHostWorkspaceSwitched(object? sender, WorkspaceEventArgs e)
            => WorkspaceSwitched?.Invoke(this, e);

        private void StatusWorkspace_HostChanged(object? sender, WorkspaceEventArgs e)
            => RefreshStatusStripState();

        private void RefreshCursorSource()
        {
            UnwireCursorSource();

            if (_activeTrackedPanel == null)
            {
                UpdateCursorLabel(null);
                return;
            }

            if (TryFindStatusProvider(_activeTrackedPanel, out var provider))
            {
                _trackedStatusProvider = provider;
                _trackedStatusProvider.StatusInfoChanged += TrackedStatusProvider_StatusInfoChanged;
                UpdateCursorLabel(_trackedStatusProvider.GetStatusInfo());
                return;
            }

            if (TryFindTextBox(_activeTrackedPanel, out var textBox))
            {
                _trackedTextBox = textBox;
                _trackedTextBox.GotFocus += TrackedTextBox_StatusChanged;
                _trackedTextBox.MouseUp += TrackedTextBox_StatusChanged;
                _trackedTextBox.KeyUp += TrackedTextBox_KeyUp;
                _trackedTextBox.TextChanged += TrackedTextBox_StatusChanged;
                if (_trackedTextBox is RichTextBox richTextBox)
                    richTextBox.SelectionChanged += TrackedRichTextBox_SelectionChanged;

                UpdateCursorLabel(CreateStatusInfo(_trackedTextBox));
                return;
            }

            UpdateCursorLabel(null);
        }

        private void UnwireCursorSource()
        {
            if (_trackedStatusProvider != null)
            {
                _trackedStatusProvider.StatusInfoChanged -= TrackedStatusProvider_StatusInfoChanged;
                _trackedStatusProvider = null;
            }

            if (_trackedTextBox == null)
                return;

            _trackedTextBox.GotFocus -= TrackedTextBox_StatusChanged;
            _trackedTextBox.MouseUp -= TrackedTextBox_StatusChanged;
            _trackedTextBox.KeyUp -= TrackedTextBox_KeyUp;
            _trackedTextBox.TextChanged -= TrackedTextBox_StatusChanged;
            if (_trackedTextBox is RichTextBox richTextBox)
                richTextBox.SelectionChanged -= TrackedRichTextBox_SelectionChanged;
            _trackedTextBox = null;
        }

        private void TrackedStatusProvider_StatusInfoChanged(object? sender, EventArgs e)
            => UpdateCursorLabel(_trackedStatusProvider?.GetStatusInfo());

        private void TrackedTextBox_StatusChanged(object? sender, EventArgs e)
            => UpdateCursorLabel(sender is TextBoxBase textBox ? CreateStatusInfo(textBox) : null);

        private void TrackedTextBox_KeyUp(object? sender, KeyEventArgs e)
            => TrackedTextBox_StatusChanged(sender, EventArgs.Empty);

        private void TrackedRichTextBox_SelectionChanged(object? sender, EventArgs e)
            => TrackedTextBox_StatusChanged(sender, e);

        private void UpdateCursorLabel(StatusBarInfo? statusInfo)
        {
            if (_statusCursor == null)
                return;

            if (statusInfo == null)
            {
                _statusCursor.Text = string.Empty;
                _statusCursor.Available = false;
                return;
            }

            _statusCursor.Text = statusInfo.SelectionLen > 0
                ? $"Ln {statusInfo.Line}, Col {statusInfo.Column}  Sel {statusInfo.SelectionLen}"
                : $"Ln {statusInfo.Line}, Col {statusInfo.Column}";
            _statusCursor.Available = true;
        }

        private static StatusBarInfo CreateStatusInfo(TextBoxBase textBox)
        {
            int selectionStart = textBox.SelectionStart;
            int lineIndex = Math.Max(0, textBox.GetLineFromCharIndex(selectionStart));
            int firstChar = textBox.GetFirstCharIndexFromLine(lineIndex);
            if (firstChar < 0)
                firstChar = 0;

            return new StatusBarInfo
            {
                Line = lineIndex + 1,
                Column = Math.Max(1, selectionStart - firstChar + 1),
                SelectionLen = textBox.SelectionLength,
            };
        }

        private static bool TryFindStatusProvider(Control root, out IDocumentStatusInfoProvider provider)
        {
            if (root is IDocumentStatusInfoProvider rootProvider)
            {
                provider = rootProvider;
                return true;
            }

            foreach (Control child in root.Controls)
            {
                if (TryFindStatusProvider(child, out provider))
                    return true;
            }

            provider = null!;
            return false;
        }

        private static bool TryFindTextBox(Control root, out TextBoxBase textBox)
        {
            if (root is TextBoxBase rootTextBox)
            {
                textBox = rootTextBox;
                return true;
            }

            foreach (Control child in root.Controls)
            {
                if (TryFindTextBox(child, out textBox))
                    return true;
            }

            textBox = null!;
            return false;
        }

        private void UpdateWorkspaceLabel()
        {
            if (_statusWorkspace == null)
                return;

            var host = GetTabbedHost();
            _statusWorkspace.Enabled = host != null;
            _statusWorkspace.Text = host == null || string.IsNullOrWhiteSpace(host.ActiveWorkspaceName)
                ? "Workspace"
                : host.ActiveWorkspaceName;
        }

        private void StatusWorkspace_DropDownOpening(object? sender, EventArgs e)
        {
            if (_statusWorkspace == null)
                return;

            _statusWorkspace.DropDownItems.Clear();
            var host = GetTabbedHost();
            if (host == null)
            {
                var unavailable = _statusWorkspace.DropDownItems.Add("No tabbed workspace host");
                unavailable.Enabled = false;
                return;
            }

            _statusWorkspace.DropDownItems.Add("Save Current As…", null, StatusWorkspace_SaveCurrentAsClicked);

            var all = host.GetAllWorkspaces();
            if (all.Count == 0)
            {
                _statusWorkspace.DropDownItems.Add("(No saved workspaces)", null, (_, _) => { }).Enabled = false;
                return;
            }

            _statusWorkspace.DropDownItems.Add(new ToolStripSeparator());
            foreach (var ws in all)
            {
                string name = ws.Name;
                var item = new ToolStripMenuItem(name)
                {
                    Checked = string.Equals(name, host.ActiveWorkspaceName, StringComparison.OrdinalIgnoreCase)
                };
                item.Click += (_, _) => SwitchWorkspaceFromStatusStrip(name);
                _statusWorkspace.DropDownItems.Add(item);
            }
        }

        private void StatusWorkspace_SaveCurrentAsClicked(object? sender, EventArgs e)
        {
            var host = GetTabbedHost();
            if (host == null)
                return;

            var (result, value) = DialogHelper.InputBox(
                "Save Workspace",
                "Workspace name:",
                host.ActiveWorkspaceName ?? "Default");

            if (result != BeepDialogResult.OK)
                return;

            string name = value?.Trim() ?? string.Empty;
            if (string.IsNullOrEmpty(name))
                return;

            try
            {
                host.SaveWorkspace(name);
                RefreshStatusStripState();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Save Workspace Failed",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void SwitchWorkspaceFromStatusStrip(string name)
        {
            var host = GetTabbedHost();
            if (host == null || string.IsNullOrWhiteSpace(name))
                return;

            try
            {
                host.SwitchWorkspace(name);
                RefreshStatusStripState();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Switch Workspace Failed",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private BeepDocumentHost? GetTabbedHost()
            => (_view as BeepTabbedView)?.Host;

        // ── Dispose ───────────────────────────────────────────────────────────

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Auto-flush layout so the next session can restore where we left off
                if (_autoSaveLayout && _view != null)
                {
                    try
                    {
                        var path = ExpandSessionPath(_sessionFile);
                        if (!string.IsNullOrEmpty(path))
                            _view.SaveLayoutToFile(path);
                    }
                    catch { /* best-effort flush — must not throw during disposal */ }
                }

                UnhookOwnerForm();
                DisposeDisplayContainer();
                DetachView();
                DetachStatusStrip();
                _view             = null;
                _windowMenuOwner  = null;
                _statusStripOwner = null;
                _extendedControls.Clear();
            }
            base.Dispose(disposing);
        }

        // ── Inner type ────────────────────────────────────────────────────────

        private sealed class AttachedDocInfo
        {
            public string? Title    { get; set; }
            public string? IconPath { get; set; }
        }
    }
}
