using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.ProgressBars.Painters
{
    internal sealed class LinearBadgePainter : IProgressPainter
    {
        public string Key => nameof(ProgressPainterKind.LinearBadge);

        public void Paint(Graphics g, Rectangle bounds, IBeepTheme theme, BeepProgressBar owner, IReadOnlyDictionary<string, object> p)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            // draw the base linear bar by delegating to LinearProgressPainter
            var linear = new LinearProgressPainter();
            linear.Paint(g, bounds, theme, owner, p);

            // compute badge position along the progress line
            float pct = owner.DisplayProgressPercentageAccessor;
            var rect = bounds; rect.Inflate(-owner.BorderThickness, -owner.BorderThickness);
            int badgeRadius = GetInt(p, "BadgeRadius", Math.Max(10, rect.Height));
            int offsetY = GetInt(p, "BadgeOffsetY", -badgeRadius - 6);
            string text = GetString(p, "BadgeText", $"{(int)(pct * 100)}%");
            var back = GetColor(p, "BadgeBackColor", theme.PrimaryColor.IsEmpty ? Color.SeaGreen : theme.PrimaryColor);
            var fore = GetColor(p, "BadgeForeColor", theme.OnPrimaryColor.IsEmpty ? Color.White : theme.OnPrimaryColor);

            int cx = rect.Left + (int)(pct * rect.Width);
            int cy = rect.Top + offsetY;
            var ellipse = new Rectangle(cx - badgeRadius, cy - badgeRadius, badgeRadius * 2, badgeRadius * 2);
            using (var shadow = new SolidBrush(Color.FromArgb(40, 0, 0, 0))) g.FillEllipse(shadow, ellipse.X + 2, ellipse.Y + 3, ellipse.Width, ellipse.Height);
            using (var b = new SolidBrush(back)) g.FillEllipse(b, ellipse);
            using (var f = new Font(owner.TextFont?.FontFamily ?? new Font("Segoe UI", 9f).FontFamily, Math.Max(8, badgeRadius/2f), FontStyle.Bold))
            using (var tb = new SolidBrush(fore))
            {
                var sz = TextUtils.MeasureText(g, text, f);
                g.DrawString(text, f, tb, new PointF(ellipse.X + (ellipse.Width - sz.Width)/2f, ellipse.Y + (ellipse.Height - sz.Height)/2f));
            }
        }

        public void UpdateHitAreas(BeepProgressBar owner, Rectangle bounds, IBeepTheme theme, IReadOnlyDictionary<string, object> p, Action<string, Rectangle> register)
        {
            // no clickable areas
        }

        private static int GetInt(IReadOnlyDictionary<string, object> p, string key, int fallback)
            => p != null && p.TryGetValue(key, out var v) && v is IConvertible ? Convert.ToInt32(v) : fallback;
        private static string GetString(IReadOnlyDictionary<string, object> p, string key, string fallback)
            => p != null && p.TryGetValue(key, out var v) && v is string s ? s : fallback;
        private static Color GetColor(IReadOnlyDictionary<string, object> p, string key, Color fallback)
            => p != null && p.TryGetValue(key, out var v) && v is Color c ? c : fallback;
    }
}
