using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Docking.Helpers;
using TheTechIdea.Beep.Winform.Controls.Docking.Models;

namespace TheTechIdea.Beep.Winform.Controls.Docking
{
    /// <summary>
    /// Krypton-style dockspace control. A dockspace owns one tab header and hosts
    /// the dock panels as pages inside that cell.
    /// </summary>
    [ToolboxItem(false)]
    [DesignerCategory("code")]
    [DesignTimeVisible(false)]
    [Designer("TheTechIdea.Beep.Winform.Controls.Design.Server.Docking.Designers.BeepDockspaceDesigner, TheTechIdea.Beep.Winform.Controls.Design.Server")]
    [DefaultProperty(nameof(DockPosition))]
    public class BeepDockspace : Panel
    {
        public const int HeaderHeight = 26;

        private const int TabMaxWidth = 160;
        private const int TabTextPadding = 6;
        private const int CaptionButtonSize = 18;
        private const int CaptionButtonSpacing = 4;

        private readonly Dictionary<string, Rectangle> _tabBounds = new(StringComparer.Ordinal);
        private Rectangle _closeBtnRect;
        private Rectangle _dropDownBtnRect;
        private Rectangle _pinBtnRect;
        private readonly DockspaceHeaderHitSurface _headerSurface;
        private BeepDockingManager _manager;
        private DockPosition _dockPosition = DockPosition.Left;
        private DockspaceHeaderStyle _headerStyle = DockspaceHeaderStyle.Document;
        private string _activePanelKey = string.Empty;
        private DockingThemeColors _themeColors = DockingThemeColors.Default;
        private int _lastVisiblePageCount = -1;

        public BeepDockspace()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.Selectable, true);

            BorderStyle = BorderStyle.None;
            MinimumSize = new Size(150, 150);
            TabStop = true;

