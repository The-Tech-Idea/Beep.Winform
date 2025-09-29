using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.StatusCards.Painters
{
    internal sealed class PerformancePainter : BaseStatCardPainter
    {
        public override string Key => nameof(StatCardPainterKind.Performance);

        public override void Paint(Graphics g, Rectangle bounds, IBeepTheme theme, BeepStatCard owner, IReadOnlyDictionary<string, object> p)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            var pad = 12;
            var inner = Inset(bounds, pad);

            var header = string.IsNullOrEmpty(owner.HeaderText) ? GetString(p, "Header", "Performance") : owner.HeaderText;
            var valueText = string.IsNullOrEmpty(owner.ValueText) ? GetString(p, "Value", "+0.0%") : owner.ValueText;
            DrawHeader(g, inner, theme, header);

            var headerHeight = 26;
            var valueRect = new Rectangle(inner.X, inner.Y + headerHeight, inner.Width, 28);
            DrawValue(g, valueRect, theme, valueText, 1.4f);

            var chartArea = new Rectangle(inner.X, valueRect.Bottom + 6, inner.Width, Math.Max(28, inner.Height - (valueRect.Bottom - inner.Y) - 16));
            DrawDottedBackground(g, chartArea, theme);
            DrawCenterLine(g, chartArea, theme);
        }

        private static void DrawDottedBackground(Graphics g, Rectangle area, IBeepTheme theme)
        {
            var dotColor = Color.FromArgb(60, theme.CardTextForeColor);
            using var b = new SolidBrush(dotColor);
            int step = 10;
            for (int y = area.Y; y < area.Bottom; y += step)
            {
                for (int x = area.X; x < area.Right; x += step)
                {
                    g.FillEllipse(b, x, y, 1, 1);
                }
            }
        }

        private static void DrawCenterLine(Graphics g, Rectangle area, IBeepTheme theme)
        {
            using var pen = new Pen(theme.PrimaryColor.IsEmpty ? Color.Goldenrod : theme.PrimaryColor, 2f);
            using var pen2 = new Pen(Color.FromArgb(120, pen.Color), 2f) { DashStyle = DashStyle.Dot };
            int midY = area.Y + area.Height / 2;
            g.DrawLine(pen2, area.Left, midY, area.Right, midY);

            // A small animated-like wavy line through the center
            var points = new PointF[12];
            for (int i = 0; i < points.Length; i++)
            {
                float t = (float)i / (points.Length - 1);
                float x = area.Left + t * area.Width;
                float y = midY + (float)Math.Sin(t * Math.PI * 2) * (area.Height * 0.2f);
                points[i] = new PointF(x, y);
            }
            g.DrawLines(pen, points);
        }
    }
}
