using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.ToolTips.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.ToolTips
{
    public partial class CustomToolTip
    {
        #region Animation Methods

        /// <summary>
        /// Animate tooltip in based on animation type
        /// Uses ToolTipAnimationHelpers for enhanced easing
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

                // Apply easing using ToolTipAnimationHelpers
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
        /// Uses ToolTipAnimationHelpers for enhanced easing
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
    }
}
