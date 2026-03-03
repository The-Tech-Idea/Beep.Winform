using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;

namespace TheTechIdea.Beep.Winform.Controls.ProgressBars.Painters
{
    internal sealed class RingProgressPainter : IProgressPainter
    {
        public string Key => nameof(ProgressPainterKind.Ring);

        public void Paint(Graphics g, Rectangle bounds, IBeepTheme theme, BeepProgressBar owner, IReadOnlyDictionary<string, object> p)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            int thickness = GetInt(p, "Thickness", Math.Max(6, bounds.Height/8));
            int pad = thickness/2 + 2;
            var ringRect = new Rectangle(bounds.X + pad, bounds.Y + pad, bounds.Width - pad*2, bounds.Height - pad*2);
            using var backPen = new Pen(Color.FromArgb(owner.Enabled ? 40 : 24, theme.CardTextForeColor), thickness) { StartCap = LineCap.Round, EndCap = LineCap.Round };
            var activeColor = theme.PrimaryColor.IsEmpty ? Color.SeaGreen : theme.PrimaryColor;
            if (!owner.Enabled)
            {
                activeColor = Color.FromArgb(120, activeColor);
            }
            using var forePen = new Pen(activeColor, thickness) { StartCap = LineCap.Round, EndCap = LineCap.Round };
            g.DrawArc(backPen, ringRect, -90, 360);
            float pct = owner.DisplayProgressPercentageAccessor;
            g.DrawArc(forePen, ringRect, -90, 360f * pct);

            // center text
            var text = p != null && p.TryGetValue("CenterText", out var v) && v is string s ? s : $"{(int)(pct*100)}%";
            using var f = BeepThemesManager.ToFont(
                theme.ProgressBarFont?.FontFamily ?? theme.FontFamily,
                Math.Max(8, ringRect.Height / 6f),
                FontWeight.Bold,
                FontStyle.Bold);
            var sz = TextUtils.MeasureText(g,text, f);
            var pt = new PointF(ringRect.X + (ringRect.Width - sz.Width)/2, ringRect.Y + (ringRect.Height - sz.Height)/2);
            using var br = new SolidBrush(owner.Enabled ? theme.CardTextForeColor : Color.FromArgb(140, theme.CardTextForeColor));
            g.DrawString(text, f, br, pt);
        }

        public void UpdateHitAreas(BeepProgressBar owner, Rectangle bounds, IBeepTheme theme, IReadOnlyDictionary<string, object> p, Action<string, Rectangle> register)
        {
            register("Ring", bounds);
        }

        private static int GetInt(IReadOnlyDictionary<string, object> p, string key, int fallback)
            => p != null && p.TryGetValue(key, out var v) && v is IConvertible ? Convert.ToInt32(v) : fallback;
    }
}
