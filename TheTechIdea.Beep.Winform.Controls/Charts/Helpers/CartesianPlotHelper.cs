using System;
using System.Collections.Generic;
using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Charts.Helpers
{
    /// <summary>
    /// Shared Cartesian coordinate-to-screen-point transform used by
    /// Line, Bar, Area, Bubble, and Scatter series painters.
    /// Eliminates the ~6 copies of identical xRange/yRange math
    /// that were copy-pasted across every painter.
    /// </summary>
    internal static class CartesianPlotHelper
    {
        /// <summary>
        /// Converts a data-space (X, Y) value to a screen point
        /// clamped to ±1e6 to prevent GDI+ overflow on extreme values.
        /// </summary>
        public static PointF ToScreen(float dataX, float dataY,
            float xMin, float xMax, float yMin, float yMax,
            Rectangle plotRect)
        {
            float xRange = xMax - xMin;
            float yRange = yMax - yMin;
            float sx = xRange > 0
                ? plotRect.Left + (dataX - xMin) / xRange * plotRect.Width
                : plotRect.Left + plotRect.Width * 0.5f;
            float sy = yRange > 0
                ? plotRect.Bottom - (dataY - yMin) / yRange * plotRect.Height
                : plotRect.Top + plotRect.Height * 0.5f;
            sx = Math.Clamp(sx, -1e6f, 1e6f);
            sy = Math.Clamp(sy, -1e6f, 1e6f);
            return new PointF(sx, sy);
        }

        /// <summary>
        /// Converts a data-space (X, Y) value to a screen point with
        /// animation applied: <c>screenY = lerp(yMin, dataY, animProgress)</c>.
        /// </summary>
        public static PointF ToScreenAnimated(float dataX, float dataY,
            float xMin, float xMax, float yMin, float yMax,
            Rectangle plotRect, float animProgress)
        {
            float animatedY = yMin + (dataY - yMin) * Math.Clamp(animProgress, 0f, 1f);
            return ToScreen(dataX, animatedY, xMin, xMax, yMin, yMax, plotRect);
        }

        /// <summary>
        /// Converts all points in a series, applying animation.
        /// Returns the screen-space point list.
        /// </summary>
        public static List<PointF> ToScreenPoints(ChartDataSeries series,
            Func<ChartDataPoint, object> toX, Func<ChartDataPoint, object> toY,
            float xMin, float xMax, float yMin, float yMax,
            Rectangle plotRect, float animProgress)
        {
            var pts = new List<PointF>(series.Points?.Count ?? 0);
            if (series.Points == null) return pts;
            foreach (var p in series.Points)
            {
                float x = toX(p) is float xf ? xf : 0f;
                float y = toY(p) is float yf ? yf : 0f;
                pts.Add(ToScreenAnimated(x, y, xMin, xMax, yMin, yMax, plotRect, animProgress));
            }
            return pts;
        }

        /// <summary>
        /// Returns a default colour for a series at the given index
        /// using the palette, with a fallback to black.
        /// </summary>
        public static Color GetSeriesColor(ChartDataSeries series, int index,
            List<Color> palette)
        {
            if (series.Color != Color.Empty) return series.Color;
            if (palette != null && palette.Count > 0)
                return palette[index % palette.Count];
            return Color.Black;
        }
    }
}
