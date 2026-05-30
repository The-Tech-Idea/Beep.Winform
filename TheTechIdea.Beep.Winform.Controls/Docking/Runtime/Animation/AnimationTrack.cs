using System;

namespace TheTechIdea.Beep.Winform.Controls.Docking.Runtime.Animation
{
    /// <summary>
    /// A single running tween: interpolates from <see cref="From"/> to <see cref="To"/> over
    /// <see cref="DurationMs"/> using an easing function, raising <see cref="OnTick"/> with the
    /// eased value and <see cref="OnComplete"/> when finished. Driven by <see cref="DockAnimator"/>.
    /// </summary>
    public sealed class AnimationTrack
    {
        private readonly Func<float, float> _easing;
        private long _startTicks;
        private bool _started;

        public AnimationTrack(float from, float to, int durationMs,
            Action<float> onTick, Action onComplete = null, Func<float, float> easing = null)
        {
            From = from;
            To = to;
            DurationMs = Math.Max(1, durationMs);
            OnTick = onTick;
            OnComplete = onComplete;
            _easing = easing ?? Easing.EaseOutQuad;
        }

        public float From { get; }
        public float To { get; }
        public int DurationMs { get; }
        public Action<float> OnTick { get; }
        public Action OnComplete { get; }
        public bool IsComplete { get; private set; }

        /// <summary>Advances the tween against the supplied timestamp (ms). Returns false when complete.</summary>
        public bool Advance(long nowMs)
        {
            if (IsComplete)
                return false;

            if (!_started)
            {
                _started = true;
                _startTicks = nowMs;
            }

            float elapsed = nowMs - _startTicks;
            float t = elapsed / DurationMs;
            if (t >= 1f)
            {
                OnTick?.Invoke(To);
                IsComplete = true;
                OnComplete?.Invoke();
                return false;
            }

            float value = From + (To - From) * _easing(t);
            OnTick?.Invoke(value);
            return true;
        }

        /// <summary>Cancels the tween without firing completion.</summary>
        public void Cancel() => IsComplete = true;
    }
}
