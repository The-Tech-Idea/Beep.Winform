using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Converters;
using TheTechIdea.Beep.Winform.Controls.Docking.Layout;
using TheTechIdea.Beep.Winform.Controls.Docking.Models;
using TheTechIdea.Beep.Winform.Controls.Docking.Painters;
using TheTechIdea.Beep.Winform.Controls.Docking.Runtime;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;

namespace TheTechIdea.Beep.Winform.Controls.Docking
{
    /// <summary>
    /// Core orchestrator for the WinForms-control docking system.
    ///
    /// Responsibilities:
    /// - Owns the panel registry and layout tree
    /// - Hosts dock panels as WinForms child controls on the host form
    /// - Provides high-level API for panel operations (add, remove, activate)
    /// - Integrates with BeepThemesManager for live theme switching
    /// - Orchestrates rendering and layout updates
    ///
    /// Design-time usage: Add manager to form, then add DockPanel components,
    /// and set their 'Manager' property. Panels auto-register at design time.
    /// </summary>
    [ToolboxItem(true)]
    [DesignTimeVisible(true)]
    [DesignerCategory("code")]
    [DefaultEvent(nameof(PageCloseRequest))]
    [DefaultProperty(nameof(Strings))]
    [Description("Docking management component.")]
    [Designer("TheTechIdea.Beep.Winform.Controls.Design.Server.Docking.Designers.BeepDockingManagerDesigner, TheTechIdea.Beep.Winform.Controls.Design.Server")]
    public partial class BeepDockingManager : IComponent, IDisposable, Runtime.DragDrop.IDockDragHost
    {
        private Form _hostForm;
        private EventHandler _hostLayoutChangedHandler;

        /// <summary>
        /// True when running inside the Visual Studio designer (no Win32 ops allowed).
        /// Uses the canonical LicenseManager approach, not DesignMode, so it works
        /// even before a Site is assigned — matching Krypton's pattern.
        /// </summary>
        private static bool IsInDesigner =>
            LicenseManager.UsageMode == LicenseUsageMode.Designtime ||
            DockingHelpers.IsWinFormsDesignerProcess();

        private bool IsDesignHosted =>
            IsInDesigner || _site?.DesignMode == true || _hostForm?.Site?.DesignMode == true;
        private DockLayoutTree _layoutTree = new DockLayoutTree();
        private DockingLayoutController _layoutController;
        private IDockingPainter _painter = NullDockingPainter.Instance;
        private TabInteractionHandler _tabHandler;
        private Dictionary<string, DockPanel> _panelsByKey = new Dictionary<string, DockPanel>();
        // Panels that have been closed but not yet permanently removed — they can be reopened.
        // Key = panel key, Value = panel snapshot with its last known state preserved.
        private Dictionary<string, DockPanel> _closedPanels = new Dictionary<string, DockPanel>();
        // One AutoHideStrip per edge — created in ManageControl, keyed by DockPosition.
        private Dictionary<DockPosition, AutoHideStrip> _autoHideStrips = new Dictionary<DockPosition, AutoHideStrip>();
        // Shared guide overlay for drag-to-dock — created lazily on first FloatPanel() call.
        private DockingGuideOverlay _guideOverlay;
        // Float windows keyed by panel key — used to tear down chrome when re-docking or disposing.
        private Dictionary<string, FloatWindow> _floatWindowsByKey = new Dictionary<string, FloatWindow>(StringComparer.Ordinal);
        // Per-edge-group splitters — keyed by DockGroup.Id. Reconciled (created/positioned/disposed)
        // exclusively by SyncSplitters() from the layout engine's DockLayoutResult.
        private Dictionary<string, BeepDockSplitter> _splitters = new Dictionary<string, BeepDockSplitter>();
        private bool _disposed = false;
        private bool _subscribedToThemeChanged = false;
        private bool _useThemeColors = true;
        private bool _allowEndUserDocking = true;
        private bool _activeAutoHideContent = true;
        private bool _showSnapGuides = true;
        private Size _defaultFloatWindowSize = Size.Empty;
        private string _themeName = string.Empty;
        private IBeepTheme _currentTheme;
        private DockingThemeColors _themeColors = DockingThemeColors.Default;
        private BeepControlStyle _style = BeepControlStyle.Material3;
        private Models.TabStyle _tabStyle = Models.TabStyle.Default;

        // MRU-ordered panel keys (head = most recently activated).
        private readonly LinkedList<string> _mruList = new LinkedList<string>();
        private KeyEventHandler _hostKeyDownHandler;
        private KeyEventHandler _hostKeyUpHandler;
        private BeepDockingNavigator _navigator;

        // Batch-update nesting counter — layout is suspended while > 0.
        private int _updateDepth;

        // IComponent implementation
        private ISite _site;
        private EventHandler _disposed_event;

        /// <summary>
        /// Raised when a panel is activated (becomes the active panel in its group).
        /// </summary>
        /// <summary>
        /// Raised when the manager absorbed a failure instead of propagating it.
        /// </summary>
        /// <remarks>
        /// Layout restore is best-effort by design - one panel that cannot be floated must not
        /// abort the whole restore. Subscribe to see which part failed; without this the layout
        /// simply comes back subtly wrong and nothing says why.
        /// </remarks>
        public event EventHandler<DockingErrorEventArgs> DockingError;

        /// <summary>Reports an absorbed failure. Never throws from the reporting path itself.</summary>
        protected virtual void OnDockingError(string context, string panelKey, Exception exception)
        {
            if (exception == null) return;
            System.Diagnostics.Trace.WriteLine($"BeepDockingManager [{context}] {panelKey}: {exception}");
            DockingError?.Invoke(this, new DockingErrorEventArgs(context, panelKey, exception));
        }

        public event EventHandler<DockPanel> PanelActivated;

        /// <summary>
        /// Raised when a panel is added to the manager.
        /// </summary>
        public event EventHandler<DockPanel> PanelAdded;

        /// <summary>
        /// Raised when a panel is removed from the manager.
        /// </summary>
        public event EventHandler<DockPanel> PanelRemoved;

        /// <summary>
        /// Raised when the active theme changes and repainting is needed.
        /// </summary>
        public event EventHandler ThemeChanged;

        /// <summary>Raised when a panel transitions to Floating state.</summary>
        public event EventHandler<DockPanel> PanelFloated;

        /// <summary>Raised when a panel transitions to AutoHidden state.</summary>
        public event EventHandler<DockPanel> PanelAutoHidden;

        /// <summary>Raised when a panel is hidden (collapsed without being closed).</summary>
        public event EventHandler<DockPanel> PanelHidden;

        /// <summary>Raised when a hidden panel becomes visible again.</summary>
        public event EventHandler<DockPanel> PanelShown;

        /// <summary>Raised when a panel is closed (moved to the closed store).</summary>
        public event EventHandler<DockPanel> PanelClosed;

        /// <summary>Raised when a previously closed panel is reopened.</summary>
        public event EventHandler<DockPanel> PanelReopened;

        /// <summary>
        /// Raised when a docked panel moves from one <see cref="DockGroup"/> to another (e.g.,
        /// via <see cref="StackPanel(string, string)"/>, <see cref="MovePanelInStack(string,int)"/>,
        /// or a tab-drag commit). Provides the old/new group and the new tab index. Mirrors
        /// DockPanelSuite's <c>DockContent.DockChanged</c> notification.
        /// </summary>
        public event EventHandler<PanelMovedBetweenGroupsEventArgs> PanelMovedBetweenGroups;

        /// <summary>
        /// Raised before the manager mutates a panel's <see cref="DockPanelState"/>. Subscribers
        /// may set <see cref="System.ComponentModel.CancelEventArgs.Cancel"/> to veto the change.
        /// The manager only raises this for programmatic API calls (float / dock / auto-hide /
        /// hide / close / reopen). Tab-drag commits raise it too. Mirrors Krypton's
        /// <c>PageFlags</c>-style veto hook.
        /// </summary>
        public event EventHandler<PanelStateChangingEventArgs> PanelStateChanging;

        // ── Cancel-able request events (mirrors Krypton User-Request category) ──────

        /// <summary>
        /// Raised before a panel close is executed.  Set
        /// <see cref="CancelPanelRequestEventArgs.Cancel"/> to <c>true</c> to prevent it.
        /// </summary>
        public event EventHandler<PanelCloseRequestEventArgs> PageCloseRequest;

        /// <summary>
        /// Raised when the user requests a panel be docked.
        /// Set <see cref="CancelPanelRequestEventArgs.Cancel"/> to prevent the transition.
        /// </summary>
        public event EventHandler<CancelPanelRequestEventArgs> PageDockedRequest;

