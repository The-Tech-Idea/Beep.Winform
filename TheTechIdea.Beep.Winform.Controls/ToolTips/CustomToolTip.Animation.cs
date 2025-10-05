using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.ToolTips.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.ToolTips
{
    /// <summary>
    /// Animation partial class for CustomToolTip
    /// </summary>
    public partial class CustomToolTip
    {
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
                Show();
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
                    Show();
                    break;

                case ToolTipAnimation.Scale:
                    // Start smaller
                    var originalSize = Size;
                    Size = new Size((int)(Size.Width * 0.8), (int)(Size.Height * 0.8));
                    Opacity = 0;
                    Show();
                    break;

                case ToolTipAnimation.Slide:
                    // Start offset based on placement
                    var offset = GetSlideOffset(_actualPlacement);
                    startLocation = new Point(Location.X + offset.X, Location.Y + offset.Y);
                    Location = startLocation;
                    Opacity = 0;
                    Show();
                    break;

                case ToolTipAnimation.Bounce:
                    Opacity = 0;
                    Show();
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
                        var currentScale = 0.8 + (0.2 * easedProgress);
                        var newSize = new Size(
                            (int)(Size.Width / currentScale),
                            (int)(Size.Height / currentScale)
                        );
                        if (newSize != Size)
                        {
                            var originalSize = Size;
                            Size = newSize;
                            // Adjust position to scale from center
                            Location = new Point(
                                Location.X - (Size.Width - originalSize.Width) / 2,
                                Location.Y - (Size.Height - originalSize.Height) / 2
                            );
                        }
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
                Hide();
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
                        var currentScale = 0.8 + (0.2 * easedProgress);
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

        #endregion

        #region Animation Helpers

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
    }
}
