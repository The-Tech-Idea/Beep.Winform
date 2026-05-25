using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Converters;
using TheTechIdea.Beep.Winform.Controls.Docking.Interop;
using TheTechIdea.Beep.Winform.Controls.Docking.Layout;
using TheTechIdea.Beep.Winform.Controls.Docking.Models;
using TheTechIdea.Beep.Winform.Controls.Docking.Painters;
using TheTechIdea.Beep.Winform.Controls.Docking.Runtime;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;

namespace TheTechIdea.Beep.Winform.Controls.Docking
{
    /// <summary>
    /// Core orchestrator for the native Win32 MDI-based docking system.
    /// 
    /// Responsibilities:
    /// - Manages MDI client window (HWND) creation and lifecycle
    /// - Owns the panel registry and layout tree
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
    public class BeepDockingManager : IComponent, IDisposable
    {
        private IntPtr _hostHwnd = IntPtr.Zero;
        private IntPtr _mdiClientHwnd = IntPtr.Zero;
        private Form _hostForm;

        /// <summary>
        /// True when running inside the Visual Studio designer (no Win32 ops allowed).
        /// Uses the canonical LicenseManager approach, not DesignMode, so it works
        /// even before a Site is assigned — matching Krypton's pattern.
        /// </summary>
        private static bool IsInDesigner =>
            LicenseManager.UsageMode == LicenseUsageMode.Designtime ||
            IsWinFormsDesignerProcess();

        private bool IsDesignHosted =>
            IsInDesigner || _site?.DesignMode == true || _hostForm?.Site?.DesignMode == true;
        private DockLayoutTree _layoutTree = new DockLayoutTree();
        private DockingLayoutController _layoutController;
        private IDockingPainter _painter = NullDockingPainter.Instance;
        private MdiPanelPositioner _positioner;
        private WindowChrome _chrome;
        private PanelWindowManager _panelWindowManager;
        private ContentHosting _contentHosting;
        private EventInterceptor _eventInterceptor;
        private DockingMouseEventHandler _mouseHandler;
        private SplitterDragHandler _dragHandler;
        private TabInteractionHandler _tabHandler;
        private PainterIntegration _painterIntegration;
        private Dictionary<string, DockPanel> _panelsByKey = new Dictionary<string, DockPanel>();
        private Dictionary<IntPtr, DockPanel> _panelsByHwnd = new Dictionary<IntPtr, DockPanel>();
        private Dictionary<IntPtr, string> _windowToPanelMap = new Dictionary<IntPtr, string>();
        // Panels that have been closed but not yet permanently removed — they can be reopened.
        // Key = panel key, Value = panel snapshot with its last known state preserved.
        private Dictionary<string, DockPanel> _closedPanels = new Dictionary<string, DockPanel>();
        // One AutoHideStrip per edge — created in ManageControl, keyed by DockPosition.
        private Dictionary<DockPosition, AutoHideStrip> _autoHideStrips = new Dictionary<DockPosition, AutoHideStrip>();
        // Dockspace controls this manager has subscribed to for Krypton-style page events.
        private HashSet<BeepDockspace> _dockspaceEventSinks = new HashSet<BeepDockspace>();
        // Shared guide overlay for drag-to-dock — created lazily on first FloatPanel() call.
        private DockingGuideOverlay _guideOverlay;
        // Per-panel splitters — keyed by panel key.  Created in AddPanel(), disposed in RemovePanel().
        private Dictionary<string, BeepDockSplitter> _splitters = new Dictionary<string, BeepDockSplitter>();
        private bool _disposed = false;
        private bool _subscribedToThemeChanged = false;
        private bool _useThemeColors = true;
        private string _themeName = string.Empty;
        private IBeepTheme _currentTheme;
        private DockingThemeColors _themeColors = DockingThemeColors.Default;

        // Batch-update nesting counter — layout is suspended while > 0.
        private int _updateDepth;

        // IComponent implementation
        private ISite _site;
        private EventHandler _disposed_event;

        /// <summary>
        /// Raised when a panel is activated (becomes the active panel in its group).
        /// </summary>
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
        /// Gets the resolved theme object currently used by the docking manager.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IBeepTheme CurrentTheme => _currentTheme;

        /// <summary>
        /// Gets the host window handle (HWND).
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IntPtr HostHwnd => _hostHwnd;

