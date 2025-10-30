using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Charts.Helpers
{
    internal sealed class PieSeriesPainter : IChartSeriesPainter
    {
        private BaseControl _owner;
        public void Initialize(BaseControl owner) => _owner = owner;

        public void DrawSeries(Graphics g, Rectangle plotRect, List<ChartDataSeries> data,
            Func<ChartDataPoint, object> toX, Func<ChartDataPoint, object> toY,
            float xMin, float xMax, float yMin, float yMax,
            List<Color> palette, Color axisColor, Color textColor,
            SeriesRenderOptions options)
        {
            var series = data.FirstOrDefault();
            if (series == null || !series.Visible || series.Points == null || !series.Points.Any()) return;

            float totalValue = series.Points.Sum(p => p.Value);
            if (totalValue <= 0) return;

            int pieDiameter = Math.Min(plotRect.Width, plotRect.Height) - 20;
            Rectangle pieRect = new Rectangle(
                plotRect.Left + (plotRect.Width - pieDiameter) / 2,
                plotRect.Top + (plotRect.Height - pieDiameter) / 2,
                pieDiameter,
                pieDiameter);

            float startAngle = 0f;
            int colorIndex = 0;
            foreach (var p in series.Points)
            {
                float sliceValue = p.Value;
                if (sliceValue <= 0) continue;
                float sweepAngle = sliceValue / totalValue * 360f * Math.Clamp(options?.AnimationProgress ?? 1f, 0f, 1f);
                Color color = p.Color != Color.Empty ? p.Color : palette[colorIndex % palette.Count];
                var brush = PaintersFactory.GetSolidBrush(color);
                var pen = PaintersFactory.GetPen(axisColor, 1);
                g.FillPie(brush, pieRect, startAngle, sweepAngle);
                g.DrawPie(pen, pieRect, startAngle, sweepAngle);
                startAngle += sweepAngle;
                colorIndex++;
            }
        }

        public void UpdateHitAreas(BaseControl owner, Rectangle plotRect, List<ChartDataSeries> data,
            Func<ChartDataPoint, PointF> toScreen, Action<string, Rectangle> notifyAreaHit) { }
    }
}
