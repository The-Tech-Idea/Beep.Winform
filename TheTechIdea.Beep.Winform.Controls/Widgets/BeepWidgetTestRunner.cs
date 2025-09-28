using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Widgets;

namespace TheTechIdea.Beep.Winform.Controls.Widgets
{
    /// <summary>
    /// Simple test form to verify that all widget types are working correctly
    /// </summary>
    public static class BeepWidgetTestRunner
    {
        /// <summary>
        /// Creates and shows a simple test form with basic widgets
        /// </summary>
        public static void RunBasicTest()
        {
            var form = new Form
            {
                Text = "Beep Widgets - Basic Test",
                Size = new Size(800, 600),
                StartPosition = FormStartPosition.CenterScreen
            };

            var panel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                Padding = new Padding(20)
            };

            try
            {
                // Test metric widget
                var metricWidget = BeepWidgetSamples.CreateSimpleValueWidget();
                metricWidget.Margin = new Padding(10);
                BeepWidgetSamples.SetupWidgetEvents(metricWidget);
                panel.Controls.Add(metricWidget);

                // Test chart widget  
                var chartWidget = BeepWidgetSamples.CreateBarChartWidget();
                chartWidget.Margin = new Padding(10);
                BeepWidgetSamples.SetupWidgetEvents(chartWidget);
                panel.Controls.Add(chartWidget);

                // Test list widget
                var listWidget = BeepWidgetSamples.CreateActivityFeedWidget();
                listWidget.Margin = new Padding(10);
                BeepWidgetSamples.SetupWidgetEvents(listWidget);
                panel.Controls.Add(listWidget);

                // Test dashboard widget
                var dashboardWidget = BeepWidgetSamples.CreateMultiMetricWidget();
                dashboardWidget.Margin = new Padding(10);
                BeepWidgetSamples.SetupWidgetEvents(dashboardWidget);
                panel.Controls.Add(dashboardWidget);

                var label = new Label
                {
                    Text = "? All widget types loaded successfully!\nClick on different parts of the widgets to test interactivity.",
                    Location = new Point(20, 20),
                    Size = new Size(500, 60),
                    Font = new Font("Segoe UI", 10f),
                    ForeColor = Color.Green,
                    AutoSize = false
                };

                form.Controls.Add(label);
                form.Controls.Add(panel);
                
                form.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating widgets: {ex.Message}", "Widget Test Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Creates and shows the comprehensive demo with all widget styles
        /// </summary>
        public static void RunFullDemo()
        {
            try
            {
                var form = BeepWidgetSamples.CreateAllWidgetsDemo();
                form.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating full demo: {ex.Message}", "Full Demo Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Creates and shows the dashboard demo
        /// </summary>
        public static void RunDashboardDemo()
        {
            try
            {
                var form = BeepWidgetSamples.CreateDashboardDemo();
                form.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating dashboard demo: {ex.Message}", "Dashboard Demo Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}