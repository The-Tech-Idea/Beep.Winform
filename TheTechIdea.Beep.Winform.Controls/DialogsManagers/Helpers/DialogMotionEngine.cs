using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.DialogsManagers.Models;

namespace TheTechIdea.Beep.Winform.Controls.DialogsManagers.Helpers
{
    internal static class DialogMotionEngine
    {
        private static readonly Dictionary<string, Timer> _activeTimers = new();

        public static double Ease(double t, DialogAnimationEasing easing)
        {
            t = Math.Max(0d, Math.Min(1d, t));
            return easing switch
            {
                DialogAnimationEasing.Linear => t,
                DialogAnimationEasing.EaseInOutQuad => t < 0.5 ? 2 * t * t : 1 - Math.Pow(-2 * t + 2, 2) / 2,
                DialogAnimationEasing.EaseOutBack => 1 + 2.70158 * Math.Pow(t - 1, 3) + 1.70158 * Math.Pow(t - 1, 2),
                DialogAnimationEasing.EaseOutElastic => t == 0 || t == 1 ? t : Math.Pow(2, -10 * t) * Math.Sin((t * 10 - 0.75) * (2 * Math.PI / 3)) + 1,
                DialogAnimationEasing.EaseOutSpring => 1 - (Math.Cos(t * 4.5 * Math.PI) * Math.Exp(-t * 6)),
                _ => 1 - Math.Pow(1 - t, 3)
            };
        }

        public static void Animate(Form form, int durationMs, Action<double> frame, Action? completed = null, string? animationKey = null)
        {
            if (durationMs <= 0)
            {
                frame(1d);
                completed?.Invoke();
                return;
            }

            string? key = string.IsNullOrWhiteSpace(animationKey) ? null : animationKey;
            if (key != null && _activeTimers.TryGetValue(key, out var existing))
            {
                existing.Stop();
                existing.Dispose();
                _activeTimers.Remove(key);
            }

            int elapsed = 0;
            var timer = new Timer { Interval = 16 };
            timer.Tick += (s, e) =>
            {
                elapsed += timer.Interval;
                double t = Math.Min(1d, elapsed / (double)durationMs);
                frame(t);
                if (t >= 1d)
                {
                    timer.Stop();
                    timer.Dispose();
                    if (key != null)
                    {
                        _activeTimers.Remove(key);
                    }
                    completed?.Invoke();
                }
            };
            if (key != null)
            {
                _activeTimers[key] = timer;
            }
            timer.Start();
        }

        public static void AnimateOpacity(Form form, double from, double to, int durationMs, DialogAnimationEasing easing, Action? completed = null, string? animationKey = null)
        {
            Animate(form, durationMs, t =>
            {
                var e = Ease(t, easing);
                form.Opacity = from + ((to - from) * e);
            }, completed, animationKey);
        }

        public static void AnimateTranslate(Form form, Point from, Point to, int durationMs, DialogAnimationEasing easing, Action? completed = null, string? animationKey = null)
        {
            Animate(form, durationMs, t =>
            {
                var e = Ease(t, easing);
                form.Location = new Point(
                    (int)Math.Round(from.X + ((to.X - from.X) * e)),
                    (int)Math.Round(from.Y + ((to.Y - from.Y) * e)));
            }, completed, animationKey);
        }

        public static void ShakeDialog(Form form, int cycles = 3, int amplitude = 10, int durationMs = 280, Action? completed = null)
        {
            if (form == null || form.IsDisposed)
            {
                completed?.Invoke();
                return;
            }

            var origin = form.Location;
            Animate(form, durationMs, t =>
            {
                double envelope = 1d - t;
                double angle = t * cycles * 2 * Math.PI;
                int offsetX = (int)Math.Round(Math.Sin(angle) * amplitude * envelope);
                form.Location = new Point(origin.X + offsetX, origin.Y);
            }, () =>
            {
                if (!form.IsDisposed)
                {
                    form.Location = origin;
                }
                completed?.Invoke();
            }, animationKey: $"dialog-shake-{form.Handle}");
        }
    }
}
