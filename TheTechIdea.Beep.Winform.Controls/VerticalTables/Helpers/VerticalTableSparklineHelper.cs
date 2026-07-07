using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;

namespace TheTechIdea.Beep.Winform.Controls.VerticalTables.Helpers
{
    /// <summary>
    /// Renders sparkline mini-charts inside vertical table cells.
    /// Supports bar, line, and win/loss sparklines for numeric comparison.
    /// </summary>
    public static class VerticalTableSparklineHelper
    {
        /// <summary>
        /// Sparkline chart type.
        /// </summary>
        public enum SparklineType
        {
            /// <summary>Horizontal bar showing relative value as a proportion of max.</summary>
            Bar,
            /// <summary>Simple line chart connecting data points.</summary>
            Line,
            /// <summary>Win/loss binary indicators (positive=up, negative=down).</summary>
            WinLoss
        }

        /// <summary>
        /// Draw a bar sparkline showing value relative to a max reference value.
        /// </summary>
        public static void DrawBarSparkline(Graphics g, Rectangle cellBounds, double value, double maxValue,
            Color barColor, Color trackColor)
        {
            if (maxValue <= 0) maxValue = 1;
            double ratio = Math.Min(1.0, Math.Max(0.0, value / maxValue));

            int padding = 4;
            int barHeight = Math.Max(4, cellBounds.Height - padding * 2);
            int barWidth = (int)((cellBounds.Width - padding * 2) * ratio);
            barWidth = Math.Max(2, barWidth);

            int y = cellBounds.Top + (cellBounds.Height - barHeight) / 2;
            int x = cellBounds.Left + padding;

            // Track background
            using (var trackBrush = new SolidBrush(trackColor))
                g.FillRectangle(trackBrush, x, y, cellBounds.Width - padding * 2, barHeight);

            // Bar fill
            if (barWidth > 0)
            {
                using (var brush = new LinearGradientBrush(
                    new Rectangle(x, y, barWidth, barHeight), barColor,
                    Color.FromArgb(160, barColor), 0f))
                {
                    g.FillRectangle(brush, x, y, barWidth, barHeight);
                }
            }

            // Value text
            string text = value >= 100 ? $"{value:F0}" : $"{value:F1}";
            using (var font = new Font("Segoe UI", 8, FontStyle.Regular))
            using (var brush = new SolidBrush(Color.FromArgb(80, 80, 90)))
            {
                var sf = new StringFormat { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Center };
                var textRect = new Rectangle(cellBounds.Left + padding, cellBounds.Top,
                    cellBounds.Width - padding * 2, cellBounds.Height);
                g.DrawString(text, font, brush, textRect, sf);
            }
        }

        /// <summary>
        /// Draw a line sparkline connecting a series of data points.
        /// </summary>
        public static void DrawLineSparkline(Graphics g, Rectangle cellBounds, double[] values,
            Color lineColor, float lineWidth = 1.5f)
        {
            if (values == null || values.Length < 2) return;

            int padding = 4;
            double min = values.Min();
            double max = values.Max();
            double range = max - min;
            if (range <= 0) range = 1;

            int chartWidth = cellBounds.Width - padding * 2;
            int chartHeight = cellBounds.Height - padding * 2;
            int chartLeft = cellBounds.Left + padding;
            int chartTop = cellBounds.Top + padding;

            var points = new PointF[values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                float x = chartLeft + (float)i / (values.Length - 1) * chartWidth;
                float y = chartTop + chartHeight - (float)((values[i] - min) / range) * chartHeight;
                points[i] = new PointF(x, y);
            }

            g.SmoothingMode = SmoothingMode.AntiAlias;
            using (var pen = new Pen(lineColor, lineWidth))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;
                g.DrawLines(pen, points);
            }

            // Dot at last point
            var lastPt = points[values.Length - 1];
            using (var brush = new SolidBrush(lineColor))
                g.FillEllipse(brush, lastPt.X - 2, lastPt.Y - 2, 5, 5);
        }

        /// <summary>
        /// Draw a win/loss sparkline showing up/down indicators.
        /// </summary>
        public static void DrawWinLossSparkline(Graphics g, Rectangle cellBounds, double[] values,
            Color upColor, Color downColor)
        {
            if (values == null || values.Length == 0) return;

            int padding = 4;
            int barCount = values.Length;
            int barWidth = Math.Max(2, (cellBounds.Width - padding * 2) / barCount - 2);
            int midY = cellBounds.Top + cellBounds.Height / 2;
            int maxBarHeight = cellBounds.Height - padding * 2;

            for (int i = 0; i < barCount; i++)
            {
                int x = cellBounds.Left + padding + i * (barWidth + 2);
                int height = Math.Min(maxBarHeight / 2, Math.Max(2, (int)(Math.Abs(values[i]) / 2 * maxBarHeight / 2)));
                int y = values[i] >= 0 ? midY - height : midY + 1;

                Color color = values[i] >= 0 ? upColor : downColor;
                using (var brush = new SolidBrush(color))
                    g.FillRectangle(brush, x, y, barWidth, height);
            }
        }

        /// <summary>
        /// Auto-detects and draws appropriate sparkline based on value type.
        /// Single number → bar. Array of numbers → line. Array with signs → win/loss.
        /// </summary>
        public static void DrawAutoSparkline(Graphics g, Rectangle cellBounds, object? value,
            Color accentColor, Color trackColor)
        {
            if (value == null) return;

            if (value is double[] doubleArray)
            {
                DrawLineSparkline(g, cellBounds, doubleArray, accentColor);
                return;
            }

            if (value is int[] intArray)
            {
                bool hasNegatives = intArray.Any(v => v < 0);
                if (hasNegatives)
                    DrawWinLossSparkline(g, cellBounds, intArray.Select(v => (double)v).ToArray(),
                        Color.FromArgb(16, 185, 129), Color.FromArgb(239, 68, 68));
                else
                    DrawLineSparkline(g, cellBounds, intArray.Select(v => (double)v).ToArray(), accentColor);
                return;
            }

            if (value is double dVal)
            {
                DrawBarSparkline(g, cellBounds, dVal, 100.0, accentColor, trackColor);
                return;
            }

            if (value is int iVal)
            {
                DrawBarSparkline(g, cellBounds, iVal, 100.0, accentColor, trackColor);
                return;
            }

            if (value is float fVal)
            {
                DrawBarSparkline(g, cellBounds, fVal, 100.0, accentColor, trackColor);
            }
        }
    }
}
