using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers
{
    /// <summary>
    /// StatusList - Items with status indicators
    /// </summary>
    internal sealed class StatusListPainter : WidgetPainterBase
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
            
            // Draw status items
            if (ctx.CustomData.ContainsKey("Items"))
            {
                var items = (List<Dictionary<string, object>>)ctx.CustomData["Items"];
                DrawStatusItems(g, ctx.ContentRect, items);
            }
        }

        private void DrawStatusItems(Graphics g, Rectangle rect, List<Dictionary<string, object>> items)
        {
            if (!items.Any()) return;
            
            int itemHeight = Math.Min(28, rect.Height / Math.Max(items.Count, 1));
            using var nameFont = new Font(Owner.Font.FontFamily, 9f, FontStyle.Regular);
            using var statusFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Regular);
            
            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                int y = rect.Y + i * itemHeight;
                
                // Status indicator dot
                string status = item.ContainsKey("Status") ? item["Status"].ToString() : "Unknown";
                Color statusColor = GetStatusColor(status);
                
                var dotRect = new Rectangle(rect.X + 8, y + itemHeight / 2 - 4, 8, 8);
                using var dotBrush = new SolidBrush(statusColor);
                g.FillEllipse(dotBrush, dotRect);
                
                // Item name
                var nameRect = new Rectangle(rect.X + 24, y, rect.Width - 120, itemHeight);
                if (item.ContainsKey("Name"))
                {
                    using var nameBrush = new SolidBrush(Color.FromArgb(180, Color.Black));
                    var nameFormat = new StringFormat { LineAlignment = StringAlignment.Center };
                    g.DrawString(item["Name"].ToString(), nameFont, nameBrush, nameRect, nameFormat);
                }
                
                // Status text
                var statusRect = new Rectangle(rect.Right - 90, y, 90, itemHeight);
                using var statusBrush = new SolidBrush(statusColor);
                var statusFormat = new StringFormat { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Center };
                g.DrawString(status, statusFont, statusBrush, statusRect, statusFormat);
            }
        }

        private Color GetStatusColor(string status)
        {
            return status.ToLower() switch
            {
                "active" or "online" or "running" => Color.Green,
                "inactive" or "offline" or "stopped" => Color.Red,
                "busy" or "warning" or "pending" => Color.Orange,
                "idle" or "paused" => Color.Gray,
                _ => Color.Gray
            };
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Optional: Draw status legend or summary counts
        }
    }
}