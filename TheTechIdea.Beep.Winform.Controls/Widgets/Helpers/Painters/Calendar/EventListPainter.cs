using System;
using System.Drawing;
using System.Collections.Generic;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Painters.Calendar
{
    /// <summary>
    /// EventList - List of upcoming events painter with hit areas and hover accents
    /// </summary>
    internal sealed class EventListPainter : WidgetPainterBase
    {
        private readonly List<(Rectangle rect, int index)> _itemRects = new();
        private Rectangle _listRectCache;

        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            var baseRect = Owner?.DrawingRect ?? drawingRect;
            ctx.DrawingRect = Rectangle.Inflate(baseRect, -4, -4);
            _listRectCache = ctx.DrawingRect;
            _itemRects.Clear();
            return ctx;
        }

        public override void DrawBackground(Graphics g, WidgetContext ctx)
        {
            using var bgBrush = new SolidBrush(Theme?.BackColor ?? Color.White);
            using var bgPath = CreateRoundedPath(ctx.DrawingRect, ctx.CornerRadius);
            g.FillPath(bgBrush, bgPath);
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            DrawEventList(g, ctx);
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx) 
        {
            DrawEventSeparators(g, ctx);
        }

        private struct EventItem
        {
            public string Title;
            public string Time;
            public string Duration;
            public Color Color;
        }

        private void DrawEventList(Graphics g, WidgetContext ctx)
        {
            var events = GetEventItems(ctx);

            int itemHeight = 40;
            int padding = 8;
            int startY = ctx.DrawingRect.Top + padding;

            using var titleFont = new Font(Owner?.Font?.FontFamily ?? SystemFonts.DefaultFont.FontFamily, 9f, FontStyle.Bold);
            using var timeFont = new Font(Owner?.Font?.FontFamily ?? SystemFonts.DefaultFont.FontFamily, 8f, FontStyle.Regular);
            using var titleBrush = new SolidBrush(Theme?.TextBoxForeColor ?? Theme?.ForeColor ?? Color.Black);
            using var timeBrush = new SolidBrush(Color.FromArgb(120, Theme?.ForeColor ?? Color.Black));

            _itemRects.Clear();
            for (int i = 0; i < events.Count && startY + i * (itemHeight + 4) + itemHeight <= ctx.DrawingRect.Bottom - padding; i++)
            {
                var eventItem = events[i];
                var itemRect = new Rectangle(
                    ctx.DrawingRect.Left + padding,
                    startY + i * (itemHeight + 4),
                    ctx.DrawingRect.Width - padding * 2,
                    itemHeight
                );
                _itemRects.Add((itemRect, i));

                bool hovered = IsAreaHovered($"EventList_Item_{i}");
                if (hovered)
                {
                    using var hover = new SolidBrush(Color.FromArgb(8, Theme?.PrimaryColor ?? Color.Blue));
                    g.FillRoundedRectangle(hover, Rectangle.Inflate(itemRect, 2, 2), 4);
                }

                // Event color indicator
                var colorRect = new Rectangle(itemRect.Left, itemRect.Top + 8, 4, itemHeight - 16);
                using (var colorBrush = new SolidBrush(eventItem.Color))
                {
                    g.FillRectangle(colorBrush, colorRect);
                }

                // Title
                var titleRect = new Rectangle(itemRect.Left + 12, itemRect.Top + 4, itemRect.Width - 80, 16);
                g.DrawString(eventItem.Title, titleFont, titleBrush, titleRect);

                // Time and duration
                var timeRect = new Rectangle(itemRect.Left + 12, itemRect.Bottom - 16, itemRect.Width - 80, 12);
                string timeText = $"{eventItem.Time} ({eventItem.Duration})";
                g.DrawString(timeText, timeFont, timeBrush, timeRect);

                // Right aligned time
                var rightTimeRect = new Rectangle(itemRect.Right - 70, itemRect.Top + 8, 65, itemHeight - 16);
                g.DrawString(eventItem.Time, timeFont, timeBrush, rightTimeRect,
                           new StringFormat { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Center });
            }
        }

        private List<EventItem> GetEventItems(WidgetContext ctx)
        {
            if (ctx.Events != null && ctx.Events.Count > 0)
            {
                var list = new List<EventItem>();
                foreach (var e in ctx.Events)
                {
                    list.Add(new EventItem
                    {
                        Title = e.Title ?? string.Empty,
                        Time = e.StartTime.ToString("h:mm tt"),
                        Duration = e.EndTime > e.StartTime
                            ? (e.EndTime - e.StartTime).TotalMinutes >= 60
                                ? $"{(int)((e.EndTime - e.StartTime).TotalHours)}h"
                                : $"{(int)((e.EndTime - e.StartTime).TotalMinutes)}m"
                            : string.Empty,
                        Color = e.Color == Color.Empty ? (Theme?.AccentColor ?? Color.SteelBlue) : e.Color
                    });
                }
                return list;
            }

            // Fallback sample
            return new List<EventItem>
            {
                new() { Title = "Team Meeting", Time = "9:00 AM", Duration = "1h", Color = Color.FromArgb(76, 175, 80) },
                new() { Title = "Project Review", Time = "2:00 PM", Duration = "2h", Color = Color.FromArgb(33, 150, 243) },
                new() { Title = "Client Call", Time = "4:30 PM", Duration = "30m", Color = Color.FromArgb(255, 193, 7) },
                new() { Title = "Code Review", Time = "5:00 PM", Duration = "1h", Color = Color.FromArgb(156, 39, 176) }
            };
        }

        private void DrawEventSeparators(Graphics g, WidgetContext ctx)
        {
            using var separatorPen = new Pen(Color.FromArgb(40, Theme?.BorderColor ?? Color.Gray), 1);
            
            int itemHeight = 40;
            int padding = 8;
            int items = Math.Min(_itemRects.Count, Math.Max(0, (ctx.DrawingRect.Height - padding * 2) / (itemHeight + 4)));

            for (int i = 1; i < items; i++)
            {
                int y = ctx.DrawingRect.Top + padding + i * (itemHeight + 4) - 2;
                g.DrawLine(separatorPen,
                          ctx.DrawingRect.Left + padding + 12,
                          y,
                          ctx.DrawingRect.Right - padding,
                          y);
            }
        }

        public override void UpdateHitAreas(BaseControl owner, WidgetContext ctx, Action<string, Rectangle>? notifyAreaHit)
        {
            if (owner == null) return;
            ClearOwnerHitAreas();

            for (int i = 0; i < _itemRects.Count; i++)
            {
                int idx = _itemRects[i].index;
                var rect = _itemRects[i].rect;
                string name = $"EventList_Item_{idx}";
                owner.AddHitArea(name, rect, null, () =>
                {
                    ctx.SelectedEventIndex = idx;
                    notifyAreaHit?.Invoke(name, rect);
                    Owner?.Invalidate();
                });
            }

            if (!_listRectCache.IsEmpty)
            {
                owner.AddHitArea("EventList_List", _listRectCache, null, () =>
                {
                    ctx.EventListClicked = true;
                    notifyAreaHit?.Invoke("EventList_List", _listRectCache);
                    Owner?.Invalidate();
                });
            }
        }
    }
}
