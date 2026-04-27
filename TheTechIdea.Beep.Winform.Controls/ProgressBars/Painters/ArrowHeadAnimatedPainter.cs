using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.ProgressBars.Helpers;
using TheTechIdea.Beep.Winform.Controls.ProgressBars.Models;
 

namespace TheTechIdea.Beep.Winform.Controls.ProgressBars.Painters
{
    internal sealed class ArrowHeadAnimatedPainter : IProgressPainter, IProgressPainterV2
    {
        private static readonly Dictionary<IntPtr, ArrowHeadState> _states = new();

        public string Key => nameof(ProgressPainterKind.ArrowHeadAnimated);

        public void Paint(Graphics g, Rectangle bounds, IBeepTheme theme, BeepProgressBar owner, IReadOnlyDictionary<string, object> p)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            var rect = bounds;

            var state = GetOrCreateState(owner.Handle, owner, p);

            using var basePen = new Pen(Color.FromArgb(80, theme.BorderColor), ProgressBarDpiHelpers.Scale(owner, 3)) { StartCap = LineCap.Round, EndCap = LineCap.Round };
            int midY = rect.Top + rect.Height / 2;
            g.DrawLine(basePen, rect.Left, midY, rect.Right, midY);

            state.DurationMs = GetDouble(p, "HeadDuration", 1000);
            string easing = ProgressPainterParameterContracts.GetString(p, "HeadEasing", "ease-in-out");
            int headWidth = ProgressBarDpiHelpers.Scale(owner, ProgressPainterParameterContracts.GetInt(p, "HeadWidth", 18));
            int headHeight = ProgressBarDpiHelpers.Scale(owner, ProgressPainterParameterContracts.GetInt(p, "HeadHeight", 8));
            Color headColor = theme.PrimaryColor.IsEmpty ? Color.SeaGreen : theme.PrimaryColor;
            if (!owner.Enabled)
            {
                headColor = Color.FromArgb(120, headColor);
            }
            bool followValue = !ProgressPainterParameterContracts.GetBool(p, "HeadLoop", false);
            bool breathing = ProgressPainterParameterContracts.GetBool(p, "HeadBreathing", true);
            float breathAmp = ProgressPainterParameterContracts.GetFloat(p, "HeadBreathingAmp", 0.2f);
            state.BreathSpeed = GetDouble(p, "HeadBreathingSpeed", 1.0);
            if (!owner.Enabled)
            {
                breathing = false;
            }

            if (breathing || !followValue)
            {
                EnsureTimer(owner.Handle, owner, (int)Math.Max(16, state.DurationMs / 60.0));
            }
            else
            {
                StopTimer(owner.Handle);
            }

            float progress = owner.DisplayProgressPercentageAccessor;
            float t = followValue ? progress : state.LoopT;
            float te = ApplyEasing(t, easing);
            int cx = rect.Left + (int)(te * rect.Width);

            using (var fillPen = new Pen(Color.FromArgb(owner.Enabled ? 160 : 110, headColor), ProgressBarDpiHelpers.Scale(owner, 4)) { StartCap = LineCap.Round, EndCap = LineCap.Round })
                g.DrawLine(fillPen, rect.Left, midY, cx, midY);

            float scale = 1f;
            if (breathing)
            {
                scale = 1f + breathAmp * (float)Math.Sin(state.Phase);
            }
            int hw = Math.Max(4, (int)(headWidth * scale));
            int hh = Math.Max(2, (int)(headHeight * scale));

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

        public void Paint(Graphics g, ProgressPainterContext context, BeepProgressBar owner)
        {
            if (context == null) return;
            Paint(g, context.Bounds, context.Theme, owner, context.Parameters);
        }

        public void UpdateHitAreas(ProgressPainterContext context, BeepProgressBar owner, Action<string, Rectangle> register)
        {
            if (context == null) return;
            UpdateHitAreas(owner, context.Bounds, context.Theme, context.Parameters, register);
        }

        private static ArrowHeadState GetOrCreateState(IntPtr handle, BeepProgressBar owner, IReadOnlyDictionary<string, object> p)
        {
            if (!_states.TryGetValue(handle, out var state))
            {
                state = new ArrowHeadState();
                _states[handle] = state;
                owner.HandleDestroyed += (s, e) => { _states.Remove(handle); StopTimer(handle); };
            }
            return state;
        }

        private static void EnsureTimer(IntPtr handle, BeepProgressBar owner, int interval)
        {
            if (_states.TryGetValue(handle, out var state))
            {
                state.Timer ??= new Timer();
                state.Timer.Interval = interval;
                state.Timer.Tick -= state.OnTick;
                state.Timer.Tick += state.OnTick;
                if (!state.Timer.Enabled)
                {
                    state.Container = owner;
                    state.Owner = owner;
                    state.Timer.Start();
                }
            }
        }

        private static void StopTimer(IntPtr handle)
        {
            if (_states.TryGetValue(handle, out var state))
            {
                if (state.Timer != null)
                {
                    state.Timer.Stop();
                    state.Timer.Tick -= state.OnTick;
                    state.Timer.Dispose();
                    state.Timer = null;
                }
                state.Container = null;
                state.Owner = null;
            }
        }

        private static double GetDouble(IReadOnlyDictionary<string, object> p, string key, double fallback)
            => p != null && p.TryGetValue(key, out var v) && v is IConvertible ? Convert.ToDouble(v) : fallback;
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

        private sealed class ArrowHeadState
        {
            public Timer Timer;
            public Control Container;
            public BeepProgressBar Owner;
            public double Phase;
            public float LoopT;
            public double DurationMs = 1000;
            public double BreathSpeed = 1.0;
            private DateTime _lastTick = DateTime.UtcNow;

            public void OnTick(object sender, EventArgs e)
            {
                if (Owner == null || Owner.IsDisposed || Owner.Disposing || Owner.PainterKind != ProgressPainterKind.ArrowHeadAnimated)
                {
                    StopTimer(Owner?.Handle ?? IntPtr.Zero);
                    return;
                }

                if (!Owner.Enabled)
                {
                    StopTimer(Owner.Handle);
                    return;
                }

                var now = DateTime.UtcNow;
                double dt = (now - _lastTick).TotalSeconds; if (dt <= 0) dt = 0.016; _lastTick = now;
                LoopT += (float)(dt * (1000.0 / Math.Max(1.0, DurationMs)));
                if (LoopT > 1f) LoopT -= 1f;
                Phase += 2 * Math.PI * BreathSpeed * dt;
                if (Phase > 2 * Math.PI) Phase -= 2 * Math.PI;
                Container?.Invalidate();
            }
        }
    }
}
