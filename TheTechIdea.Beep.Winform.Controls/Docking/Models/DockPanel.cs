using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Docking;
using TheTechIdea.Beep.Winform.Controls.Docking.Helpers;
using TheTechIdea.Beep.Winform.Controls.Docking.Layoutmanagers;
using TheTechIdea.Beep.Winform.Controls.Docking.Painters;
using TheTechIdea.Beep.Winform.Controls.Docking.Painters.Caption;

namespace TheTechIdea.Beep.Winform.Controls.Docking.Models
{
    /// <summary>
    /// A visual dockable panel.
    /// Inherits Panel so it renders as a real control on the host form —
    /// following DockPanelSuite (DockPanel : Panel) and Krypton (KryptonPanel : Panel).
    /// 
    /// The BeepDockingManager creates, positions and removes instances at runtime.
    /// All docking properties serialise into .designer.cs via the WinForms designer.
    /// </summary>
    [ToolboxItem(false)]
    [DesignerCategory("code")]
    [DesignTimeVisible(false)]
    [Category("Docking")]
    [Description("Docking panel component.")]
    [Designer("TheTechIdea.Beep.Winform.Controls.Design.Server.Docking.Designers.DockPanelDesigner, TheTechIdea.Beep.Winform.Controls.Design.Server")]
    [DefaultEvent(nameof(Activated))]
    [DefaultProperty(nameof(Title))]
    public class DockPanel : Panel
    {
        // ── dockspace header / tab strip ───────────────────────────────────
        private const int CaptionHeight = 24;
        private const int TabMaxWidth = 160;
        private const int CaptionButtonSize = 18;
        private const int CaptionButtonSpacing = 4;
        private const int MinTabWidth = 72;

        private BeepDockingManager _manager;
        private string _key = string.Empty;
        private string _title = "Tool Window";
        private string _iconPath = string.Empty;
        private DockPanelState _state = DockPanelState.Docked;
        private DockPosition _dockPosition = DockPosition.Left;
        private int _preferredWidth = 250;
        private int _preferredHeight = 150;
        private int _minWidth = 80;
        private int _minHeight = 60;
        private bool _showCaption = true;
        private bool _canClose = true;
        private bool _canFloat = true;
        private bool _canAutoHide = true;
        private DockAreas _allowedAreas = DockAreas.All;
        private DockingThemeColors _themeColors = DockingThemeColors.Default;

        // Shared caption layout + renderer (single source of caption geometry and painting).
        private readonly CaptionLayoutManager _captionLayout = new CaptionLayoutManager
        {
            ButtonSize = CaptionButtonSize,
            ButtonSpacing = CaptionButtonSpacing,
            MinTabWidth = MinTabWidth,
            MaxTabWidth = TabMaxWidth
        };
        /// <summary>Control style driving caption background/border rendering. Set by the manager.</summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        internal BeepControlStyle ControlStyle { get; set; } = BeepControlStyle.Material3;

        // Caption drag state (drag-to-float/dock). Geometry/commit live in the manager's controller.
        private bool _captionDragArmed;
        private bool _captionDragging;
        private bool _suppressCaptionClick;

        // In-strip tab reorder state. While the cursor stays inside the caption strip a left-drag
        // reorders the pressed tab; once it leaves the strip we hand off to the float/dock drag.
        private bool _reorderActive;
        private bool _reorderMoved;
        private DockPanel _reorderPanel;
        private Point _reorderPressScreen;

        /// <summary>
        /// Unique identifier for this panel (must be unique within the host).
        /// </summary>
        [Category("Docking")]
        [Description("Unique key for this panel within the docking host")]
        [DefaultValue("")]
        public string Key
        {
            get => _key;
            set
            {
                if (_key == value) return;
                // Unregister old key from manager before changing.
                if (!string.IsNullOrEmpty(_key))
                    _manager?.UnregisterExistingPanel(this);
                _key = value;
                OnPropertyChanged(nameof(Key));
                TryRegisterWithManager();
            }
        }

