using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Charts.Helpers
{
    /// <summary>
    /// Draws scatter-plot data as fixed-size circles on a Cartesian
    /// grid.  Unlike <see cref="BubbleSeriesPainter"/>, the marker
    /// radius is uniform (controlled by <c>MarkerSize</c>) rather
    /// than driven by a third data dimension.
    /// </summary>
    internal sealed class ScatterSeriesPainter : IChartSeriesPainter
    {
        private BaseControl _owner;
        private const int DefaultMarkerSize = 6;

        public void Initialize(BaseControl owner) => _owner = owner;

        public void DrawSeries(Graphics g, Rectangle plotRect, List<ChartDataSeries> data,
            Func<ChartDataPoint, object> toX, Func<ChartDataPoint, object> toY,
            float xMin, float xMax, float yMin, float yMax,
            List<Color> palette, Color axisColor, Color textColor,
            SeriesRenderOptions options)
        {
            if (data == null || data.Count == 0) return;
            float anim = Math.Clamp(options?.AnimationProgress ?? 1f, 0f, 1f);

            for (int sIndex = 0; sIndex < data.Count; sIndex++)
            {
                var series = data[sIndex];
                if (!series.Visible || series.Points == null || series.Points.Count == 0) continue;

                Color color = series.Color != Color.Empty
                    ? series.Color
                    : palette[sIndex % palette.Count];

                int markerSize = DefaultMarkerSize;
                using var fill = new SolidBrush(Color.FromArgb(160, color));
                using var stroke = new Pen(color, 1.5f);

                foreach (var p in series.Points)
                {
                    float x = toX(p) is float xf ? xf : 0f;
                    float y = toY(p) is float yf ? yf : 0f;
                    y = yMin + (y - yMin) * anim;

                    float xRange = xMax - xMin;
                    float yRange = yMax - yMin;
                    float sx = xRange > 0
                        ? plotRect.Left + (x - xMin) / xRange * plotRect.Width
                        : plotRect.Left + plotRect.Width * 0.5f;
                    float sy = yRange > 0
                        ? plotRect.Bottom - (y - yMin) / yRange * plotRect.Height
                        : plotRect.Top + plotRect.Height * 0.5f;

                    if (float.IsNaN(sx) || float.IsNaN(sy)) continue;
                    sx = Math.Clamp(sx, -1e6f, 1e6f);
                    sy = Math.Clamp(sy, -1e6f, 1e6f);

                    int ms = markerSize + 1;
                    g.FillEllipse(fill, sx - ms / 2f, sy - ms / 2f, ms, ms);
                    g.DrawEllipse(stroke, sx - ms / 2f, sy - ms / 2f, ms, ms);
                }
            }

            // Data labels (shared helper)
            ChartDataLabelHelper.DrawDataLabels(g, data, toX, toY,
                xMin, xMax, yMin, yMax, plotRect, options, textColor);
        }

        public void UpdateHitAreas(BaseControl owner, Rectangle plotRect, List<ChartDataSeries> data,
            Func<ChartDataPoint, PointF> toScreen, Action<string, Rectangle> notifyAreaHit)
        {
            if (data == null) return;
            int hitSize = DefaultMarkerSize + 8;
            for (int sIdx = 0; sIdx < data.Count; sIdx++)
            {
                var series = data[sIdx];
                if (!series.Visible || series.Points == null) continue;
                for (int pIdx = 0; pIdx < series.Points.Count; pIdx++)
                {
                    var pt = toScreen(series.Points[pIdx]);
                    var r = new Rectangle((int)(pt.X - hitSize / 2f), (int)(pt.Y - hitSize / 2f), hitSize, hitSize);
                    owner.AddHitArea($"Point_{sIdx}_{pIdx}", r, null, () =>
                        notifyAreaHit?.Invoke($"Point_{sIdx}_{pIdx}", r));
                }
            }
        }
    }
}