        /// <summary>
        /// Raised when the user requests a panel be auto-hidden.
        /// Set <see cref="CancelPanelRequestEventArgs.Cancel"/> to prevent the transition.
        /// </summary>
        public event EventHandler<CancelPanelRequestEventArgs> PageAutoHiddenRequest;

        /// <summary>
        /// Raised when the user requests a panel be floated.
        /// Set <see cref="CancelPanelRequestEventArgs.Cancel"/> to prevent the transition.
        /// </summary>
        public event EventHandler<CancelPanelRequestEventArgs> PageFloatingRequest;

        // ── Context-menu event ───────────────────────────────────────────────────────

        /// <summary>
        /// Raised when a docking context menu is about to be shown for a panel.
        /// Populate <see cref="PanelContextMenuEventArgs.ContextMenu"/> to override the built-in menu.
        /// </summary>
        public event EventHandler<PanelContextMenuEventArgs> ShowPanelContextMenu;

        // ── Lifecycle events (mirrors Krypton Control-Adding/Removed category) ────────

        /// <summary>Raised when a float window is being created for a panel.</summary>
        public event EventHandler<FloatingWindowEventArgs> FloatingWindowAdding;

        /// <summary>Raised when a float window is being destroyed.</summary>
        public event EventHandler<FloatingWindowEventArgs> FloatingWindowRemoved;

        /// <summary>Raised when a panel is being added to an auto-hide strip.</summary>
        public event EventHandler<AutoHiddenGroupEventArgs> AutoHiddenGroupAdding;

        /// <summary>Raised when a panel is being removed from an auto-hide strip.</summary>
        public event EventHandler<AutoHiddenGroupEventArgs> AutoHiddenGroupRemoved;

        /// <summary>Raised when a docked panel area (dockspace) is being created.</summary>
        public event EventHandler<DockspaceEventArgs> DockspaceAdding;

        /// <summary>Raised when a docked panel area (dockspace) is being removed.</summary>
        public event EventHandler<DockspaceEventArgs> DockspaceRemoved;

        // ── Resize events (mirrors Krypton Control-Resizing category) ─────────────────

        /// <summary>Raised when a splitter separator between docked panels is moved.</summary>
        public event EventHandler<SeparatorResizeEventArgs> DockspaceSeparatorResize;

        /// <summary>Raised when the auto-hide slide panel separator is moved.</summary>
        public event EventHandler<SeparatorResizeEventArgs> AutoHiddenSeparatorResize;

        // ── Drag-drop events (mirrors Krypton Docking category) ───────────────────────

        /// <summary>Raised after a successful drag-drop re-dock operation.</summary>
        public event EventHandler DoDragDropEnd;

        /// <summary>Raised when a drag-drop re-dock operation is cancelled by the user.</summary>
        public event EventHandler DoDragDropQuit;

        // ── Persistence events (mirrors Krypton Persistence category) ─────────────────

        /// <summary>
        /// Gets access to the set of display strings used by the docking hierarchy.
        /// Replace individual properties to localise the UI without touching runtime logic.
        /// Mirrors Krypton's <c>DockingManagerStrings</c>.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public BeepDockingStrings Strings { get; } = new BeepDockingStrings();

        private Models.DockingOptions _options;
        private Runtime.DockingFocusManager _focusManager;

        /// <summary>
        /// Grouped options for the manager. Mirrors Krypton's <c>DockingOptions</c>. Bind a
        /// <see cref="System.Windows.Forms.PropertyGrid"/> to this single property to expose all
        /// of the manager's behavior knobs under one expandable category. Assigning to a property
        /// on the bag is equivalent to assigning the same-named property on the manager itself.
        /// </summary>
        [Browsable(true)]
        [Category("Docking")]
        [Description("Grouped behavior options for the docking manager (AllowEndUserDocking, " +
                     "ActiveAutoHideContent, ShowSnapGuides, DefaultFloatWindowSize).")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Models.DockingOptions Options
        {
            get
            {
                if (_options == null)
                    _options = new Models.DockingOptions(this);
                return _options;
            }
        }

        /// <summary>
        /// Centralized focus routing for docking surfaces. Replace the default instance to
        /// implement a custom focus policy (e.g., skip focusing text boxes, focus a specific
        /// child control, suppress focus on activation). Default behavior brings the panel to
        /// the front and focuses its first focusable descendant.
        /// </summary>
        [Browsable(false)]
        public Runtime.DockingFocusManager FocusManager
        {
            get
            {
                if (_focusManager == null)
                    _focusManager = new Runtime.DockingFocusManager(this);
                return _focusManager;
            }
            set => _focusManager = value;
        }

        /// <summary>
        /// Gets or sets the Beep theme name used by the docking chrome.
        /// The manager follows global theme changes at runtime unless hosted in the designer.
        /// </summary>
        [Browsable(true)]
        [Category("Appearance")]
        [TypeConverter(typeof(ThemeEnumConverter))]
        [Description("The Beep theme used to draw dockspace headers, tabs, panels, splitters, and auto-hide chrome.")]
        public string Theme
        {
            get => _themeName;
            set
            {
                string nextTheme = value ?? string.Empty;
                if (string.Equals(_themeName, nextTheme, StringComparison.Ordinal))
                    return;

                _themeName = nextTheme;
                _currentTheme = BeepThemesManager.GetTheme(nextTheme) ?? BeepThemesManager.GetDefaultTheme();
                ApplyTheme();
            }
        }

        /// <summary>
        /// When true, docking chrome reads colours from the selected Beep theme.
        /// When false, the docking fallback palette is used.
        /// </summary>
        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(true)]
        [Description("Use Beep theme colors for dockspace headers, tabs, panels, splitters, and auto-hide chrome.")]
        public bool UseThemeColors
        {
            get => _useThemeColors;
            set
            {
                if (_useThemeColors == value)
                    return;

                _useThemeColors = value;
                ApplyTheme();
            }
        }

        /// <summary>
        /// When false, drag-to-float, drag-to-dock, tab reordering, splitter drags, auto-hide
        /// drags, and float-window drops are all suppressed. Mirrors
        /// <c>DockPanelExt.AllowEndUserDocking</c> in DockPanelSuite. Useful for "viewer" or
        /// "kiosk" modes where the layout should stay frozen at design-time.
        /// </summary>
        [Category("Behavior")]
        [DefaultValue(true)]
        [Description("When false, suppresses all user-initiated docking interactions: tab drag-to-float, " +
                     "splitter drag, tab reorder, and float-window dock-to-edge drops.")]
        public bool AllowEndUserDocking
        {
            get => _allowEndUserDocking;
            set
            {
                if (_allowEndUserDocking == value) return;
                _allowEndUserDocking = value;
                if (!value)
                {
                    // Cancel any in-flight drag so toggling mid-drag doesn't strand ghost state.
                    if (_dragController != null && _dragController.IsDragging)
                        _dragController.Cancel();
                    if (_hostForm != null && _hostForm.Capture)
                        _hostForm.Capture = false;
                }
            }
        }

        /// <summary>
        /// Default size (width, height in pixels) used when floating a panel whose
        /// <c>initialBounds</c> is empty and the panel's <see cref="DockPanel.PreferredWidth"/>
        /// / <see cref="DockPanel.PreferredHeight"/> are zero. Mirrors
        /// <c>DockPanelExt.DefaultFloatWindowSize</c> in DockPanelSuite. Set to
        /// <see cref="Size.Empty"/> (the default) to fall back to the
        /// panel's preferred size or the 320×240 fallback in <see cref="FloatWindow"/>.
        /// </summary>
        [Category("Layout")]
        [Description("Default size for new float windows when the panel has no preferred size and " +
                     "no explicit initial bounds.")]
        [DefaultValue(typeof(Size), "0,0")]
        public Size DefaultFloatWindowSize
        {
            get => _defaultFloatWindowSize;
            set => _defaultFloatWindowSize = value;
        }

        /// <summary>
        /// When true (default), the content control of an auto-hidden panel that has just slid
        /// out receives keyboard focus. Mirrors Krypton's
        /// <c>DockGlobalContext.ActiveAutoHideContent</c>. Set to false to keep focus on whatever
        /// was focused before the slide-out (useful when the user is mid-edit in a docked panel
        /// and peeks at an auto-hidden one).
        /// </summary>
        [Category("Behavior")]
        [DefaultValue(true)]
        [Description("When true, focusing the content of an auto-hidden panel after its slide-in " +
                     "animation completes. Set false to keep focus on the previous control.")]
        public bool ActiveAutoHideContent
        {
            get => _activeAutoHideContent;
            set => _activeAutoHideContent = value;
        }

