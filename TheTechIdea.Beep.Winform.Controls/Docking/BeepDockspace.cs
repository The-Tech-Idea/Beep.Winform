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
using TheTechIdea.Beep.Winform.Controls.Docking.Runtime;

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

        private readonly DockingPainterContext _paintContext = new DockingPainterContext();

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
        private bool _showHint = true;
        private Padding _dockPadding = Padding.Empty;
        private Models.TabStyle _tabPosition = Models.TabStyle.Top;

        // Tab drag-to-float/dock state.
        private DockPanel _dragPanel;
        private Point _dragStartScreen;
        private bool _dragArmed;
        private bool _isDragging;

        // Child-group splitters owned by this dockspace (one per gap between two child groups
        // when the layout tree's parent group has 2+ children). Keyed by the layout engine's
        // GroupId ({parentId}_child_{i}). Bounds are in dockspace-local coordinates.
        private readonly Dictionary<string, BeepDockSplitter> _ownedChildSplitters =
            new Dictionary<string, BeepDockSplitter>(StringComparer.Ordinal);

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

        /// <summary>
        /// Whether the dockspace should pop a tooltip showing a tab's panel title while the
       /// cursor hovers over it. Mirrors <c>DockPanelExt.ShowHint</c> from DockPanelSuite.
        /// </summary>
        [Category("Docking")]
        [Description("Show a tooltip with the panel title when the cursor hovers a tab.")]
        [DefaultValue(true)]
        public bool ShowHint
        {
            get => _showHint;
            set
            {
                if (_showHint == value) return;
                _showHint = value;
                if (!_showHint)
                    _tabToolTip.Hide(this);
            }
        }

        /// <summary>
        /// Inner padding (in pixels) applied to the dockspace's child panel bounds inside
        /// <see cref="LayoutPanels"/>. Useful for breathing room around panel content. Mirrors
        /// <c>DockPanelExt.DockPadding</c> from DockPanelSuite.
        /// </summary>
        [Category("Docking")]
        [Description("Inner padding inset applied to every child panel's bounds during layout.")]
        [DefaultValue(typeof(Padding), "0,0,0,0")]
        public Padding DockPadding
        {
            get => _dockPadding;
            set
            {
                if (_dockPadding == value) return;
                _dockPadding = value;
                LayoutPanels();
            }
        }

        [Category("Docking")]
        [Description("Tab strip position relative to the panel content area.")]
        [DefaultValue(typeof(Models.TabStyle), "Top")]
        public Models.TabStyle TabPosition
        {
            get => _tabPosition;
            set
            {
                if (_tabPosition == value) return;
                _tabPosition = value;
                LayoutPanels();
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
            DockingHelpers.IsWinFormsDesignerProcess();

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
            if (!IsInHeader(clientPoint))
                return false;

            DockPanel tabPanel = HitTestTab(clientPoint);
            if (tabPanel == null)
                return false;

            ActivatePanel(tabPanel, true);
            return true;
        }

        public DockPanel HitTestTabAt(Point clientPoint)
        {
            if (!IsInHeader(clientPoint))
                return null;

            return HitTestTab(clientPoint);
        }

        public bool HandleHeaderMouseDown(Point clientPoint, MouseButtons button)
        {
            if (!IsInHeader(clientPoint))
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

            if (e.Button != MouseButtons.Left || !IsInHeader(e.Location))
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
                // Capture mouse so OnMouseMove/OnMouseUp keep firing as the cursor leaves
                // the dockspace header during a tab drag-to-float/dock.
                Capture = true;
            }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            if (!IsInHeader(e.Location))
                return;

            DockPanel tab = HitTestTab(e.Location);
            if (tab == null)
                return;

            // Suppress the trailing click that fires after a drag commits — otherwise a drag
            // would re-activate the tab or pop a stray context menu.
            if (_isDragging)
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

            if (IsInHeader(e.Location))
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

            // Suppress tooltips while a drag is in progress or armed — otherwise they pop
            // under the ghost and flicker as the cursor leaves the tab.
            if (!_showHint || _isDragging || _dragArmed)
            {
                _tabToolTip.Hide(this);
                return;
            }

            var hoveredTab = IsInHeader(e.Location) ? _captionLayout.HitTestTab(e.Location) : null;
            if (hoveredTab?.Tag is DockPanel panel && !string.IsNullOrEmpty(panel.Title))
            {
                _tabToolTip.Show(panel.Title, this, e.X + 12, e.Y + 12, 3000);
            }
            else
            {
                _tabToolTip.Hide(this);
            }

            Cursor = IsInHeader(e.Location) &&
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
            if (Capture)
                Capture = false;
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
            var tabs = CaptionLayoutManager.BuildTabModels(panels, active);

            bool vertical = _tabPosition == Models.TabStyle.Left || _tabPosition == Models.TabStyle.Right;
            _captionLayout.IsVertical = vertical;
            _captionLayout.IsFlipped = _tabPosition == Models.TabStyle.Right;

            if (vertical)
                _captionLayout.Compute(HeaderHeight, Height, tabs, buttons);
            else
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
            Rectangle headerRect = GetHeaderRect();
            if (headerRect.IsEmpty)
                return;

            var buttons = RecomputeCaptionLayout();

            _paintContext.Update(_themeColors, ControlStyle, headerRect, IsDesigning);

            var renderers = DockingPainterFactory.GetRenderers(ControlStyle);
            renderers.Caption.Paint(g, _paintContext, _captionLayout, buttons);

            // Draw separator line along the content-adjacent edge.
            if (_tabPosition == Models.TabStyle.Left)
            {
                using var pen = new Pen(_themeColors.TabBorderColor);
                g.DrawLine(pen, headerRect.Right - 1, 0, headerRect.Right - 1, headerRect.Bottom - 1);
            }
            else if (_tabPosition == Models.TabStyle.Right)
            {
                using var pen = new Pen(_themeColors.TabBorderColor);
                g.DrawLine(pen, headerRect.X, 0, headerRect.X, headerRect.Bottom - 1);
            }
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
            {
                _tabToolTip?.Dispose();
                DisposeOwnedChildSplitters();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Reconciles the dockspace-owned child splitters with the splitter hits from a layout
        /// pass. The bounds are in <b>dockspace-local</b> coordinates (translated by the manager
        /// from the layout engine's container coordinates). Existing splitters are repositioned
        /// and re-themed; missing ones are created as children of this dockspace; orphaned
        /// splitters (no longer in the desired set) are disposed.
        /// </summary>
        /// <param name="desired">Hit records keyed by the layout engine's GroupId
        /// ({parentId}_child_{i}). Each tuple holds the dockspace-local bounds and orientation.</param>
        /// <param name="raisedByDockspace">Callback for splitter drag events. Receives the
        /// GroupId of the splitter being dragged so the manager can update the layout engine.</param>
        public void UpdateChildSplitters(
            IReadOnlyDictionary<string, (Rectangle Bounds, bool IsVertical)> desired,
            EventHandler<SplitterMovedEventArgs> raisedByDockspace)
        {
            if (desired == null)
                desired = new Dictionary<string, (Rectangle, bool)>();

            // Create / update existing.
            foreach (var kv in desired)
            {
                if (!_ownedChildSplitters.TryGetValue(kv.Key, out var splitter) || splitter.IsDisposed)
                {
                    splitter = new BeepDockSplitter
                    {
                        GroupId = kv.Key,
                        Dock = DockStyle.None   // dockspace-owned, positioned by bounds
                    };
                    splitter.ControlStyle = ControlStyle;
                    splitter.ApplyDockingTheme(_themeColors);
                    if (raisedByDockspace != null)
                        splitter.SplitterMoved += raisedByDockspace;
                    _ownedChildSplitters[kv.Key] = splitter;
                    Controls.Add(splitter);
                }

                splitter.Orientation = kv.Value.IsVertical
                    ? SplitterOrientation.Vertical
                    : SplitterOrientation.Horizontal;
                splitter.Bounds = kv.Value.Bounds;
                splitter.Visible = true;
            }

            // Dispose orphans.
            var orphanKeys = _ownedChildSplitters.Keys
                .Where(k => !desired.ContainsKey(k))
                .ToList();
            foreach (var key in orphanKeys)
            {
                var splitter = _ownedChildSplitters[key];
                _ownedChildSplitters.Remove(key);
                if (splitter == null || splitter.IsDisposed)
                    continue;
                if (raisedByDockspace != null)
                    splitter.SplitterMoved -= raisedByDockspace;
                if (Controls.Contains(splitter))
                    Controls.Remove(splitter);
                splitter.Dispose();
            }
        }

        /// <summary>Removes all dockspace-owned child splitters and disposes them.</summary>
        public void ClearChildSplitters()
        {
            var keys = _ownedChildSplitters.Keys.ToList();
            foreach (var key in keys)
            {
                var splitter = _ownedChildSplitters[key];
                _ownedChildSplitters.Remove(key);
                if (splitter == null || splitter.IsDisposed)
                    continue;
                if (Controls.Contains(splitter))
                    Controls.Remove(splitter);
                splitter.Dispose();
            }
        }

        private void DisposeOwnedChildSplitters()
        {
            foreach (var splitter in _ownedChildSplitters.Values)
            {
                if (splitter == null || splitter.IsDisposed)
                    continue;
                if (Controls.Contains(splitter))
                    Controls.Remove(splitter);
                splitter.Dispose();
            }
            _ownedChildSplitters.Clear();
        }

        /// <summary>
        /// Cancels a tab drag-in-progress (called by the manager on Escape). Resets local flags
        /// and releases mouse capture so the trailing click is treated as a normal click.
        /// </summary>
        internal void CancelDrag()
        {
            if (_isDragging)
                _manager?.CancelCaptionDrag();

            _dragArmed = false;
            _isDragging = false;
            _dragPanel = null;
            if (Capture)
                Capture = false;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if ((_isDragging || _dragArmed) && keyData == Keys.Escape)
            {
                CancelDrag();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
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

        public void LayoutPanels()
        {
            Rectangle pageBounds = GetContentRect();
            if (!_dockPadding.Equals(Padding.Empty))
            {
                pageBounds = new Rectangle(
                    pageBounds.X + _dockPadding.Left,
                    pageBounds.Y + _dockPadding.Top,
                    Math.Max(0, pageBounds.Width  - _dockPadding.Horizontal),
                    Math.Max(0, pageBounds.Height - _dockPadding.Vertical));
            }

            var panels = GetPanels();
            DockPanel active = ResolveActivePanel();

            // Group panels by their layout-tree group. When multiple groups co-exist in one
            // dockspace (nested child groups created by group-edge drops), use the layout
            // controller's GroupBounds to assign each group a sub-rectangle of the content area.
            var groupsById = panels.GroupBy(p => p.Group?.Id ?? string.Empty)
                                    .Where(g => !string.IsNullOrEmpty(g.Key))
                                    .ToList();

            if (groupsById.Count > 1 && _manager?.LayoutController != null)
            {
                var result = _manager.LayoutController.CalculateLayoutResult();
                if (result != null)
                {
                    // Compute the dockspace's position relative to the layout container (host form).
                    Point hostOffset = PointToScreen(Point.Empty);
                    if (_manager.HostForm != null)
                        hostOffset = _manager.HostForm.PointToClient(hostOffset);

                    foreach (var grp in groupsById)
                    {
                        Rectangle? groupRect = result.GetGroupBounds(grp.Key);
                        if (!groupRect.HasValue) continue;

                        Rectangle localRect = new Rectangle(
                            groupRect.Value.X - hostOffset.X,
                            groupRect.Value.Y - hostOffset.Y,
                            groupRect.Value.Width,
                            groupRect.Value.Height);

                        // Intersect with the dockspace's content area so panels cannot spill
                        // outside (e.g. into the header strip or past the dockspace edge).
                        localRect.Intersect(pageBounds);

                        foreach (DockPanel panel in grp)
                        {
                            panel.Bounds = localRect;
                            panel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
                            panel.DockPosition = _dockPosition;
                            if (_manager != null && panel.Manager == null)
                                panel.Manager = _manager;
                        }
                    }

                    active?.BringToFront();
                    return;
                }
            }

            // Single group (or no layout result available): all panels share the full content area.
            foreach (DockPanel panel in panels)
            {
                panel.Bounds = pageBounds;
                panel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
                panel.DockPosition = _dockPosition;

                if (_manager != null && panel.Manager == null)
                    panel.Manager = _manager;
            }

            active?.BringToFront();
        }

        /// <summary>Returns the rectangle occupied by the tab strip header.</summary>
        private Rectangle GetHeaderRect()
        {
            return _tabPosition switch
            {
                Models.TabStyle.Bottom => new Rectangle(0, Height - HeaderHeight, Width, HeaderHeight),
                Models.TabStyle.Left   => new Rectangle(0, 0, HeaderHeight, Height),
                Models.TabStyle.Right  => new Rectangle(Width - HeaderHeight, 0, HeaderHeight, Height),
                Models.TabStyle.None   => Rectangle.Empty,
                _                     => new Rectangle(0, 0, Width, HeaderHeight)
            };
        }

        private Rectangle GetContentRect()
        {
            return _tabPosition switch
            {
                Models.TabStyle.Bottom => new Rectangle(0, 0, Width, Math.Max(0, Height - HeaderHeight)),
                Models.TabStyle.Left   => new Rectangle(HeaderHeight, 0, Math.Max(0, Width - HeaderHeight), Height),
                Models.TabStyle.Right  => new Rectangle(0, 0, Math.Max(0, Width - HeaderHeight), Height),
                Models.TabStyle.None   => new Rectangle(0, 0, Width, Height),
                _                     => new Rectangle(0, HeaderHeight, Width, Math.Max(0, Height - HeaderHeight))
            };
        }

        private bool IsInHeader(Point clientPoint)
        {
            if (_tabPosition == Models.TabStyle.None) return false;
            return GetHeaderRect().Contains(clientPoint);
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
    }
}
