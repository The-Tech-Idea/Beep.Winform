using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(BeepBubbleChart))]
    [DefaultProperty("ChartDataSeries")]
    public class BeepBubbleChart : BeepChartBase
    {
        // Bubble chart-specific properties can be added here if needed
        public BeepBubbleChart()
        {
            if (Width <= 0 || Height <= 0) // Ensure size is only set if not already defined
            {
                Width = 200;
                Height = 200;
            }
            ApplyThemeToChilds = false;
            // Add sample data when the control is created

        }

      


        protected override void DrawDataSeries(Graphics g, Rectangle chartArea)
        {
            foreach (var series in DataSeries)
            {
                foreach (var point in series)
                {
                    // Convert X and Y values based on axis types
                    float x = Convert.ToSingle(ConvertXValue(point));
                    float y = Convert.ToSingle(ConvertYValue(point));
                    float size = point.Size;

                    // Calculate position in chart area, accounting for viewport scaling
                    float scaledX = chartArea.Left + (x - ViewportXMin) * chartArea.Width / (ViewportXMax - ViewportXMin);
                    float scaledY = chartArea.Bottom - (y - ViewportYMin) * chartArea.Height / (ViewportYMax - ViewportYMin);

                    // Adjust bubble size for scaling, keeping it within the chart's visual range
                    float bubbleSize = Math.Min(size, 20); // For example, max size can be limited to 20

                    // Draw the bubble
                    using (Brush brush = new SolidBrush(point.Color))
                    {
                        g.FillEllipse(brush, scaledX - bubbleSize / 2, scaledY - bubbleSize / 2, bubbleSize, bubbleSize);
                    }

                    // Draw optional labels, if specified
                    if (!string.IsNullOrEmpty(point.Label))
                    {
                        using (Font labelFont = new Font("Arial", 8))
                        using (Brush textBrush = new SolidBrush(ForeColor))
                        {
                            g.DrawString(point.Label, labelFont, textBrush, scaledX, scaledY);
                        }
                    }
                }
            }
        }
    }
}