        /// <summary>
        /// When true (default), a thin accent snap-line is drawn over the host form during
        /// tab-drag to indicate where the dragged tab would insert (<c>GroupCenterStack</c>) or
        /// split (<c>GroupEdge</c>). Mirrors DockPanelSuite's <c>DockDragHandler</c> snap line.
        /// </summary>
        [Category("Behavior")]
        [DefaultValue(true)]
        [Description("When true, a snap-line indicator is drawn during tab-drag over group-edges " +
                     "and center-stack drop targets. Set false to suppress.")]
        public bool ShowSnapGuides
        {
            get => _showSnapGuides;
            set
            {
                if (_showSnapGuides == value) return;
                _showSnapGuides = value;
                if (_dragController != null)
                    _dragController.ShowSnapGuides = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="BeepControlStyle"/> that drives docking chrome
        /// background/border/shadow rendering. Propagates to every docking surface (captions,
        /// tabs, splitters, strips) so they match the chosen style.
        /// </summary>
        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(BeepControlStyle.Material3)]
        [Description("The control style used to render docking chrome (captions, tabs, splitters, strips).")]
        public BeepControlStyle Style
        {
            get => _style;
            set
            {
                if (_style == value)
                    return;

                _style = value;
                PropagateControlStyle();
            }
        }

        /// <summary>
        /// Gets or sets the visual style used to paint tab headers across all docking surfaces.
        /// Controls tab shape (pill, square, trapezoid), button appearance, and accent indicators.
        /// </summary>
        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(Models.TabStyle.Default)]
        [Description("Visual tab header style: Default, VsCode, VsIde2022, JetBrains, or Browser.")]
        public Models.TabStyle TabStyle
        {
            get => _tabStyle;
            set
            {
                if (_tabStyle == value) return;
                _tabStyle = value;
                PropagateControlStyle();
            }
        }

        /// <summary>
        /// Gets the resolved theme object currently used by the docking manager.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IBeepTheme CurrentTheme => _currentTheme;

        /// <summary>
        /// Gets the layout tree containing all groups and panels.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DockLayoutTree LayoutTree => _layoutTree;

        /// <summary>
        /// Gets or sets the design-time layout snapshot for persistence in Designer.
        /// This property is automatically serialized by the Visual Studio designer.
        /// </summary>
        /// <summary>
        /// Gets the layout controller for calculating panel positions.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DockingLayoutController LayoutController => _layoutController;

        /// <summary>
        /// Gets or sets the docking painter used for rendering.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IDockingPainter Painter
        {
            get => _painter;
            set
            {
                var incoming = value ?? NullDockingPainter.Instance;
                if (_painter != incoming)
                {
                    if (!ReferenceEquals(_painter, NullDockingPainter.Instance))
                        _painter.Dispose();
                    _painter = incoming;
                    _layoutController?.InvalidateLayout();
                }
            }
        }

        /// <summary>
        /// Gets the tab interaction handler for tab management.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public TabInteractionHandler TabHandler => _tabHandler;


        /// <summary>
        /// Gets the number of panels currently managed.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int PanelCount => _panelsByKey.Count;

        /// <summary>
        /// Gets or sets the host form.  Setting this property is equivalent to calling
        /// <see cref="ManageControl"/>.  Exposed so the designer property grid can wire the
        /// manager to its host form without hand-written code.
        /// Mirrors Krypton's component-level host attachment pattern.
        /// </summary>
        [Category("Docking")]
        [Description("The form that hosts this docking manager. Set once in form Load or via property grid.")]
        [Browsable(false)]
        [DefaultValue(null)]
        public Form HostForm
        {
            get => _hostForm;
            set
            {
                if (value != null)
                    ManageControl(value);
            }
        }

        /// <summary>
        /// IComponent.Site - Gets or sets the site that binds the component to its container.
        /// </summary>
        public ISite Site
        {
            get => _site;
            set
            {
                _site = value;
                if (IsDesignHosted)
                    UnsubscribeThemeChanged();
                else
                    TrySubscribeThemeChanged(false);
            }
        }

        /// <summary>
        /// IComponent.Disposed - Raised when the component is disposed.
        /// </summary>
        public event EventHandler Disposed
        {
            add => _disposed_event += value;
            remove => _disposed_event -= value;
        }
        /// <summary>
        /// Default (parameterless) constructor — required so the manager can be dropped from the
        /// toolbox onto a form in the WinForms designer.  Mirrors KryptonDockingManager().
        /// Non-Win32 subsystems are initialised here; Win32 MDI is deferred until
        /// <see cref="ManageControl"/> is called at runtime.
        /// </summary>
        public BeepDockingManager() => InitializeManager();

        /// <summary>
        /// Container constructor used by the WinForms designer when it creates tray components.
        /// Mirrors commercial component patterns such as KryptonDockingManager(IContainer).
        /// </summary>
        public BeepDockingManager(IContainer container) : this()
        {
            container?.Add(this);
        }

        /// <summary>
        /// Convenience constructor that initialises subsystems and immediately calls
        /// <see cref="ManageControl"/> so existing code that passes a form still compiles.
        /// </summary>
        /// <param name="hostForm">The form that will host the docking layout.</param>
        public BeepDockingManager(Form hostForm) : this()
        {
            ManageControl(hostForm ?? throw new ArgumentNullException(nameof(hostForm)));
        }

        /// <summary>
        /// Initializes lightweight manager state only. Mirrors Krypton's InitializeManager:
        /// no host handles, no child controls, no Win32 runtime objects.
        /// </summary>
        private void InitializeManager()
        {
            Strings.PropertyChanged += OnStringPropertyChanged;
            _currentTheme = IsDesignHosted
                ? BeepThemesManager.GetDefaultTheme()
                : (BeepThemesManager.CurrentTheme ?? BeepThemesManager.GetDefaultTheme());
            _themeName = BeepThemesManager.GetThemeName(_currentTheme);
            ApplyTheme();
            TrySubscribeThemeChanged(IsDesignHosted);
        }

        /// <summary>
        /// Initialises all non-Win32 subsystems (painter, layout, chrome, content hosting).
        /// Safe to call at design time and before a host form is available.
        /// Mirrors KryptonDockingManager.InitializeManager().
        /// </summary>
        private void InitializeSubsystems()
        {
            if (IsDesignHosted) return;

            if (ReferenceEquals(_painter, NullDockingPainter.Instance))
                _painter = DockingPainterFactory.GetPainter(_themeName) ?? NullDockingPainter.Instance;

            if (ReferenceEquals(_painter, NullDockingPainter.Instance))
                _painter = new DockingPainterAdapter();

            ApplyDockingThemeColors(_themeColors, updatePainter: true);
            _layoutController ??= new DockingLayoutController(_layoutTree, _painter);
            _tabHandler ??= new TabInteractionHandler(ActivatePanel, _layoutTree);
            Debug.WriteLine("[BeepDockingManager] Subsystems initialised.");
        }


        private void PreparePanelForDock(DockPanel panel)
        {
            if (panel == null)
                return;

            if (panel.State == DockPanelState.Floating)
                CloseFloatWindowFor(panel);
            else if (panel.State == DockPanelState.AutoHidden)
                DetachFromAutoHideStrip(panel);

            panel.ShowCaption = true;
            panel.Visible = true;
            EnsurePanelHosted(panel);
        }


        /// <summary>
        /// Returns the <see cref="BeepDockspace"/> on <paramref name="hostForm"/> whose
        /// <see cref="BeepDockspace.DockPosition"/> matches <paramref name="position"/> and is
        /// owned by this manager. Returns <c>null</c> if no matching dockspace exists.
        /// </summary>
        /// <remarks>
        /// Looks in <see cref="Control.Controls"/> for dockspaces already placed on the host form
        /// (designer-created ones, or added at runtime). Does not create a new dockspace — the
        /// caller decides what to do when none is found.
        /// </remarks>
        public BeepDockspace FindDockspaceAt(Form hostForm, DockPosition position)
        {
            if (hostForm == null)
                return null;

            foreach (Control child in hostForm.Controls)
            {
                if (child is BeepDockspace ds &&
                    ReferenceEquals(ds.Manager, this) &&
                    ds.DockPosition == position)
                {
                    return ds;
                }
            }
            return null;
        }

