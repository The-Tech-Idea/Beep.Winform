using System;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.ToolTips.Helpers
{
    /// <summary>
    /// Sprint 7 — Timer-driven animation engine for tooltip show/hide transitions.
    /// Supports all <see cref="EasingFunction"/> variants and drives an Action&lt;float&gt; tick callback.
    /// Uses <see cref="System.Windows.Forms.Timer"/> so ticks execute on the UI thread.
    /// </summary>
    public sealed class ToolTipAnimator : IDisposable
    {
        // ──────────────────────────────────────────────────────────────────────
        // Fields
        // ──────────────────────────────────────────────────────────────────────
        private readonly Timer _timer;
        private float  _from;
        private float  _to;
        private int    _durationMs;
        private int    _elapsed;
        private EasingFunction _easing;
        private Action<float> _onTick;
        private Action        _onComplete;
        private bool   _running;
        private bool   _disposed;

        private const int TickIntervalMs = 16; // ~60 fps

        // ──────────────────────────────────────────────────────────────────────
        // Constructor
        // ──────────────────────────────────────────────────────────────────────

        public ToolTipAnimator()
        {
            _timer          = new Timer { Interval = TickIntervalMs };
            _timer.Tick    += OnTick;
        }

        // ──────────────────────────────────────────────────────────────────────
        // Public API
        // ──────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Start a new animation, cancelling any currently running one.
        /// </summary>
        /// <param name="from">Start value (e.g. 0.0 for opacity).</param>
        /// <param name="to">End value (e.g. 1.0 for opacity).</param>
        /// <param name="durationMs">Total animation length in milliseconds.</param>
        /// <param name="easing">Easing function to apply.</param>
        /// <param name="onTick">Callback receiving the interpolated value each frame.</param>
        /// <param name="onComplete">Optional callback fired when the animation completes.</param>
        public void Animate(
            float from, float to, int durationMs,
            EasingFunction easing,
            Action<float>  onTick,
            Action         onComplete = null)
        {
            if (_disposed) return;

            Stop();

            _from       = from;
            _to         = to;
            _durationMs = Math.Max(1, durationMs);
            _elapsed    = 0;
            _easing     = easing;
            _onTick     = onTick     ?? throw new ArgumentNullException(nameof(onTick));
            _onComplete = onComplete;
            _running    = true;

            // Emit first frame immediately so there is no visual delay
            _onTick(from);
            _timer.Start();
        }

        /// <summary>Stop any running animation without triggering the completion callback.</summary>
        public void Stop()
        {
            if (!_running) return;
            _timer.Stop();
            _running = false;
        }

        public bool IsRunning => _running;

        // ──────────────────────────────────────────────────────────────────────
        // Timer tick
        // ──────────────────────────────────────────────────────────────────────

        private void OnTick(object sender, EventArgs e)
        {
            if (!_running) return;

            _elapsed += TickIntervalMs;
            double t = Math.Min(1.0, (double)_elapsed / _durationMs);
            double easedT = ApplyEasing(t, _easing);

            float value = _from + (float)((_to - _from) * easedT);
            _onTick(value);

            if (t >= 1.0)
            {
                _timer.Stop();
                _running = false;
                _onComplete?.Invoke();
            }
        }

        // ──────────────────────────────────────────────────────────────────────
        // Easing calculations
        // ──────────────────────────────────────────────────────────────────────

        /// <summary>Apply the chosen easing function to a normalised progress value t ∈ [0,1].</summary>
        public static double ApplyEasing(double t, EasingFunction fn)
        {
            t = Math.Max(0.0, Math.Min(1.0, t));
            return fn switch
            {
                EasingFunction.Linear     => t,
                EasingFunction.EaseIn     => t * t * t,
                EasingFunction.EaseOut    => 1.0 - Math.Pow(1.0 - t, 3.0),
                EasingFunction.EaseInOut  => t < 0.5
                                             ? 4.0 * t * t * t
                                             : 1.0 - Math.Pow(-2.0 * t + 2.0, 3.0) / 2.0,
                EasingFunction.Spring     => SpringEasing(t),
                EasingFunction.Bounce     => BounceEasing(t),
                EasingFunction.BackOut    => BackOutEasing(t),
                _                         => t
            };
        }

        private static double SpringEasing(double t)
        {
            const double c4 = 2.0 * Math.PI / 3.0;
            if (t <= 0) return 0;
            if (t >= 1) return 1;
            return Math.Pow(2, -10 * t) * Math.Sin((t * 10 - 0.75) * c4) + 1;
        }

        private static double BounceEasing(double t)
        {
            const double n1 = 7.5625;
            const double d1 = 2.75;
            if      (t < 1.0 / d1)  return n1 * t * t;
            else if (t < 2.0 / d1)  { t -= 1.5   / d1; return n1 * t * t + 0.75; }
            else if (t < 2.5 / d1)  { t -= 2.25  / d1; return n1 * t * t + 0.9375; }
            else                     { t -= 2.625 / d1; return n1 * t * t + 0.984375; }
        }

        private static double BackOutEasing(double t)
        {
            const double c1 = 1.70158;
            const double c3 = c1 + 1.0;
            return 1.0 + c3 * Math.Pow(t - 1.0, 3.0) + c1 * Math.Pow(t - 1.0, 2.0);
        }

        // ──────────────────────────────────────────────────────────────────────
        // IDisposable
        // ──────────────────────────────────────────────────────────────────────

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            _timer.Stop();
            _timer.Dispose();
        }
    }
}
