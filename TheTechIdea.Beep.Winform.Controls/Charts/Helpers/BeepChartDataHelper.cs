using System;
using System.Collections.Generic;
using System.Linq;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Charts.Helpers
{
    internal static class BeepChartDataHelper
    {
        public static object ConvertXValue(BeepChart chart, ChartDataPoint point)
        {
            // Properties can't be passed by ref; use a local copy and write back after conversion
            DateTime dateMin = chart.XAxisDateMin;
            float result = ConvertValue(point.X, chart.BottomAxisType, chart.XAxisCategories, ref dateMin);
            if (dateMin != chart.XAxisDateMin)
            {
                chart.XAxisDateMin = dateMin;
            }
            return result;
        }

        public static object ConvertYValue(BeepChart chart, ChartDataPoint point)
        {
            // Properties can't be passed by ref; use a local copy and write back after conversion
            DateTime dateMin = chart.YAxisDateMin;
            float result = ConvertValue(point.Y, chart.LeftAxisType, chart.YAxisCategories, ref dateMin);
            if (dateMin != chart.YAxisDateMin)
            {
                chart.YAxisDateMin = dateMin;
            }
            return result;
        }

        private static float ConvertValue(string value, AxisType axisType, Dictionary<string, int> categoryMap, ref DateTime dateMin)
        {
            try
            {
                string parsedValue = value ?? string.Empty;
                switch (axisType)
                {
                    case AxisType.Numeric:
                        return float.TryParse(parsedValue, out float numericValue) ? numericValue : 0;
                    case AxisType.Date:
                        if (DateTime.TryParse(parsedValue, out DateTime dateValue))
                        {
                            if (dateMin == DateTime.MinValue) dateMin = dateValue;
                            return (float)(dateValue - dateMin).TotalDays;
                        }
                        return 0;
                    case AxisType.Text:
                        if (!categoryMap.ContainsKey(parsedValue))
                            categoryMap[parsedValue] = categoryMap.Count;
                        return categoryMap[parsedValue];
                    default:
                        return 0;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine($"Error converting value: {ex.Message}");
                return 0;
            }
        }

        public static void DetectAxisTypes(BeepChart chart)
        {
            var firstSeries = chart.DataSeries.FirstOrDefault(s => s.Points != null && s.Points.Any());
            if (firstSeries != null)
            {
                string exampleX = firstSeries.Points[0].X;
                string exampleY = firstSeries.Points[0].Y;

                if (float.TryParse(exampleX, out _))
                    chart.BottomAxisType = AxisType.Numeric;
                else if (DateTime.TryParse(exampleX, out _))
                    chart.BottomAxisType = AxisType.Date;
                else
                    chart.BottomAxisType = AxisType.Text;

                if (float.TryParse(exampleY, out _))
                    chart.LeftAxisType = AxisType.Numeric;
                else if (DateTime.TryParse(exampleY, out _))
                    chart.LeftAxisType = AxisType.Date;
                else
                    chart.LeftAxisType = AxisType.Text;
            }
        }
    }
}