        /// <summary>
        /// Convenience for <see cref="FindDockspaceAt"/>: returns the matching dockspace, or
        /// <c>null</c> if none exists. Reserved name kept symmetric with the
        /// <c>FindOrCreateGroupAtPosition</c> helper used for the layout tree; creation is
        /// deliberately NOT done at runtime to avoid duplicating designer-owned controls — adding
        /// a dockspace at runtime is a designer concern.
        /// </summary>
        private BeepDockspace FindOrCreateDockspaceAt(Form hostForm, DockPosition position) =>
            FindDockspaceAt(hostForm, position);


        private static IEnumerable<DockPanel> EnumerateDockPanels(Control root)
        {
            foreach (Control child in root.Controls)
            {
                if (child is DockPanel panel)
                    yield return panel;

                foreach (var nested in EnumerateDockPanels(child))
                    yield return nested;
            }
        }


        private void SeedGroupAndDescendants(DockGroup group, Rectangle client)
        {
            if (group.RatioInitialized || group.Position == DockPosition.Fill)
                return;

            var panel = group.ActivePanel;
            if (panel == null || panel.State != DockPanelState.Docked)
                panel = group.Panels.FirstOrDefault(p => p.State == DockPanelState.Docked);
            if (panel == null)
                return;

            bool horizontal = group.Position == DockPosition.Left || group.Position == DockPosition.Right;
            int axis = horizontal ? client.Width : client.Height;
            int preferred = horizontal ? panel.PreferredWidth : panel.PreferredHeight;

            if (axis > 0 && preferred > 0)
                group.SplitRatio = preferred / (float)axis;

            group.RatioInitialized = true;

            foreach (var child in group.Children)
                SeedGroupAndDescendants(child, client);
        }


        /// <summary>
        /// Gets a panel by its key.
        /// </summary>
        public DockPanel GetPanel(string panelKey)
        {
            _panelsByKey.TryGetValue(panelKey, out var panel);
            return panel;
        }

        /// <summary>
        /// Moves a panel to another dock edge. Panels moved to an edge with existing
        /// panels join that edge stack, matching Krypton's dockspace/page model.
        /// </summary>
        public bool MovePanel(string panelKey, DockPosition position)
        {
            var panel = GetPanel(panelKey);
            if (panel == null)
                return false;

            if (panel.State == DockPanelState.Floating)
            {
                panel.DockPosition = position;
                DockFloatingPanel(panelKey, position);
                ActivatePanel(panelKey);
                return true;
            }

            if (panel.State == DockPanelState.AutoHidden)
            {
                if (panel.DockPosition == position)
                    return true;

                panel.DockPosition = position;
                DetachFromAutoHideStrip(panel);
                panel.State = DockPanelState.AutoHidden;

                if (_autoHideStrips.TryGetValue(position, out var strip))
                {
                    strip.Visible = true;
                    strip.AddPanel(panel);
                    OnAutoHiddenGroupAdding(new AutoHiddenGroupEventArgs(panel, position));
                }

                return true;
            }

            if (panel.State == DockPanelState.Hidden)
            {
                panel.DockPosition = position;
                return true;
            }

            panel.DockPosition = position;
            ActivatePanel(panelKey);
            return true;
        }

        /// <summary>
        /// Stacks one panel with another panel by placing both in the same dock group.
        /// </summary>
        public bool StackPanel(string panelKey, string targetPanelKey)
        {
            if (string.IsNullOrWhiteSpace(panelKey) || string.IsNullOrWhiteSpace(targetPanelKey))
                return false;

            if (string.Equals(panelKey, targetPanelKey, StringComparison.Ordinal))
                return false;

            var panel = GetPanel(panelKey);
            var target = GetPanel(targetPanelKey);
            if (panel == null || target == null)
                return false;

            panel.DockPosition = target.DockPosition;

            var targetGroup = target.Group ?? GetOrCreateGroupAtPosition(target.DockPosition);
            var oldGroup = panel.Group;
            if (oldGroup != null && oldGroup != targetGroup)
                oldGroup.RemovePanel(panel);

            PreparePanelForDock(panel);
            panel.State = DockPanelState.Docked;

            targetGroup.AddPanel(panel);
            targetGroup.ActivePanel = panel;

            if (oldGroup != targetGroup)
            {
                int newIndex = targetGroup.Panels.ToList().IndexOf(panel);
                OnPanelMovedBetweenGroups(
                    new PanelMovedBetweenGroupsEventArgs(panel, oldGroup, targetGroup, newIndex));
            }

            _layoutController?.InvalidateLayout();
            RecalculateLayout();
            return true;
        }

        /// <summary>
        /// Moves a panel within its current tab stack.
        /// </summary>
        public bool MovePanelInStack(string panelKey, int newIndex)
        {
            var panel = GetPanel(panelKey);
            if (panel?.Group == null)
                return false;

            panel.Group.MovePanelToIndex(panel, newIndex);
            panel.Group.ActivePanel = panel;
            panel.BringToFront();

            _layoutController?.InvalidateLayout();
            RecalculateLayout();
            return true;
        }

        /// <summary>
        /// Gets the panels in the same stack as the named panel.
        /// </summary>
        public IReadOnlyList<DockPanel> GetStackedPanels(string panelKey)
        {
            var panel = GetPanel(panelKey);
            return panel?.Group?.Panels ?? Array.Empty<DockPanel>();
        }

        /// <summary>
        /// Activates a panel (makes it the active panel in its group).
        /// Returns false if the panel is not found.
        /// </summary>
        public bool ActivatePanel(string panelKey)
        {
            var panel = GetPanel(panelKey);
            if (panel == null)
                return false;

            if (panel.Group != null)
            {
                var oldActive = panel.Group.ActivePanel;
                panel.Group.ActivePanel = panel;

                if (oldActive != panel)
                    OnPanelActivated(panel);

                foreach (var groupedPanel in panel.Group.Panels)
                    groupedPanel.Invalidate();
            }

            if (panel.Parent != null)
                panel.BringToFront();

            // Notify the parent dockspace so its header repaints the active tab.
            if (panel.Parent is BeepDockspace dockspace)
                dockspace.ActivePanelKey = panelKey;

            // Route through the focus manager so hosts can swap in a custom focus policy.
            FocusManager.Focus(panel);

            PushMrPanel(panelKey);

            Debug.WriteLine($"[BeepDockingManager] Panel activated: {panelKey}");
            return true;
        }

        /// <summary>
        /// Activates the next panel in MRU order (for Ctrl+Tab programmatic use).
        /// Returns null if there is no panel to activate.
        /// </summary>
        public DockPanel ActivateNextPanel()
        {
            string key = GetNextMrPanel(forward: true);
            if (key == null) return null;
            PushMrPanel(key);
            ActivatePanel(key);
            return GetPanel(key);
        }

        /// <summary>
        /// Activates the previous panel in MRU order (for Ctrl+Shift+Tab programmatic use).
        /// Returns null if there is no panel to activate.
        /// </summary>
        public DockPanel ActivatePreviousPanel()
        {
            string key = GetNextMrPanel(forward: false);
            if (key == null) return null;
            PushMrPanel(key);
            ActivatePanel(key);
            return GetPanel(key);
        }

        /// <summary>
        /// Moves the currently active panel within its tab stack by <paramref name="delta"/>
        /// positions (negative = left/toward first, positive = right/toward last).
        /// Returns true on success, false if there is no active panel, no neighbors, or the
        /// panel is not in a stack (single-panel dockspace has nothing to swap with).
        /// </summary>
        public bool MoveActivePanel(int delta)
        {
            if (delta == 0) return false;
            string key = GetActivePanelKey();
            if (string.IsNullOrEmpty(key)) return false;
            var panel = GetPanel(key);
            if (panel?.Group == null) return false;

            int currentIndex = panel.Group.GetPanelIndex(panel);
            if (currentIndex < 0) return false;
            int newIndex = currentIndex + delta;
            if (newIndex < 0 || newIndex >= panel.Group.Panels.Count) return false;
            if (newIndex == currentIndex) return false;

            return MovePanelInStack(panel.Key, newIndex);
        }

        /// <summary>
        /// Gets all panels currently managed.
        /// </summary>
        public IReadOnlyList<DockPanel> GetAllPanels()
        {
            return _panelsByKey.Values.ToList().AsReadOnly();
        }

        /// <summary>
        /// Gets all panels at a specific dock position.
        /// </summary>
        public List<DockPanel> GetPanelsAtPosition(DockPosition position)
        {
            return _layoutTree.GetPanelsAtPosition(position);
        }

