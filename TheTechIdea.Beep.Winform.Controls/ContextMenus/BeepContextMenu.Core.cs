using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.ContextMenus.Helpers;
using TheTechIdea.Beep.Winform.Controls.Forms.ModernForm; // For FormStyle enum
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Styling;


namespace TheTechIdea.Beep.Winform.Controls.ContextMenus
{
    /// <summary>
    /// Core fields, properties, and initialization for BeepContextMenu
    /// Modern implementation using painter methodology
    /// </summary>
    public partial class BeepContextMenu 
    {
#pragma warning disable IL2026 // Suppress trimmer warnings for BindingList<T> used in WinForms data binding scenarios
        
        #region Helper and Painter
        
        /// <summary>
        /// The input helper that manages user interaction
        /// </summary>
        private BeepContextMenuInputHelper _inputHelper;
        
        /// <summary>
        /// The layout helper that computes item positions
        /// </summary>
        private BeepContextMenuLayoutHelper _layoutHelper;
        
        /// <summary>
        /// Gets the input helper instance for internal use
        /// </summary>
        internal BeepContextMenuInputHelper InputHelper => _inputHelper;
        
        /// <summary>
        /// Gets the layout helper instance for internal use
        /// </summary>
        internal BeepContextMenuLayoutHelper LayoutHelper => _layoutHelper;
        
        
        /// <summary>
        /// Returns the preferred item height
        /// </summary>
        public int PreferredItemHeight
        {
            get => _menuItemHeight;
        }



        #endregion

        #region Core Fields
        // Add near other fields (after _scrollOffset)
        private const int InternalPadding = 4; // Matches your 4px top/bottom padding in drawing
        // Visual Style
        private FormStyle _contextMenuType = FormStyle.Modern;
        private BeepControlStyle _controlStyle = BeepControlStyle.None;
        private bool _useThemeColors = true;
        private Forms.ModernForm.Painters.CornerRadius _cornerRadius = new Forms.ModernForm.Painters.CornerRadius(8);
        private Forms.ModernForm.Painters.ShadowEffect _shadowEffect = new Forms.ModernForm.Painters.ShadowEffect();
        
        // Menu items
        private BindingList<SimpleItem> _menuItems = new BindingList<SimpleItem>();
        // Full items (unfiltered) to support search
        private System.Collections.Generic.List<SimpleItem> _fullMenuItems = new System.Collections.Generic.List<SimpleItem>();
        private SimpleItem _selectedItem;
        private int _selectedIndex = -1;
        
        // Multi-select support
        private bool _multiSelect = false;
        private List<SimpleItem> _selectedItems = new List<SimpleItem>();
        
        // Visual options
        private bool _showCheckBox = false;
        private bool _showImage = true;
        private bool _showSeparators = true;
        private bool _showShortcuts = true;
        
        // Layout caching
        private int _menuItemHeight = 28;
        private int _imageSize = 20;
        private Rectangle _contentAreaRect;
        private int _menuWidth = 200;
        private int _minWidth = 150;
        private int _maxWidth = 400;
        
        // Search support
        private bool _showSearchBox = false;
        private string _searchText = string.Empty;
        private BeepTextBox _searchTextBox = null;

        // Scrolling support
        private int _maxHeight = 600; // Maximum height before scrolling
        private int _minHeight = 0; // Will be calculated as one item height + padding
        private Control _scrollBar; // VScrollBar or BeepScrollBar depending on availability
        private int _scrollOffset = 0;
        private bool _needsScrolling = false;
        private int _totalContentHeight = 0;
        private const int SCROLL_BAR_WIDTH = 17;
        
        // Visual state
        private SimpleItem _hoveredItem = null;
        private int _hoveredIndex = -1;
        
        // Font
        private Font _textFont = new Font("Segoe UI", 9f, FontStyle.Regular);
        private Font _shortcutFont = new Font("Segoe UI", 8f, FontStyle.Regular);
        
        // DPI scaling
        private float _scaleFactor = 1.0f;
        
