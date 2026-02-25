using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Charts.Helpers
{
    internal sealed class LineSeriesPainter : IChartSeriesPainter
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
            bool stacked = (options?.Mode ?? StackedMode.None) != StackedMode.None;
            bool stack100 = (options?.Mode ?? StackedMode.None) == StackedMode.Stack100;
            int n = data.Max(s => s.Points?.Count ?? 0);

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
                    totals[i] = sum == 0 ? 1f : sum;
                }
            }

            float[] cumul = stacked ? new float[n] : null;
            float anim = Math.Clamp(options?.AnimationProgress ?? 1f, 0f, 1f);

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
                    float yVal = toY(p) is float yf ? yf : 0f;
                    if (stack100) yVal = totals[i] > 0 ? yVal / totals[i] : 0f;
                    if (stacked)
                    {
                        float prev = cumul[i];
                        yVal += prev;
                        cumul[i] = yVal;
                    }
                    yVal = yMin + (yVal - yMin) * anim;
                    float xRange = xMax - xMin;
                    float yRange = yMax - yMin;
                    float sx = xRange > 0
                        ? plotRect.Left + (x - xMin) / xRange * plotRect.Width
                        : plotRect.Left + plotRect.Width * 0.5f;
                    float sy = yRange > 0
                        ? plotRect.Bottom - (yVal - yMin) / yRange * plotRect.Height
                        : plotRect.Top + plotRect.Height * 0.5f;

                    // Clamp to prevent GDI+ overflow on extreme values
                    sx = Math.Clamp(sx, -1e6f, 1e6f);
                    sy = Math.Clamp(sy, -1e6f, 1e6f);
                    pts.Add(new PointF(sx, sy));
                }

                if (series.ShowLine && pts.Count > 1)
                {
                    var pen = PaintersFactory.GetPen(color, 2);
                    if (options?.SmoothLines == true)
                    {
                        var spline = CatmullRomSpline(pts, 12);
                        g.DrawLines(pen, spline);
                    }
                    else
                    {
                        g.DrawLines(pen, pts.ToArray());
                    }
                }

                if ((options?.ShowMarkers ?? true) && series.ShowPoint)
                {
                    foreach (var (pt, i) in pts.Select((p, i) => (p, i)))
                    {
                        var p = series.Points[i];
                        DrawMarker(g, pt, series.DataPointStyle, color, 5);
                        var hitRect = new Rectangle((int)pt.X - 6, (int)pt.Y - 6, 12, 12);
                        _owner.AddHitArea($"Point_{sIndex}_{i}", hitRect, null, null);
                    }
                }
            }
        }

        private static PointF[] CatmullRomSpline(List<PointF> points, int subdivisions)
        {
            if (points.Count < 2) return points.ToArray();
            var output = new List<PointF>();
            for (int i = 0; i < points.Count - 1; i++)
            {
                PointF p0 = i == 0 ? points[i] : points[i - 1];
                PointF p1 = points[i];
                PointF p2 = points[i + 1];
                PointF p3 = i + 2 < points.Count ? points[i + 2] : points[i + 1];
                for (int j = 0; j <= subdivisions; j++)
                {
                    float t = j / (float)subdivisions;
                    float t2 = t * t;
                    float t3 = t2 * t;
                    float x = 0.5f * ((2 * p1.X) + (-p0.X + p2.X) * t + (2 * p0.X - 5 * p1.X + 4 * p2.X - p3.X) * t2 + (-p0.X + 3 * p1.X - 3 * p2.X + p3.X) * t3);
                    float y = 0.5f * ((2 * p1.Y) + (-p0.Y + p2.Y) * t + (2 * p0.Y - 5 * p1.Y + 4 * p2.Y - p3.Y) * t2 + (-p0.Y + 3 * p1.Y - 3 * p2.Y + p3.Y) * t3);
                    output.Add(new PointF(x, y));
                }
            }
            return output.ToArray();
        }

        private static void DrawMarker(Graphics g, PointF pt, ChartDataPointStyle style, Color color, float s)
        {
            var brush = PaintersFactory.GetSolidBrush(color);
            var pen = PaintersFactory.GetPen(Color.White, 1);
            switch (style)
            {
                case ChartDataPointStyle.Square:
                    g.FillRectangle(brush, pt.X - s, pt.Y - s, s * 2, s * 2);
                    g.DrawRectangle(pen, pt.X - s, pt.Y - s, s * 2, s * 2);
                    break;
                case ChartDataPointStyle.Diamond:
                    var diamond = new PointF[]
                    {
                        new PointF(pt.X, pt.Y - s),
                        new PointF(pt.X + s, pt.Y),
                        new PointF(pt.X, pt.Y + s),
                        new PointF(pt.X - s, pt.Y)
                    };
                    g.FillPolygon(brush, diamond);
                    g.DrawPolygon(pen, diamond);
                    break;
                case ChartDataPointStyle.Triangle:
                    var tri = new PointF[]
                    {
                        new PointF(pt.X, pt.Y - s),
                        new PointF(pt.X + s, pt.Y + s),
                        new PointF(pt.X - s, pt.Y + s)
                    };
                    g.FillPolygon(brush, tri);
                    g.DrawPolygon(pen, tri);
                    break;
                case ChartDataPointStyle.Circle:
                default:
                    g.FillEllipse(brush, pt.X - s, pt.Y - s, s * 2, s * 2);
                    g.DrawEllipse(pen, pt.X - s, pt.Y - s, s * 2, s * 2);
                    break;
            }
        }

        public void UpdateHitAreas(BaseControl owner, Rectangle plotRect, List<ChartDataSeries> data,
            Func<ChartDataPoint, PointF> toScreen, Action<string, Rectangle> notifyAreaHit)
        {
            // Optional: extend hit areas later for hover tooltips
        }
    }
}