        /// <summary>
        /// Recalculates layout for all panels based on the host form's current client area.
        /// Call this after panel add/remove or host resize.
        /// No-ops while a <see cref="BeepDockingUpdate"/> batch scope is active.
        /// </summary>
        public void RecalculateLayout()
        {
            if (_updateDepth > 0) return;
            if (_hostForm == null) return;

            // Single authoritative pass: engine computes bounds + splitter rects, manager applies.
            ApplyLayout();
        }

        /// <summary>
        /// Gets the calculated bounds for a specific panel.
        /// </summary>
        public Rectangle? GetPanelBounds(string panelKey)
        {
            return _layoutController.GetPanelBounds(panelKey);
        }

        /// <summary>
        /// Gets the calculated content bounds for a specific panel (excluding chrome).
        /// </summary>
        public Rectangle? GetPanelContentBounds(string panelKey)
        {
            return _layoutController.GetPanelContentBounds(panelKey);
        }

        /// <summary>
        /// Shows a panel that was previously hidden (sets State to Docked and makes it visible).
        /// Mirrors DockContent.Show() / Krypton ShowPage().
        /// </summary>
        public void ShowPanel(string panelKey)
        {
            var panel = GetPanel(panelKey);
            if (panel == null)
                throw new ArgumentException($"Panel '{panelKey}' not found", nameof(panelKey));

            if (panel.State == DockPanelState.Docked)
                return;

            if (panel.State != DockPanelState.Hidden)
                return;

            panel.ShowCaption = true;
            panel.Visible = true;
            panel.State = DockPanelState.Docked;

            if (panel.Group == null)
            {
            var group = GetOrCreateGroupAtPosition(panel.DockPosition);
            group.AddPanel(panel);
            group.ActivePanel = panel;
            EnsurePanelHosted(panel, makeActive: true);
            }

            EnsurePanelHosted(panel, makeActive: true);

            _layoutController?.InvalidateLayout();
            RecalculateLayout();
            OnPanelShown(panel);

            Debug.WriteLine($"[BeepDockingManager] Panel shown: {panelKey}");
        }

        /// <summary>
        /// Hides a panel without closing it — it stays in the layout tree but is collapsed.
        /// Mirrors DockContent.Hide() / Krypton HidePage().
        /// </summary>
        public void HidePanel(string panelKey)
        {
            var panel = GetPanel(panelKey);
            if (panel == null)
                throw new ArgumentException($"Panel '{panelKey}' not found", nameof(panelKey));

            if (panel.State == DockPanelState.Hidden)
                return;

            // A panel that owns the whole container cannot simply leave it: every other panel
            // is concealed and nothing would occupy the space. Restore first, then proceed
            // against the normal arrangement.
            RestoreIfMaximised(panelKey);

            panel.State = DockPanelState.Hidden;
            panel.Visible = false;

            if (panel.Parent is BeepDockspace ds)
            {
                ds.Controls.Remove(panel);
                ds.LayoutPanels();
                ds.Invalidate();
            }
            else if (_hostForm != null && _hostForm.Controls.Contains(panel))
            {
                _hostForm.Controls.Remove(panel);
            }

            // Remove from MRU so Ctrl+Tab doesn't offer a panel that isn't reachable.
            RemoveMrPanel(panelKey);

            _layoutController?.InvalidateLayout();
            // Reflow so the remaining docked panels reclaim the hidden panel's space.
            RecalculateLayout();
            OnPanelHidden(panel);

            Debug.WriteLine($"[BeepDockingManager] Panel hidden: {panelKey}");
        }

        /// <summary>
        /// Closes a panel and stores it for later reopening.
        /// Mirrors DockPanelSuite DockContentHandler close + Krypton StorePage().
        /// The panel is NOT permanently removed — call ReopenPanel to restore it.
        /// </summary>
        public void ClosePanel(string panelKey)
        {
            if (!_panelsByKey.TryGetValue(panelKey, out var panel))
                throw new ArgumentException($"Panel '{panelKey}' not found", nameof(panelKey));


            // A panel that owns the whole container cannot simply leave it: every other panel
            // is concealed and nothing would occupy the space. Restore first, then proceed
            // against the normal arrangement.
            RestoreIfMaximised(panelKey);

            // HideOnClose: keep the panel in the manager and just hide it. ShowPanel will
            // re-attach it to the layout tree. Mirrors DockContent.HideOnClose behavior.
            if (panel.HideOnClose)
            {
                if (panel.State == DockPanelState.AutoHidden)
                    DetachFromAutoHideStrip(panel);

                panel.Group?.RemovePanel(panel);
                _layoutTree.UnregisterPanel(panelKey);

                // Remove from dockspace or parent so LayoutPanels doesn't include it.
                if (panel.Parent is BeepDockspace ds)
                {
                    ds.Controls.Remove(panel);
                    ds.LayoutPanels();
                    ds.Invalidate();
                }
                else if (panel.Parent != null)
                {
                    panel.Parent.Controls.Remove(panel);
                }

                panel.Visible = false;
                panel.State = DockPanelState.Hidden;
                _layoutController?.InvalidateLayout();
                RecalculateLayout();
                panel.OnClosed();
                OnPanelHidden(panel);
                return;
            }

            // Preserve state before removal so ReopenPanel can restore it
            _closedPanels[panelKey] = panel;

            if (panel.State == DockPanelState.Floating)
                CloseFloatWindowFor(panel);
            else if (panel.State == DockPanelState.AutoHidden)
                DetachFromAutoHideStrip(panel);

            // Remove from active layout (same as RemovePanel but without discarding the object)
            if (panel.Group != null)
                panel.Group.RemovePanel(panel);

            _tabHandler?.UnregisterTab(panelKey);

            // Remove the panel control from the host form / dockspace (do not dispose — stored for reopen).
            // The edge splitter (keyed by group id) is reconciled by the RecalculateLayout below.
            if (panel.Parent is BeepDockspace closedDs)
            {
                closedDs.Controls.Remove(panel);
                closedDs.LayoutPanels();
                closedDs.Invalidate();
            }
            else if (_hostForm != null && _hostForm.Controls.Contains(panel))
            {
                _hostForm.Controls.Remove(panel);
            }
            panel.Visible = false;

            _panelsByKey.Remove(panelKey);
            _layoutTree.UnregisterPanel(panelKey);
            RemoveMrPanel(panelKey);

            panel.State = DockPanelState.Closed;
            _layoutController?.InvalidateLayout();
            RecalculateLayout();

            panel.OnClosed();
            OnPanelClosed(panel);

            Debug.WriteLine($"[BeepDockingManager] Panel closed (stored): {panelKey}");
        }

        /// <summary>
        /// Reopens a previously closed panel at its last dock position.
        /// Mirrors Krypton ClearStoredPage / DockPanelSuite Show-after-close.
        /// </summary>
        public DockPanel ReopenPanel(string panelKey)
        {
            if (!_closedPanels.TryGetValue(panelKey, out var panel))
                throw new InvalidOperationException($"No closed panel found for key '{panelKey}'");

            _closedPanels.Remove(panelKey);
            InitializeSubsystems();

            _panelsByKey[panelKey] = panel;
            if (_layoutTree.GetPanel(panelKey) == null)
                _layoutTree.RegisterPanel(panel);

            ApplyThemeToPanel(panel);
            panel.State = DockPanelState.Docked;
            panel.ShowCaption = true;
            panel.Visible = true;

            var group = GetOrCreateGroupAtPosition(panel.DockPosition);
            group.AddPanel(panel);
            group.ActivePanel = panel;
            EnsurePanelHosted(panel, makeActive: true);

            _tabHandler?.RegisterTab(panelKey, panel.Title ?? "Panel");
            _layoutController?.InvalidateLayout();
            RecalculateLayout();

            panel.OnActivated();
            OnPanelReopened(panel);

            Debug.WriteLine($"[BeepDockingManager] Panel reopened: {panelKey}");
            return panel;
        }

        /// <summary>
        /// Returns whether a panel key exists in the closed store.
        /// </summary>
        public bool IsPanelClosed(string panelKey) => _closedPanels.ContainsKey(panelKey);

        /// <summary>
        /// Gets all panel keys that are currently in the closed store.
        /// </summary>
        public IReadOnlyList<string> GetClosedPanelKeys() =>
            _closedPanels.Keys.ToList().AsReadOnly();

