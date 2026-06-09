using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Styling;

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
            var pre = StackedPrecompute.Calculate(data, options?.Mode ?? StackedMode.None, toY);
            bool stack = pre != null;
            bool stack100 = (options?.Mode ?? StackedMode.None) == StackedMode.Stack100;
            float anim = Math.Clamp(options?.AnimationProgress ?? 1f, 0f, 1f);

            for (int sIndex = 0; sIndex < data.Count; sIndex++)
            {
                var series = data[sIndex];
                if (!series.Visible || series.Points == null || series.Points.Count == 0) continue;
                Color color = CartesianPlotHelper.GetSeriesColor(series, sIndex, palette);

                var pts = new List<PointF>();
                for (int i = 0; i < series.Points.Count; i++)
                {
                    var p = series.Points[i];
                    float x = toX(p) is float xf ? xf : i;
                    float y = toY(p) is float yf ? yf : 0f;

                    if (stack100)
                        y = pre!.Totals[i] > 0 ? y / pre.Totals[i] : 0f;
                    float xRange = xMax - xMin;
                    float yRange = yMax - yMin;

                    if (stack)
                    {
                        float prev = pre!.Cumulative[i];
                        float newCumul = prev + y;
                        float yScreenTop = yRange > 0
                            ? plotRect.Bottom - (newCumul - yMin) / yRange * plotRect.Height
                            : plotRect.Top + plotRect.Height * 0.5f;
                        float yScreenPrev = yRange > 0
                            ? plotRect.Bottom - (prev - yMin) / yRange * plotRect.Height
                            : plotRect.Top + plotRect.Height * 0.5f;
                        float sx = xRange > 0
                            ? plotRect.Left + (x - xMin) / xRange * plotRect.Width
                            : plotRect.Left + plotRect.Width * 0.5f;
                        yScreenTop = yScreenPrev - (yScreenPrev - yScreenTop) * anim;
                        sx = Math.Clamp(sx, -1e6f, 1e6f);
                        yScreenTop = Math.Clamp(yScreenTop, -1e6f, 1e6f);
                        pts.Add(new PointF(sx, yScreenTop));
                        pre.Cumulative[i] = newCumul;
                    }
                    else
                    {
                        y = yMin + (y - yMin) * anim;
                        float sx = xRange > 0
                            ? plotRect.Left + (x - xMin) / xRange * plotRect.Width
                            : plotRect.Left + plotRect.Width * 0.5f;
                        float sy = yRange > 0
                            ? plotRect.Bottom - (y - yMin) / yRange * plotRect.Height
                            : plotRect.Top + plotRect.Height * 0.5f;
                        sx = Math.Clamp(sx, -1e6f, 1e6f);
                        sy = Math.Clamp(sy, -1e6f, 1e6f);
                        pts.Add(new PointF(sx, sy));
                    }
                }

                if (pts.Count < 2) continue;

                // Build area polygon (line → bottom corners → closed)
                var area = new List<PointF>(pts)
                {
                    new PointF(pts.Last().X, plotRect.Bottom),
                    new PointF(pts.First().X, plotRect.Bottom)
                };

                // Gradient fill: semi-transparent at top, opaque-ish at bottom.
                // Creates a "fade to nothing" effect typical of modern area charts.
                using var gradientBrush = new LinearGradientBrush(
                    plotRect, Color.FromArgb(140, color), Color.FromArgb(20, color),
                    LinearGradientMode.Vertical);
                g.FillPolygon(gradientBrush, area.ToArray());

                var pen = PaintersFactory.GetPen(color, 2);
                g.DrawLines(pen, pts.ToArray());
            }

            // Data labels
            ChartDataLabelHelper.DrawDataLabels(g, data, toX, toY,
                xMin, xMax, yMin, yMax, plotRect, options, textColor);
        }

        public void UpdateHitAreas(BaseControl owner, Rectangle plotRect, List<ChartDataSeries> data,
            Func<ChartDataPoint, PointF> toScreen, Action<string, Rectangle> notifyAreaHit)
        {
            if (data == null || toScreen == null) return;

            const int hitSize = 12;
            for (int sIndex = 0; sIndex < data.Count; sIndex++)
            {
                var series = data[sIndex];
                if (!series.Visible || series.Points == null) continue;

                for (int pointIndex = 0; pointIndex < series.Points.Count; pointIndex++)
                {
                    var screenPoint = toScreen(series.Points[pointIndex]);
                    if (screenPoint == PointF.Empty) continue;

                    var hitRect = new Rectangle((int)screenPoint.X - (hitSize / 2), (int)screenPoint.Y - (hitSize / 2), hitSize, hitSize);
                    owner.AddHitArea($"AreaPoint_{sIndex}_{pointIndex}", hitRect, null, () =>
                        notifyAreaHit?.Invoke($"AreaPoint_{sIndex}_{pointIndex}", hitRect));
                }
            }
        }
    }
}
