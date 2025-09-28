using System.ComponentModel;

namespace TheTechIdea.Beep.Winform.Controls.Charts.Helpers
{
    internal static class BeepChartViewportHelper
    {
        public static void AutoScaleViewport(BeepChart chart)
        {
            try
            {
                if (!chart.DataSeries.Any() || !chart.DataSeries.Any(s => s.Points != null && s.Points.Any())) 
                    return;

                float xMin = float.MaxValue, xMax = float.MinValue;
                float yMin = float.MaxValue, yMax = float.MinValue;

                Dictionary<string, int> xCategories = new Dictionary<string, int>();
                Dictionary<string, int> yCategories = new Dictionary<string, int>();
                int xCategoryIndex = 0, yCategoryIndex = 0;

                foreach (var series in chart.DataSeries)
                {
                    foreach (var point in series.Points)
                    {
                        var xValue = BeepChartDataHelper.ConvertXValue(chart, point);
                        if (xValue is float xVal)
                        {
                            xMin = Math.Min(xMin, xVal);
                            xMax = Math.Max(xMax, xVal);
                        }
                        else if (xValue is string xStr)
                        {
                            if (!xCategories.ContainsKey(xStr))
                                xCategories[xStr] = xCategoryIndex++;
                            xMin = Math.Min(xMin, xCategories[xStr]);
                            xMax = Math.Max(xMax, xCategories[xStr]);
                        }

                        var yValue = BeepChartDataHelper.ConvertYValue(chart, point);
                        if (yValue is float yVal)
                        {
                            yMin = Math.Min(yMin, yVal);
                            yMax = Math.Max(yMax, yVal);
                        }
                        else if (yValue is string yStr)
                        {
                            if (!yCategories.ContainsKey(yStr))
                                yCategories[yStr] = yCategoryIndex++;
                            yMin = Math.Min(yMin, yCategories[yStr]);
                            yMax = Math.Max(yMax, yCategories[yStr]);
                        }
                    }
                }

                if (xMin == float.MaxValue || xMax == float.MinValue || yMin == float.MaxValue || yMax == float.MinValue)
                    return;

                chart.ViewportXMin = xMin;
                chart.ViewportXMax = xMax;
                chart.ViewportYMin = yMin;
                chart.ViewportYMax = yMax;

                float xPadding = (chart.ViewportXMax - chart.ViewportXMin) * 0.1f;
                float yPadding = (chart.ViewportYMax - chart.ViewportYMin) * 0.1f;

                chart.ViewportXMin -= xPadding;
                chart.ViewportXMax += xPadding;
                chart.ViewportYMin -= yPadding;
                chart.ViewportYMax += yPadding;

                EnforceViewportLimits(chart);
                chart.Invalidate();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine($"AutoScaleViewport Error: {ex.Message}");
            }
        }

        public static void EnforceViewportLimits(BeepChart chart)
        {
            if (chart.ViewportXMin > chart.ViewportXMax)
            {
                float mid = (chart.ViewportXMin + chart.ViewportXMax) / 2;
                chart.ViewportXMin = mid - 1;
                chart.ViewportXMax = mid + 1;
            }
            if (chart.ViewportYMin > chart.ViewportYMax)
            {
                float mid = (chart.ViewportYMin + chart.ViewportYMax) / 2;
                chart.ViewportYMin = mid - 1;
                chart.ViewportYMax = mid + 1;
            }
            chart.ViewportXMin = Math.Max(chart.ViewportXMin, float.MinValue);
            chart.ViewportXMax = Math.Min(chart.ViewportXMax, float.MaxValue);
            chart.ViewportYMin = Math.Max(chart.ViewportYMin, float.MinValue);
            chart.ViewportYMax = Math.Min(chart.ViewportYMax, float.MaxValue);
        }

        public static void UpdateChartDrawingRectBase(BeepChart chart)
        {
            try
            {
                int leftPadding = 40;
                int rightPadding = 30;
                int topPadding = 40;
                int bottomPadding = 40;

                var newRect = new Rectangle(
                    chart.DrawingRect.Left + leftPadding,
                    chart.DrawingRect.Top + topPadding,
                    chart.DrawingRect.Width - leftPadding - rightPadding,
                    chart.DrawingRect.Height - topPadding - bottomPadding
                );

                // Use reflection to set the private field since ChartDrawingRect has a private setter
                var fieldInfo = typeof(BeepChart).GetField("_chartDrawingRect", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                fieldInfo?.SetValue(chart, newRect);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine($"UpdateChartDrawingRectBase Error: {ex.Message}");
                
                // Fallback - set to client rectangle
                var fieldInfo = typeof(BeepChart).GetField("_chartDrawingRect", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                fieldInfo?.SetValue(chart, chart.ClientRectangle);
            }
        }
    }
}