        /// <summary>
        /// Gets the native MDI client window handle.
        /// Returns IntPtr.Zero if not yet created.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IntPtr MdiClientHwnd => _mdiClientHwnd;

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
                    _chrome?.InvalidateCache();
                }
            }
        }

        /// <summary>
        /// Gets the panel positioner for applying layout to windows.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public MdiPanelPositioner Positioner => _positioner;

        /// <summary>
        /// Gets the window chrome renderer for tabs and decorations.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public WindowChrome Chrome => _chrome;

        /// <summary>
        /// Gets the panel window manager for lifecycle coordination.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public PanelWindowManager PanelWindowManager => _panelWindowManager;

        /// <summary>
        /// Gets the content hosting manager for control reparenting.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ContentHosting ContentHosting => _contentHosting;

        /// <summary>
        /// Gets the event interceptor for message routing.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public EventInterceptor EventInterceptor => _eventInterceptor;

        /// <summary>
        /// Gets the splitter drag handler for interactive resizing.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SplitterDragHandler DragHandler => _dragHandler;

        /// <summary>
        /// Gets the tab interaction handler for tab management.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public TabInteractionHandler TabHandler => _tabHandler;

        /// <summary>
        /// Gets the painter integration bridge.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public PainterIntegration PainterIntegration => _painterIntegration;

        /// <summary>
        /// Applies explicit theme colours to every docking surface.
        /// Call this from your application's ThemeChanged handler.
        /// Mirrors Krypton's <c>KryptonManager.GlobalPalette</c> reassignment pattern:
        /// push new colours into the painter adapter, then invalidate all panels.
        /// </summary>
        /// <param name="background">Panel / strip background colour.</param>
        /// <param name="foreground">Title / tab text colour.</param>
        /// <param name="border">Panel border / splitter colour.</param>
        /// <param name="hover">Mouse-hover highlight colour.</param>
        /// <param name="accent">Active-tab / active-caption accent colour.</param>
        public void ApplyTheme(Color background, Color foreground, Color border,
                               Color hover, Color accent)
        {
            ApplyDockingThemeColors(
                DockingThemeColors.FromExplicit(background, foreground, border, hover, accent),
                updatePainter: true);
        }

        /// <summary>
        /// Applies the currently selected Beep theme to every docking surface.
        /// </summary>
        public void ApplyTheme()
        {
            try
            {
                _currentTheme ??= BeepThemesManager.GetTheme(_themeName)
                                  ?? BeepThemesManager.CurrentTheme
                                  ?? BeepThemesManager.GetDefaultTheme();

                if (string.IsNullOrWhiteSpace(_themeName))
                    _themeName = BeepThemesManager.GetThemeName(_currentTheme);

                ApplyDockingThemeColors(DockingThemeColors.FromTheme(_currentTheme, _useThemeColors), updatePainter: true);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[BeepDockingManager] ApplyTheme error: {ex.Message}");
            }
        }

        /// <summary>
        /// Applies a Beep theme object directly to every docking surface.
        /// </summary>
        public void ApplyTheme(IBeepTheme theme)
        {
            _currentTheme = theme ?? BeepThemesManager.GetDefaultTheme();
            _themeName = BeepThemesManager.GetThemeName(_currentTheme);
            ApplyTheme();
        }

        /// <summary>
        /// Registers a delegate that will be invoked whenever the manager's theme colours change.
        /// Allows the host application to sync colours without sub-classing the manager.
        /// </summary>
        public void RegisterThemeHook(Action<Color, Color, Color, Color, Color> hook)
        {
            if (_painter is DockingPainterAdapter adapter)
            {
                adapter.ThemeChanged += (s, e) =>
                {
                    hook?.Invoke(adapter.BackgroundColor, adapter.ForegroundColor,
                                 adapter.BorderColor,    adapter.HoverColor,
                                 adapter.SelectedColor);
                };
            }
        }

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
            _chrome ??= new WindowChrome(_painter);
            _contentHosting ??= new ContentHosting();
            Debug.WriteLine("[BeepDockingManager] Subsystems initialised.");
        }

        /// <summary>
        /// Attaches the manager to a host form and activates the Win32 runtime path.
        /// Call this once from the form's Load event (or constructor after InitializeComponent).
        /// Mirrors KryptonDockingManager.ManageControl(Control).
        /// </summary>
        /// <param name="hostForm">The form that will host the docking layout.</param>
        public void ManageControl(Form hostForm)
        {
            if (hostForm == null) throw new ArgumentNullException(nameof(hostForm));
            if (_hostForm == hostForm)
            {
                if (!IsDesignHosted)
                {
                    RegisterDesignerCreatedPanels(hostForm);
                    RecalculateLayout();
                }
                return;
            }

            _hostForm = hostForm;

            if (IsDesignHosted)
            {
                Debug.WriteLine("[BeepDockingManager] ManageControl — design-time, Win32 deferred.");
                return;
            }

            InitializeSubsystems();
            _layoutController.ContainerBounds = hostForm.ClientRectangle;
            RegisterDesignerCreatedPanels(hostForm);
            RecalculateLayout();

            Debug.WriteLine($"[BeepDockingManager] ManageControl — attached to: {hostForm.Name}");
        }

        /// <summary>
        /// Creates a <see cref="BeepDockSplitter"/> docked adjacent to <paramref name="panel"/>
        /// so the user can resize it by dragging.
        /// Follows DockPanelSuite SplitterControl positioning convention.
        /// </summary>
        private BeepDockSplitter CreateSplitterFor(DockPanel panel)
        {
            DockStyle splitterDock;
            switch (panel.DockPosition)
            {
                case DockPosition.Left:   splitterDock = DockStyle.Left;   break;
                case DockPosition.Right:  splitterDock = DockStyle.Right;  break;
                case DockPosition.Top:    splitterDock = DockStyle.Top;    break;
                case DockPosition.Bottom: splitterDock = DockStyle.Bottom; break;
                default:
                    return null;   // Fill panels do not get a splitter
            }

            var splitter = new BeepDockSplitter { Dock = splitterDock };
            splitter.ApplyDockingTheme(_themeColors);

            splitter.SplitterMoved += (s, e) =>
            {
                bool horizontal = (panel.DockPosition == DockPosition.Left ||
                                   panel.DockPosition == DockPosition.Right);

                int newSize = Math.Max(50, e.SizeAtDragStart + e.Delta);

                if (horizontal)
                    panel.PreferredWidth  = newSize;
                else
                    panel.PreferredHeight = newSize;

                ApplyLayoutBounds(panel);
                _layoutController.InvalidateLayout();
            };

            return splitter;
        }

        /// <summary>
        /// Creates one <see cref="AutoHideStrip"/> per edge and adds them to the host form.
        /// Called once from <see cref="ManageControl"/> after the host form is known.
        /// Mirrors DockPanelSuite's per-edge auto-hide strip construction.
        /// </summary>
        private void CreateAutoHideStrips()
        {
            if (_hostForm == null || IsDesignHosted) return;

            var edges = new[] { DockPosition.Left, DockPosition.Right, DockPosition.Top, DockPosition.Bottom };
            foreach (var edge in edges)
            {
                if (_autoHideStrips.ContainsKey(edge))
                    continue;

                var strip = new AutoHideStrip(edge, _hostForm);
                strip.ApplyDockingTheme(_themeColors);
                strip.Visible = false;   // only show when a panel is auto-hidden on this edge
                _autoHideStrips[edge] = strip;
            }
        }

        /// <summary>
        /// Creates the native MDI client window.
        /// Must be called before adding any panels.
        /// </summary>
        public void CreateMdiClient()
        {
            if (IsDesignHosted)
            {
                Debug.WriteLine("[BeepDockingManager] CreateMdiClient skipped at design-time.");
                return;
            }

            if (_mdiClientHwnd != IntPtr.Zero)
                return;  // Already created

            InitializeSubsystems();

            if (_hostForm == null)
                throw new InvalidOperationException("Host form has not been assigned. Call ManageControl(form) first.");

            _hostHwnd = _hostForm.Handle;
            if (_hostHwnd == IntPtr.Zero)
                throw new InvalidOperationException("Host window handle is invalid");

            // Create MDI client using the native Win32 API
            _mdiClientHwnd = MdiNativeApi.CreateWindowEx(
                0,  // No extended style
                MdiConstants.MDICLIENT_CLASS,
                "",  // No title
                MdiConstants.WS_CHILD | MdiConstants.WS_VISIBLE | MdiConstants.WS_CLIPCHILDREN | MdiConstants.WS_CLIPSIBLINGS,
                0, 0, 0, 0,  // Will be sized by parent
                _hostHwnd,
                IntPtr.Zero,  // No menu
                IntPtr.Zero,  // hInstance (not used)
                IntPtr.Zero);  // No creation params

            if (_mdiClientHwnd == IntPtr.Zero)
                throw new InvalidOperationException(
                    $"Failed to create MDI client window: {MdiNativeApi.GetLastErrorMessage()}");

            _eventInterceptor ??= new EventInterceptor();
            _painterIntegration ??= new PainterIntegration(_painter, _chrome);
            _eventInterceptor.Install();
            RegisterThemeHook();
            CreateAutoHideStrips();

            // Initialize runtime managers now that we have the MDI client
            _positioner = new MdiPanelPositioner(_mdiClientHwnd, _painter);
            _panelWindowManager = new PanelWindowManager(_layoutTree, _positioner, _chrome);
            _tabHandler?.Dispose();
            _tabHandler = new TabInteractionHandler(_panelWindowManager, _layoutTree);
            foreach (var panel in _panelsByKey.Values)
                _tabHandler.RegisterTab(panel.Key, panel.Title ?? "Panel");

            // Initialize drag handler
            _dragHandler = new SplitterDragHandler(
                _layoutController,
                _positioner,
                _panelWindowManager,
                _chrome,
                _layoutTree,
                _hostForm.ClientSize
            );

            // Initialize mouse handler
            _mouseHandler = new DockingMouseEventHandler(
                _eventInterceptor,
                _chrome,
                _panelWindowManager,
                _dragHandler,
                _windowToPanelMap
            );

            Debug.WriteLine($"[BeepDockingManager] MDI client created: 0x{_mdiClientHwnd:X8}");
        }

        private static bool IsWinFormsDesignerProcess()
        {
            try
            {
                string processName = Process.GetCurrentProcess().ProcessName;
                return processName.IndexOf("DesignToolsServer", StringComparison.OrdinalIgnoreCase) >= 0 ||
                       string.Equals(processName, "devenv", StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Adds a new docking panel to the manager, layout tree, and host form's Controls.
        /// Creates a visual DockPanel (Panel) and positions it via the layout engine.
        /// </summary>
        public DockPanel AddPanel(string panelKey, string title, DockPosition dockPosition, Control content)
        {
            if (string.IsNullOrWhiteSpace(panelKey))
                throw new ArgumentException("Panel key cannot be null or empty", nameof(panelKey));

            if (_panelsByKey.ContainsKey(panelKey))
                throw new InvalidOperationException($"Panel with key '{panelKey}' already exists");

            InitializeSubsystems();

            // Create the visual panel — it IS a Panel control, not a Component
            var panel = new DockPanel
            {
                Key = panelKey,
                Title = title ?? "Panel",
                DockPosition = dockPosition,
                Content = content,
                Manager = this
            };

            _panelsByKey[panelKey] = panel;
            _layoutTree.RegisterPanel(panel);
            ApplyThemeToPanel(panel);

            var group = GetOrCreateGroupAtPosition(dockPosition);
            group.AddPanel(panel);

            // Add to host form so it is visible — same as DockPanelSuite adding to DockPanel (Panel)
            if (_hostForm != null && !IsDesignHosted)
            {
                _hostForm.Controls.Add(panel);
                ApplyLayoutBounds(panel);
                panel.BringToFront();

                // Add a splitter between this panel and the rest of the client area.
                // Mirrors DockPanelSuite SplitterControl placed adjacent to each edge panel.
                var splitter = CreateSplitterFor(panel);
                if (splitter != null)
                {
                    _splitters[panelKey] = splitter;
                    _hostForm.Controls.Add(splitter);
                    splitter.BringToFront();
                }
            }

            // Register tab for interaction handling
            _tabHandler?.RegisterTab(panelKey, title ?? "Panel");

            _layoutController.InvalidateLayout();
            OnPanelAdded(panel);

            Debug.WriteLine($"[BeepDockingManager] Panel added: {panelKey}");
            return panel;
        }

        /// <summary>
        /// Registers a DockPanel that was created by the WinForms designer and
        /// already exists on the host form. This is the design-time equivalent of
        /// <see cref="AddPanel"/> without creating a second control instance.
        /// </summary>
        internal bool RegisterExistingPanel(DockPanel panel)
        {
            if (panel == null || IsDesignHosted || _disposed)
                return false;

            if (string.IsNullOrWhiteSpace(panel.Key))
                return false;

            InitializeSubsystems();

            if (_panelsByKey.TryGetValue(panel.Key, out var existing))
                return ReferenceEquals(existing, panel);

            _panelsByKey[panel.Key] = panel;
            ApplyThemeToPanel(panel);

            if (_layoutTree.GetPanel(panel.Key) == null)
                _layoutTree.RegisterPanel(panel);

            var group = GetOrCreateGroupAtPosition(panel.DockPosition);
            group.AddPanel(panel);

            if (_hostForm != null)
            {
                if (panel.Parent == null)
                    _hostForm.Controls.Add(panel);

                if (panel.Parent == _hostForm)
                {
                    ApplyLayoutBounds(panel);
                    panel.BringToFront();
                    EnsureSplitterFor(panel);
                }
            }

            _tabHandler?.RegisterTab(panel.Key, panel.Title ?? "Panel");
            _layoutController?.InvalidateLayout();
            OnPanelAdded(panel);

            Debug.WriteLine($"[BeepDockingManager] Existing designer panel registered: {panel.Key}");
            return true;
        }

        /// <summary>
        /// Unregisters a designer-created DockPanel without disposing the control.
        /// Used when a panel's Manager property is changed in generated code or at runtime.
        /// </summary>
        internal void UnregisterExistingPanel(DockPanel panel)
        {
            if (panel == null || string.IsNullOrWhiteSpace(panel.Key))
                return;

            if (!_panelsByKey.TryGetValue(panel.Key, out var existing) || !ReferenceEquals(existing, panel))
                return;

            panel.Group?.RemovePanel(panel);
            _tabHandler?.UnregisterTab(panel.Key);

            if (_splitters.TryGetValue(panel.Key, out var splitter))
            {
                if (_hostForm != null && _hostForm.Controls.Contains(splitter))
                    _hostForm.Controls.Remove(splitter);

                splitter.Dispose();
                _splitters.Remove(panel.Key);
            }

            _panelsByKey.Remove(panel.Key);
            _layoutTree.UnregisterPanel(panel.Key);
            _layoutController?.InvalidateLayout();
            OnPanelRemoved(panel);

            Debug.WriteLine($"[BeepDockingManager] Existing designer panel unregistered: {panel.Key}");
        }

        internal void NotifyPanelDockPositionChanged(DockPanel panel, DockPosition oldPosition)
        {
            if (panel == null || string.IsNullOrWhiteSpace(panel.Key))
                return;

            if (!_panelsByKey.TryGetValue(panel.Key, out var existing) || !ReferenceEquals(existing, panel))
                return;

            panel.Group?.RemovePanel(panel);
            var group = GetOrCreateGroupAtPosition(panel.DockPosition);
            group.AddPanel(panel);

            if (_splitters.TryGetValue(panel.Key, out var splitter))
            {
                if (_hostForm != null && _hostForm.Controls.Contains(splitter))
                    _hostForm.Controls.Remove(splitter);

                splitter.Dispose();
                _splitters.Remove(panel.Key);
            }

            if (_hostForm != null && panel.Parent == _hostForm)
            {
                ApplyLayoutBounds(panel);
                EnsureSplitterFor(panel);
            }

            _layoutController?.InvalidateLayout();
            RecalculateLayout();
        }

        internal void NotifyPanelTitleChanged(DockPanel panel)
        {
            if (panel == null || string.IsNullOrWhiteSpace(panel.Key))
                return;

            _tabHandler?.UpdateTabLabel(panel.Key, panel.Title ?? "Panel");
        }

        internal void NotifyPanelPreferredSizeChanged(DockPanel panel)
        {
            if (panel == null || string.IsNullOrWhiteSpace(panel.Key))
                return;

            if (!_panelsByKey.TryGetValue(panel.Key, out var existing) || !ReferenceEquals(existing, panel))
                return;

            if (_hostForm != null && panel.Parent == _hostForm)
                ApplyLayoutBounds(panel);

            _layoutController?.InvalidateLayout();
            RecalculateLayout();
        }

        private void RegisterDesignerCreatedPanels(Control root)
        {
            if (root == null || IsDesignHosted)
                return;

            foreach (var panel in EnumerateDockPanels(root).OrderBy(p => p.TabIndex).ToList())
            {
                if (ReferenceEquals(panel.Manager, this))
                    RegisterExistingPanel(panel);
            }
        }

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

        private void EnsureSplitterFor(DockPanel panel)
        {
            if (_hostForm == null || _splitters.ContainsKey(panel.Key))
                return;

            var splitter = CreateSplitterFor(panel);
            if (splitter == null)
                return;

            _splitters[panel.Key] = splitter;
            _hostForm.Controls.Add(splitter);
            splitter.BringToFront();
        }

        /// <summary>
        /// Removes a panel from the manager, layout tree, and host form Controls.
        /// </summary>
        public bool RemovePanel(string panelKey)
        {
            if (!_panelsByKey.TryGetValue(panelKey, out var panel))
                return false;

            panel.Group?.RemovePanel(panel);

            _tabHandler?.UnregisterTab(panelKey);

            // Remove from host form Controls and dispose
            if (_hostForm != null && _hostForm.Controls.Contains(panel))
                _hostForm.Controls.Remove(panel);

            // Remove and dispose the associated splitter
            if (_splitters.TryGetValue(panelKey, out var splitter))
            {
                if (_hostForm != null && _hostForm.Controls.Contains(splitter))
                    _hostForm.Controls.Remove(splitter);
                splitter.Dispose();
                _splitters.Remove(panelKey);
            }

            panel.Dispose();

            _panelsByKey.Remove(panelKey);
            _layoutTree.UnregisterPanel(panelKey);

            _layoutController.InvalidateLayout();
            OnPanelRemoved(panel);

            Debug.WriteLine($"[BeepDockingManager] Panel removed: {panelKey}");
            return true;
        }

        /// <summary>
        /// Calculates and applies bounds for a panel based on its dock position and preferred size.
        /// Called after adding a panel to the host form so it appears in the right place immediately.
        /// </summary>
        private void ApplyLayoutBounds(DockPanel panel)
        {
            if (_hostForm == null) return;

            if (panel?.Group != null && panel.Group.Panels.Count > 1)
            {
                ApplyDockGroupBounds(panel.Group);
                return;
            }

            var client = _hostForm.ClientRectangle;

            // Determine bounds based on dock position — same logic as DockPanelSuite splitter math
            Rectangle bounds;
            switch (panel.DockPosition)
            {
                case DockPosition.Left:
                    bounds = new Rectangle(0, 0, panel.PreferredWidth, client.Height);
                    break;
                case DockPosition.Right:
                    bounds = new Rectangle(client.Width - panel.PreferredWidth, 0, panel.PreferredWidth, client.Height);
                    break;
                case DockPosition.Top:
                    bounds = new Rectangle(0, 0, client.Width, panel.PreferredHeight);
                    break;
                case DockPosition.Bottom:
                    bounds = new Rectangle(0, client.Height - panel.PreferredHeight, client.Width, panel.PreferredHeight);
                    break;
                case DockPosition.Fill:
                    bounds = client;
                    break;
                default:
                    bounds = new Rectangle(100, 100, panel.PreferredWidth, panel.PreferredHeight);
                    break;
            }

            panel.Bounds = bounds;
            panel.LayoutBounds = bounds;
        }

        private void ApplyDockGroupBounds(DockGroup group)
        {
            if (_hostForm == null || group == null || group.Panels.Count == 0)
                return;

            var client = _hostForm.ClientRectangle;
            int width = group.Panels.Max(p => p.PreferredWidth);
            int height = group.Panels.Max(p => p.PreferredHeight);

            Rectangle bounds = group.Position switch
            {
                DockPosition.Left => new Rectangle(0, 0, width, client.Height),
                DockPosition.Right => new Rectangle(client.Width - width, 0, width, client.Height),
                DockPosition.Top => new Rectangle(0, 0, client.Width, height),
                DockPosition.Bottom => new Rectangle(0, client.Height - height, client.Width, height),
                DockPosition.Fill => client,
                _ => new Rectangle(100, 100, width, height)
            };

            foreach (var panel in group.Panels)
            {
                panel.Bounds = bounds;
                panel.LayoutBounds = bounds;
                panel.Invalidate();
            }

            group.ActivePanel?.BringToFront();
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
            if (panel.Group != null && panel.Group != targetGroup)
                panel.Group.RemovePanel(panel);

            targetGroup.AddPanel(panel);
            targetGroup.ActivePanel = panel;

            if (_hostForm != null && panel.Parent == _hostForm)
            {
                ApplyLayoutBounds(panel);
                panel.BringToFront();
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
        /// </summary>
        public void ActivatePanel(string panelKey)
        {
            var panel = GetPanel(panelKey);
            if (panel == null)
                throw new ArgumentException($"Panel '{panelKey}' not found", nameof(panelKey));

            if (panel.Group != null)
            {
                var oldActive = panel.Group.ActivePanel;
                panel.Group.ActivePanel = panel;

                if (oldActive != panel)
                {
                    oldActive?.OnDeactivated();
                    panel.OnActivated();

                    OnPanelActivated(panel);
                }

                foreach (var groupedPanel in panel.Group.Panels)
                    groupedPanel.Invalidate();
            }

            if (panel.Parent != null)
                panel.BringToFront();

            Debug.WriteLine($"[BeepDockingManager] Panel activated: {panelKey}");
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
        /// Sizes the MDI client to fill the host form's client area.
        /// Should be called when the host form is resized.
        /// Also recalculates layout for all panels.
        /// </summary>
        public void ResizeMdiClient()
        {
            if (_mdiClientHwnd == IntPtr.Zero)
                return;

            if (!MdiNativeApi.GetClientRect(_hostHwnd, out var clientRect))
            {
                Debug.WriteLine($"[BeepDockingManager] Failed to get host client rect");
                return;
            }

            MdiNativeApi.SetWindowPos(
                _mdiClientHwnd,
                IntPtr.Zero,
                clientRect.Left,
                clientRect.Top,
                clientRect.Width,
                clientRect.Height,
                MdiConstants.SWP_NOZORDER);

            // Convert Win32 RECT to System.Drawing.Rectangle
            var bounds = new Rectangle(clientRect.Left, clientRect.Top, clientRect.Width, clientRect.Height);

            // Update layout controller with new container bounds
            _layoutController.ContainerBounds = bounds;

            // Recalculate layout for all panels
            RecalculateLayout();

            Debug.WriteLine($"[BeepDockingManager] MDI client resized to {bounds.Width}x{bounds.Height}");
        }

        /// <summary>
        /// Recalculates layout for all panels based on current container bounds.
        /// Call this after panel add/remove or container resize.
        /// No-ops while a <see cref="BeepDockingUpdate"/> batch scope is active.
        /// </summary>
        public void RecalculateLayout()
        {
            if (_updateDepth > 0) return;
            if (_layoutController == null) return;

            var layout = _layoutController.CalculateLayout();
            Debug.WriteLine($"[BeepDockingManager] Layout recalculated: {layout.Count} panels");

            // Apply calculated layout to actual windows
            if (_positioner != null)
            {
                var allPanels = _layoutTree.GetAllPanels();
                _positioner.ApplyLayout(layout, allPanels.ToList());
            }
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

            panel.State = DockPanelState.Docked;

            if (panel.NativeHandle != IntPtr.Zero)
                MdiNativeApi.ShowWindow(panel.NativeHandle, MdiConstants.SW_SHOWNOACTIVATE);

            _layoutController.InvalidateLayout();
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

            if (panel.State == DockPanelState.Closed)
                return;

            panel.State = DockPanelState.Closed;

            if (panel.NativeHandle != IntPtr.Zero)
                MdiNativeApi.ShowWindow(panel.NativeHandle, MdiConstants.SW_HIDE);

            _layoutController.InvalidateLayout();
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

            // Preserve state before removal so ReopenPanel can restore it
            _closedPanels[panelKey] = panel;

            // Remove from active layout (same as RemovePanel but without discarding the object)
            if (panel.Group != null)
                panel.Group.RemovePanel(panel);

            _contentHosting?.UnhostContent(panelKey);
            _tabHandler?.UnregisterTab(panelKey);

            if (panel.NativeHandle != IntPtr.Zero)
            {
                _painterIntegration?.UnregisterPanelFromRendering(panel.NativeHandle);
                _mouseHandler?.UnregisterWindow(panel.NativeHandle);
                MdiNativeApi.ShowWindow(panel.NativeHandle, MdiConstants.SW_HIDE);
            }

            _panelWindowManager?.DestroyPanel(panelKey);
            _panelsByKey.Remove(panelKey);
            _layoutTree.UnregisterPanel(panelKey);

            if (panel.NativeHandle != IntPtr.Zero)
            {
                _windowToPanelMap.Remove(panel.NativeHandle);
                _panelsByHwnd.Remove(panel.NativeHandle);
                panel.NativeHandle = IntPtr.Zero;
            }

            panel.State = DockPanelState.Closed;
            _layoutController.InvalidateLayout();

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

            // Re-add using existing AddPanel path so all registrations are re-done
            var restored = AddPanel(panelKey, panel.Title, panel.DockPosition, panel.Content);
            restored.AllowedAreas = panel.AllowedAreas;
            restored.CanClose = panel.CanClose;
            restored.PreferredWidth = panel.PreferredWidth;
            restored.PreferredHeight = panel.PreferredHeight;

            OnPanelReopened(restored);

            Debug.WriteLine($"[BeepDockingManager] Panel reopened: {panelKey}");
            return restored;
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
        /// Requires panel.CanFloat (AllowedAreas.Float) to be set.
        /// </summary>
        public void FloatPanel(string panelKey)
        {
            var panel = GetPanel(panelKey);
            if (panel == null)
                throw new ArgumentException($"Panel '{panelKey}' not found", nameof(panelKey));

            if (!panel.CanFloat)
                throw new InvalidOperationException($"Panel '{panelKey}' does not allow floating");

            if (panel.State == DockPanelState.Floating)
                return;

            // Remove from its current group in the layout tree
            panel.Group?.RemovePanel(panel);
            _layoutController.InvalidateLayout();

            // Create and show the float window
            var floatWindow = new FloatWindow(panel, _hostForm);
            floatWindow.PanelRedocked += OnFloatWindowRedocked;

            // Wire drag-guide overlay — shown while the float window is moved.
            // Follows DockPanelSuite DockDragHandler: overlay tracks cursor, HitTest on move.
            EnsureGuideOverlay();
            floatWindow.Move  += (s, e) => OnFloatWindowMoved(floatWindow);
            floatWindow.FormClosed += (s, e) => HideGuideOverlay();

            floatWindow.Show(_hostForm);

            panel.State = DockPanelState.Floating;
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
            _guideOverlay.ShowOver(_hostForm);
            DockingDropTarget.HitTest(_guideOverlay, Control.MousePosition);
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
                return;

            // Dismiss the drag-guide overlay now that a drop target is confirmed.
            // Mirrors DockPanelSuite DockDragHandler.OnEndDrag().
            HideGuideOverlay();

            panel.DockPosition = position;
            panel.State        = DockPanelState.Docked;

            var group = GetOrCreateGroupAtPosition(position);
            group.AddPanel(panel);
            _layoutController.InvalidateLayout();

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

            if (panel.State == DockPanelState.AutoHidden)
                return;

            // Remove from its current docked group in the layout tree
            panel.Group?.RemovePanel(panel);

            // Hand off to the AutoHideStrip for the panel's edge —
            // the strip adds a tab button and manages the slide panel.
            // Mirrors DockPanelSuite DockContent → DockState.DockLeftAutoHide path.
            if (_autoHideStrips.TryGetValue(panel.DockPosition, out var strip))
            {
                strip.Visible = true;   // show the strip edge if it was hidden
                strip.AddPanel(panel);
            }
            else
            {
                // Fallback when strips are not yet created (design-time or ManageControl not called)
                panel.Visible = false;
            }

            panel.State = DockPanelState.AutoHidden;
            _layoutController.InvalidateLayout();
            OnPanelAutoHidden(panel);

            Debug.WriteLine($"[BeepDockingManager] Panel auto-hidden: {panelKey}");
        }

        /// <summary>
        /// Handles a FloatWindow requesting to re-dock its panel.
        /// </summary>
        private void OnFloatWindowRedocked(object sender, DockPanel panel)
        {
            if (panel == null) return;
            DockFloatingPanel(panel.Key, panel.DockPosition);
        }

        /// <summary>
        /// Gets or creates a docking group at the specified position.
        /// </summary>
        private DockGroup GetOrCreateGroupAtPosition(DockPosition position)
        {
            // Try to find existing group at this position
            var existingGroup = _layoutTree.Root.Children.FirstOrDefault(g => g.Position == position);
            if (existingGroup != null)
                return existingGroup;

            // Create new group
            var newGroup = new DockGroup
            {
                Id = $"group_{position}_{Guid.NewGuid().ToString("N").Substring(0, 8)}",
                Position = position,
                TabStyle = Models.TabStyle.Top
            };

            _layoutTree.RegisterGroup(newGroup);
            _layoutTree.Root.AddChild(newGroup);

            return newGroup;
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

            foreach (var strip in _autoHideStrips.Values)
                strip?.ApplyDockingTheme(_themeColors);

            foreach (var splitter in _splitters.Values)
                splitter?.ApplyDockingTheme(_themeColors);

            OnThemeChanged();
        }

        internal void AttachDockspace(BeepDockspace dockspace)
        {
            if (dockspace == null || !_dockspaceEventSinks.Add(dockspace))
                return;

            dockspace.PageCloseClicked += OnDockspacePageCloseClicked;
            dockspace.PageAutoHiddenClicked += OnDockspacePageAutoHiddenClicked;
            dockspace.PageDropDownClicked += OnDockspacePageDropDownClicked;
            dockspace.PagesDoubleClicked += OnDockspacePagesDoubleClicked;
        }

        internal void DetachDockspace(BeepDockspace dockspace)
        {
            if (dockspace == null || !_dockspaceEventSinks.Remove(dockspace))
                return;

            dockspace.PageCloseClicked -= OnDockspacePageCloseClicked;
            dockspace.PageAutoHiddenClicked -= OnDockspacePageAutoHiddenClicked;
            dockspace.PageDropDownClicked -= OnDockspacePageDropDownClicked;
            dockspace.PagesDoubleClicked -= OnDockspacePagesDoubleClicked;
        }

        internal void ApplyThemeToDockspace(BeepDockspace dockspace)
        {
            if (dockspace == null)
                return;

            dockspace.ApplyDockingTheme(_themeColors);
        }

        private void OnDockspacePageCloseClicked(object sender, DockspacePageEventArgs e)
        {
            if (IsDesignHosted || string.IsNullOrWhiteSpace(e.UniqueName) || GetPanel(e.UniqueName) == null)
                return;

            CloseRequest(new[] { e.UniqueName });
        }

        private void OnDockspacePageAutoHiddenClicked(object sender, DockspacePageEventArgs e)
        {
            if (IsDesignHosted || string.IsNullOrWhiteSpace(e.UniqueName) || GetPanel(e.UniqueName) == null)
                return;

            MakeAutoHiddenRequest(e.UniqueName);
        }

        private void OnDockspacePageDropDownClicked(object sender, DockspaceDropDownEventArgs e)
        {
            if (IsDesignHosted || e?.Panel == null)
                return;

            var args = new PanelContextMenuEventArgs(e.Panel, e.ScreenPosition);
            OnShowPanelContextMenu(args);

            if (args.ContextMenu == null)
                return;

            e.Cancel = true;
            args.ContextMenu.Show(e.Panel, e.Panel.PointToClient(e.ScreenPosition));
        }

        private void OnDockspacePagesDoubleClicked(object sender, DockspacePagesEventArgs e)
        {
            if (IsDesignHosted || e?.UniqueNames == null)
                return;

            foreach (string uniqueName in e.UniqueNames)
            {
                DockPanel panel = GetPanel(uniqueName);
                if (panel?.CanFloat == true)
                    MakeFloatingRequest(uniqueName);
            }
        }

        internal void ApplyThemeToPanel(DockPanel panel)
        {
            if (panel == null)
                return;

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

        /// <summary>Shows multiple panels by key in a single layout pass.</summary>
        public void ShowPanels(IReadOnlyList<string> panelKeys)
        {
            ArgumentNullException.ThrowIfNull(panelKeys);
            using var scope = new BeepDockingUpdate(this);
            foreach (var key in panelKeys)
                ShowPanel(key);
        }

        /// <summary>Hides multiple panels by key in a single layout pass.</summary>
        public void HidePanels(IReadOnlyList<string> panelKeys)
        {
            ArgumentNullException.ThrowIfNull(panelKeys);
            using var scope = new BeepDockingUpdate(this);
            foreach (var key in panelKeys)
                HidePanel(key);
        }

        /// <summary>Shows all panels in a single layout pass.</summary>
        public void ShowAllPanels()
        {
            using var scope = new BeepDockingUpdate(this);
            foreach (var key in _panelsByKey.Keys.ToList())
                ShowPanel(key);
        }

        /// <summary>Hides all panels in a single layout pass.</summary>
        public void HideAllPanels()
        {
            using var scope = new BeepDockingUpdate(this);
            foreach (var key in _panelsByKey.Keys.ToList())
                HidePanel(key);
        }

        // ── Store / restore (mirrors Krypton StorePage / ClearStoredPage) ─────────────

        /// <summary>
        /// Closes a panel and stores it so it can be restored later.
        /// Equivalent to Krypton's <c>StorePage</c> — the panel is not disposed.
        /// </summary>
        public void StorePanel(string panelKey) => ClosePanel(panelKey);

        /// <summary>
        /// Restores a previously stored panel.  Equivalent to Krypton's <c>ClearStoredPage</c>
        /// combined with re-adding the page.
        /// </summary>
        public void RestoreStoredPanel(string panelKey) => ReopenPanel(panelKey);

        /// <summary>Restores all stored panels in a single layout pass.</summary>
        public void RestoreAllStoredPanels()
        {
            using var scope = new BeepDockingUpdate(this);
            foreach (var key in _closedPanels.Keys.ToList())
                ReopenPanel(key);
        }

        /// <summary>Closes all live panels into the closed store in a single layout pass.</summary>
        public void StoreAllPanels()
        {
            using var scope = new BeepDockingUpdate(this);
            foreach (var key in _panelsByKey.Keys.ToList())
                ClosePanel(key);
        }

        // ── Lookup / state helpers (mirrors Krypton Contains/FindPageLocation) ────────

        /// <summary>Returns true if the panel key exists in the live registry.</summary>
        public bool ContainsPanel(string panelKey)
        {
            if (string.IsNullOrWhiteSpace(panelKey))
                throw new ArgumentNullException(nameof(panelKey));
            return _panelsByKey.ContainsKey(panelKey);
        }

        /// <summary>
        /// Returns the current <see cref="DockPanelState"/> of the named panel,
        /// or <see cref="DockPanelState.Closed"/> if not found.
        /// Mirrors Krypton's <c>FindPageLocation</c>.
        /// </summary>
        public DockPanelState FindPanelLocation(string panelKey)
        {
            if (string.IsNullOrWhiteSpace(panelKey))
                throw new ArgumentNullException(nameof(panelKey));

            if (_panelsByKey.TryGetValue(panelKey, out var panel))
                return panel.State;

            if (_closedPanels.ContainsKey(panelKey))
                return DockPanelState.Closed;

            return DockPanelState.Closed;
        }

        // ── Cancel-able request entry points (mirrors Krypton Make*Request) ──────────

        /// <summary>
        /// Raises <see cref="PageDockedRequest"/> and, unless cancelled, docks the panel.
        /// Mirrors Krypton's <c>MakeDockedRequest</c>.
        /// </summary>
        public virtual void MakeDockedRequest(string panelKey)
        {
            var panel = GetPanel(panelKey);
            var args = new CancelPanelRequestEventArgs(panelKey, panel);
            OnPageDockedRequest(args);
            if (args.Cancel) return;

            if (panel?.State == DockPanelState.Floating)
                DockFloatingPanel(panelKey, panel.DockPosition);
        }

        /// <summary>
        /// Raises <see cref="PageFloatingRequest"/> and, unless cancelled, floats the panel.
        /// Mirrors Krypton's <c>MakeFloatingRequest</c>.
        /// </summary>
        public virtual void MakeFloatingRequest(string panelKey)
        {
            var panel = GetPanel(panelKey);
            var args = new CancelPanelRequestEventArgs(panelKey, panel);
            OnPageFloatingRequest(args);
            if (args.Cancel) return;
            FloatPanel(panelKey);
        }

        /// <summary>
        /// Raises <see cref="PageAutoHiddenRequest"/> and, unless cancelled, auto-hides the panel.
        /// Mirrors Krypton's <c>MakeAutoHiddenRequest</c>.
        /// </summary>
        public virtual void MakeAutoHiddenRequest(string panelKey)
        {
            var panel = GetPanel(panelKey);
            var args = new CancelPanelRequestEventArgs(panelKey, panel);
            OnPageAutoHiddenRequest(args);
            if (args.Cancel) return;
            AutoHidePanel(panelKey);
        }

        /// <summary>
        /// Raises <see cref="PageCloseRequest"/> and, unless cancelled, closes the panel.
        /// Mirrors Krypton's <c>CloseRequest</c>.
        /// </summary>
        public virtual void CloseRequest(IReadOnlyList<string> panelKeys)
        {
            ArgumentNullException.ThrowIfNull(panelKeys);
            using var scope = new BeepDockingUpdate(this);
            foreach (var key in panelKeys.ToList())
            {
                var panel = GetPanel(key);
                var args = new PanelCloseRequestEventArgs(key, panel);
                OnPageCloseRequest(args);
                if (!args.Cancel)
                    ClosePanel(key);
            }
        }

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

        /// <summary>
        /// Gets a diagnostic summary of the manager state.
        /// </summary>
        public string GetDiagnostics()
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("=== BeepDockingManager Diagnostics ===");
            sb.AppendLine($"Host Form: {_hostForm?.Name ?? "(null)"}");
            sb.AppendLine($"Host HWND: 0x{_hostHwnd:X8}");
            sb.AppendLine($"MDI Client HWND: 0x{_mdiClientHwnd:X8}");
            sb.AppendLine($"Panels: {PanelCount}");
            sb.AppendLine();
            sb.Append(_layoutTree.GetDiagnostics());
            return sb.ToString();
        }

        // ── Protected virtual OnXxx raise methods (Krypton pattern) ─────────────
        // Subclasses override these to intercept or suppress events.

        /// <summary>Raises the <see cref="PanelActivated"/> event.</summary>
        protected virtual void OnPanelActivated(DockPanel panel) =>
            PanelActivated?.Invoke(this, panel);

        /// <summary>Raises the <see cref="PanelAdded"/> event.</summary>
        protected virtual void OnPanelAdded(DockPanel panel) =>
            PanelAdded?.Invoke(this, panel);

        /// <summary>Raises the <see cref="PanelRemoved"/> event.</summary>
        protected virtual void OnPanelRemoved(DockPanel panel) =>
            PanelRemoved?.Invoke(this, panel);

        /// <summary>Raises the <see cref="ThemeChanged"/> event.</summary>
        protected virtual void OnThemeChangedRaised() =>
            ThemeChanged?.Invoke(this, EventArgs.Empty);

        /// <summary>Raises the <see cref="PanelFloated"/> event.</summary>
        protected virtual void OnPanelFloated(DockPanel panel) =>
            PanelFloated?.Invoke(this, panel);

        /// <summary>Raises the <see cref="PanelAutoHidden"/> event.</summary>
        protected virtual void OnPanelAutoHidden(DockPanel panel) =>
            PanelAutoHidden?.Invoke(this, panel);

        /// <summary>Raises the <see cref="PanelHidden"/> event.</summary>
        protected virtual void OnPanelHidden(DockPanel panel) =>
            PanelHidden?.Invoke(this, panel);

        /// <summary>Raises the <see cref="PanelShown"/> event.</summary>
        protected virtual void OnPanelShown(DockPanel panel) =>
            PanelShown?.Invoke(this, panel);

        /// <summary>Raises the <see cref="PanelClosed"/> event.</summary>
        protected virtual void OnPanelClosed(DockPanel panel) =>
            PanelClosed?.Invoke(this, panel);

        /// <summary>Raises the <see cref="PanelReopened"/> event.</summary>
        protected virtual void OnPanelReopened(DockPanel panel) =>
            PanelReopened?.Invoke(this, panel);

        /// <summary>Raises the <see cref="PageCloseRequest"/> event.</summary>
        protected virtual void OnPageCloseRequest(PanelCloseRequestEventArgs e) =>
            PageCloseRequest?.Invoke(this, e);

        /// <summary>Raises the <see cref="PageDockedRequest"/> event.</summary>
        protected virtual void OnPageDockedRequest(CancelPanelRequestEventArgs e) =>
            PageDockedRequest?.Invoke(this, e);

        /// <summary>Raises the <see cref="PageAutoHiddenRequest"/> event.</summary>
        protected virtual void OnPageAutoHiddenRequest(CancelPanelRequestEventArgs e) =>
            PageAutoHiddenRequest?.Invoke(this, e);

        /// <summary>Raises the <see cref="PageFloatingRequest"/> event.</summary>
        protected virtual void OnPageFloatingRequest(CancelPanelRequestEventArgs e) =>
            PageFloatingRequest?.Invoke(this, e);

        /// <summary>Raises the <see cref="ShowPanelContextMenu"/> event.</summary>
        protected virtual void OnShowPanelContextMenu(PanelContextMenuEventArgs e) =>
            ShowPanelContextMenu?.Invoke(this, e);

        /// <summary>Raises the <see cref="FloatingWindowAdding"/> event.</summary>
        protected virtual void OnFloatingWindowAdding(FloatingWindowEventArgs e) =>
            FloatingWindowAdding?.Invoke(this, e);

        /// <summary>Raises the <see cref="FloatingWindowRemoved"/> event.</summary>
        protected virtual void OnFloatingWindowRemoved(FloatingWindowEventArgs e) =>
            FloatingWindowRemoved?.Invoke(this, e);

        /// <summary>Raises the <see cref="AutoHiddenGroupAdding"/> event.</summary>
        protected virtual void OnAutoHiddenGroupAdding(AutoHiddenGroupEventArgs e) =>
            AutoHiddenGroupAdding?.Invoke(this, e);

        /// <summary>Raises the <see cref="AutoHiddenGroupRemoved"/> event.</summary>
        protected virtual void OnAutoHiddenGroupRemoved(AutoHiddenGroupEventArgs e) =>
            AutoHiddenGroupRemoved?.Invoke(this, e);

        /// <summary>Raises the <see cref="DockspaceAdding"/> event.</summary>
        protected virtual void OnDockspaceAdding(DockspaceEventArgs e) =>
            DockspaceAdding?.Invoke(this, e);

        /// <summary>Raises the <see cref="DockspaceRemoved"/> event.</summary>
        protected virtual void OnDockspaceRemoved(DockspaceEventArgs e) =>
            DockspaceRemoved?.Invoke(this, e);

        /// <summary>Raises the <see cref="DockspaceSeparatorResize"/> event.</summary>
        protected virtual void OnDockspaceSeparatorResize(SeparatorResizeEventArgs e) =>
            DockspaceSeparatorResize?.Invoke(this, e);

        /// <summary>Raises the <see cref="AutoHiddenSeparatorResize"/> event.</summary>
        protected virtual void OnAutoHiddenSeparatorResize(SeparatorResizeEventArgs e) =>
            AutoHiddenSeparatorResize?.Invoke(this, e);

        /// <summary>Raises the <see cref="DoDragDropEnd"/> event.</summary>
        protected virtual void OnDoDragDropEnd() =>
            DoDragDropEnd?.Invoke(this, EventArgs.Empty);

        /// <summary>Raises the <see cref="DoDragDropQuit"/> event.</summary>
        protected virtual void OnDoDragDropQuit() =>
            DoDragDropQuit?.Invoke(this, EventArgs.Empty);

        /// <summary>
        /// Disposes the manager and cleans up all resources.
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
                return;

            // Destroy MDI client if it exists
            if (_mdiClientHwnd != IntPtr.Zero)
            {
                MdiNativeApi.DestroyWindow(_mdiClientHwnd);
                _mdiClientHwnd = IntPtr.Zero;
            }

            foreach (BeepDockspace dockspace in _dockspaceEventSinks.ToArray())
                DetachDockspace(dockspace);
            _dockspaceEventSinks.Clear();

            _panelsByKey.Clear();
            _panelsByHwnd.Clear();
            _windowToPanelMap.Clear();
            _layoutTree.Clear();

            foreach (var strip in _autoHideStrips.Values)
                strip?.Dispose();
            _autoHideStrips.Clear();

            _guideOverlay?.Dispose();
            _guideOverlay = null;

            foreach (var sp in _splitters.Values)
                sp?.Dispose();
            _splitters.Clear();

            // Dispose Phase 4 components
            _dragHandler?.Dispose();
            _dragHandler = null;

            _contentHosting?.Dispose();
            _contentHosting = null;

            _tabHandler?.Dispose();
            _tabHandler = null;

            _painterIntegration?.Dispose();
            _painterIntegration = null;

            _mouseHandler = null;

            _eventInterceptor?.Dispose();
            _eventInterceptor = null;

            // Dispose Phase 3 and earlier components
            if (!ReferenceEquals(_painter, NullDockingPainter.Instance))
                _painter.Dispose();
            _painter = NullDockingPainter.Instance;
            _layoutController = null;
            _positioner = null;
            _chrome?.Dispose();
            _chrome = null;
            _panelWindowManager = null;
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