    // MenuStyle (visual Style of the context menu painter)
    private FormStyle menustyle =  FormStyle.Modern;

    // Theme management (aligns with BaseControl pattern)
    private string _themeName = ThemeManagement.BeepThemesManager.CurrentThemeName;
    private IBeepTheme _currentTheme = ThemeManagement.BeepThemesManager.GetDefaultTheme();
    private bool _subscribedToThemeChanged = false;
        
        // Owner control
        private Control _owner;
        
        // Auto-close behavior
        private bool _closeOnItemClick = true;
        private bool _closeOnFocusLost = true;
        
        // Submenu support
        private BeepContextMenu _openSubmenu = null;
        private Timer _submenuTimer;
        private SimpleItem _submenuPendingItem;
        
        // Animation
        private Timer _fadeTimer;
        private double _opacity = 0;
        private const int FADE_STEPS = 10;
        private const int FADE_INTERVAL = 20;

    // Lifecycle behavior: by default, hide instead of disposing on Close
    private bool _destroyOnClose = false;

    // Fonts adoption toggle (follow theme fonts automatically)
    private bool _useThemeFonts = true;

    // WinForms-style owner window management (prevents taskbar entry)
    private static NativeWindow _dropDownOwnerWindow;
    
    // Close reason tracking (mirrors ToolStripDropDown)
    private BeepContextMenuCloseReason _closeReason = BeepContextMenuCloseReason.AppFocusChange;
    
    // Activation message handling flag
    private bool _sendingActivateMessage = false;
    
    // Message constants
    private const int WM_NCACTIVATE = 0x0086;
    private const int WM_ACTIVATE = 0x0006;
    private const int WM_MOUSEACTIVATE = 0x0021;
    private const int MA_NOACTIVATE = 3;
    private const int WA_ACTIVE = 1;
        
        #endregion
        
        #region Initialization
        
        public BeepContextMenu():base()
        {
            InitializeComponent();
            InitializeControl();
        }
        
        private void InitializeControl()
        {
            // Form setup
            AutoScaleMode = AutoScaleMode.Inherit;
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.Manual;
            ShowInTaskbar = false;
            TopMost = true;
            
            // CRITICAL: Set minimum size to prevent ArgumentException in WndProc
            // Context menus need valid dimensions before WM_SHOWWINDOW is processed
            MinimumSize = new Size(50, 20);
            Size = new Size(200, 100); // Default size until RecalculateSize is called
            
            // CRITICAL: Enable focus and mouse events
            TabStop = true;

            Padding = new Padding(1);
            
            // Double buffering for smooth rendering - CRITICAL to prevent flickering
            this.DoubleBuffered = true;
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw, true);
            UpdateStyles();

            // DPI awareness


            // Initialize helpers
            _inputHelper = new BeepContextMenuInputHelper(this);
            _layoutHelper = new BeepContextMenuLayoutHelper(this);
            
            // Set default painter
            
            SetPainter(BeepThemesManager.CurrentStyle);
            
            // Initialize submenu timer
            _submenuTimer = new Timer();
            _submenuTimer.Interval = 300; // 300ms delay before showing submenu (DevExpress-style)
            _submenuTimer.Tick += SubmenuTimer_Tick;
            
            // Initialize fade timer
            _fadeTimer = new Timer();
            _fadeTimer.Interval = FADE_INTERVAL;
            _fadeTimer.Tick += FadeTimer_Tick;
            
