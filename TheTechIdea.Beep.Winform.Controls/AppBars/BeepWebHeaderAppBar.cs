using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.AppBars.StylePainters;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.ConfigUtil;

namespace TheTechIdea.Beep.Winform.Controls.AppBars
{
    [ToolboxItem(true)]
    [DisplayName("Beep WebHeader AppBar")]
    [Category("Beep Controls")]
    [Description("Modern website-style header with tabs, search, and action buttons")]
    public class BeepWebHeaderAppBar : BaseControl
    {
        #region Events

        public event EventHandler<SelectedItemChangedEventArgs> TabSelected;
        public event EventHandler<SelectedItemChangedEventArgs> ActionButtonClicked;
        public event EventHandler<SearchChangedEventArgs> SearchBoxChanged;
        public event EventHandler<SelectedTabChangedEventArgs> SelectedTabChanged;
        public event EventHandler<PopupEventArgs>? PopupOpened;
        public event EventHandler<PopupEventArgs>? PopupClosed;

        #endregion

        #region Private Fields

        private WebHeaderStyle _headerStyle = WebHeaderStyle.ShoppyStore1;
        private TabIndicatorStyle _indicatorStyle = TabIndicatorStyle.UnderlineSimple;
        private LabelVisibilityPolicy _labelVisibility = LabelVisibilityPolicy.Always;
        private BindingList<SimpleItem> _tabs = new();
        private BindingList<SimpleItem> _buttons = new();
        private int _selectedTabIndex = -1;
        private string _logoImagePath = "";
        private string _logoText = "";
        private string _searchText = "";
        private bool _showLogo = true;
        private bool _showSearchBox = true;
        private int _headerHeight = 60;
        private int _logoWidth = 40;
        private int _elementPadding = 12;
        private int _tabSpacing = 15;
        private int _indicatorThickness = 3;
        private Font _tabFont = new Font("Segoe UI", 12);
        private Font _buttonFont = new Font("Segoe UI", 11);

        // Interaction state (index-based)
        private int _hoveredTabIndex = -1;
        private int _hoveredButtonIndex = -1;

        // Search box interaction
        private bool _searchBoxActive = false;
        private bool _searchBoxHovered = false;
        private Rectangle _searchBounds = Rectangle.Empty;

        // Painter and colors
        private IWebHeaderStylePainter _painter;
        private WebHeaderColors _colors;
        private SimpleItem _selectedTab;

        // Underline animation state
        private RectangleF _currentUnderlineRect = RectangleF.Empty;
        private RectangleF _targetUnderlineRect = RectangleF.Empty;
        private RectangleF _startUnderlineRect = RectangleF.Empty;
        private bool _isUnderlineAnimating = false;
        private Timer _underlineTimer = null;
        private int _underlineAnimationDurationMs = 220;
        private DateTime _underlineAnimStartedAt = DateTime.MinValue;

        // Tab overflow scrolling
        private int _tabScrollOffset = 0;
        private bool _needsTabScroll = false;
        private Rectangle _scrollLeftBounds = Rectangle.Empty;
        private Rectangle _scrollRightBounds = Rectangle.Empty;
        private bool _scrollLeftHovered = false;
        private bool _scrollRightHovered = false;

        // Cached painter lists to reduce GC pressure
        private List<WebHeaderTab> _cachedPainterTabs;
        private List<WebHeaderActionButton> _cachedPainterButtons;
        private bool _painterTabsDirty = true;
        private bool _painterButtonsDirty = true;

        // Tooltip support
        private ToolTip _toolTip;
        private string _lastTooltipText = "";

        // Child popup tracking
        private bool _popupOpen = false;
        private int _popupParentIndex = -1;
        private bool _isPopupTab = true;

        // ID generation
        private static int _nextId = 1;
        private static int NextId => System.Threading.Interlocked.Increment(ref _nextId);

        #endregion

        #region Design-Time Safety
        private bool IsDesignModeSafe =>
            LicenseManager.UsageMode == LicenseUsageMode.Designtime ||
            DesignMode ||
            (Site?.DesignMode ?? false);
        #endregion

        #region Constructor

        public BeepWebHeaderAppBar()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
            Height = _headerHeight;
            BackColor = Color.White;
            _painter = new ShoppyStore1Painter();
            IsTransparentBackground = true;
            ApplyThemeToChilds = true;
            IsFrameless = true;
            IsRounded = false;
            UseThemeFont = false;
            EnableSplashEffect = false;
            EnableRippleEffect = false;
            CanBeFocused = true;
            CanBeSelected = false;
            CanBePressed = false;
            CanBeHovered = true;

            AccessibleName = "Web Header Navigation";
            AccessibleRole = AccessibleRole.MenuBar;
            AccessibleDescription = "Navigation header with tabs, search, and action buttons";

            _toolTip = new ToolTip { InitialDelay = 500, ReshowDelay = 100, ShowAlways = true };

            _tabs.ListChanged += Tabs_ListChanged;
            _buttons.ListChanged += Buttons_ListChanged;

            if (!IsDesignModeSafe)
            {
                InitializeAnimationTimer();
            }
        }

