using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Toggle.Helpers;
using TheTechIdea.Beep.Winform.Controls.Toggle.Painters;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;
using TheTechIdea.Beep.Winform.Controls.ToolTips;

namespace TheTechIdea.Beep.Winform.Controls.Toggle
{
    /// <summary>
    /// Modern toggle switch control with multiple visual styles
    /// Supports ON/OFF states with smooth animations and customizable appearance
    /// </summary>
    [ToolboxItem(true)]
    [Category("Beep Controls")]
    [Description("Modern toggle switch with multiple visual styles")]
    public partial class BeepToggle : BaseControl
    {
        #region Private Fields
        private bool _isOn = false;
        private ToggleStyle _toggleStyle = ToggleStyle.Classic;
        private Color _onColor = Color.FromArgb(52, 168, 83); // Green
        private Color _offColor = Color.FromArgb(189, 189, 189); // Gray
        private Color _thumbColor = Color.White;
        private Color _onThumbColor = Color.White;
        private Color _offThumbColor = Color.White;
        private string _onText = "ON";
        private string _offText = "OFF";
        private string _onIconPath = "";
        private string _offIconPath = "";
        private bool _showLabels = true;
        private bool _animateTransition = true;
        private int _animationDuration = 200; // milliseconds
        private float _thumbPosition = 0f; // 0 = OFF position, 1 = ON position
        private Timer _animationTimer;
        private DateTime _animationStart;
        private bool _isAnimating = false;

        // Helpers
        private BeepToggleLayoutHelper _layoutHelper;
        private BeepTogglePainterBase _painter;
        private bool _isApplyingTheme = false; // Prevent re-entrancy during theme application
        #endregion

