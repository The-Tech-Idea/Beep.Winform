using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Charts.Helpers
{
    internal static class ChartInputHelper
    {
        public static void HandleMouseWheel(BeepChart chart, MouseEventArgs e)
        {
            float zoomFactorChange = e.Delta > 0 ? 0.9f : 1.1f;

            if (chart.ChartDrawingRect.Width <= 0 || chart.ChartDrawingRect.Height <= 0) return;

            float mouseXRatio = (e.X - chart.ChartDrawingRect.Left) / (float)chart.ChartDrawingRect.Width;
            float mouseYRatio = 1.0f - (e.Y - chart.ChartDrawingRect.Top) / (float)chart.ChartDrawingRect.Height;

            float newXMin = chart.ViewportXMin + (chart.ViewportXMax - chart.ViewportXMin) * mouseXRatio * (1 - zoomFactorChange);
            float newXMax = chart.ViewportXMax - (chart.ViewportXMax - chart.ViewportXMin) * (1 - mouseXRatio) * (1 - zoomFactorChange);
            float newYMin = chart.ViewportYMin + (chart.ViewportYMax - chart.ViewportYMin) * mouseYRatio * (1 - zoomFactorChange);
            float newYMax = chart.ViewportYMax - (chart.ViewportYMax - chart.ViewportYMin) * (1 - mouseYRatio) * (1 - zoomFactorChange);

            chart.ViewportXMin = newXMin;
            chart.ViewportXMax = newXMax;
            chart.ViewportYMin = newYMin;
            chart.ViewportYMax = newYMax;
            chart.Invalidate();
        }

        public static void HandleMouseDown(ref Point lastMouseDownPoint, MouseEventArgs e)
        {
            lastMouseDownPoint = e.Location;
        }

        public static ChartDataPoint HandleMouseMove(BeepChart chart, MouseEventArgs e, ref Point lastMouseDownPoint, ref DateTime lastInvalidate, float zoomFactor)
        {
            if (e.Button == MouseButtons.Left && lastMouseDownPoint != Point.Empty)
            {
                int deltaX = lastMouseDownPoint.X - e.X;
                int deltaY = lastMouseDownPoint.Y - e.Y;
                PanViewport(chart, deltaX, deltaY, zoomFactor);
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

        private static void PanViewport(BeepChart chart, int deltaX, int deltaY, float zoomFactor)
        {
            if (chart.ChartDrawingRect.Width <= 0 || chart.ChartDrawingRect.Height <= 0) return;
            float xRange = chart.ViewportXMax - chart.ViewportXMin;
            float yRange = chart.ViewportYMax - chart.ViewportYMin;
            float panX = (float)deltaX / chart.ChartDrawingRect.Width * xRange * zoomFactor;
            float panY = (float)deltaY / chart.ChartDrawingRect.Height * yRange * zoomFactor;

            chart.ViewportXMin -= panX;
            chart.ViewportXMax -= panX;
            chart.ViewportYMin += panY;
            chart.ViewportYMax += panY;
        }

        public static ChartDataPoint GetHoveredDataPoint(BeepChart chart, Point location)
        {
            if (chart.DataSeries == null || !chart.DataSeries.Any()) return null;
            foreach (var series in chart.DataSeries)
            {
                if (series.Points == null) continue;
                foreach (var point in series.Points)
                {
                    float x = chart.ConvertXValue(point) is float xVal ? xVal : 0;
                    float y = chart.ConvertYValue(point) is float yVal ? yVal : 0;
                    PointF screenPos = new PointF(
                        chart.ChartDrawingRect.Left + (x - chart.ViewportXMin) / (chart.ViewportXMax - chart.ViewportXMin) * chart.ChartDrawingRect.Width,
                        chart.ChartDrawingRect.Bottom - (y - chart.ViewportYMin) / (chart.ViewportYMax - chart.ViewportYMin) * chart.ChartDrawingRect.Height
                    );
                    if (Math.Sqrt(Math.Pow(screenPos.X - location.X, 2) + Math.Pow(screenPos.Y - location.Y, 2)) < 10)
                        return point;
                }
            }
            return null;
        }
    }
}
