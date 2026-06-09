using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Charts.Helpers
{
    /// <summary>
    /// Computes and draws linear-regression trend lines for
    /// Cartesian chart types (Line, Area, Bar, Scatter).
    /// Activated by <see cref="SeriesRenderOptions.ShowTrendLine"/>.
    /// </summary>
    internal static class TrendLineHelper
    {
        /// <summary>
        /// Simple linear regression: y = slope * x + intercept.
        /// Returns (slope, intercept, rSquared).
        /// </summary>
        public static (float slope, float intercept, float rSquared) Compute(
            List<float> xs, List<float> ys)
        {
            if (xs.Count < 2) return (0, 0, 0);
            int n = xs.Count;
            float sumX = 0, sumY = 0, sumXY = 0, sumX2 = 0, sumY2 = 0;
            for (int i = 0; i < n; i++)
            {
                float x = xs[i], y = ys[i];
                sumX += x; sumY += y;
                sumXY += x * y;
                sumX2 += x * x;
                sumY2 += y * y;
            }
            float denom = n * sumX2 - sumX * sumX;
            if (Math.Abs(denom) < 1e-10f) return (0, sumY / n, 0);
            float slope = (n * sumXY - sumX * sumY) / denom;
            float intercept = (sumY - slope * sumX) / n;

            // R-squared
            float meanY = sumY / n;
            float ssTot = sumY2 - n * meanY * meanY;
            float ssRes = 0;
            for (int i = 0; i < n; i++)
            {
                float pred = slope * xs[i] + intercept;
                ssRes += (ys[i] - pred) * (ys[i] - pred);
            }
            float rSquared = ssTot > 1e-10f ? 1f - ssRes / ssTot : 0;
            return (slope, intercept, Math.Clamp(rSquared, 0f, 1f));
        }

        /// <summary>
        /// Draws a dashed trend line across the full plot width using
        /// the computed slope and intercept.  Data points are used
        /// only for colour derivation.
        /// </summary>
        public static void DrawTrendLine(Graphics g, Rectangle plotRect,
            float slope, float intercept,
            float xMin, float xMax, float yMin, float yMax,
            Color lineColor)
        {
            float yAtXMin = slope * xMin + intercept;
            float yAtXMax = slope * xMax + intercept;

            float xRange = xMax - xMin;
            float yRange = yMax - yMin;
            if (xRange <= 0 || yRange <= 0) return;

            float sx1 = plotRect.Left;
            float sy1 = yRange > 0
                ? plotRect.Bottom - (yAtXMin - yMin) / yRange * plotRect.Height
                : plotRect.Top + plotRect.Height * 0.5f;
            float sx2 = plotRect.Right;
            float sy2 = yRange > 0
                ? plotRect.Bottom - (yAtXMax - yMin) / yRange * plotRect.Height
                : plotRect.Top + plotRect.Height * 0.5f;

            sy1 = Math.Clamp(sy1, -1e6f, 1e6f);
            sy2 = Math.Clamp(sy2, -1e6f, 1e6f);

            using var pen = new Pen(lineColor, 1.5f)
            {
                DashStyle = System.Drawing.Drawing2D.DashStyle.Dash,
                DashPattern = new float[] { 6, 3 }
            };
            g.DrawLine(pen, sx1, sy1, sx2, sy2);
        }
    }
}
