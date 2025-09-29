using System;
using System.Drawing;
using System.Collections.Generic;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Painters.Calendar
{
    /// <summary>
    /// WeekView - Weekly calendar display painter with hit areas and hover accents
    /// </summary>
    internal sealed class WeekViewPainter : WidgetPainterBase
    {
        private readonly List<(Rectangle rect, int day)> _dayHeaderRects = new();
        private readonly List<(Rectangle rect, int day, int evtIndex)> _eventRects = new();
        private Rectangle _gridRectCache;

        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            var baseRect = Owner?.DrawingRect ?? drawingRect;
            ctx.DrawingRect = Rectangle.Inflate(baseRect, -4, -4);
            _gridRectCache = ctx.DrawingRect;
            _dayHeaderRects.Clear();
            _eventRects.Clear();
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
            // Draw week view calendar layout
            DrawWeekHeader(g, ctx);
            DrawWeekGrid(g, ctx);
            DrawWeekEvents(g, ctx);
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx) 
        {
            // Draw current day highlight and grid lines
            DrawCurrentDayHighlight(g, ctx);
            DrawWeekGridLines(g, ctx);

            // Hover accents
            for (int i = 0; i < _dayHeaderRects.Count; i++)
            {
                if (IsAreaHovered($"WeekView_DayHeader_{_dayHeaderRects[i].day}"))
                {
                    using var hover = new SolidBrush(Color.FromArgb(10, Theme?.PrimaryColor ?? Color.Blue));
                    g.FillRectangle(hover, _dayHeaderRects[i].rect);
                }
            }
            for (int i = 0; i < _eventRects.Count; i++)
            {
                var (rect, day, idx) = _eventRects[i];
                if (IsAreaHovered($"WeekView_Event_{day}_{idx}"))
                {
                    using var pen = new Pen(Theme?.AccentColor ?? Color.Gray, 1.5f);
                    g.DrawRectangle(pen, rect);
                }
            }
        }

        private void DrawWeekHeader(Graphics g, WidgetContext ctx)
        {
            // Draw day headers for the week
            var today = DateTime.Today;
            var startOfWeek = today.AddDays(-(int)today.DayOfWeek);
            
            int dayWidth = ctx.DrawingRect.Width / 7;
            int headerHeight = 30;

            using var dayFont = new Font(Owner?.Font?.FontFamily ?? SystemFonts.DefaultFont.FontFamily, 8f, FontStyle.Bold);
            using var dateFont = new Font(Owner?.Font?.FontFamily ?? SystemFonts.DefaultFont.FontFamily, 7f, FontStyle.Regular);
            using var dayBrush = new SolidBrush(Theme?.TextBoxForeColor ?? Theme?.ForeColor ?? Color.Black);
            using var dateBrush = new SolidBrush(Color.FromArgb(120, Theme?.ForeColor ?? Color.Black));

            _dayHeaderRects.Clear();
            for (int i = 0; i < 7; i++)
            {
                var currentDay = startOfWeek.AddDays(i);
                var dayRect = new Rectangle(
                    ctx.DrawingRect.Left + i * dayWidth,
                    ctx.DrawingRect.Top,
                    dayWidth,
                    headerHeight
                );
                _dayHeaderRects.Add((dayRect, i));

                // Highlight today
                if (currentDay.Date == today.Date)
                {
                    using var todayBrush = new SolidBrush(Color.FromArgb(40, Theme?.AccentColor ?? Color.Blue));
                    g.FillRectangle(todayBrush, dayRect);
                }

                // Draw day name
                var dayName = currentDay.ToString("ddd");
                var dayNameRect = new Rectangle(dayRect.X, dayRect.Y + 2, dayRect.Width, 14);
                g.DrawString(dayName, dayFont, dayBrush, dayNameRect,
                           new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });

                // Draw date
                var dateText = currentDay.Day.ToString();
                var dateRect = new Rectangle(dayRect.X, dayRect.Y + 16, dayRect.Width, 12);
                g.DrawString(dateText, dateFont, dateBrush, dateRect,
                           new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
            }
        }

        private void DrawWeekGrid(Graphics g, WidgetContext ctx)
        {
            // Draw time slots on the left side
            int timeSlotWidth = 60;
            int hourHeight = 40;
            int startHour = 8;
            int endHour = 18;

            var timeAreaRect = new Rectangle(
                ctx.DrawingRect.Left,
                ctx.DrawingRect.Top + 30,
                timeSlotWidth,
                ctx.DrawingRect.Height - 30
            );

            using var timeFont = new Font(Owner?.Font?.FontFamily ?? SystemFonts.DefaultFont.FontFamily, 7f, FontStyle.Regular);
            using var timeBrush = new SolidBrush(Color.FromArgb(100, Theme?.ForeColor ?? Color.Black));

            for (int hour = startHour; hour <= endHour; hour++)
            {
                var timeSlotRect = new Rectangle(
                    timeAreaRect.Left,
                    timeAreaRect.Top + (hour - startHour) * hourHeight,
                    timeAreaRect.Width,
                    hourHeight
                );

                var timeText = $"{hour}:00";
                g.DrawString(timeText, timeFont, timeBrush, timeSlotRect,
                           new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Near });
            }
        }

        private void DrawWeekEvents(Graphics g, WidgetContext ctx)
        {
            // Sample events throughout the week (replace with ctx.CustomData if present)
            var events = ctx.CustomData.ContainsKey("WeekEvents") ? (IEnumerable<dynamic>)ctx.CustomData["WeekEvents"] : new[]
            {
                new { Day = 1, StartHour = 9, Duration = 1, Title = "Meeting", Color = Color.FromArgb(76, 175, 80) },
                new { Day = 2, StartHour = 14, Duration = 2, Title = "Project Review", Color = Color.FromArgb(33, 150, 243) },
                new { Day = 3, StartHour = 10, Duration = 1, Title = "Client Call", Color = Color.FromArgb(255, 193, 7) },
                new { Day = 5, StartHour = 16, Duration = 1, Title = "Team Sync", Color = Color.FromArgb(156, 39, 176) }
            };

            int timeSlotWidth = 60;
            int dayWidth = (ctx.DrawingRect.Width - timeSlotWidth) / 7;
            int hourHeight = 40;
            int startHour = 8;

            using var eventFont = new Font(Owner?.Font?.FontFamily ?? SystemFonts.DefaultFont.FontFamily, 7f, FontStyle.Bold);
            using var eventTextBrush = new SolidBrush(Theme?.BackColor ?? Color.White);

            _eventRects.Clear();
            int idx = 0;
            foreach (var evt in events)
            {
                var eventRect = new Rectangle(
                    ctx.DrawingRect.Left + timeSlotWidth + evt.Day * dayWidth + 2,
                    ctx.DrawingRect.Top + 30 + (evt.StartHour - startHour) * hourHeight + 2,
                    dayWidth - 4,
                    evt.Duration * hourHeight - 4
                );
                _eventRects.Add((eventRect, evt.Day, idx));

                // Draw event background
                using var eventBrush = new SolidBrush(evt.Color);
                using var eventPath = CreateRoundedPath(eventRect, 4);
                g.FillPath(eventBrush, eventPath);

                // Hover effect
                if (IsAreaHovered($"WeekView_Event_{evt.Day}_{idx}"))
                {
                    using var hover = new SolidBrush(Color.FromArgb(10, Theme?.PrimaryColor ?? Color.Blue));
                    g.FillPath(hover, eventPath);
                }

                // Draw event title
                if (eventRect.Height > 16)
                {
                    var textRect = Rectangle.Inflate(eventRect, -4, -2);
                    g.DrawString(evt.Title, eventFont, eventTextBrush, textRect);
                }
                idx++;
            }
        }

        private void DrawCurrentDayHighlight(Graphics g, WidgetContext ctx)
        {
            // Highlight current day column
            var today = DateTime.Today;
            var dayOfWeek = (int)today.DayOfWeek;
            
            int timeSlotWidth = 60;
            int dayWidth = (ctx.DrawingRect.Width - timeSlotWidth) / 7;

            var highlightRect = new Rectangle(
                ctx.DrawingRect.Left + timeSlotWidth + dayOfWeek * dayWidth,
                ctx.DrawingRect.Top + 30,
                dayWidth,
                ctx.DrawingRect.Height - 30
            );

            using var highlightBrush = new SolidBrush(Color.FromArgb(10, Theme?.AccentColor ?? Color.Blue));
            g.FillRectangle(highlightBrush, highlightRect);
        }

        private void DrawWeekGridLines(Graphics g, WidgetContext ctx)
        {
            using var gridPen = new Pen(Color.FromArgb(40, Theme?.BorderColor ?? Color.LightGray), 1);
            
            int timeSlotWidth = 60;
            int dayWidth = (ctx.DrawingRect.Width - timeSlotWidth) / 7;
            int hourHeight = 40;
            int startHour = 8;
            int endHour = 18;

            // Vertical lines for days
            for (int i = 0; i <= 7; i++)
            {
                int x = ctx.DrawingRect.Left + timeSlotWidth + i * dayWidth;
                g.DrawLine(gridPen, x, ctx.DrawingRect.Top + 30, x, ctx.DrawingRect.Bottom);
            }

            // Horizontal lines for hours
            for (int hour = startHour; hour <= endHour; hour++)
            {
                int y = ctx.DrawingRect.Top + 30 + (hour - startHour) * hourHeight;
                g.DrawLine(gridPen, ctx.DrawingRect.Left + timeSlotWidth, y, ctx.DrawingRect.Right, y);
            }
        }

        public override void UpdateHitAreas(BaseControl owner, WidgetContext ctx, Action<string, Rectangle>? notifyAreaHit)
        {
            if (owner == null) return;
            ClearOwnerHitAreas();

            // Day headers
            foreach (var (rect, day) in _dayHeaderRects)
            {
                string name = $"WeekView_DayHeader_{day}";
                owner.AddHitArea(name, rect, null, () =>
                {
                    ctx.CustomData["SelectedDayHeader"] = day;
                    notifyAreaHit?.Invoke(name, rect);
                    Owner?.Invalidate();
                });
            }
            // Events
            foreach (var (rect, day, idx) in _eventRects)
            {
                string name = $"WeekView_Event_{day}_{idx}";
                owner.AddHitArea(name, rect, null, () =>
                {
                    ctx.CustomData["SelectedEventDay"] = day;
                    ctx.CustomData["SelectedEventIndex"] = idx;
                    notifyAreaHit?.Invoke(name, rect);
                    Owner?.Invalidate();
                });
            }
            // Grid
            if (!_gridRectCache.IsEmpty)
            {
                owner.AddHitArea("WeekView_Grid", _gridRectCache, null, () =>
                {
                    ctx.CustomData["WeekGridClicked"] = true;
                    notifyAreaHit?.Invoke("WeekView_Grid", _gridRectCache);
                    Owner?.Invalidate();
                });
            }
        }
    }
}