using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.StatusCards.Helpers;
using TheTechIdea.Beep.Winform.Controls.Helpers;

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
            DrawHeader(g, inner, theme, owner, header);
            var headerHeight = 24;

            // value and delta row
            var valueArea = new Rectangle(inner.X, inner.Y + headerHeight + 4, inner.Width, (int)(inner.Height * 0.4));
            DrawValue(g, valueArea, theme, owner, valueText, 1.8f);

            using var smallFont = StatCardFontHelpers.GetDeltaFont(owner, owner?.Style ?? BeepControlStyle.Material3);
            Color deltaColor = owner.IsTrendingUp 
                ? StatCardThemeHelpers.GetTrendUpColor(theme, owner?.UseThemeColors ?? true, null)
                : StatCardThemeHelpers.GetTrendDownColor(theme, owner?.UseThemeColors ?? true, null);
            using var deltaBrush = new SolidBrush(deltaColor);
            // Measure text to ensure it fits (using cached TextUtils)
            SizeF deltaSizeF = TextUtils.MeasureText(deltaText, smallFont, int.MaxValue);
            var deltaSize = new Size((int)deltaSizeF.Width, (int)deltaSizeF.Height);
            var deltaRect = new Rectangle(Math.Max(valueArea.Left, valueArea.Right - deltaSize.Width), valueArea.Y + 4, Math.Min(deltaSize.Width, valueArea.Width), deltaSize.Height);
            TextRenderer.DrawText(g, deltaText, smallFont, deltaRect, deltaBrush.Color, TextFormatFlags.Left | TextFormatFlags.Top | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding);

            // sparkline area
            var sparkArea = new Rectangle(inner.X, valueArea.Bottom + 8, inner.Width, Math.Max(18, inner.Height - (valueArea.Bottom - inner.Y) - 28));
            DrawSparkline(g, sparkArea, theme, owner, spark);

            // info bottom line
            if (!string.IsNullOrEmpty(info))
            {
                using var infoFont = StatCardFontHelpers.GetInfoFont(owner, owner?.Style ?? BeepControlStyle.Material3);
                var infoRect = new Rectangle(inner.X, inner.Bottom - infoFont.Height - 2, inner.Width, infoFont.Height + 2);
                // Measure text to ensure it fits (using cached TextUtils)
                SizeF infoSizeF = TextUtils.MeasureText(info, infoFont, int.MaxValue);
                var infoSize = new Size((int)infoSizeF.Width, (int)infoSizeF.Height);
                infoRect.Width = Math.Min(infoSize.Width, infoRect.Width);
                Color infoColor = StatCardThemeHelpers.GetInfoColor(theme, owner?.UseThemeColors ?? true, null);
                using var infoBrush = new SolidBrush(infoColor);
                TextRenderer.DrawText(g, info, infoFont, infoRect, infoBrush.Color, TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding);
            }
        }

        private static void DrawSparkline(Graphics g, Rectangle area, IBeepTheme theme, BeepStatCard owner, float[] data)
        {
            if (area.Width <= 2 || area.Height <= 2) return;
            if (data == null || data.Length == 0)
            {
                Color infoColor = StatCardThemeHelpers.GetInfoColor(theme, owner?.UseThemeColors ?? true, null);
                using var pen = new Pen(Color.FromArgb(40, infoColor), 1f);
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

            // Use theme-aware colors for sparkline
            Color sparklineColor = owner.IsTrendingUp 
                ? StatCardThemeHelpers.GetTrendUpColor(theme, owner?.UseThemeColors ?? true, null)
                : StatCardThemeHelpers.GetTrendDownColor(theme, owner?.UseThemeColors ?? true, null);
            using var pen2 = new Pen(sparklineColor, 2f) { LineJoin = LineJoin.Round };
            g.DrawLines(pen2, pts);
        }
    }
}