        /// <summary>
        /// Moves a panel into a floating Form window.
        /// Mirrors FloatWindow behavior in DockPanelSuite / Krypton MakeFloatingRequest.
        /// Uses <paramref name="initialBounds"/> when provided so the window appears at the
        /// restore position without a flash.
        /// </summary>
        public void FloatPanel(string panelKey, Rectangle initialBounds = default)
        {
            var panel = GetPanel(panelKey);
            if (panel == null)
                throw new ArgumentException($"Panel '{panelKey}' not found", nameof(panelKey));

            if (!panel.CanFloat)
                throw new InvalidOperationException($"Panel '{panelKey}' does not allow floating");

            // A panel that owns the whole container cannot simply leave it: every other panel
            // is concealed and nothing would occupy the space. Restore first, then proceed
            // against the normal arrangement.
            RestoreIfMaximised(panelKey);

            // Fire the UI-level cancelable request so subscribers (e.g. host application
            // that wants to confirm before floating) can veto. MakeFloatingRequest also fires
            // this event; both paths now raise it so either entry point is covered.
            var floatArgs = new CancelPanelRequestEventArgs(panelKey, panel);
            OnPageFloatingRequest(floatArgs);
            if (floatArgs.Cancel) return;

            if (panel.State == DockPanelState.Floating)
                return;

            if (panel.State == DockPanelState.AutoHidden)
                DetachFromAutoHideStrip(panel);
            else if (panel.State == DockPanelState.Hidden)
            { /* already detached from host by HidePanel */ }
            else if (panel.State == DockPanelState.Closed && _hostForm != null && _hostForm.Controls.Contains(panel))
                _hostForm.Controls.Remove(panel);

            // Remove from its current group in the layout tree.
            // Save the original group so we can restore the panel to it on cancel.
            var originalGroup = panel.Group;
            originalGroup?.RemovePanel(panel);
            _layoutController?.InvalidateLayout();

            panel.Visible = true;

            // Create and show the float window
            FloatWindow floatWindow;
            if (initialBounds.IsEmpty)
            {
                if (!_defaultFloatWindowSize.IsEmpty)
                {
                    // Place the float window near the cursor (or form center) at the configured size.
                    var origin = _hostForm != null
                        ? _hostForm.PointToClient(Control.MousePosition)
                        : new Point(0, 0);
                    if (_hostForm != null && !_hostForm.ClientRectangle.Contains(origin))
                        origin = new Point(
                            (_hostForm.ClientSize.Width  - _defaultFloatWindowSize.Width)  / 2,
                            (_hostForm.ClientSize.Height - _defaultFloatWindowSize.Height) / 2);
                    floatWindow = new FloatWindow(panel, _hostForm,
                        new Rectangle(origin, _defaultFloatWindowSize));
                }
                else
                {
                    floatWindow = new FloatWindow(panel, _hostForm);
                }
            }
            else
            {
                floatWindow = new FloatWindow(panel, _hostForm, initialBounds);
            }
            floatWindow.ControlStyle = _style;
            // Same display set the manager resolves saved float bounds against, so snapping and
            // restoring cannot disagree about which screens exist.
            floatWindow.Monitors = Monitors;
            floatWindow.ApplyDockingTheme(_themeColors);
            floatWindow.PanelRedocked += OnFloatWindowRedocked;

            // Wire drag-guide overlay — shown while the float window is moved.
            // Follows DockPanelSuite DockDragHandler: overlay tracks cursor, HitTest on move.
            // Skipped when AllowEndUserDocking is false so the user can't drop a float back into
            // the layout (they can still close the float, just not redock it).
            if (_allowEndUserDocking)
            {
                EnsureGuideOverlay();
                floatWindow.Move += (s, e) => OnFloatWindowMoved(floatWindow);
                floatWindow.MouseUp += (s, e) => TryCommitFloatWindowDrop(floatWindow, e);
                floatWindow.MoveOperationEnded += (s, e) => TryCommitFloatWindowDrop(floatWindow, null);
            }
            floatWindow.FormClosed += (s, e) =>
            {
                _floatWindowsByKey.Remove(panelKey);
                HideGuideOverlay();
            };

            _floatWindowsByKey[panelKey] = floatWindow;

            var addingArgs = new FloatingWindowEventArgs(floatWindow, panel);
            OnFloatingWindowAdding(addingArgs);

            floatWindow.Show(_hostForm);

            if (!RaiseStateChanging(panel, panel.State, DockPanelState.Floating))
            {
                // Cancel path: pull the panel back out of the float window and re-attach it to
                // the dockspace / layout group we removed it from. Without this the panel would
                // be orphaned (no parent, no group) and FloatWindow.OnFormClosing would route it
                // through CloseRequest → HidePanel on close, which is surprising.
                var orphan = floatWindow.ExtractHostedPanel();
                if (orphan != null)
                {
                    var restoreGroup = originalGroup ?? GetOrCreateGroupAtPosition(panel.DockPosition);
                    if (orphan.Group == null)
                        restoreGroup.AddPanel(orphan);
                    EnsurePanelHosted(orphan);
                }
                floatWindow.Close();
                _floatWindowsByKey.Remove(panelKey);
                RecalculateLayout();
                return;
            }
            panel.State = DockPanelState.Floating;
            RecalculateLayout();   // reflow remaining docked panels now this one left the site
            OnPanelFloated(panel);

            Debug.WriteLine($"[BeepDockingManager] Panel floated: {panelKey}");
        }

        private void EnsureGuideOverlay()
        {
            if (_guideOverlay == null || _guideOverlay.IsDisposed)
                _guideOverlay = new DockingGuideOverlay();
        }

        private void OnFloatWindowMoved(FloatWindow floatWindow)
        {
            if (_guideOverlay == null || _hostForm == null) return;
            _guideOverlay.ShowOver(_hostForm, Monitors.GetMonitors());
            DockingDropTarget.HitTest(_guideOverlay, Control.MousePosition);
        }

        private void TryCommitFloatWindowDrop(FloatWindow floatWindow, MouseEventArgs e)
        {
            if (e != null && e.Button != MouseButtons.Left)
                return;

            if (floatWindow?.Panel == null || _hostForm == null)
                return;

            var target = DockingDropTarget.HitTest(_guideOverlay, Control.MousePosition);
            HideGuideOverlay();

            if (!target.HasValue || target.Value == DockPosition.Floating)
                return;

            var panel = floatWindow.Panel;
            if (panel.State != DockPanelState.Floating)
                return;

            if (!IsPositionAllowed(panel, target.Value))
                return;

            var args = new CancelPanelRequestEventArgs(panel.Key, panel);
            OnPageDockedRequest(args);
            if (args.Cancel)
                return;

            DockFloatingPanel(panel.Key, target.Value);
        }

        private void HideGuideOverlay()
        {
            if (_guideOverlay != null && _guideOverlay.Visible)
                _guideOverlay.Hide();
        }

        /// <summary>
        /// Re-docks a panel that is currently floating, at the specified position.
        /// Mirrors DockPanelSuite DockContent.Dock() / Krypton MakeDockedRequest.
        /// </summary>
        public void DockFloatingPanel(string panelKey, DockPosition position)
        {
            var panel = GetPanel(panelKey);
            if (panel == null)
                throw new ArgumentException($"Panel '{panelKey}' not found", nameof(panelKey));

            if (panel.State != DockPanelState.Floating)
            {
                Debug.WriteLine($"[BeepDockingManager] DockFloatingPanel skipped: panel '{panelKey}' state={panel.State}, expected Floating");
                return;
            }

            // Dismiss the drag-guide overlay now that a drop target is confirmed.
            // Mirrors DockPanelSuite DockDragHandler.OnEndDrag().
            HideGuideOverlay();
            CloseFloatWindowFor(panel);

            panel.ShowCaption = true;
            panel.Visible = true;
            panel.DockPosition = position;
            panel.State = DockPanelState.Docked;

            var group = GetOrCreateGroupAtPosition(position);
            group.AddPanel(panel);
            group.ActivePanel = panel;
            EnsurePanelHosted(panel, makeActive: true);

            _layoutController?.InvalidateLayout();
            RecalculateLayout();

            Debug.WriteLine($"[BeepDockingManager] Panel docked: {panelKey} at {position}");
        }

