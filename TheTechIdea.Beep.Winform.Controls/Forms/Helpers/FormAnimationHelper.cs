using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Forms.Helpers
{
    /// <summary>
    /// Helper for form animations including fade, resize, and transition effects.
    /// Provides smooth animations for show/hide and state changes.
    /// </summary>
    internal class FormAnimationHelper : IDisposable
    {
        private readonly IBeepModernFormHost _host;
        private bool _disposed = false;
        private bool _isAnimating = false;

        /// <summary>Animation types supported</summary>
        public enum AnimationType
        {
            FadeIn,
            FadeOut,
            SlideInFromTop,
            SlideInFromBottom,
            SlideInFromLeft,
            SlideInFromRight,
            ScaleIn,
            ScaleOut
        }

        /// <summary>Animation easing functions</summary>
        public enum EasingType
        {
            Linear,
            EaseIn,
            EaseOut,
            EaseInOut
        }

        /// <summary>Gets whether an animation is currently playing</summary>
        public bool IsAnimating => _isAnimating;

        /// <summary>Gets or sets the default animation duration in milliseconds</summary>
        public int DefaultDuration { get; set; } = 300;

        /// <summary>Gets or sets the default easing type</summary>
        public EasingType DefaultEasing { get; set; } = EasingType.EaseOut;

        public FormAnimationHelper(IBeepModernFormHost host)
        {
            _host = host ?? throw new ArgumentNullException(nameof(host));
        }

        /// <summary>
        /// Plays the specified animation.
        /// </summary>
        /// <param name="type">Animation type</param>
        /// <param name="duration">Duration in milliseconds (0 = use default)</param>
        /// <param name="easing">Easing function (null = use default)</param>
        /// <returns>Task that completes when animation finishes</returns>
        public async Task PlayAsync(AnimationType type, int duration = 0, EasingType? easing = null)
        {
            if (_disposed || _isAnimating)
                return;

            var actualDuration = duration > 0 ? duration : DefaultDuration;
            var actualEasing = easing ?? DefaultEasing;

            _isAnimating = true;
            try
            {
                switch (type)
                {
                    case AnimationType.FadeIn:
                        await FadeInAsync(actualDuration, actualEasing);
                        break;
                    case AnimationType.FadeOut:
                        await FadeOutAsync(actualDuration, actualEasing);
                        break;
                    case AnimationType.SlideInFromTop:
                        await SlideInAsync(Direction.Top, actualDuration, actualEasing);
                        break;
                    case AnimationType.SlideInFromBottom:
                        await SlideInAsync(Direction.Bottom, actualDuration, actualEasing);
                        break;
                    case AnimationType.SlideInFromLeft:
                        await SlideInAsync(Direction.Left, actualDuration, actualEasing);
                        break;
                    case AnimationType.SlideInFromRight:
                        await SlideInAsync(Direction.Right, actualDuration, actualEasing);
                        break;
                    case AnimationType.ScaleIn:
                        await ScaleInAsync(actualDuration, actualEasing);
                        break;
                    case AnimationType.ScaleOut:
                        await ScaleOutAsync(actualDuration, actualEasing);
                        break;
                }
            }
            finally
            {
                _isAnimating = false;
            }
        }

        /// <summary>
        /// Plays a fade-in animation.
        /// </summary>
        private async Task FadeInAsync(int duration, EasingType easing)
        {
            var form = _host.AsForm;
            var startOpacity = form.Opacity;
            var targetOpacity = 1.0;

            await AnimatePropertyAsync(
                getValue: () => form.Opacity,
                setValue: value => form.Opacity = Math.Clamp(value, 0.0, 1.0),
                from: startOpacity,
                to: targetOpacity,
                duration: duration,
                easing: easing);
        }

        /// <summary>
        /// Plays a fade-out animation.
        /// </summary>
        private async Task FadeOutAsync(int duration, EasingType easing)
        {
            var form = _host.AsForm;
            var startOpacity = form.Opacity;
            var targetOpacity = 0.0;

            await AnimatePropertyAsync(
                getValue: () => form.Opacity,
                setValue: value => form.Opacity = Math.Clamp(value, 0.0, 1.0),
                from: startOpacity,
                to: targetOpacity,
                duration: duration,
                easing: easing);
        }

        /// <summary>
        /// Direction enumeration for slide animations.
        /// </summary>
        private enum Direction { Top, Bottom, Left, Right }

        /// <summary>
        /// Plays a slide-in animation from the specified direction.
        /// </summary>
        private async Task SlideInAsync(Direction direction, int duration, EasingType easing)
        {
            var form = _host.AsForm;
            var originalLocation = form.Location;
            var screen = Screen.FromControl(form);
            Point startLocation;

            // Calculate start position based on direction
            switch (direction)
            {
                case Direction.Top:
                    startLocation = new Point(originalLocation.X, screen.WorkingArea.Top - form.Height);
                    break;
                case Direction.Bottom:
                    startLocation = new Point(originalLocation.X, screen.WorkingArea.Bottom);
                    break;
                case Direction.Left:
                    startLocation = new Point(screen.WorkingArea.Left - form.Width, originalLocation.Y);
                    break;
                case Direction.Right:
                    startLocation = new Point(screen.WorkingArea.Right, originalLocation.Y);
                    break;
                default:
                    return;
            }

            // Set initial position
            form.Location = startLocation;

            // Animate to final position
            await AnimateLocationAsync(startLocation, originalLocation, duration, easing);
        }

        /// <summary>
        /// Plays a scale-in animation.
        /// </summary>
        private async Task ScaleInAsync(int duration, EasingType easing)
        {
            var form = _host.AsForm;
            var originalSize = form.Size;
            var originalLocation = form.Location;
            var startSize = new Size(originalSize.Width / 10, originalSize.Height / 10);
            var startLocation = new Point(
                originalLocation.X + (originalSize.Width - startSize.Width) / 2,
                originalLocation.Y + (originalSize.Height - startSize.Height) / 2);

            // Set initial scale
            form.Size = startSize;
            form.Location = startLocation;

            // Animate size and position simultaneously
            var sizeTask = AnimateSizeAsync(startSize, originalSize, duration, easing);
            var locationTask = AnimateLocationAsync(startLocation, originalLocation, duration, easing);

            await Task.WhenAll(sizeTask, locationTask);
        }

        /// <summary>
        /// Plays a scale-out animation.
        /// </summary>
        private async Task ScaleOutAsync(int duration, EasingType easing)
        {
            var form = _host.AsForm;
            var startSize = form.Size;
            var startLocation = form.Location;
            var targetSize = new Size(startSize.Width / 10, startSize.Height / 10);
            var targetLocation = new Point(
                startLocation.X + (startSize.Width - targetSize.Width) / 2,
                startLocation.Y + (startSize.Height - targetSize.Height) / 2);

            // Animate size and position simultaneously
            var sizeTask = AnimateSizeAsync(startSize, targetSize, duration, easing);
            var locationTask = AnimateLocationAsync(startLocation, targetLocation, duration, easing);

            await Task.WhenAll(sizeTask, locationTask);
        }

        /// <summary>
        /// Animates a double property over time.
        /// </summary>
        private async Task AnimatePropertyAsync(Func<double> getValue, Action<double> setValue, double from, double to, int duration, EasingType easing)
        {
            const int steps = 30; // ~60 FPS
            var stepDelay = Math.Max(1, duration / steps);
            var delta = to - from;

            for (int i = 0; i <= steps && !_disposed; i++)
            {
                var progress = (double)i / steps;
                var easedProgress = ApplyEasing(progress, easing);
                var currentValue = from + (delta * easedProgress);
                
                try
                {
                    setValue(currentValue);
                    _host.AsForm.Refresh();
                }
                catch
                {
                    // Form might be disposed during animation
                    break;
                }

                if (i < steps)
                    await Task.Delay(stepDelay);
            }
        }

        /// <summary>
        /// Animates form location over time.
        /// </summary>
        private async Task AnimateLocationAsync(Point from, Point to, int duration, EasingType easing)
        {
            await AnimatePropertyAsync(
                getValue: () => _host.AsForm.Location.X,
                setValue: value => 
                {
                    var progress = (value - from.X) / (double)(to.X - from.X);
                    var y = from.Y + (int)((to.Y - from.Y) * progress);
                    _host.AsForm.Location = new Point((int)value, y);
                },
                from: from.X,
                to: to.X,
                duration: duration,
                easing: easing);
        }

        /// <summary>
        /// Animates form size over time.
        /// </summary>
        private async Task AnimateSizeAsync(Size from, Size to, int duration, EasingType easing)
        {
            await AnimatePropertyAsync(
                getValue: () => _host.AsForm.Size.Width,
                setValue: value =>
                {
                    var progress = (value - from.Width) / (double)(to.Width - from.Width);
                    var height = from.Height + (int)((to.Height - from.Height) * progress);
                    _host.AsForm.Size = new Size((int)value, height);
                },
                from: from.Width,
                to: to.Width,
                duration: duration,
                easing: easing);
        }

        /// <summary>
        /// Applies easing function to progress value.
        /// </summary>
        private double ApplyEasing(double t, EasingType easing)
        {
            return easing switch
            {
                EasingType.Linear => t,
                EasingType.EaseIn => t * t,
                EasingType.EaseOut => 1 - Math.Pow(1 - t, 2),
                EasingType.EaseInOut => t < 0.5 ? 2 * t * t : 1 - Math.Pow(-2 * t + 2, 2) / 2,
                _ => t
            };
        }

        public void Dispose()
        {
            _disposed = true;
        }
    }
}