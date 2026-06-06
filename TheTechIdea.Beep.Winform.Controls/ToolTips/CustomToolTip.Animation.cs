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
        /// C9: Uses the existing <c>_animationTimer</c> on the UI thread with
        /// a TaskCompletionSource to bridge to async/await, replacing the
        /// previous per-frame <c>Task.Delay(16)</c> loop.
        /// </summary>
        private async Task AnimateInAsync()
        {
            _isAnimatingIn = true;
            _isAnimatingOut = false;
            _animationProgress = 0;

            // C2: AnimateOut leaves Opacity = 0; reset to 1.0 first so the
            // first frame is visible even if the user re-shows the tooltip
            // before the Fade/Scale/Start overrides below execute.
            Opacity = 1.0;

            if (_config.Animation == ToolTipAnimation.None)
            {
                _isAnimatingIn = false;
                return;
            }

            // Capture the start state for Slide animation
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

            // C9: Start the timer; OnAnimationTick will advance _animationProgress
            // and complete the TCS when progress reaches 1.0.
            _anchorStartLocation = startLocation;
            _animationTcs     = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            _animationStartTime = DateTime.Now;
            _animationTimer.Start();

            await _animationTcs.Task;

            // Ensure final state
            Opacity = 1.0;
            Location = targetLocation;
            _animationProgress = 1.0;
            _isAnimatingIn = false;
            Invalidate();
        }

        /// <summary>
        /// Animate tooltip out based on animation type
        /// C9: Same timer-driven pattern as AnimateInAsync.
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

            // C9: Start the timer with reverse-progress logic in OnAnimationTick.
            _anchorStartLocation = Location;
            _animationTcs     = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            _animationStartTime = DateTime.Now;
            _animationTimer.Start();

            await _animationTcs.Task;

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
        /// C9: Animation timer tick handler. The old version only called
        /// Invalidate(); the per-frame state work was in the Task.Delay
        /// loops. Now the timer drives both the repaint AND the per-frame
        /// state updates, then signals the TCS when the animation finishes.
        /// </summary>
        private void OnAnimationTick(object sender, EventArgs e)
        {
            if (_config == null)
            {
                _animationTimer.Stop();
                return;
            }

            if (_isAnimatingIn)
            {
                var duration = _config.AnimationDuration;
                if (duration <= 0) duration = 1;
                var elapsed = (DateTime.Now - _animationStartTime).TotalMilliseconds;
                var progress = Math.Min(1.0, elapsed / duration);
                var easedProgress = ApplyEasing(progress, _config.Animation);
                _animationProgress = easedProgress;

                switch (_config.Animation)
                {
                    case ToolTipAnimation.Fade:
                    case ToolTipAnimation.Scale:
                    case ToolTipAnimation.Bounce:
                        Opacity = easedProgress;
                        break;

                    case ToolTipAnimation.Slide:
                        var slideOffset = GetSlideOffset(_actualPlacement);
                        var targetX = _anchorStartLocation.X + (int)(slideOffset.X * (1.0 - easedProgress));
                        var targetY = _anchorStartLocation.Y + (int)(slideOffset.Y * (1.0 - easedProgress));
                        Location = new Point(targetX, targetY);
                        Opacity = easedProgress;
                        break;
                }

                Invalidate();

                if (progress >= 1.0)
                {
                    _animationTimer.Stop();
                    _animationTcs?.TrySetResult(true);
                }
            }
            else if (_isAnimatingOut)
            {
                var duration = _config.AnimationDuration / 2; // Faster fade out
                if (duration <= 0) duration = 1;
                var elapsed = (DateTime.Now - _animationStartTime).TotalMilliseconds;
                var progress = Math.Min(1.0, elapsed / duration);
                var easedProgress = 1.0 - progress; // Reverse progress
                _animationProgress = easedProgress;

                switch (_config.Animation)
                {
                    case ToolTipAnimation.Fade:
                    case ToolTipAnimation.Scale:
                    case ToolTipAnimation.Bounce:
                        Opacity = easedProgress;
                        break;

                    case ToolTipAnimation.Slide:
                        var offset = GetSlideOffset(_actualPlacement);
                        var x = _anchorStartLocation.X + (int)(offset.X * progress);
                        var y = _anchorStartLocation.Y + (int)(offset.Y * progress);
                        Location = new Point(x, y);
                        Opacity = easedProgress;
                        break;
                }

                Invalidate();

                if (progress >= 1.0)
                {
                    _animationTimer.Stop();
                    _animationTcs?.TrySetResult(true);
                }
            }
        }

        #endregion
    }
}