            // Initialize scrollbar - prefer BeepScrollBar if available
            try
            {
                var beepScrollType = Type.GetType("TheTechIdea.Beep.Winform.Controls.BeepScrollBar, TheTechIdea.Beep.Winform.Controls");
                if (beepScrollType != null)
                {
                    var beepScroll = (Control)Activator.CreateInstance(beepScrollType);
                    beepScroll.Dock = DockStyle.Right;
                    beepScroll.Width = SCROLL_BAR_WIDTH;
                    beepScroll.Visible = false;
                    // Wire generic event handler (ValueChanged) to our ScrollBar_Scroll
                    var eventInfo = beepScrollType.GetEvent("ValueChanged");
                    if (eventInfo != null)
                    {
                        eventInfo.AddEventHandler(beepScroll, new EventHandler((s, e) => InternalScrollBarValueChanged(s, e)));
                    }
                    // Try to set theme and some properties for consistent styling
                    try
                    {
                        var themeProp = beepScrollType.GetProperty("Theme");
                        if (themeProp != null) themeProp.SetValue(beepScroll, _themeName);
                        var applyThemeMethod = beepScrollType.GetMethod("ApplyTheme");
                        applyThemeMethod?.Invoke(beepScroll, null);
                    }
                    catch { }
                    _scrollBar = beepScroll;
                    this.Controls.Add(_scrollBar);
                }
                else
                {
                    var vscroll = new VScrollBar();
                    vscroll.Dock = DockStyle.Right;
                    vscroll.Width = SCROLL_BAR_WIDTH;
                    vscroll.Visible = false;
                    vscroll.Scroll += ScrollBar_Scroll;
                    vscroll.TabStop = false;
                    _scrollBar = vscroll;
                    this.Controls.Add(_scrollBar);
                }
            }
            catch
            {
                var vscroll = new VScrollBar();
                vscroll.Dock = DockStyle.Right;
                vscroll.Width = SCROLL_BAR_WIDTH;
                vscroll.Visible = false;
                vscroll.Scroll += ScrollBar_Scroll;
                vscroll.TabStop = false;
                _scrollBar = vscroll;
                this.Controls.Add(_scrollBar);
            }
            
            // Event handlers
            this.MouseMove += BeepContextMenu_MouseMove;
            this.MouseClick += BeepContextMenu_MouseClick;
            this.MouseLeave += BeepContextMenu_MouseLeave;
            this.Deactivate += BeepContextMenu_Deactivate;
            this.VisibleChanged += BeepContextMenu_VisibleChanged;
            this.MouseWheel += BeepContextMenu_MouseWheel;

            // Initialize theme based on BeepThemesManager
            try
            {
                _themeName = ThemeManagement.BeepThemesManager.CurrentThemeName;
                _currentTheme = ThemeManagement.BeepThemesManager.GetTheme(_themeName)
                                  ?? ThemeManagement.BeepThemesManager.GetDefaultTheme();
                // Adopt theme fonts initially
                ApplyThemeFontsSafely();
            }
            catch { /* best-effort */ }
        }

   

        public void SetPainter(FormStyle type)
        {
            // Map FormStyle to BeepControlStyle using BeepStyling
            _controlStyle = BeepStyling.GetControlStyle(type);
            Invalidate();
        }

