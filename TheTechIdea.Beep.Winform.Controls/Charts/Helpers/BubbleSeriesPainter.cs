using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Charts.Helpers
{
    internal sealed class BubbleSeriesPainter : IChartSeriesPainter
    {
        private BaseControl _owner;
        public void Initialize(BaseControl owner) => _owner = owner;

        public void DrawSeries(Graphics g, Rectangle plotRect, List<ChartDataSeries> data,
            Func<ChartDataPoint, object> toX, Func<ChartDataPoint, object> toY,
            float xMin, float xMax, float yMin, float yMax,
            List<Color> palette, Color axisColor, Color textColor,
            SeriesRenderOptions options)
        {
            const float maxBubbleSize = 50f;
            const float minBubbleSize = 5f;
            float maxValue = data.SelectMany(s => s.Points)
                .DefaultIfEmpty(new ChartDataPoint { Value = 1 })
                .Max(p => p.Value);
            float anim = Math.Clamp(options?.AnimationProgress ?? 1f, 0f, 1f);

            int sIndex = 0;
            foreach (var series in data)
            {
                if (!series.Visible) { sIndex++; continue; }
                Color color = CartesianPlotHelper.GetSeriesColor(series, sIndex, palette);

                foreach (var p in series.Points)
                {
                    float xv = toX(p) is float xf ? xf : 0f;
                    float yv = toY(p) is float yf ? yf : 0f;
                    var pt = CartesianPlotHelper.ToScreenAnimated(
                        xv, yv, xMin, xMax, yMin, yMax, plotRect, anim);

                    float bubbleSize = maxValue > 0 ? p.Value / maxValue * maxBubbleSize : minBubbleSize;
                    bubbleSize = Math.Max(bubbleSize * anim, minBubbleSize);

                    var brush = PaintersFactory.GetSolidBrush(Color.FromArgb(150, color));
                    var pen = PaintersFactory.GetPen(axisColor, 1);
                    g.FillEllipse(brush, pt.X - bubbleSize / 2, pt.Y - bubbleSize / 2, bubbleSize, bubbleSize);
                    g.DrawEllipse(pen, pt.X - bubbleSize / 2, pt.Y - bubbleSize / 2, bubbleSize, bubbleSize);
                }
                sIndex++;
            }
        }

        public void UpdateHitAreas(BaseControl owner, Rectangle plotRect, List<ChartDataSeries> data,
            Func<ChartDataPoint, PointF> toScreen, Action<string, Rectangle> notifyAreaHit)
        {
            if (data == null || toScreen == null) return;

            const int hitSize = 14;
            for (int sIndex = 0; sIndex < data.Count; sIndex++)
            {
                var series = data[sIndex];
                if (!series.Visible || series.Points == null) continue;

                for (int pointIndex = 0; pointIndex < series.Points.Count; pointIndex++)
                {
                    var screenPoint = toScreen(series.Points[pointIndex]);
                    if (screenPoint == PointF.Empty) continue;

                    var hitRect = new Rectangle((int)screenPoint.X - (hitSize / 2), (int)screenPoint.Y - (hitSize / 2), hitSize, hitSize);
                    owner.AddHitArea($"BubblePoint_{sIndex}_{pointIndex}", hitRect, null, () =>
                        notifyAreaHit?.Invoke($"BubblePoint_{sIndex}_{pointIndex}", hitRect));
                }
            }
        }
    }
}
