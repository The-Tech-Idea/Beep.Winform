using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Charts.Helpers
{
    internal static class ChartInputHelper
    {
        private const float MinViewportRange = 0.0001f;

        public static void HandleKeyboardZoom(BeepChart chart, bool zoomIn, float stepPercent = 0.10f)
        {
            float normalizedStep = Math.Clamp(stepPercent, 0.01f, 0.90f);
            float factor = zoomIn ? 1f - normalizedStep : 1f + normalizedStep;
            float xRange = chart.ViewportXMax - chart.ViewportXMin;
            float yRange = chart.ViewportYMax - chart.ViewportYMin;
            if (xRange <= 0 || yRange <= 0) return;

            float xMid = (chart.ViewportXMin + chart.ViewportXMax) * 0.5f;
            float yMid = (chart.ViewportYMin + chart.ViewportYMax) * 0.5f;
            float newXHalf = (xRange * factor) * 0.5f;
            float newYHalf = (yRange * factor) * 0.5f;

            ApplyViewport(chart, xMid - newXHalf, xMid + newXHalf, yMid - newYHalf, yMid + newYHalf);
            chart.Invalidate();
        }

        public static void HandleKeyboardPan(BeepChart chart, float horizontalPercent, float verticalPercent)
        {
            float xRange = chart.ViewportXMax - chart.ViewportXMin;
            float yRange = chart.ViewportYMax - chart.ViewportYMin;
            if (xRange <= 0 || yRange <= 0) return;

            float panX = xRange * horizontalPercent;
            float panY = yRange * verticalPercent;

            ApplyViewport(
                chart,
                chart.ViewportXMin + panX,
                chart.ViewportXMax + panX,
                chart.ViewportYMin + panY,
                chart.ViewportYMax + panY);
            chart.Invalidate();
        }

        public static void HandleMouseWheel(BeepChart chart, MouseEventArgs e, float stepPercent = 0.10f)
        {
            float normalizedStep = Math.Clamp(stepPercent, 0.01f, 0.90f);
            float zoomFactorChange = e.Delta > 0 ? 1f - normalizedStep : 1f + normalizedStep;

            if (chart.ChartDrawingRect.Width <= 0 || chart.ChartDrawingRect.Height <= 0) return;

            float xRange = chart.ViewportXMax - chart.ViewportXMin;
            float yRange = chart.ViewportYMax - chart.ViewportYMin;
            if (xRange <= 0 || yRange <= 0) return;

            float mouseXRatio = (e.X - chart.ChartDrawingRect.Left) / (float)chart.ChartDrawingRect.Width;
            float mouseYRatio = 1.0f - (e.Y - chart.ChartDrawingRect.Top) / (float)chart.ChartDrawingRect.Height;

            float newXMin = chart.ViewportXMin + xRange * mouseXRatio * (1 - zoomFactorChange);
            float newXMax = chart.ViewportXMax - xRange * (1 - mouseXRatio) * (1 - zoomFactorChange);
            float newYMin = chart.ViewportYMin + yRange * mouseYRatio * (1 - zoomFactorChange);
            float newYMax = chart.ViewportYMax - yRange * (1 - mouseYRatio) * (1 - zoomFactorChange);

            ApplyViewport(chart, newXMin, newXMax, newYMin, newYMax);
            chart.Invalidate();
        }

        public static void HandleMouseDown(ref Point lastMouseDownPoint, MouseEventArgs e)
        {
            lastMouseDownPoint = e.Location;
        }

        public static ChartDataPoint HandleMouseMove(
            BeepChart chart,
            MouseEventArgs e,
            ref Point lastMouseDownPoint,
            ref DateTime lastInvalidate,
            float panFactor,
            bool allowDragPan = true)
        {
            if (allowDragPan && e.Button == MouseButtons.Left && lastMouseDownPoint != Point.Empty)
            {
                int deltaX = lastMouseDownPoint.X - e.X;
                int deltaY = lastMouseDownPoint.Y - e.Y;
                PanViewport(chart, deltaX, deltaY, panFactor);
                lastMouseDownPoint = e.Location;
                if ((DateTime.Now - lastInvalidate).TotalMilliseconds > 50)
                {
                    chart.Invalidate();
                    lastInvalidate = DateTime.Now;
                }
                return null;
            }
            else
            {
                return GetHoveredDataPoint(chart, e.Location);
            }
        }

        public static void HandleMouseUp(BeepChart chart, ref Point lastMouseDownPoint)
        {
            lastMouseDownPoint = Point.Empty;
            chart.Invalidate();
        }

        private static void PanViewport(BeepChart chart, int deltaX, int deltaY, float panFactor)
        {
            if (chart.ChartDrawingRect.Width <= 0 || chart.ChartDrawingRect.Height <= 0) return;
            float xRange = chart.ViewportXMax - chart.ViewportXMin;
            float yRange = chart.ViewportYMax - chart.ViewportYMin;
            if (xRange <= 0 || yRange <= 0) return;

            float normalizedPanFactor = Math.Clamp(panFactor, 0.10f, 5.0f);
            float panX = (float)deltaX / chart.ChartDrawingRect.Width * xRange * normalizedPanFactor;
            float panY = (float)deltaY / chart.ChartDrawingRect.Height * yRange * normalizedPanFactor;

            ApplyViewport(
                chart,
                chart.ViewportXMin - panX,
                chart.ViewportXMax - panX,
                chart.ViewportYMin + panY,
                chart.ViewportYMax + panY);
        }

        private static void ApplyViewport(BeepChart chart, float xMin, float xMax, float yMin, float yMax)
        {
            EnsureMinRange(ref xMin, ref xMax);
            EnsureMinRange(ref yMin, ref yMax);

            chart.ViewportXMin = xMin;
            chart.ViewportXMax = xMax;
            chart.ViewportYMin = yMin;
            chart.ViewportYMax = yMax;

            BeepChartViewportHelper.EnforceViewportLimits(chart);
        }

        private static void EnsureMinRange(ref float min, ref float max)
        {
            if (max < min)
            {
                (min, max) = (max, min);
            }

            float range = max - min;
            if (range >= MinViewportRange)
            {
                return;
            }

            float mid = (min + max) * 0.5f;
            float half = MinViewportRange * 0.5f;
            min = mid - half;
            max = mid + half;
        }

        public static ChartDataPoint GetHoveredDataPoint(BeepChart chart, Point location)
        {
            if (chart.DataSeries == null || !chart.DataSeries.Any()) return null;

            float xRange = chart.ViewportXMax - chart.ViewportXMin;
            float yRange = chart.ViewportYMax - chart.ViewportYMin;
            if (chart.ChartDrawingRect.Width <= 0 || chart.ChartDrawingRect.Height <= 0 || xRange <= 0 || yRange <= 0)
                return null;

            const float hoverRadius = 10f;
            float hoverRadiusSquared = hoverRadius * hoverRadius;

            foreach (var series in chart.DataSeries)
            {
                if (!series.Visible || series.Points == null) continue;
                foreach (var point in series.Points)
                {
                    float x = chart.ConvertXValue(point) is float xVal ? xVal : 0;
                    float y = chart.ConvertYValue(point) is float yVal ? yVal : 0;
                    PointF screenPos = new PointF(
                        chart.ChartDrawingRect.Left + (x - chart.ViewportXMin) / xRange * chart.ChartDrawingRect.Width,
                        chart.ChartDrawingRect.Bottom - (y - chart.ViewportYMin) / yRange * chart.ChartDrawingRect.Height
                    );

                    float dx = screenPos.X - location.X;
                    float dy = screenPos.Y - location.Y;
                    if ((dx * dx) + (dy * dy) < hoverRadiusSquared)
                        return point;
                }
            }
            return null;
        }

        public static ChartDataPoint FindClosestPointAtX(BeepChart chart, ChartDataSeries series, float dataXCoordinate)
        {
            if (series?.Points == null || !series.Points.Any()) return null;

            ChartDataPoint closest = null;
            float minDistance = float.MaxValue;

            foreach (var point in series.Points)
            {
                float x = chart.ConvertXValue(point) is float xVal ? xVal : 0;
                float distance = Math.Abs(x - dataXCoordinate);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closest = point;
                }
            }

            return closest;
        }
    }
}