        private void EnsureSearchTextBox()
        {
            if (!_showSearchBox)
            {
                if (_searchTextBox != null)
                {
                    try { _searchTextBox.TextChanged -= SearchTextBox_TextChanged; } catch { }
                    try { Controls.Remove(_searchTextBox); } catch { }
                    try { _searchTextBox.Dispose(); } catch { }
                    _searchTextBox = null;
                }
                return;
            }
            if (_searchTextBox != null) return;
            try
            {
                _searchTextBox = new BeepTextBox();
                _searchTextBox.BorderStyle = BorderStyle.None;
                _searchTextBox.AutoSize = false;
                // Slightly smaller height and compact style
                _searchTextBox.Height = 34;
                _searchTextBox.Width = Math.Max(100, _menuWidth - 24);
                _searchTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                _searchTextBox.TextChanged += SearchTextBox_TextChanged;
                _searchTextBox.KeyDown += SearchTextBox_KeyDown;
                // Visual polish: leading icon, trailing clear icon and placeholder
                try
                {
                    _searchTextBox.Theme = _themeName;
                    _searchTextBox.PlaceholderText = "Search...";
                    _searchTextBox.PlaceholderTextColor = _currentTheme?.TextBoxPlaceholderColor ?? _searchTextBox.PlaceholderTextColor;
                    _searchTextBox.LeadingIconPath = TheTechIdea.Beep.Icons.Svgs.Search;
                    _searchTextBox.TrailingIconPath = TheTechIdea.Beep.Icons.Svgs.Close;
                    _searchTextBox.ShowClearButton = true;
                    _searchTextBox.LeadingIconClickable = false;
                    _searchTextBox.TrailingIconClickable = true;
                    _searchTextBox.IsRounded = true;
                    _searchTextBox.BorderRadius = _currentTheme?.BorderRadius ?? _searchTextBox.BorderRadius;
                    // Apply theme and ensure consistent colors
                    _searchTextBox.ApplyTheme();
                    // Ensure trailing click clears the field
                    _searchTextBox.TrailingIconClicked += (s, ev) =>
                    {
                        try { _searchTextBox.Text = string.Empty; } catch { }
                    };
                }
                catch { }
                Controls.Add(_searchTextBox);
            }
            catch
            {
                var tb = new System.Windows.Forms.TextBox();
                tb.BorderStyle = BorderStyle.FixedSingle;
                tb.Height = 28;
                tb.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                try
                {
                    // Attempt to set colors to match theme (safe fallback)
                    tb.BackColor = _currentTheme?.TextBoxBackColor ?? tb.BackColor;
                    tb.ForeColor = _currentTheme?.TextBoxForeColor ?? tb.ForeColor;
                }
                catch { }
                tb.TextChanged += (s, e) => { _searchText = tb.Text; FilterMenuItems(); };
                Controls.Add(tb);
            }
        }

        private void SearchTextBox_TextChanged(object sender, EventArgs e)
        {
            if (_searchTextBox != null) _searchText = _searchTextBox.Text;
            else if (sender is System.Windows.Forms.TextBox tb) _searchText = tb.Text;
            FilterMenuItems();
        }

        private void FilterMenuItems()
        {
            if (_fullMenuItems == null || _fullMenuItems.Count == 0) _fullMenuItems = _menuItems.ToList();
            if (string.IsNullOrEmpty(_searchText))
            {
                _menuItems.Clear();
                foreach (var it in _fullMenuItems) _menuItems.Add(it);
            }
            else
            {
                var matches = _fullMenuItems.Where(it => (it.DisplayField ?? "").IndexOf(_searchText, StringComparison.OrdinalIgnoreCase) >= 0
                                                         || (it.SubText ?? "").IndexOf(_searchText, StringComparison.OrdinalIgnoreCase) >= 0
                                                         || (it.Description ?? "").IndexOf(_searchText, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
                _menuItems.Clear();
                foreach (var it in matches) _menuItems.Add(it);
            }
            RecalculateSize();
            Invalidate();
        }

        private void SearchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            // Support arrow navigation (up/down) while using the search textbox
            if (e.KeyCode == Keys.Up)
            {
                SelectPreviousItem();
                EnsureIndexVisible(_hoveredIndex);
                e.Handled = true;
                e.SuppressKeyPress = true;
                return;
            }
            if (e.KeyCode == Keys.Down)
            {
                SelectNextItem();
                EnsureIndexVisible(_hoveredIndex);
                e.Handled = true;
                e.SuppressKeyPress = true;
                return;
            }
            if (e.KeyCode == Keys.Escape)
            {
                // Close the menu
                _closeReason = BeepContextMenuCloseReason.AppFocusChange;
                Close();
                e.Handled = true;
                return;
            }
            if (e.KeyCode == Keys.Enter)
            {
                // Select the first enabled item in the current (filtered) list
                var candidate = _menuItems.FirstOrDefault(it => it != null && it.IsEnabled && !IsSeparator(it));
                if (candidate != null)
                {
                    SelectedItem = candidate;
                    OnItemClicked(candidate);
                    if (_closeOnItemClick) Close();
                }
                e.Handled = true;
            }
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            TrySubscribeThemeChanged(DesignMode);
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            // Unsubscribe to avoid leaks
            try
            {
                ThemeManagement.BeepThemesManager.ThemeChanged -= OnGlobalThemeChanged;
                _subscribedToThemeChanged = false;
            }
            catch { /* best-effort */ }
            base.OnHandleDestroyed(e);
        }

        // Subscribe to BeepThemesManager to auto-apply theme changes (mirrors BaseControl)
        private void TrySubscribeThemeChanged(bool isDesignMode)
        {
            if (_subscribedToThemeChanged || isDesignMode) return;
            try
            {
                ThemeManagement.BeepThemesManager.ThemeChanged -= OnGlobalThemeChanged;
                ThemeManagement.BeepThemesManager.ThemeChanged += OnGlobalThemeChanged;
                _subscribedToThemeChanged = true;
            }
            catch { /* best-effort */ }
        }

        private void OnGlobalThemeChanged(object? sender, EventArgs e)
        {
            if (IsDisposed) return;
            try
            {
                var newThemeName = ThemeManagement.BeepThemesManager.CurrentThemeName;
                _themeName = newThemeName;
                _currentTheme = ThemeManagement.BeepThemesManager.GetTheme(newThemeName)
                               ?? ThemeManagement.BeepThemesManager.GetDefaultTheme();
                ApplyThemeFontsSafely();
                try { if (_searchTextBox != null) { _searchTextBox.Theme = _themeName; _searchTextBox.ApplyTheme(); _searchTextBox.BorderRadius = _currentTheme?.BorderRadius ?? _searchTextBox.BorderRadius; }} catch { }
                Invalidate();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"BeepContextMenu: OnGlobalThemeChanged error: {ex.Message}");
            }
        }

