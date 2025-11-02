using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.BaseImage;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Painters;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Helpers;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Enums;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Models;

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
        
        // Split button area tracking (for Toggle with Split shape)
        private bool _leftAreaHovered = false;
        private bool _rightAreaHovered = false;
        private bool _leftAreaPressed = false;
        private bool _rightAreaPressed = false;
        
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
            
            // Enable splash effect from BaseControl
            EnableSplashEffect = true;
            
            // Use BaseControl's BorderThickness for consistency
            BorderThickness = 2;
            
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

        private void InitializePainter()
        {
            _currentPainter = ButtonPainterFactory.CreatePainter(_buttonStyle);
            
            // Set default shape based on button style
            _buttonShape = GetDefaultShapeForStyle(_buttonStyle);
            
            // Register split button areas if needed
            RegisterSplitButtonAreas();
        }

        /// <summary>
        /// Register or clear hit areas for split buttons
        /// </summary>
        private void RegisterSplitButtonAreas()
        {
            // Clear existing split button hit areas
            _hitTest?.HitList.RemoveAll(h => h.Name == "LeftButtonArea" || h.Name == "RightButtonArea");
            
            // Only register if this is a Split shape
            if (_buttonShape == ButtonShape.Split && Width > 0 && Height > 0)
            {
                int halfWidth = Width / 2;
                
                // Left area
                Rectangle leftArea = new Rectangle(0, 0, halfWidth, Height);
                _hitTest?.AddHitArea("LeftButtonArea", leftArea, component: null!, () =>
                {
                    HandleLeftAreaClick();
                });
                
                // Right area
                Rectangle rightArea = new Rectangle(halfWidth, 0, halfWidth, Height);
                _hitTest?.AddHitArea("RightButtonArea", rightArea, component: null!, () =>
                {
                    HandleRightAreaClick();
                });
            }
        }

        /// <summary>
        /// Handle left area click for split buttons
        /// </summary>
        private void HandleLeftAreaClick()
        {
            if (!Enabled) return;
            
            // For toggle buttons, activate left area (IsToggled = true)
            if (_buttonStyle == AdvancedButtonStyle.Toggle)
            {
                bool oldValue = IsToggled;
                IsToggled = true;
                
                if (oldValue != IsToggled)
                {
                    ToggleChanged?.Invoke(this, new ToggleChangedEventArgs(IsToggled, "Left"));
                }
            }
            
            LeftAreaClicked?.Invoke(this, EventArgs.Empty);
            Invalidate();
        }

        /// <summary>
        /// Handle right area click for split buttons
        /// </summary>
        private void HandleRightAreaClick()
        {
            if (!Enabled) return;
            
            // For toggle buttons, activate right area (IsToggled = false)
            if (_buttonStyle == AdvancedButtonStyle.Toggle)
            {
                bool oldValue = IsToggled;
                IsToggled = false;
                
                if (oldValue != IsToggled)
                {
                    ToggleChanged?.Invoke(this, new ToggleChangedEventArgs(IsToggled, "Right"));
                }
            }
            
            RightAreaClicked?.Invoke(this, EventArgs.Empty);
            Invalidate();
        }

        /// <summary>
        /// Update area hover states based on mouse position
        /// </summary>
        private void UpdateAreaHoverStates(Point mouseLocation)
        {
            if (_buttonShape == ButtonShape.Split && Width > 0)
            {
                int halfWidth = Width / 2;
                Rectangle clientRect = ClientRectangle;
                
                // Check which area is hovered
                bool wasLeftHovered = _leftAreaHovered;
                bool wasRightHovered = _rightAreaHovered;
                
                _leftAreaHovered = mouseLocation.X < halfWidth && clientRect.Contains(mouseLocation);
                _rightAreaHovered = mouseLocation.X >= halfWidth && clientRect.Contains(mouseLocation);
                
                // Invalidate if hover state changed
                if (wasLeftHovered != _leftAreaHovered || wasRightHovered != _rightAreaHovered)
                {
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets the default shape for a given button style
        /// </summary>
        private ButtonShape GetDefaultShapeForStyle(AdvancedButtonStyle style)
        {
            return style switch
            {
                AdvancedButtonStyle.FAB => ButtonShape.Circle,
                AdvancedButtonStyle.Ghost => ButtonShape.Pill,
                AdvancedButtonStyle.Toggle => ButtonShape.Split,
                AdvancedButtonStyle.Solid => ButtonShape.RoundedRectangle,
                AdvancedButtonStyle.Icon => ButtonShape.RoundedRectangle,
                AdvancedButtonStyle.Text => ButtonShape.RoundedRectangle,
                AdvancedButtonStyle.Outlined => ButtonShape.RoundedRectangle,
                AdvancedButtonStyle.Link => ButtonShape.Rectangle,
                AdvancedButtonStyle.Gradient => ButtonShape.RoundedRectangle,
                AdvancedButtonStyle.IconText => ButtonShape.RoundedRectangle,
                _ => ButtonShape.RoundedRectangle
            };
        }

        private void UpdateButtonSize()
        {
            Size newSize = _buttonSize switch
            {
                AdvancedButtonSize.Small => _smallSize,
                AdvancedButtonSize.Medium => _mediumSize,
                AdvancedButtonSize.Large => _largeSize,
                _ => _mediumSize
            };

            // For FAB buttons, make them square
            if (_buttonStyle == AdvancedButtonStyle.FAB)
            {
                int size = _buttonSize switch
                {
                    AdvancedButtonSize.Small => 40,
                    AdvancedButtonSize.Medium => 56,
                    AdvancedButtonSize.Large => 72,
                    _ => 56
                };
                newSize = new Size(size, size);
            }

            Size = newSize;
        }

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
                
                var context = new AdvancedButtonPaintContext
                {
                    Graphics = g,
                    Bounds = ClientRectangle,
                    State = GetCurrentState(),
                    Text = Text,
                    ImagePainter = _imagePainter,
                    IconLeft = _iconLeft,
                    IconRight = _iconRight,
                    IsToggled = _isToggled,
                    IsLoading = _isLoading,
                    Theme = theme!,
                    Shape = _buttonShape,
                    
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
                    SolidBackground = _solidBackground != Color.Empty ? _solidBackground : theme?.ButtonBackColor ?? Color.Gray,
                    SolidForeground = _solidForeground != Color.Empty ? _solidForeground : theme?.ButtonForeColor ?? Color.White,
                    HoverBackground = HoverBackColor != Color.Empty ? HoverBackColor : theme?.ButtonHoverBackColor ?? Color.LightGray,
                    HoverForeground = HoverForeColor != Color.Empty ? HoverForeColor : theme?.ButtonHoverForeColor ?? Color.White,
                    PressedBackground = PressedBackColor != Color.Empty ? PressedBackColor : theme?.ButtonPressedBackColor ?? Color.DarkGray,
                    DisabledBackground = DisabledBackColor != Color.Empty ? DisabledBackColor : theme?.DisabledBackColor ?? Color.LightGray,
                    DisabledForeground = DisabledForeColor != Color.Empty ? DisabledForeColor : theme?.DisabledForeColor ?? Color.Gray,
                    
                    // Additional colors from theme
                    BackgroundColor = theme?.ButtonBackColor ?? Color.Gray,
                    TextColor = theme?.ButtonForeColor ?? Color.White,
                    IconColor = theme?.ButtonForeColor ?? Color.White,
                    SecondaryColor = theme?.SecondaryColor ?? Color.LightGray,
                    
                    // Glow and effects
                    GlowColor = theme?.AccentColor ?? theme?.PrimaryColor ?? Color.Blue,
                    RippleColor = theme?.AccentColor ?? theme?.PrimaryColor ?? Color.Blue,
                    LoadingIndicatorColor = theme?.AccentColor ?? theme?.PrimaryColor ?? Color.Blue,
                    
                    // Toggle colors
                    ToggleOnColor = theme?.ButtonSelectedBackColor ?? theme?.AccentColor ?? Color.Green,
                    ToggleOffColor = theme?.ButtonBackColor ?? Color.Gray,
                    ToggleBorderColor = theme?.ButtonBorderColor ?? Color.DarkGray,
                    
                    // Badge/Chip colors
                    BadgeColor = theme?.BadgeBackColor ?? theme?.AccentColor ?? Color.Red,
                    BadgeText = string.Empty,
                    
                    // Contact button icon background
                    IconBackgroundColor = theme?.AccentColor ?? theme?.PrimaryColor ?? Color.Blue,
                    
                    // Shadow properties from BaseControl
                    ShowShadow = ShowShadow,
                    ShadowBlur = ShadowOffset,
                    ShadowColor = ShadowColor,
                    
                    // Animation/interaction state
                    RippleActive = false,
                    RippleCenter = Point.Empty,
                    RippleProgress = 0f,
                    
                    // State flags
                    IsHovered = IsHovered,
                    IsPressed = IsPressed,
                    ShowBorder = BorderThickness > 0,
                    BorderThickness = BorderThickness,
                    
                    // Typography
                    Font = Font,
                    ButtonSize = _buttonSize,
                    ButtonShape = _buttonShape
                };

                _currentPainter.Paint(context);
            }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            IsHovered = true;
            Point mousePos = PointToClient(Cursor.Position);
            UpdateAreaHoverStates(mousePos);
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            IsHovered = false;
            _leftAreaHovered = false;
            _rightAreaHovered = false;
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
            if (e.Button == MouseButtons.Left)
            {
                IsPressed = true; // Use BaseControl property
                
                // Update pressed state for split button areas
                if (_buttonShape == ButtonShape.Split && Width > 0)
                {
                    int halfWidth = Width / 2;
                    _leftAreaPressed = e.Location.X < halfWidth;
                    _rightAreaPressed = e.Location.X >= halfWidth;
                }
                
                Invalidate();
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
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
            Invalidate();
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            Invalidate();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
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
            if (IsHovered)
                return AdvancedButtonState.Hover;
            return AdvancedButtonState.Normal;
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
            Update(); // Force synchronous paint
        }

        #endregion
    }
}
