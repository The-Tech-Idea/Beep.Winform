// BeepDocumentHost.Properties.cs
// Backing fields, designable properties, and public events for BeepDocumentHost.
// All state fields live in BeepDocumentHost.cs (core partial).
// ─────────────────────────────────────────────────────────────────────────────────────────
using System.ComponentModel;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.DocumentHost.Tokens;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost
{
    /// <summary>Controls the border/elevation appearance of a <see cref="BeepDocumentHost"/>.</summary>
    public enum DocumentHostStyle
    {
        /// <summary>No border — content area merges seamlessly with the parent surface.</summary>
        Flat,
        /// <summary>Thin 1 px border drawn using the theme's BorderColor.</summary>
        Thin,
        /// <summary>Raised 3D border that lifts the host above surrounding controls.</summary>
        Raised
    }

    public partial class BeepDocumentHost
    {
        // ─────────────────────────────────────────────────────────────────────
        // Backing fields  (referenced in the constructor in the core partial)
        // ─────────────────────────────────────────────────────────────────────

        private TabStripPosition _tabPosition   = TabStripPosition.Top;
        private bool             _showAddButton = true;
        private TabCloseMode     _closeMode     = TabCloseMode.OnHover;
        private DocumentTabStyle _tabStyle      = DocumentTabStyle.Chrome;
        private DocumentHostStyle _controlStyle = DocumentHostStyle.Flat;
        private bool             _keyboardShortcutsEnabled = true;
        private TabColorMode     _tabColorMode  = TabColorMode.None;
        private bool             _autoSaveLayout = false;
        private string           _sessionFile   = string.Empty;

        // 6.x — Command registry and breadcrumb
        private BeepCommandRegistry? _commandRegistry;
        private BeepDocumentBreadcrumb? _breadcrumb;
        private bool _showBreadcrumb;

        // 7.x — Templates, history, undo/redo, cloud sync
        private BeepLayoutTemplateLibrary? _templateLibrary;
        private BeepLayoutHistory?         _layoutHistory;
        private BeepLayoutUndoRedo?        _undoRedo;
        private BeepCloudSyncSettings?     _cloudSyncSettings;
        private BeepCloudSyncManager?      _cloudSyncManager;

        // 8.x — Phase 8 features
        private Features.DiffViewPanel?           _diffViewer;
        private Features.BeepGitStatusProvider?   _gitStatus;
        private Features.TerminalPanel?           _terminal;
        private Features.BeepDocumentStatusBar?   _statusBar;
        private Features.BeepDocumentMiniToolbar? _miniToolbar;
        private bool _showStatusBar = true;

        // MRU tracking
        private readonly System.Collections.Generic.LinkedList<string>  _mruList     = new System.Collections.Generic.LinkedList<string>();
        private int _maxRecentHistory = 20;

        // Closed-tab history
        private readonly System.Collections.Generic.Stack<ClosedTabRecord> _closedStack = new System.Collections.Generic.Stack<ClosedTabRecord>();
        private int _maxClosedHistory = 10;

        // ─────────────────────────────────────────────────────────────────────
        // Read-only / hidden properties
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>Id of the currently visible (active) document, or null.</summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string? ActiveDocumentId => _activeDocumentId;

        /// <summary>Returns the active <see cref="BeepDocumentPanel"/>, or null.</summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public BeepDocumentPanel? ActivePanel =>
            _activeDocumentId != null && _panels.TryGetValue(_activeDocumentId, out var p) ? p : null;

        /// <summary>Number of docked (non-floated) documents.</summary>
        [Browsable(false)]
        public int DocumentCount => _panels.Count;

        /// <summary>
        /// A lazily-created <see cref="IDocumentHostCommandService"/> that wraps this host.
        /// Use this to wire commands in MVVM or to pass the service via dependency injection.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IDocumentHostCommandService CommandService
            => _commandService ??= new DocumentHostCommandService(this);
        private IDocumentHostCommandService? _commandService;

        private bool _showEmptyState = true;
        /// <summary>
        /// When <see langword="true"/> (default) a centered illustration is painted in the content
        /// area whenever there are no open documents.  Set to <see langword="false"/> to suppress it.
        /// </summary>
        [DefaultValue(true)]
        [Description("Show a centred empty-state illustration when no documents are open.")]
        public bool ShowEmptyState
        {
            get => _showEmptyState;
            set
            {
                _showEmptyState = value;
                if (_contentArea != null) _contentArea.Invalidate();
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // Designable properties
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>Where the tab strip is positioned relative to the document panels.</summary>
        [DefaultValue(TabStripPosition.Top)]
        [Description("Position of the tab strip relative to the document panels.")]
        public TabStripPosition TabPosition
        {
            get => _tabPosition;
            set { _tabPosition = value; RecalculateLayout(); }
        }

        /// <summary>Show the new-document (+) button in the tab strip.</summary>
        [DefaultValue(true)]
        [Description("Show the new-document (+) button in the tab strip.")]
        public bool ShowAddButton
        {
            get => _showAddButton;
            set { _showAddButton = value; _tabStrip.ShowAddButton = value; }
        }

        /// <summary>Controls when the close (×) button appears on tabs.</summary>
        [DefaultValue(TabCloseMode.OnHover)]
        [Description("Controls when the close (×) button is visible on each tab.")]
        public TabCloseMode CloseMode
        {
            get => _closeMode;
            set { _closeMode = value; _tabStrip.CloseMode = value; }
        }

        /// <summary>Visual rendering style propagated to the tab strip.</summary>
        [DefaultValue(DocumentTabStyle.Chrome)]
        [Description("Visual style of the tab strip (Chrome / VSCode / Underline / Pill).")]
        public DocumentTabStyle TabStyle
        {
            get => _tabStyle;
            set { _tabStyle = value; _tabStrip.TabStyle = value; }
        }

        /// <summary>Specifies the border/elevation style of the document host container.</summary>
        [DefaultValue(DocumentHostStyle.Flat)]
        [Description("Border/elevation style of the document host container.")]
        public DocumentHostStyle ControlStyle
        {
            get => _controlStyle;
            set { _controlStyle = value; ApplyThemeColors(); Invalidate(); }
        }

        /// <summary>
        /// When true, keyboard shortcuts on the tab strip (Ctrl+Tab, Ctrl+W, Ctrl+1-9) are active.
        /// Propagated to the underlying <see cref="BeepDocumentTabStrip"/>.
        /// </summary>
        [DefaultValue(true)]
        [Description("Enable built-in keyboard shortcuts on the tab strip.")]
        public bool KeyboardShortcutsEnabled
        {
            get => _keyboardShortcutsEnabled;
            set { _keyboardShortcutsEnabled = value; _tabStrip.KeyboardShortcutsEnabled = value; }
        }

        /// <summary>Controls how per-document tab colours are applied to tab backgrounds.</summary>
        [DefaultValue(TabColorMode.None)]
        [Description("How per-document tab colours are rendered: None, AccentBar, FullBackground, or BottomBorder.")]
        public TabColorMode TabColorMode
        {
            get => _tabColorMode;
            set { _tabColorMode = value; _tabStrip.TabColorMode = value; _tabStrip.Invalidate(); }
        }

        /// <summary>
        /// When true, the tab layout is automatically saved to <see cref="SessionFile"/> on dispose
        /// and restored on the next load if the file exists.
        /// </summary>
        [DefaultValue(false)]
        [Description("Automatically save and restore the tab layout using SessionFile.")]
        public bool AutoSaveLayout
        {
            get => _autoSaveLayout;
            set => _autoSaveLayout = value;
        }

        /// <summary>Path to the JSON session file used by <see cref="AutoSaveLayout"/>.</summary>
        [DefaultValue("")]
        [Description("File path for the automatic layout save/restore (AutoSaveLayout must be true).")]
        public string SessionFile
        {
            get => _sessionFile;
            set => _sessionFile = value ?? string.Empty;
        }

        /// <summary>Beep theme name propagated to the tab strip and all document panels.</summary>
        [DefaultValue("")]
        [Description("Beep theme name propagated to the tab strip and document panels.")]
        public string ThemeName
        {
            get => ThemeName;
            set
            {
                ThemeName = value ?? string.Empty;
                _currentTheme = ThemeManagement.BeepThemesManager.GetTheme(ThemeName)
                         ?? ThemeManagement.BeepThemesManager.GetDefaultTheme();
                PropagateTheme(ThemeName);
                Invalidate();
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // 5.7 — Memory management
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Maximum number of document panels whose content stays loaded simultaneously.
        /// When exceeded, the least-recently-used panels beyond this limit are unloaded via
        /// <see cref="EvictInactivePanels"/>.  Set to 0 (default) for unlimited.
        /// </summary>
        [DefaultValue(0)]
        [Description("Maximum simultaneously loaded panels. 0 = unlimited. " +
                     "Least-recently-used panels beyond this limit are unloaded to free memory.")]
        public int MaxActivePanels
        {
            get => _maxActivePanels;
            set
            {
                _maxActivePanels = System.Math.Max(0, value);
                if (_maxActivePanels > 0) EvictInactivePanels();
            }
        }

        /// <summary>
        /// Unloads the content of panels that exceed <see cref="MaxActivePanels"/>.
        /// The least-recently-used documents are unloaded first.
        /// Has no effect when <see cref="MaxActivePanels"/> is 0.
        /// </summary>
        public void EvictInactivePanels()
        {
            if (_maxActivePanels <= 0 || _panels.Count <= _maxActivePanels) return;

            // Build a keep-set from the most-recently-used MRU positions
            var keepSet = new System.Collections.Generic.HashSet<string>(StringComparer.Ordinal);
            int kept = 0;
            foreach (var id in _mruList)
            {
                if (kept >= _maxActivePanels) break;
                keepSet.Add(id);
                kept++;
            }
            // Never evict the currently active document
            if (_activeDocumentId != null) keepSet.Add(_activeDocumentId);

            foreach (var (id, panel) in _panels)
            {
                if (!keepSet.Contains(id))
                    panel.UnloadContent();
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // 5.8 — Performance profiler
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Lazily-created performance profiler.  Access to start recording FPS, layout
        /// timings, and memory stats.  Use <see cref="BeepDocumentHostProfiler.GetSnapshot"/>
        /// to read the current values.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public BeepDocumentHostProfiler Profiler => _profiler ??= new BeepDocumentHostProfiler();

        // ─────────────────────────────────────────────────────────────────────
        // 7.1 — Layout template library
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Library of named layout templates (10 built-ins + custom).
        /// Call <see cref="BeepLayoutTemplateLibrary.ApplyTemplate"/> or use the
        /// palette command <c>template.pick</c> to apply a template interactively.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public BeepLayoutTemplateLibrary TemplateLibrary =>
            _templateLibrary ??= new BeepLayoutTemplateLibrary();

        // ─────────────────────────────────────────────────────────────────────
        // 7.5 — Layout version history
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Bounded version history of layout snapshots (default depth: 20).
        /// <see cref="PushLayoutVersion"/> is called automatically on significant
        /// layout changes when <see cref="TrackLayoutHistory"/> is <c>true</c>.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public BeepLayoutHistory LayoutHistory => _layoutHistory ??= new BeepLayoutHistory();

        private bool _trackLayoutHistory = true;
        /// <summary>
        /// When <c>true</c> (default) a layout snapshot is automatically pushed to
        /// <see cref="LayoutHistory"/> on every significant structural change
        /// (split, merge, document move).
        /// </summary>
        [DefaultValue(true)]
        [Description("Automatically record layout snapshots on structural changes.")]
        public bool TrackLayoutHistory
        {
            get => _trackLayoutHistory;
            set => _trackLayoutHistory = value;
        }

        /// <summary>
        /// Pushes the current layout state to <see cref="LayoutHistory"/>
        /// with an optional descriptive <paramref name="label"/>.
        /// </summary>
        public LayoutVersionEntry PushLayoutVersion(string? label = null) =>
            LayoutHistory.Push(SaveLayout(),
                label         : label,
                documentCount : _panels.Count,
                splitCount    : _groups.Count);

        // ─────────────────────────────────────────────────────────────────────
        // 7.7 — Layout undo/redo
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Multi-level undo/redo manager for layout changes (default depth: 50).
        /// Ctrl+Z and Ctrl+Y are wired automatically when
        /// <see cref="KeyboardShortcutsEnabled"/> is <c>true</c>.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public BeepLayoutUndoRedo UndoRedo => _undoRedo ??= new BeepLayoutUndoRedo();

        /// <summary>
        /// Saves the current layout onto the undo stack.
        /// Call this <em>before</em> applying a structural layout change.
        /// </summary>
        internal void PushUndoState()
        {
            _undoRedo ??= new BeepLayoutUndoRedo();
            _undoRedo.Push(SaveLayout());
        }

        internal void UndoLayout()
        {
            if (_undoRedo == null || !_undoRedo.CanUndo) return;
            var prev = _undoRedo.Undo(SaveLayout());
            if (prev != null) RestoreLayout(prev);
        }

        internal void RedoLayout()
        {
            if (_undoRedo == null || !_undoRedo.CanRedo) return;
            var next = _undoRedo.Redo();
            if (next != null) RestoreLayout(next);
        }

        // ─────────────────────────────────────────────────────────────────────
        // 7.3 — Cloud sync settings
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Configuration for cloud sync.  Assign and then call
        /// <see cref="ConfigureCloudSync"/> to activate.
        /// Defaults to <c>null</c> (sync disabled).
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public BeepCloudSyncSettings? CloudSyncSettings
        {
            get => _cloudSyncSettings;
            set => _cloudSyncSettings = value;
        }

        /// <summary>
        /// Activates cloud sync using <see cref="CloudSyncSettings"/>.
        /// Disposes any previously created <see cref="BeepCloudSyncManager"/>.
        /// Pass <c>null</c> to disable sync.
        /// </summary>
        public void ConfigureCloudSync(BeepCloudSyncSettings? settings)
        {
            _cloudSyncManager?.Dispose();
            _cloudSyncManager  = null;
            _cloudSyncSettings = settings;

            if (settings == null || settings.ProviderType == "None") return;

            var provider = BeepCloudSyncManager.CreateProvider(settings);
            _cloudSyncManager = new BeepCloudSyncManager(provider, Workspaces, settings);
        }

        /// <summary>
        /// Triggers an on-demand workspace sync to the configured cloud provider.
        /// Returns immediately when cloud sync is not configured.
        /// </summary>
        public async System.Threading.Tasks.Task SyncToCloudAsync(
            System.Threading.CancellationToken ct = default)
        {
            if (_cloudSyncManager != null)
                await _cloudSyncManager.SyncWorkspacesAsync(ct).ConfigureAwait(false);
        }

        // ─────────────────────────────────────────────────────────────────────
        // 6.x — Command registry
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Central command registry for this host.  Register custom commands or query/invoke
        /// built-in commands.  Access the palette interactively via Ctrl+Shift+P.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public BeepCommandRegistry CommandRegistry => EnsureCommandRegistry();

        internal BeepCommandRegistry EnsureCommandRegistry()
        {
            if (_commandRegistry != null) return _commandRegistry;
            _commandRegistry = new BeepCommandRegistry();
            RegisterBuiltinCommands(_commandRegistry);
            return _commandRegistry;
        }

        // ─────────────────────────────────────────────────────────────────────
        // 6.x — Breadcrumb navigation
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// When <see langword="true"/> a clickable breadcrumb bar is shown above the document
        /// content area (Workspace › DocumentTitle).  Toggle via Ctrl+K Ctrl+B.
        /// </summary>
        [DefaultValue(false)]
        [Description("Show a clickable breadcrumb navigation bar above the document content area.")]
        public bool ShowBreadcrumb
        {
            get => _showBreadcrumb;
            set
            {
                if (_showBreadcrumb == value) return;
                _showBreadcrumb = value;
                if (value) EnsureBreadcrumb();
                else if (_breadcrumb != null) _breadcrumb.Visible = false;
                RecalculateLayout();
            }
        }

        internal void EnsureBreadcrumb()
        {
            if (_breadcrumb != null) { _breadcrumb.Visible = true; return; }

            _breadcrumb = new BeepDocumentBreadcrumb();
            _breadcrumb.ApplyTheme(_currentTheme);
            Controls.Add(_breadcrumb);
            _breadcrumb.BringToFront();

            // Keep breadcrumb in sync with active document
            ActiveDocumentChanged += (_, _) => UpdateBreadcrumb();
        }

        internal void UpdateBreadcrumb()
        {
            if (_breadcrumb == null || !_showBreadcrumb) return;
            string? workspace = ActiveWorkspaceName;
            string? docTitle  = null;
            if (_activeDocumentId != null)
            {
                var tab = _tabStrip.Tabs.FirstOrDefault(t => t.Id == _activeDocumentId);
                docTitle = tab?.Title;
            }
            _breadcrumb.SetActiveDocument(workspace, null, docTitle);
        }

        // ─────────────────────────────────────────────────────────────────────
        // 6.x — Built-in command registration (30+ commands)
        // ─────────────────────────────────────────────────────────────────────

        private void RegisterBuiltinCommands(BeepCommandRegistry reg)
        {
            // ── Documents ─────────────────────────────────────────────────────
            reg.Register(new BeepCommandEntry
            {
                Id = "document.new", Title = "New Document", Category = "Documents",
                Shortcut = "Ctrl+N", Execute = () => CommandService.NewDocument()
            });
            reg.Register(new BeepCommandEntry
            {
                Id = "document.close", Title = "Close Document", Category = "Documents",
                Shortcut = "Ctrl+W",
                Execute  = () => { if (_activeDocumentId != null) CloseDocument(_activeDocumentId); },
                CanExecute = () => _activeDocumentId != null
            });
            reg.Register(new BeepCommandEntry
            {
                Id = "document.close.all", Title = "Close All Documents", Category = "Documents",
                Shortcut = "Ctrl+K Ctrl+W", Execute = () => CloseAllDocuments()
            });
            reg.Register(new BeepCommandEntry
            {
                Id = "document.reopen", Title = "Reopen Closed Document", Category = "Documents",
                Shortcut = "Ctrl+Shift+T", Execute = () => ReopenLastClosed()
            });
            reg.Register(new BeepCommandEntry
            {
                Id = "document.float", Title = "Float Document", Category = "Documents",
                Shortcut = "Ctrl+K Ctrl+F",
                Execute  = () => { if (_activeDocumentId != null) FloatDocument(_activeDocumentId); },
                CanExecute = () => _activeDocumentId != null
            });
            reg.Register(new BeepCommandEntry
            {
                Id = "document.pin", Title = "Pin / Unpin Document", Category = "Documents",
                Shortcut = "Ctrl+K Ctrl+P",
                Execute = () =>
                {
                    if (_activeDocumentId == null) return;
                    var tab = _tabStrip.Tabs.FirstOrDefault(t => t.Id == _activeDocumentId);
                    if (tab != null) PinDocument(_activeDocumentId, !tab.IsPinned);
                },
                CanExecute = () => _activeDocumentId != null
            });

            // ── Navigation ────────────────────────────────────────────────────
            reg.Register(new BeepCommandEntry
            {
                Id = "navigate.command.palette", Title = "Command Palette", Category = "Navigation",
                Shortcut = "Ctrl+Shift+P", Execute = () => ShowCommandPalette(CommandPaletteMode.Commands)
            });
            reg.Register(new BeepCommandEntry
            {
                Id = "navigate.goto.file", Title = "Go to File", Category = "Navigation",
                Shortcut = "Ctrl+P", Execute = () => ShowCommandPalette(CommandPaletteMode.GoToFile)
            });
            reg.Register(new BeepCommandEntry
            {
                Id = "breadcrumb.toggle", Title = "Toggle Breadcrumb", Category = "Navigation",
                Shortcut = "Ctrl+K Ctrl+B", Execute = () => ShowBreadcrumb = !ShowBreadcrumb
            });
            reg.Register(new BeepCommandEntry
            {
                Id = "navigate.workspace.switcher", Title = "Workspace Switcher", Category = "Navigation",
                Execute = () => ShowWorkspaceSwitcher()
            });
            reg.Register(new BeepCommandEntry
            {
                Id = "shortcut.help", Title = "Keyboard Shortcuts", Category = "Navigation",
                Shortcut = "Ctrl+K Ctrl+H", Execute = () => ShowShortcutHelp()
            });
            reg.Register(new BeepCommandEntry
            {
                Id = "navigate.workspace.save", Title = "Save Current Workspace", Category = "Navigation",
                Execute = () => SaveWorkspace(ActiveWorkspaceName ?? "Default")
            });

            // ── Tabs ──────────────────────────────────────────────────────────
            reg.Register(new BeepCommandEntry
            {
                Id = "tabs.next", Title = "Next Tab", Category = "Tabs",
                Shortcut = "Ctrl+Tab",
                Execute = () => _activeGroup.TabStrip.HandleShortcut(Keys.Control | Keys.Tab)
            });
            reg.Register(new BeepCommandEntry
            {
                Id = "tabs.prev", Title = "Previous Tab", Category = "Tabs",
                Shortcut = "Ctrl+Shift+Tab",
                Execute = () => _activeGroup.TabStrip.HandleShortcut(Keys.Control | Keys.Shift | Keys.Tab)
            });
            reg.Register(new BeepCommandEntry
            {
                Id = "tabs.cycle.position", Title = "Cycle Tab Position", Category = "Tabs",
                Shortcut = "Ctrl+K Ctrl+T",
                Execute = () => TabPosition = (TabStripPosition)(((int)TabPosition + 1) % 5)
            });
            reg.Register(new BeepCommandEntry
            {
                Id = "tabs.cycle.style", Title = "Cycle Tab Style", Category = "Tabs",
                Shortcut = "Ctrl+K Ctrl+S",
                Execute = () => TabStyle = (DocumentTabStyle)(((int)TabStyle + 1) % System.Enum.GetValues<DocumentTabStyle>().Length)
            });
            for (int n = 1; n <= 9; n++)
            {
                int captured = n;
                reg.Register(new BeepCommandEntry
                {
                    Id       = $"tabs.jump.{captured}",
                    Title    = $"Jump to Tab {captured}",
                    Category = "Tabs",
                    Shortcut = $"Ctrl+{captured}",
                    Execute  = () => _activeGroup.TabStrip.HandleShortcut(Keys.Control | (Keys.D0 + captured))
                });
            }

            // ── View ──────────────────────────────────────────────────────────
            reg.Register(new BeepCommandEntry
            {
                Id = "view.fullscreen", Title = "Toggle Fullscreen", Category = "View",
                Shortcut = "F11", Execute = () => ToggleFullscreen()
            });
            reg.Register(new BeepCommandEntry
            {
                Id = "view.density.compact", Title = "Compact Density", Category = "View",
                Execute = () => _tabStrip.TabDensity = TabDensityMode.Compact
            });
            reg.Register(new BeepCommandEntry
            {
                Id = "view.density.comfortable", Title = "Comfortable Density", Category = "View",
                Execute = () => _tabStrip.TabDensity = TabDensityMode.Comfortable
            });
            reg.Register(new BeepCommandEntry
            {
                Id = "view.density.dense", Title = "Dense Density", Category = "View",
                Execute = () => _tabStrip.TabDensity = TabDensityMode.Dense
            });

            // ── Layout / Splits ───────────────────────────────────────────────
            reg.Register(new BeepCommandEntry
            {
                Id = "split.horizontal", Title = "Split View Horizontal", Category = "Layout",
                Shortcut   = "Ctrl+K Ctrl+\\",
                Execute    = () => { if (_activeDocumentId != null) SplitDocumentHorizontal(_activeDocumentId); },
                CanExecute = () => _activeDocumentId != null
            });
            reg.Register(new BeepCommandEntry
            {
                Id = "split.vertical", Title = "Split View Vertical", Category = "Layout",
                Shortcut   = "Ctrl+K Ctrl+|",
                Execute    = () => { if (_activeDocumentId != null) SplitDocumentVertical(_activeDocumentId); },
                CanExecute = () => _activeDocumentId != null
            });
            reg.Register(new BeepCommandEntry
            {
                Id = "split.focus.left", Title = "Focus Left Split", Category = "Layout",
                Shortcut = "Ctrl+K ←", Execute = () => FocusSplitGroup(-1)
            });
            reg.Register(new BeepCommandEntry
            {
                Id = "split.focus.right", Title = "Focus Right Split", Category = "Layout",
                Shortcut = "Ctrl+K →", Execute = () => FocusSplitGroup(+1)
            });
            reg.Register(new BeepCommandEntry
            {
                Id = "split.focus.up", Title = "Focus Upper Split", Category = "Layout",
                Shortcut = "Ctrl+K ↑", Execute = () => FocusSplitGroup(-1)
            });
            reg.Register(new BeepCommandEntry
            {
                Id = "split.focus.down", Title = "Focus Lower Split", Category = "Layout",
                Shortcut = "Ctrl+K ↓", Execute = () => FocusSplitGroup(+1)
            });

            // ── Workspace ─────────────────────────────────────────────────────
            reg.Register(new BeepCommandEntry
            {
                Id = "workspace.save", Title = "Save Workspace", Category = "Workspace",
                Execute = () => SaveWorkspace(ActiveWorkspaceName ?? "Default")
            });
            reg.Register(new BeepCommandEntry
            {
                Id = "workspace.switcher", Title = "Switch Workspace", Category = "Workspace",
                Execute = () => ShowWorkspaceSwitcher()
            });

            // ── Phase 7: Templates, History, Undo/Redo, Export/Import ─────────
            reg.Register(new BeepCommandEntry
            {
                Id = "template.pick", Title = "Apply Layout Template…", Category = "Templates",
                Execute = () => ShowTemplatePicker()
            });
            reg.Register(new BeepCommandEntry
            {
                Id = "layout.undo", Title = "Undo Layout Change", Category = "Layout",
                Shortcut   = "Ctrl+Z",
                Execute    = () => UndoLayout(),
                CanExecute = () => _undoRedo?.CanUndo == true
            });
            reg.Register(new BeepCommandEntry
            {
                Id = "layout.redo", Title = "Redo Layout Change", Category = "Layout",
                Shortcut   = "Ctrl+Y",
                Execute    = () => RedoLayout(),
                CanExecute = () => _undoRedo?.CanRedo == true
            });
            reg.Register(new BeepCommandEntry
            {
                Id = "workspace.export.json", Title = "Export Workspace to JSON…", Category = "Workspace",
                Execute = () => ExportWorkspaceToJsonFile()
            });
            reg.Register(new BeepCommandEntry
            {
                Id = "workspace.import.json", Title = "Import Workspace from JSON…", Category = "Workspace",
                Execute = () => ImportWorkspaceFromJsonFile()
            });
            reg.Register(new BeepCommandEntry
            {
                Id = "workspace.export.zip", Title = "Export All Workspaces to ZIP…", Category = "Workspace",
                Execute = () => ExportWorkspacesToZipFile()
            });
            reg.Register(new BeepCommandEntry
            {
                Id = "workspace.import.zip", Title = "Import Workspaces from ZIP…", Category = "Workspace",
                Execute = () => ImportWorkspacesFromZipFile()
            });
            reg.Register(new BeepCommandEntry
            {
                Id = "workspace.export.clipboard", Title = "Copy Workspace to Clipboard", Category = "Workspace",
                Execute    = () => { var ws = Workspaces.GetActive(); if (ws != null) BeepWorkspacePorter.CopyToClipboard(ws); },
                CanExecute = () => Workspaces.GetActive() != null
            });
            reg.Register(new BeepCommandEntry
            {
                Id = "workspace.import.clipboard", Title = "Paste Workspace from Clipboard", Category = "Workspace",
                Execute = () => ImportWorkspaceFromClipboard()
            });
            reg.Register(new BeepCommandEntry
            {
                Id = "layout.history.browse", Title = "Browse Layout History…", Category = "Layout",
                Execute = () => ShowLayoutHistoryPicker()
            });
            reg.Register(new BeepCommandEntry
            {
                Id = "layout.sync.cloud", Title = "Sync Workspaces to Cloud", Category = "Workspace",
                Execute    = async () => await SyncToCloudAsync().ConfigureAwait(false),
                CanExecute = () => _cloudSyncManager != null
            });
        }

        // ─────────────────────────────────────────────────────────────────────
        // Events
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>Raised when the active document changes.</summary>
        public event System.EventHandler<DocumentEventArgs>? ActiveDocumentChanged;

        /// <summary>Raised when the user clicks the (+) add button.</summary>
        public event System.EventHandler? NewDocumentRequested;

        /// <summary>
        /// Raised just before a document is closed (after the tab strip fires TabClosing).
        /// Set <see cref="TabClosingEventArgs.Cancel"/> = true to prevent the close.
        /// </summary>
        public event System.EventHandler<TabClosingEventArgs>? DocumentClosing;

        /// <summary>Raised after a document is closed and removed.</summary>
        public event System.EventHandler<DocumentEventArgs>? DocumentClosed;

        /// <summary>Raised after a document is floated into its own window.</summary>
        public event System.EventHandler<DocumentEventArgs>? DocumentFloated;

        /// <summary>Raised after a previously floated document is docked back into the host.</summary>
        public event System.EventHandler<DocumentEventArgs>? DocumentDocked;

        /// <summary>Raised when a document's pinned state changes (from context menu or code).</summary>
        public event System.EventHandler<DocumentEventArgs>? DocumentPinChanged;

        /// <summary>
        /// Raised when <see cref="BeepDocumentHost.ReopenLastClosed"/> is invoked and a closed
        /// document record is available.  The consumer should re-open the panel and may read
        /// <see cref="ClosedTabRecord.RestoreData"/> to restore previous state.
        /// </summary>
        public event System.EventHandler<ClosedTabRecord>? ReopenDocumentRequested;

        // ─────────────────────────────────────────────────────────────────────
        // MRU + Closed-history properties
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>Maximum number of document activations to remember in the MRU list.</summary>
        [DefaultValue(20)]
        [Description("Maximum number of recently-used documents tracked for MRU navigation.")]
        public int MaxRecentHistory
        {
            get => _maxRecentHistory;
            set => _maxRecentHistory = Math.Max(1, value);
        }

        /// <summary>Maximum number of closed documents kept for the Reopen Closed Tab feature.</summary>
        [DefaultValue(10)]
        [Description("Maximum number of recently-closed documents available for Ctrl+Shift+T.")]
        public int MaxClosedHistory
        {
            get => _maxClosedHistory;
            set => _maxClosedHistory = Math.Max(0, value);
        }

        /// <summary>Ordered list of document ids by most-recent activation (read-only snapshot).</summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public System.Collections.Generic.IReadOnlyList<string> RecentDocuments =>
            new System.Collections.Generic.List<string>(_mruList);

        /// <summary>True when there is at least one closed document available for reopen.</summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool CanReopenClosed => _closedStack.Count > 0;

        // ─────────────────────────────────────────────────────────────────────
        // Cross-host drag
        // ─────────────────────────────────────────────────────────────────────

        private bool _allowDragBetweenHosts = false;

        /// <summary>
        /// When true, a tab dragged past the edge of this host will transfer to another
        /// <see cref="BeepDocumentHost"/> located under the cursor (if one is registered).
        /// </summary>
        [DefaultValue(false)]
        [Description("Allow tabs to be dragged from this host to another BeepDocumentHost in the same process.")]
        public bool AllowDragBetweenHosts
        {
            get => _allowDragBetweenHosts;
            set => _allowDragBetweenHosts = value;
        }

        /// <summary>
        /// Raised before a document is detached and transferred to another host.
        /// Set <see cref="DocumentTransferEventArgs.Cancel"/> = true to block the transfer.
        /// </summary>
        public event System.EventHandler<DocumentTransferEventArgs>? DocumentDetaching;

        /// <summary>Raised after a document has been received from another host.</summary>
        public event System.EventHandler<DocumentEventArgs>? DocumentAttached;

        // ─────────────────────────────────────────────────────────────────────
        // Dock constraints (Feature 4.7)
        // ─────────────────────────────────────────────────────────────────────

        private DockConstraints _dockConstraints = DockConstraints.Unrestricted();

        /// <summary>
        /// Declarative restrictions on docking operations for this host.
        /// Assign a <see cref="DockConstraints"/> instance to disable specific zones,
        /// enforce minimum pane sizes, or block incoming cross-host transfers.
        /// </summary>
        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.DesignerSerializationVisibility(
            System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public DockConstraints DockConstraints
        {
            get => _dockConstraints;
            set => _dockConstraints = value ?? DockConstraints.Unrestricted();
        }

        // ─────────────────────────────────────────────────────────────────────
        // Document Groups — split view (feature 2.1)
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>Maximum number of simultaneous split groups (default 4).</summary>
        [DefaultValue(4)]
        [Description("Maximum number of side-by-side document groups allowed.")]
        public int MaxGroups
        {
            get => _maxGroups;
            set => _maxGroups = Math.Max(1, value);
        }

        /// <summary>
        /// When true (default), groups are split side-by-side horizontally.
        /// When false, groups are split one above the other.
        /// </summary>
        [DefaultValue(true)]
        [Description("True = side-by-side (horizontal) split; False = top/bottom (vertical) split.")]
        public bool SplitHorizontal
        {
            get => _splitHorizontal;
            set
            {
                if (_splitHorizontal == value) return;
                _splitHorizontal = value;
                if (_splitterBar != null)
                    _splitterBar.IsHorizontal = value;
                RecalculateLayout();
            }
        }

        /// <summary>
        /// Fraction (0.1 – 0.9) of the host's width (or height in vertical split)
        /// allocated to the first group.  Default 0.5 = equal split.
        /// </summary>
        [DefaultValue(0.5f)]
        [Description("Ratio of host space assigned to the first group (0.1 – 0.9).")]
        public float SplitRatio
        {
            get => _splitRatio;
            set
            {
                _splitRatio = Math.Max(0.1f, Math.Min(0.9f, value));
                if (_groups.Count > 1) RecalculateLayout();
            }
        }

        /// <summary>Live snapshot of all active groups (read-only).</summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public System.Collections.Generic.IReadOnlyList<BeepDocumentGroup> Groups
            => _groups.AsReadOnly();

        // ─────────────────────────────────────────────────────────────────────
        // Tab-strip pass-through properties
        // These let the Properties grid and smart-tag control the inner strip
        // without requiring consumers to cast or drill into internals.
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>How tab widths are calculated: Equal, FitToContent, Compact, or Fixed.</summary>
        [DefaultValue(TabSizeMode.Equal)]
        [Description("How tab widths are calculated: Equal, FitToContent, Compact, or Fixed.")]
        public TabSizeMode TabSizeMode
        {
            get => _tabStrip.TabSizeMode;
            set { _tabStrip.TabSizeMode = value; }
        }

        /// <summary>Width of each tab in pixels when <see cref="TabSizeMode"/> = Fixed.</summary>
        [DefaultValue(160)]
        [Description("Fixed width per tab in pixels (applies when TabSizeMode = Fixed).")]
        public int FixedTabWidth
        {
            get => _tabStrip.FixedTabWidth;
            set { _tabStrip.FixedTabWidth = value; }
        }

        /// <summary>Controls the tooltip style on tab hover: None, Simple, or Rich (thumbnail).</summary>
        [DefaultValue(TabTooltipMode.Simple)]
        [Description("Tooltip style on tab hover: None = off, Simple = title, Rich = thumbnail panel.")]
        public TabTooltipMode TabTooltipMode
        {
            get => _tabStrip.TooltipMode;
            set { _tabStrip.TooltipMode = value; }
        }

        /// <summary>Allow the user to drag a tab out of the strip to float it in a new window.</summary>
        [DefaultValue(true)]
        [Description("Allow the user to drag a tab out of the strip to open it in a floating window.")]
        public bool AllowDragFloat
        {
            get => _tabStrip.AllowDragFloat;
            set { _tabStrip.AllowDragFloat = value; }
        }

        // ─────────────────────────────────────────────────────────────────────
        // Phase 8 — Power User Feature Properties
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Lazily-created side-by-side diff viewer panel.
        /// Attach to any split group or floating window for document comparison.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Features.DiffViewPanel DiffViewer
            => _diffViewer ??= new Features.DiffViewPanel();

        /// <summary>Opens a side-by-side diff view for two text snippets in a new document tab.</summary>
        public void ShowDiff(string textA, string textB, string titleA = "A", string titleB = "B")
        {
            var viewer = DiffViewer;
            viewer.SetDocuments(textA, textB);
            viewer.AccessibleName = $"Diff: {titleA} \u2194 {titleB}";

            if (!viewer.IsHandleCreated)
            {
                var docId = $"__diff_{System.Guid.NewGuid():N}";
                AddDocument(docId, $"Diff \u2014 {titleA} / {titleB}", string.Empty, activate: true);
                var panel = _panels[docId];
                viewer.Dock = System.Windows.Forms.DockStyle.Fill;
                panel.Controls.Add(viewer);
            }
            else
            {
                viewer.Invalidate();
            }
        }

        /// <summary>
        /// Lazily-created Git status provider.  Set
        /// <see cref="Features.BeepGitStatusProvider.RepositoryRoot"/> and call
        /// <see cref="Features.BeepGitStatusProvider.Start"/> before querying.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Features.BeepGitStatusProvider GitStatusProvider
            => _gitStatus ??= new Features.BeepGitStatusProvider();

        /// <summary>
        /// Lazily-created embedded terminal panel.
        /// Use <see cref="OpenTerminal"/> to show it in a tab, or dock it manually.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Features.TerminalPanel Terminal
            => _terminal ??= new Features.TerminalPanel();

        /// <summary>Opens the embedded terminal in a dedicated document tab.</summary>
        public void OpenTerminal()
        {
            const string TerminalDocId = "__terminal__";
            if (_panels.ContainsKey(TerminalDocId))
            {
                SetActiveDocument(TerminalDocId);
                return;
            }

            var panel = AddDocument(TerminalDocId, "Terminal", string.Empty, activate: true);
            var term  = Terminal;
            term.Dock = System.Windows.Forms.DockStyle.Fill;
            panel.Controls.Add(term);
            term.Start();
        }

        /// <summary>
        /// Lazily-created rich status bar.  Visibility is controlled by
        /// <see cref="ShowStatusBar"/>.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Features.BeepDocumentStatusBar StatusBar
            => _statusBar ??= CreateStatusBar();

        private Features.BeepDocumentStatusBar CreateStatusBar()
        {
            var bar = new Features.BeepDocumentStatusBar();
            Controls.Add(bar);
            bar.BringToFront();
            return bar;
        }

        /// <summary>
        /// Show or hide the rich status bar along the bottom edge of the host.
        /// Default is <see langword="true"/>.
        /// </summary>
        [DefaultValue(true)]
        [Description("Show the rich status bar along the bottom edge of the document host.")]
        public bool ShowStatusBar
        {
            get => _showStatusBar;
            set
            {
                _showStatusBar = value;
                StatusBar.Visible = value;
            }
        }

        /// <summary>
        /// Lazily-created floating mini toolbar.  Call
        /// <see cref="Features.BeepDocumentMiniToolbar.ShowAt"/> to position it
        /// beside a document panel.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Features.BeepDocumentMiniToolbar MiniToolbar
            => _miniToolbar ??= new Features.BeepDocumentMiniToolbar(
                   System.Array.Empty<Features.MiniToolbarAction>());

        // ─────────────────────────────────────────────────────────────────────
        // Design-Time Documents collection
        // Seeded at design time; applied to the host in OnHandleCreated.
        // Serialised into InitializeComponent by the designer.
        // ─────────────────────────────────────────────────────────────────────

        private readonly System.Collections.ObjectModel.Collection<DocumentDescriptor>
            _designTimeDocuments = new System.Collections.ObjectModel.Collection<DocumentDescriptor>();

        /// <summary>
        /// Documents to open automatically when the host is first created.
        /// Configure at design time in the Properties grid or via the
        /// "Edit Design-Time Documents…" smart-tag action.
        /// At runtime, if <see cref="AutoSaveLayout"/> is true and a session
        /// file exists, the restored layout takes precedence over this collection.
        /// </summary>
        [Description("Documents opened when the host is first created. Configure at design time.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public System.Collections.ObjectModel.Collection<DocumentDescriptor> DesignTimeDocuments
            => _designTimeDocuments;

        /// <summary>
        /// Applies any entries in <see cref="DesignTimeDocuments"/> that are not already open.
        /// Called automatically from <c>OnHandleCreated</c>; safe to call again manually.
        /// </summary>
        public void ApplyDesignTimeDocuments()
        {
            foreach (var desc in _designTimeDocuments)
            {
                if (string.IsNullOrEmpty(desc.Id) || _panels.ContainsKey(desc.Id)) continue;

                var panel = AddDocument(desc.Id, desc.Title, desc.IconPath, activate: false);

                if (desc.IsPinned)    PinDocument(desc.Id, true);
                if (desc.IsModified)  panel.IsModified = true;
                if (desc.CanClose == false) panel.CanClose = false;

                // Seed placeholder content based on InitialContent
                switch (desc.InitialContent)
                {
                    case DocumentInitialContent.Label:
                        var lbl = new System.Windows.Forms.Label
                        {
                            Text      = desc.Title,
                            Dock      = System.Windows.Forms.DockStyle.Fill,
                            TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                            Font      = new System.Drawing.Font("Segoe UI", 11f)
                        };
                        panel.Controls.Add(lbl);
                        break;
                    case DocumentInitialContent.RichTextBox:
                        panel.Controls.Add(new System.Windows.Forms.RichTextBox
                            { Dock = System.Windows.Forms.DockStyle.Fill, BorderStyle = System.Windows.Forms.BorderStyle.None });
                        break;
                }
            }

            // Activate the first document if nothing is active yet
            if (_activeDocumentId == null && _panels.Count > 0)
                SetActiveDocument(_panels.Keys.First());
        }
    }
}