        /// <summary>
        /// Collapses a panel to the auto-hide edge tab strip.
        /// Mirrors AutoHideWindowControl in DockPanelSuite / Krypton MakeAutoHiddenRequest.
        /// Requires panel.CanAutoHide (AllowedAreas.AutoHide) to be set.
        /// </summary>
        public void AutoHidePanel(string panelKey)
        {
            var panel = GetPanel(panelKey);
            if (panel == null)
                throw new ArgumentException($"Panel '{panelKey}' not found", nameof(panelKey));

            if (!panel.CanAutoHide)
                throw new InvalidOperationException($"Panel '{panelKey}' does not allow auto-hide");

            // A panel that owns the whole container cannot simply leave it: every other panel
            // is concealed and nothing would occupy the space. Restore first, then proceed
            // against the normal arrangement.
            RestoreIfMaximised(panelKey);

            var hideArgs = new CancelPanelRequestEventArgs(panelKey, panel);
            OnPageAutoHiddenRequest(hideArgs);
            if (hideArgs.Cancel) return;

            if (panel.State == DockPanelState.AutoHidden)
                return;

            if (panel.State == DockPanelState.Floating)
                CloseFloatWindowFor(panel);

            // Remove from its current docked group in the layout tree
            panel.Group?.RemovePanel(panel);

            // Detach from the host / dockspace — the slide panel hosts it while peeked.
            if (panel.Parent is BeepDockspace ds)
            {
                ds.Controls.Remove(panel);
                ds.LayoutPanels();
                ds.Invalidate();
            }
            else if (_hostForm != null && _hostForm.Controls.Contains(panel))
            {
                _hostForm.Controls.Remove(panel);
            }
            panel.Dock = DockStyle.None;

            // Hand off to the AutoHideStrip for the panel's edge —
            // the strip adds a tab button and manages the slide panel.
            // Mirrors DockPanelSuite DockContent → DockState.DockLeftAutoHide path.
            if (_autoHideStrips.TryGetValue(panel.DockPosition, out var strip))
            {
                strip.Visible = true;   // show the strip edge if it was hidden
                strip.AddPanel(panel);
                OnAutoHiddenGroupAdding(new AutoHiddenGroupEventArgs(panel, panel.DockPosition));
            }
            else
            {
                // Fallback when strips are not yet created (design-time or ManageControl not called)
                panel.Visible = false;
            }

            if (!RaiseStateChanging(panel, panel.State, DockPanelState.AutoHidden))
                return;

            panel.State = DockPanelState.AutoHidden;

            // Remove from MRU — the auto-hide strip owns the panel now and Ctrl+Tab
            // filters to docked panels only, so leaving it in MRU would be dead state.
            RemoveMrPanel(panelKey);

            _layoutController?.InvalidateLayout();
            RecalculateLayout();   // reflow the panels that remain docked + reconcile splitters
            OnPanelAutoHidden(panel);

            Debug.WriteLine($"[BeepDockingManager] Panel auto-hidden: {panelKey}");
        }

        /// <summary>
        /// Restores (unpins) an auto-hidden panel back to a real docked group at its edge.
        /// Inverse of <see cref="AutoHidePanel"/>; triggered by clicking the edge strip tab.
        /// </summary>
        public void RestoreAutoHiddenPanel(string panelKey)
        {
            var panel = GetPanel(panelKey);
            if (panel == null || panel.State != DockPanelState.AutoHidden)
                return;

            DetachFromAutoHideStrip(panel);
            panel.ShowCaption = true;
            panel.Visible = true;
            if (!RaiseStateChanging(panel, panel.State, DockPanelState.Docked))
                return;
            panel.State = DockPanelState.Docked;

            var group = GetOrCreateGroupAtPosition(panel.DockPosition);
            group.AddPanel(panel);
            group.ActivePanel = panel;

            _layoutController?.InvalidateLayout();
            RecalculateLayout();
            OnPanelShown(panel);

            Debug.WriteLine($"[BeepDockingManager] Panel restored from auto-hide: {panelKey}");
        }

        private void OnStripRestoreRequested(object sender, DockPanel panel)
        {
            if (panel != null)
                RestoreAutoHiddenPanel(panel.Key);
        }

        private void OnStripCloseRequested(object sender, DockPanel panel)
        {
            if (panel != null)
                CloseRequest(panel.Key);
        }

        private void OnStripFloatRequested(object sender, DockPanel panel)
        {
            if (panel != null)
                FloatPanel(panel.Key);
        }

        /// <summary>
        /// Handles <see cref="AutoHideStrip.SlideShown"/>: when the slide-in animation finishes,
        /// bring the hosted panel to the front of the slide's z-order and (when
        /// <see cref="ActiveAutoHideContent"/> is true) focus it so the user can type into it
        /// without an extra click.
        /// </summary>
        private void OnStripSlideShown(object sender, DockPanel panel)
        {
            if (panel == null) return;
            if (_activeAutoHideContent)
                FocusManager.Focus(panel);
        }


        /// <summary>
        /// Gets or creates a docking group at the specified position.
        /// </summary>
        /// <summary>
        /// Returns a <b>leaf</b> group at <paramref name="position"/>, creating the edge group if it
        /// does not exist yet. Every caller uses the result to place a panel in.
        /// </summary>
        /// <remarks>
        /// The leaf part matters. Once an edge has been split it is a parent of child groups, and
        /// <c>AssignPanelsRecursive</c> allocates bounds to a group's own panels only when it has no
        /// visible children — so a panel added directly to a split edge is docked, registered, and
        /// invisible, with no bounds at all. That is the <see cref="Layout.ErrorType.MixedContent"/>
        /// state the layout validator already names; this stops it being produced in the first
        /// place. Descending prefers the child holding an active panel, so a new panel joins the
        /// group the user was last working in rather than an arbitrary one.
        /// </remarks>
        private DockGroup GetOrCreateGroupAtPosition(DockPosition position)
        {
            // Try to find existing group at this position
            var existingGroup = _layoutTree.Root.Children.FirstOrDefault(g => g.Position == position);
            if (existingGroup != null)
                return ResolveLeafGroup(existingGroup);

            // Create new group
            var newGroup = new DockGroup
            {
                Id = $"group_{position}_{Guid.NewGuid().ToString("N").Substring(0, 8)}",
                Position = position,
                HeaderPosition = HeaderPosition.Top
            };

            _layoutTree.RegisterGroup(newGroup);
            _layoutTree.Root.AddChild(newGroup);

            return newGroup;
        }

        /// <summary>Descends to a leaf group, preferring the branch holding an active panel.</summary>
        private static DockGroup ResolveLeafGroup(DockGroup group)
        {
            // Bounded by the tree depth; a cycle would be a MissingRoot/CircularReference fault the
            // validator reports separately, and is not worth a second guard here.
            while (group.Children.Count > 0)
            {
                group = group.Children.FirstOrDefault(c => c.ActivePanel != null)
                        ?? group.Children[0];
            }
            return group;
        }

        /// <summary>
        /// Hooks into the theme change event to invalidate painter caches.
        /// </summary>
        private void RegisterThemeHook()
        {
            TrySubscribeThemeChanged(IsDesignHosted);
        }

        private void TrySubscribeThemeChanged(bool isDesignMode)
        {
            if (_subscribedToThemeChanged || isDesignMode)
                return;

            try
            {
                BeepThemesManager.ThemeChanged -= OnGlobalThemeChanged;
                BeepThemesManager.ThemeChanged += OnGlobalThemeChanged;
                _subscribedToThemeChanged = true;
            }
            catch
            {
                // Theme subscription is best-effort so designer/runtime creation stays stable.
            }
        }

        private void UnsubscribeThemeChanged()
        {
            if (!_subscribedToThemeChanged)
                return;

            try
            {
                BeepThemesManager.ThemeChanged -= OnGlobalThemeChanged;
            }
            catch
            {
                // best-effort
            }

            _subscribedToThemeChanged = false;
        }

        private void OnGlobalThemeChanged(object sender, ThemeChangeEventArgs e)
        {
            if (_disposed)
                return;

            try
            {
                _themeName = e?.NewThemeName ?? BeepThemesManager.CurrentThemeName;
                _currentTheme = e?.NewTheme
                                ?? BeepThemesManager.GetTheme(_themeName)
                                ?? BeepThemesManager.GetDefaultTheme();
                ApplyTheme();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[BeepDockingManager] Global theme change error: {ex.Message}");
            }
        }

        private void WalkSurfaces(Action<BeepDockspace> onDockspace, Action<AutoHideStrip> onStrip, Action<BeepDockSplitter> onSplitter)
        {
            foreach (var ds in GetManagedDockspaces()) onDockspace(ds);
            foreach (var strip in _autoHideStrips.Values) { if (strip != null) onStrip(strip); }
            foreach (var sp in _splitters.Values) { if (sp != null) onSplitter(sp); }
        }

