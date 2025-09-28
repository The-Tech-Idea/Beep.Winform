using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Sample
{
    /// <summary>
    /// Sample implementations for BeepMetricWidget with all metric styles
    /// </summary>
    public static class BeepMetricWidgetSamples
    {
        /// <summary>
        /// Creates a simple value metric widget
        /// Uses SimpleValuePainter.cs
        /// </summary>
        public static BeepMetricWidget CreateSimpleValueWidget()
        {
            return new BeepMetricWidget
            {
                Style = MetricWidgetStyle.SimpleValue,
                Title = "Total Sales",
                Value = "$127,340",
                Units = "",
                Size = new Size(200, 120),
                AccentColor = Color.FromArgb(33, 150, 243)
            };
        }

        /// <summary>
        /// Creates a metric widget with trend indicator
        /// Uses TrendMetricPainter.cs
        /// </summary>
        public static BeepMetricWidget CreateTrendWidget()
        {
            return new BeepMetricWidget
            {
                Style = MetricWidgetStyle.ValueWithTrend,
                Title = "Active Users",
                Value = "23,456",
                Units = "users",
                ShowTrend = true,
                TrendValue = "+15.3%",
                TrendDirection = "up",
                TrendPercentage = 15.3,
                Size = new Size(250, 120),
                AccentColor = Color.FromArgb(76, 175, 80)
            };
        }

        /// <summary>
        /// Creates a progress metric widget
        /// Uses ProgressMetricPainter.cs
        /// </summary>
        public static BeepMetricWidget CreateProgressWidget()
        {
            return new BeepMetricWidget
            {
                Style = MetricWidgetStyle.ProgressMetric,
                Title = "Goal Progress",
                Value = "72%",
                TrendPercentage = 72.0,
                Size = new Size(200, 120),
                AccentColor = Color.FromArgb(255, 193, 7)
            };
        }

        /// <summary>
        /// Creates a gauge metric widget  
        /// Uses GaugeMetricPainter.cs
        /// </summary>
        public static BeepMetricWidget CreateGaugeWidget()
        {
            return new BeepMetricWidget
            {
                Style = MetricWidgetStyle.GaugeMetric,
                Title = "Server Load",
                Value = "68%",
                TrendPercentage = 68.0,
                Size = new Size(180, 180),
                AccentColor = Color.FromArgb(156, 39, 176)
            };
        }

        /// <summary>
        /// Creates a comparison metric widget
        /// Uses ComparisonMetricPainter.cs
        /// </summary>
        public static BeepMetricWidget CreateComparisonWidget()
        {
            return new BeepMetricWidget
            {
                Style = MetricWidgetStyle.ComparisonMetric,
                Title = "This Month vs Last",
                Value = "$45,230",
                TrendValue = "$38,120",
                Units = "",
                Size = new Size(280, 120),
                AccentColor = Color.FromArgb(33, 150, 243),
                TrendColor = Color.FromArgb(255, 152, 0)
            };
        }

        /// <summary>
        /// Creates a card-style metric widget
        /// Uses CardMetricPainter.cs
        /// </summary>
        public static BeepMetricWidget CreateCardWidget()
        {
            return new BeepMetricWidget
            {
                Style = MetricWidgetStyle.CardMetric,
                Title = "Revenue",
                Value = "$847,391",
                ShowTrend = true,
                TrendValue = "+8.2%",
                TrendDirection = "up",
                ShowIcon = true,
                IconPath = "revenue-icon.svg", // Set to actual icon path
                Size = new Size(300, 150),
                AccentColor = Color.FromArgb(76, 175, 80)
            };
        }

        /// <summary>
        /// Gets all metric widget samples
        /// </summary>
        public static BeepMetricWidget[] GetAllSamples()
        {
            return new BeepMetricWidget[]
            {
                CreateSimpleValueWidget(),
                CreateTrendWidget(),
                CreateProgressWidget(),
                CreateGaugeWidget(),
                CreateComparisonWidget(),
                CreateCardWidget()
            };
        }
    }
}