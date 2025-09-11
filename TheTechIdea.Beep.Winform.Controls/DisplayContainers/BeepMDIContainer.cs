using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Vis;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Vis.Modules.Managers;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.DisplayContainers
{
    [ToolboxItem(true)]
    [Category("Beep Controls")]
    [DisplayName("Beep MDI Container ")]
    [Description("A modern MDI style container (floating or tabbed) for addins.")]
    public class BeepMDIContainer : BaseControl, IDisplayContainer
    {
        #region Fields
        private readonly List<MdiDocument> _documents = new();
        private readonly Dictionary<string, MdiChildHost> _hosts = new();
        private MdiChildHost _activeHost;
        private int _cascadeOffset = 28;
        private bool _suspendLayoutFlag;
        // Tab strip visuals
        private readonly List<TabVisual> _tabVisuals = new();
        private TabVisual _hoveredTab;
        private Rectangle _tabStripRect;
        private Rectangle _tabsClipRect;
        private Rectangle _scrollLeftRect;
        private Rectangle _scrollRightRect;
        private bool _tabsOverflow;
        private int _tabScrollOffset;
        private int _totalTabsWidth;
        // Drag reorder
        private bool _draggingTab;
        private TabVisual _dragTab;
        private Point _dragStart;
        private int _dragInsertIndex = -1;
        private int _dragOriginalIndex = -1;
        // Context menu
        private ContextMenuStrip _tabMenu;
        private TabVisual _menuTab;
        // Theme colors cache
        private Color _clrTabBack;
        private Color _clrTabFore;
        private Color _clrTabActiveBack;
        private Color _clrTabActiveFore;
        private Color _clrTabInactiveBack;
        private Color _clrTabInactiveFore;
        private Color _clrTabHoverBack;
        private Color _clrTabHoverFore;
        private Color _clrTabBorder;
        private Color _clrTabActiveBorder;
        private Color _clrTabHoverBorder;
        private ToolTip _tabToolTip;
        private TabVisual _lastTooltipTab;
        private int _tabStripTotalHeight; // actual height when multi-line
        private Font _themeTabFont;
        private Font _themeTabHoverFont;
        private Font _themeTabSelectedFont;
        // MISSING FIELD (added)
        private Point _lastMousePoint;
        #endregion

        #region Tab Strip Settings
        [Category("Appearance"), DefaultValue(false)]
        [Description("If true the MDI container shows documents as tabs (like BeepDisplayContainer2). If false it behaves as floating windows.")]
        public bool UseTabbedHeader { get; set; } = true;

        [Category("Appearance"), DefaultValue(30)] public int TabStripHeight { get; set; } = 30;
        [Category("Appearance"), DefaultValue(110)] public int TabMinWidth { get; set; } = 110;
        [Category("Appearance"), DefaultValue(240)] public int TabMaxWidth { get; set; } = 240;
        [Category("Appearance"), DefaultValue(true)] public bool ShowTabCloseButtons { get; set; } = true;
        [Category("Appearance"), DefaultValue(8)] public int TabHorizontalPadding { get; set; } = 8;
        [Category("Appearance"), DefaultValue(4)] public int TabInnerSpacing { get; set; } = 4;
        [Category("Behavior"), DefaultValue(false)] public bool ActivateOnHover { get; set; } = false;
        [Category("Behavior"), DefaultValue(true)] public bool EnableTabScrolling { get; set; } = true;
        [Category("Behavior"), DefaultValue(80)] public int TabScrollStep { get; set; } = 80;
        [Category("Behavior"), DefaultValue(true)] public bool AllowTabDragReorder { get; set; } = true;
        [Category("Appearance"), DefaultValue(true)] public bool ShowTabIcons { get; set; } = true;
        [Category("Appearance"), DefaultValue(true)] public bool ShowTabTooltips { get; set; } = true;
        [Category("Appearance"), DefaultValue(false)] public bool MultiLineTabs { get; set; } = false;
        [Category("Appearance")]
        [Description("Icon size for tabs (when ShowTabIcons = true)")]
        public Size TabIconSize { get; set; } = new Size(16, 16);
        [Browsable(false)]
        public Func<IDM_Addin, Image> TabIconSelector { get; set; } = null; // user can assign custom icon provider
        #endregion

        #region IDisplayContainer Implementation Members & Events
        public ContainerTypeEnum ContainerType { get; set; } = ContainerTypeEnum.TabbedPanel;
        public event EventHandler<ContainerEvents> AddinAdded;
        public event EventHandler<ContainerEvents> AddinRemoved;
        public event EventHandler<ContainerEvents> AddinMoved;
        public event EventHandler<ContainerEvents> AddinChanging;
        public event EventHandler<ContainerEvents> AddinChanged;
        public event EventHandler<IPassedArgs> PreCallModule;
        public event EventHandler<IPassedArgs> PreShowItem;
        public event EventHandler<KeyCombination> KeyPressed;
        #endregion

        #region Constructor & Styles
        public BeepMDIContainer()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            BackColor = Color.FromArgb(245, 245, 248);
            BuildContextMenu();
            UpdateThemeColors();
            _tabToolTip = new ToolTip { ShowAlways = true, AutomaticDelay = 400 };
        }
        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            if (UseTabbedHeader)
            {
                // initialize padding so first added host docks below header
                int headerHeight = MultiLineTabs ? _tabStripTotalHeight : TabStripHeight;
                if (headerHeight <= 0) headerHeight = TabStripHeight; // fallback
                Padding = new Padding(0, headerHeight, 0, 0);
            }
        }
        #endregion

        #region Theme
        public override void ApplyTheme()
        {
            base.ApplyTheme();
            UpdateThemeColors();
            UpdateTabFonts();
            Invalidate();
        }
        private void UpdateThemeColors()
        {
            if (_currentTheme != null)
            {
                _clrTabBack = _currentTheme.TabBackColor == Color.Empty ? Color.FromArgb(235, 235, 238) : _currentTheme.TabBackColor;
                _clrTabFore = _currentTheme.TabForeColor == Color.Empty ? Color.Black : _currentTheme.TabForeColor;
                _clrTabActiveBack = _currentTheme.ActiveTabBackColor == Color.Empty ? Color.White : _currentTheme.ActiveTabBackColor;
                _clrTabActiveFore = _currentTheme.ActiveTabForeColor == Color.Empty ? _clrTabFore : _currentTheme.ActiveTabForeColor;
                _clrTabInactiveBack = _currentTheme.InactiveTabBackColor == Color.Empty ? Color.FromArgb(245, 245, 247) : _currentTheme.InactiveTabBackColor;
                _clrTabInactiveFore = _currentTheme.InactiveTabForeColor == Color.Empty ? _clrTabFore : _currentTheme.InactiveTabForeColor;
                _clrTabHoverBack = _currentTheme.TabHoverBackColor == Color.Empty ? Color.FromArgb(250, 250, 252) : _currentTheme.TabHoverBackColor;
                _clrTabHoverFore = _currentTheme.TabHoverForeColor == Color.Empty ? _clrTabFore : _currentTheme.TabHoverForeColor;
                _clrTabBorder = _currentTheme.TabBorderColor == Color.Empty ? Color.FromArgb(200, 200, 205) : _currentTheme.TabBorderColor;
                _clrTabActiveBorder = _currentTheme.TabSelectedBorderColor == Color.Empty ? Color.FromArgb(120, 150, 210) : _currentTheme.TabSelectedBorderColor;
                _clrTabHoverBorder = _currentTheme.TabHoverBorderColor == Color.Empty ? _clrTabBorder : _currentTheme.TabHoverBorderColor;
            }
            else
            {
                _clrTabBack = Color.FromArgb(235, 235, 238);
                _clrTabFore = Color.Black;
                _clrTabActiveBack = Color.White;
                _clrTabActiveFore = Color.Black;
                _clrTabInactiveBack = Color.FromArgb(245, 245, 247);
                _clrTabInactiveFore = Color.Black;
                _clrTabHoverBack = Color.FromArgb(250, 250, 252);
                _clrTabHoverFore = Color.Black;
                _clrTabBorder = Color.FromArgb(200, 200, 205);
                _clrTabActiveBorder = Color.FromArgb(120, 150, 210);
                _clrTabHoverBorder = _clrTabBorder;
            }
        }
        private void UpdateTabFonts()
        {
            // Dispose previous fonts if created
            _themeTabFont?.Dispose();
            _themeTabHoverFont?.Dispose();
            _themeTabSelectedFont?.Dispose();

            if (_currentTheme != null && UseThemeFont)
            {
                try { _themeTabFont = BeepThemesManager.ToFont(_currentTheme.TabFont) ?? Font; } catch { _themeTabFont = Font; }
                try { _themeTabHoverFont = BeepThemesManager.ToFont(_currentTheme.TabHoverFont) ?? _themeTabFont ?? Font; } catch { _themeTabHoverFont = _themeTabFont ?? Font; }
                try { _themeTabSelectedFont = BeepThemesManager.ToFont(_currentTheme.TabSelectedFont) ?? _themeTabFont ?? Font; } catch { _themeTabSelectedFont = _themeTabFont ?? Font; }
            }
            else
            {
                _themeTabFont = Font;
                _themeTabHoverFont = Font;
                _themeTabSelectedFont = Font;
            }
        }
        #endregion

        #region Public API (Add/Remove/Show)
        public bool AddControl(string TitleText, IDM_Addin control, ContainerTypeEnum pcontainerType)
        {
            try
            {
                if (control is not Control ui) return false;
                string id = Guid.NewGuid().ToString();
                var doc = new MdiDocument { Id = id, Title = TitleText, Addin = control, GuidId = control?.GuidID };
                _documents.Add(doc);
                var host = new MdiChildHost(doc) { Anchor = AnchorStyles.None };
                // Tabbed vs floating setup
                if (UseTabbedHeader)
                {
                    host.Dock = DockStyle.Fill; // fill remaining area (we add padding for header)
                }
                else
                {
                    host.Dock = DockStyle.None;
                    host.Bounds = GetDefaultHostBounds();
                }
                host.CloseRequested += (_, __) => RemoveControl(TitleText, control);
                host.Activated += (_, __) => ActivateHost(host);
                host.Moved += (_, __) => RaiseAddinMoved(doc);
                ui.Dock = DockStyle.Fill; ui.Visible = true; host.ContentPanel.Controls.Add(ui);
                host.Visible = true; // ensure visible
                _hosts.Add(id, host); Controls.Add(host);
                if (!UseTabbedHeader)
                {
                    host.BringToFront();
                    CascadeNextPosition(host);
                }
                ActivateHost(host); RaiseAddinAdded(doc);
                if (UseTabbedHeader) LayoutTabbedHosts(); else Invalidate();
                return true;
            }
            catch { return false; }
        }
        public bool RemoveControl(string TitleText, IDM_Addin control)
        {
            var doc = _documents.FirstOrDefault(d => d.Title == TitleText && d.Addin == control);
            if (doc == null) return false;
            if (_hosts.TryGetValue(doc.Id, out var host)) { Controls.Remove(host); host.Dispose(); _hosts.Remove(doc.Id); }
            _documents.Remove(doc);
            if (_activeHost?.Document == doc)
            {
                _activeHost = null;
                var next = _documents.LastOrDefault();
                if (next != null && _hosts.TryGetValue(next.Id, out var nextHost)) ActivateHost(nextHost, false);
            }
            RaiseAddinRemoved(doc);
            if (UseTabbedHeader) LayoutTabbedHosts();
            Invalidate();
            return true;
        }
        public bool RemoveControlByGuidTag(string guidid)
        { var doc = _documents.FirstOrDefault(d => d.GuidId == guidid); return doc != null && RemoveControl(doc.Title, doc.Addin); }
        public bool RemoveControlByName(string name)
        { var doc = _documents.FirstOrDefault(d => d.Title == name); return doc != null && RemoveControl(doc.Title, doc.Addin); }
        public bool ShowControl(string TitleText, IDM_Addin control)
        {
            var doc = _documents.FirstOrDefault(d => d.Title == TitleText && d.Addin == control);
            if (doc == null) return false;
            if (_hosts.TryGetValue(doc.Id, out var host)) { ActivateHost(host); if (UseTabbedHeader) LayoutTabbedHosts(); return true; }
            return false;
        }
        public void Clear() { foreach (var d in _documents.ToList()) RemoveControl(d.Title, d.Addin); }
        public bool IsControlExit(IDM_Addin control) => _documents.Any(d => d.Addin == control);
        public IErrorsInfo PressKey(KeyCombination keyCombination) { KeyPressed?.Invoke(this, keyCombination); return new ErrorsInfo { Flag = Errors.Ok }; }
        #endregion

        #region Layout & Arrange Functions (Floating)
        private Rectangle GetDefaultHostBounds()
        { int w = (int)(Width * 0.55); int h = (int)(Height * 0.55); return new Rectangle(10, 10, Math.Max(200, w), Math.Max(150, h)); }
        private void CascadeNextPosition(MdiChildHost host)
        { int index = _documents.IndexOf(host.Document); int x = 10 + (index * _cascadeOffset); int y = 10 + (index * _cascadeOffset); host.Location = new Point(x % Math.Max(20, Width - 150), y % Math.Max(20, Height - 100)); }
        public void Cascade() { if (UseTabbedHeader) return; PerformArrange(hs => { int i = 0; foreach (var h in hs) { h.Bounds = new Rectangle(10 + i * _cascadeOffset, 10 + i * _cascadeOffset, Math.Max(300, Width / 2), Math.Max(220, Height / 2)); i++; } }); }
        public void TileHorizontal() { if (UseTabbedHeader) return; PerformArrange(hs => { if (hs.Count == 0) return; int h = Math.Max(80, (Height - 12) / hs.Count); int top = 6; foreach (var hst in hs) { hst.Bounds = new Rectangle(6, top, Width - 12, h - 6); top += h; } }); }
        public void TileVertical() { if (UseTabbedHeader) return; PerformArrange(hs => { if (hs.Count == 0) return; int w = Math.Max(80, (Width - 12) / hs.Count); int left = 6; foreach (var hst in hs) { hst.Bounds = new Rectangle(left, 6, w - 6, Height - 12); left += w; } }); }
        private void PerformArrange(Action<List<MdiChildHost>> arrange)
        { if (arrange == null) return; _suspendLayoutFlag = true; try { var hs = _hosts.Values.ToList(); arrange(hs); foreach (var h in hs) h.Invalidate(); } finally { _suspendLayoutFlag = false; Invalidate(); } }
        #endregion

        #region Tabbed Mode Layout
        private void UpdateHeaderPadding()
        {
            if (UseTabbedHeader)
            {
                int headerHeight = MultiLineTabs ? _tabStripTotalHeight : TabStripHeight;
                Padding = new Padding(0, headerHeight, 0, 0);
            }
            else
            {
                Padding = Padding.Empty;
            }
        }
        private Rectangle GetTabbedContentRectangle()
        {
            int headerHeight = UseTabbedHeader ? (MultiLineTabs ? _tabStripTotalHeight : TabStripHeight) : 0;
            return new Rectangle(0, headerHeight, Width, Math.Max(0, Height - headerHeight));
        }
        private void LayoutTabbedHosts()
        {
            if (!UseTabbedHeader) return;
            BuildTabVisuals();
            UpdateHeaderPadding();
            foreach (var kv in _hosts)
            {
                var host = kv.Value;
                if (host == _activeHost)
                {
                    host.Dock = DockStyle.Fill;
                    host.Visible = true;
                }
                else
                {
                    host.Dock = DockStyle.None;
                    host.Visible = false;
                }
            }
            Invalidate();
        }
        private void BuildTabVisuals()
        {
            _tabVisuals.Clear(); _tabsOverflow = false; _totalTabsWidth = 0; if (!UseTabbedHeader) return;
            int singleRowHeight = TabStripHeight; _tabStripTotalHeight = singleRowHeight;
            if (!MultiLineTabs) { _tabStripRect = new Rectangle(0, 0, Width, singleRowHeight); }
            int available = Width - 10; int count = _documents.Count; if (count == 0) { _tabStripRect = new Rectangle(0, 0, Width, singleRowHeight); return; }
            int x = 6; int y = 2; int ideal = Math.Min(TabMaxWidth, Math.Max(TabMinWidth, (available - 4) / Math.Max(1, count))); int rowHeightInner = singleRowHeight - 4;
            foreach (var doc in _documents)
            {
                int w = ideal;
                if (!MultiLineTabs)
                {
                    var rawRect = new Rectangle(x - _tabScrollOffset, 2, w, rowHeightInner); Rectangle close = Rectangle.Empty;
                    if (ShowTabCloseButtons) close = new Rectangle(rawRect.Right - 18, rawRect.Top + (rawRect.Height - 16) / 2, 16, 16);
                    _tabVisuals.Add(new TabVisual(doc, rawRect, close)); x += w + TabInnerSpacing; _totalTabsWidth = x - TabInnerSpacing + 6;
                }
                else
                {
                    if (x + w > Width - 12) { x = 6; y += singleRowHeight; _tabStripTotalHeight += singleRowHeight; }
                    var rawRect = new Rectangle(x, y, w, rowHeightInner); Rectangle close = Rectangle.Empty;
                    if (ShowTabCloseButtons) close = new Rectangle(rawRect.Right - 18, rawRect.Top + (rawRect.Height - 16) / 2, 16, 16);
                    _tabVisuals.Add(new TabVisual(doc, rawRect, close)); x += w + TabInnerSpacing;
                }
            }
            if (!MultiLineTabs)
            {
                if (EnableTabScrolling && _totalTabsWidth > Width - 80) _tabsOverflow = true;
                if (_tabScrollOffset < 0) _tabScrollOffset = 0; int maxOffset = Math.Max(0, _totalTabsWidth - (Width - 60)); if (_tabScrollOffset > maxOffset) _tabScrollOffset = maxOffset;
                if (_tabsOverflow) { int btnW = 22; _scrollRightRect = new Rectangle(Width - btnW - 4, 4, btnW, singleRowHeight - 8); _scrollLeftRect = new Rectangle(_scrollRightRect.Left - btnW - 4, 4, btnW, singleRowHeight - 8); }
                else { _scrollLeftRect = Rectangle.Empty; _scrollRightRect = Rectangle.Empty; }
                _tabStripRect = new Rectangle(0, 0, Width, singleRowHeight);
            }
            else
            {
                _scrollLeftRect = Rectangle.Empty; _scrollRightRect = Rectangle.Empty; _tabsOverflow = false; _tabScrollOffset = 0; _tabStripRect = new Rectangle(0, 0, Width, _tabStripTotalHeight);
            }
        }
        #endregion

        #region Activation & Event Raising
        private void ActivateHost(MdiChildHost host, bool raiseChangingEvent = true)
        {
            if (host == null || host == _activeHost) return; var old = _activeHost;
            if (old != null && raiseChangingEvent) { RaiseAddinChanging(old.Document); old.IsActive = false; old.Invalidate(); }
            _activeHost = host; host.IsActive = true; host.Visible = true; host.BringToFront(); host.Focus(); foreach (var h in _hosts.Values.Where(h => h != host && UseTabbedHeader)) h.Visible = false;
            RaiseAddinChanged(host.Document); host.Invalidate(); if (UseTabbedHeader) LayoutTabbedHosts();
        }
        private void RaiseAddinAdded(MdiDocument doc) => AddinAdded?.Invoke(this, ToEvent(doc));
        private void RaiseAddinRemoved(MdiDocument doc) => AddinRemoved?.Invoke(this, ToEvent(doc));
        private void RaiseAddinChanging(MdiDocument doc) => AddinChanging?.Invoke(this, ToEvent(doc));
        private void RaiseAddinChanged(MdiDocument doc) => AddinChanged?.Invoke(this, ToEvent(doc));
        private void RaiseAddinMoved(MdiDocument doc) => AddinMoved?.Invoke(this, ToEvent(doc));
        private ContainerEvents ToEvent(MdiDocument doc) => new() { TitleText = doc.Title, Control = doc.Addin, ContainerType = ContainerType, Guidid = doc.GuidId };
        #endregion

        #region Overrides / Painting
        protected override void OnResize(EventArgs e)
        { base.OnResize(e); if (UseTabbedHeader) LayoutTabbedHosts(); else if (!_suspendLayoutFlag) ConstrainHostsToClient(); }
        protected override void OnPaint(PaintEventArgs e)
        { base.OnPaint(e); if (UseTabbedHeader) { DrawTabStrip(e.Graphics); if (_draggingTab && _dragTab != null && _dragInsertIndex >= 0) DrawDragIndicator(e.Graphics); } }
        private void DrawTabStrip(Graphics g)
        {
            using (var back = new SolidBrush(_clrTabBack)) g.FillRectangle(back, _tabStripRect);
            using (var pen = new Pen(_clrTabBorder)) g.DrawLine(pen, _tabStripRect.Left, _tabStripRect.Bottom - 1, _tabStripRect.Right, _tabStripRect.Bottom - 1);
            foreach (var tv in _tabVisuals)
            {
                bool active = (_activeHost?.Document == tv.Document); bool hovered = tv == _hoveredTab;
                var r = tv.Bounds; if (r.Right < 0 || r.Left > Width || r.Bottom > _tabStripRect.Bottom) continue;
                Color backColor = active ? _clrTabActiveBack : hovered ? _clrTabHoverBack : _clrTabInactiveBack;
                Color foreColor = active ? _clrTabActiveFore : hovered ? _clrTabHoverFore : _clrTabInactiveFore;
                Color border = active ? _clrTabActiveBorder : hovered ? _clrTabHoverBorder : _clrTabBorder;
                using (var br = new SolidBrush(backColor)) g.FillRectangle(br, r);
                using (var p = new Pen(border)) g.DrawRectangle(p, new Rectangle(r.X, r.Y, r.Width - 1, r.Height - 1));
                int iconSpace = 0; if (ShowTabIcons)
                { Image ic = TabIconSelector?.Invoke(tv.Document.Addin) ?? GetFallbackIcon(); if (ic != null) { var iconRect = new Rectangle(r.X + TabHorizontalPadding, r.Y + (r.Height - TabIconSize.Height) / 2, TabIconSize.Width, TabIconSize.Height); g.DrawImage(ic, iconRect); iconSpace = TabIconSize.Width + 4; } }
                var textRect = new Rectangle(r.X + TabHorizontalPadding + iconSpace, r.Y + 4, r.Width - (ShowTabCloseButtons ? 22 : 8) - iconSpace - TabHorizontalPadding, r.Height - 8);
                Font drawFont = active ? (_themeTabSelectedFont ?? _themeTabFont ?? Font) : hovered ? (_themeTabHoverFont ?? _themeTabFont ?? Font) : (_themeTabFont ?? Font);
                TextRenderer.DrawText(g, tv.Document.Title, drawFont, textRect, foreColor, TextFormatFlags.EndEllipsis | TextFormatFlags.VerticalCenter);
                if (ShowTabCloseButtons) DrawCloseGlyph(g, tv.CloseButtonRect, active, hovered && tv.CloseButtonRect.Contains(_lastMousePoint));
            }
            if (_tabsOverflow) DrawScrollButtons(g);
        }
        private Image GetFallbackIcon()
        {
            return null;
        }
        private void DrawCloseGlyph(Graphics g, Rectangle r, bool active, bool hot)
        {
            Color back = hot ? Color.FromArgb(220, 60, 60) : (active ? Color.FromArgb(225, 225, 230) : Color.FromArgb(210, 210, 215));
            using (var br = new SolidBrush(back)) g.FillRectangle(br, r);
            using (var pen = new Pen(Color.Black, 1.5f))
            { g.DrawLine(pen, r.Left + 4, r.Top + 4, r.Right - 4, r.Bottom - 4); g.DrawLine(pen, r.Left + 4, r.Bottom - 4, r.Right - 4, r.Top + 4); }
        }
        private void DrawScrollButtons(Graphics g)
        {
            DrawScrollButton(g, _scrollLeftRect, false);
            DrawScrollButton(g, _scrollRightRect, true);
        }
        private void DrawScrollButton(Graphics g, Rectangle r, bool right)
        {
            if (r == Rectangle.Empty) return;
            using (var br = new SolidBrush(Color.FromArgb(230, 230, 234))) g.FillRectangle(br, r);
            using (var p = new Pen(_clrTabBorder)) g.DrawRectangle(p, r);
            Point c = new Point(r.X + r.Width / 2, r.Y + r.Height / 2);
            Point[] pts = right
                ? new[] { new Point(c.X - 4, c.Y - 5), new Point(c.X + 3, c.Y), new Point(c.X - 4, c.Y + 5) }
                : new[] { new Point(c.X + 4, c.Y - 5), new Point(c.X - 3, c.Y), new Point(c.X + 4, c.Y + 5) };
            using (var br2 = new SolidBrush(Color.Black)) g.FillPolygon(br2, pts);
        }
        private void DrawDragIndicator(Graphics g)
        {
            if (_dragInsertIndex < 0 || _dragInsertIndex > _tabVisuals.Count) return;
            int x = (_dragInsertIndex == _tabVisuals.Count) ? _tabVisuals.Last().Bounds.Right + 2 : _tabVisuals[_dragInsertIndex].Bounds.Left - 2;
            using var pen = new Pen(Color.Red, 2); g.DrawLine(pen, x, 3, x, TabStripHeight - 4);
        }
        #endregion

        #region Mouse (Tab Strip)
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e); _lastMousePoint = e.Location; if (!UseTabbedHeader) return;
            if (_draggingTab && _dragTab != null)
            { int idx = 0; foreach (var tv in _tabVisuals) { if (e.X < tv.Bounds.Left + tv.Bounds.Width / 2) { _dragInsertIndex = idx; break; } idx++; _dragInsertIndex = idx; } Invalidate(_tabStripRect); return; }
            var prev = _hoveredTab; _hoveredTab = _tabVisuals.FirstOrDefault(t => t.Bounds.Contains(e.Location));
            if (ShowTabTooltips)
            { if (_hoveredTab != _lastTooltipTab) { if (_hoveredTab == null) _tabToolTip.Hide(this); else _tabToolTip.Show(_hoveredTab.Document.Title, this, PointToClient(MousePosition) + new Size(16, 18), 3000); _lastTooltipTab = _hoveredTab; } }
            if (ActivateOnHover && _hoveredTab != null && _activeHost?.Document != _hoveredTab.Document)
                if (_hosts.TryGetValue(_hoveredTab.Document.Id, out var host)) ActivateHost(host);
            if (prev != _hoveredTab) Invalidate(_tabStripRect);
        }
        protected override void OnMouseLeave(EventArgs e)
        { base.OnMouseLeave(e); if (!UseTabbedHeader) return; if (_hoveredTab != null) { _hoveredTab = null; Invalidate(_tabStripRect); } if (ShowTabTooltips) { _tabToolTip.Hide(this); _lastTooltipTab = null; } }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e); if (!UseTabbedHeader) return; if (e.Button == MouseButtons.Right) { ShowTabMenu(e.Location); return; }
            if (e.Button != MouseButtons.Left) return;
            if (_tabsOverflow)
            {
                if (_scrollLeftRect.Contains(e.Location)) { _tabScrollOffset -= TabScrollStep; if (_tabScrollOffset < 0) _tabScrollOffset = 0; BuildTabVisuals(); Invalidate(); return; }
                if (_scrollRightRect.Contains(e.Location)) { int maxOffset = Math.Max(0, _totalTabsWidth - (Width - 60)); _tabScrollOffset += TabScrollStep; if (_tabScrollOffset > maxOffset) _tabScrollOffset = maxOffset; BuildTabVisuals(); Invalidate(); return; }
            }
            var hit = _tabVisuals.FirstOrDefault(t => t.Bounds.Contains(e.Location));
            if (hit != null)
            {
                if (ShowTabCloseButtons && hit.CloseButtonRect.Contains(e.Location)) { RemoveControl(hit.Document.Title, hit.Document.Addin); return; }
                if (_hosts.TryGetValue(hit.Document.Id, out var host)) ActivateHost(host);
                if (AllowTabDragReorder) { _draggingTab = true; _dragTab = hit; _dragStart = e.Location; _dragOriginalIndex = _tabVisuals.IndexOf(hit); _dragInsertIndex = _dragOriginalIndex; }
            }
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e); if (!UseTabbedHeader) return; if (_draggingTab)
            {
                _draggingTab = false;
                if (_dragTab != null && _dragInsertIndex != _dragOriginalIndex && _dragInsertIndex >= 0)
                { var doc = _dragTab.Document; _documents.Remove(doc); if (_dragInsertIndex >= _documents.Count) _documents.Add(doc); else _documents.Insert(_dragInsertIndex, doc); }
                _dragTab = null; BuildTabVisuals(); Invalidate();
            }
        }
        #endregion

        #region Context Menu
        private void BuildContextMenu()
        {
            _tabMenu = new ContextMenuStrip();
            _tabMenu.Items.Add("Close", null, (_, __) => { if (_menuTab != null) RemoveControl(_menuTab.Document.Title, _menuTab.Document.Addin); });
            _tabMenu.Items.Add("Close Others", null, (_, __) => CloseOthers());
            _tabMenu.Items.Add("Close All", null, (_, __) => CloseAllTabs());
        }
        private void ShowTabMenu(Point location)
        { _menuTab = _tabVisuals.FirstOrDefault(t => t.Bounds.Contains(location)); if (_menuTab != null) _tabMenu.Show(this, location); }
        private void CloseOthers()
        { if (_menuTab == null) return; var keep = _menuTab.Document; foreach (var d in _documents.Where(d => d != keep).ToList()) RemoveControl(d.Title, d.Addin); }
        private void CloseAllTabs() { foreach (var d in _documents.ToList()) RemoveControl(d.Title, d.Addin); }
        #endregion

        #region Helpers
        private void ConstrainHostsToClient()
        { foreach (var h in _hosts.Values) { var r = h.Bounds; if (r.Right > Width) r.X = Math.Max(0, Width - r.Width - 2); if (r.Bottom > Height) r.Y = Math.Max(0, Height - r.Height - 2); if (r.X < 0) r.X = 0; if (r.Y < 0) r.Y = 0; h.Bounds = r; } }
        #endregion

        #region Inner Classes
        private class MdiDocument { public string Id { get; set; } public string GuidId { get; set; } public string Title { get; set; } public IDM_Addin Addin { get; set; } }
        private class MdiChildHost : Panel
        {
            private const int CAPTION_HEIGHT = 26; private const int BUTTON_SIZE = 18; private bool _dragging; private Point _dragOrigin;
            public MdiDocument Document { get; }
            public bool IsActive { get; set; }
            public Panel ContentPanel { get; }
            public event EventHandler CloseRequested; public event EventHandler Activated; public event EventHandler Moved;
            public MdiChildHost(MdiDocument doc) { Document = doc; DoubleBuffered = true; BackColor = Color.White; ForeColor = Color.Black; Padding = new Padding(1, CAPTION_HEIGHT + 1, 1, 1); ContentPanel = new Panel { Dock = DockStyle.Fill, BackColor = Color.White }; Controls.Add(ContentPanel); }
            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e); if (Parent is BeepMDIContainer parent && parent.UseTabbedHeader) { using var pen = new Pen(parent._clrTabBorder); e.Graphics.DrawRectangle(pen, new Rectangle(0, 0, Width - 1, Height - 1)); return; }
                var g = e.Graphics; g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias; Rectangle captionRect = new Rectangle(0, 0, Width - 1, CAPTION_HEIGHT);
                Color start = IsActive ? Color.FromArgb(70, 120, 200) : Color.FromArgb(180, 180, 185); Color end = IsActive ? Color.FromArgb(40, 80, 160) : Color.FromArgb(160, 160, 165);
                using (var br = new System.Drawing.Drawing2D.LinearGradientBrush(captionRect, start, end, 90f)) g.FillRectangle(br, captionRect);
                using (var pen = new Pen(Color.DimGray)) g.DrawRectangle(pen, new Rectangle(0, 0, Width - 1, Height - 1));
                TextRenderer.DrawText(g, Document.Title ?? "Untitled", Font, new Rectangle(8, 2, captionRect.Width - 50, captionRect.Height - 4), Color.White, TextFormatFlags.EndEllipsis | TextFormatFlags.VerticalCenter);
                var closeRect = new Rectangle(Width - (BUTTON_SIZE + 6), 4, BUTTON_SIZE, BUTTON_SIZE);
                using (var br = new SolidBrush(Color.FromArgb(220, 60, 60))) g.FillRectangle(br, closeRect);
                using (var pen2 = new Pen(Color.White, 1.6f)) { g.DrawLine(pen2, closeRect.Left + 4, closeRect.Top + 4, closeRect.Right - 4, closeRect.Bottom - 4); g.DrawLine(pen2, closeRect.Left + 4, closeRect.Bottom - 4, closeRect.Right - 4, closeRect.Top + 4); }
            }
            protected override void OnMouseDown(MouseEventArgs e)
            { base.OnMouseDown(e); if (Parent is BeepMDIContainer p && p.UseTabbedHeader) return; if (e.Button == MouseButtons.Left) { var cb = new Rectangle(Width - (BUTTON_SIZE + 6), 4, BUTTON_SIZE, BUTTON_SIZE); if (cb.Contains(e.Location)) return; if (e.Y <= CAPTION_HEIGHT) { _dragging = true; _dragOrigin = e.Location; Capture = true; Activated?.Invoke(this, EventArgs.Empty); } } }
            protected override void OnMouseMove(MouseEventArgs e)
            { base.OnMouseMove(e); if (Parent is BeepMDIContainer p && p.UseTabbedHeader) return; if (_dragging) { var parentCtl = Parent; if (parentCtl == null) return; int dx = e.X - _dragOrigin.X; int dy = e.Y - _dragOrigin.Y; var r = Bounds; r.Offset(dx, dy); if (r.X < 0) r.X = 0; if (r.Y < 0) r.Y = 0; if (r.Right > parentCtl.Width) r.X = parentCtl.Width - r.Width; if (r.Bottom > parentCtl.Height) r.Y = parentCtl.Height - r.Height; Bounds = r; Moved?.Invoke(this, EventArgs.Empty); } }
            protected override void OnMouseUp(MouseEventArgs e) { base.OnMouseUp(e); if (_dragging && e.Button == MouseButtons.Left) { _dragging = false; Capture = false; } }
            protected override void OnMouseClick(MouseEventArgs e) { base.OnMouseClick(e); if (Parent is BeepMDIContainer p && p.UseTabbedHeader) return; if (e.Button == MouseButtons.Left) { var cb = new Rectangle(Width - (BUTTON_SIZE + 6), 4, BUTTON_SIZE, BUTTON_SIZE); if (cb.Contains(e.Location)) { CloseRequested?.Invoke(this, EventArgs.Empty); return; } if (e.Y <= CAPTION_HEIGHT) Activated?.Invoke(this, EventArgs.Empty); } }
        }
        private sealed class TabVisual { public MdiDocument Document { get; } public Rectangle Bounds { get; } public Rectangle CloseButtonRect { get; } public TabVisual(MdiDocument doc, Rectangle bounds, Rectangle closeRect) { Document = doc; Bounds = bounds; CloseButtonRect = closeRect; } }
        #endregion
    }
}
