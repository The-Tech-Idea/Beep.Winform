using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
    /// <summary>
    /// Modern website-style header control with painter-based rendering
    /// Uses SimpleItem for tabs with support for nested menus
    /// </summary>
    [ToolboxItem(true)]
    [DisplayName("Beep WebHeader AppBar")]
    [Category("Beep Controls")]
    [Description("Modern website-style header with tabs, search, and action buttons")]
    public class BeepWebHeaderAppBar : BaseControl
    {
        #region Events

        /// <summary>Raised when a tab is selected</summary>
        public event EventHandler<SelectedItemChangedEventArgs> TabSelected;

        /// <summary>Raised when an action button is clicked</summary>
        public event EventHandler<SelectedItemChangedEventArgs> ActionButtonClicked;

        /// <summary>Raised when search text changes</summary>
        public event EventHandler<SearchChangedEventArgs> SearchBoxChanged;

        #endregion

        #region Private Fields

        private WebHeaderStyle _headerStyle = WebHeaderStyle.ShoppyStore1;
        private TabIndicatorStyle _indicatorStyle = TabIndicatorStyle.UnderlineSimple;
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

        // Interaction state
        private string _hoveredTabName = null;
        private string _hoveredButtonName = null;

        // Painter and colors
        private IWebHeaderStylePainter _painter;
        private WebHeaderColors _colors;
        private SimpleItem _selectedTab;
        // Underline animation state
        private RectangleF _currentUnderlineRect = RectangleF.Empty;
        private RectangleF _targetUnderlineRect = RectangleF.Empty;
        private RectangleF _startUnderlineRect = RectangleF.Empty;
        private bool _isUnderlineAnimating = false;
        private System.Windows.Forms.Timer _underlineTimer = null;
        private const int UnderlineAnimationDurationMs = 220; // ms
        private DateTime _underlineAnimStartedAt = DateTime.MinValue;

        #endregion
        
        #region Design-Time Safety
        /// <summary>
        /// Safe design-time detection that works in all scenarios
        /// </summary>
        private bool IsDesignModeSafe => 
            LicenseManager.UsageMode == LicenseUsageMode.Designtime || 
            DesignMode || 
            (Site?.DesignMode ?? false);
        #endregion

        #region Constructor

        /// <summary>Initialize the control</summary>
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
            CanBeFocused = false;
            CanBeSelected = false;
            CanBePressed = false;
            CanBeHovered = false;

            // Only initialize animation timer at runtime to prevent design-time flickering
            if (!IsDesignModeSafe)
            {
                InitializeAnimationTimer();
            }
        }
        
        /// <summary>
        /// Initialize the underline animation timer (runtime only)
        /// </summary>
        private void InitializeAnimationTimer()
        {
            _underlineTimer = new System.Windows.Forms.Timer();
            _underlineTimer.Interval = 15; // ~60 fps
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
            double t = Math.Min(1.0, elapsed / UnderlineAnimationDurationMs);
            // Use ease-out cubic easing
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

        /// <summary>Gets or sets the header style variant</summary>
        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(WebHeaderStyle.ShoppyStore1)]
        [Description("The visual style for the web header.")]
        public WebHeaderStyle HeaderStyle
        {
            get => _headerStyle;
            set
            {
                if (_headerStyle != value)
                {
                    _headerStyle = value;
                    if (!IsDesignModeSafe)
                    {
                        _painter = CreatePainterForStyle(value);
                    }
                    Invalidate();
                }
            }
        }
        
        /// <summary>Gets or sets the logo text (displayed when no logo image)</summary>
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Text to display as logo when no logo image is set.")]
        public string LogoText
        {
            get => _logoText;
            set
            {
                _logoText = value;
                Invalidate();
            }
        }

        /// <summary>Gets or sets the tab indicator style</summary>
        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(TabIndicatorStyle.UnderlineSimple)]
        public TabIndicatorStyle IndicatorStyle
        {
            get => _indicatorStyle;
            set
            {
                _indicatorStyle = value;
                Invalidate();
            }
        }

        /// <summary>Gets or sets the tabs collection</summary>
        [Browsable(true)]
        [Category("Data")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public BindingList<SimpleItem> Tabs
        {
            get => _tabs;
            set
            {
                _tabs = value ?? new BindingList<SimpleItem>();
                Invalidate();
            }
        }

        /// <summary>Gets or sets the action buttons collection</summary>
        [Browsable(true)]
        [Category("Data")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public BindingList<SimpleItem> ActionButtons
        {
            get => _buttons;
            set
            {
                _buttons = value ?? new BindingList<SimpleItem>();
                Invalidate();
            }
        }

        /// <summary>Gets or sets the logo image path</summary>
        [Browsable(true)]
        [Category("Appearance")]
        public string LogoImagePath
        {
            get => _logoImagePath;
            set
            {
                _logoImagePath = value;
                Invalidate();
            }
        }

        /// <summary>Gets or sets the logo width</summary>
        [Browsable(true)]
        [Category("Layout")]
        [DefaultValue(40)]
        public int LogoWidth
        {
            get => _logoWidth;
            set
            {
                _logoWidth = value;
                Invalidate();
            }
        }

        /// <summary>Gets or sets the header height</summary>
        [Browsable(true)]
        [Category("Layout")]
        [DefaultValue(60)]
        public int HeaderHeight
        {
            get => _headerHeight;
            set
            {
                _headerHeight = value;
                Height = value;
                Invalidate();
            }
        }

        /// <summary>Gets or sets whether to show the logo</summary>
        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(true)]
        public bool ShowLogo
        {
            get => _showLogo;
            set
            {
                _showLogo = value;
                Invalidate();
            }
        }

        /// <summary>Gets or sets whether to show the search box</summary>
        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(true)]
        public bool ShowSearchBox
        {
            get => _showSearchBox;
            set
            {
                _showSearchBox = value;
                Invalidate();
            }
        }

        /// <summary>Gets or sets the selected tab</summary>
        [Browsable(false)]
        public SimpleItem SelectedTab
        {
            get => _selectedTab;
            set
            {
                if (_selectedTab != value)
                {
                    _selectedTab = value;
                    TabSelected?.Invoke(this, new SelectedItemChangedEventArgs(_selectedTab));
                    Invalidate();
                }
            }
        }

        /// <summary>Gets or sets the selected tab index</summary>
        [Browsable(false)]
        public int SelectedTabIndex
        {
            get => _selectedTabIndex;
            set
            {
                if (value >= 0 && value < _tabs.Count)
                {
                    _selectedTabIndex = value;
                    SelectedTab = _tabs[value];
                    // After selection, animate underline to the selected rect
                    StartUnderlineAnimationToTab(value);
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>Add a tab</summary>
        public void AddTab(string text, string imagePath = "")
        {
            var tab = new SimpleItem { Text = text, ImagePath = imagePath };
            _tabs.Add(tab);
            Invalidate();
        }

        /// <summary>Add a tab with children (submenu)</summary>
        public void AddTab(string text, BindingList<SimpleItem> children, string imagePath = "")
        {
            var tab = new SimpleItem { Text = text, ImagePath = imagePath, Children = children };
            _tabs.Add(tab);
            Invalidate();
        }

        /// <summary>Remove a tab by index</summary>
        public void RemoveTabAt(int index)
        {
            if (index >= 0 && index < _tabs.Count)
            {
                _tabs.RemoveAt(index);
                Invalidate();
            }
        }

        /// <summary>Clear all tabs</summary>
        public void ClearTabs()
        {
            _tabs.Clear();
            _selectedTabIndex = -1;
            _selectedTab = null;
            Invalidate();
        }

        /// <summary>Add an action button</summary>
        public void AddActionButton(string text, string imagePath = "")
        {
            var btn = new SimpleItem { Text = text, ImagePath = imagePath };
            _buttons.Add(btn);
            Invalidate();
        }

        /// <summary>Remove an action button by index</summary>
        public void RemoveActionButtonAt(int index)
        {
            if (index >= 0 && index < _buttons.Count)
            {
                _buttons.RemoveAt(index);
                Invalidate();
            }
        }

        /// <summary>Clear all action buttons</summary>
        public void ClearActionButtons()
        {
            _buttons.Clear();
            Invalidate();
        }

        #endregion

        #region Painting

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            
            // Design-time: render simple placeholder to prevent flickering
            if (IsDesignModeSafe)
            {
                PaintDesignTimePlaceholder(e.Graphics);
                return;
            }

            if (_painter == null)
                _painter = CreatePainterForStyle(_headerStyle);
            
            // Ensure colors are initialized
            if (_colors == null)
                _colors = WebHeaderColors.FromTheme(_currentTheme);

            // Convert SimpleItem to WebHeaderTab for painter (adaptor pattern)
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
                IsTransparentBackground);

            // Draw sliding indicator overlay if enabled
            if (_indicatorStyle == TabIndicatorStyle.SlidingUnderline)
            {
                if (_currentUnderlineRect.IsEmpty && _selectedTabIndex >= 0)
                {
                    var tabsList2 = ConvertTabsForPainter();
                    var rectInit = _painter.GetTabBounds(_selectedTabIndex, new Rectangle(0, 0, Width, Height), tabsList2);
                    if (rectInit != Rectangle.Empty)
                    {
                        _currentUnderlineRect = new RectangleF(rectInit.Left, rectInit.Bottom - _indicatorThickness, rectInit.Width, _indicatorThickness);
                    }
                }

                if (!_currentUnderlineRect.IsEmpty)
                {
                    using (var pen = new Pen(_currentTheme?.ForeColor ?? Color.Black, _indicatorThickness))
                    using (var brush = new SolidBrush((_currentTheme?.ForeColor ?? Color.Black)))
                    {
                        var rect = Rectangle.Round(_currentUnderlineRect);
                        // draw filled underline
                        e.Graphics.FillRectangle(brush, rect);
                    }
                }
            }
        }
        
        /// <summary>
        /// Simple placeholder rendering for Visual Studio Designer
        /// </summary>
        private void PaintDesignTimePlaceholder(Graphics g)
        {
            // Background
            g.Clear(Color.FromArgb(255, 255, 255));
            
            // Bottom border line
            using (var pen = new Pen(Color.FromArgb(230, 230, 230), 1))
            {
                g.DrawLine(pen, 0, Height - 1, Width, Height - 1);
            }
            
            // Logo placeholder
            int logoX = 16;
            int logoY = (Height - 32) / 2;
            using (var brush = new SolidBrush(Color.FromArgb(66, 133, 244)))
            using (var font = new Font("Segoe UI", 11, FontStyle.Bold))
            {
                string logoDisplay = !string.IsNullOrEmpty(_logoText) ? _logoText : "Logo";
                g.DrawString(logoDisplay, font, brush, logoX, logoY + 6);
            }
            
            // Tab placeholders
            int tabX = 120;
            int tabCount = _tabs?.Count ?? 0;
            if (tabCount == 0) tabCount = 4; // Show sample tabs in design mode
            
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
            
            // Action button placeholders (right side)
            int btnX = Width - 120;
            int btnCount = _buttons?.Count ?? 0;
            if (btnCount == 0) btnCount = 2;
            
            using (var font = new Font("Segoe UI", 9))
            {
                // Sign In button (outline)
                var signInRect = new Rectangle(btnX - 80, (Height - 32) / 2, 70, 32);
                using (var pen = new Pen(Color.FromArgb(200, 200, 200), 1))
                using (var brush = new SolidBrush(Color.FromArgb(80, 80, 80)))
                {
                    g.DrawRectangle(pen, signInRect);
                    g.DrawString("Sign In", font, brush, signInRect.X + 12, signInRect.Y + 8);
                }
                
                // Get Started button (solid)
                var ctaRect = new Rectangle(btnX, (Height - 32) / 2, 90, 32);
                using (var fillBrush = new SolidBrush(Color.FromArgb(66, 133, 244)))
                using (var textBrush = new SolidBrush(Color.White))
                {
                    g.FillRectangle(fillBrush, ctaRect);
                    g.DrawString("Get Started", font, textBrush, ctaRect.X + 8, ctaRect.Y + 8);
                }
            }
            
            // Design mode label
            using (var font = new Font("Segoe UI", 8))
            using (var brush = new SolidBrush(Color.FromArgb(180, 180, 180)))
            {
                g.DrawString($"BeepWebHeaderAppBar (Design Mode) - Style: {_headerStyle}", font, brush, 4, Height - 16);
            }
        }

        private void StartUnderlineAnimationToTab(int tabIndex)
        {
            // Skip animation during design-time
            if (IsDesignModeSafe)
                return;
                
            if (_painter == null || tabIndex < 0 || tabIndex >= _tabs.Count)
                return;

            var tabsList = ConvertTabsForPainter();
            var rect = _painter.GetTabBounds(tabIndex, new Rectangle(0, 0, Width, Height), tabsList);
            if (rect == Rectangle.Empty)
                return;

            var target = new RectangleF(rect.Left, rect.Bottom - _indicatorThickness, rect.Width, _indicatorThickness);
            if (_currentUnderlineRect.IsEmpty)
            {
                _currentUnderlineRect = target;
            }
            _startUnderlineRect = _currentUnderlineRect;
            _targetUnderlineRect = target;
            _underlineAnimStartedAt = DateTime.Now;
            _isUnderlineAnimating = true;
            
            // Initialize timer if not already done
            if (_underlineTimer == null)
            {
                InitializeAnimationTimer();
            }
            _underlineTimer?.Start();
        }

        private static RectangleF Lerp(RectangleF a, RectangleF b, double t)
        {
            return new RectangleF(
                (float)(a.Left + (b.Left - a.Left) * t),
                (float)(a.Top + (b.Top - a.Top) * t),
                (float)(a.Width + (b.Width - a.Width) * t),
                (float)(a.Height + (b.Height - a.Height) * t)
            );
        }

        private List<WebHeaderTab> ConvertTabsForPainter()
        {
            var result = new List<WebHeaderTab>();
            for (int i = 0; i < _tabs.Count; i++)
            {
                var item = _tabs[i];
                var tab = new WebHeaderTab(item.Text, item.ImagePath, item.ID)
                {
                    IsActive = (i == _selectedTabIndex),
                    IsHovered = (_hoveredTabName == $"Tab_{i}"),
                    Tooltip = item.Description,
                    // Set HasChildren so painters can show dropdown indicator
                    HasChildren = item.Children != null && item.Children.Count > 0
                };
                result.Add(tab);
            }
            return result;
        }

        private List<WebHeaderActionButton> ConvertButtonsForPainter()
        {
            var result = new List<WebHeaderActionButton>();
            for (int i = 0; i < _buttons.Count; i++)
            {
                var item = _buttons[i];
                var btn = new WebHeaderActionButton(item.Text, item.ImagePath, WebHeaderButtonStyle.Solid, item.ID)
                {
                    IsHovered = (_hoveredButtonName == $"Button_{i}"),
                    // Set BadgeCount from the item's badge property (if available via Tag or custom property)
                    BadgeCount = GetBadgeCount(item)
                };
                result.Add(btn);
            }
            return result;
        }
        
        /// <summary>
        /// Gets the badge count for a button item.
        /// Override this method to provide custom badge count logic.
        /// </summary>
        protected virtual int GetBadgeCount(SimpleItem item)
        {
            // Check if Tag contains a badge count
            if (item.Tag is int count)
                return count;
            
            // Check if Tag is a dictionary with BadgeCount key
            if (item.Tag is Dictionary<string, object> dict && dict.TryGetValue("BadgeCount", out var value))
            {
                if (value is int badgeCount)
                    return badgeCount;
            }
            
            return 0;
        }

        #endregion

        #region Mouse Events

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            
            // Skip during design-time
            if (IsDesignModeSafe)
                return;

            if (_painter == null)
                return;

            var tabsList = ConvertTabsForPainter();
            var buttonsList = ConvertButtonsForPainter();

            int hit = _painter.GetHitElement(
                e.Location,
                new Rectangle(0, 0, Width, Height),
                tabsList,
                buttonsList);

            // Update hover states
            bool needsInvalidate = false;
            string previousTab = _hoveredTabName;
            string previousBtn = _hoveredButtonName;

            _hoveredTabName = null;
            _hoveredButtonName = null;

            // Check if hovering over tab
            if (hit >= 0 && hit < _tabs.Count)
            {
                _hoveredTabName = $"Tab_{hit}";
                Cursor = Cursors.Hand;
                if (previousTab != _hoveredTabName)
                    needsInvalidate = true;
            }
            // Check if hovering over button
            else if (hit < -1)
            {
                int btnIndex = -(hit + 1);
                if (btnIndex >= 0 && btnIndex < _buttons.Count)
                {
                    _hoveredButtonName = $"Button_{btnIndex}";
                    Cursor = Cursors.Hand;
                    if (previousBtn != _hoveredButtonName)
                        needsInvalidate = true;
                }
            }
            else
            {
                Cursor = Cursors.Default;
                if (previousTab != null || previousBtn != null)
                    needsInvalidate = true;
            }

            if (needsInvalidate)
                Invalidate();
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            
            // Skip during design-time
            if (IsDesignModeSafe)
                return;

            if (e.Button != MouseButtons.Left || _painter == null)
                return;

            var tabsList = ConvertTabsForPainter();
            var buttonsList = ConvertButtonsForPainter();

            int hit = _painter.GetHitElement(
                e.Location,
                new Rectangle(0, 0, Width, Height),
                tabsList,
                buttonsList);

            if (hit >= 0 && hit < _tabs.Count)
            {
                HandleTabClick(hit);
            }
            else if (hit < -1)
            {
                int btnIndex = -(hit + 1);
                if (btnIndex >= 0 && btnIndex < _buttons.Count)
                {
                    HandleActionButtonClick(btnIndex);
                }
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            
            // Skip during design-time
            if (IsDesignModeSafe)
                return;

            bool needsInvalidate = false;

            if (_hoveredTabName != null || _hoveredButtonName != null)
            {
                _hoveredTabName = null;
                _hoveredButtonName = null;
                Cursor = Cursors.Default;
                needsInvalidate = true;
            }

            if (needsInvalidate)
                Invalidate();
        }

        #endregion

        #region Tab/Button Handlers

        private void HandleTabClick(int tabIndex)
        {
            if (tabIndex < 0 || tabIndex >= _tabs.Count)
                return;

            var tab = _tabs[tabIndex];
            SelectedTabIndex = tabIndex;

            // If tab has children, show context menu
            if (tab.Children != null && tab.Children.Count > 0)
            {
                ShowTabMenu(tabIndex);
            }
            else if (!string.IsNullOrEmpty(tab.MethodName))
            {
                RunMethodFromGlobalFunctions(tab, tab.Text);
            }
        }

        private void HandleActionButtonClick(int btnIndex)
        {
            if (btnIndex < 0 || btnIndex >= _buttons.Count)
                return;

            var btn = _buttons[btnIndex];
            
            // If button has children, show popup menu (like BeepMenuBar does)
            if (btn.Children != null && btn.Children.Count > 0)
            {
                ShowButtonMenu(btnIndex);
            }
            else
            {
                // Fire event for buttons without children
                ActionButtonClicked?.Invoke(this, new SelectedItemChangedEventArgs(btn));

                if (!string.IsNullOrEmpty(btn.MethodName))
                {
                    RunMethodFromGlobalFunctions(btn, btn.Text);
                }
            }
        }
        
        /// <summary>
        /// Shows a popup menu for an action button with children
        /// </summary>
        private void ShowButtonMenu(int btnIndex)
        {
            if (btnIndex < 0 || btnIndex >= _buttons.Count)
                return;

            var btn = _buttons[btnIndex];
            if (btn.Children == null || btn.Children.Count == 0)
                return;

            // Get button bounds for positioning
            var buttonsList = ConvertButtonsForPainter();
            var rect = _painter.GetButtonBounds(btnIndex, new Rectangle(0, 0, Width, Height), buttonsList);

            if (rect != Rectangle.Empty)
            {
                // Position menu below the button
                var screenLocation = this.PointToScreen(new Point(rect.Left, rect.Bottom + 2));
                var selectedItem = base.ShowContextMenu(new List<SimpleItem>(btn.Children), screenLocation, multiSelect: false);

                if (selectedItem != null)
                {
                    // Fire event with selected child item
                    ActionButtonClicked?.Invoke(this, new SelectedItemChangedEventArgs(selectedItem));
                    
                    if (!string.IsNullOrEmpty(selectedItem.MethodName))
                    {
                        RunMethodFromGlobalFunctions(selectedItem, selectedItem.Text);
                    }
                }
            }
        }

        private void ShowTabMenu(int tabIndex)
        {
            if (tabIndex < 0 || tabIndex >= _tabs.Count)
                return;

            var tab = _tabs[tabIndex];
            if (tab.Children == null || tab.Children.Count == 0)
                return;

            // Get tab bounds for positioning
            var tabsList = ConvertTabsForPainter();
            var rect = _painter.GetTabBounds(tabIndex, new Rectangle(0, 0, Width, Height), tabsList);

            if (rect != Rectangle.Empty)
            {
                var screenLocation = this.PointToScreen(new Point(rect.Left, rect.Bottom + 2));
                var selectedItem = base.ShowContextMenu(new List<SimpleItem>(tab.Children), screenLocation, multiSelect: false);

                if (selectedItem != null)
                {
                    SelectedTab = selectedItem;
                    if (!string.IsNullOrEmpty(selectedItem.MethodName))
                    {
                        RunMethodFromGlobalFunctions(selectedItem, selectedItem.Text);
                    }
                }
            }
        }

        #endregion

        #region Helpers

        /// <summary>Create painter instance for WebHeaderStyle</summary>
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
        
        /// <summary>
        /// Applies the current theme to the control
        /// </summary>
        public override void ApplyTheme()
        {
            base.ApplyTheme();
            
            // Update colors from theme
            _colors = WebHeaderColors.FromTheme(_currentTheme);
            
            // Update fonts from theme if enabled
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
}
