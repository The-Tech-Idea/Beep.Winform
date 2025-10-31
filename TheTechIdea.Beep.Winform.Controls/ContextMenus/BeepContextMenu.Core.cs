using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.ContextMenus.Helpers;
using TheTechIdea.Beep.Winform.Controls.Forms.ModernForm;
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

        // Visual Style
        private FormStyle _contextMenuType = FormStyle.Modern;
        private BeepControlStyle _controlStyle = BeepControlStyle.None;
        private bool _useThemeColors = true;
        
        // Menu items
        private BindingList<SimpleItem> _menuItems = new BindingList<SimpleItem>();
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
        
        // Scrolling support
        private int _maxHeight = 600; // Maximum height before scrolling
        private int _minHeight = 0; // Will be calculated as one item height + padding
        private VScrollBar _scrollBar;
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
            BackColor = Color.White;
          

            Padding = new Padding(1);
            //Double buffering for smooth rendering

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
            _submenuTimer.Interval = 400; // 400ms delay before showing submenu
            _submenuTimer.Tick += SubmenuTimer_Tick;
            
            // Initialize fade timer
            _fadeTimer = new Timer();
            _fadeTimer.Interval = FADE_INTERVAL;
            _fadeTimer.Tick += FadeTimer_Tick;
            
            // Initialize scrollbar
            _scrollBar = new VScrollBar();
            _scrollBar.Dock = DockStyle.Right;
            _scrollBar.Width = SCROLL_BAR_WIDTH;
            _scrollBar.Visible = false;
            _scrollBar.Scroll += ScrollBar_Scroll;
            _scrollBar.TabStop = false;
            this.Controls.Add(_scrollBar);
            
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
        //        _submenuTimer?.Stop();
        //        _submenuTimer?.Dispose();
        //        _fadeTimer?.Stop();
        //        _fadeTimer?.Dispose();
        //        _openSubmenu?.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}
        
        #endregion
        
#pragma warning restore IL2026
    }
}
