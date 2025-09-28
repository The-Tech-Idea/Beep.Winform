using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers
{
    /// <summary>
    /// DataTable - Structured data table
    /// </summary>
    internal sealed class DataTablePainter : WidgetPainterBase
    {
        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = 16;
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -8, -8);
            
            // Header row
            ctx.HeaderRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.DrawingRect.Top + pad,
                ctx.DrawingRect.Width - pad * 2,
                28
            );
            
            // Data rows
            ctx.ContentRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.HeaderRect.Bottom + 2,
                ctx.DrawingRect.Width - pad * 2,
                ctx.DrawingRect.Height - ctx.HeaderRect.Height - pad * 3
            );
            
            return ctx;
        }

        public override void DrawBackground(Graphics g, WidgetContext ctx)
        {
            DrawSoftShadow(g, ctx.DrawingRect, 12, layers: 4, offset: 2);
            using var bgBrush = new SolidBrush(Theme?.BackColor ?? Color.White);
            using var bgPath = CreateRoundedPath(ctx.DrawingRect, ctx.CornerRadius);
            g.FillPath(bgBrush, bgPath);
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            // Draw header
            if (ctx.ShowHeader && ctx.Labels?.Any() == true)
            {
                DrawTableHeader(g, ctx.HeaderRect, ctx.Labels);
            }
            
            // Draw data rows
            if (ctx.CustomData.ContainsKey("Items"))
            {
                var items = (List<Dictionary<string, object>>)ctx.CustomData["Items"];
                int maxItems = ctx.CustomData.ContainsKey("MaxVisibleItems") ? (int)ctx.CustomData["MaxVisibleItems"] : 10;
                
                DrawTableRows(g, ctx.ContentRect, items.Take(maxItems).ToList(), ctx.Labels);
            }
        }

        private void DrawTableHeader(Graphics g, Rectangle rect, List<string> columns)
        {
            // Header background
            using var headerBrush = new SolidBrush(Color.FromArgb(20, Color.Gray));
            g.FillRectangle(headerBrush, rect);
            
            // Column headers
            using var headerFont = new Font(Owner.Font.FontFamily, 9f, FontStyle.Bold);
            using var headerTextBrush = new SolidBrush(Color.FromArgb(150, Color.Black));
            
            int colWidth = rect.Width / Math.Max(columns.Count, 1);
            for (int i = 0; i < columns.Count; i++)
            {
                var colRect = new Rectangle(rect.X + i * colWidth, rect.Y, colWidth - 1, rect.Height);
                var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(columns[i], headerFont, headerTextBrush, colRect, format);
                
                // Column separator
                if (i < columns.Count - 1)
                {
                    using var separatorPen = new Pen(Color.FromArgb(50, Color.Gray), 1);
                    g.DrawLine(separatorPen, colRect.Right, rect.Top, colRect.Right, rect.Bottom);
                }
            }
        }

        private void DrawTableRows(Graphics g, Rectangle rect, List<Dictionary<string, object>> items, List<string> columns)
        {
            if (!items.Any() || !columns.Any()) return;
            
            int rowHeight = Math.Min(24, rect.Height / Math.Max(items.Count, 1));
            int colWidth = rect.Width / columns.Count;
            
            using var cellFont = new Font(Owner.Font.FontFamily, 8f);
            using var cellBrush = new SolidBrush(Color.FromArgb(150, Color.Black));
            
            for (int row = 0; row < items.Count; row++)
            {
                var item = items[row];
                int y = rect.Y + row * rowHeight;
                
                // Alternate row background
                if (row % 2 == 1)
                {
                    using var altRowBrush = new SolidBrush(Color.FromArgb(10, Color.Gray));
                    g.FillRectangle(altRowBrush, rect.X, y, rect.Width, rowHeight);
                }
                
                // Cell data
                for (int col = 0; col < columns.Count; col++)
                {
                    var cellRect = new Rectangle(rect.X + col * colWidth + 4, y, colWidth - 8, rowHeight);
                    string cellValue = item.ContainsKey(columns[col]) ? item[columns[col]]?.ToString() ?? "" : "";
                    
                    var format = new StringFormat { LineAlignment = StringAlignment.Center, Trimming = StringTrimming.EllipsisCharacter };
                    g.DrawString(cellValue, cellFont, cellBrush, cellRect, format);
                }
                
                // Row separator
                using var rowPen = new Pen(Color.FromArgb(30, Color.Gray), 1);
                g.DrawLine(rowPen, rect.X, y + rowHeight - 1, rect.Right, y + rowHeight - 1);
            }
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Optional: Draw selection highlights or sort indicators
        }
    }
}