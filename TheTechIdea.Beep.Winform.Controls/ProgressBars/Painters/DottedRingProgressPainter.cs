using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.ProgressBars.Painters
{
    internal sealed class DottedRingProgressPainter : IProgressPainter
    {
        public string Key => nameof(ProgressPainterKind.DottedRing);

        public void Paint(Graphics g, Rectangle bounds, IBeepTheme theme, BeepProgressBar owner, IReadOnlyDictionary<string, object> p)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            int dots = GetInt(p, "Dots", 24);
            int pad = 6;
            var rect = new Rectangle(bounds.X + pad, bounds.Y + pad, bounds.Width - pad*2, bounds.Height - pad*2);
            var center = new PointF(rect.X + rect.Width/2f, rect.Y + rect.Height/2f);
            float radius = Math.Min(rect.Width, rect.Height)/2f - 6;
            float pct = Math.Max(0f, Math.Min(1f, owner.Value / (float)Math.Max(1, owner.Maximum)));
            int active = (int)(dots * pct + 0.5f);
            var on = theme.PrimaryColor.IsEmpty ? Color.SeaGreen : theme.PrimaryColor;
            var off = Color.FromArgb(100, theme.CardTextForeColor);
            for (int i = 0; i < dots; i++)
            {
                double angle = -Math.PI/2 + i * (2*Math.PI / dots);
                float x = center.X + (float)(radius * Math.Cos(angle));
                float y = center.Y + (float)(radius * Math.Sin(angle));
                int size = Math.Max(4, (int)(radius*0.08f));
                using var b = new SolidBrush(i < active ? on : off);
                g.FillEllipse(b, x - size/2f, y - size/2f, size, size);
            }
        }

        public void UpdateHitAreas(BeepProgressBar owner, Rectangle bounds, IBeepTheme theme, IReadOnlyDictionary<string, object> p, Action<string, Rectangle> register)
        {
            register("RingDots", bounds);
        }

        private static int GetInt(IReadOnlyDictionary<string, object> p, string key, int fallback)
            => p != null && p.TryGetValue(key, out var v) && v is IConvertible ? Convert.ToInt32(v) : fallback;
    }
}
