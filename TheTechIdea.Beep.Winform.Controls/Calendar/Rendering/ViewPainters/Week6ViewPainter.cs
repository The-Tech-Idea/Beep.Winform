using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Calendar.CellRender;
using TheTechIdea.Beep.Winform.Controls.Calendar.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Calendar.Rendering.ViewPainters
{
    /// <summary>
    /// Per-view painter for <see cref="CalendarViewMode.Week6"/>. Matches
    /// the c6 reference image: 6 day columns (Mon-Sat) with each column
    /// showing events listed in chronological order (time, title). No
    /// toolbar chrome — the column header shows the date number.
    /// </summary>
    public sealed class Week6ViewPainter : ICalendarViewPainter
    {
        public CalendarViewMode ViewMode => CalendarViewMode.Week6;
        public string Key => "week6";
        public string DisplayLabel => "Week 6";
        public int VisibleDayCount => 6;
        public bool IsTimedView => false;
        public bool IsMonthGrid => false;
        public bool RequiresLeftGutter => false;
        public bool HasAllDayStrip => false;
        public bool SupportsEventDrag => false;
        public bool IsHorizontalTimeAxis => false;

        public DateTime NavigatePrevious(DateTime d) => d.AddDays(-7);
        public DateTime NavigateNext(DateTime d) => d.AddDays(7);
        public string GetHeaderText(DateTime d) => d.AddDays(-(int)d.DayOfWeek).ToString("MMMM yyyy");
        public DateTime GetVisibleRangeStart(DateTime d) => d.AddDays(-(int)d.DayOfWeek).Date;
        public DateTime GetVisibleRangeEnd(DateTime d) => GetVisibleRangeStart(d).AddDays(6);

        private const int DayHeaderHeightPx = 48;
        private const int EventRowHeightPx = 64;
        private static readonly string[] DayShortNames = { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };

        public void Layout(ViewPaintArgs args) { }

        public void Paint(Graphics g, ViewPaintArgs args)
        {
            if (args.Surface == null) return;
            var grid = args.Rects.CalendarGridRect;
            int dayHeaderH = DayHeaderHeightPx;
            int contentTop = grid.Y + dayHeaderH;
            int contentH = Math.Max(0, grid.Height - dayHeaderH);
            if (grid.Width <= 0 || contentH <= 0) return;
            DateTime startOfWeek = GetVisibleRangeStart(args.Surface.CurrentDate);

            for (int day = 0; day < 6; day++)
            {
                var dayDate = startOfWeek.AddDays(day);
                var headerRect = new Rectangle(grid.X + (grid.Width * day / 6), grid.Y,
                    grid.Width / 6, dayHeaderH);

                // W8 - delegate to developer's IBeepUIComponent DateCell factory for
                // the week6 day header (day name + day number) when one is registered.
                // Falls through to the default header text + today-pill otherwise.
                string cellKey = $"date:{dayDate:yyyy-MM-dd}:week6-header";
                var ctx = new CalendarCellContext(
                    CalendarCellKind.DateCell, null, dayDate,
                    args.Surface?.ViewMode ?? CalendarViewMode.Week6, 0, day);
                if (CalendarPainterHelpers.TryDrawCellComponent(g, headerRect, cellKey, ctx, args)) continue;

                CalendarPainterHelpers.DrawText(g, DayShortNames[day].ToUpper(), args.DayFont,
                    args.ForegroundColor,
                    new Rectangle(headerRect.X, headerRect.Y, headerRect.Width, 18),
                    StringAlignment.Center, StringAlignment.Near);
                var color = dayDate.Date == DateTime.Today ? args.TodayForeColor : args.ForegroundColor;
                if (dayDate.Date == DateTime.Today)
                    CalendarPainterHelpers.FillRoundedRect(g,
                        new Rectangle(headerRect.X + headerRect.Width / 2 - 16, headerRect.Y + 18, 32, 24),
                        4, args.PrimaryColor);
                CalendarPainterHelpers.DrawText(g, dayDate.Day.ToString(), args.HeaderFont,
                    dayDate.Date == DateTime.Today ? Color.White : color,
                    new Rectangle(headerRect.X, headerRect.Y + 18, headerRect.Width, 24),
                    StringAlignment.Center, StringAlignment.Center);

                var dayColumn = new Rectangle(grid.X + (grid.Width * day / 6) + 4, contentTop,
                    grid.Width / 6 - 8, contentH);
                var dayEvents = (args.EventService?.GetEventsForDate(dayDate) ?? new List<CalendarEvent>())
                    .OrderBy(e => e.StartTime).ToList();
                int y = dayColumn.Y + 4;
                int cardH = Math.Min(EventRowHeightPx, Math.Max(40, dayColumn.Height / Math.Max(1, dayEvents.Count) - 4));
                int maxCards = Math.Max(1, dayColumn.Height / (cardH + 6));
                for (int i = 0; i < Math.Min(dayEvents.Count, maxCards); i++)
                {
                    var evt = dayEvents[i];
                    var r = new Rectangle(dayColumn.X, y, dayColumn.Width, cardH);
                    PaintEventCard(g, r, evt, dayDate, day, i, args);
                    y += cardH + 6;
                }
            }
        }

        public CalendarInteractionHitTestResult HitTest(Point location, ViewPaintArgs args)
        {
            if (args.Surface == null) return EmptyHit(args);
            var grid = args.Rects.CalendarGridRect;
            if (grid.Width <= 0) return EmptyHit(args);
            int col = Math.Max(0, Math.Min(5, (location.X - grid.X) * 6 / grid.Width));
            DateTime dayDate = GetVisibleRangeStart(args.Surface.CurrentDate).AddDays(col);
            int dayHeaderH = DayHeaderHeightPx;
            int contentTop = grid.Y + dayHeaderH;
            if (location.Y < contentTop)
            {
                return new CalendarInteractionHitTestResult
                {
                    TargetKind = CalendarInteractionTargetKind.DateCell,
                    RequestedMode = CalendarInteractionMode.SelectDate,
                    Location = location,
                    Date = dayDate
                };
            }
            int contentH = Math.Max(0, grid.Height - dayHeaderH);
            var dayColumn = new Rectangle(grid.X + (grid.Width * col / 6) + 4, contentTop,
                grid.Width / 6 - 8, contentH);
            var dayEvents = (args.EventService?.GetEventsForDate(dayDate) ?? new List<CalendarEvent>())
                .OrderBy(e => e.StartTime).ToList();
            int cardH = Math.Min(EventRowHeightPx, Math.Max(40, dayColumn.Height / Math.Max(1, dayEvents.Count) - 4));
            int row = Math.Max(0, (location.Y - dayColumn.Y - 4) / Math.Max(1, cardH + 6));
            // W2-Redo-10 GAP 3 - Paint limits the visible cards to
            // count = Math.Min(dayEvents.Count, maxCards) where
            // maxCards = Math.Max(1, dayColumn.Height / (cardH + 6))
            // (see Paint above). Without the matching maxCards check in
            // HitTest, a click on a Y position where Paint drew no card
            // (because the day has more events than fit) would still
            // resolve to dayEvents[row] and select an event that isn't
            // visible on screen. Add the same maxCards guard.
            int maxCards = Math.Max(0, dayColumn.Height / (cardH + 6));
            if (row < dayEvents.Count && row < maxCards)
            {
                var evt = dayEvents[row];
                var r = new Rectangle(dayColumn.X, dayColumn.Y + 4 + row * (cardH + 6), dayColumn.Width, cardH);
                if (r.Contains(location))
                {
                    var edge = CalendarPainterHelpers.ResolveResizeEdge(location, r, 6);
                    return new CalendarInteractionHitTestResult
                    {
                        TargetKind = CalendarInteractionTargetKind.EventBlock,
                        RequestedMode = edge == CalendarEventResizeEdge.Start
                            ? CalendarInteractionMode.ResizeStart
                            : edge == CalendarEventResizeEdge.End
                                ? CalendarInteractionMode.ResizeEnd
                                : CalendarInteractionMode.SelectEvent,
                        ResizeEdge = edge,
                        Location = location,
                        Date = dayDate,
                        Event = evt,
                        Bounds = r
                    };
                }
            }
            return new CalendarInteractionHitTestResult
            {
                TargetKind = CalendarInteractionTargetKind.DateCell,
                RequestedMode = CalendarInteractionMode.CreateEvent,
                Location = location,
                Date = dayDate
            };
        }

        public DateTime? GetDateTimeFromLocation(Point location, ViewPaintArgs args)
        {
            if (args.Surface == null || args.Rects.CalendarGridRect.Width <= 0) return null;
            int col = Math.Max(0, Math.Min(5, (location.X - args.Rects.CalendarGridRect.X) * 6 / args.Rects.CalendarGridRect.Width));
            return GetVisibleRangeStart(args.Surface.CurrentDate).AddDays(col);
        }

        private static void PaintEventCard(Graphics g, Rectangle rect, CalendarEvent evt, DateTime dayDate, int dayIndex, int cardIndex, ViewPaintArgs args)
        {
            if (rect.Width <= 0 || rect.Height <= 0) return;
            string cellKey = $"evt:{evt.Id}";
            var ctx = new CalendarCellContext(CalendarCellKind.EventBlock, evt, dayDate, args.Surface.ViewMode, cardIndex, dayIndex);
            if (CalendarPainterHelpers.TryDrawCellComponent(g, rect, cellKey, ctx, args)) return;

            CalendarPainterHelpers.FillRoundedRect(g, rect, 6, args.GetCategoryColor(evt.CategoryId));
            if (args.SelectedEvent?.Id == evt.Id)
                CalendarPainterHelpers.StrokeRoundedRect(g, rect, 6, args.PrimaryColor, 2f);
            CalendarPainterHelpers.DrawText(g, evt.Title, args.EventFont, args.ForegroundColor,
                new Rectangle(rect.X + 8, rect.Y + 6, rect.Width - 12, 18),
                StringAlignment.Near, StringAlignment.Near);
            CalendarPainterHelpers.DrawText(g, evt.StartTime.ToString("H:mm"), args.DayFont, args.ForegroundColor,
                new Rectangle(rect.X + 8, rect.Y + 26, rect.Width - 12, 16),
                StringAlignment.Near, StringAlignment.Near);
        }

        private static CalendarInteractionHitTestResult EmptyHit(ViewPaintArgs args) =>
            new CalendarInteractionHitTestResult
            {
                TargetKind = CalendarInteractionTargetKind.EmptySurface,
                RequestedMode = CalendarInteractionMode.CreateEvent,
                Location = Point.Empty,
                Date = args.Surface?.CurrentDate.Date ?? DateTime.Today
            };
    }
}
