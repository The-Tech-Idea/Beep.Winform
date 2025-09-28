using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Sample
{
    /// <summary>
    /// Sample implementations for BeepChartWidget with all chart styles
    /// </summary>
    public static class BeepChartWidgetSamples
    {
        /// <summary>
        /// Creates a bar chart widget
        /// Uses BarChartPainter.cs
        /// </summary>
        public static BeepChartWidget CreateBarChartWidget()
        {
            return new BeepChartWidget
            {
                Style = ChartWidgetStyle.BarChart,
                Title = "Monthly Sales",
                Values = new List<double> { 15, 25, 18, 30, 22, 35 },
                Labels = new List<string> { "Jan", "Feb", "Mar", "Apr", "May", "Jun" },
                ShowLegend = true,
                ShowGrid = true,
                Size = new Size(350, 250),
                AccentColor = Color.FromArgb(33, 150, 243)
            };
        }

        /// <summary>
        /// Creates a line chart widget
        /// Uses LineChartPainter.cs
        /// </summary>
        public static BeepChartWidget CreateLineChartWidget()
        {
            return new BeepChartWidget
            {
                Style = ChartWidgetStyle.LineChart,
                Title = "User Growth Trend",
                Values = new List<double> { 10, 15, 12, 20, 25, 30, 28 },
                Labels = new List<string> { "Week 1", "Week 2", "Week 3", "Week 4", "Week 5", "Week 6", "Week 7" },
                ShowLegend = false,
                Size = new Size(400, 200),
                AccentColor = Color.FromArgb(76, 175, 80)
            };
        }

        /// <summary>
        /// Creates a pie chart widget
        /// Uses PieChartPainter.cs
        /// </summary>
        public static BeepChartWidget CreatePieChartWidget()
        {
            return new BeepChartWidget
            {
                Style = ChartWidgetStyle.PieChart,
                Title = "Market Share",
                Values = new List<double> { 30, 25, 20, 15, 10 },
                Labels = new List<string> { "Product A", "Product B", "Product C", "Product D", "Others" },
                Colors = new List<Color> 
                { 
                    Color.FromArgb(33, 150, 243), Color.FromArgb(76, 175, 80), 
                    Color.FromArgb(255, 193, 7), Color.FromArgb(244, 67, 54), Color.FromArgb(156, 39, 176) 
                },
                Size = new Size(300, 300),
                AccentColor = Color.FromArgb(33, 150, 243)
            };
        }

        /// <summary>
        /// Creates a gauge chart widget
        /// Uses GaugeChartPainter.cs
        /// </summary>
        public static BeepChartWidget CreateGaugeChartWidget()
        {
            return new BeepChartWidget
            {
                Style = ChartWidgetStyle.GaugeChart,
                Title = "Performance Score",
                Values = new List<double> { 75 },
                MinValue = 0,
                MaxValue = 100,
                Size = new Size(250, 200),
                AccentColor = Color.FromArgb(255, 193, 7)
            };
        }

        /// <summary>
        /// Creates a sparkline widget
        /// Uses SparklinePainter.cs
        /// </summary>
        public static BeepChartWidget CreateSparklineWidget()
        {
            return new BeepChartWidget
            {
                Style = ChartWidgetStyle.Sparkline,
                Values = new List<double> { 10, 15, 12, 18, 16, 22, 20, 25, 23, 28 },
                Size = new Size(200, 50),
                AccentColor = Color.FromArgb(33, 150, 243)
            };
        }

        /// <summary>
        /// Creates a heatmap chart widget
        /// Uses HeatmapPainter.cs
        /// </summary>
        public static BeepChartWidget CreateHeatmapWidget()
        {
            return new BeepChartWidget
            {
                Style = ChartWidgetStyle.HeatmapChart,
                Title = "Activity Heatmap",
                Values = new List<double> { 0.2, 0.5, 0.8, 0.3, 0.9, 0.1, 0.6, 0.7, 0.4, 0.8, 0.9, 0.3, 0.2, 0.6 },
                Size = new Size(300, 200),
                AccentColor = Color.FromArgb(76, 175, 80)
            };
        }

        /// <summary>
        /// Creates a combination chart widget
        /// Uses CombinationChartPainter.cs
        /// </summary>
        public static BeepChartWidget CreateCombinationChartWidget()
        {
            return new BeepChartWidget
            {
                Style = ChartWidgetStyle.CombinationChart,
                Title = "Sales & Growth Combined",
                Values = new List<double> { 15, 25, 18, 30, 22, 35, 10, 15, 12, 20 }, // First 6 for bars, last 4 for line
                Labels = new List<string> { "Q1", "Q2", "Q3", "Q4", "Q5", "Q6" },
                ShowLegend = true,
                Size = new Size(400, 250),
                AccentColor = Color.FromArgb(33, 150, 243)
            };
        }

        /// <summary>
        /// Gets all chart widget samples
        /// </summary>
        public static BeepChartWidget[] GetAllSamples()
        {
            return new BeepChartWidget[]
            {
                CreateBarChartWidget(),
                CreateLineChartWidget(),
                CreatePieChartWidget(),
                CreateGaugeChartWidget(),
                CreateSparklineWidget(),
                CreateHeatmapWidget(),
                CreateCombinationChartWidget()
            };
        }
    }
}