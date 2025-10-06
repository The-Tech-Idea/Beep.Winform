using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
 

namespace TheTechIdea.Beep.Winform.Controls.ProgressBars.Painters
{
    internal sealed class ArrowHeadAnimatedPainter : IProgressPainter
    {
        public string Key => nameof(ProgressPainterKind.ArrowHeadAnimated);

        // Timer drives either breathing at rest or optional looping mode
        private Timer _timer;
        private Control _container;
        private double _phase;        // breathing phase
        private float _loopT;         // 0..1 when HeadLoop = true
        private double _durationMs = 1000; // cached for loop
        private double _breathSpeed = 1.0; // Hz-ish

        public void Paint(Graphics g, Rectangle bounds, IBeepTheme theme, BeepProgressBar owner, IReadOnlyDictionary<string, object> p)
        {
            _container = owner;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            var rect = bounds; rect.Inflate(-owner.BorderThickness, -owner.BorderThickness);

            // baseline
            using var basePen = new Pen(Color.FromArgb(80, theme.BorderColor), 3) { StartCap = LineCap.Round, EndCap = LineCap.Round };
            int midY = rect.Top + rect.Height / 2;
            g.DrawLine(basePen, rect.Left, midY, rect.Right, midY);

            // config
            _durationMs = GetDouble(p, "HeadDuration", 1000);      // used only for loop mode
            string easing = GetString(p, "HeadEasing", "ease-in-out");
            int headWidth = GetInt(p, "HeadWidth", 18);
            int headHeight = GetInt(p, "HeadHeight", 8);
            Color headColor = theme.PrimaryColor.IsEmpty ? Color.SeaGreen : theme.PrimaryColor;
            bool followValue = !GetBool(p, "HeadLoop", false);      // default: follow actual Value
            bool breathing = GetBool(p, "HeadBreathing", true);
            float breathAmp = GetFloat(p, "HeadBreathingAmp", 0.2f); // +/- 20% size
            _breathSpeed = GetDouble(p, "HeadBreathingSpeed", 1.0); // approx Hz

            // Ensure timer when needed (breathing or loop)
            if (breathing || !followValue)
            {
                EnsureTimer((int)Math.Max(16, _durationMs / 60.0));
            }
            else if (_timer != null && _timer.Enabled)
            {
                // no animation required
                _timer.Stop();
            }

            // Determine head position
            float progress = owner.DisplayProgressPercentageAccessor; // already animates between old and new when enabled
            float t = followValue ? progress : _loopT;
            float te = ApplyEasing(t, easing);
            int cx = rect.Left + (int)(te * rect.Width);

            // draw filled line up to head
            using (var fillPen = new Pen(Color.FromArgb(160, headColor), 4) { StartCap = LineCap.Round, EndCap = LineCap.Round })
                g.DrawLine(fillPen, rect.Left, midY, cx, midY);

            // Breathing scale
            float scale = 1f;
            if (breathing)
            {
                scale = 1f + breathAmp * (float)Math.Sin(_phase);
            }
            int hw = Math.Max(4, (int)(headWidth * scale));
            int hh = Math.Max(2, (int)(headHeight * scale));

            // draw arrow head at current value
            var pts = new[]
            {
                new Point(cx, midY),
                new Point(cx - hw, midY - hh),
                new Point(cx - hw, midY + hh)
            };
            using var head = new SolidBrush(headColor);
            g.FillPolygon(head, pts);
        }

        public void UpdateHitAreas(BeepProgressBar owner, Rectangle bounds, IBeepTheme theme, IReadOnlyDictionary<string, object> p, Action<string, Rectangle> register) { }

        private void EnsureTimer(int interval)
        {
            _timer ??= new Timer();
            _timer.Interval = interval;
            _timer.Tick -= OnTick; _timer.Tick += OnTick;
            if (!_timer.Enabled) _timer.Start();
        }

        private DateTime _lastTick = DateTime.UtcNow;
        private void OnTick(object sender, EventArgs e)
        {
            var now = DateTime.UtcNow;
            double dt = (now - _lastTick).TotalSeconds; if (dt <= 0) dt = 0.016; _lastTick = now;
            // advance loop position and breathing phase
            _loopT += (float)(dt * (1000.0 / Math.Max(1.0, _durationMs)));
            if (_loopT > 1f) _loopT -= 1f;
            _phase += 2 * Math.PI * _breathSpeed * dt;
            if (_phase > 2 * Math.PI) _phase -= 2 * Math.PI;
            _container?.Invalidate();
        }

        private static double GetDouble(IReadOnlyDictionary<string, object> p, string key, double fallback)
            => p != null && p.TryGetValue(key, out var v) && v is IConvertible ? Convert.ToDouble(v) : fallback;
        private static string GetString(IReadOnlyDictionary<string, object> p, string key, string fallback)
            => p != null && p.TryGetValue(key, out var v) && v is string s ? s : fallback;
        private static int GetInt(IReadOnlyDictionary<string, object> p, string key, int fallback)
            => p != null && p.TryGetValue(key, out var v) && v is IConvertible ? Convert.ToInt32(v) : fallback;
        private static bool GetBool(IReadOnlyDictionary<string, object> p, string key, bool fallback)
            => p != null && p.TryGetValue(key, out var v) && v is bool b ? b : fallback;
        private static float GetFloat(IReadOnlyDictionary<string, object> p, string key, float fallback)
            => p != null && p.TryGetValue(key, out var v) && v is IConvertible ? Convert.ToSingle(v) : fallback;
        private static float ApplyEasing(float t, string easing)
        {
            t = Math.Max(0f, Math.Min(1f, t));
            switch (easing.Trim().ToLowerInvariant())
            {
                case "linear": return t;
                case "ease-in": return t * t;
                case "ease-out": return 1f - (1f - t) * (1f - t);
                case "ease-in-out": return t < 0.5f ? 2f * t * t : 1f - (float)Math.Pow(-2f * t + 2f, 2) / 2f;
                default: return t;
            }
        }
    }
}
