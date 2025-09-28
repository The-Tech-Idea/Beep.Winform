using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers
{
    /// <summary>
    /// TaskList - Checklist/todo style  
    /// </summary>
    internal sealed class TaskListPainter : WidgetPainterBase
    {
        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = 16;
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -8, -8);
            
            ctx.HeaderRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.DrawingRect.Top + pad, ctx.DrawingRect.Width - pad * 2, 24);
            ctx.ContentRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.HeaderRect.Bottom + 8, ctx.DrawingRect.Width - pad * 2, ctx.DrawingRect.Height - ctx.HeaderRect.Height - pad * 3);
            
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
            // Draw title
            if (ctx.ShowHeader && !string.IsNullOrEmpty(ctx.Title))
            {
                using var titleFont = new Font(Owner.Font.FontFamily, 11f, FontStyle.Bold);
                using var titleBrush = new SolidBrush(Color.FromArgb(150, Color.Black));
                g.DrawString(ctx.Title, titleFont, titleBrush, ctx.HeaderRect);
            }
            
            // Draw task items
            if (ctx.CustomData.ContainsKey("Items"))
            {
                var items = (List<Dictionary<string, object>>)ctx.CustomData["Items"];
                DrawTaskItems(g, ctx.ContentRect, items);
            }
        }

        private void DrawTaskItems(Graphics g, Rectangle rect, List<Dictionary<string, object>> items)
        {
            if (!items.Any()) return;
            
            int itemHeight = Math.Min(28, rect.Height / Math.Max(items.Count, 1));
            using var taskFont = new Font(Owner.Font.FontFamily, 9f, FontStyle.Regular);
            
            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                int y = rect.Y + i * itemHeight;
                
                // Checkbox
                var checkboxRect = new Rectangle(rect.X + 8, y + itemHeight / 2 - 6, 12, 12);
                bool isCompleted = item.ContainsKey("Status") && item["Status"].ToString().ToLower() == "completed";
                
                using var checkboxPen = new Pen(Color.FromArgb(150, Color.Gray), 1);
                g.DrawRectangle(checkboxPen, checkboxRect);
                
                if (isCompleted)
                {
                    using var checkBrush = new SolidBrush(Color.Green);
                    g.FillRectangle(checkBrush, Rectangle.Inflate(checkboxRect, -2, -2));
                    
                    // Checkmark
                    using var checkPen = new Pen(Color.White, 2);
                    g.DrawLines(checkPen, new Point[]
                    {
                        new Point(checkboxRect.X + 3, checkboxRect.Y + 6),
                        new Point(checkboxRect.X + 6, checkboxRect.Y + 9),
                        new Point(checkboxRect.X + 10, checkboxRect.Y + 3)
                    });
                }
                
                // Task text
                var taskRect = new Rectangle(rect.X + 28, y, rect.Width - 36, itemHeight);
                if (item.ContainsKey("Name"))
                {
                    Color textColor = isCompleted ? Color.FromArgb(120, Color.Gray) : Color.FromArgb(180, Color.Black);
                    FontStyle fontStyle = isCompleted ? FontStyle.Strikeout : FontStyle.Regular;
                    
                    using var taskTextFont = new Font(Owner.Font.FontFamily, 9f, fontStyle);
                    using var taskBrush = new SolidBrush(textColor);
                    var taskFormat = new StringFormat { LineAlignment = StringAlignment.Center };
                    g.DrawString(item["Name"].ToString(), taskTextFont, taskBrush, taskRect, taskFormat);
                }
            }
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Optional: Draw progress indicator or completion summary
        }
    }
}