        #region Constructor
        public BeepToggle()
        {
            // Initialize helpers
            _layoutHelper = new BeepToggleLayoutHelper(this);
            UpdatePainter();

            // Initialize animation timer
            _animationTimer = new Timer();
            _animationTimer.Interval = 16; // ~60 FPS
            _animationTimer.Tick += AnimationTimer_Tick;

            // Set default size
            Size = new Size(60, 28);
            MinimumSize = new Size(40, 20);

            // Enable double buffering for smooth rendering
            SetStyle(ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.ResizeRedraw, true);

            // Subscribe to mouse events for interaction
            Click += BeepToggle_Click;

            // Apply initial accessibility settings
            ApplyAccessibilitySettings();

            // Initialize tooltip if auto-generate is enabled
            if (AutoGenerateTooltip)
            {
                UpdateToggleTooltip();
            }
        }
        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets whether the toggle is ON
        /// </summary>
        [Category("Beep")]
        [Description("Gets or sets whether the toggle is ON")]
        [DefaultValue(false)]
        public bool IsOn
        {
            get => _isOn;
            set
            {
                if (_isOn != value)
                {
                    _isOn = value;
                    
                    // Update value based on IsOn state
                    UpdateValueFromIsOn();
                    
                    // Update accessibility settings when state changes
                    ApplyAccessibilitySettings();
                    
                    // Update tooltip text to reflect new state
                    UpdateToggleTooltip();
                    
                    OnIsOnChanged(EventArgs.Empty);
                    OnValueChanged(EventArgs.Empty);
                    
                    // Check if animations should be disabled (reduced motion)
                    bool shouldAnimate = _animateTransition && 
                        !ToggleAccessibilityHelpers.ShouldDisableAnimations(_animateTransition);
                    
                    if (shouldAnimate && IsHandleCreated)
                    {
                        StartAnimation();
                    }
                    else
                    {
                        _thumbPosition = _isOn ? 1f : 0f;
                        Invalidate();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the visual style of the toggle
        /// </summary>
        [Category("Beep")]
        [Description("Visual style of the toggle switch")]
        [DefaultValue(ToggleStyle.Classic)]
        public ToggleStyle ToggleStyle
        {
            get => _toggleStyle;
            set
            {
                if (_toggleStyle != value)
                {
                    _toggleStyle = value;
                    UpdatePainter();
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Color when toggle is ON
        /// </summary>
        [Category("Beep")]
        [Description("Color when toggle is ON")]
        public Color OnColor
        {
            get => _onColor;
            set { _onColor = value; Invalidate(); }
        }

        /// <summary>
        /// Color when toggle is OFF
        /// </summary>
        [Category("Beep")]
        [Description("Color when toggle is OFF")]
        public Color OffColor
        {
            get => _offColor;
            set { _offColor = value; Invalidate(); }
        }

        /// <summary>
        /// Thumb (slider) color
        /// </summary>
        [Category("Beep")]
        [Description("Color of the toggle thumb")]
        public Color ThumbColor
        {
            get => _thumbColor;
            set { _thumbColor = value; Invalidate(); }
        }

        /// <summary>
        /// Thumb color when ON
        /// </summary>
        [Category("Beep")]
        [Description("Thumb color when toggle is ON")]
        public Color OnThumbColor
        {
            get => _onThumbColor;
            set { _onThumbColor = value; Invalidate(); }
        }

        /// <summary>
        /// Thumb color when OFF
        /// </summary>
        [Category("Beep")]
        [Description("Thumb color when toggle is OFF")]
        public Color OffThumbColor
        {
            get => _offThumbColor;
            set { _offThumbColor = value; Invalidate(); }
        }

        /// <summary>
        /// Text to display when ON
        /// </summary>
        [Category("Beep")]
        [Description("Text to display when toggle is ON")]
        [DefaultValue("ON")]
        public string OnText
        {
            get => _onText;
            set 
            { 
                _onText = value; 
                // Update tooltip if auto-generate is enabled
                if (AutoGenerateTooltip)
                {
                    UpdateToggleTooltip();
                }
                Invalidate(); 
            }
        }

        /// <summary>
        /// Text to display when OFF
        /// </summary>
        [Category("Beep")]
        [Description("Text to display when toggle is OFF")]
        [DefaultValue("OFF")]
        public string OffText
        {
            get => _offText;
            set 
            { 
                _offText = value; 
                // Update tooltip if auto-generate is enabled
                if (AutoGenerateTooltip)
                {
                    UpdateToggleTooltip();
                }
                Invalidate(); 
            }
        }

        /// <summary>
        /// Icon path for ON state
        /// </summary>
        [Category("Beep")]
        [Description("Icon path for ON state")]
        public string OnIconPath
        {
            get => _onIconPath;
            set { _onIconPath = value; Invalidate(); }
        }

        /// <summary>
        /// Icon path for OFF state
        /// </summary>
        [Category("Beep")]
        [Description("Icon path for OFF state")]
        public string OffIconPath
        {
            get => _offIconPath;
            set { _offIconPath = value; Invalidate(); }
        }

        /// <summary>
        /// Show ON/OFF labels
        /// </summary>
        [Category("Beep")]
        [Description("Show ON/OFF text labels")]
        [DefaultValue(true)]
        public bool ShowLabels
        {
            get => _showLabels;
            set { _showLabels = value; Invalidate(); }
        }

        /// <summary>
        /// Enable smooth animation when toggling
        /// </summary>
        [Category("Beep")]
        [Description("Enable smooth animation when toggling")]
        [DefaultValue(true)]
        public bool AnimateTransition
        {
            get => _animateTransition;
            set => _animateTransition = value;
        }

        /// <summary>
        /// Animation duration in milliseconds
        /// </summary>
        [Category("Beep")]
        [Description("Animation duration in milliseconds")]
        [DefaultValue(200)]
        public int AnimationDuration
        {
            get => _animationDuration;
            set => _animationDuration = Math.Max(50, Math.Min(1000, value));
        }

        /// <summary>
        /// Current thumb position (0 to 1) for animation
        /// </summary>
        [Browsable(false)]
        public float ThumbPosition => _thumbPosition;

        #endregion

        #region Events

        /// <summary>
        /// Raised when IsOn property changes
        /// </summary>
        [Category("Beep")]
        [Description("Raised when toggle state changes")]
        public event EventHandler IsOnChanged;

        /// <summary>
        /// Raised when user toggles the switch
        /// </summary>
        [Category("Beep")]
        [Description("Raised when user toggles the switch")]
        public event EventHandler<ToggleEventArgs> Toggled;

        #endregion

        #region Protected Methods

        protected virtual void OnIsOnChanged(EventArgs e)
        {
            IsOnChanged?.Invoke(this, e);
            Toggled?.Invoke(this, new ToggleEventArgs(_isOn));
        }

        /// <summary>
        /// DrawContent override - called by BaseControl
        /// </summary>
        protected override void DrawContent(Graphics g)
        {
            Paint(g, DrawingRect);
        }

        /// <summary>
        /// Draw override - called by BeepGridPro and containers
        /// </summary>
        public override void Draw(Graphics graphics, Rectangle rectangle)
        {
            Paint(graphics, rectangle);
        }

        /// <summary>
        /// Main paint function - centralized painting logic
        /// Called from both DrawContent and Draw
        /// </summary>
        private void Paint(Graphics g, Rectangle bounds)
        {
            if (_painter == null)
                return;

            // Determine current control state
            ControlState state = ControlState.Normal;
            if (!Enabled)
                state = ControlState.Disabled;
            else if (IsPressed)
                state = ControlState.Pressed;
            else if (IsHovered)
                state = ControlState.Hovered;

            // Paint the toggle using the current painter
            _painter.Paint(g, bounds, state);
        }

        #endregion

        #region Private Methods

        private void UpdatePainter()
        {
            _painter = BeepTogglePainterFactory.CreatePainter(_toggleStyle, this, _layoutHelper);
        }

        private void BeepToggle_Click(object sender, EventArgs e)
        {
            if (!Enabled)
                return;

            // Toggle the state
            IsOn = !IsOn;
        }

        private void StartAnimation()
        {
            _animationStart = DateTime.Now;
            _isAnimating = true;
            _animationTimer.Start();
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            if (!_isAnimating)
            {
                _animationTimer.Stop();
                return;
            }

            var elapsed = (DateTime.Now - _animationStart).TotalMilliseconds;
            var progress = Math.Min(1f, (float)(elapsed / _animationDuration));

            // Use ToggleAnimationHelpers for consistent easing
            float easedProgress = ToggleAnimationHelpers.GetEasingFunction(_animationEasing)(progress);

            // Calculate thumb position using helper
            _thumbPosition = ToggleAnimationHelpers.CalculateThumbPosition(progress, _animationEasing, _isOn);

            // Update glow effect if enabled
            if (_enableFocusGlow && Focused)
            {
                UpdateGlowEffect();
            }

            Invalidate();

            if (progress >= 1f)
            {
                // Final position
                _thumbPosition = _isOn ? 1f : 0f;
                _isAnimating = false;
                _animationTimer.Stop();
                Invalidate();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _animationTimer?.Stop();
                _animationTimer?.Dispose();
            }
            base.Dispose(disposing);
        }

        #endregion

        #region Theme Integration

        /// <summary>
        /// Apply theme colors to the toggle
        /// Overrides BaseControl.ApplyTheme() to integrate with toggle-specific colors
        /// </summary>
        public override void ApplyTheme()
        {
            base.ApplyTheme();

            if (_isApplyingTheme) return;

            _isApplyingTheme = true;
            try
            {
                // Get current theme from BaseControl (set by ApplyTheme())
                var theme = _currentTheme ?? (UseThemeColors ? BeepThemesManager.CurrentTheme : null);
                var useTheme = UseThemeColors && theme != null;

                if (useTheme && theme != null)
                {
                    // Apply theme colors using ToggleThemeHelpers
                    ToggleThemeHelpers.ApplyThemeColors(this, theme, useTheme);
                }

                // Invalidate to redraw with new colors
                Invalidate();
            }
            finally
            {
                _isApplyingTheme = false;
            }
        }

        /// <summary>
        /// Apply theme colors when theme changes
        /// Called from BaseControl when theme is set
        /// </summary>
        public override void ApplyTheme(IBeepTheme theme)
        {
            base.ApplyTheme(theme);
            
            if (_isApplyingTheme) return;

            _isApplyingTheme = true;
            try
            {
                var useTheme = UseThemeColors && theme != null;

                if (useTheme && theme != null)
                {
                    // Apply theme colors
                    ToggleThemeHelpers.ApplyThemeColors(this, theme, useTheme);
                }

                // Apply font theme based on ControlStyle
                ToggleFontHelpers.ApplyFontTheme(this, ControlStyle);

                // Apply accessibility adjustments (high contrast, etc.)
                ApplyAccessibilityAdjustments(theme, useTheme);

                Invalidate();
            }
            finally
            {
                _isApplyingTheme = false;
            }
        }

        #endregion

        #region Tooltip Integration

        /// <summary>
        /// Gets or sets whether to auto-generate tooltip text based on toggle state
        /// When true, tooltip text is automatically generated from OnText/OffText
        /// </summary>
        [Category("Beep")]
        [Description("Auto-generate tooltip text based on toggle state")]
        [DefaultValue(false)]
        public bool AutoGenerateTooltip { get; set; } = false;

        /// <summary>
        /// Update tooltip text to reflect current toggle state
        /// Called automatically when IsOn changes
        /// </summary>
        private void UpdateToggleTooltip()
        {
            if (!EnableTooltip)
                return;

            // If auto-generate is enabled and no explicit tooltip text is set
            if (AutoGenerateTooltip && string.IsNullOrEmpty(TooltipText))
            {
                // Generate tooltip text from current state
                string stateText = _isOn ? _onText : _offText;
                string tooltipText = $"Toggle is currently {stateText}. Click to switch to {(_isOn ? _offText : _onText)}.";
                
                // Setting TooltipText will automatically call UpdateTooltip() via property setter
                if (TooltipText != tooltipText)
                {
                    TooltipText = tooltipText;
                }
            }
            // If explicit tooltip text is set, it will be updated automatically via TooltipText property
            // when the state changes, since BaseControl.UpdateTooltip() is called by the property setter
        }

        /// <summary>
        /// Set tooltip with toggle-specific content
        /// Automatically includes current state information
        /// Setting TooltipText will automatically trigger UpdateTooltip() via property setter
        /// </summary>
        public void SetToggleTooltip(string text, string title = null, ToolTipType type = ToolTipType.Default)
        {
            TooltipType = type; // Set type first
            if (!string.IsNullOrEmpty(title))
            {
                TooltipTitle = title; // Set title second
            }
            TooltipText = text; // Setting TooltipText last will trigger UpdateTooltip() automatically
        }

        /// <summary>
        /// Show a notification when toggle state changes
        /// Useful for providing feedback to users
        /// </summary>
        public void ShowToggleNotification(bool showOnChange = true)
        {
            if (!showOnChange)
                return;

            string message = _isOn 
                ? $"Turned {_onText}" 
                : $"Turned {_offText}";
            
            ToolTipType notificationType = _isOn ? ToolTipType.Success : ToolTipType.Info;
            
            ShowNotification(message, notificationType, 1500);
        }

        #endregion

        #region Accessibility Integration

        /// <summary>
        /// Apply accessibility settings to the toggle
        /// Sets ARIA attributes for screen readers
        /// </summary>
        private void ApplyAccessibilitySettings()
        {
            ToggleAccessibilityHelpers.ApplyAccessibilitySettings(
                this,
                _isOn,
                _onText,
                _offText);
        }

        /// <summary>
        /// Apply accessibility adjustments (high contrast, reduced motion, etc.)
        /// </summary>
        private void ApplyAccessibilityAdjustments(IBeepTheme theme = null, bool useThemeColors = false)
        {
            // Apply high contrast adjustments
            if (ToggleAccessibilityHelpers.IsHighContrastMode())
            {
                ToggleAccessibilityHelpers.ApplyHighContrastAdjustments(this, theme, useThemeColors);
            }

            // Disable animations if reduced motion is enabled
            if (ToggleAccessibilityHelpers.IsReducedMotionEnabled())
            {
                _animateTransition = false;
                EnableFocusGlow = false;
            }

            // Ensure minimum accessible size
            var accessibleMinSize = ToggleAccessibilityHelpers.GetAccessibleMinimumSize(MinimumSize);
            if (accessibleMinSize.Width > MinimumSize.Width || accessibleMinSize.Height > MinimumSize.Height)
            {
                MinimumSize = accessibleMinSize;
            }
        }

        #endregion
    }

    #region Event Args

    /// <summary>
    /// Event arguments for toggle state changes
    /// </summary>
    public class ToggleEventArgs : EventArgs
    {
        public bool IsOn { get; }

        public ToggleEventArgs(bool isOn)
        {
            IsOn = isOn;
        }
    }

    #endregion
}
