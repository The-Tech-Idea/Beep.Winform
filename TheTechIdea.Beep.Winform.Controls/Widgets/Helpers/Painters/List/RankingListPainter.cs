using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers
{
    /// <summary>
    /// RankingList - Ordered ranking list
    /// </summary>
    internal sealed class RankingListPainter : WidgetPainterBase
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
            
            // Draw ranking items
            if (ctx.CustomData.ContainsKey("Items"))
            {
                var items = (List<Dictionary<string, object>>)ctx.CustomData["Items"];
                DrawRankingItems(g, ctx.ContentRect, items);
            }
        }

        private void DrawRankingItems(Graphics g, Rectangle rect, List<Dictionary<string, object>> items)
        {
            if (!items.Any()) return;
            
            int itemHeight = Math.Min(32, rect.Height / Math.Max(items.Count, 1));
            using var nameFont = new Font(Owner.Font.FontFamily, 9f, FontStyle.Regular);
            using var rankFont = new Font(Owner.Font.FontFamily, 10f, FontStyle.Bold);
            using var valueFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Regular);
            
            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                int y = rect.Y + i * itemHeight;
                
                // Rank badge
                var rankRect = new Rectangle(rect.X, y + 4, 24, itemHeight - 8);
                Color rankColor = i < 3 ? GetRankColor(i) : Color.FromArgb(100, Color.Gray);
                
                using var rankBrush = new SolidBrush(rankColor);
                using var rankPath = CreateRoundedPath(rankRect, 4);
                g.FillPath(rankBrush, rankPath);
                
                using var rankTextBrush = new SolidBrush(Color.White);
                var rankFormat = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString((i + 1).ToString(), rankFont, rankTextBrush, rankRect, rankFormat);
                
                // Item content
                var contentRect = new Rectangle(rect.X + 32, y, rect.Width - 100, itemHeight);
                var valueRect = new Rectangle(rect.Right - 60, y, 60, itemHeight);
                
                if (item.ContainsKey("Name"))
                {
                    using var nameBrush = new SolidBrush(Color.FromArgb(180, Color.Black));
                    var nameFormat = new StringFormat { LineAlignment = StringAlignment.Center };
                    g.DrawString(item["Name"].ToString(), nameFont, nameBrush, contentRect, nameFormat);
                }
                
                if (item.ContainsKey("Value"))
                {
                    using var valueBrush = new SolidBrush(Color.FromArgb(120, Color.Black));
                    var valueFormat = new StringFormat { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Center };
                    g.DrawString(item["Value"].ToString(), valueFont, valueBrush, valueRect, valueFormat);
                }
            }
        }

        private Color GetRankColor(int rank)
        {
            return rank switch
            {
                0 => Color.FromArgb(255, 193, 7),  // Gold
                1 => Color.FromArgb(158, 158, 158), // Silver
                2 => Color.FromArgb(205, 127, 50),  // Bronze
                _ => Color.Gray
            };
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Optional: Draw medals or special indicators for top ranks
        }
    }
}