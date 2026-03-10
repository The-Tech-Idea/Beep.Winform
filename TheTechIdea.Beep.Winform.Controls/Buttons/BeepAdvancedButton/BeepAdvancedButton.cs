using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.BaseImage;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Painters;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Helpers;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Enums;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Models;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;

namespace TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton
{
    /// <summary>
    /// Advanced button control with multiple style painters and rich features.
    /// Supports various button types: Solid, Icon, Text, Toggle, FAB, Ghost, etc.
    /// Uses partial classes for organization and custom painters for rendering.
    /// </summary>
    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(Button))]
    [Category("Beep Controls")]
    [Description("Advanced button with multiple styles and rich features")]
    [DisplayName("Beep Advanced Button")]
    public partial class BeepAdvancedButton : BaseControl
    {
        #region "Fields"

        private AdvancedButtonStyle _buttonStyle = AdvancedButtonStyle.Solid;
        private AdvancedButtonSize _buttonSize = AdvancedButtonSize.Medium;
        private ButtonShape _buttonShape = ButtonShape.RoundedRectangle;
        private IAdvancedButtonPainter? _currentPainter;
        private ImagePainter _imagePainter;
        
        // State tracking - use BaseControl.IsPressed and IsHovered
        private bool _isToggled = false;
        private bool _isLoading = false;
        private bool _reduceMotion = false;
        private bool _showFocusRing = true;
        private bool _suppressClickWhileLoading = true;
        private int _focusRingThickness = 2;
        private int _focusRingOffset = 2;
        private int _focusRingRadiusDelta = 2;
        private bool _keyboardFocusVisible = false;
        private ButtonIntent _intent = ButtonIntent.Primary;
        private NewsBannerVariant _newsBannerVariant = NewsBannerVariant.Auto;
        private ContactVariant _contactVariant = ContactVariant.Auto;
        private ChevronVariant _chevronVariant = ChevronVariant.Auto;
        private FlatWebVariant _flatWebVariant = FlatWebVariant.Auto;
        private LowerThirdVariant _lowerThirdVariant = LowerThirdVariant.Auto;
        private StickerLabelVariant _stickerLabelVariant = StickerLabelVariant.Auto;
        
        // Split button area tracking (for Toggle with Split shape)
        private bool _leftAreaHovered = false;
        private bool _rightAreaHovered = false;
        private bool _leftAreaPressed = false;
        private bool _rightAreaPressed = false;
        private bool _keyboardSplitSelectRight = false;
        private bool _spaceKeyDown = false;
        private bool _enterKeyDown = false;
        
        // Content - using BaseControl.ImagePath for main image
        private string _iconLeft = string.Empty;
        private string _iconRight = string.Empty;
        
        // Colors - Default values, will be overridden by theme
        private Color _solidBackground = Color.FromArgb(79, 70, 229); // Indigo-600
        private Color _solidForeground = Color.White;
        
        // Sizes (based on UI cheat sheet images)
        private Size _smallSize = new Size(80, 32);
        private Size _mediumSize = new Size(100, 40);
        private Size _largeSize = new Size(120, 48);

        // Animation state
        private readonly Timer _animationTimer = new Timer();
        private float _hoverProgress;
        private float _pressProgress;
        private bool _rippleActive;
        private Point _rippleCenter;
        private float _rippleProgress;
        private float _loadingRotationAngle;
        
        #endregion

        #region "Events"

        /// <summary>
        /// Raised when toggle state changes (for Toggle buttons)
        /// </summary>
        public event EventHandler<ToggleChangedEventArgs>? ToggleChanged;

        /// <summary>
        /// Raised when left area is clicked (for Split shape Toggle buttons)
        /// </summary>
        public event EventHandler? LeftAreaClicked;

        /// <summary>
        /// Raised when right area is clicked (for Split shape Toggle buttons)
        /// </summary>
        public event EventHandler? RightAreaClicked;

        #endregion

        #region "Constructor"

        public BeepAdvancedButton()
        {
            // BaseControl already sets up double buffering and control styles
            // Just add button-specific initialization
            
            // Initialize painter
            _imagePainter = new ImagePainter();
            InitializePainter();
            
            // Set default size
            Size = _mediumSize;
            MinimumSize = _smallSize;
            
            // Enable tab stop for accessibility
            TabStop = true;
            CanBeFocused = true;
            
            // Enable splash effect from BaseControl
            EnableSplashEffect = true;
            
            // Use BaseControl's BorderThickness for consistency
            BorderThickness = 2;

            // Shared animation timer for hover/press/ripple/loading.
            _animationTimer.Interval = 16;
            _animationTimer.Tick += OnAnimationTick;
            
            // Apply initial theme
            if (_currentTheme != null)
            {
                ApplyTheme();
            }
        }

        #endregion

        #region "Properties"

        [Browsable(true)]
        [Category("Appearance")]
        [Description("The style of the button (Solid, Icon, Text, Toggle, FAB, Ghost)")]
        [DefaultValue(AdvancedButtonStyle.Solid)]
        public AdvancedButtonStyle ButtonStyle
        {
            get => _buttonStyle;
            set
            {
                if (_buttonStyle != value)
                {
                    _buttonStyle = value;
                    InitializePainter();
                    // Don't auto-resize unless it's FAB style which requires square shape
                    if (value == AdvancedButtonStyle.FAB)
                    {
                        UpdateButtonSize();
                    }
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Variants")]
        [Description("Explicit news banner layout variant. Auto keeps heuristic layout selection.")]
        [DefaultValue(NewsBannerVariant.Auto)]
        public NewsBannerVariant NewsBannerVariant
        {
            get => _newsBannerVariant;
            set
            {
                if (_newsBannerVariant != value)
                {
                    _newsBannerVariant = value;
                    InitializePainter(resetShape: false);
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Variants")]
        [Description("Explicit contact button layout variant. Auto keeps heuristic layout selection.")]
        [DefaultValue(ContactVariant.Auto)]
        public ContactVariant ContactVariant
        {
            get => _contactVariant;
            set
            {
                if (_contactVariant != value)
                {
                    _contactVariant = value;
                    InitializePainter(resetShape: false);
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Variants")]
        [Description("Explicit navigation chevron layout variant. Auto keeps heuristic layout selection.")]
        [DefaultValue(ChevronVariant.Auto)]
        public ChevronVariant ChevronVariant
        {
            get => _chevronVariant;
            set
            {
                if (_chevronVariant != value)
                {
                    _chevronVariant = value;
                    InitializePainter(resetShape: false);
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Variants")]
        [Description("Explicit flat web layout variant selector. Auto keeps heuristic layout selection.")]
        [DefaultValue(FlatWebVariant.Auto)]
        public FlatWebVariant FlatWebVariant
        {
            get => _flatWebVariant;
            set
            {
                if (_flatWebVariant != value)
                {
                    _flatWebVariant = value;
                    InitializePainter(resetShape: false);
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Variants")]
        [Description("Explicit lower-third layout variant selector. Auto keeps heuristic layout selection.")]
        [DefaultValue(LowerThirdVariant.Auto)]
        public LowerThirdVariant LowerThirdVariant
        {
            get => _lowerThirdVariant;
            set
            {
                if (_lowerThirdVariant != value)
                {
                    _lowerThirdVariant = value;
                    InitializePainter(resetShape: false);
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Variants")]
        [Description("Explicit sticker label layout variant selector. Auto keeps heuristic layout selection.")]
        [DefaultValue(StickerLabelVariant.Auto)]
        public StickerLabelVariant StickerLabelVariant
        {
            get => _stickerLabelVariant;
            set
            {
                if (_stickerLabelVariant != value)
                {
                    _stickerLabelVariant = value;
                    InitializePainter(resetShape: false);
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("The size preset of the button (Small, Medium, Large)")]
        [DefaultValue(AdvancedButtonSize.Medium)]
        public AdvancedButtonSize ButtonSize
        {
            get => _buttonSize;
            set
            {
                if (_buttonSize != value)
                {
                    _buttonSize = value;
                    UpdateButtonSize();
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("The shape of the button (RoundedRectangle, Circle, Pill, Split, etc.). Each style has a default shape.")]
        [DefaultValue(ButtonShape.RoundedRectangle)]
        public ButtonShape Shape
        {
            get => _buttonShape;
            set
            {
                if (_buttonShape != value)
                {
                    _buttonShape = value;
                    RegisterSplitButtonAreas(); // Register/unregister hit areas
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("The text displayed on the button")]
        [DefaultValue("Button")]
        public override string Text
        {
            get => _text;
            set
            {
                if (_text != value)
                {
                    _text = value;
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Path to the image/icon for the button")]
        [Editor("TheTechIdea.Beep.Winform.Controls.Design.Server.Editors.BeepImagePathEditor, TheTechIdea.Beep.Winform.Controls.Design.Server", typeof(System.Drawing.Design.UITypeEditor))]
        public string ImagePath
        {
            get => LeadingImagePath; // Use BaseControl property
            set
            {
                if (LeadingImagePath != value)
                {
                    LeadingImagePath = value; // Use BaseControl property
                    _imagePainter.ImagePath = value;
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Icon path for left side of button (for buttons with dual icons)")]
        public string IconLeft
        {
            get => _iconLeft;
            set
            {
                if (_iconLeft != value)
                {
                    _iconLeft = value;
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Icon path for right side of button")]
        public string IconRight
        {
            get => _iconRight;
            set
            {
                if (_iconRight != value)
                {
                    _iconRight = value;
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Behavior")]
        [Description("For toggle buttons, indicates if the button is toggled on")]
        [DefaultValue(false)]
        public bool IsToggled
        {
            get => _isToggled;
            set
            {
                if (_isToggled != value)
                {
                    _isToggled = value;
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Shows a loading spinner animation")]
        [DefaultValue(false)]
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    UpdateAnimationTimerState();
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Behavior")]
        [Description("Semantic intent used for tokenized color mapping.")]
        [DefaultValue(ButtonIntent.Primary)]
        public ButtonIntent Intent
        {
            get => _intent;
            set
            {
                if (_intent != value)
                {
                    _intent = value;
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Behavior")]
        [Description("When true, disables motion-heavy transitions and ripple animation.")]
        [DefaultValue(false)]
        public bool ReduceMotion
        {
            get => _reduceMotion;
            set
            {
                if (_reduceMotion != value)
                {
                    _reduceMotion = value;
                    if (_reduceMotion)
                    {
                        _rippleActive = false;
                        _rippleProgress = 0f;
                        _hoverProgress = IsHovered ? 1f : 0f;
                        _pressProgress = IsPressed ? 1f : 0f;
                        _loadingRotationAngle = 0f;
                    }

                    UpdateAnimationTimerState();
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Displays a keyboard-only focus ring when focus-visible is active.")]
        [DefaultValue(true)]
        public bool ShowFocusRing
        {
            get => _showFocusRing;
            set
            {
                if (_showFocusRing != value)
                {
                    _showFocusRing = value;
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Behavior")]
        [Description("When true, click activation is suppressed while loading is active.")]
        [DefaultValue(true)]
        public bool SuppressClickWhileLoading
        {
            get => _suppressClickWhileLoading;
            set => _suppressClickWhileLoading = value;
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Focus ring stroke thickness in pixels.")]
        [DefaultValue(2)]
        public int FocusRingThickness
        {
            get => _focusRingThickness;
            set
            {
                int sanitized = Math.Max(1, value);
                if (_focusRingThickness != sanitized)
                {
                    _focusRingThickness = sanitized;
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Inset offset for focus ring rendering in pixels.")]
        [DefaultValue(2)]
        public int FocusRingOffset
        {
            get => _focusRingOffset;
            set
            {
                int sanitized = Math.Max(0, value);
                if (_focusRingOffset != sanitized)
                {
                    _focusRingOffset = sanitized;
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Additional radius applied to focus ring over button border radius.")]
        [DefaultValue(2)]
        public int FocusRingRadiusDelta
        {
            get => _focusRingRadiusDelta;
            set
            {
                int sanitized = Math.Max(0, value);
                if (_focusRingRadiusDelta != sanitized)
                {
                    _focusRingRadiusDelta = sanitized;
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Background color for solid buttons")]
        public Color SolidBackground
        {
            get => _solidBackground;
            set { _solidBackground = value; Invalidate(); }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Foreground/text color for solid buttons")]
        public Color SolidForeground
        {
            get => _solidForeground;
            set { _solidForeground = value; Invalidate(); }
        }

        #endregion

        #region "Public Methods"

        /// <summary>
        /// Programmatically trigger a click
        /// </summary>
        public void PerformClick()
        {
            OnClick(EventArgs.Empty);
        }

        /// <summary>
        /// Toggle the button state (for toggle buttons)
        /// </summary>
        public void Toggle()
        {
            IsToggled = !IsToggled;
        }

        #endregion

        #region "Private Methods"

        private void InitializePainter(bool resetShape = true)
        {
            _currentPainter = ButtonPainterFactory.CreatePainter(
                _buttonStyle,
                _newsBannerVariant,
                _contactVariant,
                _chevronVariant);
             
            // Set default shape based on button style
            if (resetShape)
            {
                _buttonShape = GetDefaultShapeForStyle(_buttonStyle);
            }
             
            // Register split button areas if needed
            RegisterSplitButtonAreas();
        }

        // Layout and split-area behavior moved to BeepAdvancedButton.Layout.cs.

        #endregion

        #region "Overrides"

        protected override void DrawContent(Graphics g)
        {
            // If UseFormStylePaint is enabled, use BaseControl's painter rendering
            if (UseFormStylePaint)
            {
                base.DrawContent(g);
                return;
            }

            // Otherwise use custom advanced button painter rendering
            if (_currentPainter != null)
            {
                var theme = _currentTheme as BeepTheme;
                var textFont = ResolveTextFont(theme);
                var intentTokens = ResolveIntentColors(theme);
                
                var context = new AdvancedButtonPaintContext
                {
                    Graphics = g,
                    OwnerControl = this,
                    Bounds = ClientRectangle,
                    State = GetCurrentState(),
                    Text = Text,
                    ImagePath = LeadingImagePath ?? string.Empty,
                    ImagePainter = _imagePainter,
                    IconLeft = _iconLeft,
                    IconRight = _iconRight,
                    IsToggled = _isToggled,
                    IsLoading = _isLoading,
                    Theme = theme!,
                    Shape = _buttonShape,
                    NewsBannerVariant = _newsBannerVariant,
                    ContactVariant = _contactVariant,
                    ChevronVariant = _chevronVariant,
                    FlatWebVariant = _flatWebVariant,
                    LowerThirdVariant = _lowerThirdVariant,
                    StickerLabelVariant = _stickerLabelVariant,
                    
                    // Split button area states (from BaseControl input helpers)
                    LeftAreaHovered = _leftAreaHovered,
                    RightAreaHovered = _rightAreaHovered,
                    LeftAreaPressed = _leftAreaPressed,
                    RightAreaPressed = _rightAreaPressed,
                    
                    // Border properties from BaseControl
                    BorderRadius = BorderRadius,
                    BorderWidth = BorderThickness,
                    BorderColor = BorderColor,
                    
                    // Core colors - fallback to theme if not explicitly set
                    SolidBackground = _solidBackground != Color.Empty ? _solidBackground : intentTokens.Background,
                    SolidForeground = _solidForeground != Color.Empty ? _solidForeground : intentTokens.Foreground,
                    HoverBackground = HoverBackColor != Color.Empty ? HoverBackColor : intentTokens.HoverBackground,
                    HoverForeground = HoverForeColor != Color.Empty ? HoverForeColor : intentTokens.HoverForeground,
                    PressedBackground = PressedBackColor != Color.Empty ? PressedBackColor : intentTokens.PressedBackground,
                    DisabledBackground = DisabledBackColor != Color.Empty ? DisabledBackColor : theme?.DisabledBackColor ?? Color.LightGray,
                    DisabledForeground = DisabledForeColor != Color.Empty ? DisabledForeColor : theme?.DisabledForeColor ?? Color.Gray,
                    
                    // Additional colors from theme
                    BackgroundColor = intentTokens.Background,
                    TextColor = intentTokens.Foreground,
                    IconColor = intentTokens.Foreground,
                    SecondaryColor = theme?.SecondaryColor ?? Color.LightGray,
                    FocusRingColor = theme?.PrimaryColor ?? SystemColors.Highlight,
                    
                    // Glow and effects
                    GlowColor = theme?.AccentColor ?? theme?.PrimaryColor ?? Color.FromArgb(0, 255, 153),
                    RippleColor = theme?.AccentColor ?? theme?.PrimaryColor ?? Color.FromArgb(0, 255, 153),
                    LoadingIndicatorColor = theme?.AccentColor ?? theme?.PrimaryColor ?? Color.FromArgb(0, 255, 153),
                    
                    // Toggle colors
                    ToggleOnColor = theme?.ButtonSelectedBackColor ?? theme?.AccentColor ?? Color.Green,
                    ToggleOffColor = theme?.ButtonBackColor ?? Color.Gray,
                    ToggleBorderColor = theme?.ButtonBorderColor ?? Color.DarkGray,
                    
                    // Badge/Chip colors
                    BadgeColor = theme?.BadgeBackColor ?? theme?.AccentColor ?? Color.Red,
                    BadgeText = string.Empty,
                    
                    // Contact button icon background
                    IconBackgroundColor = theme?.AccentColor ?? theme?.PrimaryColor ?? Color.FromArgb(0, 255, 153),
                    
                    // Shadow properties from BaseControl
                    ShowShadow = ShowShadow,
                    ShadowBlur = ShadowOffset,
                    ShadowColor = ShadowColor,
                    
                    // Animation/interaction state
                    RippleActive = _rippleActive,
                    RippleCenter = _rippleCenter,
                    RippleProgress = _rippleProgress,
                    HoverProgress = _hoverProgress,
                    PressProgress = _pressProgress,
                    LoadingRotationAngle = _loadingRotationAngle,
                    ReduceMotion = _reduceMotion,
                    
                    // State flags
                    IsHovered = IsHovered,
                    IsPressed = IsPressed,
                    ShowBorder = BorderThickness > 0,
                    BorderThickness = BorderThickness,
                    ShowFocusRing = _showFocusRing,
                    FocusRingThickness = _focusRingThickness,
                    FocusRingOffset = _focusRingOffset,
                    FocusRingRadiusDelta = _focusRingRadiusDelta,
                    
                    // Typography
                    TextFont = textFont,
                    Intent = _intent,
                    ButtonSize = _buttonSize,
                    ButtonShape = _buttonShape
                };

                _currentPainter.Paint(context);
                DrawFocusRing(g, context);
            }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            IsHovered = true;
            Point mousePos = PointToClient(Cursor.Position);
            UpdateAreaHoverStates(mousePos);
            UpdateAnimationTimerState();
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            IsHovered = false;
            _leftAreaHovered = false;
            _rightAreaHovered = false;
            UpdateAnimationTimerState();
            Invalidate();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            UpdateAreaHoverStates(e.Location);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (_isLoading && _suppressClickWhileLoading)
            {
                return;
            }

            if (e.Button == MouseButtons.Left)
            {
                _keyboardFocusVisible = false;
                IsPressed = true; // Use BaseControl property
                StartRipple(e.Location);
                
                // Update pressed state for split button areas
                if (_buttonShape == ButtonShape.Split && Width > 0)
                {
                    int halfWidth = Width / 2;
                    _leftAreaPressed = e.Location.X < halfWidth;
                    _rightAreaPressed = e.Location.X >= halfWidth;
                }
                
                UpdateAnimationTimerState();
                Invalidate();
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (_isLoading && _suppressClickWhileLoading)
            {
                IsPressed = false;
                _leftAreaPressed = false;
                _rightAreaPressed = false;
                UpdateAnimationTimerState();
                Invalidate();
                return;
            }

            IsPressed = false; // Use BaseControl property
            _leftAreaPressed = false;
            _rightAreaPressed = false;
            
            // For non-split toggle buttons, toggle on click
            if (_buttonStyle == AdvancedButtonStyle.Toggle && _buttonShape != ButtonShape.Split && ClientRectangle.Contains(e.Location))
            {
                bool oldValue = IsToggled;
                IsToggled = !IsToggled;
                
                if (oldValue != IsToggled)
                {
                    ToggleChanged?.Invoke(this, new ToggleChangedEventArgs(IsToggled, null));
                }
            }
            
            UpdateAnimationTimerState();
            Invalidate();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            // Re-register hit areas when control resizes
            RegisterSplitButtonAreas();
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);
            UpdateAnimationTimerState();
            Invalidate();
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            Invalidate();
        }

        protected override bool IsInputKey(Keys keyData)
        {
            Keys key = keyData & Keys.KeyCode;
            if (key == Keys.Enter || key == Keys.Space || key == Keys.Left || key == Keys.Right)
            {
                return true;
            }

            return base.IsInputKey(keyData);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (!Enabled)
            {
                return;
            }

            if (_isLoading && _suppressClickWhileLoading)
            {
                return;
            }

            if (e.KeyCode == Keys.Tab || e.KeyCode == Keys.Left || e.KeyCode == Keys.Right || e.KeyCode == Keys.Up || e.KeyCode == Keys.Down)
            {
                _keyboardFocusVisible = true;
            }

            if (_buttonStyle == AdvancedButtonStyle.Toggle && _buttonShape == ButtonShape.Split)
            {
                if (e.KeyCode == Keys.Left)
                {
                    _keyboardSplitSelectRight = false;
                    _keyboardFocusVisible = true;
                    Invalidate();
                    e.Handled = true;
                    return;
                }

                if (e.KeyCode == Keys.Right)
                {
                    _keyboardSplitSelectRight = true;
                    _keyboardFocusVisible = true;
                    Invalidate();
                    e.Handled = true;
                    return;
                }
            }

            if (e.KeyCode == Keys.Space || e.KeyCode == Keys.Enter)
            {
                bool firstPress = e.KeyCode == Keys.Space ? !_spaceKeyDown : !_enterKeyDown;
                if (firstPress)
                {
                    if (e.KeyCode == Keys.Space) _spaceKeyDown = true;
                    if (e.KeyCode == Keys.Enter) _enterKeyDown = true;

                    _keyboardFocusVisible = true;
                    IsPressed = true;
                    StartRipple(new Point(Width / 2, Height / 2));
                    UpdateAnimationTimerState();
                    Invalidate();
                }

                e.Handled = true;
            }
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);

            if (!Enabled)
            {
                return;
            }

            if (_isLoading && _suppressClickWhileLoading)
            {
                return;
            }

            if (e.KeyCode == Keys.Space || e.KeyCode == Keys.Enter)
            {
                bool wasDown = e.KeyCode == Keys.Space ? _spaceKeyDown : _enterKeyDown;
                if (e.KeyCode == Keys.Space) _spaceKeyDown = false;
                if (e.KeyCode == Keys.Enter) _enterKeyDown = false;

                if (wasDown)
                {
                    IsPressed = false;
                    ActivateFromKeyboard();
                    UpdateAnimationTimerState();
                    Invalidate();
                }

                e.Handled = true;
            }
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            UpdateAnimationTimerState();
            Invalidate();
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            _keyboardFocusVisible = false;
            _spaceKeyDown = false;
            _enterKeyDown = false;
            IsPressed = false;
            UpdateAnimationTimerState();
            Invalidate();
        }

        protected override void OnClick(EventArgs e)
        {
            if (_isLoading && _suppressClickWhileLoading)
            {
                return;
            }

            base.OnClick(e);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _animationTimer.Stop();
                _animationTimer.Tick -= OnAnimationTick;
                _animationTimer.Dispose();
                _imagePainter?.Dispose();
            }
            base.Dispose(disposing);
        }

        #endregion

        #region "Helper Methods"

        private AdvancedButtonState GetCurrentState()
        {
            if (!Enabled)
                return AdvancedButtonState.Disabled;
            if (IsPressed) // Use BaseControl property
                return AdvancedButtonState.Pressed;
            if (Focused && _keyboardFocusVisible)
                return AdvancedButtonState.Focused;
            if (IsHovered)
                return AdvancedButtonState.Hover;
            return AdvancedButtonState.Normal;
        }

        private Font ResolveTextFont(BeepTheme? theme)
        {
            TypographyStyle style = theme?.ButtonFont;

            if (style == null)
            {
                var controlFont = Font ?? SystemFonts.DefaultFont;
                style = new TypographyStyle
                {
                    FontFamily = controlFont.FontFamily.Name,
                    FontSize = controlFont.Size,
                    FontWeight = controlFont.Bold ? FontWeight.Bold : FontWeight.Regular,
                    IsUnderlined = controlFont.Underline,
                    IsStrikeout = controlFont.Strikeout
                };
            }

            return BeepThemesManager.ToFont(style, applyDpiScaling: true);
        }

        private void ActivateFromKeyboard()
        {
            if (!Enabled || _isLoading)
            {
                return;
            }

            if (_buttonStyle == AdvancedButtonStyle.Toggle)
            {
                if (_buttonShape == ButtonShape.Split)
                {
                    if (_keyboardSplitSelectRight)
                    {
                        HandleRightAreaClick();
                    }
                    else
                    {
                        HandleLeftAreaClick();
                    }
                }
                else
                {
                    bool oldValue = IsToggled;
                    IsToggled = !IsToggled;
                    if (oldValue != IsToggled)
                    {
                        ToggleChanged?.Invoke(this, new ToggleChangedEventArgs(IsToggled, null));
                    }
                }
            }

            OnClick(EventArgs.Empty);
        }

        private void StartRipple(Point center)
        {
            if (_reduceMotion)
            {
                _rippleActive = false;
                _rippleProgress = 0f;
                return;
            }

            _rippleCenter = center;
            _rippleProgress = 0f;
            _rippleActive = true;
            UpdateAnimationTimerState();
        }

        private void OnAnimationTick(object? sender, EventArgs e)
        {
            bool needsInvalidate = false;

            float targetHover = IsHovered ? 1f : 0f;
            float targetPress = IsPressed ? 1f : 0f;

            if (_reduceMotion)
            {
                if (_hoverProgress != targetHover || _pressProgress != targetPress || _loadingRotationAngle != 0f)
                {
                    _hoverProgress = targetHover;
                    _pressProgress = targetPress;
                    _loadingRotationAngle = 0f;
                    needsInvalidate = true;
                }

                _rippleActive = false;
                _rippleProgress = 0f;
            }
            else
            {
                needsInvalidate |= StepTowards(ref _hoverProgress, targetHover, 0.22f);
                needsInvalidate |= StepTowards(ref _pressProgress, targetPress, 0.3f);

                if (_rippleActive)
                {
                    _rippleProgress += 0.08f;
                    if (_rippleProgress >= 1f)
                    {
                        _rippleProgress = 1f;
                        _rippleActive = false;
                    }
                    needsInvalidate = true;
                }

                if (_isLoading)
                {
                    _loadingRotationAngle += 14f;
                    if (_loadingRotationAngle >= 360f)
                    {
                        _loadingRotationAngle -= 360f;
                    }
                    needsInvalidate = true;
                }
                else if (_loadingRotationAngle != 0f)
                {
                    _loadingRotationAngle = 0f;
                    needsInvalidate = true;
                }
            }

            UpdateAnimationTimerState();
            if (needsInvalidate)
            {
                Invalidate();
            }
        }

        private bool StepTowards(ref float current, float target, float factor)
        {
            float previous = current;
            current += (target - current) * factor;
            if (Math.Abs(current - target) < 0.01f)
            {
                current = target;
            }
            return Math.Abs(previous - current) > 0.0001f;
        }

        private void UpdateAnimationTimerState()
        {
            bool needsTimer =
                !_reduceMotion &&
                (_rippleActive ||
                 _isLoading ||
                 Math.Abs(_hoverProgress - (IsHovered ? 1f : 0f)) > 0.01f ||
                 Math.Abs(_pressProgress - (IsPressed ? 1f : 0f)) > 0.01f);

            if (needsTimer)
            {
                if (!_animationTimer.Enabled)
                {
                    _animationTimer.Start();
                }
            }
            else if (_animationTimer.Enabled)
            {
                _animationTimer.Stop();
            }
        }

        private void DrawFocusRing(Graphics g, AdvancedButtonPaintContext context)
        {
            if (!context.ShowFocusRing || context.State != AdvancedButtonState.Focused || !Enabled)
            {
                return;
            }

            Rectangle ringBounds = context.Bounds;
            ringBounds.Inflate(-context.FocusRingOffset, -context.FocusRingOffset);
            if (ringBounds.Width <= 0 || ringBounds.Height <= 0)
            {
                return;
            }

            ButtonShape ringShape = context.Shape == ButtonShape.Split ? ButtonShape.RoundedRectangle : context.Shape;
            int ringRadius = Math.Max(0, context.BorderRadius + context.FocusRingRadiusDelta);
            using GraphicsPath ringPath = ButtonShapeHelper.CreateShapePath(ringShape, ringBounds, ringRadius);
            using Pen focusPen = new Pen(context.FocusRingColor, context.FocusRingThickness)
            {
                Alignment = PenAlignment.Inset
            };

            g.DrawPath(focusPen, ringPath);
        }

        private IntentColors ResolveIntentColors(BeepTheme? theme)
        {
            Color defaultBack = theme?.ButtonBackColor ?? Color.FromArgb(79, 70, 229);
            Color defaultFore = theme?.ButtonForeColor ?? Color.White;
            Color accent = theme?.AccentColor ?? theme?.PrimaryColor ?? defaultBack;
            Color secondary = theme?.SecondaryColor ?? defaultBack;

            return _intent switch
            {
                ButtonIntent.Secondary => new IntentColors(secondary, defaultFore, Shift(secondary, 10), Shift(secondary, -12)),
                ButtonIntent.Tertiary => new IntentColors(defaultBack, defaultFore, Shift(defaultBack, 8), Shift(defaultBack, -10)),
                ButtonIntent.Destructive => new IntentColors(Color.FromArgb(220, 53, 69), Color.White, Color.FromArgb(230, 72, 88), Color.FromArgb(184, 38, 52)),
                ButtonIntent.Success => new IntentColors(Color.FromArgb(25, 135, 84), Color.White, Color.FromArgb(40, 153, 102), Color.FromArgb(18, 112, 69)),
                ButtonIntent.Neutral => new IntentColors(Color.FromArgb(108, 117, 125), Color.White, Color.FromArgb(123, 132, 140), Color.FromArgb(87, 95, 102)),
                _ => new IntentColors(accent, defaultFore, theme?.ButtonHoverBackColor ?? Shift(accent, 10), theme?.ButtonPressedBackColor ?? Shift(accent, -10))
            };
        }

        private static Color Shift(Color color, int delta)
        {
            int r = Math.Clamp(color.R + delta, 0, 255);
            int g = Math.Clamp(color.G + delta, 0, 255);
            int b = Math.Clamp(color.B + delta, 0, 255);
            return Color.FromArgb(color.A, r, g, b);
        }

        private readonly struct IntentColors
        {
            public IntentColors(Color background, Color foreground, Color hoverBackground, Color pressedBackground, Color? hoverForeground = null)
            {
                Background = background;
                Foreground = foreground;
                HoverBackground = hoverBackground;
                PressedBackground = pressedBackground;
                HoverForeground = hoverForeground ?? foreground;
            }

            public Color Background { get; }
            public Color Foreground { get; }
            public Color HoverBackground { get; }
            public Color HoverForeground { get; }
            public Color PressedBackground { get; }
        }

        #endregion

        #region "Theme Application"

        /// <summary>
        /// Apply theme colors to the button using BaseControl's properties
        /// </summary>
        public override void ApplyTheme()
        {
            base.ApplyTheme(); // Let BaseControl set all the standard properties

            if (_currentTheme == null)
                return;

            // CRITICAL: Update button-specific colors from theme
            // These are used by the painters when UseFormStylePaint = false
            _solidBackground = _currentTheme.ButtonBackColor;
            _solidForeground = _currentTheme.ButtonForeColor;
            
            // IMPORTANT: Also update BaseControl's BackColor and ForeColor
            // This ensures consistency when UseFormStylePaint = true
            try
            {
                BackColor = _currentTheme.ButtonBackColor;
                ForeColor = _currentTheme.ButtonForeColor;
            }
            catch
            {
                // In case of any issues setting colors, use defaults
            }
            
            // Apply theme to image painter for icons
            if (_imagePainter != null && !string.IsNullOrEmpty(LeadingImagePath))
            {
                _imagePainter.Theme = Theme;
                _imagePainter.FillColor = _currentTheme.ButtonForeColor;
                _imagePainter.StrokeColor = _currentTheme.ButtonForeColor;
            }
            
            // BaseControl.ApplyTheme() already sets:
            // - HoverBackColor, HoverForeColor
            // - PressedBackColor  
            // - DisabledBackColor, DisabledForeColor
            // - BorderColor, ShadowColor
            // These are read directly from BaseControl properties in OnPaint

            // Force immediate redraw with new theme colors
            Invalidate();
            UpdateAnimationTimerState();
        }

        #endregion
    }
}
