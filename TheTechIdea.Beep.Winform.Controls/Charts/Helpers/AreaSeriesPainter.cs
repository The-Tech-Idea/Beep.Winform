using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.Charts.Helpers
{
    internal sealed class AreaSeriesPainter : IChartSeriesPainter
    {
        private BaseControl _owner;
        public void Initialize(BaseControl owner) => _owner = owner;

        public void DrawSeries(Graphics g, Rectangle plotRect, List<ChartDataSeries> data,
            Func<ChartDataPoint, object> toX, Func<ChartDataPoint, object> toY,
            float xMin, float xMax, float yMin, float yMax,
            List<Color> palette, Color axisColor, Color textColor,
            SeriesRenderOptions options)
        {
            if (data == null || data.Count == 0) return;

            int n = data.Max(s => s.Points?.Count ?? 0);
            float anim = Math.Clamp(options?.AnimationProgress ?? 1f, 0f, 1f);
            bool stack = (options?.Mode ?? StackedMode.None) != StackedMode.None;
            bool stack100 = (options?.Mode ?? StackedMode.None) == StackedMode.Stack100;

            // Precompute totals per point index for 100%
            float[] totals = null;
            if (stack100)
            {
                totals = new float[n];
                for (int i = 0; i < n; i++)
                {
                    float sum = 0f;
                    foreach (var s in data)
                    {
                        if (!s.Visible || s.Points == null || i >= s.Points.Count) continue;
                        sum += toY(s.Points[i]) is float yf ? yf : 0f;
                    }
                    totals[i] = sum == 0 ? 1f : sum; // avoid div by zero
                }
            }

            // cumulative stack
            float[] cumul = stack ? new float[n] : null;

            for (int sIndex = 0; sIndex < data.Count; sIndex++)
            {
                var series = data[sIndex];
                if (!series.Visible || series.Points == null || series.Points.Count == 0) continue;
                Color color = series.Color != Color.Empty ? series.Color : palette[sIndex % palette.Count];

                var pts = new List<PointF>();
                for (int i = 0; i < series.Points.Count; i++)
                {
                    var p = series.Points[i];
                    float x = toX(p) is float xf ? xf : i;
                    float y = toY(p) is float yf ? yf : 0f;

                    if (stack100)
                    {
                        y = totals[i] > 0 ? y / totals[i] : 0f;
                    }
                    if (stack)
                    {
                        float prev = cumul[i];
                        float newCumul = prev + y;
                        float yScreenTop = plotRect.Bottom - (newCumul - yMin) / (yMax - yMin) * plotRect.Height;
                        float yScreenPrev = plotRect.Bottom - (prev - yMin) / (yMax - yMin) * plotRect.Height;
                        float sx = plotRect.Left + (x - xMin) / (xMax - xMin) * plotRect.Width;
                        yScreenTop = yScreenPrev - (yScreenPrev - yScreenTop) * anim; // animate
                        pts.Add(new PointF(sx, yScreenTop));
                        cumul[i] = newCumul;
                    }
                    else
                    {
                        y = yMin + (y - yMin) * anim;
                        float sx = plotRect.Left + (x - xMin) / (xMax - xMin) * plotRect.Width;
                        float sy = plotRect.Bottom - (y - yMin) / (yMax - yMin) * plotRect.Height;
                        pts.Add(new PointF(sx, sy));
                    }
                }

                if (pts.Count < 2) continue;

                // build area polygon
                var area = new List<PointF>(pts)
                {
                    new PointF(pts.Last().X, plotRect.Bottom),
                    new PointF(pts.First().X, plotRect.Bottom)
                };

                using var brush = new SolidBrush(Color.FromArgb(100, color));
                g.FillPolygon(brush, area.ToArray());
                using var pen = new Pen(color, 2);
                g.DrawLines(pen, pts.ToArray());
            }
        }

        public void UpdateHitAreas(BaseControl owner, Rectangle plotRect, List<ChartDataSeries> data,
            Func<ChartDataPoint, PointF> toScreen, Action<string, Rectangle> notifyAreaHit) { }
    }
}
