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
        private BeepDockingManager _manager;
        private DockPosition _dockPosition = DockPosition.Left;
        private string _activePanelKey = string.Empty;
        private DockingThemeColors _themeColors = DockingThemeColors.Default;

        public BeepDockspace()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.ResizeRedraw, true);

            BorderStyle = BorderStyle.None;
            MinimumSize = new Size(150, 150);
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

                _manager = value;
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

                _activePanelKey = safeValue;
                LayoutPanels();
                Invalidate();
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IReadOnlyList<DockPanel> Panels => GetPanels();

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DockPanel ActivePanel => ResolveActivePanel();

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

            if (button != MouseButtons.Left)
                return false;

            DockPanel active = ActivePanel;
            if (active != null)
            {
                UpdateButtonRects(active);

                if (active.CanClose && _closeBtnRect.Contains(clientPoint))
                {
                    _manager?.ClosePanel(active.Key);
                    return true;
                }

                if (_dropDownBtnRect.Contains(clientPoint))
                {
                    ShowActivePageMenu(active, clientPoint);
                    return true;
                }

                if (active.CanAutoHide && _pinBtnRect.Contains(clientPoint))
                {
                    _manager?.AutoHidePanel(active.Key);
                    return true;
                }
            }

            return SelectTabAt(clientPoint);
        }

        private void ActivatePanel(DockPanel panel, bool selectInDesigner)
        {
            if (panel == null || !Controls.Contains(panel))
                return;

            if (!string.IsNullOrWhiteSpace(panel.Key))
                ActivePanelKey = panel.Key;

            LayoutPanels();
            panel.BringToFront();
            Invalidate();

            if (IsDesigning)
            {
                if (selectInDesigner)
                    SelectPanelInDesigner(panel);
            }
            else
            {
                _manager?.ActivatePanel(panel.Key);
            }
        }

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
            DrawHeader(e.Graphics);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            HandleHeaderMouseDown(e.Location, e.Button);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            Cursor = e.Y <= HeaderHeight &&
                     (HitTestTab(e.Location) != null ||
                      _closeBtnRect.Contains(e.Location) ||
                      _dropDownBtnRect.Contains(e.Location) ||
                      _pinBtnRect.Contains(e.Location))
                ? Cursors.Hand
                : Cursors.Default;
        }

        private void DrawHeader(Graphics g)
        {
            var headerRect = new Rectangle(0, 0, Width, HeaderHeight);
            var panels = GetPanels();
            DockPanel active = ResolveActivePanel();

            Color stripBackColor = _themeColors.HeaderBackColor;
            Color inactiveTabColor = _themeColors.InactiveTabBackColor;
            Color activeTabColor = _themeColors.ActiveTabBackColor;
            Color borderColor = _themeColors.TabBorderColor;

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
                g.DrawLine(pen, 0, HeaderHeight - 1, Width - 1, HeaderHeight - 1);
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
            int x = Width - 4;

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
            int left = Width;
            if (!_closeBtnRect.IsEmpty) left = Math.Min(left, _closeBtnRect.Left);
            if (!_dropDownBtnRect.IsEmpty) left = Math.Min(left, _dropDownBtnRect.Left);
            if (!_pinBtnRect.IsEmpty) left = Math.Min(left, _pinBtnRect.Left);
            return left == Width ? Width : Math.Max(0, left - CaptionButtonSpacing);
        }

        private void ShowActivePageMenu(DockPanel active, Point location)
        {
            if (active == null)
                return;

            var menu = new ContextMenuStrip();

            if (active.CanFloat)
            {
                var floating = new ToolStripMenuItem("Floating");
                floating.Click += (s, e) => _manager?.FloatPanel(active.Key);
                menu.Items.Add(floating);
            }

            if (active.CanAutoHide)
            {
                var autoHide = new ToolStripMenuItem("Auto Hide");
                autoHide.Click += (s, e) => _manager?.AutoHidePanel(active.Key);
                menu.Items.Add(autoHide);
            }

            if (active.CanClose)
            {
                if (menu.Items.Count > 0)
                    menu.Items.Add(new ToolStripSeparator());

                var close = new ToolStripMenuItem("Close");
                close.Click += (s, e) => _manager?.ClosePanel(active.Key);
                menu.Items.Add(close);
            }

            if (menu.Items.Count > 0)
                menu.Show(this, location);
            else
                menu.Dispose();
        }

        private DockPanel HitTestTab(Point location)
        {
            var panels = GetPanels();
            if (panels.Count > 0 && panels.All(panel => panel.TabBounds.IsEmpty))
                UpdateTabBounds(panels);

            foreach (DockPanel panel in panels)
            {
                if (!panel.TabBounds.IsEmpty && panel.TabBounds.Contains(location))
                    return panel;
            }

            return null;
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