        private void Tabs_ListChanged(object sender, ListChangedEventArgs e)
        {
            InvalidatePainterCache();
            RecalculateScroll();
            Invalidate();
        }

        private void Buttons_ListChanged(object sender, ListChangedEventArgs e)
        {
            InvalidatePainterCache();
            Invalidate();
        }

        private void InitializeAnimationTimer()
        {
            _underlineTimer = new Timer { Interval = 15 };
            _underlineTimer.Tick += UnderlineTimer_Tick;
        }

        private void UnderlineTimer_Tick(object sender, EventArgs e)
        {
            if (!_isUnderlineAnimating || IsDesignModeSafe)
            {
                _underlineTimer?.Stop();
                return;
            }

            var elapsed = (DateTime.Now - _underlineAnimStartedAt).TotalMilliseconds;
            double t = Math.Min(1.0, elapsed / _underlineAnimationDurationMs);
            double tt = 1 - Math.Pow(1 - t, 3);

            _currentUnderlineRect = Lerp(_startUnderlineRect, _targetUnderlineRect, tt);
            Invalidate();

            if (t >= 1.0)
            {
                _isUnderlineAnimating = false;
                _underlineTimer?.Stop();
            }
        }

        #endregion

        #region Properties

        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(WebHeaderStyle.ShoppyStore1)]
        public WebHeaderStyle HeaderStyle
        {
            get => _headerStyle;
            set
            {
                if (_headerStyle != value)
                {
                    _headerStyle = value;
                    _painter = CreatePainterForStyle(value);
                    InvalidatePainterCache();
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        public string LogoText
        {
            get => _logoText;
            set { _logoText = value; Invalidate(); }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(TabIndicatorStyle.UnderlineSimple)]
        public TabIndicatorStyle IndicatorStyle
        {
            get => _indicatorStyle;
            set { _indicatorStyle = value; Invalidate(); }
        }

        [Browsable(true)]
        [Category("Data")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public BindingList<SimpleItem> Tabs
        {
            get => _tabs;
            set
            {
                _tabs.ListChanged -= Tabs_ListChanged;
                _tabs = value ?? new BindingList<SimpleItem>();
                _tabs.ListChanged += Tabs_ListChanged;
                RecalculateScroll();
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Data")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public BindingList<SimpleItem> ActionButtons
        {
            get => _buttons;
            set
            {
                _buttons.ListChanged -= Buttons_ListChanged;
                _buttons = value ?? new BindingList<SimpleItem>();
                _buttons.ListChanged += Buttons_ListChanged;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        public string LogoImagePath
        {
            get => _logoImagePath;
            set { _logoImagePath = value; Invalidate(); }
        }

        [Browsable(true)]
        [Category("Layout")]
        [DefaultValue(40)]
        public int LogoWidth
        {
            get => _logoWidth;
            set { _logoWidth = value; Invalidate(); }
        }

        [Browsable(true)]
        [Category("Layout")]
        [DefaultValue(60)]
        public int HeaderHeight
        {
            get => _headerHeight;
            set { _headerHeight = value; Height = value; Invalidate(); }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(true)]
        public bool ShowLogo
        {
            get => _showLogo;
            set { _showLogo = value; Invalidate(); }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(true)]
        public bool ShowSearchBox
        {
            get => _showSearchBox;
            set { _showSearchBox = value; Invalidate(); }
        }

        [Browsable(false)]
        public SimpleItem SelectedTab
        {
            get => _selectedTab;
            set
            {
                if (_selectedTab != value)
                {
                    var old = _selectedTab;
                    _selectedTab = value;
                    int idx = _tabs.IndexOf(value);
                    if (idx >= 0) _selectedTabIndex = idx;
                    SelectedTabChanged?.Invoke(this, new SelectedTabChangedEventArgs(old, value, _selectedTabIndex));
                    TabSelected?.Invoke(this, new SelectedItemChangedEventArgs(_selectedTab));
                    Invalidate();
                }
            }
        }

        [Browsable(false)]
        public int SelectedTabIndex
        {
            get => _selectedTabIndex;
            set
            {
                if (value >= 0 && value < _tabs.Count && value != _selectedTabIndex)
                {
                    var old = _selectedTab;
                    _selectedTabIndex = value;
                    _selectedTab = _tabs[value];
                    InvalidatePainterCache();
                    SelectedTabChanged?.Invoke(this, new SelectedTabChangedEventArgs(old, _selectedTab, value));
                    TabSelected?.Invoke(this, new SelectedItemChangedEventArgs(_selectedTab));
                    StartUnderlineAnimationToTab(value);
                }
            }
        }

        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(true)]
        public bool AllowTabScroll { get; set; } = true;

        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(220)]
        [Description("Duration of the tab underline animation in milliseconds")]
        public int UnderlineAnimationDurationMs
        {
            get => _underlineAnimationDurationMs;
            set => _underlineAnimationDurationMs = Math.Max(0, value);
        }

        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(LabelVisibilityPolicy.Always)]
        [Description("Controls when tab and button labels are visible")]
        public LabelVisibilityPolicy LabelVisibility
        {
            get => _labelVisibility;
            set
            {
                if (_labelVisibility != value)
                {
                    _labelVisibility = value;
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Layout")]
        [DefaultValue(48)]
        [Description("Minimum touch target width for tabs and buttons (pixels)")]
        public int MinTouchTargetWidth { get; set; } = 48;

        [Browsable(false)]
        public bool IsPopupOpen => _popupOpen;

        #endregion

        #region Methods

        public void AddTab(string text, string imagePath = "")
        {
            var tab = new SimpleItem { Text = text, ImagePath = imagePath, ID = NextId };
            _tabs.Add(tab);
        }

        public void AddTab(string text, BindingList<SimpleItem> children, string imagePath = "")
        {
            var tab = new SimpleItem { Text = text, ImagePath = imagePath, Children = children, ID = NextId };
            _tabs.Add(tab);
        }

        public void RemoveTabAt(int index)
        {
            if (index >= 0 && index < _tabs.Count)
            {
                _tabs.RemoveAt(index);
                if (_selectedTabIndex >= _tabs.Count) _selectedTabIndex = _tabs.Count - 1;
            }
        }

        public void ClearTabs()
        {
            _tabs.Clear();
            _selectedTabIndex = -1;
            _selectedTab = null;
        }

        public void AddActionButton(string text, string imagePath = "")
        {
            var btn = new SimpleItem { Text = text, ImagePath = imagePath, ID = NextId };
            _buttons.Add(btn);
        }

        public void RemoveActionButtonAt(int index)
        {
            if (index >= 0 && index < _buttons.Count)
                _buttons.RemoveAt(index);
        }

        public void ClearActionButtons()
        {
            _buttons.Clear();
        }

        private void RecalculateScroll()
        {
            if (!AllowTabScroll || _tabs.Count == 0)
            {
                _needsTabScroll = false;
                _tabScrollOffset = 0;
                return;
            }

            int totalWidth = 0;
            foreach (var tab in _tabs)
            {
                using var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                var size = TextRenderer.MeasureText(tab.Text, _tabFont);
                int itemWidth = Math.Max(MinTouchTargetWidth, size.Width + _elementPadding * 2);
                totalWidth += itemWidth + _tabSpacing;
            }

            int availableWidth = Width - 160;
            _needsTabScroll = totalWidth > availableWidth;
            if (!_needsTabScroll)
                _tabScrollOffset = 0;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            RecalculateScroll();
        }

        #endregion

        #region Painting

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (IsDesignModeSafe)
            {
                PaintDesignTimePlaceholder(e.Graphics);
                return;
            }

            if (_painter == null)
                _painter = CreatePainterForStyle(_headerStyle);

            if (_colors == null)
                _colors = WebHeaderColors.FromTheme(_currentTheme);

            var tabsList = ConvertTabsForPainter();
            var buttonsList = ConvertButtonsForPainter();

            _painter.PaintHeader(
                e.Graphics,
                new Rectangle(0, 0, Width, Height),
                _currentTheme,
                _colors,
                tabsList,
                buttonsList,
                _selectedTabIndex,
                _logoImagePath,
                _logoText,
                _showLogo,
                _showSearchBox,
                _searchText,
                _tabFont,
                _buttonFont,
                _tabScrollOffset,
                IsTransparentBackground,
                out _searchBounds);

            if (_indicatorStyle == TabIndicatorStyle.SlidingUnderline)
            {
                if (_currentUnderlineRect.IsEmpty && _selectedTabIndex >= 0)
                {
                    var tabsList2 = ConvertTabsForPainter();
                    var rectInit = _painter.GetTabBounds(_selectedTabIndex, new Rectangle(0, 0, Width, Height), tabsList2);
                    if (rectInit != Rectangle.Empty)
                        _currentUnderlineRect = new RectangleF(rectInit.Left, rectInit.Bottom - _indicatorThickness, rectInit.Width, _indicatorThickness);
                }

                if (!_currentUnderlineRect.IsEmpty)
                {
                    using var brush = new SolidBrush(_currentTheme?.ForeColor ?? Color.Black);
                    e.Graphics.FillRectangle(brush, Rectangle.Round(_currentUnderlineRect));
                }
            }

            if (_needsTabScroll && AllowTabScroll)
            {
                DrawScrollArrows(e.Graphics);
            }
        }

        private void DrawScrollArrows(Graphics g)
        {
            int arrowSize = 24;
            int y = (Height - arrowSize) / 2;

            _scrollLeftBounds = new Rectangle(2, y, arrowSize, arrowSize);
            _scrollRightBounds = new Rectangle(Width - arrowSize - 2, y, arrowSize, arrowSize);

            var color = _scrollLeftHovered ? Color.FromArgb(200, 0, 0, 0) : Color.FromArgb(100, 0, 0, 0);
            using var brush = new SolidBrush(color);
            using var path = new System.Drawing.Drawing2D.GraphicsPath();

            if (_tabScrollOffset > 0)
            {
                path.AddPolygon(new[] {
                    new Point(_scrollLeftBounds.Right - 8, _scrollLeftBounds.Top + 4),
                    new Point(_scrollLeftBounds.Left + 8, _scrollLeftBounds.Top + arrowSize / 2),
                    new Point(_scrollLeftBounds.Right - 8, _scrollLeftBounds.Bottom - 4)
                });
                g.FillPath(brush, path);
            }

            path.Reset();
            if (_tabScrollOffset < GetMaxScrollOffset())
            {
                path.AddPolygon(new[] {
                    new Point(_scrollRightBounds.Left + 8, _scrollRightBounds.Top + 4),
                    new Point(_scrollRightBounds.Right - 8, _scrollRightBounds.Top + arrowSize / 2),
                    new Point(_scrollRightBounds.Left + 8, _scrollRightBounds.Bottom - 4)
                });
                g.FillPath(brush, path);
            }
        }

        private int GetMaxScrollOffset()
        {
            int totalWidth = 0;
            foreach (var tab in _tabs)
            {
                var size = TextRenderer.MeasureText(tab.Text, _tabFont);
                int itemWidth = Math.Max(MinTouchTargetWidth, size.Width + _elementPadding * 2);
                totalWidth += itemWidth + _tabSpacing;
            }
            return Math.Max(0, totalWidth - (Width - 160));
        }

        private void PaintDesignTimePlaceholder(Graphics g)
        {
            g.Clear(Color.FromArgb(255, 255, 255));
            using (var pen = new Pen(Color.FromArgb(230, 230, 230), 1))
                g.DrawLine(pen, 0, Height - 1, Width, Height - 1);

            int logoX = 16;
            int logoY = (Height - 32) / 2;
            using (var brush = new SolidBrush(Color.FromArgb(66, 133, 244)))
            using (var font = new Font("Segoe UI", 11, FontStyle.Bold))
            {
                string logoDisplay = !string.IsNullOrEmpty(_logoText) ? _logoText : "Logo";
                g.DrawString(logoDisplay, font, brush, logoX, logoY + 6);
            }

            int tabX = 120;
            int tabCount = _tabs?.Count ?? 0;
            if (tabCount == 0) tabCount = 4;

            using (var font = new Font("Segoe UI", 10))
            using (var brush = new SolidBrush(Color.FromArgb(80, 80, 80)))
            using (var activeBrush = new SolidBrush(Color.FromArgb(66, 133, 244)))
            {
                string[] sampleTabs = { "Home", "Products", "About", "Contact" };
                for (int i = 0; i < Math.Min(tabCount, 6); i++)
                {
                    string tabText = _tabs != null && i < _tabs.Count ? _tabs[i].Text : sampleTabs[i % sampleTabs.Length];
                    var textBrush = i == _selectedTabIndex ? activeBrush : brush;
                    g.DrawString(tabText, font, textBrush, tabX, (Height - 16) / 2);
                    tabX += 80;
                }
            }

            int btnX = Width - 120;
            int btnCount = _buttons?.Count ?? 0;
            if (btnCount == 0) btnCount = 2;

            using (var font = new Font("Segoe UI", 9))
            {
                var signInRect = new Rectangle(btnX - 80, (Height - 32) / 2, 70, 32);
                using (var pen = new Pen(Color.FromArgb(200, 200, 200), 1))
                using (var brush = new SolidBrush(Color.FromArgb(80, 80, 80)))
                {
                    g.DrawRectangle(pen, signInRect);
                    g.DrawString("Sign In", font, brush, signInRect.X + 12, signInRect.Y + 8);
                }

                var ctaRect = new Rectangle(btnX, (Height - 32) / 2, 90, 32);
                using (var fillBrush = new SolidBrush(Color.FromArgb(66, 133, 244)))
                using (var textBrush = new SolidBrush(Color.White))
                {
                    g.FillRectangle(fillBrush, ctaRect);
                    g.DrawString("Get Started", font, textBrush, ctaRect.X + 8, ctaRect.Y + 8);
                }
            }

            using (var font = new Font("Segoe UI", 8))
            using (var brush = new SolidBrush(Color.FromArgb(180, 180, 180)))
                g.DrawString($"BeepWebHeaderAppBar (Design Mode) - Style: {_headerStyle}", font, brush, 4, Height - 16);
        }

        private void StartUnderlineAnimationToTab(int tabIndex)
        {
            if (IsDesignModeSafe || _painter == null || tabIndex < 0 || tabIndex >= _tabs.Count)
                return;

            var tabsList = ConvertTabsForPainter();
            var rect = _painter.GetTabBounds(tabIndex, new Rectangle(0, 0, Width, Height), tabsList);
            if (rect == Rectangle.Empty) return;

            var target = new RectangleF(rect.Left, rect.Bottom - _indicatorThickness, rect.Width, _indicatorThickness);
            if (_currentUnderlineRect.IsEmpty)
                _currentUnderlineRect = target;

            _startUnderlineRect = _currentUnderlineRect;
            _targetUnderlineRect = target;
            _underlineAnimStartedAt = DateTime.Now;
            _isUnderlineAnimating = true;

            if (_underlineTimer == null)
                InitializeAnimationTimer();
            _underlineTimer?.Start();
        }

        private static RectangleF Lerp(RectangleF a, RectangleF b, double t)
        {
            return new RectangleF(
                (float)(a.Left + (b.Left - a.Left) * t),
                (float)(a.Top + (b.Top - a.Top) * t),
                (float)(a.Width + (b.Width - a.Width) * t),
                (float)(a.Height + (b.Height - a.Height) * t));
        }

        private List<WebHeaderTab> ConvertTabsForPainter()
        {
            if (!_painterTabsDirty && _cachedPainterTabs != null)
                return _cachedPainterTabs;

            if (_cachedPainterTabs == null)
                _cachedPainterTabs = new List<WebHeaderTab>(_tabs.Count);
            else
                _cachedPainterTabs.Clear();

            for (int i = 0; i < _tabs.Count; i++)
            {
                var item = _tabs[i];
                _cachedPainterTabs.Add(new WebHeaderTab(item.Text, item.ImagePath, item.ID)
                {
                    IsActive = (i == _selectedTabIndex),
                    IsHovered = (i == _hoveredTabIndex),
                    Tooltip = item.Description,
                    HasChildren = item.Children != null && item.Children.Count > 0
                });
            }

            _painterTabsDirty = false;
            return _cachedPainterTabs;
        }

        private List<WebHeaderActionButton> ConvertButtonsForPainter()
        {
            if (!_painterButtonsDirty && _cachedPainterButtons != null)
                return _cachedPainterButtons;

            if (_cachedPainterButtons == null)
                _cachedPainterButtons = new List<WebHeaderActionButton>(_buttons.Count);
            else
                _cachedPainterButtons.Clear();

            for (int i = 0; i < _buttons.Count; i++)
            {
                var item = _buttons[i];
                _cachedPainterButtons.Add(new WebHeaderActionButton(item.Text, item.ImagePath, WebHeaderButtonStyle.Solid, item.ID)
                {
                    IsHovered = (i == _hoveredButtonIndex),
                    BadgeCount = GetBadgeCount(item)
                });
            }

            _painterButtonsDirty = false;
            return _cachedPainterButtons;
        }

        private void InvalidatePainterCache()
        {
            _painterTabsDirty = true;
            _painterButtonsDirty = true;
        }

        protected virtual int GetBadgeCount(SimpleItem item)
        {
            if (item.Tag is int count)
                return count;
            return 0;
        }

        #endregion

        #region Mouse Events

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (IsDesignModeSafe || _painter == null) return;

            var tabsList = ConvertTabsForPainter();
            var buttonsList = ConvertButtonsForPainter();

            int hit = _painter.GetHitElement(
                e.Location,
                new Rectangle(0, 0, Width, Height),
                tabsList,
                buttonsList);

            bool needsInvalidate = false;
            int prevTab = _hoveredTabIndex;
            int prevBtn = _hoveredButtonIndex;
            bool prevSearch = _searchBoxHovered;

            _hoveredTabIndex = -1;
            _hoveredButtonIndex = -1;
            _searchBoxHovered = false;

            if (_needsTabScroll && AllowTabScroll)
            {
                if (_scrollLeftBounds.Contains(e.Location))
                {
                    _scrollLeftHovered = true;
                    Cursor = Cursors.Hand;
                    needsInvalidate = true;
                }
                else if (_scrollRightBounds.Contains(e.Location))
                {
                    _scrollRightHovered = true;
                    Cursor = Cursors.Hand;
                    needsInvalidate = true;
                }
                else
                {
                    _scrollLeftHovered = false;
                    _scrollRightHovered = false;
                }
            }

            if (_showSearchBox && _searchBounds.Contains(e.Location))
            {
                _searchBoxHovered = true;
                Cursor = _searchBoxActive ? Cursors.IBeam : Cursors.Hand;
                needsInvalidate = true;
                ShowTooltipForText(!string.IsNullOrEmpty(_searchText) ? _searchText : "Click to search");
            }
            else if (hit >= 0 && hit < _tabs.Count)
            {
                _hoveredTabIndex = hit;
                Cursor = Cursors.Hand;
                if (prevTab != hit) needsInvalidate = true;
                var tab = _tabs[hit];
                string tooltip = !string.IsNullOrEmpty(tab.Description) ? tab.Description : tab.Text;
                ShowTooltipForText(tooltip);
            }
            else if (hit < -1)
            {
                int btnIndex = -(hit + 1);
                if (btnIndex >= 0 && btnIndex < _buttons.Count)
                {
                    _hoveredButtonIndex = btnIndex;
                    Cursor = Cursors.Hand;
                    if (prevBtn != btnIndex) needsInvalidate = true;
                    var btn = _buttons[btnIndex];
                    string tooltip = !string.IsNullOrEmpty(btn.Description) ? btn.Description : btn.Text;
                    ShowTooltipForText(tooltip);
                }
            }
            else
            {
                Cursor = Cursors.Default;
                if (prevTab != -1 || prevBtn != -1 || prevSearch) needsInvalidate = true;
                HideTooltip();
            }

            if (needsInvalidate)
                Invalidate();
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            if (IsDesignModeSafe || _painter == null || e.Button != MouseButtons.Left) return;

            if (_popupOpen)
            {
                CloseChildPopup();
                return;
            }

            if (_needsTabScroll && AllowTabScroll)
            {
                if (_scrollLeftBounds.Contains(e.Location) && _tabScrollOffset > 0)
                {
                    _tabScrollOffset = Math.Max(0, _tabScrollOffset - 50);
                    Invalidate();
                    return;
                }
                if (_scrollRightBounds.Contains(e.Location) && _tabScrollOffset < GetMaxScrollOffset())
                {
                    _tabScrollOffset = Math.Min(GetMaxScrollOffset(), _tabScrollOffset + 50);
                    Invalidate();
                    return;
                }
            }

            if (_showSearchBox && _searchBounds.Contains(e.Location))
            {
                _searchBoxActive = !_searchBoxActive;
                if (_searchBoxActive)
                {
                    Focus();
                    Invalidate();
                }
                return;
            }

            var tabsList = ConvertTabsForPainter();
            var buttonsList = ConvertButtonsForPainter();

            int hit = _painter.GetHitElement(
                e.Location,
                new Rectangle(0, 0, Width, Height),
                tabsList,
                buttonsList);

            if (hit >= 0 && hit < _tabs.Count)
                HandleTabClick(hit);
            else if (hit < -1)
            {
                int btnIndex = -(hit + 1);
                if (btnIndex >= 0 && btnIndex < _buttons.Count)
                    HandleActionButtonClick(btnIndex);
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            if (IsDesignModeSafe) return;

            bool needsInvalidate = _hoveredTabIndex != -1 || _hoveredButtonIndex != -1 || _searchBoxHovered;
            _hoveredTabIndex = -1;
            _hoveredButtonIndex = -1;
            _searchBoxHovered = false;
            _scrollLeftHovered = false;
            _scrollRightHovered = false;
            Cursor = Cursors.Default;
            InvalidatePainterCache();
            HideTooltip();

            if (needsInvalidate)
                Invalidate();
        }

        private void ShowTooltipForText(string text)
        {
            if (_toolTip == null || string.IsNullOrEmpty(text)) return;
            if (_popupOpen) return;
            if (text == _lastTooltipText) return;

            _lastTooltipText = text;
            _toolTip.SetToolTip(this, text);
        }

        private void HideTooltip()
        {
            if (_toolTip == null) return;
            _lastTooltipText = "";
            _toolTip.SetToolTip(this, "");
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            if (!_needsTabScroll || !AllowTabScroll) return;

            int delta = e.Delta > 0 ? -50 : 50;
            _tabScrollOffset = Math.Max(0, Math.Min(GetMaxScrollOffset(), _tabScrollOffset + delta));
            Invalidate();
        }

        #endregion

        #region Keyboard Navigation

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (IsDesignModeSafe) return base.ProcessCmdKey(ref msg, keyData);

            if (_searchBoxActive)
            {
                if (keyData == Keys.Escape)
                {
                    _searchBoxActive = false;
                    Invalidate();
                    return true;
                }
                if (keyData == Keys.Enter)
                {
                    SearchBoxChanged?.Invoke(this, new SearchChangedEventArgs(_searchText));
                    return true;
                }
                if (keyData == (Keys.Control | Keys.V))
                {
                    var clipboard = Clipboard.GetText();
                    if (!string.IsNullOrEmpty(clipboard))
                    {
                        _searchText += clipboard;
                        SearchBoxChanged?.Invoke(this, new SearchChangedEventArgs(_searchText));
                        Invalidate();
                    }
                    return true;
                }
                if (keyData == (Keys.Control | Keys.C))
                {
                    if (!string.IsNullOrEmpty(_searchText))
                        Clipboard.SetText(_searchText);
                    return true;
                }
                if (keyData == (Keys.Control | Keys.A))
                {
                    return true;
                }
                if (keyData == (Keys.Control | Keys.Back))
                {
                    int lastSpace = _searchText.TrimEnd().LastIndexOf(' ');
                    _searchText = lastSpace >= 0 ? _searchText.Substring(0, lastSpace + 1) : "";
                    SearchBoxChanged?.Invoke(this, new SearchChangedEventArgs(_searchText));
                    Invalidate();
                    return true;
                }
                if (keyData == Keys.Back && _searchText.Length > 0)
                {
                    _searchText = _searchText.Substring(0, _searchText.Length - 1);
                    SearchBoxChanged?.Invoke(this, new SearchChangedEventArgs(_searchText));
                    Invalidate();
                    return true;
                }
                if (keyData == Keys.Delete)
                {
                    if (_searchText.Length > 0)
                    {
                        _searchText = "";
                        SearchBoxChanged?.Invoke(this, new SearchChangedEventArgs(_searchText));
                        Invalidate();
                    }
                    return true;
                }
                if (keyData == Keys.Left || keyData == Keys.Right)
                {
                    return base.ProcessCmdKey(ref msg, keyData);
                }

                const int WM_CHAR = 0x0102;
                if (msg.Msg == WM_CHAR)
                {
                    int charCode = msg.WParam.ToInt32();
                    if (charCode >= 32 && charCode != 127)
                    {
                        _searchText += (char)charCode;
                        SearchBoxChanged?.Invoke(this, new SearchChangedEventArgs(_searchText));
                        Invalidate();
                        return true;
                    }
                }

                return base.ProcessCmdKey(ref msg, keyData);
            }

            switch (keyData)
            {
                case Keys.Escape:
                    if (_popupOpen)
                    {
                        CloseChildPopup();
                        return true;
                    }
                    if (_searchBoxActive)
                    {
                        _searchBoxActive = false;
                        Invalidate();
                        return true;
                    }
                    return false;
                case Keys.Left:
                    if (_selectedTabIndex > 0)
                        SelectedTabIndex = _selectedTabIndex - 1;
                    return true;
                case Keys.Right:
                    if (_selectedTabIndex < _tabs.Count - 1)
                        SelectedTabIndex = _selectedTabIndex + 1;
                    return true;
                case Keys.Home:
                    if (_tabs.Count > 0)
                        SelectedTabIndex = 0;
                    return true;
                case Keys.End:
                    if (_tabs.Count > 0)
                        SelectedTabIndex = _tabs.Count - 1;
                    return true;
                case Keys.Enter:
                    if (_selectedTabIndex >= 0 && _selectedTabIndex < _tabs.Count)
                        HandleTabClick(_selectedTabIndex);
                    return true;
                case (Keys.Control | Keys.Tab):
                    if (_tabs.Count > 0)
                        SelectedTabIndex = (_selectedTabIndex + 1) % _tabs.Count;
                    return true;
                case (Keys.Control | Keys.Shift | Keys.Tab):
                    if (_tabs.Count > 0)
                        SelectedTabIndex = (_selectedTabIndex - 1 + _tabs.Count) % _tabs.Count;
                    return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        #endregion

        #region Tab/Button Handlers

        private void HandleTabClick(int tabIndex)
        {
            if (tabIndex < 0 || tabIndex >= _tabs.Count) return;

            var tab = _tabs[tabIndex];
            SelectedTabIndex = tabIndex;

            if (tab.Children != null && tab.Children.Count > 0)
                ShowTabMenu(tabIndex);
            else if (!string.IsNullOrEmpty(tab.MethodName))
                RunMethodFromGlobalFunctions(tab, tab.Text);
        }

        private void HandleActionButtonClick(int btnIndex)
        {
            if (btnIndex < 0 || btnIndex >= _buttons.Count) return;

            var btn = _buttons[btnIndex];

            if (btn.Children != null && btn.Children.Count > 0)
                ShowButtonMenu(btnIndex);
            else
            {
                ActionButtonClicked?.Invoke(this, new SelectedItemChangedEventArgs(btn));
                if (!string.IsNullOrEmpty(btn.MethodName))
                    RunMethodFromGlobalFunctions(btn, btn.Text);
            }
        }

        private void ShowButtonMenu(int btnIndex)
        {
            if (btnIndex < 0 || btnIndex >= _buttons.Count) return;

            var btn = _buttons[btnIndex];
            if (btn.Children == null || btn.Children.Count == 0) return;

            var buttonsList = ConvertButtonsForPainter();
            var rect = _painter.GetButtonBounds(btnIndex, new Rectangle(0, 0, Width, Height), buttonsList);

            if (rect != Rectangle.Empty)
            {
                _popupOpen = true;
                _popupParentIndex = btnIndex;
                _isPopupTab = false;
                PopupOpened?.Invoke(this, new PopupEventArgs(btnIndex, btn, rect, false));

                var screenLocation = this.PointToScreen(new Point(rect.Left, rect.Bottom + 2));
                var selectedItem = base.ShowContextMenu(new List<SimpleItem>(btn.Children), screenLocation, multiSelect: false);

                _popupOpen = false;
                _popupParentIndex = -1;
                PopupClosed?.Invoke(this, new PopupEventArgs(btnIndex, btn, rect, false));

                if (selectedItem != null)
                {
                    ActionButtonClicked?.Invoke(this, new SelectedItemChangedEventArgs(selectedItem));
                    if (!string.IsNullOrEmpty(selectedItem.MethodName))
                        RunMethodFromGlobalFunctions(selectedItem, selectedItem.Text);
                }
            }
        }

        private void ShowTabMenu(int tabIndex)
        {
            if (tabIndex < 0 || tabIndex >= _tabs.Count) return;

            var tab = _tabs[tabIndex];
            if (tab.Children == null || tab.Children.Count == 0) return;

            var tabsList = ConvertTabsForPainter();
            var rect = _painter.GetTabBounds(tabIndex, new Rectangle(0, 0, Width, Height), tabsList);

            if (rect != Rectangle.Empty)
            {
                _popupOpen = true;
                _popupParentIndex = tabIndex;
                _isPopupTab = true;
                PopupOpened?.Invoke(this, new PopupEventArgs(tabIndex, tab, rect, true));

                var screenLocation = this.PointToScreen(new Point(rect.Left, rect.Bottom + 2));
                var selectedItem = base.ShowContextMenu(new List<SimpleItem>(tab.Children), screenLocation, multiSelect: false);

                _popupOpen = false;
                _popupParentIndex = -1;
                PopupClosed?.Invoke(this, new PopupEventArgs(tabIndex, tab, rect, true));

                if (selectedItem != null)
                {
                    SelectedTab = selectedItem;
                    if (!string.IsNullOrEmpty(selectedItem.MethodName))
                        RunMethodFromGlobalFunctions(selectedItem, selectedItem.Text);
                }
            }
        }

        public void CloseChildPopup()
        {
            if (_popupOpen)
            {
                var parentItem = _isPopupTab
                    ? (_popupParentIndex >= 0 && _popupParentIndex < _tabs.Count ? _tabs[_popupParentIndex] : null)
                    : (_popupParentIndex >= 0 && _popupParentIndex < _buttons.Count ? _buttons[_popupParentIndex] : null);

                PopupClosed?.Invoke(this, new PopupEventArgs(_popupParentIndex, parentItem, Rectangle.Empty, _isPopupTab));
                _popupOpen = false;
                _popupParentIndex = -1;
            }
        }

        #endregion

        #region Helpers

        private IWebHeaderStylePainter CreatePainterForStyle(WebHeaderStyle style)
        {
            return style switch
            {
                WebHeaderStyle.ShoppyStore1 => new ShoppyStore1Painter(),
                WebHeaderStyle.ShoppyStore2 => new ShoppyStore2Painter(),
                WebHeaderStyle.TrendModern => new TrendModernPainter(),
                WebHeaderStyle.StudiofokMinimal => new StudiofokMinimalPainter(),
                WebHeaderStyle.EcommerceDark => new EcommerceDarkPainter(),
                WebHeaderStyle.SaaSProfessional => new SaaSProfessionalPainter(),
                WebHeaderStyle.CreativeAgency => new CreativeAgencyPainter(),
                WebHeaderStyle.CorporateMinimal => new CorporateMinimalPainter(),
                WebHeaderStyle.MobileFirst => new MobileFirstPainter(),
                WebHeaderStyle.MaterialDesign3 => new MaterialDesign3Painter(),
                WebHeaderStyle.MinimalClean => new MinimalCleanPainter(),
                WebHeaderStyle.MultiRowCompact => new MultiRowCompactPainter(),
                WebHeaderStyle.StartupHero => new StartupHeroPainter(),
                WebHeaderStyle.PortfolioMinimal => new PortfolioMinimalPainter(),
                WebHeaderStyle.EcommerceModern => new EcommerceModernPainter(),
                _ => new ShoppyStore1Painter()
            };
        }

        public IErrorsInfo RunMethodFromGlobalFunctions(SimpleItem item, string methodName)
        {
            var errorsInfo = new ErrorsInfo();
            try
            {
                SimpleItemFactory.RunFunctionWithTreeHandler(item, methodName);
            }
            catch (Exception ex)
            {
                errorsInfo.Flag = Errors.Failed;
                errorsInfo.Message = ex.Message;
                errorsInfo.Ex = ex;
            }
            return errorsInfo;
        }

        #endregion

        #region Theme Support

        public override void ApplyTheme()
        {
            base.ApplyTheme();
            _colors = WebHeaderColors.FromTheme(_currentTheme);

            if (UseThemeFont && _currentTheme != null)
            {
                if (_currentTheme.LabelFont != null)
                {
                    _tabFont?.Dispose();
                    _tabFont = FontListHelper.CreateFontFromTypography(_currentTheme.LabelFont);
                }
                if (_currentTheme.ButtonFont != null)
                {
                    _buttonFont?.Dispose();
                    _buttonFont = FontListHelper.CreateFontFromTypography(_currentTheme.ButtonFont);
                }
            }

            Invalidate();
        }

        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _tabs.ListChanged -= Tabs_ListChanged;
                _buttons.ListChanged -= Buttons_ListChanged;
                _tabFont?.Dispose();
                _buttonFont?.Dispose();
                if (_underlineTimer != null)
                {
                    _underlineTimer.Stop();
                    _underlineTimer.Dispose();
                    _underlineTimer = null;
                }
            }
            base.Dispose(disposing);
        }
    }

    public class SelectedTabChangedEventArgs : EventArgs
    {
        public SimpleItem OldTab { get; }
        public SimpleItem NewTab { get; }
        public int NewTabIndex { get; }

        public SelectedTabChangedEventArgs(SimpleItem oldTab, SimpleItem newTab, int newTabIndex)
        {
            OldTab = oldTab;
            NewTab = newTab;
            NewTabIndex = newTabIndex;
        }
    }

    public class PopupEventArgs : EventArgs
    {
        public int ParentIndex { get; }
        public SimpleItem ParentItem { get; }
        public Rectangle AnchorRect { get; }
        public bool IsTabPopup { get; }

        public PopupEventArgs(int parentIndex, SimpleItem parentItem, Rectangle anchorRect, bool isTabPopup)
        {
            ParentIndex = parentIndex;
            ParentItem = parentItem;
            AnchorRect = anchorRect;
            IsTabPopup = isTabPopup;
        }
    }
}
