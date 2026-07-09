using System;
using System.Collections.Generic;
using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Charts.Helpers
{
    internal sealed class RightSideLegendPainter : IChartLegendPainter
    {
        public void DrawLegend(Graphics g, Rectangle chartRect, List<ChartDataSeries> data, List<Color> palette, Font font, Color textColor, Color backColor, Color shapeColor, BaseControl owner, Action<int> onToggleSeries, Action<string, Rectangle> notifyAreaHit = null, LegendPlacement placement = LegendPlacement.Right)
        {
            if (data == null || data.Count == 0) return;
            int S(int px) => DpiScalingHelper.ScaleValue(px, owner);

            int itemHeight = S(20);
            int swatch = S(15);
            int padding = S(6);
            int width = S(160);

            Rectangle legendRect = placement switch
            {
                LegendPlacement.Top => new Rectangle(chartRect.Left, chartRect.Top - (itemHeight + padding) * data.Count - padding, chartRect.Width, (itemHeight + padding) * data.Count),
                LegendPlacement.Bottom => new Rectangle(chartRect.Left, chartRect.Bottom + padding, chartRect.Width, (itemHeight + padding) * data.Count),
                LegendPlacement.InsideTopRight => new Rectangle(chartRect.Right - width - padding, chartRect.Top + padding, width, (itemHeight + padding) * data.Count),
                _ => new Rectangle(chartRect.Right + padding, chartRect.Top, width, (itemHeight + padding) * data.Count)
            };

            var backBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(180, backColor));
            var pen = PaintersFactory.GetPen(shapeColor, 1);
            var useFont = font ?? PaintersFactory.GetFont(SystemFonts.DefaultFont);

            g.FillRectangle(backBrush, legendRect);
            owner.AddHitArea("Legend", legendRect, null, () => notifyAreaHit?.Invoke("Legend", legendRect));

            int currentY = legendRect.Top + padding / 2;
            for (int i = 0; i < data.Count; i++)
            {
                var series = data[i];
                Color seriesColor = CartesianPlotHelper.GetSeriesColor(series, i, palette);

                var itemRect = new Rectangle(legendRect.Left + padding, currentY, legendRect.Width - (padding * 2), itemHeight - 2);
                var swatchRect = new Rectangle(itemRect.Left, itemRect.Top + 2, swatch, swatch);

                var swatchBrush = PaintersFactory.GetSolidBrush(seriesColor);
                g.FillRectangle(swatchBrush, swatchRect);
                g.DrawRectangle(pen, swatchRect);

                string name = string.IsNullOrEmpty(series.Name) ? $"Series {i + 1}" : series.Name;
                TextRenderer.DrawText(g, name, useFont,
                    new Point(swatchRect.Right + S(6), itemRect.Top + 2), textColor,
                    TextFormatFlags.NoPrefix | TextFormatFlags.NoPadding);

                owner.AddHitArea($"LegendItem_{i}", itemRect, null, () =>
                {
                    onToggleSeries?.Invoke(i);
                    notifyAreaHit?.Invoke($"LegendItem_{i}", itemRect);
                });
                currentY += itemHeight;
            }
        }
    }
}
