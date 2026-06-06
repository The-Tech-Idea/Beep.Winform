using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Calendar.CellRender;
using TheTechIdea.Beep.Winform.Controls.Calendar.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Calendar.Rendering.ViewPainters
{
    /// <summary>
    /// Per-view painter for <see cref="CalendarViewMode.Week5"/>. Matches
    /// the c5 reference image: 7 day columns (Mon-Sun) with each column
    /// showing event cards stacked (no time grid). Day-of-week tabs at
    /// the top let the user switch the focused day.
    /// </summary>
    public sealed class Week5ViewPainter : ICalendarViewPainter
    {
        public CalendarViewMode ViewMode => CalendarViewMode.Week5;
        public string Key => "week5";
        public string DisplayLabel => "Week 5";
        public int VisibleDayCount => 7;
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
        public DateTime GetVisibleRangeEnd(DateTime d) => GetVisibleRangeStart(d).AddDays(7);

        private const int DayTabsHeightPx = 48;
        private const int DateRowHeightPx = 28;
        private static readonly string[] DayShortNames = { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun" };

        public void Layout(ViewPaintArgs args) { }

        public void Paint(Graphics g, ViewPaintArgs args)
        {
            if (args.Surface == null) return;
            var grid = args.Rects.CalendarGridRect;
            int dayTabsH = DayTabsHeightPx;
            int dateRowH = DateRowHeightPx;
            int contentTop = grid.Y + dayTabsH + dateRowH;
            int contentH = Math.Max(0, grid.Height - dayTabsH - dateRowH);
            if (grid.Width <= 0 || contentH <= 0) return;
            DateTime startOfWeek = GetVisibleRangeStart(args.Surface.CurrentDate);

            for (int day = 0; day < 7; day++)
            {
                var dayDate = startOfWeek.AddDays(day);
                var tabRect = new Rectangle(grid.X + (grid.Width * day / 7), grid.Y,
                    grid.Width / 7, dayTabsH);
                var dateRect = new Rectangle(grid.X + (grid.Width * day / 7), grid.Y + dayTabsH,
                    grid.Width / 7, dateRowH);

                // W8 - delegate to developer's IBeepUIComponent DateCell factory for
                // the week5 day header (tab + date row) when one is registered. The
                // entire header band (tabs + date row) is delegated as one date cell
                // so the developer can render a single cohesive header.
                var headerBand = new Rectangle(tabRect.X, tabRect.Y,
                    tabRect.Width, tabRect.Height + dateRect.Height);
                string cellKey = $"date:{dayDate:yyyy-MM-dd}:week5-header";
                var ctx = new CalendarCellContext(
                    CalendarCellKind.DateCell, null, dayDate,
                    args.Surface?.ViewMode ?? CalendarViewMode.Week5, 0, day);
                if (CalendarPainterHelpers.TryDrawCellComponent(g, headerBand, cellKey, ctx, args)) continue;

                bool isActive = dayDate.Date == args.Surface.CurrentDate.Date;
                if (isActive)
                    CalendarPainterHelpers.FillRoundedRect(g, new Rectangle(tabRect.X + 4, tabRect.Y + 4, tabRect.Width - 8, tabRect.Height - 8),
                        6, args.PrimaryColor);
                CalendarPainterHelpers.DrawText(g, DayShortNames[day], args.DayFont,
                    isActive ? Color.White : args.ForegroundColor,
                    tabRect, StringAlignment.Center, StringAlignment.Center);

                CalendarPainterHelpers.DrawText(g, dayDate.Day.ToString(), args.HeaderFont,
                    dayDate.Date == DateTime.Today ? args.TodayForeColor : args.ForegroundColor,
                    dateRect, StringAlignment.Center, StringAlignment.Center);
            }

            for (int day = 0; day < 7; day++)
            {
                var dayDate = startOfWeek.AddDays(day);
                var dayColumn = new Rectangle(grid.X + (grid.Width * day / 7) + 2, contentTop,
                    grid.Width / 7 - 4, contentH);
                var dayEvents = (args.EventService?.GetEventsForDate(dayDate) ?? new List<CalendarEvent>())
                    .OrderBy(e => e.StartTime).ToList();
                int cardH = 56;
                int y = dayColumn.Y + 4;
                int count = Math.Min(dayEvents.Count, dayColumn.Height / (cardH + 4));
                for (int i = 0; i < count; i++)
                {
                    var evt = dayEvents[i];
                    var r = new Rectangle(dayColumn.X, y, dayColumn.Width, cardH);
                    PaintEventCard(g, r, evt, dayDate, day, i, args);
                    y += cardH + 4;
                }
            }
        }

        public CalendarInteractionHitTestResult HitTest(Point location, ViewPaintArgs args)
        {
            if (args.Surface == null) return EmptyHit(args);
            var grid = args.Rects.CalendarGridRect;
            if (grid.Width <= 0) return EmptyHit(args);
            int dayTabsH = DayTabsHeightPx;
            int dateRowH = DateRowHeightPx;
            int contentTop = grid.Y + dayTabsH + dateRowH;
            int contentH = Math.Max(0, grid.Height - dayTabsH - dateRowH);
            int col = Math.Max(0, Math.Min(6, (location.X - grid.X) * 7 / grid.Width));
            DateTime dayDate = GetVisibleRangeStart(args.Surface.CurrentDate).AddDays(col);
            if (location.Y < grid.Y + dayTabsH)
            {
                return new CalendarInteractionHitTestResult
                {
                    TargetKind = CalendarInteractionTargetKind.DateCell,
                    RequestedMode = CalendarInteractionMode.SelectDate,
                    Location = location,
                    Date = dayDate
                };
            }
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
            int cardH = 56;
            var dayColumn = new Rectangle(grid.X + (grid.Width * col / 7) + 2, contentTop,
                grid.Width / 7 - 4, contentH);
            int row = Math.Max(0, (location.Y - dayColumn.Y) / (cardH + 4));
            var dayEvents = (args.EventService?.GetEventsForDate(dayDate) ?? new List<CalendarEvent>())
                .OrderBy(e => e.StartTime).ToList();
            // W2-Redo-10 GAP 2 - Paint limits the visible cards to
            // Math.Min(dayEvents.Count, dayColumn.Height / (cardH + 4))
            // (see Paint above). Without the matching maxRows check in
            // HitTest, a click on a Y position where Paint drew no card
            // (because the day has more events than fit) would still
            // resolve to an event in dayEvents[row] and the user would
            // see a "selected event" highlight for a card that isn't
            // visible. Add the same maxRows guard.
            int maxRows = Math.Max(0, dayColumn.Height / (cardH + 4));
            if (row < dayEvents.Count && row < maxRows)
            {
                var evt = dayEvents[row];
                var r = new Rectangle(dayColumn.X, dayColumn.Y + row * (cardH + 4) + 4, dayColumn.Width, cardH);
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
            int col = Math.Max(0, Math.Min(6, (location.X - args.Rects.CalendarGridRect.X) * 7 / args.Rects.CalendarGridRect.Width));
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
                new Rectangle(rect.X + 8, rect.Y + 6, rect.Width - 12, 18), StringAlignment.Near, StringAlignment.Near);
            CalendarPainterHelpers.DrawText(g, $"{evt.StartTime:h:mm tt} - {evt.EndTime:h:mm tt}",
                args.DayFont, args.ForegroundColor,
                new Rectangle(rect.X + 8, rect.Y + 26, rect.Width - 12, 16), StringAlignment.Near, StringAlignment.Near);
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
