using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Styling.Effects
{
    /// <summary>
    /// Material-style ripple effect painter
    /// Creates animated circular ripple effects on button clicks
    /// </summary>
    public class RippleEffectPainter : IDisposable
    {
        private Timer _animationTimer;
        private RippleAnimation _currentRipple;
        private Control _targetControl;
        private bool _disposed = false;

        /// <summary>
        /// Initialize ripple effect painter for a control
        /// </summary>
        public RippleEffectPainter(Control targetControl)
        {
            _targetControl = targetControl ?? throw new ArgumentNullException(nameof(targetControl));
            _targetControl.Paint += TargetControl_Paint;
        }

        /// <summary>
        /// Trigger ripple effect at specified point
        /// </summary>
        /// <param name="centerPoint">Center point of ripple (typically mouse click location)</param>
        /// <param name="rippleColor">Ripple color (typically primary color)</param>
        /// <param name="duration">Animation duration in milliseconds (default 400ms)</param>
        public void TriggerRipple(Point centerPoint, Color rippleColor, int duration = 400)
        {
            if (_currentRipple != null && _currentRipple.IsAnimating)
            {
                _currentRipple.Stop();
            }

            _currentRipple = new RippleAnimation
            {
                CenterPoint = centerPoint,
                RippleColor = rippleColor,
                Duration = duration,
                StartTime = DateTime.Now
            };

            // Start animation timer
            if (_animationTimer == null)
            {
                _animationTimer = new Timer();
                _animationTimer.Interval = 16; // ~60 FPS
                _animationTimer.Tick += AnimationTimer_Tick;
            }

            _animationTimer.Start();
            _targetControl.Invalidate();
        }

        /// <summary>
        /// Animation timer tick handler
        /// </summary>
        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            if (_currentRipple == null || !_currentRipple.IsAnimating)
            {
                _animationTimer?.Stop();
                return;
            }

            _currentRipple.Update();
            _targetControl.Invalidate();

            if (!_currentRipple.IsAnimating)
            {
                _animationTimer.Stop();
            }
        }

        /// <summary>
        /// Paint ripple effect
        /// </summary>
        private void TargetControl_Paint(object sender, PaintEventArgs e)
        {
            if (_currentRipple != null && _currentRipple.IsAnimating)
            {
                PaintRipple(e.Graphics, _currentRipple);
            }
        }

        /// <summary>
        /// Paint a single ripple
        /// </summary>
        private void PaintRipple(Graphics g, RippleAnimation ripple)
        {
            if (g == null || ripple == null) return;

            float progress = ripple.Progress;
            if (progress <= 0 || progress >= 1) return;

            // Calculate ripple radius (expands from 0 to max)
            float maxRadius = Math.Max(_targetControl.Width, _targetControl.Height) * 1.5f;
            float currentRadius = maxRadius * progress;

            // Calculate opacity (fades out as ripple expands)
            int alpha = (int)(255 * (1.0f - progress));
            alpha = Math.Max(0, Math.Min(255, alpha));

            if (alpha <= 0) return;

            // Create ripple circle
            using (var path = new GraphicsPath())
            {
                RectangleF rippleBounds = new RectangleF(
                    ripple.CenterPoint.X - currentRadius,
                    ripple.CenterPoint.Y - currentRadius,
                    currentRadius * 2,
                    currentRadius * 2
                );

                path.AddEllipse(rippleBounds);

                // Use radial gradient for smoother ripple
                using (var brush = new PathGradientBrush(path))
                {
                    brush.CenterColor = Color.FromArgb(alpha, ripple.RippleColor);
                    brush.SurroundColors = new[] { Color.Transparent };
                    brush.CenterPoint = ripple.CenterPoint;

                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.FillPath(brush, path);
                }
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                if (_animationTimer != null)
                {
                    _animationTimer.Stop();
                    _animationTimer.Dispose();
                    _animationTimer = null;
                }

                if (_targetControl != null)
                {
                    _targetControl.Paint -= TargetControl_Paint;
                }

                _currentRipple = null;
                _disposed = true;
            }
        }
    }

    /// <summary>
    /// Ripple animation state
    /// </summary>
    internal class RippleAnimation
    {
        public Point CenterPoint { get; set; }
        public Color RippleColor { get; set; }
        public int Duration { get; set; }
        public DateTime StartTime { get; set; }

        public bool IsAnimating
        {
            get
            {
                var elapsed = (DateTime.Now - StartTime).TotalMilliseconds;
                return elapsed < Duration;
            }
        }

        public float Progress
        {
            get
            {
                if (!IsAnimating) return 1.0f;
                var elapsed = (DateTime.Now - StartTime).TotalMilliseconds;
                return (float)(elapsed / Duration);
            }
        }

        public void Update()
        {
            // Animation state is calculated on-demand via Progress property
        }

        public void Stop()
        {
            // Mark as complete by setting start time far in the past
            StartTime = DateTime.Now.AddMilliseconds(-Duration - 1);
        }
    }
}
