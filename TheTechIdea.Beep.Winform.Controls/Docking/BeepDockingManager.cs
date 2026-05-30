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
        private string _themeName = string.Empty;
        private IBeepTheme _currentTheme;
        private DockingThemeColors _themeColors = DockingThemeColors.Default;
        private BeepControlStyle _style = BeepControlStyle.Material3;

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
        }

        private void DetachHostFormHandlers()
        {
            if (_hostForm == null || _hostLayoutChangedHandler == null)
                return;

            _hostForm.Resize -= _hostLayoutChangedHandler;
            _hostForm.ClientSizeChanged -= _hostLayoutChangedHandler;
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

            EnsurePanelHostedForDock(panel);
        }

        private void EnsurePanelHostedForDock(DockPanel panel)
        {
            if (panel == null || _hostForm == null || IsDesignHosted)
                return;

            panel.ShowCaption = true;
            panel.Dock = DockStyle.None;
            panel.Visible = true;

            if (panel.Parent != _hostForm)
            {
                panel.Parent?.Controls.Remove(panel);
                _hostForm.Controls.Add(panel);
            }
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
        /// rectangles produced by the layout engine. One splitter per edge group, positioned
        /// by explicit bounds (not WinForms Dock). Orphaned splitters are removed.
        /// </summary>
        private void SyncSplitters(DockLayoutResult result)
        {
            if (_hostForm == null || IsDesignHosted || result == null)
                return;

            var desired = new HashSet<string>(StringComparer.Ordinal);

            foreach (var hit in result.Splitters)
            {
                if (string.IsNullOrEmpty(hit.GroupId))
                    continue;

                desired.Add(hit.GroupId);

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

            // Dispose splitters whose group no longer needs one.
            var orphans = _splitters.Keys.Where(k => !desired.Contains(k)).ToList();
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
                panel.BringToFront();

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
                    panel.BringToFront();
                    ApplyLayout();
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

            // Reparent DockPanel children from BeepDockspace containers to the host form.
            var dockspaces = EnumerateDockspaces(root).ToList();
            foreach (var ds in dockspaces)
            {
                foreach (var panel in ds.Panels.ToArray())
                {
                    OnDockspaceAdding(new DockspaceEventArgs(panel));
                    ds.Controls.Remove(panel);
                    panel.DockPosition = ds.DockPosition;
                    panel.ShowCaption = true;
                    panel.Dock = DockStyle.None;
                    panel.Visible = true;
                    if (root is Form hostForm)
                    {
                        hostForm.Controls.Add(panel);
                        panel.Manager ??= this;
                    }
                }
            }

            foreach (var panel in EnumerateDockPanels(root).OrderBy(p => p.TabIndex).ToList())
            {
                if (ReferenceEquals(panel.Manager, this))
                    RegisterExistingPanel(panel);
            }

            // Remove now-empty BeepDockspace controls.
            foreach (var ds in dockspaces.Where(d => d.Panels.Count == 0))
            {
                OnDockspaceRemoved(new DockspaceEventArgs(null));
                ds.Parent?.Controls.Remove(ds);
                ds.Dispose();
            }
        }

        private static IEnumerable<BeepDockspace> EnumerateDockspaces(Control root)
        {
            foreach (Control child in root.Controls)
            {
                if (child is BeepDockspace ds)
                    yield return ds;
                foreach (var nested in EnumerateDockspaces(child))
                    yield return nested;
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
                foreach (var panel in _panelsByKey.Values)
                {
                    if (panel == null || panel.Parent != _hostForm)
                        continue;
                    if (panel.State != DockPanelState.Docked)
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
            if (panel.Group != null && panel.Group != targetGroup)
                panel.Group.RemovePanel(panel);

            PreparePanelForDock(panel);
            panel.State = DockPanelState.Docked;

            targetGroup.AddPanel(panel);
            targetGroup.ActivePanel = panel;

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

            Debug.WriteLine($"[BeepDockingManager] Panel activated: {panelKey}");
            return true;
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

            EnsurePanelHostedForDock(panel);
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
            EnsurePanelHostedForDock(panel);

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
            var floatWindow = initialBounds.IsEmpty
                ? new FloatWindow(panel, _hostForm)
                : new FloatWindow(panel, _hostForm, initialBounds);
            floatWindow.ControlStyle = _style;
            floatWindow.ApplyDockingTheme(_themeColors);
            floatWindow.PanelRedocked += OnFloatWindowRedocked;

            // Wire drag-guide overlay — shown while the float window is moved.
            // Follows DockPanelSuite DockDragHandler: overlay tracks cursor, HitTest on move.
            EnsureGuideOverlay();
            floatWindow.Move += (s, e) => OnFloatWindowMoved(floatWindow);
            floatWindow.MouseUp += (s, e) => TryCommitFloatWindowDrop(floatWindow, e);
            floatWindow.MoveOperationEnded += (s, e) => TryCommitFloatWindowDrop(floatWindow, null);
            floatWindow.FormClosed += (s, e) =>
            {
                _floatWindowsByKey.Remove(panelKey);
                HideGuideOverlay();
            };

            _floatWindowsByKey[panelKey] = floatWindow;

            var addingArgs = new FloatingWindowEventArgs(floatWindow, panel);
            OnFloatingWindowAdding(addingArgs);

            floatWindow.Show(_hostForm);

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
            EnsurePanelHostedForDock(panel);

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

            panel.State = DockPanelState.AutoHidden;
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
            EnsurePanelHostedForDock(panel);
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
