using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.StatusCards.Painters
{
    internal sealed class HeartRatePainter : BaseStatCardPainter
    {
        public override string Key => nameof(StatCardPainterKind.HeartRate);

        public override void Paint(Graphics g, Rectangle bounds, IBeepTheme theme, BeepStatCard owner, IReadOnlyDictionary<string, object> p)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var pad = 10;
            var inner = Inset(bounds, pad);

            var header = string.IsNullOrEmpty(owner.HeaderText) ? GetString(p, "Header", "Heart Rate") : owner.HeaderText;
            var valueText = string.IsNullOrEmpty(owner.ValueText) ? GetString(p, "Value", "76 bpm") : owner.ValueText;
            var labels = GetStringArray(p, "Labels");
            var series = GetFloatArray(p, "Series");

            // Fallback demo data
            if (series == null || series.Length == 0)
            {
                series = new float[] { 65, 68, 72, 70, 74, 78, 80, 77, 75, 73, 70, 68 };
            }

            DrawHeader(g, inner, theme, owner, header);
            var headerHeight = 26;

            var valueRect = new Rectangle(inner.X, inner.Y + headerHeight, inner.Width, 28);
            DrawValue(g, valueRect, theme, owner, valueText, 1.3f);

            // chart area below value
            var chartArea = new Rectangle(inner.X, valueRect.Bottom + 6, inner.Width, Math.Max(28, inner.Height - (valueRect.Bottom - inner.Y) - 14));
            DrawHeartRateChart(g, chartArea, series, labels, theme);
        }

        private static void DrawHeartRateChart(Graphics g, Rectangle area, float[] series, string[] labels, IBeepTheme theme)
        {
            if (area.Width <= 4 || area.Height <= 4) return;

            float min = series.Min();
            float max = series.Max();
            if (Math.Abs(max - min) < 1e-6f)
            {
                max = min + 1f; // avoid div by zero
            }

            // Padding inside chart for axes/labels
            int leftPad = 4;
            int rightPad = 2;
            int topPad = 2;
            int bottomPad = labels != null && labels.Length > 0 ? 18 : 6;
            var plot = new Rectangle(area.X + leftPad, area.Y + topPad, Math.Max(0, area.Width - leftPad - rightPad), Math.Max(0, area.Height - topPad - bottomPad));
            if (plot.Width <= 0 || plot.Height <= 0) return;

            // dotted background
            using (var dotPen = new Pen(Color.FromArgb(40, theme.CardTextForeColor)))
            {
                dotPen.DashStyle = DashStyle.Dot;
                int rows = 4;
                for (int i = 1; i < rows; i++)
                {
                    int y = plot.Y + (plot.Height * i) / rows;
                    g.DrawLine(dotPen, plot.Left, y, plot.Right, y);
                }
            }

            int n = series.Length;
            int gap = Math.Max(2, plot.Width / Math.Max(16, n));
            int barW = Math.Max(6, (plot.Width - gap * (n + 1)) / n);
            int x = plot.X + gap;

            var c = theme.PrimaryColor.IsEmpty ? Color.Crimson : theme.PrimaryColor;
            var cLight = Color.FromArgb(180, c);

            for (int i = 0; i < n; i++)
            {
                float v = series[i];
                int h = (int)Math.Round(((v - min) / (max - min)) * plot.Height);
                h = Math.Min(plot.Height - 1, Math.Max(2, h));
                var bar = new Rectangle(x, plot.Bottom - h, barW, h);
                FillRoundedBar(g, bar, cLight, 4);
                x += barW + gap;
            }

            // baseline axis
            using (var axisPen = new Pen(Color.FromArgb(80, theme.CardTextForeColor), 1f))
            {
                g.DrawLine(axisPen, plot.Left, plot.Bottom, plot.Right, plot.Bottom);
            }

            // x labels (optional)
            if (labels != null && labels.Length > 0)
            {
                using var smallFontRaw =  BeepThemesManager.ToFont(theme.SmallText);
                using var small = new Font(smallFontRaw.FontFamily, smallFontRaw.Size, FontStyle.Regular);
                for (int i = 0, xi = plot.X + gap; i < n && i < labels.Length; i++, xi += barW + gap)
                {
                    var txt = labels[i];
                    SizeF sizeF = TextUtils.MeasureText(txt, small, int.MaxValue);
                    var size = new Size((int)sizeF.Width, (int)sizeF.Height);
                    var pt = new Point(xi + (barW - size.Width) / 2, plot.Bottom + 2);
                    TextRenderer.DrawText(g, txt, small, pt, theme.CardTextForeColor);
                }
            }
        }

        private static void FillRoundedBar(Graphics g, Rectangle r, Color color, int radius)
        {
            using var path = new GraphicsPath();
            int d = radius * 2;
            path.AddArc(r.X, r.Y, d, d, 180, 90);
            path.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            path.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            path.CloseFigure();

            using var brush = new SolidBrush(color);
            g.FillPath(brush, path);
        }
    }
}
