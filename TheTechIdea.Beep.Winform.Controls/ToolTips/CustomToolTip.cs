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
        private ToolTipPlacement _actualPlacement;
        private IToolTipPainter _painter;

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
        }

        #endregion

        #region Properties

        /// <summary>
        /// Current tooltip configuration
        /// </summary>
        public ToolTipConfig Config => _config;

        /// <summary>
        /// Current theme for rendering
        /// </summary>
        public IBeepTheme Theme
        {
            get => _theme;
            set
            {
                _theme = value;
                Invalidate();
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

            // Apply theme
            if (_config.UseBeepThemeColors && BeepThemesManager.CurrentTheme != null)
            {
                _theme = BeepThemesManager.CurrentTheme;
            }

            // Calculate tooltip size
            using (var g = CreateGraphics())
            {
                var size = _painter.CalculateSize(g, _config);
                Size = size;
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

            // Calculate optimal placement and position
            _actualPlacement = CalculatePlacement(position);
            var finalPosition = AdjustPositionForPlacement(position, _actualPlacement);

            // Apply screen bounds constraints
            finalPosition = ConstrainToScreen(finalPosition);

            Location = finalPosition;

            // Show and animate
            Show();

            if (_config.Animation != ToolTipAnimation.None)
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
            if (_config?.Animation != ToolTipAnimation.None)
            {
                await AnimateOutAsync();
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
        /// </summary>
        private double ApplyEasing(double progress, ToolTipAnimation animation)
        {
            return animation switch
            {
                ToolTipAnimation.Fade => ToolTipHelpers.EaseOutCubic(progress),
                ToolTipAnimation.Scale => ToolTipHelpers.EaseOutCubic(progress),
                ToolTipAnimation.Slide => ToolTipHelpers.EaseOutCubic(progress),
                ToolTipAnimation.Bounce => ToolTipHelpers.EaseBounce(progress),
                _ => progress
            };
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
        /// </summary>
        private ToolTipPlacement CalculatePlacement(Point targetPosition)
        {
            if (_config.Placement != ToolTipPlacement.Auto)
            {
                return _config.Placement;
            }

            var screen = Screen.FromPoint(targetPosition);
            var screenBounds = screen.WorkingArea;

            // Calculate available space in each direction
            int spaceAbove = targetPosition.Y - screenBounds.Top;
            int spaceBelow = screenBounds.Bottom - targetPosition.Y;
            int spaceLeft = targetPosition.X - screenBounds.Left;
            int spaceRight = screenBounds.Right - targetPosition.X;

            // Choose placement with most available space
            if (spaceBelow >= Height || spaceBelow >= spaceAbove)
            {
                return ToolTipPlacement.Bottom;
            }
            else if (spaceAbove >= Height)
            {
                return ToolTipPlacement.Top;
            }
            else if (spaceRight >= Width)
            {
                return ToolTipPlacement.Right;
            }
            else
            {
                return ToolTipPlacement.Left;
            }
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
        /// </summary>
        private Point ConstrainToScreen(Point position)
        {
            var screen = Screen.FromPoint(position);
            var bounds = screen.WorkingArea;

            int x = Math.Max(bounds.Left + 10, Math.Min(position.X, bounds.Right - Width - 10));
            int y = Math.Max(bounds.Top + 10, Math.Min(position.Y, bounds.Bottom - Height - 10));

            return new Point(x, y);
        }

        #endregion

        #region Drawing Methods

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (_config == null || _painter == null)
                return;

            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // Get the actual painting bounds
            var bounds = new Rectangle(0, 0, ClientSize.Width, ClientSize.Height);

            // Use painter to render the tooltip
            _painter.Paint(g, bounds, _config, _actualPlacement, _theme);

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
