using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers
{
    /// <summary>
    /// ProfileList - User/profile listings
    /// </summary>
    internal sealed class ProfileListPainter : WidgetPainterBase
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
            
            // Draw profile items
            if (ctx.CustomData.ContainsKey("Items"))
            {
                var items = (List<Dictionary<string, object>>)ctx.CustomData["Items"];
                DrawProfileItems(g, ctx.ContentRect, items, ctx.AccentColor);
            }
        }

        private void DrawProfileItems(Graphics g, Rectangle rect, List<Dictionary<string, object>> items, Color accentColor)
        {
            if (!items.Any()) return;
            
            int itemHeight = Math.Min(48, rect.Height / Math.Max(items.Count, 1));
            using var nameFont = new Font(Owner.Font.FontFamily, 9f, FontStyle.Bold);
            using var roleFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Regular);
            
            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                int y = rect.Y + i * itemHeight;
                
                // Avatar placeholder
                var avatarRect = new Rectangle(rect.X + 8, y + 8, itemHeight - 16, itemHeight - 16);
                using var avatarBrush = new SolidBrush(Color.FromArgb(30, accentColor));
                g.FillEllipse(avatarBrush, avatarRect);
                
                // Avatar border
                using var avatarPen = new Pen(Color.FromArgb(100, accentColor), 1);
                g.DrawEllipse(avatarPen, avatarRect);
                
                // Profile info
                var nameRect = new Rectangle(rect.X + itemHeight + 8, y + 8, rect.Width - itemHeight - 16, (itemHeight - 16) / 2);
                var roleRect = new Rectangle(rect.X + itemHeight + 8, nameRect.Bottom, rect.Width - itemHeight - 16, (itemHeight - 16) / 2);
                
                if (item.ContainsKey("Name"))
                {
                    using var nameBrush = new SolidBrush(Color.FromArgb(180, Color.Black));
                    var nameFormat = new StringFormat { LineAlignment = StringAlignment.Center };
                    g.DrawString(item["Name"].ToString(), nameFont, nameBrush, nameRect, nameFormat);
                }
                
                if (item.ContainsKey("Value"))
                {
                    using var roleBrush = new SolidBrush(Color.FromArgb(120, Color.Gray));
                    var roleFormat = new StringFormat { LineAlignment = StringAlignment.Center };
                    g.DrawString(item["Value"].ToString(), roleFont, roleBrush, roleRect, roleFormat);
                }
            }
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Optional: Draw online status indicators or action buttons
        }
    }
}