        private void ApplyDockingThemeColors(DockingThemeColors colors, bool updatePainter)
        {
            _themeColors = colors ?? DockingThemeColors.Default;

            if (updatePainter && _painter is DockingPainterAdapter adapter)
            {
                adapter.ApplyTheme(
                    _themeColors.PanelBackColor,
                    _themeColors.PanelForeColor,
                    _themeColors.BorderColor,
                    _themeColors.HoverBackColor,
                    _themeColors.ActiveTabBackColor);
            }

            foreach (var panel in _panelsByKey.Values)
                ApplyThemeToPanel(panel);

            foreach (var dockspace in GetManagedDockspaces())
                ApplyThemeToDockspace(dockspace);

            WalkSurfaces(
                _ => { },
                strip => { strip.ControlStyle = _style; strip.ApplyDockingTheme(_themeColors); },
                sp => { sp.ControlStyle = _style; sp.ApplyDockingTheme(_themeColors); });

            OnThemeChanged();
        }

        /// <summary>
        /// Pushes the current <see cref="Style"/> to every docking surface and repaints them.
        /// </summary>
        private void PropagateControlStyle()
        {
            foreach (var panel in _panelsByKey.Values)
            {
                if (panel == null) continue;
                panel.ControlStyle = _style;
                panel.Invalidate();
            }

            WalkSurfaces(
                ds => { ds.ControlStyle = _style; ds.TabStyle = _tabStyle; ds.Invalidate(); },
                strip => { strip.ControlStyle = _style; strip.Invalidate(); },
                sp => { sp.ControlStyle = _style; sp.Invalidate(); });
        }

        internal void ApplyThemeToDockspace(BeepDockspace dockspace)
        {
            if (dockspace == null)
                return;

            dockspace.ControlStyle = _style;
            dockspace.TabStyle = _tabStyle;
            dockspace.ApplyDockingTheme(_themeColors);
        }

        internal void ApplyThemeToPanel(DockPanel panel)
        {
            if (panel == null)
                return;

            panel.ControlStyle = _style;
            panel.ApplyDockingTheme(_themeColors);
        }

        private IEnumerable<BeepDockspace> GetManagedDockspaces()
        {
            var seen = new HashSet<BeepDockspace>();

            if (_site?.Container != null)
            {
                foreach (BeepDockspace dockspace in _site.Container.Components.OfType<BeepDockspace>())
                {
                    if (ReferenceEquals(dockspace.Manager, this) && seen.Add(dockspace))
                        yield return dockspace;
                }
            }

            if (_hostForm != null)
            {
                foreach (BeepDockspace dockspace in EnumerateControls(_hostForm).OfType<BeepDockspace>())
                {
                    if (ReferenceEquals(dockspace.Manager, this) && seen.Add(dockspace))
                        yield return dockspace;
                }
            }
        }

        private static IEnumerable<Control> EnumerateControls(Control root)
        {
            if (root == null)
                yield break;

            foreach (Control child in root.Controls)
            {
                yield return child;

                foreach (Control grandChild in EnumerateControls(child))
                    yield return grandChild;
            }
        }

        private void OnStringPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            foreach (var panel in _panelsByKey.Values)
                panel.Invalidate();

            foreach (var strip in _autoHideStrips.Values)
                strip?.Invalidate();
        }

        /// <summary>
        /// Called when the active theme changes; signals that UI needs repainting.
        /// </summary>
        private void OnThemeChanged()
        {
            OnThemeChangedRaised();
            Debug.WriteLine("[BeepDockingManager] Theme changed - invalidate paint cache");
        }

        // ── Batch-update (mirrors KryptonDockingMultiUpdate / DockingMultiUpdate) ────

        /// <summary>
        /// Suspends layout recalculation.  Call <see cref="EndUpdate"/> to resume.
        /// Prefer the disposable <see cref="BeepDockingUpdate"/> wrapper.
        /// </summary>
        public void BeginUpdate() => _updateDepth++;

        /// <summary>
        /// Resumes layout recalculation.  A single recalculation pass is applied when the
        /// outermost scope exits.  Mirrors KryptonDockingMultiUpdate.Dispose().
        /// </summary>
        public void EndUpdate()
        {
            if (_updateDepth > 0)
                _updateDepth--;

            if (_updateDepth == 0)
                RecalculateLayout();
        }

        // ── Show / Hide bulk overloads (mirrors Krypton ShowPages / HidePages) ───────


        // ── Collection accessors (mirrors Krypton Pages / PagesDocked / etc.) ────────

        /// <summary>Gets all live (non-closed) panels.</summary>
        public DockPanel[] Pages => _panelsByKey.Values.ToArray();

        /// <summary>Gets all panels currently in the Docked state.</summary>
        public DockPanel[] PagesDocked =>
            _panelsByKey.Values.Where(p => p.State == DockPanelState.Docked).ToArray();

        /// <summary>Gets all panels currently in the AutoHidden state.</summary>
        public DockPanel[] PagesAutoHidden =>
            _panelsByKey.Values.Where(p => p.State == DockPanelState.AutoHidden).ToArray();

        /// <summary>Gets all panels currently in the Floating state.</summary>
        public DockPanel[] PagesFloating =>
            _panelsByKey.Values.Where(p => p.State == DockPanelState.Floating).ToArray();

        /// <summary>Gets all panels currently in the Hidden state.</summary>
        public DockPanel[] PagesHidden =>
            _panelsByKey.Values.Where(p => p.State == DockPanelState.Hidden).ToArray();

        /// <summary>
        /// Gets a diagnostic summary of the manager state.
        /// </summary>
        public string GetDiagnostics()
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("=== BeepDockingManager Diagnostics ===");
            sb.AppendLine($"Host Form: {_hostForm?.Name ?? "(null)"}");
            sb.AppendLine($"Panels: {PanelCount}");
            sb.AppendLine();
            sb.Append(_layoutTree.GetDiagnostics());
            return sb.ToString();
        }

        // ── Protected virtual OnXxx raise methods (Krypton pattern) ─────────────
        // Subclasses override these to intercept or suppress events.


        /// <summary>
        /// Returns the key of the panel currently considered "active" — preferring the head of
        /// the MRU list when that panel is in the <see cref="DockPanelState.Docked"/> state,
        /// otherwise the first docked panel registered with the manager. Returns null if no
        /// docked panel is registered.
        /// </summary>
        public string GetActivePanelKey()
        {
            if (_mruList.First?.Value is string key &&
                _panelsByKey.TryGetValue(key, out var panel) &&
                panel.State == DockPanelState.Docked)
            {
                return key;
            }

            var fallback = _panelsByKey.Values
                .FirstOrDefault(p => p.State == DockPanelState.Docked);
            return fallback?.Key;
        }

        /// <summary>
        /// Returns the key of the first panel whose state matches <paramref name="state"/>, in
        /// the order the manager registers them. Returns null if no panel matches. Useful for
        /// callers that need to act on auto-hidden, floating, or closed panels (e.g., restore
        /// the most recent auto-hidden panel via the keyboard).
        /// </summary>
        public string GetActivePanelKey(DockPanelState state)
        {
            foreach (var kv in _panelsByKey)
            {
                if (kv.Value != null && kv.Value.State == state)
                    return kv.Key;
            }
            return null;
        }

        /// <summary>
        /// Disposes the manager and cleans up all resources.
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
                return;

            DetachHostFormHandlers();
            CloseAllFloatWindows();
            ClearAllAutoHidePanels();

            foreach (var panel in _panelsByKey.Values.ToList())
            {
                if (_hostForm != null && _hostForm.Controls.Contains(panel))
                    _hostForm.Controls.Remove(panel);
                panel.Dispose();
            }

            _panelsByKey.Clear();
            _layoutTree.Clear();

            // Closed panels were removed from the host form's Controls, so the form will not
            // dispose them — release their controls here to avoid leaking them.
            foreach (var closed in _closedPanels.Values)
                closed?.Dispose();
            _closedPanels.Clear();

            foreach (var strip in _autoHideStrips.Values)
                strip?.Dispose();
            _autoHideStrips.Clear();

            _guideOverlay?.Dispose();
            _guideOverlay = null;

            _dragController?.Dispose();
            _dragController = null;

            _navigator?.Close();
            _navigator?.Dispose();
            _navigator = null;

            foreach (var sp in _splitters.Values)
                sp?.Dispose();
            _splitters.Clear();

            _tabHandler?.Dispose();
            _tabHandler = null;

            if (!ReferenceEquals(_painter, NullDockingPainter.Instance))
                _painter.Dispose();
            _painter = NullDockingPainter.Instance;
            _layoutController = null;
            Strings.PropertyChanged -= OnStringPropertyChanged;
            UnsubscribeThemeChanged();

            // IComponent cleanup
            _site?.Container?.Remove(this);
            _site = null;

            _disposed = true;
            _disposed_event?.Invoke(this, EventArgs.Empty);
            Debug.WriteLine("[BeepDockingManager] Disposed");
        }
    }
}
