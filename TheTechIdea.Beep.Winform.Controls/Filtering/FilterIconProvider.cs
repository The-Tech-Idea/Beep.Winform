using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Utilities;

namespace TheTechIdea.Beep.Winform.Controls.Filtering
{
    /// <summary>
    /// Provides icons for filter UI elements
    /// Renders icons for column types, operators, and filter states
    /// </summary>
    public static class FilterIconProvider
    {
        /// <summary>
        /// Gets an emoji icon for a column type
        /// </summary>
        public static string GetColumnTypeIcon(DbFieldCategory columnType)
        {
            return columnType switch
            {
                DbFieldCategory.String => "🔤",
                DbFieldCategory.Text => "📝",
                DbFieldCategory.Numeric => "🔢",
                DbFieldCategory.Integer => "🔢",
                DbFieldCategory.Decimal => "💰",
                DbFieldCategory.Currency => "💵",
                DbFieldCategory.Date => "📅",
                DbFieldCategory.DateTime => "🕒",
                DbFieldCategory.Boolean => "✓",
                DbFieldCategory.Image => "🖼️",
                DbFieldCategory.Guid => "🔑",
                _ => "📋"
            };
        }
        
        /// <summary>
        /// Gets an emoji icon for a filter operator
        /// </summary>
        public static string GetOperatorIcon(FilterOperator op)
        {
            return op switch
            {
                FilterOperator.Equals => "=",
                FilterOperator.NotEquals => "≠",
                FilterOperator.Contains => "⊃",
                FilterOperator.NotContains => "⊅",
                FilterOperator.StartsWith => "^",
                FilterOperator.EndsWith => "$",
                FilterOperator.GreaterThan => ">",
                FilterOperator.GreaterThanOrEqual => "≥",
                FilterOperator.LessThan => "<",
                FilterOperator.LessThanOrEqual => "≤",
                FilterOperator.Between => "↔",
                FilterOperator.NotBetween => "↮",
                FilterOperator.IsNull => "∅",
                FilterOperator.IsNotNull => "∃",
                FilterOperator.Regex => ".*",
                FilterOperator.In => "∈",
                FilterOperator.NotIn => "∉",
                _ => "?"
            };
        }
        
        /// <summary>
        /// Draws a column type icon
        /// </summary>
        public static void DrawTypeIcon(Graphics g, Rectangle bounds, DbFieldCategory columnType, Color color)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            using (var brush = new SolidBrush(color))
            using (var pen = new Pen(color, 1.5f))
            {
                switch (columnType)
                {
                    case DbFieldCategory.String:
                    case DbFieldCategory.Text:
                        DrawTextIcon(g, bounds, brush, pen);
                        break;
                    
                    case DbFieldCategory.Numeric:
                    case DbFieldCategory.Integer:
                    case DbFieldCategory.Decimal:
                    case DbFieldCategory.Currency:
                        DrawNumberIcon(g, bounds, brush, pen);
                        break;
                    
                    case DbFieldCategory.Date:
                    case DbFieldCategory.DateTime:
                        DrawDateIcon(g, bounds, brush, pen);
                        break;
                    
                    case DbFieldCategory.Boolean:
                        DrawBooleanIcon(g, bounds, brush, pen);
                        break;
                    
                    default:
                        DrawGenericIcon(g, bounds, brush, pen);
                        break;
                }
            }
        }
        
