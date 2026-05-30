using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Docking.Runtime.Animation
{
    /// <summary>
    /// A small shared, timer-based tween runner. Drives any number of
    /// <see cref="AnimationTrack"/>s from a single WinForms <see cref="Timer"/> so docking
    /// animations (float fade-in, peek delays) share one clock instead of spawning timers.
    /// </summary>
    public sealed class DockAnimator : IDisposable
    {
        private readonly Timer _timer;
        private readonly List<AnimationTrack> _tracks = new List<AnimationTrack>();
        private readonly Stopwatch _clock = Stopwatch.StartNew();

        public DockAnimator(int tickMs = 15)
        {
            _timer = new Timer { Interval = Math.Max(5, tickMs) };
            _timer.Tick += OnTick;
        }

        /// <summary>True while at least one track is running.</summary>
        public bool IsRunning => _timer.Enabled;

        /// <summary>Queues a track and starts the shared timer if needed.</summary>
        public AnimationTrack Run(AnimationTrack track)
        {
            if (track == null) return null;
            _tracks.Add(track);
            if (!_timer.Enabled)
                _timer.Start();
            return track;
        }

        /// <summary>Convenience overload that builds and runs a track.</summary>
        public AnimationTrack Run(float from, float to, int durationMs,
            Action<float> onTick, Action onComplete = null, Func<float, float> easing = null)
            => Run(new AnimationTrack(from, to, durationMs, onTick, onComplete, easing));

        private void OnTick(object sender, EventArgs e)
        {
            long now = _clock.ElapsedMilliseconds;

            for (int i = _tracks.Count - 1; i >= 0; i--)
            {
                var track = _tracks[i];
                bool running;
                try
                {
                    running = track.Advance(now);
                }
                catch
                {
                    running = false;   // a faulting callback must not wedge the shared timer
                }

                if (!running)
                    _tracks.RemoveAt(i);
            }

            if (_tracks.Count == 0)
                _timer.Stop();
        }

        public void Dispose()
        {
            _timer.Stop();
            _timer.Tick -= OnTick;
            _timer.Dispose();
            _tracks.Clear();
        }
    }
}
