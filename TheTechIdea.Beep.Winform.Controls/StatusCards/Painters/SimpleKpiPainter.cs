using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.StatusCards.Painters
{
    internal sealed class SimpleKpiPainter : BaseStatCardPainter
    {
        public override string Key => nameof(StatCardPainterKind.SimpleKpi);

        public override void Paint(Graphics g, Rectangle bounds, IBeepTheme theme, BeepStatCard owner, IReadOnlyDictionary<string, object> p)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            var pad = 10;
            var inner = Inset(bounds, pad);

            var header = string.IsNullOrEmpty(owner.HeaderText) ? GetString(p, "Header", "KPI") : owner.HeaderText;
            var valueText = string.IsNullOrEmpty(owner.ValueText) ? GetString(p, "Value", "0") : owner.ValueText;
            var deltaText = string.IsNullOrEmpty(owner.PercentageText) ? GetString(p, "Delta", "+0%") : owner.PercentageText;
            var info = string.IsNullOrEmpty(owner.InfoText) ? GetString(p, "Info", string.Empty) : owner.InfoText;
            var spark = GetFloatArray(p, "Spark");

            // header
            DrawHeader(g, inner, theme, header);
            var headerHeight = 24;

            // value and delta row
            var valueArea = new Rectangle(inner.X, inner.Y + headerHeight + 4, inner.Width, (int)(inner.Height * 0.4));
            DrawValue(g, valueArea, theme, valueText, 1.8f);

            using var smallFontRaw = TheTechIdea.Beep.Vis.Modules.Managers.BeepThemesManager.ToFont(theme.SmallText);
            using var smallFont = new Font(smallFontRaw.FontFamily, smallFontRaw.Size, FontStyle.Regular);
            using var deltaBrush = new SolidBrush(owner.IsTrendingUp ? Color.Green : Color.Red);
            var deltaSize = TextRenderer.MeasureText(deltaText, smallFont);
            var deltaPoint = new Point(valueArea.Right - deltaSize.Width, valueArea.Y + 4);
            TextRenderer.DrawText(g, deltaText, smallFont, deltaPoint, deltaBrush.Color);

            // sparkline area
            var sparkArea = new Rectangle(inner.X, valueArea.Bottom + 8, inner.Width, Math.Max(18, inner.Height - (valueArea.Bottom - inner.Y) - 28));
            DrawSparkline(g, sparkArea, theme, spark, owner.IsTrendingUp ? Color.SeaGreen : Color.IndianRed);

            // info bottom line
            if (!string.IsNullOrEmpty(info))
            {
                var infoRect = new Rectangle(inner.X, inner.Bottom - smallFont.Height - 2, inner.Width, smallFont.Height + 2);
                using var infoBrush = new SolidBrush(theme.CardTextForeColor);
                TextRenderer.DrawText(g, info, smallFont, infoRect, infoBrush.Color, TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
            }
        }

        private static void DrawSparkline(Graphics g, Rectangle area, IBeepTheme theme, float[] data, Color lineColor)
        {
            if (area.Width <= 2 || area.Height <= 2) return;
            if (data == null || data.Length == 0)
            {
                using var pen = new Pen(Color.FromArgb(40, theme.CardTextForeColor), 1f);
                g.DrawLine(pen, area.Left, area.Bottom - 2, area.Right, area.Bottom - 2);
                return;
            }

            float min = data.Min();
            float max = data.Max();
            if (Math.Abs(max - min) < 1e-6) max = min + 1; // avoid div by zero

            PointF[] pts = new PointF[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                float t = (float)i / (data.Length - 1);
                float x = area.Left + t * area.Width;
                float y = area.Bottom - (data[i] - min) / (max - min) * area.Height;
                pts[i] = new PointF(x, y);
            }

            using var pen2 = new Pen(lineColor, 2f) { LineJoin = LineJoin.Round };
            g.DrawLines(pen2, pts);
        }
    }
}
