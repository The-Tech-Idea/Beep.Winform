using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.ProgressBars.Painters
{
    internal sealed class RadialSegmentedPainter : IProgressPainter
    {
        public string Key => nameof(ProgressPainterKind.RadialSegmented);

        public void Paint(Graphics g, Rectangle bounds, IBeepTheme theme, BeepProgressBar owner, IReadOnlyDictionary<string, object> p)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            int segments = GetInt(p, "Segments", 24);
            float gap = GetFloat(p, "GapAngle", 2f);
            float start = GetFloat(p, "StartAngle", -90f);
            float totalSweep = GetFloat(p, "SweepAngle", 360f);
            int thickness = GetInt(p, "Thickness", Math.Max(6, Math.Min(bounds.Width, bounds.Height)/10));
            var color = theme.PrimaryColor.IsEmpty ? Color.SeaGreen : theme.PrimaryColor;
            var off = Color.FromArgb(40, color);

            var rect = Rectangle.Inflate(bounds, -thickness - 4, -thickness - 4);
            float pct = Math.Max(0f, Math.Min(1f, owner.Value/(float)Math.Max(1, owner.Maximum)));
            int active = (int)Math.Round(segments * pct);

            float sweepPer = (totalSweep - segments * gap) / segments;
            using var penOn = new Pen(color, thickness) { StartCap = LineCap.Round, EndCap = LineCap.Round };
            using var penOff = new Pen(off, thickness) { StartCap = LineCap.Round, EndCap = LineCap.Round };
            float ang = start;
            for (int i = 0; i < segments; i++)
            {
                var pen = i < active ? penOn : penOff;
                g.DrawArc(pen, rect, ang, sweepPer);
                ang += sweepPer + gap;
            }

            // Center text
            var txt = GetString(p, "CenterText", $"{(int)(pct*100)}%");
            using var f = new Font("Segoe UI", Math.Max(8, rect.Height/6f), FontStyle.Bold);
            var sz = TextUtils.MeasureText(g,txt, f);
            var pt = new PointF(rect.X + (rect.Width - sz.Width)/2, rect.Y + (rect.Height - sz.Height)/2);
            using var br = new SolidBrush(theme.CardTextForeColor);
            g.DrawString(txt, f, br, pt);
        }

        public void UpdateHitAreas(BeepProgressBar owner, Rectangle bounds, IBeepTheme theme, IReadOnlyDictionary<string, object> p, Action<string, Rectangle> register)
        {
            register("Ring", bounds);
        }

        private static int GetInt(IReadOnlyDictionary<string, object> p, string key, int fallback)
            => p != null && p.TryGetValue(key, out var v) && v is IConvertible ? Convert.ToInt32(v) : fallback;
        private static float GetFloat(IReadOnlyDictionary<string, object> p, string key, float fallback)
            => p != null && p.TryGetValue(key, out var v) && v is IConvertible ? Convert.ToSingle(v) : fallback;
        private static string GetString(IReadOnlyDictionary<string, object> p, string key, string fallback)
            => p != null && p.TryGetValue(key, out var v) && v is string s ? s : fallback;
    }
}
