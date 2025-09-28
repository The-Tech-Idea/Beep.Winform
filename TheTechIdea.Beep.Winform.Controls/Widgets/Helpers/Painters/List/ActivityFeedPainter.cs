using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers
{
    /// <summary>
    /// ActivityFeed - Timeline-style activities
    /// </summary>
    internal sealed class ActivityFeedPainter : WidgetPainterBase
    {
        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = 16;
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -8, -8);
            
            // Header with title
            ctx.HeaderRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.DrawingRect.Top + pad,
                ctx.DrawingRect.Width - pad * 2,
                24
            );
            
            // Content area for activities
            ctx.ContentRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.HeaderRect.Bottom + 8,
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
            // Draw title
            if (ctx.ShowHeader && !string.IsNullOrEmpty(ctx.Title))
            {
                using var titleFont = new Font(Owner.Font.FontFamily, 11f, FontStyle.Bold);
                using var titleBrush = new SolidBrush(Color.FromArgb(150, Color.Black));
                var format = new StringFormat { LineAlignment = StringAlignment.Center };
                g.DrawString(ctx.Title, titleFont, titleBrush, ctx.HeaderRect, format);
            }
            
            // Draw activity items
            if (ctx.CustomData.ContainsKey("Items"))
            {
                var items = (List<Dictionary<string, object>>)ctx.CustomData["Items"];
                int maxItems = ctx.CustomData.ContainsKey("MaxVisibleItems") ? (int)ctx.CustomData["MaxVisibleItems"] : 10;
                
                DrawActivityItems(g, ctx.ContentRect, items.Take(maxItems).ToList());
            }
        }

        private void DrawActivityItems(Graphics g, Rectangle rect, List<Dictionary<string, object>> items)
        {
            if (!items.Any()) return;
            
            int itemHeight = Math.Min(40, rect.Height / Math.Max(items.Count, 1));
            using var nameFont = new Font(Owner.Font.FontFamily, 9f, FontStyle.Regular);
            using var timeFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Regular);
            using var nameBrush = new SolidBrush(Color.FromArgb(180, Color.Black));
            using var timeBrush = new SolidBrush(Color.FromArgb(120, Color.Gray));
            
            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                int y = rect.Y + i * itemHeight;
                
                // Timeline dot
                var dotRect = new Rectangle(rect.X + 8, y + itemHeight / 2 - 4, 8, 8);
                using var dotBrush = new SolidBrush(Theme?.AccentColor ?? Color.Blue);
                g.FillEllipse(dotBrush, dotRect);
                
                // Timeline line (except for last item)
                if (i < items.Count - 1)
                {
                    using var linePen = new Pen(Color.FromArgb(50, Color.Gray), 1);
                    g.DrawLine(linePen, dotRect.X + 4, dotRect.Bottom, dotRect.X + 4, y + itemHeight);
                }
                
                // Content
                var contentRect = new Rectangle(rect.X + 24, y + 4, rect.Width - 32, itemHeight - 8);
                var nameRect = new Rectangle(contentRect.X, contentRect.Y, contentRect.Width * 2 / 3, contentRect.Height / 2);
                var timeRect = new Rectangle(contentRect.Right - contentRect.Width / 3, contentRect.Y, contentRect.Width / 3, contentRect.Height / 2);
                var descRect = new Rectangle(contentRect.X, contentRect.Y + contentRect.Height / 2, contentRect.Width, contentRect.Height / 2);
                
                // Draw name and time
                if (item.ContainsKey("Name"))
                {
                    g.DrawString(item["Name"].ToString(), nameFont, nameBrush, nameRect);
                }
                
                if (item.ContainsKey("Time"))
                {
                    var timeFormat = new StringFormat { Alignment = StringAlignment.Far };
                    g.DrawString(item["Time"].ToString(), timeFont, timeBrush, timeRect, timeFormat);
                }
                
                if (item.ContainsKey("Value"))
                {
                    g.DrawString(item["Value"].ToString(), timeFont, timeBrush, descRect);
                }
            }
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Optional: Draw scroll indicators or load more button
        }
    }
}