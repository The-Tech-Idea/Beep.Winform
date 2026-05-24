using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Docking;
using TheTechIdea.Beep.Winform.Controls.Docking.Helpers;

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
        private bool _canClose = true;
        private bool _canFloat = true;
        private bool _canAutoHide = true;
        private DockAreas _allowedAreas = DockAreas.All;
        private DockingThemeColors _themeColors = DockingThemeColors.Default;

        // button hit rects — recalculated in OnResize / OnPaint
        private Rectangle _closeBtnRect;
        private Rectangle _floatBtnRect;
        private Rectangle _autoHideBtnRect;

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
            : new Rectangle(0, CaptionHeight, Width, Math.Max(0, Height - CaptionHeight));

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
            Invalidate();
        }

        // ── visual rendering ─────────────────────────────────────────────────

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (IsHostedInDockspace)
                return;

            DrawCaption(e.Graphics);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            UpdateButtonRects();

            if (_content != null)
                _content.Bounds = ContentBounds;
        }

        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);
            Padding = IsHostedInDockspace ? Padding.Empty : new Padding(0, CaptionHeight, 0, 0);
            TryRegisterWithManager();
        }

        private bool IsHostedInDockspace => Parent is BeepDockspace;

        private void TryRegisterWithManager()
        {
            if (_manager == null || IsDesigning || Parent == null || string.IsNullOrWhiteSpace(_key))
                return;

            _manager.RegisterExistingPanel(this);
        }

        private void UpdateButtonRects()
        {
            int y = (CaptionHeight - CaptionButtonSize) / 2;
            int x = Width - 4;

            _closeBtnRect = Rectangle.Empty;
            _floatBtnRect = Rectangle.Empty;
            _autoHideBtnRect = Rectangle.Empty;

            if (_canClose)
            {
                x -= CaptionButtonSize + CaptionButtonSpacing;
                _closeBtnRect = new Rectangle(x, y, CaptionButtonSize, CaptionButtonSize);
            }

            if (_canFloat)
            {
                x -= CaptionButtonSize + CaptionButtonSpacing;
                _floatBtnRect = new Rectangle(x, y, CaptionButtonSize, CaptionButtonSize);
            }

            if (_canAutoHide)
            {
                x -= CaptionButtonSize + CaptionButtonSpacing;
                _autoHideBtnRect = new Rectangle(x, y, CaptionButtonSize, CaptionButtonSize);
            }
        }

        private void DrawCaption(Graphics g)
        {
            if (!IsCaptionHost())
                return;

            UpdateButtonRects();

            var captionRect = new Rectangle(0, 0, Width, CaptionHeight);
            var panels = GetHeaderPanels();
            var activePanel = GetActiveHeaderPanel(panels);
            Color stripBackColor = _themeColors.HeaderBackColor;
            Color inactiveTabColor = _themeColors.InactiveTabBackColor;
            Color activeTabColor = _themeColors.ActiveTabBackColor;
            Color borderColor = _themeColors.TabBorderColor;

            using (var brush = new SolidBrush(stripBackColor))
                g.FillRectangle(brush, captionRect);

            int buttonLeft = FirstButtonLeft();
            int tabsWidth = Math.Max(0, buttonLeft - 2);
            int tabWidth = panels.Count == 0
                ? tabsWidth
                : Math.Max(MinTabWidth, Math.Min(TabMaxWidth, tabsWidth / panels.Count));
            int x = 0;

            foreach (var panel in panels)
                panel.TabBounds = Rectangle.Empty;

            using (var font = new Font("Segoe UI", 9f, FontStyle.Regular))
            {
                foreach (var panel in panels)
                {
                    if (x >= tabsWidth)
                        break;

                    bool tabActive = ReferenceEquals(activePanel, panel);
                    var tabRect = new Rectangle(x, 0, Math.Min(tabWidth, tabsWidth - x), CaptionHeight);
                    panel.TabBounds = tabRect;

                    Color tabBack = tabActive ? activeTabColor : inactiveTabColor;
                    Color tabFore = tabActive
                        ? _themeColors.ActiveTabForeColor
                        : _themeColors.InactiveTabForeColor;
                    bool showTabIcon = DockingCaptionPainter.HasTabIcon(panel.IconPath);

                    using (var brush = new SolidBrush(tabBack))
                        g.FillRectangle(brush, tabRect);

                    using (var pen = new Pen(borderColor))
                        g.DrawRectangle(pen, tabRect.X, tabRect.Y, Math.Max(0, tabRect.Width - 1), Math.Max(0, tabRect.Height - 1));

                    if (showTabIcon)
                        DockingCaptionPainter.PaintTabIcon(g, tabRect, panel.IconPath, tabFore);

                    int textLeft = tabRect.Left + DockingCaptionPainter.GetTabContentLeft(showTabIcon);
                    var textRect = new Rectangle(
                        textLeft,
                        tabRect.Top,
                        Math.Max(0, tabRect.Right - textLeft - DockingCaptionPainter.TabTextPadding),
                        tabRect.Height);

                    using (var brush = new SolidBrush(tabFore))
                    using (var sf = new StringFormat
                    {
                        LineAlignment = StringAlignment.Center,
                        Trimming = StringTrimming.EllipsisCharacter,
                        FormatFlags = StringFormatFlags.NoWrap
                    })
                    {
                        g.DrawString(panel.Title ?? "Panel", font, brush, textRect, sf);
                    }

                    if (panel.IsDirty)
                    {
                        var dot = new Rectangle(tabRect.Right - 8, tabRect.Top + 6, 5, 5);
                        using var dirtyBrush = new SolidBrush(activeTabColor);
                        g.FillEllipse(dirtyBrush, dot);
                    }

                    x += tabRect.Width;
                }
            }

            Color buttonTint = _themeColors.HeaderButtonForeColor;
            DrawCaptionButton(g, _closeBtnRect, buttonTint, CaptionButtonType.Close);
            DrawCaptionButton(g, _floatBtnRect, buttonTint, CaptionButtonType.Float);
            DrawCaptionButton(g, _autoHideBtnRect, buttonTint, CaptionButtonType.AutoHide);

            // bottom border
            using (var pen = new Pen(borderColor))
                g.DrawLine(pen, 0, CaptionHeight - 1, Width - 1, CaptionHeight - 1);
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

        private int FirstButtonLeft()
        {
            int left = Width;
            if (!_closeBtnRect.IsEmpty) left = Math.Min(left, _closeBtnRect.Left);
            if (!_floatBtnRect.IsEmpty) left = Math.Min(left, _floatBtnRect.Left);
            if (!_autoHideBtnRect.IsEmpty) left = Math.Min(left, _autoHideBtnRect.Left);
            return left == Width ? Width : Math.Max(0, left - CaptionButtonSpacing);
        }

        private static Color GetReadableTextColor(Color background) =>
            DockingThemeColors.GetReadableTextColor(background);

        private enum CaptionButtonType { Close, Float, AutoHide }

        private void DrawCaptionButton(Graphics g, Rectangle r, Color color, CaptionButtonType type)
        {
            if (r.IsEmpty)
                return;

            string icon = type switch
            {
                CaptionButtonType.Close => DockingCaptionPainter.CaptionIcons.Close,
                CaptionButtonType.Float => DockingCaptionPainter.CaptionIcons.Float,
                CaptionButtonType.AutoHide => DockingCaptionPainter.CaptionIcons.Pin,
                _ => string.Empty
            };

            if (!string.IsNullOrEmpty(icon))
            {
                DockingCaptionPainter.PaintIcon(g, r, icon, color);
            }

            switch (type)
            {
                case CaptionButtonType.Close:
                    DockingCaptionPainter.PaintCloseFallback(g, r, color);
                    break;
                case CaptionButtonType.Float:
                    DockingCaptionPainter.PaintFloatFallback(g, r, color);
                    break;
                case CaptionButtonType.AutoHide:
                    DockingCaptionPainter.PaintPinFallback(g, r, color);
                    break;
            }
        }

        // ── mouse — caption button hit-testing ───────────────────────────────

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            if (IsHostedInDockspace)
                return;

            if (e.Y > CaptionHeight || !IsCaptionHost())
                return;

            if (e.Button == MouseButtons.Left)
            {
                if (_canClose && _closeBtnRect.Contains(e.Location))
                {
                    _manager?.ClosePanel(_key);
                    return;
                }

                if (_canFloat && _floatBtnRect.Contains(e.Location))
                {
                    _manager?.FloatPanel(_key);
                    return;
                }

                if (_canAutoHide && _autoHideBtnRect.Contains(e.Location))
                {
                    _manager?.AutoHidePanel(_key);
                    return;
                }

                var tabPanel = HitTestHeaderTab(e.Location);
                if (tabPanel != null)
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

        /// <summary>
        /// Shows the caption right-click context menu.
        /// Menu items match the panel's allowed operations, following DockPanelSuite
        /// DockPaneStripBase context-menu pattern.
        /// </summary>
        private void ShowCaptionContextMenu(Point location)
        {
            var menu = new ContextMenuStrip();

            if (_canFloat)
            {
                var itemFloat = new ToolStripMenuItem("Floating");
                itemFloat.Click += (s, e) => _manager?.FloatPanel(_key);
                menu.Items.Add(itemFloat);
            }

            if (_canAutoHide)
            {
                string label = _state == DockPanelState.AutoHidden ? "Dock" : "Auto Hide";
                var itemAutoHide = new ToolStripMenuItem(label);
                itemAutoHide.Click += (s, e) =>
                {
                    if (_state == DockPanelState.AutoHidden)
                        _manager?.DockFloatingPanel(_key, _dockPosition);
                    else
                        _manager?.AutoHidePanel(_key);
                };
                menu.Items.Add(itemAutoHide);
            }

            if (_canClose)
            {
                if (menu.Items.Count > 0)
                    menu.Items.Add(new ToolStripSeparator());
                var itemClose = new ToolStripMenuItem("Close");
                itemClose.Click += (s, e) => _manager?.ClosePanel(_key);
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

            if (e.Y <= CaptionHeight)
                Cursor = HitTestHeaderTab(e.Location) != null ||
                         _closeBtnRect.Contains(e.Location) ||
                         _floatBtnRect.Contains(e.Location) ||
                         _autoHideBtnRect.Contains(e.Location)
                    ? Cursors.Hand
                    : Cursors.Default;
        }

        private DockPanel HitTestHeaderTab(Point location)
        {
            if (location.Y > CaptionHeight)
                return null;

            var panels = GetHeaderPanels();
            foreach (var panel in panels)
            {
                if (!panel.TabBounds.IsEmpty && panel.TabBounds.Contains(location))
                    return panel;
            }

            return null;
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