        private static void DrawTextIcon(Graphics g, Rectangle bounds, Brush brush, Pen pen)
        {
            // Draw "Aa" text icon
            using (var font = new Font("Segoe UI", bounds.Height * 0.5f, FontStyle.Bold))
            {
                var format = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };
                g.DrawString("Aa", font, brush, bounds, format);
            }
        }
        
        private static void DrawNumberIcon(Graphics g, Rectangle bounds, Brush brush, Pen pen)
        {
            // Draw "123" or "#" icon
            using (var font = new Font("Segoe UI", bounds.Height * 0.45f, FontStyle.Bold))
            {
                var format = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };
                g.DrawString("#", font, brush, bounds, format);
            }
        }
        
        private static void DrawDateIcon(Graphics g, Rectangle bounds, Brush brush, Pen pen)
        {
            // Draw calendar icon
            int padding = bounds.Width / 6;
            var calRect = new Rectangle(
                bounds.X + padding,
                bounds.Y + padding,
                bounds.Width - padding * 2,
                bounds.Height - padding * 2
            );
            
            // Calendar body
            g.DrawRectangle(pen, calRect);
            
            // Calendar header
            var headerRect = new Rectangle(calRect.X, calRect.Y, calRect.Width, calRect.Height / 4);
            g.FillRectangle(brush, headerRect);
            
            // Calendar rings
            int ringWidth = calRect.Width / 6;
            g.DrawLine(pen, calRect.X + ringWidth, calRect.Y - padding / 2, 
                           calRect.X + ringWidth, calRect.Y + padding / 2);
            g.DrawLine(pen, calRect.Right - ringWidth, calRect.Y - padding / 2,
                           calRect.Right - ringWidth, calRect.Y + padding / 2);
        }
        
        private static void DrawBooleanIcon(Graphics g, Rectangle bounds, Brush brush, Pen pen)
        {
            // Draw checkmark
            int padding = bounds.Width / 5;
            Point[] points = new[]
            {
                new Point(bounds.X + padding, bounds.Y + bounds.Height / 2),
                new Point(bounds.X + bounds.Width / 2 - padding / 2, bounds.Bottom - padding),
                new Point(bounds.Right - padding, bounds.Y + padding)
            };
            
            using (var thickPen = new Pen(brush, 2f))
            {
                thickPen.StartCap = LineCap.Round;
                thickPen.EndCap = LineCap.Round;
                thickPen.LineJoin = LineJoin.Round;
                g.DrawLines(thickPen, points);
            }
        }
        
        private static void DrawGenericIcon(Graphics g, Rectangle bounds, Brush brush, Pen pen)
        {
            // Draw generic document icon
            int padding = bounds.Width / 6;
            var docRect = new Rectangle(
                bounds.X + padding,
                bounds.Y + padding,
                bounds.Width - padding * 2,
                bounds.Height - padding * 2
            );
            
            g.DrawRectangle(pen, docRect);
            
            // Draw lines inside
            int lineSpacing = docRect.Height / 4;
            for (int i = 1; i <= 2; i++)
            {
                int y = docRect.Y + lineSpacing * i;
                g.DrawLine(pen, docRect.X + padding, y, docRect.Right - padding, y);
            }
        }
        
        /// <summary>
        /// Draws a filter state icon (active, disabled, error)
        /// </summary>
        public static void DrawStateIcon(Graphics g, Rectangle bounds, FilterState state, Color color)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            switch (state)
            {
                case FilterState.Active:
                    DrawCheckIcon(g, bounds, color);
                    break;
                
                case FilterState.Disabled:
                    DrawDisabledIcon(g, bounds, color);
                    break;
                
                case FilterState.Error:
                    DrawErrorIcon(g, bounds, color);
                    break;
                
                case FilterState.Warning:
                    DrawWarningIcon(g, bounds, color);
                    break;
            }
        }
        
        private static void DrawCheckIcon(Graphics g, Rectangle bounds, Color color)
        {
            using (var pen = new Pen(color, 2f))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;
                
                Point[] points = new[]
                {
                    new Point(bounds.X + 2, bounds.Y + bounds.Height / 2),
                    new Point(bounds.X + bounds.Width / 2 - 1, bounds.Bottom - 3),
                    new Point(bounds.Right - 2, bounds.Y + 2)
                };
                
                g.DrawLines(pen, points);
            }
        }
        
        private static void DrawDisabledIcon(Graphics g, Rectangle bounds, Color color)
        {
            using (var pen = new Pen(Color.FromArgb(100, color), 2f))
            {
                // Draw circle with slash
                g.DrawEllipse(pen, bounds);
                g.DrawLine(pen, bounds.X + 2, bounds.Y + 2, bounds.Right - 2, bounds.Bottom - 2);
            }
        }
        
        private static void DrawErrorIcon(Graphics g, Rectangle bounds, Color color)
        {
            var errorColor = Color.FromArgb(220, 53, 69); // Red
            using (var brush = new SolidBrush(errorColor))
            {
                g.FillEllipse(brush, bounds);
            }
            
            // Draw X
            using (var pen = new Pen(Color.White, 2f))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;
                
                int padding = bounds.Width / 4;
                g.DrawLine(pen, bounds.X + padding, bounds.Y + padding, 
                               bounds.Right - padding, bounds.Bottom - padding);
                g.DrawLine(pen, bounds.Right - padding, bounds.Y + padding,
                               bounds.X + padding, bounds.Bottom - padding);
            }
        }
        
        private static void DrawWarningIcon(Graphics g, Rectangle bounds, Color color)
        {
            var warningColor = Color.FromArgb(255, 193, 7); // Amber
            
            // Draw triangle
            Point[] triangle = new[]
            {
                new Point(bounds.X + bounds.Width / 2, bounds.Y + 2),
                new Point(bounds.X + 2, bounds.Bottom - 2),
                new Point(bounds.Right - 2, bounds.Bottom - 2)
            };
            
            using (var brush = new SolidBrush(warningColor))
            {
                g.FillPolygon(brush, triangle);
            }
            
            // Draw exclamation mark
            using (var pen = new Pen(Color.White, 2f))
            {
                int cx = bounds.X + bounds.Width / 2;
                g.DrawLine(pen, cx, bounds.Y + bounds.Height / 3, cx, bounds.Y + bounds.Height * 2 / 3);
            }
            
            using (var brush = new SolidBrush(Color.White))
            {
                var dotRect = new Rectangle(
                    bounds.X + bounds.Width / 2 - 1,
                    bounds.Bottom - bounds.Height / 4 - 1,
                    3, 3);
                g.FillEllipse(brush, dotRect);
            }
        }
    }
    
    /// <summary>
    /// Filter state for visual indication
    /// </summary>
    public enum FilterState
    {
        Active,
        Disabled,
        Error,
        Warning
    }
}

