using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Docking.Helpers;
using TheTechIdea.Beep.Winform.Controls.Docking.Layoutmanagers;
using TheTechIdea.Beep.Winform.Controls.Docking.Models;
using TheTechIdea.Beep.Winform.Controls.Docking.Painters;
using TheTechIdea.Beep.Winform.Controls.Docking.Painters.Caption;

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
        private const int CaptionButtonSize = 18;
        private const int CaptionButtonSpacing = 4;

        private readonly CaptionLayoutManager _captionLayout = new CaptionLayoutManager
        {
            ButtonSize = CaptionButtonSize,
            ButtonSpacing = CaptionButtonSpacing,
            MinTabWidth = 1,
            MaxTabWidth = TabMaxWidth
        };
        private BeepDockingManager _manager;

        /// <summary>Control style driving header background/border rendering. Set by the manager.</summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        internal BeepControlStyle ControlStyle { get; set; } = BeepControlStyle.Material3;
        private DockPosition _dockPosition = DockPosition.Left;
        private string _activePanelKey = string.Empty;
        private DockingThemeColors _themeColors = DockingThemeColors.Default;
        private readonly ToolTip _tabToolTip = new ToolTip();

        // Tab drag-to-float/dock state.
        private DockPanel _dragPanel;
        private Point _dragStartScreen;
        private bool _dragArmed;
        private bool _isDragging;

        public BeepDockspace()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.ResizeRedraw, true);

            BorderStyle = BorderStyle.None;
            MinimumSize = new Size(150, 150);
            _tabToolTip.SetToolTip(this, string.Empty);
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
                RecomputeCaptionLayout();

                switch (_captionLayout.HitTestButton(clientPoint))
                {
                    case CaptionButtonKind.Close when active.CanClose:
                        _manager?.ClosePanel(active.Key);
                        return true;
                    case CaptionButtonKind.DropDown:
                        ShowActivePageMenu(active, clientPoint);
                        return true;
                    case CaptionButtonKind.Pin when active.CanAutoHide:
                        _manager?.MakeAutoHiddenRequest(active.Key);
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

            // Reset drag state on every mouse-down.
            _dragArmed = false;
            _isDragging = false;
            _dragPanel = null;

            if (e.Button != MouseButtons.Left || e.Y > HeaderHeight)
                return;

            if (_captionLayout.HitTestButton(e.Location) != null)
            {
                HandleHeaderMouseDown(e.Location, e.Button);
                return;
            }

            var tab = HitTestTab(e.Location);
            if (tab != null)
            {
                _dragPanel = tab;
                _dragStartScreen = PointToScreen(e.Location);
                _dragArmed = true;
            }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            if (e.Y < 0 || e.Y > HeaderHeight)
                return;

            DockPanel tab = HitTestTab(e.Location);
            if (tab == null)
                return;

            if (e.Button == MouseButtons.Middle && tab.CanClose)
            {
                _manager?.ClosePanel(tab.Key);
            }
            else if (e.Button == MouseButtons.Right)
            {
                ShowTabContextMenu(tab, e.Location);
            }
            else if (e.Button == MouseButtons.Left)
            {
                ActivatePanel(tab);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (e.Y <= HeaderHeight)
                RecomputeCaptionLayout();

            // Tab drag to float/dock — start when system drag threshold is crossed.
            if (_dragArmed && (e.Button & MouseButtons.Left) != 0 && !_isDragging)
            {
                Point screen = PointToScreen(e.Location);
                Size delta = new Size(
                    Math.Abs(screen.X - _dragStartScreen.X),
                    Math.Abs(screen.Y - _dragStartScreen.Y));
                if (delta.Width > SystemInformation.DragSize.Width ||
                    delta.Height > SystemInformation.DragSize.Height)
                {
                    _isDragging = true;
                    _manager?.BeginCaptionDrag(_dragPanel, _dragStartScreen);
                    _manager?.UpdateCaptionDrag(screen);
                }
            }

            if (_isDragging && (e.Button & MouseButtons.Left) != 0)
            {
                _manager?.UpdateCaptionDrag(PointToScreen(e.Location));
                return;
            }

            var hoveredTab = e.Y <= HeaderHeight ? _captionLayout.HitTestTab(e.Location) : null;
            if (hoveredTab?.Tag is DockPanel panel && !string.IsNullOrEmpty(panel.Title))
            {
                _tabToolTip.Show(panel.Title, this, e.X + 12, e.Y + 12, 3000);
            }
            else
            {
                _tabToolTip.Hide(this);
            }

            Cursor = e.Y <= HeaderHeight &&
                     (hoveredTab != null ||
                      _captionLayout.HitTestButton(e.Location) != null)
                ? Cursors.Hand
                : Cursors.Default;
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (_isDragging)
            {
                _manager?.EndCaptionDrag(PointToScreen(e.Location), commit: true);
            }

            _dragArmed = false;
            _isDragging = false;
            _dragPanel = null;
        }

        /// <summary>Builds the ordered caption button set (right-to-left placement) for the active page.</summary>
        private List<CaptionButtonKind> BuildCaptionButtons(DockPanel active)
        {
            var buttons = new List<CaptionButtonKind>(3);
            if (active?.CanClose == true) buttons.Add(CaptionButtonKind.Close);
            if (active?.CanAutoHide == true) buttons.Add(CaptionButtonKind.Pin);
            if (active != null) buttons.Add(CaptionButtonKind.DropDown);
            return buttons;
        }

        /// <summary>
        /// Recomputes header geometry into the shared caption layout manager and mirrors tab
        /// rects back onto each page's <see cref="DockPanel.TabBounds"/> for compatibility.
        /// </summary>
        private List<CaptionButtonKind> RecomputeCaptionLayout()
        {
            var panels = GetPanels();
            DockPanel active = ResolveActivePanel();
            var buttons = BuildCaptionButtons(active);

            var tabs = new List<CaptionTabModel>(panels.Count);
            foreach (DockPanel panel in panels)
            {
                tabs.Add(new CaptionTabModel
                {
                    Key = panel.Key,
                    Title = panel.Title,
                    IconPath = panel.IconPath,
                    IsDirty = panel.IsDirty,
                    IsActive = ReferenceEquals(panel, active),
                    Tag = panel
                });
            }

            _captionLayout.Compute(Width, HeaderHeight, tabs, buttons);

            foreach (DockPanel panel in panels)
                panel.TabBounds = Rectangle.Empty;
            foreach (var kv in _captionLayout.TabRects)
            {
                if (kv.Key.Tag is DockPanel dp)
                    dp.TabBounds = kv.Value;
            }

            return buttons;
        }

        private void DrawHeader(Graphics g)
        {
            var buttons = RecomputeCaptionLayout();

            var ctx = new DockingPainterContext
            {
                Colors = _themeColors,
                Style = ControlStyle,
                Bounds = new Rectangle(0, 0, Width, HeaderHeight),
                IsDesignTime = IsDesigning
            };

            DockingPainterFactory.GetRenderers(ControlStyle).Caption.Paint(g, ctx, _captionLayout, buttons);
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
                autoHide.Click += (s, e) => _manager?.MakeAutoHiddenRequest(active.Key);
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

        private void ShowTabContextMenu(DockPanel panel, Point location)
        {
            if (panel == null) return;

            if (_manager?.TryShowPanelContextMenu(panel, location) == true)
                return;

            // Activate the panel when right-clicking its tab
            if (!ReferenceEquals(ActivePanel, panel))
                ActivatePanel(panel);

            var menu = new ContextMenuStrip();

            if (panel.CanFloat)
            {
                var floating = new ToolStripMenuItem("Floating");
                floating.Click += (s, e) => _manager?.FloatPanel(panel.Key);
                menu.Items.Add(floating);
            }

            if (panel.CanAutoHide)
            {
                string label = panel.State == DockPanelState.AutoHidden ? "Dock" : "Auto Hide";
                var autoHide = new ToolStripMenuItem(label);
                autoHide.Click += (s, e) =>
                {
                    if (panel.State == DockPanelState.AutoHidden)
                        _manager?.RestoreAutoHiddenPanel(panel.Key);
                    else
                        _manager?.MakeAutoHiddenRequest(panel.Key);
                };
                menu.Items.Add(autoHide);
            }

            if (panel.CanClose)
            {
                if (menu.Items.Count > 0)
                    menu.Items.Add(new ToolStripSeparator());

                var close = new ToolStripMenuItem("Close");
                close.Click += (s, e) => _manager?.ClosePanel(panel.Key);
                menu.Items.Add(close);
            }

            if (menu.Items.Count > 0)
                menu.Show(this, location);
            else
                menu.Dispose();
        }

        private DockPanel HitTestTab(Point location)
        {
            RecomputeCaptionLayout();
            var tab = _captionLayout.HitTestTab(location);
            return tab?.Tag as DockPanel;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _tabToolTip?.Dispose();
            base.Dispose(disposing);
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
