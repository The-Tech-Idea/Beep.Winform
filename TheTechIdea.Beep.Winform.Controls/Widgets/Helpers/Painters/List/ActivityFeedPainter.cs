using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Widgets.Models;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers
{
    /// <summary>
    /// ActivityFeed - Timeline-Style activities with hover and per-item hit areas
    /// </summary>
    internal sealed class ActivityFeedPainter : WidgetPainterBase
    {
        private readonly List<Rectangle> _itemRects = new();
        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = 16;
            var baseRect = Owner?.DrawingRect ?? drawingRect;
            ctx.DrawingRect = Rectangle.Inflate(baseRect, -8, -8);
            
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

            _itemRects.Clear();
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
                using var titleFont = new Font(Owner?.Font?.FontFamily ?? SystemFonts.DefaultFont.FontFamily, 11f, FontStyle.Bold);
                using var titleBrush = new SolidBrush(Color.FromArgb(150, Theme?.ForeColor ?? Color.Black));
                var format = new StringFormat { LineAlignment = StringAlignment.Center };
                g.DrawString(ctx.Title, titleFont, titleBrush, ctx.HeaderRect, format);
            }
            
            // Draw activity items
            var items = ctx.ListItems;
            if (items != null && items.Count > 0)
            {
                int maxItems = ctx.MaxVisibleItems;
                
                DrawActivityItems(g, ctx, ctx.ContentRect, items.Take(maxItems).ToList());
            }
        }

        private void DrawActivityItems(Graphics g, WidgetContext ctx, Rectangle rect, List<ListItem> items)
        {
            if (!items.Any()) return;
            
            int itemHeight = Math.Min(40, rect.Height / Math.Max(items.Count, 1));
            using var nameFont = new Font(Owner?.Font?.FontFamily ?? SystemFonts.DefaultFont.FontFamily, 9f, FontStyle.Regular);
            using var timeFont = new Font(Owner?.Font?.FontFamily ?? SystemFonts.DefaultFont.FontFamily, 8f, FontStyle.Regular);
            using var nameBrush = new SolidBrush(Color.FromArgb(180, Theme?.ForeColor ?? Color.Black));
            using var timeBrush = new SolidBrush(Color.FromArgb(120, Theme?.ForeColor ?? Color.Gray));
            
            _itemRects.Clear();

            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                int y = rect.Y + i * itemHeight;
                var itemRect = new Rectangle(rect.X, y, rect.Width, itemHeight);
                _itemRects.Add(itemRect);

                // Hover background
                if (IsAreaHovered($"ActivityFeed_Item_{i}"))
                {
                    using var hover = new SolidBrush(Color.FromArgb(6, Theme?.PrimaryColor ?? Color.Blue));
                    g.FillRectangle(hover, itemRect);
                }
                
                // Timeline dot
                var dotRect = new Rectangle(rect.X + 8, y + itemHeight / 2 - 4, 8, 8);
                using var dotBrush = new SolidBrush(Theme?.AccentColor ?? Color.Blue);
                g.FillEllipse(dotBrush, dotRect);
                
                // Timeline line (except for last item)
                if (i < items.Count - 1)
                {
                    using var linePen = new Pen(Color.FromArgb(50, Theme?.BorderColor ?? Color.Gray), 1);
                    g.DrawLine(linePen, dotRect.X + 4, dotRect.Bottom, dotRect.X + 4, y + itemHeight);
                }
                
                // Content
                var contentRect = new Rectangle(rect.X + 24, y + 4, rect.Width - 32, itemHeight - 8);
                var nameRect = new Rectangle(contentRect.X, contentRect.Y, contentRect.Width * 2 / 3, contentRect.Height / 2);
                var timeRect = new Rectangle(contentRect.Right - contentRect.Width / 3, contentRect.Y, contentRect.Width / 3, contentRect.Height / 2);
                var descRect = new Rectangle(contentRect.X, contentRect.Y + contentRect.Height / 2, contentRect.Width, contentRect.Height / 2);
                
                // Draw name and time
                if (!string.IsNullOrEmpty(item.Title))
                {
                    g.DrawString(item.Title, nameFont, nameBrush, nameRect);
                }
                
                if (item.Timestamp != default(DateTime))
                {
                    var timeFormat = new StringFormat { Alignment = StringAlignment.Far };
                    g.DrawString(item.Timestamp.ToString("HH:mm"), timeFont, timeBrush, timeRect, timeFormat);
                }
                
                if (!string.IsNullOrEmpty(item.Subtitle))
                {
                    g.DrawString(item.Subtitle, timeFont, timeBrush, descRect);
                }
            }
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Optional: Draw scroll indicators or load more button
        }

        public override void UpdateHitAreas(BaseControl owner, WidgetContext ctx, Action<string, Rectangle>? notifyAreaHit)
        {
            if (owner == null) return;
            ClearOwnerHitAreas();
            for (int i = 0; i < _itemRects.Count; i++)
            {
                int idx = i;
                var rect = _itemRects[i];
                owner.AddHitArea($"ActivityFeed_Item_{idx}", rect, null, () =>
                {
                    ctx.SelectedActivityIndex = idx;
                    notifyAreaHit?.Invoke($"ActivityFeed_Item_{idx}", rect);
                    Owner?.Invalidate();
                });
            }
        }
    }
}