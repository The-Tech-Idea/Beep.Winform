using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;

using TheTechIdea.Beep.Winform.Controls.ToolTips.Painters;
using TheTechIdea.Beep.Winform.Controls.ToolTips.Helpers;
using TheTechIdea.Beep.Winform.Controls.Forms.ModernForm;

namespace TheTechIdea.Beep.Winform.Controls.ToolTips
{
    /// <summary>
    /// Modern tooltip form that inherits from BeepiFormPro for consistent theming
    /// Consolidates all functionality (Main, Animation, Drawing) into single file
    /// Matches BeepNotification architecture pattern
    /// </summary>
    public class CustomToolTip : BeepiFormPro
    {
        #region Constants

        private const int DefaultArrowSize = 8;

        #endregion

        #region Fields

        private ToolTipConfig _config;
        private IBeepTheme _theme;
        private IBeepTheme _currentTheme; // Theme from ApplyTheme() - highest priority
        private ToolTipPlacement _actualPlacement;
        private IToolTipPainter _painter;
        private bool _isApplyingTheme = false;

        // Animation state
        private bool _isAnimatingIn;
        private bool _isAnimatingOut;
        private double _animationProgress;
        private Timer _animationTimer;

        #endregion

        #region Constructor

        public CustomToolTip()
        {
            // Initialize base form properties (like BeepNotification)
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.Manual;
            ShowInTaskbar = false;
            TopMost = true;
            ShowCaptionBar = false; // BeepiFormPro property

            // Additional tooltip-specific properties
            BackColor = Color.FromArgb(45, 45, 48); // Default dark tooltip
            ForeColor = Color.White;
           
            DoubleBuffered = true;

            // Initialize animation timer
            _animationTimer = new Timer();
            _animationTimer.Interval = 16; // ~60 FPS
            _animationTimer.Tick += OnAnimationTick;

            // Set default theme from BeepThemesManager
            _theme = BeepThemesManager.DefaultTheme;

            // Set accessibility properties for screen readers
            SetAccessibilityProperties();
        }

        /// <summary>
        /// Set accessibility properties for screen readers
        /// </summary>
        private void SetAccessibilityProperties()
        {
            // Set accessible role
            AccessibleRole = AccessibleRole.ToolTip;
            AccessibleName = "Tooltip";
            AccessibleDescription = "Additional information tooltip";
        }

        /// <summary>
        /// Update accessibility properties with tooltip content
        /// </summary>
        private void UpdateAccessibilityProperties()
        {
            if (_config == null) return;

            // Build accessible description from tooltip content
            var description = new System.Text.StringBuilder();

            if (!string.IsNullOrEmpty(_config.Title))
            {
                description.Append(_config.Title);
                if (!string.IsNullOrEmpty(_config.Text))
                {
                    description.Append(". ");
                }
            }

            if (!string.IsNullOrEmpty(_config.Text))
            {
                description.Append(_config.Text);
            }

            if (description.Length > 0)
            {
                AccessibleDescription = description.ToString();
                AccessibleName = !string.IsNullOrEmpty(_config.Title) 
                    ? _config.Title 
                    : "Tooltip";
            }

            // Set tooltip type for screen readers
            var typeDescription = _config.Type switch
            {
                ToolTipType.Success => "Success message",
                ToolTipType.Warning => "Warning message",
                ToolTipType.Error => "Error message",
                ToolTipType.Info => "Information",
                ToolTipType.Help => "Help information",
                _ => "Tooltip"
            };

            if (!string.IsNullOrEmpty(AccessibleDescription))
            {
                AccessibleDescription = $"{typeDescription}: {AccessibleDescription}";
            }
            else
            {
                AccessibleDescription = typeDescription;
            }
        }

