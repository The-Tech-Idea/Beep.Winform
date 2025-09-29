using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Linq;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers
{
    /// <summary>
    /// Static helper class providing common rendering utilities for all widget painters
    /// </summary>
    internal static class WidgetRenderingHelpers
    {
        /// <summary>
        /// Creates a rounded rectangle graphics path
        /// </summary>
        public static GraphicsPath CreateRoundedPath(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            if (radius <= 0)
            {
                path.AddRectangle(rect);
                return path;
            }
            
            int d = radius * 2;
            path.AddArc(rect.Left, rect.Top, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Top, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.Left, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        /// <summary>
        /// Draws a multi-layer soft shadow effect
        /// </summary>
        public static void DrawSoftShadow(Graphics g, Rectangle rect, int radius, int layers = 4, int offset = 2)
        {
            for (int i = layers; i > 0; i--)
            {
                int spread = i;
                int alpha = (int)(10 * (i / (float)layers));
                using var shadowBrush = new SolidBrush(Color.FromArgb(alpha, Color.Black));
                var shadowRect = new Rectangle(
                    rect.X + offset - spread / 2,
                    rect.Y + offset - spread / 2,
                    rect.Width + spread,
                    rect.Height + spread
                );
                using var shadowPath = CreateRoundedPath(shadowRect, radius + i);
                g.FillPath(shadowBrush, shadowPath);
            }
        }

        /// <summary>
        /// Draws trend arrows (up, down, neutral)
        /// </summary>
        public static void DrawTrendArrow(Graphics g, Rectangle rect, string direction, Color color)
        {
            if (rect.IsEmpty) return;
            
            using var brush = new SolidBrush(color);
            var center = new PointF(rect.X + rect.Width / 2f, rect.Y + rect.Height / 2f);
            var size = Math.Min(rect.Width, rect.Height) / 2;
            
            PointF[] points;
            
            switch (direction.ToLower())
            {
                case "up":
                    points = new PointF[]
                    {
                        new PointF(center.X, center.Y - size),
                        new PointF(center.X - size, center.Y + size),
                        new PointF(center.X + size, center.Y + size)
                    };
                    break;
                case "down":
                    points = new PointF[]
                    {
                        new PointF(center.X, center.Y + size),
                        new PointF(center.X - size, center.Y - size),
                        new PointF(center.X + size, center.Y - size)
                    };
                    break;
                default: // neutral
                    var neutralRect = new RectangleF(center.X - size, center.Y - size/4, size * 2, size/2);
                    g.FillRectangle(brush, neutralRect);
                    return;
            }
            
            g.FillPolygon(brush, points);
        }

        /// <summary>
        /// Draws a progress bar
        /// </summary>
        public static void DrawProgressBar(Graphics g, Rectangle rect, double percentage, Color fillColor, Color backgroundColor)
        {
            if (rect.IsEmpty) return;
            
            percentage = Math.Max(0, Math.Min(100, percentage));
            
            // Draw background
            using var bgBrush = new SolidBrush(backgroundColor);
            using var bgPath = CreateRoundedPath(rect, rect.Height / 2);
            g.FillPath(bgBrush, bgPath);
            
            // Draw fill
            if (percentage > 0)
            {
                int fillWidth = (int)(rect.Width * percentage / 100);
                var fillRect = new Rectangle(rect.X, rect.Y, fillWidth, rect.Height);
                using var fillBrush = new SolidBrush(fillColor);
                using var fillPath = CreateRoundedPath(fillRect, rect.Height / 2);
                g.FillPath(fillBrush, fillPath);
            }
        }

        /// <summary>
        /// Draws a value with optional units
        /// </summary>
        public static void DrawValue(Graphics g, Rectangle rect, string value, string units, Font font, Color color, StringAlignment alignment = StringAlignment.Center)
        {
            if (string.IsNullOrEmpty(value) || rect.IsEmpty) return;
            
            using var brush = new SolidBrush(color);
            var format = new StringFormat 
            { 
                Alignment = alignment, 
                LineAlignment = StringAlignment.Center,
                Trimming = StringTrimming.EllipsisCharacter
            };
            
            string displayText = string.IsNullOrEmpty(units) ? value : $"{value} {units}";
            g.DrawString(displayText, font, brush, rect, format);
        }

        /// <summary>
        /// Draws a simple bar chart
        /// </summary>
        public static void DrawBarChart(Graphics g, Rectangle rect, IEnumerable<double> values, Color barColor, Color backgroundColor)
        {
            if (rect.IsEmpty || values == null) return;
            
            var valueList = values.ToList();
            if (!valueList.Any()) return;
            
            // Draw background
            using var bgBrush = new SolidBrush(backgroundColor);
            g.FillRectangle(bgBrush, rect);
            
            // Calculate bar dimensions
            var maxValue = valueList.Max();
            if (maxValue <= 0) return;
            
            int barCount = valueList.Count;
            int barWidth = (rect.Width - (barCount + 1) * 2) / barCount; // 2px spacing
            if (barWidth <= 0) return;
            
            using var barBrush = new SolidBrush(barColor);
            
            for (int i = 0; i < valueList.Count; i++)
            {
                var value = valueList[i];
                int barHeight = (int)(rect.Height * value / maxValue);
                int x = rect.X + 2 + i * (barWidth + 2);
                int y = rect.Bottom - barHeight;
                
                var barRect = new Rectangle(x, y, barWidth, barHeight);
                g.FillRectangle(barBrush, barRect);
            }
        }

        /// <summary>
        /// Draws a simple line chart
        /// </summary>
        public static void DrawLineChart(Graphics g, Rectangle rect, IEnumerable<double> values, Color lineColor, float lineWidth = 2f)
        {
            if (rect.IsEmpty || values == null) return;
            
            var valueList = values.ToList();
            if (valueList.Count < 2) return;
            
            var minValue = valueList.Min();
            var maxValue = valueList.Max();
            var valueRange = maxValue - minValue;
            if (valueRange <= 0) return;
            
            // Calculate points
            var points = new PointF[valueList.Count];
            for (int i = 0; i < valueList.Count; i++)
            {
                float x = rect.X + (float)i / (valueList.Count - 1) * rect.Width;
                float y = (float)(rect.Bottom - (float)(valueList[i] - minValue) / valueRange * rect.Height);
                points[i] = new PointF(x, y);
            }
            
            // Draw line
            using var pen = new Pen(lineColor, lineWidth);
            g.DrawLines(pen, points);
        }

        /// <summary>
        /// Draws a pie chart segment
        /// </summary>
        public static void DrawPieSlice(Graphics g, Rectangle rect, float startAngle, float sweepAngle, Color color)
        {
            if (rect.IsEmpty || sweepAngle <= 0) return;
            
            using var brush = new SolidBrush(color);
            g.FillPie(brush, rect, startAngle, sweepAngle);
        }

        /// <summary>
        /// Draws a gauge/speedometer
        /// </summary>
        public static void DrawGauge(Graphics g, Rectangle rect, double value, double minValue, double maxValue, Color fillColor, Color backgroundColor, int thickness = 10)
        {
            if (rect.IsEmpty || maxValue <= minValue) return;
            
            value = Math.Max(minValue, Math.Min(maxValue, value));
            var percentage = (value - minValue) / (maxValue - minValue);
            
            // Draw background arc
            using var bgPen = new Pen(backgroundColor, thickness);
            g.DrawArc(bgPen, rect, 180, 180);
            
            // Draw fill arc
            if (percentage > 0)
            {
                using var fillPen = new Pen(fillColor, thickness);
                float sweepAngle = (float)(180 * percentage);
                g.DrawArc(fillPen, rect, 180, sweepAngle);
            }
        }

        /// <summary>
        /// Draws a sparkline (mini line chart)
        /// </summary>
        public static void DrawSparkline(Graphics g, Rectangle rect, IEnumerable<double> values, Color lineColor)
        {
            DrawLineChart(g, rect, values, lineColor, 1.5f);
        }

        /// <summary>
        /// Draws a heatmap cell
        /// </summary>
        public static void DrawHeatmapCell(Graphics g, Rectangle rect, double intensity, Color baseColor, int cornerRadius = 2)
        {
            if (rect.IsEmpty) return;
            
            intensity = Math.Max(0, Math.Min(1, intensity));
            int alpha = (int)(255 * intensity);
            using var brush = new SolidBrush(Color.FromArgb(alpha, baseColor));
            using var path = CreateRoundedPath(rect, cornerRadius);
            g.FillPath(brush, path);
        }

        /// <summary>
        /// Draws a notification icon based on type
        /// </summary>
        public static void DrawNotificationIcon(Graphics g, Rectangle rect, NotificationType type, Color color)
        {
            using var brush = new SolidBrush(color);
            using var pen = new Pen(color, 2);
            
            switch (type)
            {
                case NotificationType.Success:
                    // Draw checkmark
                    var checkPoints = new Point[]
                    {
                        new Point(rect.X + 4, rect.Y + rect.Height / 2),
                        new Point(rect.X + rect.Width / 2, rect.Y + rect.Height - 4),
                        new Point(rect.Right - 4, rect.Y + 4)
                    };
                    g.DrawLines(pen, checkPoints);
                    break;
                    
                case NotificationType.Warning:
                    // Draw exclamation mark
                    g.DrawLine(pen, rect.X + rect.Width / 2, rect.Y + 4, rect.X + rect.Width / 2, rect.Y + rect.Height - 8);
                    g.FillEllipse(brush, rect.X + rect.Width / 2 - 1, rect.Bottom - 4, 2, 2);
                    break;
                    
                case NotificationType.Error:
                    // Draw X
                    g.DrawLine(pen, rect.X + 4, rect.Y + 4, rect.Right - 4, rect.Bottom - 4);
                    g.DrawLine(pen, rect.Right - 4, rect.Y + 4, rect.X + 4, rect.Bottom - 4);
                    break;
                    
                case NotificationType.Info:
                    // Draw i
                    g.FillEllipse(brush, rect.X + rect.Width / 2 - 1, rect.Y + 4, 2, 2);
                    g.DrawLine(pen, rect.X + rect.Width / 2, rect.Y + 8, rect.X + rect.Width / 2, rect.Bottom - 4);
                    break;
                    
                case NotificationType.Progress:
                    // Draw circular arrow
                    var progressRect = Rectangle.Inflate(rect, -2, -2);
                    g.DrawArc(pen, progressRect, 0, 270);
                    break;
            }
        }

        /// <summary>
        /// Draws a dropdown arrow
        /// </summary>
        public static void DrawDropdownArrow(Graphics g, Rectangle rect, Color color)
        {
            using var brush = new SolidBrush(color);
            var points = new Point[]
            {
                new Point(rect.X + 4, rect.Y + 6),
                new Point(rect.X + rect.Width / 2, rect.Y + rect.Height - 6),
                new Point(rect.Right - 4, rect.Y + 6)
            };
            g.FillPolygon(brush, points);
        }

        /// <summary>
        /// Draws a calendar icon
        /// </summary>
        public static void DrawCalendarIcon(Graphics g, Rectangle rect, Color color)
        {
            using var pen = new Pen(color, 1);
            using var brush = new SolidBrush(color);
            
            // Calendar body
            var calendarRect = Rectangle.Inflate(rect, -2, -2);
            g.DrawRectangle(pen, calendarRect);
            
            // Calendar header
            var headerRect = new Rectangle(calendarRect.X, calendarRect.Y, calendarRect.Width, 4);
            g.FillRectangle(brush, headerRect);
            
            // Calendar rings
            g.DrawLine(pen, calendarRect.X + 3, calendarRect.Y - 2, calendarRect.X + 3, calendarRect.Y + 2);
            g.DrawLine(pen, calendarRect.Right - 3, calendarRect.Y - 2, calendarRect.Right - 3, calendarRect.Y + 2);
        }

        /// <summary>
        /// Draws a search icon
        /// </summary>
        public static void DrawSearchIcon(Graphics g, Rectangle rect, Color color)
        {
            using var pen = new Pen(color, 2);
            
            // Search circle
            var circleRect = new Rectangle(rect.X + 2, rect.Y + 2, rect.Width - 6, rect.Height - 6);
            g.DrawEllipse(pen, circleRect);
            
            // Search handle
            g.DrawLine(pen, rect.Right - 4, rect.Bottom - 4, rect.Right - 2, rect.Bottom - 2);
        }

        /// <summary>
        /// Draws a close/dismiss icon
        /// </summary>
        public static void DrawCloseIcon(Graphics g, Rectangle rect, Color color)
        {
            using var pen = new Pen(color, 2);
            g.DrawLine(pen, rect.X + 2, rect.Y + 2, rect.Right - 2, rect.Bottom - 2);
            g.DrawLine(pen, rect.Right - 2, rect.Y + 2, rect.X + 2, rect.Bottom - 2);
        }
    }

    /// <summary>
    /// Graphics extension methods for widget painters
    /// </summary>
    internal static class GraphicsExtensions
    {
        /// <summary>
        /// Fills a rounded rectangle - used by BalanceCardPainter and other painters
        /// </summary>
        public static void FillRoundedRectangle(this Graphics g, Brush brush, Rectangle rect, int radius)
        {
            if (g == null || brush == null || rect.IsEmpty) return;
            
            using var path = WidgetRenderingHelpers.CreateRoundedPath(rect, radius);
            g.FillPath(brush, path);
        }

        /// <summary>
        /// Draws a rounded rectangle outline
        /// </summary>
        public static void DrawRoundedRectangle(this Graphics g, Pen pen, Rectangle rect, int radius)
        {
            if (g == null || pen == null || rect.IsEmpty) return;
            
            using var path = WidgetRenderingHelpers.CreateRoundedPath(rect, radius);
            g.DrawPath(pen, path);
        }
    }

    /// <summary>
    /// Notification types for icon drawing
    /// </summary>
    internal enum NotificationType
    {
        Success,
        Warning,
        Error,
        Info,
        Progress
    }
}