using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Charts.Helpers
{
    internal sealed class BarSeriesPainter : IChartSeriesPainter
    {
        private BaseControl _owner;
        public void Initialize(BaseControl owner) => _owner = owner;

        public void DrawSeries(Graphics g, Rectangle plotRect, List<ChartDataSeries> data,
            Func<ChartDataPoint, object> toX, Func<ChartDataPoint, object> toY,
            float xMin, float xMax, float yMin, float yMax,
            List<Color> palette, Color axisColor, Color textColor,
            SeriesRenderOptions options)
        {
            if (!data.Any() || !data.Any(s => s.Points != null && s.Points.Any())) return;

            int categories = data.Max(s => s.Points?.Count ?? 0);
            if (categories <= 0) return;

            bool stacked = options?.Stacked == true;
            int visibleSeries = data.Count(s => s.Visible);
            float categoryWidth = plotRect.Width / (float)categories;
            float barWidth = stacked ? categoryWidth * 0.6f : categoryWidth / Math.Max(1, visibleSeries);

            // precompute cumulative heights for stacked mode
            float[] cumul = stacked ? new float[categories] : null;

            int visibleIndex = 0;
            for (int sIndex = 0; sIndex < data.Count; sIndex++)
            {
                var series = data[sIndex];
                if (!series.Visible) continue;
                Color color = series.Color != Color.Empty ? series.Color : palette[visibleIndex % palette.Count];

                for (int i = 0; i < categories; i++)
                {
                    float yVal = 0f;
                    if (series.Points != null && i < series.Points.Count)
                    {
                        var p = series.Points[i];
                        yVal = toY(p) is float yf ? yf : 0f;
                    }

                    if (options != null)
                    {
                        yVal = yMin + (yVal - yMin) * Math.Clamp(options.AnimationProgress, 0f, 1f);
                    }

                    float baseX = plotRect.Left + i * categoryWidth;
                    float sx = stacked ? baseX + (categoryWidth - barWidth) / 2f
                                       : baseX + visibleIndex * barWidth;

                    float sy = plotRect.Bottom - (yVal - yMin) / (yMax - yMin) * plotRect.Height;
                    float height = plotRect.Bottom - sy;

                    if (stacked)
                    {
                        float prevHeight = 0f;
                        if (cumul != null)
                        {
                            prevHeight = cumul[i];
                        }
                        // convert cumulative base to screen pixels
                        float prevYVal = yMin + prevHeight;
                        float prevSy = plotRect.Bottom - (prevYVal - yMin) / (yMax - yMin) * plotRect.Height;
                        float stackedHeight = plotRect.Bottom - sy;
                        sy = prevSy - stackedHeight;
                        height = stackedHeight;
                        if (cumul != null)
                        {
                            cumul[i] += (yVal - yMin);
                        }
                    }

                    var b = PaintersFactory.GetSolidBrush(color);
                    var pen = PaintersFactory.GetPen(axisColor, 1);
                    g.FillRectangle(b, sx, sy, barWidth - 2, height);
                    g.DrawRectangle(pen, sx, sy, barWidth - 2, height);
                }
                visibleIndex++;
            }
        }

        public void UpdateHitAreas(BaseControl owner, Rectangle plotRect, List<ChartDataSeries> data,
            Func<ChartDataPoint, PointF> toScreen, Action<string, Rectangle> notifyAreaHit)
        {
            // Optional: register per-bar hit areas
        }
    }
}
