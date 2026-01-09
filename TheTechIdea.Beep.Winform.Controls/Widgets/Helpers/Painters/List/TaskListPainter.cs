using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Widgets.Models;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers
{
    /// <summary>
    /// TaskList - Checklist/todo Style with checkbox and item hit areas
    /// </summary>
    internal sealed class TaskListPainter : WidgetPainterBase
    {
        private readonly List<Rectangle> _itemRects = new();
        private readonly List<Rectangle> _checkboxRects = new();

        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = 16;
            var baseRect = Owner?.DrawingRect ?? drawingRect;
            ctx.DrawingRect = Rectangle.Inflate(baseRect, -8, -8);
            
            ctx.HeaderRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.DrawingRect.Top + pad, ctx.DrawingRect.Width - pad * 2, 24);
            ctx.ContentRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.HeaderRect.Bottom + 8, ctx.DrawingRect.Width - pad * 2, ctx.DrawingRect.Height - ctx.HeaderRect.Height - pad * 3);
            
            // Precompute item and checkbox rects
            _itemRects.Clear();
            _checkboxRects.Clear();
            var items = ctx.ListItems;
            if (items != null && items.Count > 0)
            {
                int itemHeight = Math.Min(28, ctx.ContentRect.Height / Math.Max(items.Count, 1));
                for (int i = 0; i < items.Count; i++)
                {
                    int y = ctx.ContentRect.Y + i * itemHeight;
                    var itemRect = new Rectangle(ctx.ContentRect.X, y, ctx.ContentRect.Width, itemHeight);
                    _itemRects.Add(itemRect);
                    _checkboxRects.Add(new Rectangle(itemRect.X + 8, y + itemHeight / 2 - 6, 12, 12));
                }
            }
            
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
            var items = ctx.ListItems;
            if (items != null && items.Count > 0)
            {
                DrawTaskItems(g, ctx.ContentRect, items);
            }
        }

        private void DrawTaskItems(Graphics g, Rectangle rect, List<ListItem> items)
        {
            if (!items.Any()) return;
            
            int itemHeight = Math.Min(28, rect.Height / Math.Max(items.Count, 1));
            using var taskFont = new Font(Owner.Font.FontFamily, 9f, FontStyle.Regular);
            
            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                int y = rect.Y + i * itemHeight;

                var itemRect = _itemRects.Count > i ? _itemRects[i] : new Rectangle(rect.X, y, rect.Width, itemHeight);
                var checkboxRect = _checkboxRects.Count > i ? _checkboxRects[i] : new Rectangle(itemRect.X + 8, y + itemHeight / 2 - 6, 12, 12);

                // Hover background
                if (IsAreaHovered($"TaskList_Item_{i}"))
                {
                    using var hover = new SolidBrush(Color.FromArgb(6, Theme?.PrimaryColor ?? Color.Blue));
                    g.FillRectangle(hover, itemRect);
                }
                
                // Checkbox
                // Check if it's a TaskItem and if it's completed
                bool isCompleted = item is TaskItem taskItem && taskItem.IsCompleted;
                using var checkboxPen = new Pen(Color.FromArgb(IsAreaHovered($"TaskList_Check_{i}") ? 200 : 150, Color.Gray), 1);
                g.DrawRectangle(checkboxPen, checkboxRect);
                
                if (isCompleted)
                {
                    using var checkBrush = new SolidBrush(Color.Green);
                    g.FillRectangle(checkBrush, Rectangle.Inflate(checkboxRect, -2, -2));
                    using var checkPen = new Pen(Color.White, 2);
                    g.DrawLines(checkPen, new Point[]
                    {
                        new Point(checkboxRect.X + 3, checkboxRect.Y + 6),
                        new Point(checkboxRect.X + 6, checkboxRect.Y + 9),
                        new Point(checkboxRect.X + 10, checkboxRect.Y + 3)
                    });
                }
                
                // Task text
                var taskRect = new Rectangle(itemRect.X + 28, y, itemRect.Width - 36, itemHeight);
                if (!string.IsNullOrEmpty(item.Title))
                {
                    Color textColor = isCompleted ? Color.FromArgb(120, Color.Gray) : Color.FromArgb(180, Color.Black);
                    FontStyle fontStyle = isCompleted ? FontStyle.Strikeout : FontStyle.Regular;
                    using var taskTextFont = new Font(Owner.Font.FontFamily, 9f, fontStyle);
                    using var taskBrush = new SolidBrush(textColor);
                    var taskFormat = new StringFormat { LineAlignment = StringAlignment.Center };
                    g.DrawString(item.Title, taskTextFont, taskBrush, taskRect, taskFormat);
                }
            }
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Checkbox hover stroke
            for (int i = 0; i < _checkboxRects.Count; i++)
            {
                if (IsAreaHovered($"TaskList_Check_{i}"))
                {
                    using var pen = new Pen(Theme?.AccentColor ?? Color.Blue, 1.5f);
                    g.DrawRectangle(pen, _checkboxRects[i]);
                }
            }
        }

        public override void UpdateHitAreas(BaseControl owner, WidgetContext ctx, Action<string, Rectangle>? notifyAreaHit)
        {
            if (owner == null) return;
            ClearOwnerHitAreas();

            for (int i = 0; i < _itemRects.Count; i++)
            {
                int idx = i;
                var itemRect = _itemRects[i];
                var checkRect = _checkboxRects[i];

                owner.AddHitArea($"TaskList_Item_{idx}", itemRect, null, () =>
                {
                    ctx.SelectedTaskIndex = idx;
                    notifyAreaHit?.Invoke($"TaskList_Item_{idx}", itemRect);
                    Owner?.Invalidate();
                });

                owner.AddHitArea($"TaskList_Check_{idx}", checkRect, null, () =>
                {
                    ctx.ToggleTaskIndex = idx;
                    notifyAreaHit?.Invoke($"TaskList_Check_{idx}", checkRect);
                    Owner?.Invalidate();
                });
            }
        }
    }
}