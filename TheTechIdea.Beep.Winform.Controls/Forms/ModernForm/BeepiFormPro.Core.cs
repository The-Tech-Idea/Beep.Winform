using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Converters;
using TheTechIdea.Beep.Winform.Controls.Forms.ModernForm.Painters;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Forms.ModernForm
{
    public partial class BeepiFormPro
    {
        private bool InDesignModeSafe =>
           LicenseManager.UsageMode == LicenseUsageMode.Designtime ||
           (Site?.DesignMode ?? false);
        // Theme properties
        private string _theme = "DefaultTheme";
        [Browsable(true)]
        [TypeConverter(typeof(ThemeConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string Theme
        {
            get => _theme;
            set
            {
                if (value != _theme)
                    _theme = value;
               _currentTheme = BeepThemesManager.GetTheme(value);
                ApplyTheme();
            }
        }
        private Color _bordercolor= Color.LightGray;
        public Color BorderColor
        {
            get => _bordercolor;
            set
            {
                if (_bordercolor != value)
                {
                    _bordercolor = value;
                    Invalidate();
                }
            }
        }

        // Style properties
        private FormStyle _formstyle = FormStyle.Modern;
        public FormStyle FormStyle
        {
            get => _formstyle;
            set
            {
                if (_formstyle != value)
                {
                    _formstyle = value;
                    ApplyFormStyle();
                    if (!DesignMode) RecalculateLayoutAndHitAreas();
                    DebouncedInvalidate();
                }
            }
        }

        // UseThemeColors property - when true, metrics use theme colors; when false, use default skin colors
        private bool _useThemeColors = true;
        [System.ComponentModel.Category("Beep Theme")]
        [System.ComponentModel.DefaultValue(true)]
        [System.ComponentModel.Description("When true, uses theme colors from the current theme. When false, uses default skin-specific colors.")]
        public bool UseThemeColors
        {
            get => _useThemeColors;
            set
            {
                if (_useThemeColors != value)
                {
                    _useThemeColors = value;
                    // Reset metrics to force recalculation with new color mode
                    _formpaintermaterics = null;
                    Invalidate();
                }
            }
        }
        
        // Painters - Hidden from designer to prevent serialization
        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public IFormPainter ActivePainter { get; set; }
        
        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public List<IFormPainter> Painters { get; } = new();

        // Regions API
        private readonly List<FormRegion> _regions = new();

        // Managers
        internal BeepiFormProLayoutManager _layout;
        internal BeepiFormProHitAreaManager _hits;
        internal BeepiFormProInteractionManager _interact;

        // Current layout info calculated by the active painter - Hidden from designer
        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public PainterLayoutInfo CurrentLayout { get;  set; } = new();

        // Built-in caption elements
        public FormRegion _iconRegion;
        private FormRegion _titleRegion;
        private FormRegion _minimizeButton;
        private FormRegion _maximizeButton;
        private FormRegion _closeButton;
        public FormRegion _customActionButton; // New: custom clickable region
        public FormRegion _themeButton;
        public FormRegion _styleButton;
        private FormRegion _profileButton;
        private FormRegion _searchBox;

        // Internal properties for manager access
        internal FormRegion IconRegion => _iconRegion;
        internal FormRegion TitleRegion => _titleRegion;
        internal FormRegion MinimizeButton => _minimizeButton;
        internal FormRegion MaximizeButton => _maximizeButton;
        internal FormRegion CloseButton => _closeButton;
        internal FormRegion CustomActionButton => _customActionButton;
        internal FormRegion ThemeButton => _themeButton;
        internal FormRegion StyleButton => _styleButton;
        internal FormRegion ProfileButton => _profileButton;
        internal FormRegion SearchBox => _searchBox;
        internal List<FormRegion> Regions => _regions;

        // Button visibility flags
        private bool _showThemeButton = false;
        private bool _showStyleButton = false;
        private bool _showProfileButton = false;
        private bool _showSearchBox = false;
        private bool _showCloseButton = true;
        private bool _showMinMaxButtons = true;

        // Events for region interaction
        public event EventHandler<RegionEventArgs> RegionHover;
        public event EventHandler<RegionEventArgs> RegionClick;
        
        // Events for button actions
        public event EventHandler ThemeButtonClicked;
        public event EventHandler StyleButtonClicked;
        public event EventHandler ProfileButtonClicked;
        public event EventHandler<string> SearchBoxTextChanged;

          public event EventHandler OnFormClose;
        public event EventHandler OnFormLoad;
        public event EventHandler OnFormShown;
        public event EventHandler<FormClosingEventArgs> PreClose;

        // DPI scaling factor
        private float _dpiScale = 1.0f;

        // Caption bar properties
        private bool _showCaptionBar = true;
        private int _captionHeight = 32;

        // Modern effect properties
        private ShadowEffect _shadowEffect = new ShadowEffect();
        private CornerRadius _cornerRadius = new CornerRadius(8);
        private AntiAliasMode _antiAliasMode = AntiAliasMode.High;
        private bool _enableAnimations = true;
        private int _animationDuration = 200;

        /// <summary>
        /// Gets or sets whether to show the caption bar
        /// </summary>
        [System.ComponentModel.Category("Beep Caption")]
        [System.ComponentModel.DefaultValue(true)]
        public bool ShowCaptionBar
        {
            get => _showCaptionBar;
            set
            {
                if (_showCaptionBar != value)
                {
                    _showCaptionBar = value;
                    Invalidate();
                    PerformLayout();
                    if (!DesignMode) RecalculateLayoutAndHitAreas();
                }
            }
        }

        /// <summary>
        /// Gets or sets the height of the caption bar
        /// </summary>
        [System.ComponentModel.Category("Beep Caption")]
        [System.ComponentModel.DefaultValue(32)]
        public int CaptionHeight
        {
            get => _captionHeight;
            set
            {
                if (_captionHeight != value)
                {
                    _captionHeight = value;
                    Invalidate();
                    PerformLayout();
                    if (!DesignMode) RecalculateLayoutAndHitAreas();
                }
            }
        }

        /// <summary>
        /// Gets or sets the shadow effect for the form
        /// </summary>
        [System.ComponentModel.Category("Beep Effects")]
        [System.ComponentModel.Description("Shadow effect configuration")]
        public ShadowEffect ShadowEffect
        {
            get => _shadowEffect ?? (_shadowEffect = new ShadowEffect());
            set
            {
                _shadowEffect = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the corner radius for the form
        /// </summary>
        [System.ComponentModel.Category("Beep Effects")]
        [System.ComponentModel.Description("Corner radius configuration")]
        public CornerRadius CornerRadius
        {
            get => _cornerRadius ?? (_cornerRadius = new CornerRadius(8));
            set
            {
                _cornerRadius = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the anti-aliasing mode
        /// </summary>
        [System.ComponentModel.Category("Beep Effects")]
        [System.ComponentModel.DefaultValue(AntiAliasMode.High)]
        [System.ComponentModel.Description("Anti-aliasing quality mode")]
        public AntiAliasMode AntiAliasMode
        {
            get => _antiAliasMode;
            set
            {
                _antiAliasMode = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the rendering quality preset
        /// </summary>
        [System.ComponentModel.Category("Beep Effects")]
        [System.ComponentModel.DefaultValue(RenderingQuality.Auto)]
        [System.ComponentModel.Description("Rendering quality preset for performance optimization")]
        public RenderingQuality RenderingQuality { get; set; } = RenderingQuality.Auto;

        /// <summary>
        /// Gets or sets whether animations are enabled
        /// </summary>
        [System.ComponentModel.Category("Beep Animation")]
        [System.ComponentModel.DefaultValue(true)]
        [System.ComponentModel.Description("Enable smooth animations")]
        public bool EnableAnimations
        {
            get => _enableAnimations;
            set => _enableAnimations = value;
        }

        /// <summary>
        /// Gets or sets whether to paint decorative effects over the entire form including content area.
        /// When true, effects like blur, gradients, and overlays are painted over controls.
        /// Controls remain interactive as mouse events are not intercepted.
        /// </summary>
        [System.ComponentModel.Category("Beep Effects")]
        [System.ComponentModel.DefaultValue(false)]
        [System.ComponentModel.Description("Paint decorative effects over entire form including content area. Controls remain interactive.")]
        public bool PaintOverContentArea { get; set; } = false;

        /// <summary>
        /// Gets or sets the animation duration in milliseconds
        /// </summary>
        [System.ComponentModel.Category("Beep Animation")]
        [System.ComponentModel.DefaultValue(200)]
        [System.ComponentModel.Description("Animation duration in milliseconds")]
        public int AnimationDuration
        {
            get => _animationDuration;
            set => _animationDuration = Math.Max(0, value);
        }

        // Advanced modern properties for the best modern form experience

        /// <summary>
        /// Gets or sets the backdrop effect mode (Mica, Acrylic, etc.)
        /// </summary>
        [System.ComponentModel.Category("Beep Effects")]
        [System.ComponentModel.DefaultValue(BackdropEffect.None)]
        [System.ComponentModel.Description("Advanced backdrop effect for modern appearance")]
        public BackdropEffect BackdropEffect { get; set; } = BackdropEffect.None;

        /// <summary>
        /// Gets or sets whether high contrast mode is enabled for accessibility
        /// </summary>
        [System.ComponentModel.Category("Beep Accessibility")]
        [System.ComponentModel.DefaultValue(false)]
        [System.ComponentModel.Description("Enable high contrast mode for better accessibility")]
        public bool HighContrastMode { get; set; } = false;

        /// <summary>
        /// Gets or sets whether screen reader support is enabled
        /// </summary>
        [System.ComponentModel.Category("Beep Accessibility")]
        [System.ComponentModel.DefaultValue(true)]
        [System.ComponentModel.Description("Enable screen reader support for accessibility")]
        public bool ScreenReaderSupport { get; set; } = true;

        /// <summary>
        /// Gets or sets the focus indicator Style for keyboard navigation
        /// </summary>
        [System.ComponentModel.Category("Beep Accessibility")]
        [System.ComponentModel.DefaultValue(FocusIndicatorStyle.Subtle)]
        [System.ComponentModel.Description("Style of focus indicators for keyboard navigation")]
        public FocusIndicatorStyle FocusIndicatorStyle { get; set; } = FocusIndicatorStyle.Subtle;

        /// <summary>
        /// Gets or sets whether micro-interactions are enabled
        /// </summary>
        [System.ComponentModel.Category("Beep Animation")]
        [System.ComponentModel.DefaultValue(true)]
        [System.ComponentModel.Description("Enable subtle micro-interactions and hover effects")]
        public bool EnableMicroInteractions { get; set; } = true;

        /// <summary>
        /// Gets or sets the adaptive layout mode for responsive design
        /// </summary>
        [System.ComponentModel.Category("Beep Layout")]
        [System.ComponentModel.DefaultValue(AdaptiveLayoutMode.Auto)]
        [System.ComponentModel.Description("Adaptive layout behavior for different screen sizes")]
        public AdaptiveLayoutMode AdaptiveLayoutMode { get; set; } = AdaptiveLayoutMode.Auto;

        /// <summary>
        /// Gets or sets whether smart invalidation is enabled for performance
        /// </summary>
        [System.ComponentModel.Category("Beep Performance")]
        [System.ComponentModel.DefaultValue(true)]
        [System.ComponentModel.Description("Enable smart invalidation to improve rendering performance")]
        public bool EnableSmartInvalidation { get; set; } = true;

        private bool _showModernRenderingInDesignMode = false;

       

        /// <summary>
        /// Gets or sets the hover animation duration for micro-interactions
        /// </summary>
        [System.ComponentModel.Category("Beep Animation")]
        [System.ComponentModel.DefaultValue(150)]
        [System.ComponentModel.Description("Duration of hover animations in milliseconds")]
        public int HoverAnimationDuration { get; set; } = 150;

        /// <summary>
        /// Gets or sets the focus transition duration
        /// </summary>
        [System.ComponentModel.Category("Beep Animation")]
        [System.ComponentModel.DefaultValue(200)]
        [System.ComponentModel.Description("Duration of focus transitions in milliseconds")]
        public int FocusTransitionDuration { get; set; } = 200;

        /// <summary>
        /// Gets or sets whether smooth window resizing is enabled
        /// </summary>
        [System.ComponentModel.Category("Beep Animation")]
        [System.ComponentModel.DefaultValue(true)]
        [System.ComponentModel.Description("Enable smooth animations during window resize")]
        public bool EnableSmoothResize { get; set; } = true;

        /// <summary>
        /// Gets or sets the window state transition duration
        /// </summary>
        [System.ComponentModel.Category("Beep Animation")]
        [System.ComponentModel.DefaultValue(300)]
        [System.ComponentModel.Description("Duration of minimize/maximize/restore transitions")]
        public int WindowStateTransitionDuration { get; set; } = 300;

        /// <summary>
        /// Gets or sets whether to show the theme button in the caption bar
        /// </summary>
        [System.ComponentModel.Category("Beep Caption")]
        [System.ComponentModel.DefaultValue(false)]
        public bool ShowThemeButton
        {
            get => _showThemeButton;
            set
            {
                if (_showThemeButton != value)
                {
                    _showThemeButton = value;
                  
                    if (!DesignMode) RecalculateLayoutAndHitAreas();
                }
            }
        }

        /// <summary>
        /// Gets or sets whether to show the Style button in the caption bar
        /// </summary>
        [System.ComponentModel.Category("Beep Caption")]
        [System.ComponentModel.DefaultValue(false)]
        public bool ShowStyleButton
        {
            get => _showStyleButton;
            set
            {
                if (_showStyleButton != value)
                {
                    _showStyleButton = value;
                   
                    if (!DesignMode) RecalculateLayoutAndHitAreas();
                }
            }
        }

        /// <summary>
        /// Gets or sets whether to show the profile button in the caption bar
        /// </summary>
        [System.ComponentModel.Category("Beep Caption")]
        [System.ComponentModel.DefaultValue(false)]
        [System.ComponentModel.Description("Show profile/user button in caption bar")]
        public bool ShowProfileButton
        {
            get => _showProfileButton;
            set
            {
                if (_showProfileButton != value)
                {
                    _showProfileButton = value;
                  
                    if (!DesignMode) RecalculateLayoutAndHitAreas();
                }
            }
        }

        /// <summary>
        /// Gets or sets whether to show the search box in the caption bar
        /// </summary>
        [System.ComponentModel.Category("Beep Caption")]
        [System.ComponentModel.DefaultValue(false)]
        [System.ComponentModel.Description("Show search box in caption bar")]
        public bool ShowSearchBox
        {
            get => _showSearchBox;
            set
            {
                if (_showSearchBox != value)
                {
                    _showSearchBox = value;
                  
                    if (!DesignMode) RecalculateLayoutAndHitAreas();
                }
            }
        }

        /// <summary>
        /// Gets or sets whether to show the close button in the caption bar
        /// </summary>
        [System.ComponentModel.Category("Beep Caption")]
        [System.ComponentModel.DefaultValue(true)]
        [System.ComponentModel.Description("Show close button in caption bar")]
        public bool ShowCloseButton
        {
            get => _showCloseButton;
            set
            {
                if (_showCloseButton != value)
                {
                    _showCloseButton = value;
                 
                    if (!DesignMode) RecalculateLayoutAndHitAreas();
                }
            }
        }

        /// <summary>
        /// Gets or sets whether to show minimize and maximize buttons in the caption bar
        /// </summary>
        [System.ComponentModel.Category("Beep Caption")]
        [System.ComponentModel.DefaultValue(true)]
        [System.ComponentModel.Description("Show minimize and maximize buttons in caption bar")]
        public bool ShowMinMaxButtons
        {
            get => _showMinMaxButtons;
            set
            {
                if (_showMinMaxButtons != value)
                {
                    _showMinMaxButtons = value;
                   
                    if (!DesignMode) RecalculateLayoutAndHitAreas();
                }
            }
        }

        // Public API to register regions
        public void AddRegion(FormRegion region)
        {
            if (region == null) return;
            _regions.Add(region);
            Invalidate();
        }

        public void ClearRegions()
        {
            _regions.Clear();
            Invalidate();
        }

     

        private void InitializeBuiltInRegions()
        {
            // Icon region (left side of caption)
            FormPainterMetrics pnt = FormPainterMetrics.DefaultFor(FormStyle, UseThemeColors ? _currentTheme : null);
            _iconRegion = new FormRegion
            {
                Id = "system:icon",
                Dock = RegionDock.Caption,
                OnPaint = (g, r) =>
                {
                    if (Icon != null && r.Width > 0 && r.Height > 0)
                    {
                        int size = Math.Min(r.Width, r.Height) - 4;
                        var iconRect = new Rectangle(r.Left + 2, r.Top + (r.Height - size) / 2, size, size);
                        g.DrawIcon(Icon, iconRect);
                    }
                }
            };

            // Title region (center of caption)
            _titleRegion = new FormRegion
            {
                Id = "system:title",
                Dock = RegionDock.Caption,
                OnPaint = (g, r) =>
                {
                    if (string.IsNullOrEmpty(Text) || r.Width <= 0 || r.Height <= 0) return;

                    var fg = pnt.ForegroundColor;
                    TextRenderer.DrawText(g, Text, Font, r, fg,
                        TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
                }
            };

            // System buttons for Modern/Minimal form styles
            int btnSize = 32;
            _minimizeButton = new FormRegion
            {
                Id = "system:minimize",
                Dock = RegionDock.Caption,
                OnPaint = (g, r) =>
                {
                    if (!_showMinMaxButtons || r.Width <= 0 || r.Height <= 0) return;
                    bool isHover = _interact?.IsHovered(_hits?.GetHitArea("minimize")) ?? false;
                    FormPainterRenderHelper.DrawSystemButton(
                        g,
                        r,
                        "−",
                        isHover,
                        false,
                        Font,
                        pnt.ForegroundColor,
                        pnt.CaptionButtonHoverColor);
                }
            };

            _maximizeButton = new FormRegion
            {
                Id = "system:maximize",
                Dock = RegionDock.Caption,
                OnPaint = (g, r) =>
                {
                    if (!_showMinMaxButtons || r.Width <= 0 || r.Height <= 0) return;
                    bool isHover = _interact?.IsHovered(_hits?.GetHitArea("maximize")) ?? false;
                    string symbol = WindowState == FormWindowState.Maximized ? "❐" : "□";
                    FormPainterRenderHelper.DrawSystemButton(
                        g,
                        r,
                        symbol,
                        isHover,
                        false,
                        Font,
                        pnt.ForegroundColor,
                        pnt.CaptionButtonHoverColor);
                }
            };

            _closeButton = new FormRegion
            {
                Id = "system:close",
                Dock = RegionDock.Caption,
                OnPaint = (g, r) =>
                {
                    if (!_showCloseButton || r.Width <= 0 || r.Height <= 0) return;
                    bool isHover = _interact?.IsHovered(_hits?.GetHitArea("close")) ?? false;
                    FormPainterRenderHelper.DrawSystemButton(
                        g,
                        r,
                        "✕",
                        isHover,
                        true,
                        Font,
                        pnt.ForegroundColor,
                        pnt.CaptionButtonHoverColor,
                        Color.FromArgb(232, 17, 35));
                }
            };

            // Custom action button (between title and system buttons)
            _customActionButton = new FormRegion
            {
                Id = "custom:action",
                Dock = RegionDock.Caption,
                OnPaint = (g, r) =>
                {
                    if (r.Width <= 0 || r.Height <= 0) return;
                   
                     var isHovered = _interact?.IsHovered(_hits?.GetHitArea("customAction")) ?? false;
                    var isPressed = _interact?.IsPressed(_hits?.GetHitArea("customAction")) ?? false;
                    
                    // Hover/press indicator: circle outline around the icon
                    var hoverColor = isPressed ? pnt.CaptionButtonPressedColor : pnt.CaptionButtonHoverColor;
                    if (isHovered || isPressed)
                        FormPainterRenderHelper.DrawHoverOutlineCircle(g, r, hoverColor, isPressed ? 3 : 2, 6);

                    // Draw icon (⚙ gear/settings icon)
                    var fg = pnt.ForegroundColor;
                    using var font = new Font("Segoe UI Symbol", Font.Size + 2, FontStyle.Regular);
                    TextRenderer.DrawText(g, "⚙", font, r, fg,
                        TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
                }
            };

            // MenuStyle button (palette icon)
            _themeButton = new FormRegion
            {
                Id = "system:theme",
                Dock = RegionDock.Caption,
                OnPaint = (g, r) =>
                {
                    if (r.Width <= 0 || r.Height <= 0 || !_showThemeButton) return;
                   
                    var isHovered = _interact?.IsHovered(_hits?.GetHitArea("theme")) ?? false;
                    var isPressed = _interact?.IsPressed(_hits?.GetHitArea("theme")) ?? false;
                    
                    // Hover/press indicator: circle outline
                    var hoverColor = isPressed ? pnt.CaptionButtonPressedColor : pnt.CaptionButtonHoverColor;
                    if (isHovered || isPressed)
                        FormPainterRenderHelper.DrawHoverOutlineCircle(g, r, hoverColor, isPressed ? 3 : 2, 6);

                    // Draw icon (🎨 palette icon)
                    var fg = pnt.ForegroundColor;
                    using var font = new Font("Segoe UI Emoji", Font.Size, FontStyle.Regular);
                    TextRenderer.DrawText(g, "🎨", font, r, fg,
                        TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
                }
            };

            // Style button (layout icon)
            _styleButton = new FormRegion
            {
                Id = "system:Style",
                Dock = RegionDock.Caption,
                OnPaint = (g, r) =>
                {
                    if (r.Width <= 0 || r.Height <= 0 || !_showStyleButton) return;
                   
                    var isHovered = _interact?.IsHovered(_hits?.GetHitArea("Style")) ?? false;
                    var isPressed = _interact?.IsPressed(_hits?.GetHitArea("Style")) ?? false;
                    
                    // Hover/press indicator: circle outline
                    var hoverColor = isPressed ? pnt.CaptionButtonPressedColor : pnt.CaptionButtonHoverColor;
                    if (isHovered || isPressed)
                        FormPainterRenderHelper.DrawHoverOutlineCircle(g, r, hoverColor, isPressed ? 3 : 2, 6);

                    // Draw icon (◧ layout/Style icon)
                    var fg = pnt.ForegroundColor;
                    using var font = new Font("Segoe UI Symbol", Font.Size + 2, FontStyle.Regular);
                    TextRenderer.DrawText(g, "◧", font, r, fg,
                        TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
                }
            };
        }

        protected void OnRegionClicked(HitArea area)
        {
            if (area?.Name == null) return;

            // Normalize region names so painters can register simple keys like "close"/"minimize"/"title"
            // but actions still work with our canonical "region:*" keys.
            string key = area.Name;
            switch (key)
            {
                // Accept simple keys
                case "close": key = "region:system:close"; break;
                case "maximize": key = "region:system:maximize"; break;
                case "minimize": key = "region:system:minimize"; break;
                case "theme": key = "region:system:theme"; break;
                case "Style": key = "region:system:Style"; break;
                case "customAction": key = "region:custom:action"; break;
                case "title": key = "caption"; break; // treat title hit as caption drag

                // Accept semi-prefixed keys (without leading "region:")
                case "system:close": key = "region:system:close"; break;
                case "system:maximize": key = "region:system:maximize"; break;
                case "system:minimize": key = "region:system:minimize"; break;
                case "system:theme": key = "region:system:theme"; break;
                case "system:Style": key = "region:system:Style"; break;
                case "custom:action": key = "region:custom:action"; break;
            }

            // Raise event for extensibility with the original name for consumers
            var regionData = area.Data as FormRegion;
            RegionClick?.Invoke(this, new RegionEventArgs(area.Name, regionData, area.Bounds));

            switch (key)
            {
                case "region:system:minimize":
                    WindowState = FormWindowState.Minimized;
                    break;

                case "region:system:maximize":
                    WindowState = WindowState == FormWindowState.Maximized
                        ? FormWindowState.Normal
                        : FormWindowState.Maximized;
                    break;

                case "region:system:close":
                    // Defer close to avoid reentrancy during mouse event processing
                    BeginInvoke(new Action(() => Close()));
                    break;

                case "region:custom:action":
                    // Custom action button clicked - override or subscribe to event
                    OnCustomActionClicked();
                    break;

                case "region:system:theme":
                    // MenuStyle button clicked
                    ThemeButtonClicked?.Invoke(this, EventArgs.Empty);
                    ShowThemeMenu();
                    break;

                case "region:system:Style":
                    // Style button clicked
                    StyleButtonClicked?.Invoke(this, EventArgs.Empty);
                    ShowFormStyleMenu();
                    break;

                case "caption":
                    // Allow window dragging
                    if (WindowState == FormWindowState.Normal)
                    {
                        ReleaseCapture();
                        SendMessage(Handle, 0xA1, 0x2, 0);
                    }
                    break;
            }
        }
        private void ShowFormStyleMenu()
        {
            var menu = new ContextMenuStrip();
            try
            {
                foreach (FormStyle style in Enum.GetValues(typeof(FormStyle)).Cast<FormStyle>())
                {
                    var item = new ToolStripMenuItem(style.ToString());
                    item.Click += (s, e) =>
                    {
                        // Defer theme change to avoid repaints during menu interaction
                        //BeginInvoke(new Action(() =>
                        //{
                        try
                        {

                            FormStyle = style;
                          BeepThemesManager.SetCurrentStyle(style);
                        }
                        catch { }

                    };
                    menu.Items.Add(item);
                }
            }
            catch { }
            // Show menu below the Style button using the current layout rectangle
            var styleRect = CurrentLayout.StyleButtonRect;
            Point pt;
            if (styleRect.Width > 0 && styleRect.Height > 0)
            {
                pt = PointToScreen(new Point(styleRect.Left, styleRect.Bottom));
            }
            else
            {
                // Fallback: show at cursor to avoid (0,0) when rect isn't available
                pt = Cursor.Position;
            }
            menu.Show(pt);
        }   
        private void ShowThemeMenu()
        {
            var menu = new ContextMenuStrip();
            try
            {
                foreach (string theme in BeepThemesManager.GetThemeNames())
                {
                    var item = new ToolStripMenuItem(theme);
                    item.Click += (s, e) =>
                    {
                         // Defer theme change to avoid repaints during menu interaction
                         BeginInvoke(new Action(() =>
                          {
                            try
                          {
                                BeepThemesManager.SetCurrentTheme(theme);
                               //  Theme = theme; // This will handle invalidation smartly
                          }
                             catch { }
                            }));
                    };
                    menu.Items.Add(item);
                }
            }
            catch { }
            // Show menu below the theme button using the current layout rectangle
            var themeRect = CurrentLayout.ThemeButtonRect;
            Point pt;
            if (themeRect.Width > 0 && themeRect.Height > 0)
            {
                pt = PointToScreen(new Point(themeRect.Left, themeRect.Bottom));
            }
            else
            {
                // Fallback: show at cursor to avoid (0,0) when rect isn't available
                pt = Cursor.Position;
            }
            menu.Show(pt);
        }

        protected virtual void OnCustomActionClicked()
        {
            // Override in derived class or subscribe to RegionClick event
            MessageBox.Show("Custom action button clicked! Override OnCustomActionClicked or subscribe to RegionClick event.", 
                "BeepiFormPro", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// <summary>
        /// Calculates layout and updates hit areas when form properties change
        /// </summary>
        private void RecalculateLayoutAndHitAreas()
        {
            // Make layout recalculation safe in design-time and runtime.
            // Ensure manager objects exist (they're lightweight) so painters can
            // compute layout even when the form is being edited in the designer.
            try
            {
                if (_layout == null)
                    _layout = new BeepiFormProLayoutManager(this);

                if (_hits == null)
                    _hits = new BeepiFormProHitAreaManager(this);

                if (_interact == null)
                    _interact = new BeepiFormProInteractionManager(this, _hits);

                // Ensure painter is available. ApplyFormStyle is idempotent and
                // safe to call; painters are simple renderers and are designer-safe.
                if (ActivePainter == null)
                {
                    try { ApplyFormStyle(); } catch { /* ignore in design-time */ }
                }

                if (ActivePainter == null)
                {
                    // Nothing to do if we still have no painter
                    return;
                }

                // Let the painter compute the layout. Wrap in try/catch to avoid
                // designer crashes if a painter implementation relies on runtime
                // resources that are unavailable in the designer.
                try
                {
                    ActivePainter.CalculateLayoutAndHitAreas(this);
                }
                catch
                {
                    // Swallow painter exceptions in design-time to keep the designer
                    // usable. At worst the painter won't show preview, but the form
                    // remains editable.
                }

                // Update hit areas for interaction (clear then register caption)
                _hits.Clear();
                if (ShowCaptionBar && CurrentLayout != null && CurrentLayout.CaptionRect.Width > 0 && CurrentLayout.CaptionRect.Height > 0)
                {
                    _hits.RegisterHitArea("caption", CurrentLayout.CaptionRect, HitAreaType.Caption);
                }
            }
            catch
            {
                // Top-level safety: swallow any unexpected exceptions so the
                // Visual Studio designer does not throw when loading the form.
            }
        }

        // P/Invoke for window dragging
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool ReleaseCapture();

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
    }
}