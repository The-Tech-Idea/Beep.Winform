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
            IsWinFormsDesignerProcess();

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
        /// Re-applies the active theme + style to every docking surface, including float windows
        /// and any other transient chrome that <see cref="ApplyTheme()"/> may not reach when
        /// the host application's <c>IBeepTheme</c> changes at runtime. Use this from a theme
        /// change handler when you want a single call to refresh all managed surfaces.
        /// </summary>
        public void RefreshTheme()
        {
            try
            {
                // 1. Rebuild palette from the current theme and walk every managed surface.
                ApplyTheme();

                // 2. Push style + repaint to surfaces not covered by ApplyTheme().
                PropagateControlStyle();

                // 3. Float windows live outside the host form's control tree; push to each.
                foreach (var fw in _floatWindowsByKey.Values)
                {
                    if (fw == null || fw.IsDisposed) continue;
                    fw.ControlStyle = _style;
                    fw.ApplyDockingTheme(_themeColors);
                }

                // 4. The navigator popup, when open, reads the cached theme colours directly.
                if (_navigator != null && !_navigator.IsDisposed)
                    _navigator.RefreshTheme(_themeColors);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[BeepDockingManager] RefreshTheme error: {ex.Message}");
            }
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
            _tabHandler ??= new TabInteractionHandler(ActivatePanel, _layoutTree);
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

            DetachHostFormHandlers();
            _hostForm = hostForm;

            if (IsDesignHosted)
            {
                Debug.WriteLine("[BeepDockingManager] ManageControl — design-time, Win32 deferred.");
                return;
            }

            AttachHostFormHandlers(hostForm);
            InitializeSubsystems();
            _layoutController.ContainerBounds = hostForm.DisplayRectangle;
            CreateAutoHideStrips();
            RegisterDesignerCreatedPanels(hostForm);

            // Apply a layout deserialized from the host *.Designer.cs (populated into the backing
            // LayoutDefinition during InitializeComponent) now that the panels are registered.
            if (_layoutDefinition != null && !_layoutDefinition.IsEmpty)
            {
                MaterializeFromDefinition(_layoutDefinition);
            }
            else
            {
                RecalculateLayout();
            }

            Debug.WriteLine($"[BeepDockingManager] ManageControl — attached to: {hostForm.Name}");
        }

        /// <summary>Subscribes to host resize so the layout engine tracks the client area.</summary>
        private void AttachHostFormHandlers(Form hostForm)
        {
            if (hostForm == null)
                return;

            _hostLayoutChangedHandler ??= OnHostFormLayoutChanged;
            hostForm.Resize += _hostLayoutChangedHandler;
            hostForm.ClientSizeChanged += _hostLayoutChangedHandler;

            _hostKeyDownHandler ??= OnHostFormKeyDown;
            _hostKeyUpHandler ??= OnHostFormKeyUp;
            hostForm.KeyPreview = true;
            hostForm.KeyDown += _hostKeyDownHandler;
            hostForm.KeyUp += _hostKeyUpHandler;
        }

        private void DetachHostFormHandlers()
        {
            if (_hostForm == null)
                return;

            if (_hostLayoutChangedHandler != null)
            {
                _hostForm.Resize -= _hostLayoutChangedHandler;
                _hostForm.ClientSizeChanged -= _hostLayoutChangedHandler;
            }

            if (_hostKeyDownHandler != null)
            {
                _hostForm.KeyDown -= _hostKeyDownHandler;
            }

            if (_hostKeyUpHandler != null)
            {
                _hostForm.KeyUp -= _hostKeyUpHandler;
            }
        }

        private void OnHostFormLayoutChanged(object sender, EventArgs e)
        {
            if (_hostForm == null || _hostForm.IsDisposed || _layoutController == null || _disposed || IsDesignHosted)
                return;

            _layoutController.ContainerBounds = GetLayoutClientBounds(_hostForm.DisplayRectangle);

            if (_updateDepth > 0)
                return;

            ApplyLayout();
        }

        /// <summary>
        /// Detaches a panel from float/auto-hide chrome and re-parents it onto the host form
        /// so the layout engine can position it.
        /// </summary>
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
        }

        private void CloseFloatWindowFor(DockPanel panel)
        {
            if (panel == null || string.IsNullOrWhiteSpace(panel.Key))
                return;

            if (_floatWindowsByKey.TryGetValue(panel.Key, out var tracked) && tracked != null && !tracked.IsDisposed)
            {
                tracked.PanelRedocked -= OnFloatWindowRedocked;
                tracked.ExtractHostedPanel();
                OnFloatingWindowRemoved(new FloatingWindowEventArgs(tracked, panel));
                tracked.Close();
                _floatWindowsByKey.Remove(panel.Key);
                return;
            }

            if (_hostForm == null)
                return;

            foreach (Form owned in _hostForm.OwnedForms)
            {
                if (owned is FloatWindow fw && ReferenceEquals(fw.Panel, panel))
                {
                    fw.PanelRedocked -= OnFloatWindowRedocked;
                    fw.ExtractHostedPanel();
                    OnFloatingWindowRemoved(new FloatingWindowEventArgs(fw, panel));
                    fw.Close();
                    _floatWindowsByKey.Remove(panel.Key);
                    break;
                }
            }
        }

        private void CloseAllFloatWindows()
        {
            foreach (var fw in _floatWindowsByKey.Values.ToList())
            {
                if (fw == null || fw.IsDisposed)
                    continue;

                fw.PanelRedocked -= OnFloatWindowRedocked;
                fw.ExtractHostedPanel();
                fw.Close();
            }

            _floatWindowsByKey.Clear();
        }

        private void ClearAllAutoHidePanels()
        {
            foreach (var strip in _autoHideStrips.Values)
            {
                if (strip == null)
                    continue;

                foreach (var panel in strip.Panels.ToList())
                    strip.RemovePanel(panel);

                strip.Visible = false;
            }
        }

        private void DetachFromAutoHideStrip(DockPanel panel)
        {
            if (panel == null)
                return;

            foreach (var kv in _autoHideStrips)
            {
                var strip = kv.Value;
                if (strip == null || !strip.Panels.Contains(panel))
                    continue;

                strip.RemovePanel(panel);
                if (strip.Panels.Count == 0)
                    strip.Visible = false;

                OnAutoHiddenGroupRemoved(new AutoHiddenGroupEventArgs(panel, kv.Key));
                break;
            }

            if (panel.Parent != null && panel.Parent != _hostForm)
                panel.Parent.Controls.Remove(panel);
        }

        /// <summary>
        /// Reconciles the live <see cref="BeepDockSplitter"/> controls with the splitter
        /// rectangles produced by the layout engine.
        ///
        /// Two kinds of splitters:
        /// <list type="bullet">
        ///   <item><b>Root-level edge splitters</b> (GroupId matches a root-level edge group's id)
        ///   stay on the host form, positioned in container coordinates. They control the size
        ///   of a dockspace docked at a form edge.</item>
        ///   <item><b>Child splitters</b> (GroupId has <c>_child_</c> in it, format
        ///   <c>{parentId}_child_{i}</c>) are owned by the dockspace that hosts the parent group.
        ///   Their bounds are translated to dockspace-local coordinates and the splitter is
        ///   parented to that dockspace, so it floats over the dockspace's child panels.</item>
        /// </list>
        ///
        /// Orphaned splitters (no longer in the result) are disposed in both populations.
        /// </summary>
        private void SyncSplitters(DockLayoutResult result)
        {
            if (_hostForm == null || IsDesignHosted || result == null)
                return;

            // 1) Index managed dockspaces by their primary group's id so we can route child
            //    splitters to the right owner. A dockspace's primary group is the group of any
            //    one of its child panels.
            var dockspaceByGroupId = new Dictionary<string, BeepDockspace>(StringComparer.Ordinal);
            foreach (var dockspace in GetManagedDockspaces())
            {
                if (dockspace == null || dockspace.IsDisposed)
                    continue;

                foreach (var panel in dockspace.Panels)
                {
                    var groupId = panel?.Group?.Id;
                    if (!string.IsNullOrEmpty(groupId) && !dockspaceByGroupId.ContainsKey(groupId))
                        dockspaceByGroupId[groupId] = dockspace;
                }
            }

            // 2) Bucket each hit into root-level (host form) or child (dockspace-owned).
            var desiredRoot = new HashSet<string>(StringComparer.Ordinal);
            var desiredChildByDockspace = new Dictionary<BeepDockspace, Dictionary<string, (Rectangle Bounds, bool IsVertical)>>();

            foreach (var hit in result.Splitters)
            {
                if (string.IsNullOrEmpty(hit.GroupId))
                    continue;

                int childMarker = hit.GroupId.IndexOf("_child_", StringComparison.Ordinal);
                if (childMarker > 0)
                {
                    string parentId = hit.GroupId.Substring(0, childMarker);
                    if (!dockspaceByGroupId.TryGetValue(parentId, out var dockspace) || dockspace == null)
                        continue;

                    // Translate the engine's container-coordinate bounds into the dockspace's
                    // local client coords.
                    var local = new Rectangle(
                        hit.Bounds.X - dockspace.Bounds.X,
                        hit.Bounds.Y - dockspace.Bounds.Y,
                        hit.Bounds.Width,
                        hit.Bounds.Height);

                    if (!desiredChildByDockspace.TryGetValue(dockspace, out var map))
                        desiredChildByDockspace[dockspace] = map = new Dictionary<string, (Rectangle, bool)>(StringComparer.Ordinal);
                    map[hit.GroupId] = (local, hit.IsVertical);
                    continue;
                }

                desiredRoot.Add(hit.GroupId);

                if (!_splitters.TryGetValue(hit.GroupId, out var splitter) || splitter == null || splitter.IsDisposed)
                {
                    splitter = new BeepDockSplitter { GroupId = hit.GroupId };
                    splitter.ControlStyle = _style;
                    splitter.ApplyDockingTheme(_themeColors);
                    splitter.SplitterMoved += OnEngineSplitterMoved;
                    _splitters[hit.GroupId] = splitter;
                    _hostForm.Controls.Add(splitter);
                }

                splitter.Orientation = hit.IsVertical ? SplitterOrientation.Vertical : SplitterOrientation.Horizontal;
                splitter.Bounds = hit.Bounds;
                splitter.Visible = true;
                splitter.BringToFront();
            }

            // 3) Reconcile dockspace-owned child splitters.
            foreach (var ds in GetManagedDockspaces())
            {
                if (ds == null || ds.IsDisposed)
                    continue;
                if (desiredChildByDockspace.TryGetValue(ds, out var map))
                    ds.UpdateChildSplitters(map, OnEngineSplitterMoved);
                else
                    ds.ClearChildSplitters();
            }

            // 4) Dispose root-level orphans.
            var orphans = _splitters.Keys.Where(k => !desiredRoot.Contains(k)).ToList();
            foreach (var key in orphans)
            {
                var splitter = _splitters[key];
                if (splitter != null)
                {
                    splitter.SplitterMoved -= OnEngineSplitterMoved;
                    if (_hostForm.Controls.Contains(splitter))
                        _hostForm.Controls.Remove(splitter);
                    splitter.Dispose();
                }
                _splitters.Remove(key);
            }
        }

        /// <summary>
        /// Handles a live splitter drag: converts the pixel delta into an edge-group ratio
        /// via the layout engine, then re-applies the whole layout.
        /// </summary>
        private void OnEngineSplitterMoved(object sender, SplitterMovedEventArgs e)
        {
            if (sender is BeepDockSplitter splitter && !string.IsNullOrEmpty(splitter.GroupId))
            {
                _layoutController?.DragSplitter(splitter.GroupId, e.Delta);
                ApplyLayout();

                var group = _layoutTree.GetGroup(splitter.GroupId);
                var panel = group?.ActivePanel;
                if (panel == null || panel.State != DockPanelState.Docked)
                    panel = group?.Panels.FirstOrDefault(p => p.State == DockPanelState.Docked);

                OnDockspaceSeparatorResize(new SeparatorResizeEventArgs(panel, splitter.Bounds));
            }
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
                strip.ControlStyle = _style;
                strip.ApplyDockingTheme(_themeColors);
                strip.Visible = false;   // only show when a panel is auto-hidden on this edge
                strip.PanelRestoreRequested += OnStripRestoreRequested;
                strip.PanelCloseRequested += OnStripCloseRequested;
                strip.PanelFloatRequested += OnStripFloatRequested;
                strip.SlideShown += OnStripSlideShown;
                strip.SlideLayoutChanged += (_, __) => RecalculateLayout();
                strip.SlideSeparatorResized += (_, e) =>
                {
                    OnAutoHiddenSeparatorResize(e);
                    RecalculateLayout();
                };
                _autoHideStrips[edge] = strip;
            }
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
        /// Adds a new docking panel to the manager, layout tree, and a matching
        /// <see cref="BeepDockspace"/> on the host form. The dockspace is the persistent runtime
        /// view that hosts the panel (same model as designer-created panels). If no matching
        /// dockspace exists yet on the host form, the panel is hosted directly on the host form
        /// (legacy path) and a warning is written to the debug log.
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

            if (_hostForm != null && !IsDesignHosted)
            {
                // Preferred path: place the panel inside a matching BeepDockspace, exactly like
                // designer-created panels. The dockspace's LayoutPanels / OnLayout will size it.
                var dockspace = FindOrCreateDockspaceAt(_hostForm, dockPosition);
                if (dockspace != null)
                {
                    if (panel.Parent != dockspace)
                    {
                        panel.Parent?.Controls.Remove(panel);
                        dockspace.Controls.Add(panel);
                    }
                    panel.ShowCaption = true;
                    panel.Visible = true;
                    dockspace.LayoutPanels();
                    dockspace.Invalidate();
                }
                else
                {
                    // Legacy fallback: no dockspace on the host yet. Add to host form and let the
                    // layout engine position the panel. This path is rare (dockspaces are normally
                    // designer-created); we keep it so callers without a dockspace still work.
                    Debug.WriteLine($"[BeepDockingManager] AddPanel('{panelKey}'): no BeepDockspace for {dockPosition}; falling back to host-form hosting.");
                    _hostForm.Controls.Add(panel);
                    panel.BringToFront();
                }

                // Position panels + create/place edge splitters via the layout engine.
                ApplyLayout();
            }

            // Register tab for interaction handling
            _tabHandler?.RegisterTab(panelKey, title ?? "Panel");

            _layoutController?.InvalidateLayout();
            OnPanelAdded(panel);

            Debug.WriteLine($"[BeepDockingManager] Panel added: {panelKey}");
            return panel;
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

            // The designer has already placed this panel inside its dockspace (or directly on
            // the host form for legacy paths). We do NOT reparent — the dockspace's
            // LayoutPanels() / OnLayout is the authoritative layout for its child panels.
            if (panel.Parent is BeepDockspace dockspace)
            {
                dockspace.LayoutPanels();
                dockspace.Invalidate();
            }
            else if (_hostForm != null && panel.Parent == _hostForm)
            {
                panel.BringToFront();
                ApplyLayout();
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

            if (panel.State == DockPanelState.Floating)
                CloseFloatWindowFor(panel);
            else if (panel.State == DockPanelState.AutoHidden)
                DetachFromAutoHideStrip(panel);

            panel.Group?.RemovePanel(panel);
            _tabHandler?.UnregisterTab(panel.Key);

            _panelsByKey.Remove(panel.Key);
            _layoutTree.UnregisterPanel(panel.Key);
            _layoutController?.InvalidateLayout();
            // Reflow so the orphaned edge splitter (keyed by group id) is disposed by SyncSplitters.
            RecalculateLayout();
            OnPanelRemoved(panel);

            Debug.WriteLine($"[BeepDockingManager] Existing designer panel unregistered: {panel.Key}");
        }

        internal void NotifyPanelDockPositionChanged(DockPanel panel, DockPosition oldPosition)
        {
            if (panel == null || string.IsNullOrWhiteSpace(panel.Key))
                return;

            if (!_panelsByKey.TryGetValue(panel.Key, out var existing) || !ReferenceEquals(existing, panel))
                return;

            // Position metadata for floating/auto-hidden panels is applied when they re-dock.
            if (panel.State != DockPanelState.Docked && panel.State != DockPanelState.Hidden)
                return;

            panel.Group?.RemovePanel(panel);
            var group = GetOrCreateGroupAtPosition(panel.DockPosition);
            group.AddPanel(panel);

            if (_hostForm != null && panel.Parent == _hostForm)
                panel.BringToFront();

            _layoutController?.InvalidateLayout();
            RecalculateLayout();   // engine repositions panels + reconciles splitters
        }

        internal void NotifyPanelTitleChanged(DockPanel panel)
        {
            if (panel == null || string.IsNullOrWhiteSpace(panel.Key))
                return;

            _tabHandler?.UpdateTabLabel(panel.Key, panel.Title ?? "Panel");

            // Repaint the parent dockspace header so the tab label updates immediately.
            if (panel.Parent is BeepDockspace dockspace)
                dockspace.Invalidate();

            // Update float window caption if the panel is floating.
            if (_floatWindowsByKey.TryGetValue(panel.Key, out var fw) && fw != null && !fw.IsDisposed)
                fw.Text = panel.Title ?? panel.Key;
        }

        internal void NotifyPanelPreferredSizeChanged(DockPanel panel)
        {
            if (panel == null || string.IsNullOrWhiteSpace(panel.Key))
                return;

            if (!_panelsByKey.TryGetValue(panel.Key, out var existing) || !ReferenceEquals(existing, panel))
                return;

            if (_hostForm != null && panel.Parent == _hostForm)
                panel.Invalidate();

            _layoutController?.InvalidateLayout();
            RecalculateLayout();
        }

        private void RegisterDesignerCreatedPanels(Control root)
        {
            if (root == null || IsDesignHosted)
                return;

            // The designer (and any host-form code) has already placed DockPanel children inside
            // BeepDockspace containers. We respect that structure: the dockspace stays in
            // hostForm.Controls and arranges its own child panels. We just walk every panel we
            // can find and add it to the layout tree.
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

        /// <summary>
        /// Removes a panel from the manager, layout tree, and host form Controls.
        /// </summary>
        public bool RemovePanel(string panelKey)
        {
            if (!_panelsByKey.TryGetValue(panelKey, out var panel))
                return false;

            if (panel.State == DockPanelState.Floating)
                CloseFloatWindowFor(panel);
            else if (panel.State == DockPanelState.AutoHidden)
                DetachFromAutoHideStrip(panel);

            panel.Group?.RemovePanel(panel);

            _tabHandler?.UnregisterTab(panelKey);

            // Remove from host form Controls and dispose
            if (_hostForm != null && _hostForm.Controls.Contains(panel))
                _hostForm.Controls.Remove(panel);

            panel.Dispose();

            _panelsByKey.Remove(panelKey);
            _layoutTree.UnregisterPanel(panelKey);
            RemoveMrPanel(panelKey);

            _layoutController?.InvalidateLayout();
            // Reflow so remaining panels reclaim the freed space and SyncSplitters disposes
            // any now-orphaned edge splitter (splitters are keyed by group id, not panel key).
            RecalculateLayout();
            OnPanelRemoved(panel);

            Debug.WriteLine($"[BeepDockingManager] Panel removed: {panelKey}");
            return true;
        }

        /// <summary>
        /// Single layout pass: seeds edge-group ratios from preferred sizes (first time only),
        /// runs the <see cref="DockingLayoutController"/> against the host client area, then
        /// positions every docked panel and reconciles the edge splitters from the result.
        /// This is the sole positioning path — all callers route through here.
        ///
        /// Panels that live inside a <see cref="BeepDockspace"/> are positioned by the dockspace
        /// itself (its <c>LayoutPanels</c> runs in <c>OnLayout</c>); we just call PerformLayout
        /// on each dockspace. Panels that live directly on the host form (legacy paths) get
        /// bounds from the layout controller result, as before.
        /// </summary>
        public void ApplyLayout()
        {
            if (_hostForm == null || IsDesignHosted || _disposed || _layoutController == null)
                return;

            PruneEmptyRootGroups();

            var client = GetLayoutClientBounds(_hostForm.DisplayRectangle);
            _layoutController.ContainerBounds = client;
            SeedEdgeRatios(client);

            var result = _layoutController.CalculateLayout(client);

            _hostForm.SuspendLayout();
            try
            {
                // 1) Position dockspaces on the host form via DockStyle. The WinForms layout
                //    engine will then handle their actual Bounds in its own layout pass.
                SyncDockspaceDockStyles();

                foreach (var panel in _panelsByKey.Values)
                {
                    if (panel == null)
                        continue;
                    if (panel.State != DockPanelState.Docked)
                        continue;

                    // 2) Panels hosted by a dockspace: let the dockspace do the layout.
                    if (panel.Parent is BeepDockspace dockspace)
                    {
                        dockspace.PerformLayout();
                        continue;
                    }

                    // 3) Legacy path: panels directly on the host form get bounds from the engine.
                    if (panel.Parent != _hostForm)
                        continue;

                    var bounds = result.GetPanelBounds(panel.Key);
                    if (!bounds.HasValue)
                        continue;

                    panel.Bounds = bounds.Value;
                    panel.LayoutBounds = bounds.Value;
                }

                SyncSplitters(result);

                // Raise the active panel of each tabbed group above its stack-mates.
                foreach (var group in _layoutTree.Root.Children)
                    BringActivePanelToFrontRecursive(group);
            }
            finally
            {
                _hostForm.ResumeLayout();
            }
        }

        /// <summary>
        /// Sets the <see cref="DockStyle"/> on every managed <see cref="BeepDockspace"/> based on
        /// its <see cref="BeepDockspace.DockPosition"/>. The WinForms layout engine then sizes
        /// each dockspace to the matching edge of the host form (or Fill for the central
        /// workspace dockspace). Dockspaces whose DockStyle is already correct are skipped.
        /// </summary>
        private void SyncDockspaceDockStyles()
        {
            foreach (var dockspace in GetManagedDockspaces())
            {
                if (dockspace == null || dockspace.IsDisposed)
                    continue;

                DockStyle desired = ConvertDockPositionToStyle(dockspace.DockPosition);
                if (dockspace.Dock != desired)
                    dockspace.Dock = desired;
            }
        }

        private static DockStyle ConvertDockPositionToStyle(DockPosition position)
        {
            switch (position)
            {
                case DockPosition.Left:   return DockStyle.Left;
                case DockPosition.Right:  return DockStyle.Right;
                case DockPosition.Top:    return DockStyle.Top;
                case DockPosition.Bottom: return DockStyle.Bottom;
                default:                  return DockStyle.Fill;
            }
        }

        private static void BringActivePanelToFrontRecursive(DockGroup group)
        {
            if (group == null) return;
            group.ActivePanel?.BringToFront();
            foreach (var child in group.Children)
                BringActivePanelToFrontRecursive(child);
        }

        /// <summary>
        /// Seeds each edge group's <see cref="DockGroup.SplitRatio"/> from its active panel's
        /// preferred size the first time it is laid out, so the engine reproduces the panel's
        /// requested width/height. Once seeded (or after a user splitter drag) the ratio is the
        /// canonical resize state and is not overwritten.
        /// </summary>
        private void SeedEdgeRatios(Rectangle client)
        {
            foreach (var group in _layoutTree.Root.Children)
                SeedGroupAndDescendants(group, client);
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
        /// Shrinks the layout engine client area so docked panels do not draw under auto-hide strips.
        /// </summary>
        private Rectangle GetLayoutClientBounds(Rectangle fullClient)
        {
            int left = fullClient.X;
            int top = fullClient.Y;
            int right = fullClient.Right;
            int bottom = fullClient.Bottom;

            foreach (var kv in _autoHideStrips)
            {
                var strip = kv.Value;
                if (strip == null || strip.Panels.Count == 0)
                    continue;

                int inset = AutoHideStrip.TabSize + strip.SlideExtent;
                switch (kv.Key)
                {
                    case DockPosition.Left:
                        left += inset;
                        break;
                    case DockPosition.Right:
                        right -= inset;
                        break;
                    case DockPosition.Top:
                        top += inset;
                        break;
                    case DockPosition.Bottom:
                        bottom -= inset;
                        break;
                }
            }

            int width = Math.Max(0, right - left);
            int height = Math.Max(0, bottom - top);
            return new Rectangle(left, top, width, height);
        }

        /// <summary>Removes root-level groups that no longer contain any docked panels.</summary>
        private void PruneEmptyRootGroups()
        {
            foreach (var child in _layoutTree.Root.Children.ToList())
            {
                if (GroupHasContent(child))
                    continue;

                _layoutTree.Root.RemoveChild(child);
                _layoutTree.UnregisterGroup(child.Id);
            }
        }

        /// <summary>
        /// Back-compat shim: any legacy caller that asked to position a single panel now
        /// triggers a full engine-driven layout pass.
        /// </summary>
        private void ApplyLayoutBounds(DockPanel panel) => ApplyLayout();

        /// <summary>
        /// Back-compat shim: any legacy caller that asked to position a group now triggers a
        /// full engine-driven layout pass.
        /// </summary>
        private void ApplyDockGroupBounds(DockGroup group) => ApplyLayout();

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
            panel.Visible = true;

            if (panel.Group == null)
            {
                var group = GetOrCreateGroupAtPosition(panel.DockPosition);
                group.AddPanel(panel);
                group.ActivePanel = panel;
            }

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

            panel.State = DockPanelState.Hidden;
            panel.Visible = false;

            if (_hostForm != null && _hostForm.Controls.Contains(panel))
                _hostForm.Controls.Remove(panel);

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

            // HideOnClose: keep the panel in the manager and just hide it. ShowPanel will
            // re-attach it to the layout tree. Mirrors DockContent.HideOnClose behavior.
            if (panel.HideOnClose)
            {
                if (panel.State == DockPanelState.AutoHidden)
                    DetachFromAutoHideStrip(panel);

                panel.Group?.RemovePanel(panel);
                _layoutTree.UnregisterPanel(panelKey);
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

            // Remove the panel control from the host form (do not dispose — stored for reopen).
            // The edge splitter (keyed by group id) is reconciled by the RecalculateLayout below.
            if (_hostForm != null && _hostForm.Controls.Contains(panel))
                _hostForm.Controls.Remove(panel);
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

            if (panel.State == DockPanelState.Floating)
                return;

            if (panel.State == DockPanelState.AutoHidden)
                DetachFromAutoHideStrip(panel);
            else if (panel.State == DockPanelState.Hidden)
            { /* already detached from host by HidePanel */ }
            else if (panel.State == DockPanelState.Closed && _hostForm != null && _hostForm.Controls.Contains(panel))
                _hostForm.Controls.Remove(panel);

            // Remove from its current group in the layout tree
            panel.Group?.RemovePanel(panel);
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
                    var dockspace = FindDockspaceAt(_hostForm, panel.DockPosition);
                    if (dockspace != null && orphan.Parent != dockspace)
                    {
                        if (orphan.Parent != null)
                            orphan.Parent.Controls.Remove(orphan);
                        dockspace.Controls.Add(orphan);
                        dockspace.LayoutPanels();
                    }
                    else if (_hostForm != null && !_hostForm.Controls.Contains(orphan))
                    {
                        _hostForm.Controls.Add(orphan);
                    }

                    var restoreGroup = GetOrCreateGroupAtPosition(panel.DockPosition);
                    if (orphan.Group == null)
                        restoreGroup.AddPanel(orphan);
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
            _guideOverlay.ShowOver(_hostForm);
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
                return;

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

            if (panel.State == DockPanelState.AutoHidden)
                return;

            if (panel.State == DockPanelState.Floating)
                CloseFloatWindowFor(panel);

            // Remove from its current docked group in the layout tree
            panel.Group?.RemovePanel(panel);

            // Detach from the host — the slide panel hosts it while peeked.
            if (_hostForm != null && _hostForm.Controls.Contains(panel))
                _hostForm.Controls.Remove(panel);
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
            {
                if (strip == null) continue;
                strip.ControlStyle = _style;
                strip.ApplyDockingTheme(_themeColors);
            }

            foreach (var splitter in _splitters.Values)
            {
                if (splitter == null) continue;
                splitter.ControlStyle = _style;
                splitter.ApplyDockingTheme(_themeColors);
            }

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

            foreach (var dockspace in GetManagedDockspaces())
            {
                if (dockspace == null) continue;
                dockspace.ControlStyle = _style;
                dockspace.Invalidate();
            }

            foreach (var strip in _autoHideStrips.Values)
            {
                if (strip == null) continue;
                strip.ControlStyle = _style;
                strip.Invalidate();
            }

            foreach (var splitter in _splitters.Values)
            {
                if (splitter == null) continue;
                splitter.ControlStyle = _style;
                splitter.Invalidate();
            }
        }

        internal void ApplyThemeToDockspace(BeepDockspace dockspace)
        {
            if (dockspace == null)
                return;

            dockspace.ControlStyle = _style;
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
            else if (panel?.State == DockPanelState.AutoHidden)
                RestoreAutoHiddenPanel(panelKey);
            else if (panel?.State == DockPanelState.Hidden)
                ShowPanel(panelKey);
            else if (_closedPanels.ContainsKey(panelKey))
                ReopenPanel(panelKey);
        }

        /// <summary>
        /// Raises <see cref="PageFloatingRequest"/> and, unless cancelled, floats the panel.
        /// Mirrors Krypton's <c>MakeFloatingRequest</c>.
        /// </summary>
        public virtual void MakeFloatingRequest(string panelKey)
        {
            var panel = GetPanel(panelKey);
            if (panel == null) return;

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
            if (panel == null) return;

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
                CloseRequest(key);
        }

        /// <summary>
        /// Raises <see cref="PageCloseRequest"/> for a single panel and, unless cancelled,
        /// closes it. This is the entry point for user-initiated closes (caption close button,
        /// middle-click, context menu) so handlers can veto the close.
        /// </summary>
        public virtual void CloseRequest(string panelKey)
        {
            var panel = GetPanel(panelKey);
            if (panel == null)
                return;

            var args = new PanelCloseRequestEventArgs(panelKey, panel);
            OnPageCloseRequest(args);
            if (args.Cancel)
                return;

            switch (args.CloseRequest)
            {
                case DockingCloseRequest.None:
                    break;
                case DockingCloseRequest.RemovePanel:
                    RemovePanel(panelKey);
                    break;
                case DockingCloseRequest.RemovePanelAndDispose:
                    RemovePanel(panelKey);
                    break;
                case DockingCloseRequest.HidePanel:
                default:
                    HidePanel(panelKey);
                    break;
            }
        }

        /// <summary>
        /// Raises <see cref="ShowPanelContextMenu"/> and, if a custom menu was supplied, shows it.
        /// Returns <c>true</c> when a custom menu was shown (built-in menu should be skipped).
        /// </summary>
        internal bool TryShowPanelContextMenu(DockPanel panel, Point clientLocation)
        {
            if (panel == null)
                return false;

            var screen = panel.PointToScreen(clientLocation);
            var args = new PanelContextMenuEventArgs(panel, screen);
            OnShowPanelContextMenu(args);

            if (args.ContextMenu == null)
                return false;

            args.ContextMenu.Show(panel, clientLocation);
            return true;
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

        /// <summary>Raises the <see cref="PanelMovedBetweenGroups"/> event.</summary>
        protected virtual void OnPanelMovedBetweenGroups(PanelMovedBetweenGroupsEventArgs e) =>
            PanelMovedBetweenGroups?.Invoke(this, e);

        /// <summary>Raises the <see cref="PanelStateChanging"/> event. Returns true when the
        /// change is allowed (no subscriber canceled it).</summary>
        protected virtual bool RaiseStateChanging(DockPanel panel,
            DockPanelState currentState, DockPanelState requestedState)
        {
            if (panel == null || PanelStateChanging == null) return true;
            var args = new PanelStateChangingEventArgs(panel, currentState, requestedState);
            PanelStateChanging.Invoke(this, args);
            return !args.Cancel;
        }

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

        // ── MRU (Most Recently Used) panel ordering ─────────────────────────

        private void PushMrPanel(string panelKey)
        {
            if (string.IsNullOrEmpty(panelKey)) return;
            _mruList.Remove(panelKey);
            _mruList.AddFirst(panelKey);
        }

        internal void RemoveMrPanel(string panelKey)
        {
            _mruList.Remove(panelKey);
        }

        private string GetNextMrPanel(bool forward)
        {
            if (_mruList.Count == 0) return null;

            string active = _mruList.First?.Value;
            if (active == null) return null;

            var current = _mruList.Find(active);
            if (current == null) return _mruList.First.Value;

            if (forward)
            {
                var next = current.Next ?? _mruList.First;
                return next?.Value;
            }
            else
            {
                var prev = current.Previous ?? _mruList.Last;
                return prev?.Value;
            }
        }

        // ── Keyboard handling (Ctrl+Tab navigator, Ctrl+F4, Escape) ────────────

        private void OnHostFormKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Handled || _disposed) return;

            if (e.Control && !e.Shift && e.KeyCode == Keys.Tab)
            {
                if (_navigator == null || _navigator.IsDisposed)
                {
                    ShowNavigator();
                }
                else
                {
                    _navigator.SelectNext();
                }
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
            else if (e.Control && e.Shift && e.KeyCode == Keys.Tab)
            {
                if (_navigator == null || _navigator.IsDisposed)
                {
                    ShowNavigator();
                    _navigator?.SelectPrevious();
                }
                else
                {
                    _navigator.SelectPrevious();
                }
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
            else if (e.Control && (e.KeyCode == Keys.F4 || e.KeyCode == Keys.W))
            {
                // Only consume the key when we actually have a panel to close — otherwise
                // let TextBox/other controls see Ctrl+W (delete word) and Ctrl+F4 as usual.
                string activeKey = GetActivePanelKey();
                if (!string.IsNullOrEmpty(activeKey))
                {
                    ClosePanel(activeKey);
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                }
            }
            else if (e.Control && e.Shift && e.KeyCode == Keys.Left)
            {
                if (MoveActivePanel(-1))
                {
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                }
            }
            else if (e.Control && e.Shift && e.KeyCode == Keys.Right)
            {
                if (MoveActivePanel(1))
                {
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                }
            }
            else if (e.KeyCode == Keys.Escape)
            {
                bool handled = false;
                if (_navigator != null && !_navigator.IsDisposed)
                {
                    _navigator.Cancel();
                    handled = true;
                }
                else if (_dragController != null && _dragController.IsDragging)
                {
                    _dragController.Cancel();
                    foreach (var dockspace in GetManagedDockspaces())
                        dockspace.CancelDrag();
                    handled = true;
                }

                // Only consume Escape if we actually had something to cancel; otherwise let
                // the focused control (e.g. a TextBox) see the key and clear its content.
                if (handled)
                    e.Handled = true;
            }
        }

        private void OnHostFormKeyUp(object sender, KeyEventArgs e)
        {
            if (_disposed) return;

            // Releasing Ctrl while the navigator is open commits the highlighted entry
            // (matches Visual Studio's Ctrl+Tab UX).
            if (e.KeyCode == Keys.ControlKey)
            {
                if (_navigator != null && !_navigator.IsDisposed)
                {
                    CommitNavigatorSelection();
                }
            }
        }

        private void ShowNavigator()
        {
            if (_hostForm == null || _hostForm.IsDisposed) return;

            var dockedPanels = _mruList
                .Select(k => _panelsByKey.TryGetValue(k, out var p) ? p : null)
                .Where(p => p != null && p.State == DockPanelState.Docked)
                .ToList();

            if (dockedPanels.Count == 0) return;

            Point screenCenter = _hostForm.PointToScreen(
                new Point(_hostForm.ClientSize.Width / 2, _hostForm.ClientSize.Height / 2));

            _navigator = new BeepDockingNavigator(dockedPanels, _themeColors, screenCenter);
            _navigator.FormClosed += (_, _) => _navigator = null;
            _navigator.Show(_hostForm);
        }

        private void CommitNavigatorSelection()
        {
            if (_navigator == null || _navigator.IsDisposed) return;

            string key = _navigator.SelectedPanelKey;
            if (!string.IsNullOrEmpty(key))
            {
                ActivatePanel(key);
            }
            _navigator.Close();
            _navigator = null;
        }

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
