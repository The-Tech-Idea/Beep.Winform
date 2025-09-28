using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Sample
{
    /// <summary>
    /// Sample implementations for BeepDashboardWidget with all dashboard styles
    /// </summary>
    public static class BeepDashboardWidgetSamples
    {
        /// <summary>
        /// Creates a multi-metric dashboard widget
        /// Uses MultiMetricPainter.cs
        /// </summary>
        public static BeepDashboardWidget CreateMultiMetricWidget()
        {
            var widget = new BeepDashboardWidget
            {
                Style = DashboardWidgetStyle.MultiMetric,
                Title = "Key Metrics Overview",
                ShowTitle = true,
                Columns = 2,
                Rows = 2,
                Size = new Size(400, 300),
                AccentColor = Color.FromArgb(33, 150, 243)
            };

            widget.Metrics = new List<Dictionary<string, object>>
            {
                new() { ["Title"] = "Revenue", ["Value"] = "$127K", ["Trend"] = "+12%", ["Color"] = Color.Green },
                new() { ["Title"] = "Users", ["Value"] = "23,456", ["Trend"] = "+8%", ["Color"] = Color.Blue },
                new() { ["Title"] = "Orders", ["Value"] = "1,234", ["Trend"] = "-2%", ["Color"] = Color.Red },
                new() { ["Title"] = "Growth", ["Value"] = "15.7%", ["Trend"] = "+5%", ["Color"] = Color.Orange }
            };

            return widget;
        }

        /// <summary>
        /// Creates a chart grid dashboard widget
        /// Uses ChartGridPainter.cs
        /// </summary>
        public static BeepDashboardWidget CreateChartGridWidget()
        {
            var widget = new BeepDashboardWidget
            {
                Style = DashboardWidgetStyle.ChartGrid,
                Title = "Analytics Dashboard",
                ShowTitle = true,
                Columns = 2,
                Rows = 2,
                Size = new Size(450, 350),
                AccentColor = Color.FromArgb(76, 175, 80)
            };

            widget.Metrics = new List<Dictionary<string, object>>
            {
                new() { ["Title"] = "Sales Trend", ["Color"] = Color.Blue },
                new() { ["Title"] = "User Activity", ["Color"] = Color.Green },
                new() { ["Title"] = "Performance", ["Color"] = Color.Orange },
                new() { ["Title"] = "Conversion", ["Color"] = Color.Purple }
            };

            return widget;
        }

        /// <summary>
        /// Creates a timeline view dashboard widget
        /// Uses TimelineViewPainter.cs
        /// </summary>
        public static BeepDashboardWidget CreateTimelineWidget()
        {
            var widget = new BeepDashboardWidget
            {
                Style = DashboardWidgetStyle.TimelineView,
                Title = "Project Timeline",
                ShowTitle = true,
                Size = new Size(400, 300),
                AccentColor = Color.FromArgb(156, 39, 176)
            };

            widget.Metrics = new List<Dictionary<string, object>>
            {
                new() { ["Title"] = "Project Kickoff", ["Value"] = "March 1, 2024" },
                new() { ["Title"] = "Alpha Release", ["Value"] = "March 15, 2024" },
                new() { ["Title"] = "Beta Testing", ["Value"] = "April 1, 2024" },
                new() { ["Title"] = "Final Release", ["Value"] = "April 15, 2024" }
            };

            return widget;
        }

        /// <summary>
        /// Creates a status overview dashboard widget
        /// Uses StatusOverviewPainter.cs
        /// </summary>
        public static BeepDashboardWidget CreateStatusOverviewWidget()
        {
            var widget = new BeepDashboardWidget
            {
                Style = DashboardWidgetStyle.StatusOverview,
                Title = "System Status",
                ShowTitle = true,
                Size = new Size(350, 280),
                AccentColor = Color.FromArgb(76, 175, 80)
            };

            widget.Metrics = new List<Dictionary<string, object>>
            {
                new() { ["Title"] = "Running", ["Value"] = "Web Server" },
                new() { ["Title"] = "Running", ["Value"] = "Database" },
                new() { ["Title"] = "Warning", ["Value"] = "Cache Service" },
                new() { ["Title"] = "Error", ["Value"] = "Mail Service" },
                new() { ["Title"] = "Running", ["Value"] = "API Gateway" }
            };

            return widget;
        }

        /// <summary>
        /// Creates a comparison grid dashboard widget
        /// Uses ComparisonGridPainter.cs
        /// </summary>
        public static BeepDashboardWidget CreateComparisonGridWidget()
        {
            var widget = new BeepDashboardWidget
            {
                Style = DashboardWidgetStyle.ComparisonGrid,
                Title = "Performance Comparison",
                ShowTitle = true,
                Size = new Size(400, 250),
                AccentColor = Color.FromArgb(255, 152, 0)
            };

            widget.Metrics = new List<Dictionary<string, object>>
            {
                new() { ["Title"] = "Current Month", ["Value"] = "$45,230", ["Trend"] = "+12%" },
                new() { ["Title"] = "Previous Month", ["Value"] = "$38,120", ["Trend"] = "+5%" }
            };

            return widget;
        }

        /// <summary>
        /// Creates an analytics panel dashboard widget
        /// Uses AnalyticsPanelPainter.cs
        /// </summary>
        public static BeepDashboardWidget CreateAnalyticsPanelWidget()
        {
            var widget = new BeepDashboardWidget
            {
                Style = DashboardWidgetStyle.AnalyticsPanel,
                Title = "Analytics Overview",
                ShowTitle = true,
                Size = new Size(500, 400),
                AccentColor = Color.FromArgb(33, 150, 243)
            };

            widget.Metrics = new List<Dictionary<string, object>>
            {
                new() { ["Title"] = "Sessions", ["Value"] = "12,847", ["Color"] = Color.Blue },
                new() { ["Title"] = "Users", ["Value"] = "8,392", ["Color"] = Color.Green },
                new() { ["Title"] = "Bounce Rate", ["Value"] = "34.2%", ["Color"] = Color.Orange },
                new() { ["Title"] = "Conversion", ["Value"] = "2.8%", ["Color"] = Color.Purple }
            };

            return widget;
        }

        /// <summary>
        /// Gets all dashboard widget samples
        /// </summary>
        public static BeepDashboardWidget[] GetAllSamples()
        {
            return new BeepDashboardWidget[]
            {
                CreateMultiMetricWidget(),
                CreateChartGridWidget(),
                CreateTimelineWidget(),
                CreateStatusOverviewWidget(),
                CreateComparisonGridWidget(),
                CreateAnalyticsPanelWidget()
            };
        }
    }
}