        /// <summary>
        /// Apply theme fonts to TextFont and ShortcutFont when enabled. Safe fallbacks retained.
        /// </summary>
        private void ApplyThemeFontsSafely()
        {
            if (!_useThemeFonts) return;
            try
            {
                var theme = _currentTheme;
                if (theme != null && theme.LabelFont != null && theme.LabelFont.FontFamily != null)
                {
                    try {
                        _textFont?.Dispose();
                        _textFont = new Font(theme.LabelFont.FontFamily, theme.LabelFont.FontSize, theme.LabelFont.FontStyle);
                    } catch {
                        _textFont = new Font("Segoe UI", 9f, FontStyle.Regular);
                    }
                } else {
                    _textFont = new Font("Segoe UI", 9f, FontStyle.Regular);
                }
                // Shortcut font: slightly smaller than text font
                if (_textFont != null && _textFont.FontFamily != null)
                {
                    float size = Math.Max(6f, _textFont.Size - 1f);
                    try {
                        _shortcutFont?.Dispose();
                        _shortcutFont = new Font(_textFont.FontFamily, size, _textFont.Style);
                    } catch {
                        _shortcutFont = new Font("Segoe UI", 8f, FontStyle.Regular);
                    }
                }
            }
            catch { /* keep existing fonts on any error */ }
        }
        
        #endregion
        
        #region IDisposable Support
        
        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        try { _submenuTimer?.Stop(); _submenuTimer?.Dispose(); } catch { }
        //        try { _fadeTimer?.Stop(); _fadeTimer?.Dispose(); } catch { }
        //        try { _openSubmenu?.Dispose(); } catch { }
        //        try { if (_scrollBar != null) { if (_scrollBar is VScrollBar v) v.Scroll -= ScrollBar_Scroll; else { var ev = _scrollBar.GetType().GetEvent("ValueChanged"); ev?.RemoveEventHandler(_scrollBar, new EventHandler((s, e) => InternalScrollBarValueChanged(s, e))); } } } catch { }
        //        try { if (_searchTextBox != null) { _searchTextBox.TextChanged -= SearchTextBox_TextChanged; Controls.Remove(_searchTextBox); _searchTextBox.Dispose(); _searchTextBox = null; } } catch { }
        //    }
        //    base.Dispose(disposing);
        //}
        
        #endregion
        
#pragma warning restore IL2026
    }
}
