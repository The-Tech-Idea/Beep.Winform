using System;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost.Helpers
{
    public class DocumentAnimationHelper
    {
        private readonly Timer _timer;
        private readonly Action<double> _onUpdate;
        private readonly Action _onComplete;
        private readonly double _durationMs;
        private readonly Func<double, double> _easing;
        private double _startMs;
        private double _from;
        private double _to;

        public bool IsRunning { get; private set; }

        public DocumentAnimationHelper(double durationMs, Action<double> onUpdate, Action onComplete = null, Func<double, double> easing = null)
        {
            _durationMs = durationMs;
            _onUpdate = onUpdate;
            _onComplete = onComplete;
            _easing = easing ?? EaseOutCubic;
            _timer = new Timer();
            _timer.Tick += OnTick;
        }

        public void Start(double from, double to)
        {
            _from = from;
            _to = to;
            _startMs = Environment.TickCount;
            IsRunning = true;
            _timer.Interval = 16;
            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
            IsRunning = false;
        }

        private void OnTick(object sender, EventArgs e)
        {
            double elapsed = Environment.TickCount - _startMs;
            double progress = Math.Min(1.0, elapsed / _durationMs);
            double eased = _easing(progress);
            double value = _from + (_to - _from) * eased;

            _onUpdate(value);

            if (progress >= 1.0)
            {
                _timer.Stop();
                IsRunning = false;
                _onComplete?.Invoke();
            }
        }

        public static double EaseOutCubic(double t) => 1 - Math.Pow(1 - t, 3);
        public static double EaseInOutCubic(double t) => t < 0.5 ? 4 * t * t * t : 1 - Math.Pow(-2 * t + 2, 3) / 2;
        public static double EaseOutQuart(double t) => 1 - Math.Pow(1 - t, 4);

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