        /// <summary>
        /// The BeepDockingManager that orchestrates this panel at runtime.
        /// </summary>
        [Category("Docking")]
        [Description("The docking manager that owns this panel")]
        [Browsable(false)]
        [TypeConverter(typeof(ReferenceConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public BeepDockingManager Manager
        {
            get => _manager;
            set
            {
                if (_manager == value) return;
                var previous = _manager;
                if (previous != null && !IsDesigning)
                    previous.UnregisterExistingPanel(this);

                _manager = value;
                _manager?.ApplyThemeToPanel(this);
                OnPropertyChanged(nameof(Manager));
                TryRegisterWithManager();
            }
        }

        /// <summary>
        /// True when hosted in the Visual Studio designer.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsDesigning =>
            Site?.DesignMode == true ||
            LicenseManager.UsageMode == LicenseUsageMode.Designtime ||
            IsWinFormsDesignerProcess();

        /// <summary>
        /// Gets the rectangle available for hosted content (below the caption strip).
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Rectangle ContentBounds => IsHostedInDockspace
            ? new Rectangle(0, 0, Width, Height)
            : new Rectangle(0, EffectiveCaptionHeight, Width, Math.Max(0, Height - EffectiveCaptionHeight));

        /// <summary>
        /// Whether this panel draws its own caption strip. The float window sets this to
        /// <c>false</c> so the floating chrome can own the (themed) caption without duplication.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ShowCaption
        {
            get => _showCaption;
            set
            {
                if (_showCaption == value) return;
                _showCaption = value;
                if (!IsHostedInDockspace)
                    Padding = new Padding(0, EffectiveCaptionHeight, 0, 0);
                if (_content != null)
                    _content.Bounds = ContentBounds;
                Invalidate();
            }
        }

        /// <summary>Caption height honored for this instance (0 when <see cref="ShowCaption"/> is false).</summary>
        private int EffectiveCaptionHeight => _showCaption ? CaptionHeight : 0;

        /// <summary>Display title shown in the caption strip.</summary>
        [Category("Docking")]
        [Description("Title displayed in the panel caption")]
        [DefaultValue("Tool Window")]
        public string Title
        {
            get => _title;
            set
            {
                if (_title == value) return;
                _title = value;
                Invalidate();
                OnPropertyChanged(nameof(Title));
                if (!IsDesigning)
                    _manager?.NotifyPanelTitleChanged(this);
            }
        }

        /// <summary>Path to icon file. Leave empty for no icon.</summary>
        [Category("Docking")]
        [Description("Optional path to icon file shown in the caption")]
        [DefaultValue("")]
        public string IconPath
        {
            get => _iconPath;
            set
            {
                if (_iconPath == value) return;
                _iconPath = value;
                Invalidate();
                OnPropertyChanged(nameof(IconPath));
            }
        }

        /// <summary>
        /// The user control / panel containing this panel's content.
        /// Set at runtime; not serialized by the designer.
        /// When set the control is added as a child and fills ContentBounds.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Control Content
        {
            get => _content;
            set
            {
                if (_content == value) return;

                if (_content != null)
                    Controls.Remove(_content);

                _content = value;

                if (_content != null)
                {
                    _content.Bounds = ContentBounds;
                    _content.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
                    Controls.Add(_content);
                }
            }
        }
        private Control _content;

        /// <summary>Current display state of the panel.</summary>
        [Category("Docking")]
        [Description("Current display state of the panel")]
        [DefaultValue(DockPanelState.Docked)]
        public DockPanelState State
        {
            get => _state;
            set
            {
                if (_state == value) return;
                _state = value;
                OnPropertyChanged(nameof(State));
                if (!IsDesigning)
                    _manager?.RecalculateLayout();
            }
        }

        /// <summary>The edge this panel docks to.</summary>
        [Category("Docking")]
        [Description("The edge this panel docks to")]
        [DefaultValue(DockPosition.Left)]
        public DockPosition DockPosition
        {
            get => _dockPosition;
            set
            {
                if (_dockPosition == value) return;
                var oldPosition = _dockPosition;
                _dockPosition = value;
                OnPropertyChanged(nameof(DockPosition));
                if (!IsDesigning)
                    _manager?.NotifyPanelDockPositionChanged(this, oldPosition);
            }
        }

        /// <summary>Preferred width when docked Left or Right.</summary>
        [Category("Docking")]
        [Description("Preferred width when docked Left or Right")]
        [DefaultValue(250)]
        public int PreferredWidth
        {
            get => _preferredWidth;
            set
            {
                if (_preferredWidth == value) return;
                _preferredWidth = value;
                OnPropertyChanged(nameof(PreferredWidth));
                if (!IsDesigning)
                    _manager?.NotifyPanelPreferredSizeChanged(this);
            }
        }

        /// <summary>Preferred height when docked Top or Bottom.</summary>
        [Category("Docking")]
        [Description("Preferred height when docked Top or Bottom")]
        [DefaultValue(150)]
        public int PreferredHeight
        {
            get => _preferredHeight;
            set
            {
                if (_preferredHeight == value) return;
                _preferredHeight = value;
                OnPropertyChanged(nameof(PreferredHeight));
                if (!IsDesigning)
                    _manager?.NotifyPanelPreferredSizeChanged(this);
            }
        }

        /// <summary>Minimum width this panel may be resized to when docked Left or Right.</summary>
        [Category("Docking")]
        [Description("Minimum width this panel may be resized to when docked Left or Right")]
        [DefaultValue(80)]
        public int MinWidth
        {
            get => _minWidth;
            set
            {
                int v = Math.Max(1, value);
                if (_minWidth == v) return;
                _minWidth = v;
                OnPropertyChanged(nameof(MinWidth));
                if (!IsDesigning)
                    _manager?.NotifyPanelPreferredSizeChanged(this);
            }
        }

        /// <summary>Minimum height this panel may be resized to when docked Top or Bottom.</summary>
        [Category("Docking")]
        [Description("Minimum height this panel may be resized to when docked Top or Bottom")]
        [DefaultValue(60)]
        public int MinHeight
        {
            get => _minHeight;
            set
            {
                int v = Math.Max(1, value);
                if (_minHeight == v) return;
                _minHeight = v;
                OnPropertyChanged(nameof(MinHeight));
                if (!IsDesigning)
                    _manager?.NotifyPanelPreferredSizeChanged(this);
            }
        }

        /// <summary>The group this panel is currently in (set by DockGroup).</summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DockGroup Group { get; internal set; }

        /// <summary>Whether the user can close this panel.</summary>
        [Category("Docking")]
        [Description("Whether the user can close this panel")]
        [DefaultValue(true)]
        public bool CanClose
        {
            get => _canClose;
            set
            {
                if (_canClose == value) return;
                _canClose = value;
                Invalidate();
                OnPropertyChanged(nameof(CanClose));
            }
        }

        /// <summary>Whether this panel can float as a separate window.</summary>
        [Category("Docking")]
        [Description("Whether this panel can float as a separate window")]
        [DefaultValue(true)]
        public bool CanFloat
        {
            get => _canFloat;
            set
            {
                if (_canFloat == value) return;
                _canFloat = value;
                Invalidate();
                OnPropertyChanged(nameof(CanFloat));
            }
        }

        /// <summary>Whether this panel can be auto-hidden to a side tab strip.</summary>
        [Category("Docking")]
        [Description("Whether this panel can be auto-hidden")]
        [DefaultValue(true)]
        public bool CanAutoHide
        {
            get => _canAutoHide;
            set
            {
                if (_canAutoHide == value) return;
                _canAutoHide = value;
                Invalidate();
                OnPropertyChanged(nameof(CanAutoHide));
            }
        }

        /// <summary>
        /// Flags controlling which dock positions and states are allowed.
        /// Mirrors DockContent.DockAreas in DockPanelSuite and per-page flags in Krypton.
        /// </summary>
        [Category("Docking")]
        [Description("Flags controlling which dock positions and states are allowed for this panel")]
        [DefaultValue(DockAreas.All)]
        public DockAreas AllowedAreas
        {
            get => _allowedAreas;
            set
            {
                if (_allowedAreas == value) return;
                _allowedAreas = value;
                _canFloat = (_allowedAreas & DockAreas.Float) != 0;
                _canAutoHide = (_allowedAreas & DockAreas.AutoHide) != 0;
                Invalidate();
                OnPropertyChanged(nameof(AllowedAreas));
            }
        }

        /// <summary>Whether this panel's content has unsaved changes.</summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsDirty { get; set; }

        /// <summary>Cached native window handle. Set by runtime manager when using Win32 MDI path.</summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IntPtr NativeHandle { get; internal set; } = IntPtr.Zero;

        /// <summary>Cached layout bounds set by the layout engine.</summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Rectangle LayoutBounds { get; internal set; } = Rectangle.Empty;

        /// <summary>Cached tab strip bounds for this panel.</summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Rectangle TabBounds { get; internal set; } = Rectangle.Empty;

        /// <summary>Optional user-defined metadata tag.</summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new object Tag { get; set; }

        // ── events ──────────────────────────────────────────────────────────

        /// <summary>Raised when this panel is activated (becomes active in its group).</summary>
        public event EventHandler Activated;

        /// <summary>Raised when this panel is deactivated.</summary>
        public event EventHandler Deactivated;

        /// <summary>Raised when this panel is closed.</summary>
        public event EventHandler Closed;

        /// <summary>Raised when any docking property changes.</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        // ── constructor ──────────────────────────────────────────────────────

        public DockPanel()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.ResizeRedraw, true);

            BorderStyle = BorderStyle.None;
            Padding = new Padding(0, CaptionHeight, 0, 0);
        }

        internal void ApplyDockingTheme(DockingThemeColors colors)
        {
            _themeColors = colors ?? DockingThemeColors.Default;
            BackColor = _themeColors.PanelBackColor;
            ForeColor = _themeColors.PanelForeColor;
            if (_content != null)
            {
                _content.BackColor = _themeColors.PanelBackColor;
                _content.ForeColor = _themeColors.PanelForeColor;
            }
            Invalidate();
        }

        // ── visual rendering ─────────────────────────────────────────────────

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (IsHostedInDockspace || !_showCaption)
                return;

            DrawCaption(e.Graphics);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            RecomputeCaptionLayout();

            if (_content != null)
                _content.Bounds = ContentBounds;
        }

        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);
            Padding = IsHostedInDockspace ? Padding.Empty : new Padding(0, EffectiveCaptionHeight, 0, 0);
            TryRegisterWithManager();
        }

        private bool IsHostedInDockspace => Parent is BeepDockspace;

        private void TryRegisterWithManager()
        {
            if (_manager == null || IsDesigning || Parent == null || string.IsNullOrWhiteSpace(_key))
                return;

            _manager.RegisterExistingPanel(this);
        }

        /// <summary>Builds the ordered caption button set (right-to-left placement) for this panel.</summary>
        private List<CaptionButtonKind> BuildCaptionButtons()
        {
            var buttons = new List<CaptionButtonKind>(3);
            if (_canClose) buttons.Add(CaptionButtonKind.Close);
            if (_canFloat) buttons.Add(CaptionButtonKind.Float);
            if (_canAutoHide) buttons.Add(CaptionButtonKind.AutoHide);
            return buttons;
        }

        /// <summary>
        /// Recomputes caption geometry (tab + button rects) into the shared layout manager and
        /// mirrors the tab rects back onto each panel's <see cref="TabBounds"/> for compatibility.
        /// </summary>
        private List<CaptionButtonKind> RecomputeCaptionLayout()
        {
            var panels = GetHeaderPanels();
            var activePanel = GetActiveHeaderPanel(panels);
            var buttons = BuildCaptionButtons();

            var tabs = new List<CaptionTabModel>(panels.Count);
            foreach (var panel in panels)
            {
                tabs.Add(new CaptionTabModel
                {
                    Key = panel.Key,
                    Title = panel.Title,
                    IconPath = panel.IconPath,
                    IsDirty = panel.IsDirty,
                    IsActive = ReferenceEquals(activePanel, panel),
                    Tag = panel
                });
            }

            _captionLayout.Compute(Width, CaptionHeight, tabs, buttons);

            foreach (var panel in panels)
                panel.TabBounds = Rectangle.Empty;
            foreach (var kv in _captionLayout.TabRects)
            {
                if (kv.Key.Tag is DockPanel dp)
                    dp.TabBounds = kv.Value;
            }

            return buttons;
        }

        private void DrawCaption(Graphics g)
        {
            if (!IsCaptionHost())
                return;

            var buttons = RecomputeCaptionLayout();

            var ctx = new DockingPainterContext
            {
                Colors = _themeColors,
                Style = ControlStyle,
                Bounds = new Rectangle(0, 0, Width, CaptionHeight),
                IsDesignTime = IsDesigning
            };

            DockingPainterFactory.GetRenderers(ControlStyle).Caption.Paint(g, ctx, _captionLayout, buttons);
        }

        private IReadOnlyList<DockPanel> GetHeaderPanels()
        {
            var visualStack = GetVisualHeaderPanels();

            if (IsDesigning && visualStack.Count > 0)
                return visualStack;

            if (Group?.Panels != null && Group.Panels.Count > 1)
                return Group.Panels;

            if (visualStack.Count > 1)
                return visualStack;

            if (Group?.Panels != null && Group.Panels.Count > 0)
                return Group.Panels;

            if (visualStack.Count > 0)
                return visualStack;

            return new[] { this };
        }

        private IReadOnlyList<DockPanel> GetVisualHeaderPanels()
        {
            if (_manager == null || Parent == null)
                return Array.Empty<DockPanel>();

            return Parent.Controls
                .OfType<DockPanel>()
                .Where(panel => !panel.IsDisposed &&
                                ReferenceEquals(panel.Manager, _manager) &&
                                panel.DockPosition == _dockPosition &&
                                IsSameVisualStack(panel))
                .OrderBy(panel => panel.TabIndex)
                .ThenBy(panel => Parent.Controls.IndexOf(panel))
                .ToList();
        }

        private bool IsSameVisualStack(DockPanel panel)
        {
            if (ReferenceEquals(panel, this))
                return true;

            if (Bounds.Width <= 0 || Bounds.Height <= 0 ||
                panel.Bounds.Width <= 0 || panel.Bounds.Height <= 0)
            {
                return true;
            }

            return Math.Abs(panel.Bounds.Left - Bounds.Left) <= 2 &&
                   Math.Abs(panel.Bounds.Top - Bounds.Top) <= 2 &&
                   Math.Abs(panel.Bounds.Width - Bounds.Width) <= 2 &&
                   Math.Abs(panel.Bounds.Height - Bounds.Height) <= 2;
        }

        private DockPanel GetActiveHeaderPanel(IReadOnlyList<DockPanel> panels)
        {
            if (Group?.ActivePanel != null && panels.Contains(Group.ActivePanel))
                return Group.ActivePanel;

            return this;
        }

        /// <summary>
        /// Krypton shows one tab strip per dock cell; only the active stacked panel paints chrome.
        /// </summary>
        private bool IsCaptionHost()
        {
            if (Group?.Panels == null || Group.Panels.Count <= 1)
                return true;

            if (Group.ActivePanel != null)
                return ReferenceEquals(Group.ActivePanel, this);

            return ReferenceEquals(Group.Panels[0], this);
        }

        // ── mouse — caption button hit-testing ───────────────────────────────

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            _captionDragArmed = false;
            _captionDragging = false;
            _reorderActive = false;
            _reorderMoved = false;
            _reorderPanel = null;

            if (IsHostedInDockspace)
                return;
            if (e.Button != MouseButtons.Left)
                return;
            if (!_showCaption || e.Y > EffectiveCaptionHeight || !IsCaptionHost())
                return;

            RecomputeCaptionLayout();

            // Caption buttons + overflow chevron own their click — never start a drag from them.
            if (_captionLayout.HitTestButton(e.Location) != null || _captionLayout.HitTestOverflow(e.Location))
                return;

            // Pressing one of several tabs starts an in-strip reorder; we only hand off to the
            // float/dock drag once the cursor leaves the caption strip (see OnMouseMove).
            var pressedTab = _captionLayout.HitTestTab(e.Location)?.Tag as DockPanel;
            if (pressedTab != null && Group != null && Group.Panels.Count > 1)
            {
                _reorderActive = true;
                _reorderPanel = pressedTab;
                _reorderPressScreen = PointToScreen(e.Location);
                Capture = true;
                Focus();
                return;
            }

            _captionDragArmed = true;
            _manager?.BeginCaptionDrag(this, PointToScreen(e.Location));
            Capture = true;
            Focus();   // ensure ProcessCmdKey receives Escape during the drag
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            // A completed drag swallows the trailing click so it doesn't re-activate/toggle.
            if (_suppressCaptionClick)
            {
                _suppressCaptionClick = false;
                return;
            }

            if (IsHostedInDockspace)
                return;

            if (!_showCaption || e.Y > EffectiveCaptionHeight || !IsCaptionHost())
                return;

            // Middle-click a tab closes it (honoring CanClose).
            if (e.Button == MouseButtons.Middle)
            {
                RecomputeCaptionLayout();
                if (_captionLayout.HitTestTab(e.Location)?.Tag is DockPanel midPanel && midPanel.CanClose)
                    _manager?.CloseRequest(midPanel.Key);
                return;
            }

            if (e.Button == MouseButtons.Left)
            {
                RecomputeCaptionLayout();

                if (_captionLayout.HitTestOverflow(e.Location))
                {
                    ShowOverflowMenu(_captionLayout.OverflowButtonRect);
                    return;
                }

                switch (_captionLayout.HitTestButton(e.Location))
                {
                    case CaptionButtonKind.Close when _canClose:
                        _manager?.CloseRequest(_key);
                        return;
                    case CaptionButtonKind.Float when _canFloat:
                        _manager?.MakeFloatingRequest(_key);
                        return;
                    case CaptionButtonKind.AutoHide when _canAutoHide:
                        _manager?.MakeAutoHiddenRequest(_key);
                        return;
                }

                var tab = _captionLayout.HitTestTab(e.Location);
                if (tab?.Tag is DockPanel tabPanel)
                {
                    _manager?.ActivatePanel(tabPanel.Key);
                    return;
                }

                // click anywhere else on caption — activate this panel
                _manager?.ActivatePanel(_key);
            }
            else if (e.Button == MouseButtons.Right)
            {
                ShowCaptionContextMenu(e.Location);
            }
        }

        /// <summary>Shows the chevron dropdown listing the tabs that overflowed the caption strip.</summary>
        private void ShowOverflowMenu(Rectangle anchor)
        {
            var overflow = _captionLayout.OverflowTabs;
            if (overflow == null || overflow.Count == 0)
                return;

            var menu = new ContextMenuStrip();
            foreach (var tab in overflow)
            {
                if (tab.Tag is not DockPanel panel)
                    continue;
                var item = new ToolStripMenuItem(string.IsNullOrEmpty(panel.Title) ? panel.Key : panel.Title)
                {
                    Checked = ReferenceEquals(Group?.ActivePanel, panel)
                };
                var keyCopy = panel.Key;
                item.Click += (s, ev) => _manager?.ActivatePanel(keyCopy);
                menu.Items.Add(item);
            }

            if (menu.Items.Count > 0)
                menu.Show(this, new Point(anchor.Left, anchor.Bottom));
            else
                menu.Dispose();
        }

        /// <summary>
        /// Shows the caption right-click context menu.
        /// Menu items match the panel's allowed operations, following DockPanelSuite
        /// DockPaneStripBase context-menu pattern.
        /// </summary>
        private void ShowCaptionContextMenu(Point location)
        {
            if (_manager?.TryShowPanelContextMenu(this, location) == true)
                return;

            var menu = new ContextMenuStrip();

            if (_state == DockPanelState.Hidden)
            {
                var itemShow = new ToolStripMenuItem("Show");
                itemShow.Click += (s, e) => _manager?.MakeDockedRequest(_key);
                menu.Items.Add(itemShow);
            }

            if (_canFloat)
            {
                var itemFloat = new ToolStripMenuItem("Floating");
                itemFloat.Click += (s, e) => _manager?.MakeFloatingRequest(_key);
                menu.Items.Add(itemFloat);
            }

            if (_canAutoHide)
            {
                string label = _state == DockPanelState.AutoHidden ? "Dock" : "Auto Hide";
                var itemAutoHide = new ToolStripMenuItem(label);
                itemAutoHide.Click += (s, e) =>
                {
                    if (_state == DockPanelState.AutoHidden)
                        _manager?.RestoreAutoHiddenPanel(_key);   // unpin back to a docked group
                    else
                        _manager?.MakeAutoHiddenRequest(_key);
                };
                menu.Items.Add(itemAutoHide);
            }

            if (_canClose)
            {
                if (menu.Items.Count > 0)
                    menu.Items.Add(new ToolStripSeparator());
                var itemClose = new ToolStripMenuItem("Close");
                itemClose.Click += (s, e) => _manager?.CloseRequest(_key);
                menu.Items.Add(itemClose);
            }

            if (menu.Items.Count > 0)
                menu.Show(this, location);
            else
                menu.Dispose();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (IsHostedInDockspace)
                return;

            // In-strip reorder: while the cursor stays within the caption band, reorder the pressed
            // tab; if it leaves the band, hand off to the float/dock drag controller.
            if (_reorderActive && (e.Button & MouseButtons.Left) == MouseButtons.Left)
            {
                bool insideStrip = e.Y >= 0 && e.Y <= EffectiveCaptionHeight;
                if (insideStrip)
                {
                    ReorderUnderCursor(e.Location);
                    return;
                }

                // Cursor left the strip — convert to a float/dock drag.
                _reorderActive = false;
                _captionDragArmed = true;
                _manager?.BeginCaptionDrag(this, _reorderPressScreen);
                _manager?.UpdateCaptionDrag(PointToScreen(e.Location));
                if (_manager?.IsPanelDragging == true)
                    _captionDragging = true;
                return;
            }

            // While armed, feed moves to the drag controller (it applies the drag threshold).
            if (_captionDragArmed && (e.Button & MouseButtons.Left) == MouseButtons.Left)
            {
                _manager?.UpdateCaptionDrag(PointToScreen(e.Location));
                if (_manager?.IsPanelDragging == true)
                    _captionDragging = true;
                return;
            }

            if (_showCaption && e.Y <= EffectiveCaptionHeight)
            {
                RecomputeCaptionLayout();
                Cursor = _captionLayout.HitTestTab(e.Location) != null ||
                         _captionLayout.HitTestButton(e.Location) != null ||
                         _captionLayout.HitTestOverflow(e.Location)
                    ? Cursors.Hand
                    : Cursors.Default;
            }
        }

        /// <summary>Reorders the dragged tab to the position under the cursor within the caption strip.</summary>
        private void ReorderUnderCursor(Point location)
        {
            if (_reorderPanel == null || Group == null)
                return;

            RecomputeCaptionLayout();
            var targetTab = _captionLayout.HitTestTab(location)?.Tag as DockPanel;
            if (targetTab == null || ReferenceEquals(targetTab, _reorderPanel))
                return;

            int targetIndex = Group.GetPanelIndex(targetTab);
            if (targetIndex < 0)
                return;

            Group.MovePanelToIndex(_reorderPanel, targetIndex);
            _reorderMoved = true;
            RecomputeCaptionLayout();
            Parent?.Invalidate(true);   // repaint the caption host with the new tab order
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            // A reorder that stayed inside the strip never started a caption drag — finalize here.
            if (_reorderActive)
            {
                _reorderActive = false;
                _suppressCaptionClick = _reorderMoved;   // swallow the trailing click if we reordered
                _reorderPanel = null;
                if (Capture)
                    Capture = false;
                return;
            }

            if (!_captionDragArmed)
                return;

            bool wasDragging = _manager?.IsPanelDragging == true;
            _suppressCaptionClick = wasDragging;
            _manager?.EndCaptionDrag(PointToScreen(e.Location), commit: true);

            _captionDragArmed = false;
            _captionDragging = false;
            if (Capture)
                Capture = false;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (_captionDragging && keyData == Keys.Escape)
            {
                _manager?.CancelCaptionDrag();
                _captionDragArmed = false;
                _captionDragging = false;
                _suppressCaptionClick = true;
                if (Capture)
                    Capture = false;
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        // ── diagnostics / overrides ──────────────────────────────────────────

        public override string ToString() =>
            $"DockPanel[Key={Key}, Title={Title}, State={State}, Position={DockPosition}]";

        // ── internal event raisers ───────────────────────────────────────────

        internal void OnActivated() => Activated?.Invoke(this, EventArgs.Empty);
        internal void OnDeactivated() => Deactivated?.Invoke(this, EventArgs.Empty);
        internal void OnClosed() => Closed?.Invoke(this, EventArgs.Empty);
        internal void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private static bool IsWinFormsDesignerProcess()
        {
            try
            {
                string processName = System.Diagnostics.Process.GetCurrentProcess().ProcessName;
                return processName.IndexOf("DesignToolsServer", StringComparison.OrdinalIgnoreCase) >= 0 ||
                       string.Equals(processName, "devenv", StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }
    }
}
