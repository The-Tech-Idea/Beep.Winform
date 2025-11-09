using System;
using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Notifications
{
    /// <summary>
    /// Handles animations for notification show/hide
    /// Supports: Slide, Fade, SlideAndFade, Bounce, Scale
    /// </summary>
    internal class BeepNotificationAnimator
    {
        private const int ANIMATION_DURATION = 300; // milliseconds
        private const int ANIMATION_FPS = 60;
        private Timer _animationTimer;
        private DateTime _animationStart;
        private NotificationAnimation _animationType;
        private bool _isShowAnimation;
        private BeepNotification _notification;
        private Point _targetLocation;
        private Point _startLocation;
        private float _startOpacity;
        private float _targetOpacity;
        private Action _onComplete;

        public BeepNotificationAnimator()
        {
            _animationTimer = new Timer { Interval = 1000 / ANIMATION_FPS };
            _animationTimer.Tick += AnimationTimer_Tick;
        }

        /// <summary>
        /// Animate notification appearing
        /// </summary>
        public void AnimateShow(BeepNotification notification, Point targetLocation, NotificationAnimation animation, Action onComplete = null)
        {
            if (notification == null)
                return;

            _notification = notification;
            _targetLocation = targetLocation;
            _animationType = animation;
            _isShowAnimation = true;
            _onComplete = onComplete;
            _animationStart = DateTime.Now;

            // Set initial state based on animation type
            switch (animation)
            {
                case NotificationAnimation.Slide:
                    SetupSlideIn();
                    break;

                case NotificationAnimation.Fade:
                    SetupFadeIn();
                    break;

                case NotificationAnimation.SlideAndFade:
                    SetupSlideAndFadeIn();
                    break;

                case NotificationAnimation.Bounce:
                    SetupBounceIn();
                    break;

                case NotificationAnimation.Scale:
                    SetupScaleIn();
                    break;

                case NotificationAnimation.None:
                default:
                    notification.Location = targetLocation;
                    notification.Visible = true;
                    onComplete?.Invoke();
                    return;
            }

            _animationTimer.Start();
        }

        /// <summary>
        /// Animate notification disappearing
        /// </summary>
        public void AnimateHide(BeepNotification notification, NotificationAnimation animation, Action onComplete = null)
        {
            if (notification == null)
                return;

            _notification = notification;
            _startLocation = notification.Location;
            _animationType = animation;
            _isShowAnimation = false;
            _onComplete = onComplete;
            _animationStart = DateTime.Now;

            switch (animation)
            {
                case NotificationAnimation.Slide:
                    SetupSlideOut();
                    break;

                case NotificationAnimation.Fade:
                    SetupFadeOut();
                    break;

                case NotificationAnimation.SlideAndFade:
                    SetupSlideAndFadeOut();
                    break;

                case NotificationAnimation.Bounce:
                    SetupBounceOut();
                    break;

                case NotificationAnimation.Scale:
                    SetupScaleOut();
                    break;

                case NotificationAnimation.None:
                default:
                    onComplete?.Invoke();
                    return;
            }

            _animationTimer.Start();
        }

        /// <summary>
        /// Stop any running animation
        /// </summary>
        public void Stop()
        {
            _animationTimer?.Stop();
        }

        #region Setup Methods - Show Animations

        private void SetupSlideIn()
        {
            // Slide from right (off-screen to target)
            _startLocation = new Point(_targetLocation.X + 300, _targetLocation.Y);
            _notification.Location = _startLocation;
            _notification.Opacity = 1.0;
            _startOpacity = 1.0f;
            _targetOpacity = 1.0f;
        }

        private void SetupFadeIn()
        {
            _notification.Location = _targetLocation;
            _notification.Opacity = 0.0;
            _startOpacity = 0.0f;
            _targetOpacity = 1.0f;
        }

        private void SetupSlideAndFadeIn()
        {
            _startLocation = new Point(_targetLocation.X + 300, _targetLocation.Y);
            _notification.Location = _startLocation;
            _notification.Opacity = 0.0;
            _startOpacity = 0.0f;
            _targetOpacity = 1.0f;
        }

        private void SetupBounceIn()
        {
            _startLocation = new Point(_targetLocation.X + 50, _targetLocation.Y);
            _notification.Location = _startLocation;
            _notification.Opacity = 0.0;
            _startOpacity = 0.0f;
            _targetOpacity = 1.0f;
        }

        private void SetupScaleIn()
        {
            _notification.Location = _targetLocation;
            _notification.Opacity = 0.0;
            _startOpacity = 0.0f;
            _targetOpacity = 1.0f;
            // Scale is handled in the animation tick
        }

        #endregion

        #region Setup Methods - Hide Animations

        private void SetupSlideOut()
        {
            _targetLocation = new Point(_startLocation.X + 300, _startLocation.Y);
            _startOpacity = 1.0f;
            _targetOpacity = 1.0f;
        }

        private void SetupFadeOut()
        {
            _targetLocation = _startLocation;
            _startOpacity = 1.0f;
            _targetOpacity = 0.0f;
        }

        private void SetupSlideAndFadeOut()
        {
            _targetLocation = new Point(_startLocation.X + 300, _startLocation.Y);
            _startOpacity = 1.0f;
            _targetOpacity = 0.0f;
        }

        private void SetupBounceOut()
        {
            _targetLocation = new Point(_startLocation.X + 50, _startLocation.Y);
            _startOpacity = 1.0f;
            _targetOpacity = 0.0f;
        }

        private void SetupScaleOut()
        {
            _targetLocation = _startLocation;
            _startOpacity = 1.0f;
            _targetOpacity = 0.0f;
        }

        #endregion

        #region Animation Timer

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            if (_notification == null || _notification.IsDisposed)
            {
                CompleteAnimation();
                return;
            }

            var elapsed = (DateTime.Now - _animationStart).TotalMilliseconds;
            var progress = Math.Min(1.0f, (float)(elapsed / ANIMATION_DURATION));

            // Apply easing
            float easedProgress = ApplyEasing(progress);

            // Update based on animation type
            switch (_animationType)
            {
                case NotificationAnimation.Slide:
                    AnimateSlide(easedProgress);
                    break;

                case NotificationAnimation.Fade:
                    AnimateFade(easedProgress);
                    break;

                case NotificationAnimation.SlideAndFade:
                    AnimateSlideAndFade(easedProgress);
                    break;

                case NotificationAnimation.Bounce:
                    AnimateBounce(easedProgress);
                    break;

                case NotificationAnimation.Scale:
                    AnimateScale(easedProgress);
                    break;
            }

            // Complete animation when done
            if (progress >= 1.0f)
            {
                CompleteAnimation();
            }
        }

        private void CompleteAnimation()
        {
            _animationTimer?.Stop();

            if (_notification != null && !_notification.IsDisposed)
            {
                // Ensure final state
                if (_isShowAnimation)
                {
                    _notification.Location = _targetLocation;
                    _notification.Opacity = 1.0;
                }
                else
                {
                    _notification.Opacity = _targetOpacity;
                }
            }

            _onComplete?.Invoke();
            _notification = null;
        }

        #endregion

        #region Animation Methods

        private void AnimateSlide(float progress)
        {
            int x = (int)Lerp(_startLocation.X, _targetLocation.X, progress);
            int y = (int)Lerp(_startLocation.Y, _targetLocation.Y, progress);
            _notification.Location = new Point(x, y);
        }

        private void AnimateFade(float progress)
        {
            float opacity = Lerp(_startOpacity, _targetOpacity, progress);
            _notification.Opacity = Math.Max(0, Math.Min(1, opacity));
        }

        private void AnimateSlideAndFade(float progress)
        {
            AnimateSlide(progress);
            AnimateFade(progress);
        }

        private void AnimateBounce(float progress)
        {
            // Bounce easing
            float bounceProgress = progress;
            if (progress < 1.0f)
            {
                float n1 = 7.5625f;
                float d1 = 2.75f;

                if (progress < 1 / d1)
                {
                    bounceProgress = n1 * progress * progress;
                }
                else if (progress < 2 / d1)
                {
                    bounceProgress = n1 * (progress -= 1.5f / d1) * progress + 0.75f;
                }
                else if (progress < 2.5 / d1)
                {
                    bounceProgress = n1 * (progress -= 2.25f / d1) * progress + 0.9375f;
                }
                else
                {
                    bounceProgress = n1 * (progress -= 2.625f / d1) * progress + 0.984375f;
                }
            }

            int x = (int)Lerp(_startLocation.X, _targetLocation.X, bounceProgress);
            int y = (int)Lerp(_startLocation.Y, _targetLocation.Y, bounceProgress);
            _notification.Location = new Point(x, y);

            float opacity = Lerp(_startOpacity, _targetOpacity, progress);
            _notification.Opacity = Math.Max(0, Math.Min(1, opacity));
        }

        private void AnimateScale(float progress)
        {
            // Scale from center
            float opacity = Lerp(_startOpacity, _targetOpacity, progress);
            _notification.Opacity = Math.Max(0, Math.Min(1, opacity));

            // Scale effect via size (simplified - could use transform if available)
            if (!_isShowAnimation || progress > 0.1f) // Skip initial scale for show
            {
                _notification.Location = _targetLocation;
            }
        }

        #endregion

        #region Easing Functions

        private float ApplyEasing(float t)
        {
            // Ease out cubic for smooth deceleration
            return 1 - (float)Math.Pow(1 - t, 3);
        }

        private float Lerp(float start, float end, float t)
        {
            return start + (end - start) * t;
        }

        #endregion

        #region Cleanup

        public void Dispose()
        {
            Stop();
            _animationTimer?.Dispose();
            _animationTimer = null;
            _notification = null;
            _onComplete = null;
        }

        #endregion
    }
}