            _headerSurface = new DockspaceHeaderHitSurface(this);
            Controls.Add(_headerSurface);
            _headerSurface.BringToFront();
        }

        [Category("Docking")]
        [Description("The docking manager that owns this dockspace.")]
        [Browsable(false)]
        [TypeConverter(typeof(ReferenceConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public BeepDockingManager Manager
        {
            get => _manager;
            set
            {
                if (ReferenceEquals(_manager, value))
                    return;

                _manager?.DetachDockspace(this);
                _manager = value;
                _manager?.AttachDockspace(this);
                _manager?.ApplyThemeToDockspace(this);
                SyncPanelDockingProperties();
                Invalidate();
            }
        }

        [Category("Docking")]
        [Description("The edge represented by this dockspace.")]
        [DefaultValue(DockPosition.Left)]
        public DockPosition DockPosition
        {
            get => _dockPosition;
            set
            {
                if (_dockPosition == value)
                    return;

                _dockPosition = value;
                SyncPanelDockingProperties();
                Invalidate();
            }
        }

        [Category("Docking")]
        [Description("Visual style used by the dockspace-owned header and tabs.")]
        [DefaultValue(DockspaceHeaderStyle.Document)]
        public DockspaceHeaderStyle HeaderStyle
        {
            get => _headerStyle;
            set
            {
                if (_headerStyle == value)
                    return;

                _headerStyle = value;
                UpdateTabBounds(GetPanels());
                Invalidate();
            }
        }

        [Category("Docking")]
        [Description("Key of the active panel page in this dockspace.")]
        [DefaultValue("")]
        public string ActivePanelKey
        {
            get => _activePanelKey;
            set
            {
                string safeValue = value ?? string.Empty;
                if (_activePanelKey == safeValue)
                    return;

                SetActivePanelKey(safeValue);
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IReadOnlyList<DockPanel> Panels => GetPanels();

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DockPanel ActivePanel => ResolveActivePanel();

        /// <summary>
        /// Raised when the dockspace selects a page and the design surface should select
        /// the matching DockPanel component. This mirrors Krypton's dockspace event model,
        /// where the dockspace owns page selection and higher layers react to it.
        /// </summary>
        public event EventHandler<DockPanel> PanelSelectionRequested;

        /// <summary>
        /// Raised when the active page changes in this dockspace cell.
        /// Mirrors Krypton's workspace cell selected-page change path.
        /// </summary>
        public event EventHandler<DockspacePageEventArgs> SelectedPageChanged;

        /// <summary>
        /// Raised when the dockspace visible tab count changes.
        /// Mirrors Krypton's TabVisibleCountChanged path.
        /// </summary>
        public event EventHandler TabVisibleCountChanged;

        /// <summary>
        /// Raised when the dockspace close button is clicked for the selected page.
        /// Mirrors Krypton's PageCloseClicked event.
        /// </summary>
        public event EventHandler<DockspacePageEventArgs> PageCloseClicked;

        /// <summary>
        /// Raised when the dockspace auto-hide/pin button is clicked for the selected page.
        /// Mirrors Krypton's PageAutoHiddenClicked event.
        /// </summary>
        public event EventHandler<DockspacePageEventArgs> PageAutoHiddenClicked;

        /// <summary>
        /// Raised when the dockspace drop-down button or context menu is requested.
        /// Mirrors Krypton's PageDropDownClicked event.
        /// </summary>
        public event EventHandler<DockspaceDropDownEventArgs> PageDropDownClicked;

        /// <summary>
        /// Raised when a page tab or header is double-clicked.
        /// Mirrors Krypton's PagesDoubleClicked event.
        /// </summary>
        public event EventHandler<DockspacePagesEventArgs> PagesDoubleClicked;

        /// <summary>
        /// Raised before a page tab drag is handled by the docking layer.
        /// Mirrors Krypton's BeforePageDrag event.
        /// </summary>
        public event EventHandler<DockspacePageDragCancelEventArgs> BeforePageDrag;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsDesigning =>
            Site?.DesignMode == true ||
            LicenseManager.UsageMode == LicenseUsageMode.Designtime ||
            IsWinFormsDesignerProcess();

        internal void ApplyDockingTheme(DockingThemeColors colors)
        {
            _themeColors = colors ?? DockingThemeColors.Default;
            BackColor = _themeColors.PanelBackColor;
            ForeColor = _themeColors.PanelForeColor;

            foreach (DockPanel panel in GetPanels())
                panel.ApplyDockingTheme(_themeColors);

            Invalidate();
        }

        public void ActivatePanel(DockPanel panel)
        {
            ActivatePanel(panel, true);
        }

        /// <summary>
        /// Krypton-style page selection API for the dockspace cell.
        /// </summary>
        public void SelectPage(string panelKey)
        {
            DockPanel panel = PageForKey(panelKey);
            if (panel != null)
                ActivatePanel(panel);
        }

        /// <summary>
        /// Returns the page with the specified key from this dockspace.
        /// </summary>
        public DockPanel PageForKey(string panelKey)
        {
            if (string.IsNullOrWhiteSpace(panelKey))
                return null;

            return GetPanels()
                .FirstOrDefault(panel => string.Equals(panel.Key, panelKey, StringComparison.Ordinal));
        }

        /// <summary>
        /// Returns the visible pages in the same dockspace cell as the named page.
        /// Mirrors Krypton's CellVisiblePages pattern.
        /// </summary>
        public DockPanel[] CellVisiblePages(string panelKey)
        {
            return PageForKey(panelKey) == null
                ? Array.Empty<DockPanel>()
                : GetPanels().ToArray();
        }

        public bool SelectTabAt(Point clientPoint)
        {
            if (clientPoint.Y < 0 || clientPoint.Y > HeaderHeight)
                return false;

            DockPanel tabPanel = HitTestTab(clientPoint);
            if (tabPanel == null)
                return false;

            ActivatePanel(tabPanel, true);
            return true;
        }

        public DockPanel HitTestTabAt(Point clientPoint)
        {
            if (clientPoint.Y < 0 || clientPoint.Y > HeaderHeight)
                return null;

            return HitTestTab(clientPoint);
        }

        public bool HandleHeaderMouseDown(Point clientPoint, MouseButtons button)
        {
            if (clientPoint.Y < 0 || clientPoint.Y > HeaderHeight)
                return false;

            if (button == MouseButtons.Right)
            {
                DockPanel contextPanel = HitTestTab(clientPoint) ?? ActivePanel;
                if (contextPanel == null)
                    return false;

                ActivatePanel(contextPanel, true);
                ShowActivePageMenu(contextPanel, clientPoint);
                return true;
            }

            if (button != MouseButtons.Left)
                return false;

            DockPanel active = ActivePanel;
            if (active != null)
            {
                UpdateButtonRects(active);

                if (active.CanClose && _closeBtnRect.Contains(clientPoint))
                {
                    OnPageCloseClicked(new DockspacePageEventArgs(active.Key, active));
                    return true;
                }

                if (_dropDownBtnRect.Contains(clientPoint))
                {
                    ShowActivePageMenu(active, clientPoint);
                    return true;
                }

                if (active.CanAutoHide && _pinBtnRect.Contains(clientPoint))
                {
                    OnPageAutoHiddenClicked(new DockspacePageEventArgs(active.Key, active));
                    return true;
                }
            }

            return SelectTabAt(clientPoint);
        }

        public bool HandleHeaderDoubleClick(Point clientPoint, MouseButtons button)
        {
            if (button != MouseButtons.Left || clientPoint.Y < 0 || clientPoint.Y > HeaderHeight)
                return false;

            DockPanel tabPanel = HitTestTab(clientPoint);
            IReadOnlyList<DockPanel> pages = tabPanel != null
                ? new[] { tabPanel }
                : GetPanels();

            if (pages.Count == 0)
                return false;

            var names = pages
                .Where(panel => !string.IsNullOrWhiteSpace(panel.Key))
                .Select(panel => panel.Key)
                .ToArray();

            OnPagesDoubleClicked(new DockspacePagesEventArgs(names, pages));

            return true;
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DockspacePageDragCancelEventArgs RaiseBeforePageDrag(
            DockPanel panel,
            Point screenPoint,
            Point elementOffset,
            Control control)
        {
            DockPanel[] pages = panel == null
                ? Array.Empty<DockPanel>()
                : new[] { panel };

            var args = new DockspacePageDragCancelEventArgs(panel, screenPoint, elementOffset, control ?? this, pages);
            OnBeforePageDrag(args);
            return args;
        }

        private void ActivatePanel(DockPanel panel, bool selectInDesigner)
        {
            if (panel == null || !Controls.Contains(panel))
                return;

            DockPanel previous = ResolveActivePanel();

            if (!string.IsNullOrWhiteSpace(panel.Key))
                SetActivePanelKey(panel.Key);

            LayoutPanels();
            panel.BringToFront();
            Invalidate();

            if (!ReferenceEquals(previous, panel) && string.IsNullOrWhiteSpace(panel.Key))
                OnSelectedPageChanged(new DockspacePageEventArgs(string.Empty, panel));

            if (IsDesigning)
            {
                if (selectInDesigner)
                {
                    OnPanelSelectionRequested(panel);
                    SelectPanelInDesigner(panel);
                }
            }
            else
            {
                _manager?.ActivatePanel(panel.Key);
            }
        }

        protected virtual void OnPanelSelectionRequested(DockPanel panel) =>
            PanelSelectionRequested?.Invoke(this, panel);

        protected virtual void OnSelectedPageChanged(DockspacePageEventArgs e) =>
            SelectedPageChanged?.Invoke(this, e);

        protected virtual void OnTabVisibleCountChanged(EventArgs e) =>
            TabVisibleCountChanged?.Invoke(this, e);

        protected virtual void OnPageCloseClicked(DockspacePageEventArgs e) =>
            PageCloseClicked?.Invoke(this, e);

        protected virtual void OnPageAutoHiddenClicked(DockspacePageEventArgs e) =>
            PageAutoHiddenClicked?.Invoke(this, e);

        protected virtual void OnPageDropDownClicked(DockspaceDropDownEventArgs e) =>
            PageDropDownClicked?.Invoke(this, e);

        protected virtual void OnPagesDoubleClicked(DockspacePagesEventArgs e) =>
            PagesDoubleClicked?.Invoke(this, e);

        protected virtual void OnBeforePageDrag(DockspacePageDragCancelEventArgs e) =>
            BeforePageDrag?.Invoke(this, e);

        private void SelectPanelInDesigner(DockPanel panel)
        {
            ISelectionService selection =
                Site?.GetService(typeof(ISelectionService)) as ISelectionService ??
                panel.Site?.GetService(typeof(ISelectionService)) as ISelectionService;

            selection?.SetSelectedComponents(new object[] { panel }, SelectionTypes.Replace);
        }

        public void SyncPages()
        {
            SyncPanelDockingProperties();
            LayoutPanels();
            NotifyVisiblePageCountChanged();
            Invalidate();
        }

        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);

            if (e.Control is DockPanel panel)
            {
                if (panel.Manager == null && _manager != null)
                    panel.Manager = _manager;

                panel.DockPosition = _dockPosition;
                panel.ApplyDockingTheme(_themeColors);

                if (string.IsNullOrWhiteSpace(_activePanelKey))
                    _activePanelKey = panel.Key ?? string.Empty;
            }

            LayoutPanels();
            NotifyVisiblePageCountChanged();
            Invalidate();
        }

        protected override void OnControlRemoved(ControlEventArgs e)
        {
            base.OnControlRemoved(e);

            if (e.Control is DockPanel panel &&
                string.Equals(_activePanelKey, panel.Key, StringComparison.Ordinal))
            {
                _activePanelKey = string.Empty;
            }

            LayoutPanels();
            NotifyVisiblePageCountChanged();
            Invalidate();
        }

        protected override void OnLayout(LayoutEventArgs levent)
        {
            base.OnLayout(levent);
            LayoutPanels();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            PaintHeader(e.Graphics);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            Focus();
            HandleHeaderMouseDown(e.Location, e.Button);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            Cursor = e.Y <= HeaderHeight ? GetHeaderCursor(e.Location) : Cursors.Default;
        }

        internal Cursor GetHeaderCursor(Point location) =>
            GetHeaderCursorCore(location);

        private Cursor GetHeaderCursorCore(Point location)
        {
            UpdateTabBounds(GetPanels());
            return HitTestTab(location) != null ||
                   _closeBtnRect.Contains(location) ||
                   _dropDownBtnRect.Contains(location) ||
                   _pinBtnRect.Contains(location)
                ? Cursors.Hand
                : Cursors.Default;
        }

        internal void PaintHeader(Graphics g)
        {
            int headerWidth = Width;
            var headerRect = new Rectangle(0, 0, Math.Max(0, headerWidth), HeaderHeight);
            if (headerRect.Width <= 0)
                return;

            var panels = GetPanels();
            DockPanel active = ResolveActivePanel();

            Color stripBackColor = _themeColors.HeaderBackColor;
            Color inactiveTabColor = _themeColors.InactiveTabBackColor;
            Color activeTabColor = _themeColors.ActiveTabBackColor;
            Color borderColor = _themeColors.TabBorderColor;

            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            using (var brush = new SolidBrush(stripBackColor))
                g.FillRectangle(brush, headerRect);

            UpdateButtonRects(active);

            int buttonLeft = FirstButtonLeft();
            int tabsWidth = Math.Max(0, buttonLeft - 2);
            int tabWidth = panels.Count == 0
                ? tabsWidth
                : Math.Max(1, Math.Min(TabMaxWidth, tabsWidth / panels.Count));
            int x = 0;

            _tabBounds.Clear();
            foreach (DockPanel panel in panels)
                panel.TabBounds = Rectangle.Empty;

            using (var font = new Font("Segoe UI", 9f, FontStyle.Regular))
            {
                foreach (DockPanel panel in panels)
                {
                    if (x >= tabsWidth)
                        break;

                    var tabRect = new Rectangle(x, 0, Math.Min(tabWidth, tabsWidth - x), HeaderHeight);
                    panel.TabBounds = tabRect;
                    if (!string.IsNullOrWhiteSpace(panel.Key))
                        _tabBounds[panel.Key] = tabRect;

                    bool isActive = ReferenceEquals(panel, active);
                    Color tabBack = isActive ? activeTabColor : inactiveTabColor;
                    Color tabFore = isActive
                        ? _themeColors.ActiveTabForeColor
                        : _themeColors.InactiveTabForeColor;
                    bool showTabIcon = DockingCaptionPainter.HasTabIcon(panel.IconPath);

                    DrawTabSurface(g, tabRect, tabBack, stripBackColor, borderColor, isActive);

                    if (showTabIcon)
                        DockingCaptionPainter.PaintTabIcon(g, tabRect, panel.IconPath, tabFore);

                    int textLeft = tabRect.Left + DockingCaptionPainter.GetTabContentLeft(showTabIcon);
                    var textRect = new Rectangle(
                        textLeft,
                        tabRect.Top,
                        Math.Max(0, tabRect.Right - textLeft - TabTextPadding),
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

                    x += tabRect.Width;
                }
            }

            Color buttonTint = _themeColors.HeaderButtonForeColor;
            DrawCaptionButton(g, _dropDownBtnRect, buttonTint, CaptionButtonType.DropDown);
            DrawCaptionButton(g, _pinBtnRect, buttonTint, CaptionButtonType.Pin);
            DrawCaptionButton(g, _closeBtnRect, buttonTint, CaptionButtonType.Close);

            using (var pen = new Pen(borderColor))
                g.DrawLine(pen, 0, HeaderHeight - 1, headerRect.Width - 1, HeaderHeight - 1);
        }

        private void DrawTabSurface(
            Graphics g,
            Rectangle tabRect,
            Color tabBack,
            Color stripBack,
            Color borderColor,
            bool isActive)
        {
            Rectangle r = tabRect;

            if (_headerStyle == DockspaceHeaderStyle.Buttons)
                r = Rectangle.Inflate(tabRect, -2, -3);
            else if (_headerStyle == DockspaceHeaderStyle.Document && isActive)
                r = new Rectangle(tabRect.X, tabRect.Y + 1, tabRect.Width, tabRect.Height);

            switch (_headerStyle)
            {
                case DockspaceHeaderStyle.Flat:
                    using (var brush = new SolidBrush(tabBack))
                        g.FillRectangle(brush, r);
                    using (var pen = new Pen(borderColor))
                        g.DrawRectangle(pen, r.X, r.Y, Math.Max(0, r.Width - 1), Math.Max(0, r.Height - 1));
                    break;

                case DockspaceHeaderStyle.Underline:
                    using (var brush = new SolidBrush(isActive ? Blend(tabBack, stripBack, 0.18f) : stripBack))
                        g.FillRectangle(brush, r);
                    if (isActive)
                    {
                        using var underline = new SolidBrush(tabBack);
                        g.FillRectangle(underline, r.Left + 4, HeaderHeight - 4, Math.Max(0, r.Width - 8), 3);
                    }
                    using (var pen = new Pen(borderColor))
                        g.DrawLine(pen, r.Right - 1, 5, r.Right - 1, HeaderHeight - 6);
                    break;

                case DockspaceHeaderStyle.Buttons:
                    using (var brush = new SolidBrush(tabBack))
                        g.FillRectangle(brush, r);
                    using (var pen = new Pen(borderColor))
                        g.DrawRectangle(pen, r.X, r.Y, Math.Max(0, r.Width - 1), Math.Max(0, r.Height - 1));
                    break;

                case DockspaceHeaderStyle.Document:
                default:
                    using (var brush = new SolidBrush(tabBack))
                        g.FillRectangle(brush, r);
                    using (var pen = new Pen(borderColor))
                    {
                        g.DrawLine(pen, r.Left, r.Top, r.Right - 1, r.Top);
                        g.DrawLine(pen, r.Left, r.Top, r.Left, r.Bottom - 1);
                        g.DrawLine(pen, r.Right - 1, r.Top, r.Right - 1, r.Bottom - 1);
                        if (!isActive)
                            g.DrawLine(pen, r.Left, r.Bottom - 1, r.Right - 1, r.Bottom - 1);
                    }
                    break;
            }
        }

        private void DrawCaptionButton(Graphics g, Rectangle r, Color color, CaptionButtonType type)
        {
            if (r.IsEmpty)
                return;

            string icon = type switch
            {
                CaptionButtonType.Close => DockingCaptionPainter.CaptionIcons.Close,
                CaptionButtonType.DropDown => DockingCaptionPainter.CaptionIcons.DropDown,
                CaptionButtonType.Pin => DockingCaptionPainter.CaptionIcons.Pin,
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
                case CaptionButtonType.DropDown:
                    DockingCaptionPainter.PaintDropDownFallback(g, r, color);
                    break;
                case CaptionButtonType.Pin:
                    DockingCaptionPainter.PaintPinFallback(g, r, color);
                    break;
            }
        }

        private void UpdateButtonRects(DockPanel active)
        {
            int y = (HeaderHeight - CaptionButtonSize) / 2;
            int width = Width;
            int x = width - 4;

            _closeBtnRect = Rectangle.Empty;
            _dropDownBtnRect = Rectangle.Empty;
            _pinBtnRect = Rectangle.Empty;

            if (active?.CanClose == true)
            {
                x -= CaptionButtonSize + CaptionButtonSpacing;
                _closeBtnRect = new Rectangle(x, y, CaptionButtonSize, CaptionButtonSize);
            }

            if (active?.CanAutoHide == true)
            {
                x -= CaptionButtonSize + CaptionButtonSpacing;
                _pinBtnRect = new Rectangle(x, y, CaptionButtonSize, CaptionButtonSize);
            }

            if (active != null)
            {
                x -= CaptionButtonSize + CaptionButtonSpacing;
                _dropDownBtnRect = new Rectangle(x, y, CaptionButtonSize, CaptionButtonSize);
            }
        }

        private int FirstButtonLeft()
        {
            int width = Width;
            int left = width;
            if (!_closeBtnRect.IsEmpty) left = Math.Min(left, _closeBtnRect.Left);
            if (!_dropDownBtnRect.IsEmpty) left = Math.Min(left, _dropDownBtnRect.Left);
            if (!_pinBtnRect.IsEmpty) left = Math.Min(left, _pinBtnRect.Left);
            return left == width ? width : Math.Max(0, left - CaptionButtonSpacing);
        }

        private void ShowActivePageMenu(DockPanel active, Point location)
        {
            if (active == null)
                return;

            var menu = new ContextMenuStrip();

            if (active.CanFloat)
            {
                var floating = new ToolStripMenuItem("Floating");
                floating.Click += (s, e) =>
                {
                    if (!IsDesigning && !string.IsNullOrWhiteSpace(active.Key))
                        _manager?.MakeFloatingRequest(active.Key);
                };
                menu.Items.Add(floating);
            }

            if (active.CanAutoHide)
            {
                var autoHide = new ToolStripMenuItem("Auto Hide");
                autoHide.Click += (s, e) =>
                {
                    if (!IsDesigning && !string.IsNullOrWhiteSpace(active.Key))
                        _manager?.MakeAutoHiddenRequest(active.Key);
                };
                menu.Items.Add(autoHide);
            }

            if (active.CanClose)
            {
                if (menu.Items.Count > 0)
                    menu.Items.Add(new ToolStripSeparator());

                var close = new ToolStripMenuItem("Close");
                close.Click += (s, e) =>
                {
                    if (!IsDesigning && _manager != null && !string.IsNullOrWhiteSpace(active.Key))
                        _manager.CloseRequest(new[] { active.Key });
                };
                menu.Items.Add(close);
            }

            Point screenPosition = PointToScreen(location);
            var args = new DockspaceDropDownEventArgs(menu, active, screenPosition);
            OnPageDropDownClicked(args);

            if (args.Cancel || menu.Items.Count == 0)
                menu.Dispose();
            else
                menu.Show(this, location);
        }

        private DockPanel HitTestTab(Point location)
        {
            var panels = GetPanels();
            UpdateTabBounds(panels);

            foreach (DockPanel panel in panels)
            {
                if (!panel.TabBounds.IsEmpty && panel.TabBounds.Contains(location))
                    return panel;
            }

            return null;
        }

        private static Color Blend(Color first, Color second, float amount)
        {
            amount = Math.Max(0f, Math.Min(1f, amount));
            float inverse = 1f - amount;

            return Color.FromArgb(
                first.A,
                (int)(first.R * inverse + second.R * amount),
                (int)(first.G * inverse + second.G * amount),
                (int)(first.B * inverse + second.B * amount));
        }

        private void UpdateTabBounds(IReadOnlyList<DockPanel> panels)
        {
            UpdateButtonRects(ResolveActivePanel());

            int buttonLeft = FirstButtonLeft();
            int tabsWidth = Math.Max(0, buttonLeft - 2);
            int tabWidth = panels.Count == 0
                ? tabsWidth
                : Math.Max(1, Math.Min(TabMaxWidth, tabsWidth / panels.Count));
            int x = 0;

            _tabBounds.Clear();
            foreach (DockPanel panel in panels)
            {
                panel.TabBounds = Rectangle.Empty;
                if (x >= tabsWidth)
                    continue;

                Rectangle tabRect = new Rectangle(x, 0, Math.Min(tabWidth, tabsWidth - x), HeaderHeight);
                panel.TabBounds = tabRect;
                if (!string.IsNullOrWhiteSpace(panel.Key))
                    _tabBounds[panel.Key] = tabRect;

                x += tabRect.Width;
            }
        }

        private DockPanel ResolveActivePanel()
        {
            var panels = GetPanels();
            if (panels.Count == 0)
                return null;

            if (!string.IsNullOrWhiteSpace(_activePanelKey))
            {
                DockPanel active = panels.FirstOrDefault(p => string.Equals(p.Key, _activePanelKey, StringComparison.Ordinal));
                if (active != null)
                    return active;
            }

            DockPanel front = panels
                .OrderByDescending(p => Controls.GetChildIndex(p))
                .FirstOrDefault();

            return front ?? panels[0];
        }

        private List<DockPanel> GetPanels() =>
            Controls.OfType<DockPanel>()
                .Where(panel => !panel.IsDisposed)
                .OrderBy(panel => panel.TabIndex)
                .ThenBy(panel => Controls.IndexOf(panel))
                .ToList();

        private void LayoutPanels()
        {
            Rectangle pageBounds = new Rectangle(0, HeaderHeight, Width, Math.Max(0, Height - HeaderHeight));
            DockPanel active = ResolveActivePanel();

            foreach (DockPanel panel in GetPanels())
            {
                panel.Bounds = pageBounds;
                panel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
                panel.DockPosition = _dockPosition;

                if (_manager != null && panel.Manager == null)
                    panel.Manager = _manager;
            }

            active?.BringToFront();
            LayoutHeaderSurface();
            _headerSurface?.BringToFront();
        }

        private void LayoutHeaderSurface()
        {
            if (_headerSurface == null || _headerSurface.IsDisposed)
                return;

            _headerSurface.Bounds = new Rectangle(0, 0, Math.Max(0, Width), HeaderHeight);
            _headerSurface.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        }

        private void SetActivePanelKey(string safeValue)
        {
            DockPanel previous = ResolveActivePanel();

            _activePanelKey = safeValue ?? string.Empty;
            LayoutPanels();
            Invalidate();

            DockPanel current = ResolveActivePanel();
            if (!ReferenceEquals(previous, current))
                OnSelectedPageChanged(new DockspacePageEventArgs(current?.Key ?? string.Empty, current));
        }

        private void NotifyVisiblePageCountChanged()
        {
            int count = GetPanels().Count;
            if (_lastVisiblePageCount == count)
                return;

            _lastVisiblePageCount = count;
            OnTabVisibleCountChanged(EventArgs.Empty);
        }

        private void SyncPanelDockingProperties()
        {
            foreach (DockPanel panel in GetPanels())
            {
                if (_manager != null && panel.Manager == null)
                    panel.Manager = _manager;

                panel.DockPosition = _dockPosition;
            }
        }

        private static Color GetReadableTextColor(Color background) =>
            DockingThemeColors.GetReadableTextColor(background);

        private enum CaptionButtonType
        {
            Close,
            DropDown,
            Pin
        }

        private sealed class DockspaceHeaderHitSurface : Control
        {
            private readonly BeepDockspace _owner;

            public DockspaceHeaderHitSurface(BeepDockspace owner)
            {
                _owner = owner;
                SetStyle(ControlStyles.OptimizedDoubleBuffer |
                         ControlStyles.AllPaintingInWmPaint |
                         ControlStyles.UserPaint |
                         ControlStyles.ResizeRedraw |
                         ControlStyles.Selectable, false);
                TabStop = false;
                Cursor = Cursors.Hand;
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);
                _owner.PaintHeader(e.Graphics);
            }

            protected override void OnMouseDown(MouseEventArgs e)
            {
                base.OnMouseDown(e);
                _owner.Focus();
                _owner.HandleHeaderMouseDown(e.Location, e.Button);
            }

            protected override void OnMouseDoubleClick(MouseEventArgs e)
            {
                base.OnMouseDoubleClick(e);
                _owner.HandleHeaderDoubleClick(e.Location, e.Button);
            }

            protected override void OnMouseMove(MouseEventArgs e)
            {
                base.OnMouseMove(e);
                Cursor = _owner.GetHeaderCursor(e.Location);
            }
        }

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
