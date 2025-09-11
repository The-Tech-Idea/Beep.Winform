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
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.DisplayContainers.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.DisplayContainers
{
    [ToolboxItem(true)]
    [Category("Beep Controls")]
    [DisplayName("Beep Display Container 2")]
    [Description("A modern, self-contained display container for addins with native rendering and advanced features.")]
    public partial class BeepDisplayContainer2 : BeepControl, IDisplayContainer
    {
        #region Fields and Properties

        private readonly List<AddinTab> _tabs = new();
        private readonly Dictionary<string, IDM_Addin> _addins = new();
        private AddinTab _activeTab;
        private AddinTab _hoveredTab;
        private ContainerTypeEnum _containerType = ContainerTypeEnum.TabbedPanel;
        private ContainerDisplayMode _displayMode = ContainerDisplayMode.Tabbed;
        private TabPosition _tabPosition = TabPosition.Top;
        private bool _showCloseButtons = true;
        private bool _allowTabReordering = true;
        private bool _enableAnimations = true;
        private AnimationSpeed _animationSpeed = AnimationSpeed.Normal;
        private System.Windows.Forms.Timer _animationTimer;
        private Rectangle _tabArea;
        private Rectangle _contentArea;
        private int _tabHeight = 36;
        private int _tabMinWidth = 80;
        private int _tabMaxWidth = 200;
        private int _scrollOffset = 0;
        private bool _needsScrolling = false;
        private Rectangle _scrollLeftButton;
        private Rectangle _scrollRightButton;
        private Rectangle _newTabButton;

        // Helper classes
        private TabPaintHelper _paintHelper;
        private TabLayoutHelper _layoutHelper;
        private TabAnimationHelper _animationHelper;

        // Theme colors
        private Color _tabBackColor;
        private Color _tabForeColor;
        private Color _activeTabBackColor;
        private Color _activeTabForeColor;
        private Color _hoverTabBackColor;
        private Color _borderColor;
        private Color _contentBackColor;

        [Category("Appearance")]
        [DefaultValue(ContainerDisplayMode.Tabbed)]
        public ContainerDisplayMode DisplayMode
        {
            get => _displayMode;
            set { _displayMode = value; RecalculateLayout(); Invalidate(); }
        }

        [Category("Appearance")]
        [DefaultValue(TabPosition.Top)]
        public TabPosition TabPosition
        {
            get => _tabPosition;
            set { _tabPosition = value; RecalculateLayout(); Invalidate(); }
        }

        [Category("Behavior")]
        [DefaultValue(true)]
        public bool ShowCloseButtons
        {
            get => _showCloseButtons;
            set { _showCloseButtons = value; Invalidate(); }
        }

        [Category("Behavior")]
        [DefaultValue(true)]
        public bool AllowTabReordering
        {
            get => _allowTabReordering;
            set => _allowTabReordering = value;
        }

        [Category("Animation")]
        [DefaultValue(true)]
        public bool EnableAnimations
        {
            get => _enableAnimations;
            set => _enableAnimations = value;
        }

        [Category("Animation")]
        [DefaultValue(AnimationSpeed.Normal)]
        public AnimationSpeed AnimationSpeed
        {
            get => _animationSpeed;
            set => _animationSpeed = value;
        }

        [Category("Appearance")]
        [DefaultValue(36)]
        public int TabHeight
        {
            get => _tabHeight;
            set { _tabHeight = Math.Max(24, value); RecalculateLayout(); Invalidate(); }
        }

        public ContainerTypeEnum ContainerType
        {
            get => _containerType;
            set => _containerType = value;
        }

        #endregion

        #region Events

        public event EventHandler<ContainerEvents> AddinAdded;
        public event EventHandler<ContainerEvents> AddinRemoved;
        public event EventHandler<ContainerEvents> AddinMoved;
        public event EventHandler<ContainerEvents> AddinChanging;
        public event EventHandler<ContainerEvents> AddinChanged;
        public event EventHandler<IPassedArgs> PreCallModule;
        public event EventHandler<IPassedArgs> PreShowItem;
        public event EventHandler<KeyCombination> KeyPressed;

        #endregion

        #region Constructor

        public BeepDisplayContainer2():base()
        {
            InitializeComponent();
            InitializeContainer();
        }

        private void InitializeComponent()
        {
          
            
            BackColor = Color.White;
            Font = new Font("Segoe UI", 9F, FontStyle.Regular);
        }

        private void InitializeContainer()
        {
            // Initialize helpers
            _paintHelper = new TabPaintHelper(_currentTheme);
            _layoutHelper = new TabLayoutHelper();
            _animationHelper = new TabAnimationHelper(() => Invalidate());
            
            _animationTimer = new System.Windows.Forms.Timer { Interval = 16 }; // 60 FPS
            _animationTimer.Tick += (s, e) => _animationHelper.UpdateAnimations(_tabs, _animationSpeed);

            // Initialize theme colors
            ApplyTheme();

            // Calculate initial layout
            RecalculateLayout();
        }

        #endregion

        #region Theme Integration

        public override void ApplyTheme()
        {
            base.ApplyTheme();
            
            // Update paint helper with new theme
            _paintHelper = new TabPaintHelper(_currentTheme);
            
            if (_currentTheme != null)
            {
                _tabBackColor = _currentTheme.PanelBackColor;
                _tabForeColor = _currentTheme.ForeColor;
                _activeTabBackColor = _currentTheme.ButtonBackColor;
                _activeTabForeColor = _currentTheme.ButtonForeColor;
                _hoverTabBackColor = ControlPaint.Light(_currentTheme.ButtonBackColor, 0.3f);
                _borderColor = _currentTheme.BorderColor;
                _contentBackColor = _currentTheme.BackColor;
                BackColor = _contentBackColor;
            }
            else
            {
                // Fallback colors
                _tabBackColor = Color.FromArgb(245, 245, 245);
                _tabForeColor = Color.FromArgb(64, 64, 64);
                _activeTabBackColor = Color.White;
                _activeTabForeColor = Color.FromArgb(32, 32, 32);
                _hoverTabBackColor = Color.FromArgb(235, 235, 235);
                _borderColor = Color.FromArgb(200, 200, 200);
                _contentBackColor = Color.White;
                BackColor = _contentBackColor;
            }
            
            Invalidate();
        }

        #endregion

        #region Layout Management

        private void RecalculateLayout()
        {
            if (Width <= 0 || Height <= 0) return;

            switch (_tabPosition)
            {
                case TabPosition.Top:
                    _tabArea = new Rectangle(0, 0, Width, _tabHeight);
                    _contentArea = new Rectangle(0, _tabHeight, Width, Height - _tabHeight);
                    break;
                case TabPosition.Bottom:
                    _contentArea = new Rectangle(0, 0, Width, Height - _tabHeight);
                    _tabArea = new Rectangle(0, Height - _tabHeight, Width, _tabHeight);
                    break;
                case TabPosition.Left:
                    _tabArea = new Rectangle(0, 0, _tabHeight, Height);
                    _contentArea = new Rectangle(_tabHeight, 0, Width - _tabHeight, Height);
                    break;
                case TabPosition.Right:
                    _contentArea = new Rectangle(0, 0, Width - _tabHeight, Height);
                    _tabArea = new Rectangle(Width - _tabHeight, 0, _tabHeight, Height);
                    break;
            }

            CalculateTabLayout();
            PositionActiveAddin();
        }

        private void CalculateTabLayout()
        {
            if (_tabs.Count == 0 || _layoutHelper == null) return;

            var result = _layoutHelper.CalculateTabLayout(_tabs, _tabArea, _tabPosition, 
                _tabMinWidth, _tabMaxWidth, _scrollOffset);
            
            _needsScrolling = result.NeedsScrolling;
            _scrollLeftButton = result.ScrollLeftButton;
            _scrollRightButton = result.ScrollRightButton;
            _newTabButton = result.NewTabButton;
        }

        private void PositionActiveAddin()
        {
            if (_activeTab?.Addin == null) return;

            var control = _activeTab.Addin as Control;
            if (control != null)
            {
                control.Bounds = _contentArea;
                if (!Controls.Contains(control))
                {
                    Controls.Add(control);
                }
                control.BringToFront();
                control.Visible = true;
            }

            // Hide other addins
            foreach (var tab in _tabs.Where(t => t != _activeTab))
            {
                var ctrl = tab.Addin as Control;
                if (ctrl != null)
                {
                    ctrl.Visible = false;
                }
            }
        }

        #endregion

        #region Painting

        protected override void DrawContent(Graphics g)
        {
            base.DrawContent(g);



            if (_displayMode == ContainerDisplayMode.Tabbed)
            {
                DrawTabs(g);
                DrawScrollButtons(g);
            }
        }
        private void DrawTabs(Graphics g)
        {
            if (_paintHelper == null) return;
            
            foreach (var tab in _tabs.Where(t => t.IsVisible))
            {
                DrawTab(g, tab);
            }
        }

        private void DrawTab(Graphics g, AddinTab tab)
        {
            var isActive = tab == _activeTab;
            var isHovered = tab == _hoveredTab;
            
            _paintHelper.DrawProfessionalTab(g, tab.Bounds, tab.Title, Font,
                isActive, isHovered, _showCloseButtons, 
                tab.IsCloseHovered, tab.AnimationProgress);
        }

        private void DrawScrollButtons(Graphics g)
        {
            if (!_needsScrolling) return;

            // Left/Up scroll button
            using (var brush = new SolidBrush(_tabBackColor))
            {
                g.FillRectangle(brush, _scrollLeftButton);
            }
            using (var pen = new Pen(_tabForeColor))
            {
                g.DrawRectangle(pen, _scrollLeftButton);
                // Draw arrow
                DrawArrow(g, _scrollLeftButton, _tabPosition == TabPosition.Top || _tabPosition == TabPosition.Bottom ? ArrowDirection.Left : ArrowDirection.Up);
            }

            // Right/Down scroll button
            using (var brush = new SolidBrush(_tabBackColor))
            {
                g.FillRectangle(brush, _scrollRightButton);
            }
            using (var pen = new Pen(_tabForeColor))
            {
                g.DrawRectangle(pen, _scrollRightButton);
                // Draw arrow
                DrawArrow(g, _scrollRightButton, _tabPosition == TabPosition.Top || _tabPosition == TabPosition.Bottom ? ArrowDirection.Right : ArrowDirection.Down);
            }

            // New tab button
            using (var brush = new SolidBrush(_tabBackColor))
            {
                g.FillRectangle(brush, _newTabButton);
            }
            using (var pen = new Pen(_tabForeColor))
            {
                g.DrawRectangle(pen, _newTabButton);
                // Draw plus
                var center = new Point(_newTabButton.X + _newTabButton.Width / 2, _newTabButton.Y + _newTabButton.Height / 2);
                g.DrawLine(pen, center.X - 4, center.Y, center.X + 4, center.Y);
                g.DrawLine(pen, center.X, center.Y - 4, center.X, center.Y + 4);
            }
        }

        private void DrawArrow(Graphics g, Rectangle bounds, ArrowDirection direction)
        {
            var center = new Point(bounds.X + bounds.Width / 2, bounds.Y + bounds.Height / 2);
            var points = new Point[3];

            switch (direction)
            {
                case ArrowDirection.Left:
                    points[0] = new Point(center.X - 3, center.Y);
                    points[1] = new Point(center.X + 2, center.Y - 3);
                    points[2] = new Point(center.X + 2, center.Y + 3);
                    break;
                case ArrowDirection.Right:
                    points[0] = new Point(center.X + 3, center.Y);
                    points[1] = new Point(center.X - 2, center.Y - 3);
                    points[2] = new Point(center.X - 2, center.Y + 3);
                    break;
                case ArrowDirection.Up:
                    points[0] = new Point(center.X, center.Y - 3);
                    points[1] = new Point(center.X - 3, center.Y + 2);
                    points[2] = new Point(center.X + 3, center.Y + 2);
                    break;
                case ArrowDirection.Down:
                    points[0] = new Point(center.X, center.Y + 3);
                    points[1] = new Point(center.X - 3, center.Y - 2);
                    points[2] = new Point(center.X + 3, center.Y - 2);
                    break;
            }

            using (var brush = new SolidBrush(_tabForeColor))
            {
                g.FillPolygon(brush, points);
            }
        }

        #endregion

        #region Helper Methods

        private AddinTab GetTabAt(Point point)
        {
            return _layoutHelper?.GetTabAt(_tabs, point);
        }

        private Rectangle GetCloseButtonRect(Rectangle tabBounds)
        {
            return _layoutHelper?.GetCloseButtonRect(tabBounds) ?? Rectangle.Empty;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            RecalculateLayout();
        }

        #endregion

        #region Animation

        private void StartAnimation(AddinTab tab, float targetProgress)
        {
            if (!_enableAnimations || _animationHelper == null) return;
            _animationHelper.StartAnimation(tab, targetProgress);
        }

        #endregion

        #region Mouse Handling

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            var hitTab = GetTabAt(e.Location);
            
            if (hitTab != _hoveredTab)
            {
                // Update hover state
                if (_hoveredTab != null)
                {
                    _hoveredTab.IsCloseHovered = false;
                    StartAnimation(_hoveredTab, 0f);
                }

                _hoveredTab = hitTab;
                
                if (_hoveredTab != null)
                {
                    StartAnimation(_hoveredTab, 1f);
                }

                Invalidate();
            }

            // Check close button hover
            if (_hoveredTab != null && _showCloseButtons && _hoveredTab.CanClose)
            {
                var closeRect = GetCloseButtonRect(_hoveredTab.Bounds);
                bool isCloseHovered = closeRect.Contains(e.Location);
                
                if (isCloseHovered != _hoveredTab.IsCloseHovered)
                {
                    _hoveredTab.IsCloseHovered = isCloseHovered;
                    Cursor = isCloseHovered ? Cursors.Hand : Cursors.Default;
                    Invalidate();
                }
            }
            else
            {
                Cursor = Cursors.Default;
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            if (_hoveredTab != null)
            {
                _hoveredTab.IsCloseHovered = false;
                StartAnimation(_hoveredTab, 0f);
                _hoveredTab = null;
                Cursor = Cursors.Default;
                Invalidate();
            }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            if (e.Button == MouseButtons.Left)
            {
                var hitTab = GetTabAt(e.Location);
                if (hitTab != null)
                {
                    // Check if clicking close button
                    if (_showCloseButtons && hitTab.CanClose)
                    {
                        var closeRect = GetCloseButtonRect(hitTab.Bounds);
                        if (closeRect.Contains(e.Location))
                        {
                            RemoveTab(hitTab);
                            return;
                        }
                    }

                    // Activate tab
                    ActivateTab(hitTab);
                }
                else
                {
                    // Check scroll buttons
                    if (_needsScrolling)
                    {
                        if (_scrollLeftButton.Contains(e.Location))
                        {
                            ScrollTabs(-1);
                        }
                        else if (_scrollRightButton.Contains(e.Location))
                        {
                            ScrollTabs(1);
                        }
                        else if (_newTabButton.Contains(e.Location))
                        {
                            // Trigger new tab event
                            OnNewTabRequested();
                        }
                    }
                }
            }
        }

        private void ScrollTabs(int direction)
        {
            _scrollOffset += direction;
            _scrollOffset = Math.Max(0, Math.Min(_tabs.Count - 1, _scrollOffset));
            CalculateTabLayout();
            Invalidate();
        }

        private void OnNewTabRequested()
        {
            // Override in derived classes or handle via events
        }

        #endregion

        #region Tab Management

        private void ActivateTab(AddinTab tab)
        {
            if (tab == _activeTab) return;

            var oldTab = _activeTab;
            _activeTab = tab;

            // Raise events
            if (oldTab != null)
            {
                OnAddinChanging(new ContainerEvents 
                { 
                    TitleText = oldTab.Id, 
                    Control = oldTab.Addin, 
                    ContainerType = _containerType, 
                    Guidid = oldTab.Addin?.GuidID 
                });
            }

            PositionActiveAddin();
            OnAddinChanged(new ContainerEvents 
            { 
                TitleText = tab.Id, 
                Control = tab.Addin, 
                ContainerType = _containerType, 
                Guidid = tab.Addin?.GuidID 
            });
            
            Invalidate();
        }

        private void RemoveTab(AddinTab tab)
        {
            if (tab.CanClose)
            {
                _tabs.Remove(tab);
                _addins.Remove(tab.Id);

                // Dispose the addin
                if (tab.Addin is IDisposable disposable)
                {
                    disposable.Dispose();
                }

                // If this was the active tab, activate another
                if (tab == _activeTab)
                {
                    _activeTab = _tabs.FirstOrDefault();
                    if (_activeTab != null)
                    {
                        ActivateTab(_activeTab);
                    }
                }

                OnAddinRemoved(new ContainerEvents 
                { 
                    TitleText = tab.Id, 
                    Control = tab.Addin, 
                    ContainerType = _containerType, 
                    Guidid = tab.Addin?.GuidID 
                });
                RecalculateLayout();
                Invalidate();
            }
        }

        #endregion

        #region IDisplayContainer Implementation

        public bool AddControl(string titleText, IDM_Addin control, ContainerTypeEnum pcontainerType)
        {
            try
            {
                var id = Guid.NewGuid().ToString();
                var tab = new AddinTab
                {
                    Id = id,
                    Title = titleText,
                    Addin = control,
                    CanClose = true,
                    IsVisible = true
                };

                _tabs.Add(tab);
                _addins[id] = control;

                // If this is the first tab, make it active
                if (_activeTab == null)
                {
                    _activeTab = tab;
                }

                RecalculateLayout();
                OnAddinAdded(new ContainerEvents 
                { 
                    TitleText = titleText, 
                    Control = control, 
                    ContainerType = pcontainerType, 
                    Guidid = control?.GuidID 
                });
                Invalidate();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void Clear()
        {
            foreach (var tab in _tabs.ToList())
            {
                RemoveTab(tab);
            }
        }

        public bool IsControlExit(IDM_Addin control)
        {
            return _addins.ContainsValue(control);
        }

        public IErrorsInfo PressKey(KeyCombination keyCombination)
        {
            // Handle keyboard shortcuts
            KeyPressed?.Invoke(this, keyCombination);
            return new ErrorsInfo { Flag = Errors.Ok };
        }

        public bool RemoveControl(string titleText, IDM_Addin control)
        {
            var tab = _tabs.FirstOrDefault(t => t.Title == titleText && t.Addin == control);
            if (tab != null)
            {
                RemoveTab(tab);
                return true;
            }
            return false;
        }

        public bool RemoveControlByGuidTag(string guidid)
        {
            var tab = _tabs.FirstOrDefault(t => t.Id == guidid);
            if (tab != null)
            {
                RemoveTab(tab);
                return true;
            }
            return false;
        }

        public bool RemoveControlByName(string name)
        {
            var tab = _tabs.FirstOrDefault(t => t.Title == name);
            if (tab != null)
            {
                RemoveTab(tab);
                return true;
            }
            return false;
        }

        public bool ShowControl(string titleText, IDM_Addin control)
        {
            var tab = _tabs.FirstOrDefault(t => t.Title == titleText && t.Addin == control);
            if (tab != null)
            {
                ActivateTab(tab);
                return true;
            }
            return false;
        }

        #endregion

        #region Event Handlers

        protected virtual void OnAddinAdded(ContainerEvents e)
        {
            AddinAdded?.Invoke(this, e);
        }

        protected virtual void OnAddinRemoved(ContainerEvents e)
        {
            AddinRemoved?.Invoke(this, e);
        }

        protected virtual void OnAddinMoved(ContainerEvents e)
        {
            AddinMoved?.Invoke(this, e);
        }

        protected virtual void OnAddinChanging(ContainerEvents e)
        {
            AddinChanging?.Invoke(this, e);
        }

        protected virtual void OnAddinChanged(ContainerEvents e)
        {
            AddinChanged?.Invoke(this, e);
        }

        #endregion

        #region Cleanup

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _animationTimer?.Stop();
                _animationTimer?.Dispose();
                _animationHelper?.Dispose();
                
                // Dispose all addins
                foreach (var addin in _addins.Values)
                {
                    if (addin is IDisposable disposable)
                    {
                        disposable.Dispose();
                    }
                }
                
                _tabs.Clear();
                _addins.Clear();
            }
            
            base.Dispose(disposing);
        }

        #endregion
    }

    #region Supporting Classes and Enums

    public class AddinTab
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public IDM_Addin Addin { get; set; }
        public Rectangle Bounds { get; set; }
        public bool IsVisible { get; set; } = true;
        public bool CanClose { get; set; } = true;
        public bool IsCloseHovered { get; set; }
        public float AnimationProgress { get; set; }
        public float TargetAnimationProgress { get; set; }
    }

    public enum ContainerDisplayMode
    {
        Tabbed,
        Tiles,
        List,
        Accordion,
        Stack
    }

    public enum TabPosition
    {
        Top,
        Bottom,
        Left,
        Right
    }

    public enum AnimationSpeed
    {
        Slow,
        Normal,
        Fast
    }

    public enum ArrowDirection
    {
        Up,
        Down,
        Left,
        Right
    }

    #endregion
}