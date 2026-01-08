using System;
using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Styling.Animations
{
    /// <summary>
    /// Helper for smooth color transitions in WinForms
    /// Uses Timer-based interpolation for smooth color changes
    /// </summary>
    public class ColorTransitionHelper : IDisposable
    {
        private Timer _transitionTimer;
        private ColorTransition _currentTransition;
        private Action<Color> _onColorChanged;
        private bool _disposed = false;

        /// <summary>
        /// Initialize color transition helper
        /// </summary>
        /// <param name="onColorChanged">Callback when color changes during transition</param>
        public ColorTransitionHelper(Action<Color> onColorChanged)
        {
            _onColorChanged = onColorChanged ?? throw new ArgumentNullException(nameof(onColorChanged));
        }

        /// <summary>
        /// Start a color transition
        /// </summary>
        /// <param name="fromColor">Starting color</param>
        /// <param name="toColor">Target color</param>
        /// <param name="duration">Transition duration in milliseconds (default 250ms)</param>
        /// <param name="easing">Easing function (default EaseInOut)</param>
        public void StartTransition(Color fromColor, Color toColor, int duration = 250, EasingFunction easing = EasingFunction.EaseInOut)
        {
            // Stop any existing transition
            StopTransition();

            _currentTransition = new ColorTransition
            {
                FromColor = fromColor,
                ToColor = toColor,
                Duration = duration,
                StartTime = DateTime.Now,
                Easing = easing
            };

            // Start transition timer
            if (_transitionTimer == null)
            {
                _transitionTimer = new Timer();
                _transitionTimer.Interval = 16; // ~60 FPS
                _transitionTimer.Tick += TransitionTimer_Tick;
            }

            _transitionTimer.Start();
        }

        /// <summary>
        /// Stop current transition
        /// </summary>
        public void StopTransition()
        {
            _transitionTimer?.Stop();
            _currentTransition = null;
        }

        /// <summary>
        /// Get current interpolated color
        /// </summary>
        public Color GetCurrentColor()
        {
            if (_currentTransition == null || !_currentTransition.IsActive)
                return Color.Empty;

            return _currentTransition.GetCurrentColor();
        }

        /// <summary>
        /// Transition timer tick handler
        /// </summary>
        private void TransitionTimer_Tick(object sender, EventArgs e)
        {
            if (_currentTransition == null || !_currentTransition.IsActive)
            {
                _transitionTimer?.Stop();
                return;
            }

            Color currentColor = _currentTransition.GetCurrentColor();
            _onColorChanged?.Invoke(currentColor);

            if (!_currentTransition.IsActive)
            {
                _transitionTimer.Stop();
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                StopTransition();

                if (_transitionTimer != null)
                {
                    _transitionTimer.Dispose();
                    _transitionTimer = null;
                }

                _currentTransition = null;
                _disposed = true;
            }
        }
    }

    /// <summary>
    /// Color transition state
    /// </summary>
    internal class ColorTransition
    {
        public Color FromColor { get; set; }
        public Color ToColor { get; set; }
        public int Duration { get; set; }
        public DateTime StartTime { get; set; }
        public EasingFunction Easing { get; set; }

        public bool IsActive
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
                if (!IsActive) return 1.0f;
                var elapsed = (DateTime.Now - StartTime).TotalMilliseconds;
                float rawProgress = (float)(elapsed / Duration);
                return ApplyEasing(rawProgress, Easing);
            }
        }

        public Color GetCurrentColor()
        {
            float progress = Progress;
            progress = Math.Max(0, Math.Min(1, progress));

            return InterpolateColor(FromColor, ToColor, progress);
        }

        /// <summary>
        /// Interpolate between two colors
        /// </summary>
        private Color InterpolateColor(Color from, Color to, float progress)
        {
            return Color.FromArgb(
                (int)(from.A + (to.A - from.A) * progress),
                (int)(from.R + (to.R - from.R) * progress),
                (int)(from.G + (to.G - from.G) * progress),
                (int)(from.B + (to.B - from.B) * progress)
            );
        }

        /// <summary>
        /// Apply easing function to progress
        /// </summary>
        private float ApplyEasing(float t, EasingFunction easing)
        {
            return easing switch
            {
                EasingFunction.Linear => t,
                EasingFunction.EaseIn => t * t,
                EasingFunction.EaseOut => t * (2 - t),
                EasingFunction.EaseInOut => t < 0.5f ? 2 * t * t : -1 + (4 - 2 * t) * t,
                _ => t
            };
        }
    }

    /// <summary>
    /// Easing function types
    /// </summary>
    public enum EasingFunction
    {
        Linear,
        EaseIn,
        EaseOut,
        EaseInOut
    }
}