        /// <summary>
        /// Apply accessibility enhancements (high contrast, contrast ratios, etc.)
        /// </summary>
        private void ApplyAccessibilityEnhancements()
        {
            if (_config == null) return;

            // Check high contrast mode
            if (ToolTipAccessibilityHelpers.IsHighContrastMode())
            {
                var (backColor, foreColor, borderColor) = ToolTipAccessibilityHelpers.GetHighContrastColors();
                
                // Override colors with high contrast system colors
                if (!_config.BackColor.HasValue)
                    _config.BackColor = backColor;
                if (!_config.ForeColor.HasValue)
                    _config.ForeColor = foreColor;
                if (!_config.BorderColor.HasValue)
                    _config.BorderColor = borderColor;
            }
            else
            {
                // Ensure contrast ratios meet WCAG AA standards
                var backColor = _config.BackColor ?? BackColor;
                var foreColor = _config.ForeColor ?? ForeColor;
                var borderColor = _config.BorderColor ?? Color.Gray;

                var accessibleColors = ToolTipAccessibilityHelpers.GetAccessibleColors(
                    backColor, foreColor, borderColor);

                if (!_config.BackColor.HasValue)
                    _config.BackColor = accessibleColors.backColor;
                if (!_config.ForeColor.HasValue)
                    _config.ForeColor = accessibleColors.foreColor;
                if (!_config.BorderColor.HasValue)
                    _config.BorderColor = accessibleColors.borderColor;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Current tooltip configuration
        /// </summary>
        public ToolTipConfig Config => _config;

        /// <summary>
        /// Current theme for rendering
        /// Note: Use ApplyTheme() to set theme from BaseControl pattern
        /// </summary>
        public IBeepTheme Theme
        {
            get => _currentTheme ?? _theme;
            set
            {
                _theme = value;
                if (_currentTheme == null)
                {
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Apply theme colors from ApplyTheme() pattern
        /// This is the preferred method for theme integration
        /// </summary>
        public void ApplyTheme(IBeepTheme theme, bool useThemeColors = true)
        {
            if (_isApplyingTheme) return;

            _isApplyingTheme = true;
            try
            {
                _currentTheme = theme;
                
                // Apply theme colors to config if available
                if (_config != null && theme != null)
                {
                    ToolTipThemeHelpers.ApplyThemeColors(_config, theme, useThemeColors);
                    
                    // Update form colors
                    if (!_config.BackColor.HasValue)
                    {
                        BackColor = ToolTipThemeHelpers.GetToolTipBackColor(theme, _config.Type, useThemeColors);
                    }
                    
                    if (!_config.ForeColor.HasValue)
                    {
                        ForeColor = ToolTipThemeHelpers.GetToolTipForeColor(theme, _config.Type, useThemeColors);
                    }
                }

                // Trigger repaint
                Invalidate();
            }
            finally
            {
                _isApplyingTheme = false;
            }
        }

        /// <summary>
        /// Tooltip painter (defaults to BeepStyledToolTipPainter)
        /// </summary>
        public IToolTipPainter Painter
        {
            get => _painter;
            set
            {
                _painter = value;
                Invalidate();
            }
        }

        #endregion

        #region Public Methods - Lifecycle

        /// <summary>
        /// Apply tooltip configuration and prepare for display
        /// </summary>
        public void ApplyConfig(ToolTipConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));

            // Initialize painter if not already set
            _painter ??= new BeepStyledToolTipPainter();

            // Apply theme - use _currentTheme if set (from ApplyTheme()), otherwise use BeepThemesManager
            if (_currentTheme != null)
            {
                _theme = _currentTheme;
            }
            else if (_config.UseBeepThemeColors && BeepThemesManager.CurrentTheme != null)
            {
                _theme = BeepThemesManager.CurrentTheme;
            }

            // Apply theme colors to config
            if (_config.UseBeepThemeColors && _theme != null)
            {
                ToolTipThemeHelpers.ApplyThemeColors(_config, _theme, true);
            }

            // Apply accessibility enhancements (high contrast, contrast ratios)
            ApplyAccessibilityEnhancements();

            // Update accessibility properties with tooltip content
            UpdateAccessibilityProperties();

            // Calculate tooltip size with responsive sizing
            using (var g = CreateGraphics())
            {
                var contentSize = _painter.CalculateSize(g, _config);
                
                // Apply responsive sizing based on screen size
                var screenBounds = ToolTipPositioningHelpers.GetScreenBounds(_config.Position);
                var maxSize = _config.MaxSize ?? new Size(0, 0); // 0 means use default
                var minSize = new Size(120, 40); // Minimum readable size
                
                var responsiveSize = ToolTipPositioningHelpers.CalculateResponsiveSize(
                    contentSize, maxSize, minSize, screenBounds);
                
                Size = responsiveSize;
            }

            // Apply custom colors if specified
            if (_config.BackColor.HasValue)
            {
                BackColor = _config.BackColor.Value;
            }

            if (_config.ForeColor.HasValue)
            {
                ForeColor = _config.ForeColor.Value;
            }

            Invalidate();
        }

        /// <summary>
        /// Show tooltip at specified position with animation
        /// </summary>
        public async Task ShowAsync(Point position, CancellationToken cancellationToken = default)
        {
            if (_config == null)
            {
                throw new InvalidOperationException("Must call ApplyConfig before showing tooltip");
            }

            // Use smart positioning with collision detection
            var targetRect = new Rectangle(position, new Size(1, 1));
            var (placement, finalPosition) = ToolTipPositioningHelpers.FindBestPlacement(
                targetRect, Size, _config.Placement, _config.Offset);
            _actualPlacement = placement;

            Location = finalPosition;

            // Show and animate (respect reduced motion preference)
            Show();

            // Check if animations should be disabled for accessibility
            var shouldAnimate = _config.Animation != ToolTipAnimation.None && 
                               !ToolTipAccessibilityHelpers.ShouldDisableAnimations(_config.Animation);

            if (shouldAnimate)
            {
                await AnimateInAsync();
            }
            else
            {
                Opacity = 1.0;
            }
        }

        /// <summary>
        /// Hide tooltip with animation
        /// </summary>
        public async Task HideAsync()
        {
            // Check if animations should be disabled for accessibility
            var shouldAnimate = _config?.Animation != ToolTipAnimation.None && 
                               !ToolTipAccessibilityHelpers.ShouldDisableAnimations(_config.Animation);

            if (shouldAnimate)
            {
                await AnimateOutAsync();
            }
            else
            {
                Opacity = 0;
            }

            Hide();
        }

        /// <summary>
        /// Update tooltip position (for follow-cursor scenarios)
        /// </summary>
        public void UpdatePosition(Point newPosition)
        {
            var adjustedPosition = AdjustPositionForPlacement(newPosition, _actualPlacement);
            var constrainedPosition = ConstrainToScreen(adjustedPosition);
            Location = constrainedPosition;
        }

        ///// <summary>
        ///// Apply accessibility enhancements (high contrast, contrast ratios, etc.)
        ///// </summary>
        //private void ApplyAccessibilityEnhancements()
        //{
        //    if (_config == null) return;

        //    // Check high contrast mode
        //    if (ToolTipAccessibilityHelpers.IsHighContrastMode())
        //    {
        //        var (backColor, foreColor, borderColor) = ToolTipAccessibilityHelpers.GetHighContrastColors();
                
        //        // Override colors with high contrast system colors
        //        if (!_config.BackColor.HasValue)
        //            _config.BackColor = backColor;
        //        if (!_config.ForeColor.HasValue)
        //            _config.ForeColor = foreColor;
        //        if (!_config.BorderColor.HasValue)
        //            _config.BorderColor = borderColor;
        //    }
        //    else
        //    {
        //        // Ensure contrast ratios meet WCAG AA standards
        //        var backColor = _config.BackColor ?? BackColor;
        //        var foreColor = _config.ForeColor ?? ForeColor;
        //        var borderColor = _config.BorderColor ?? Color.Gray;

        //        var accessibleColors = ToolTipAccessibilityHelpers.GetAccessibleColors(
        //            backColor, foreColor, borderColor);

        //        if (!_config.BackColor.HasValue)
        //            _config.BackColor = accessibleColors.backColor;
        //        if (!_config.ForeColor.HasValue)
        //            _config.ForeColor = accessibleColors.foreColor;
        //        if (!_config.BorderColor.HasValue)
        //            _config.BorderColor = accessibleColors.borderColor;
        //    }
        //}

        #endregion

        #region Animation Methods

        /// <summary>
        /// Animate tooltip in based on animation type
        /// </summary>
        private async Task AnimateInAsync()
        {
            _isAnimatingIn = true;
            _isAnimatingOut = false;
            _animationProgress = 0;

            if (_config.Animation == ToolTipAnimation.None)
            {
                Opacity = 1.0;
                _isAnimatingIn = false;
                return;
            }

            var duration = _config.AnimationDuration;
            var startTime = DateTime.Now;
            var startLocation = Location;
            var targetLocation = Location;

            // Calculate animation offsets based on animation type
            switch (_config.Animation)
            {
                case ToolTipAnimation.Fade:
                    Opacity = 0;
                    break;

                case ToolTipAnimation.Scale:
                    // Start smaller
                    var originalSize = Size;
                    Size = new Size((int)(Size.Width * 0.8), (int)(Size.Height * 0.8));
                    Opacity = 0;
                    break;

                case ToolTipAnimation.Slide:
                    // Start offset based on placement
                    var offset = GetSlideOffset(_actualPlacement);
                    startLocation = new Point(Location.X + offset.X, Location.Y + offset.Y);
                    Location = startLocation;
                    Opacity = 0;
                    break;

                case ToolTipAnimation.Bounce:
                    Opacity = 0;
                    break;
            }

            // Animation loop
            while ((DateTime.Now - startTime).TotalMilliseconds < duration)
            {
                var elapsed = (DateTime.Now - startTime).TotalMilliseconds;
                var progress = Math.Min(1.0, elapsed / duration);

                // Apply easing
                var easedProgress = ApplyEasing(progress, _config.Animation);
                _animationProgress = easedProgress;

                // Update based on animation type
                switch (_config.Animation)
                {
                    case ToolTipAnimation.Fade:
                        Opacity = easedProgress;
                        break;

                    case ToolTipAnimation.Scale:
                        Opacity = easedProgress;
                        break;

                    case ToolTipAnimation.Slide:
                        var slideOffset = GetSlideOffset(_actualPlacement);
                        Location = new Point(
                            startLocation.X - (int)(slideOffset.X * easedProgress),
                            startLocation.Y - (int)(slideOffset.Y * easedProgress)
                        );
                        Opacity = easedProgress;
                        break;

                    case ToolTipAnimation.Bounce:
                        Opacity = easedProgress;
                        break;
                }

                Invalidate();
                await Task.Delay(16); // ~60 FPS
            }

            // Ensure final state
            Opacity = 1.0;
            Location = targetLocation;
            _animationProgress = 1.0;
            _isAnimatingIn = false;
            Invalidate();
        }

        /// <summary>
        /// Animate tooltip out based on animation type
        /// </summary>
        private async Task AnimateOutAsync()
        {
            _isAnimatingOut = true;
            _isAnimatingIn = false;
            _animationProgress = 1.0;

            if (_config.Animation == ToolTipAnimation.None)
            {
                _isAnimatingOut = false;
                return;
            }

            var duration = _config.AnimationDuration / 2; // Faster fade out
            var startTime = DateTime.Now;
            var startLocation = Location;

            // Animation loop
            while ((DateTime.Now - startTime).TotalMilliseconds < duration)
            {
                var elapsed = (DateTime.Now - startTime).TotalMilliseconds;
                var progress = Math.Min(1.0, elapsed / duration);
                var easedProgress = 1.0 - progress; // Reverse progress

                _animationProgress = easedProgress;

                // Update based on animation type
                switch (_config.Animation)
                {
                    case ToolTipAnimation.Fade:
                        Opacity = easedProgress;
                        break;

                    case ToolTipAnimation.Scale:
                        Opacity = easedProgress;
                        break;

                    case ToolTipAnimation.Slide:
                        var offset = GetSlideOffset(_actualPlacement);
                        Location = new Point(
                            startLocation.X + (int)(offset.X * progress),
                            startLocation.Y + (int)(offset.Y * progress)
                        );
                        Opacity = easedProgress;
                        break;

                    case ToolTipAnimation.Bounce:
                        Opacity = easedProgress;
                        break;
                }

                Invalidate();
                await Task.Delay(16); // ~60 FPS
            }

            _animationProgress = 0;
            _isAnimatingOut = false;
            Opacity = 0;
        }

        /// <summary>
        /// Get slide animation offset based on placement
        /// </summary>
        private Point GetSlideOffset(ToolTipPlacement placement)
        {
            const int slideDistance = 20;

            return placement switch
            {
                ToolTipPlacement.Top or ToolTipPlacement.TopStart or ToolTipPlacement.TopEnd => new Point(0, slideDistance),
                ToolTipPlacement.Bottom or ToolTipPlacement.BottomStart or ToolTipPlacement.BottomEnd => new Point(0, -slideDistance),
                ToolTipPlacement.Left or ToolTipPlacement.LeftStart or ToolTipPlacement.LeftEnd => new Point(slideDistance, 0),
                ToolTipPlacement.Right or ToolTipPlacement.RightStart or ToolTipPlacement.RightEnd => new Point(-slideDistance, 0),
                _ => new Point(0, slideDistance)
            };
        }

        /// <summary>
        /// Apply easing function to animation progress
        /// Now uses ToolTipAnimationHelpers for enhanced easing
        /// </summary>
        private double ApplyEasing(double progress, ToolTipAnimation animation)
        {
            var easingFunc = ToolTipAnimationHelpers.GetEasingFunction(animation);
            return easingFunc(progress);
        }

        /// <summary>
        /// Animation timer tick handler
        /// </summary>
        private void OnAnimationTick(object sender, EventArgs e)
        {
            if (_isAnimatingIn || _isAnimatingOut)
            {
                Invalidate();
            }
        }

        #endregion

        #region Positioning Methods

        /// <summary>
        /// Calculate optimal placement based on available screen space
        /// Now uses ToolTipPositioningHelpers for smart collision detection
        /// </summary>
        private ToolTipPlacement CalculatePlacement(Point targetPosition)
        {
            if (_config.Placement != ToolTipPlacement.Auto)
            {
                // Still validate that preferred placement fits
                var targetRect = new Rectangle(targetPosition, new Size(1, 1));
                var tooltipSize = Size;
                var placement = ToolTipPositioningHelpers.CalculateOptimalPlacement(
                    targetRect, tooltipSize, _config.Placement, _config.Offset);
                return placement;
            }

            // Use smart positioning helper
            var targetRectForCalc = new Rectangle(targetPosition, new Size(1, 1));
            return ToolTipPositioningHelpers.CalculateOptimalPlacement(
                targetRectForCalc, Size, ToolTipPlacement.Auto, _config.Offset);
        }

        /// <summary>
        /// Adjust position based on placement
        /// </summary>
        private Point AdjustPositionForPlacement(Point targetPosition, ToolTipPlacement placement)
        {
            int arrowSize = _config.ShowArrow ? DefaultArrowSize : 0;
            int offset = _config.Offset;

            return placement switch
            {
                ToolTipPlacement.Top => new Point(
                    targetPosition.X - Width / 2,
                    targetPosition.Y - Height - arrowSize - offset
                ),
                ToolTipPlacement.Bottom => new Point(
                    targetPosition.X - Width / 2,
                    targetPosition.Y + arrowSize + offset
                ),
                ToolTipPlacement.Left => new Point(
                    targetPosition.X - Width - arrowSize - offset,
                    targetPosition.Y - Height / 2
                ),
                ToolTipPlacement.Right => new Point(
                    targetPosition.X + arrowSize + offset,
                    targetPosition.Y - Height / 2
                ),
                ToolTipPlacement.TopStart => new Point(
                    targetPosition.X,
                    targetPosition.Y - Height - arrowSize - offset
                ),
                ToolTipPlacement.TopEnd => new Point(
                    targetPosition.X - Width,
                    targetPosition.Y - Height - arrowSize - offset
                ),
                ToolTipPlacement.BottomStart => new Point(
                    targetPosition.X,
                    targetPosition.Y + arrowSize + offset
                ),
                ToolTipPlacement.BottomEnd => new Point(
                    targetPosition.X - Width,
                    targetPosition.Y + arrowSize + offset
                ),
                _ => new Point(targetPosition.X - Width / 2, targetPosition.Y + arrowSize + offset)
            };
        }

        /// <summary>
        /// Constrain tooltip position to stay within screen bounds
        /// Now uses ToolTipPositioningHelpers for better edge handling
        /// </summary>
        private Point ConstrainToScreen(Point position)
        {
            var tooltipBounds = new Rectangle(position, Size);
            return ToolTipPositioningHelpers.AdjustForScreenEdges(tooltipBounds, position);
        }

        #endregion

        #region Drawing Methods

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            
            // Use _currentTheme if available (from ApplyTheme()), otherwise use _theme
            var activeTheme = _currentTheme ?? _theme;

            if (_config == null || _painter == null)
                return;

            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // Get the actual painting bounds
            var bounds = new Rectangle(0, 0, ClientSize.Width, ClientSize.Height);

            // Use painter to render the tooltip with active theme (from ApplyTheme() if available)
            _painter.Paint(g, bounds, _config, _actualPlacement, activeTheme);

            // Apply animation opacity
            if (_isAnimatingIn || _isAnimatingOut)
            {
                Opacity = _animationProgress;
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // Fill with transparency key color for rounded corners
            using (var brush = new SolidBrush(TransparencyKey))
            {
                e.Graphics.FillRectangle(brush, ClientRectangle);
            }
        }

        #endregion

        #region Cleanup

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
    }
}
