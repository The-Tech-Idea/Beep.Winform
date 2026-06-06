using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Calendar.CellRender;
using TheTechIdea.Beep.Winform.Controls.Calendar.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Calendar.Rendering.ViewPainters
{
    /// <summary>
    /// Per-view painter for <see cref="CalendarViewMode.Week4"/>. Matches
    /// the c4 reference image: 6×7 month grid with a right detail panel
    /// showing the selected event (title, date, time, location,
    /// description, person-in-charge, team). Day cells render up to
    /// <c>MaxEventsPerCell</c> event bars.
    /// </summary>
    public sealed class Week4ViewPainter : ICalendarViewPainter
    {
        public CalendarViewMode ViewMode => CalendarViewMode.Week4;
        public string Key => "week4";
        public string DisplayLabel => "Week 4";
        public int VisibleDayCount => 7;
        public bool IsTimedView => false;
        public bool IsMonthGrid => true;
        public bool RequiresLeftGutter => false;
        public bool HasAllDayStrip => false;
        public bool SupportsEventDrag => false;
        public bool IsHorizontalTimeAxis => false;

        public DateTime NavigatePrevious(DateTime d) => d.AddMonths(-1);
        public DateTime NavigateNext(DateTime d) => d.AddMonths(1);
        public string GetHeaderText(DateTime d) => d.ToString("MMMM yyyy");
        public DateTime GetVisibleRangeStart(DateTime d)
        {
            var first = new DateTime(d.Year, d.Month, 1);
            return first.AddDays(-(int)first.DayOfWeek);
        }
        public DateTime GetVisibleRangeEnd(DateTime d) => GetVisibleRangeStart(d).AddDays(42);

        private const int DetailPanelWidthPx = 320;
        private const int DayHeaderHeightPx = 36;
        private static readonly string[] DayNames = { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };

        public void Layout(ViewPaintArgs args) { }

        public void Paint(Graphics g, ViewPaintArgs args)
        {
            if (args.Surface == null) return;
            var grid = args.Rects.CalendarGridRect;
            int dayHeaderHeight = DayHeaderHeightPx;
            int detailW = Math.Min(DetailPanelWidthPx, Math.Max(0, grid.Width / 4));
            var contentRect = new Rectangle(grid.X, grid.Y,
                Math.Max(0, grid.Width - detailW), grid.Height);
            if (contentRect.Width <= 0 || contentRect.Height <= 0) return;
            int bodyTop = contentRect.Y + dayHeaderHeight;
            int bodyHeight = Math.Max(0, contentRect.Height - dayHeaderHeight);
            int rowH = bodyHeight / 6;
            int firstDayOffset = (int)new DateTime(args.Surface.CurrentDate.Year, args.Surface.CurrentDate.Month, 1).DayOfWeek;

            PaintDetailPanel(g, new Rectangle(grid.Right - detailW, grid.Y, detailW, grid.Height), args);

            var firstDayOfGrid = new DateTime(args.Surface.CurrentDate.Year, args.Surface.CurrentDate.Month, 1)
                .AddDays(-firstDayOffset);

            for (int i = 0; i < 7; i++)
            {
                var headerRect = new Rectangle(contentRect.X + (contentRect.Width * i / 7), contentRect.Y,
                    contentRect.Width / 7, dayHeaderHeight);
                var headerDate = firstDayOfGrid.AddDays(i);
                string cellKey = $"date:{headerDate:yyyy-MM-dd}:cell";
                var hdrCtx = new CalendarCellContext(
                    CalendarCellKind.DateCell, null, headerDate,
                    args.Surface?.ViewMode ?? CalendarViewMode.Week4, 0, i);
                if (CalendarPainterHelpers.TryDrawCellComponent(g, headerRect, cellKey, hdrCtx, args)) continue;

                CalendarPainterHelpers.DrawText(g, DayNames[i], args.DaysHeaderFont ?? args.HeaderFont,
                    args.ForegroundColor, headerRect, StringAlignment.Center, StringAlignment.Center);
            }

            for (int week = 0; week < 6; week++)
            {
                for (int day = 0; day < 7; day++)
                {
                    int cellX = contentRect.X + (contentRect.Width * day / 7);
                    int cellW = contentRect.Width / 7;
                    int cellY = bodyTop + week * rowH;
                    var cellDate = new DateTime(args.Surface.CurrentDate.Year, args.Surface.CurrentDate.Month, 1)
                        .AddDays(week * 7 + day - firstDayOffset);
                    var cellRect = new Rectangle(cellX, cellY, cellW, rowH);

                    // W8 - delegate to developer's IBeepUIComponent DateCell factory
                    // for the month cell when one is registered. Falls through to the
                    // default stroked cell + day-number otherwise.
                    string cellKey = $"date:{cellDate:yyyy-MM-dd}:cell";
                    var ctx = new CalendarCellContext(
                        CalendarCellKind.DateCell, null, cellDate,
                        args.Surface?.ViewMode ?? CalendarViewMode.Week4, week, day);
                    if (CalendarPainterHelpers.TryDrawCellComponent(g, cellRect, cellKey, ctx, args)) continue;

                    CalendarPainterHelpers.StrokeRoundedRect(g, cellRect, 2, args.BorderColor);
                    CalendarPainterHelpers.DrawText(g, cellDate.Day.ToString(), args.DayFont,
                        cellDate.Month == args.Surface.CurrentDate.Month ? args.ForegroundColor : args.OutOfMonthForeColor,
                        new Rectangle(cellX + 4, cellY + 2, cellW - 8, 20),
                        StringAlignment.Far, StringAlignment.Near);
                    var dayEvents = args.EventService?.GetEventsForDate(cellDate) ?? new List<CalendarEvent>();
                    int evY = cellY + 24;
                    int count = Math.Min(dayEvents.Count, 3);
                    for (int i = 0; i < count; i++)
                    {
                        var evt = dayEvents[i];
                        int evH = 20;
                        var evRect = new Rectangle(cellX + 4, evY, cellW - 8, evH);
                        PaintEventCard(g, evRect, evt, cellDate, week, day, args);
                        evY += evH + 2;
                    }
                }
            }
        }

        public CalendarInteractionHitTestResult HitTest(Point location, ViewPaintArgs args)
        {
            if (args.Surface == null) return EmptyHit(args);
            var grid = args.Rects.CalendarGridRect;
            int detailW = Math.Min(DetailPanelWidthPx, Math.Max(0, grid.Width / 4));
            if (location.X >= grid.Right - detailW)
            {
                return new CalendarInteractionHitTestResult
                {
                    TargetKind = CalendarInteractionTargetKind.DateCell,
                    RequestedMode = CalendarInteractionMode.SelectDate,
                    Location = location,
                    Date = args.Surface.CurrentDate.Date
                };
            }
            int dayHeaderHeight = DayHeaderHeightPx;
            if (location.Y < grid.Y + dayHeaderHeight)
            {
                int hdrContentWidth = Math.Max(0, grid.Width - detailW);
                int hdrCol = Math.Max(0, Math.Min(6, (location.X - grid.X) * 7 / Math.Max(1, hdrContentWidth)));
                int hdrFirstDayOffset = (int)new DateTime(args.Surface.CurrentDate.Year, args.Surface.CurrentDate.Month, 1).DayOfWeek;
                var hdrDate = new DateTime(args.Surface.CurrentDate.Year, args.Surface.CurrentDate.Month, 1)
                    .AddDays(hdrCol - hdrFirstDayOffset);
                var hdrRect = new Rectangle(grid.X + (hdrContentWidth * hdrCol / 7), grid.Y,
                    Math.Max(1, hdrContentWidth / 7), dayHeaderHeight);
                return new CalendarInteractionHitTestResult
                {
                    TargetKind = CalendarInteractionTargetKind.DateCell,
                    RequestedMode = CalendarInteractionMode.SelectDate,
                    Location = location,
                    Date = hdrDate,
                    Bounds = hdrRect
                };
            }
            int contentWidth = Math.Max(0, grid.Width - detailW);
            int bodyTop = grid.Y + dayHeaderHeight;
            int bodyHeight = Math.Max(0, grid.Height - dayHeaderHeight);
            int rowH = Math.Max(1, bodyHeight / 6);
            int col = Math.Max(0, Math.Min(6, (location.X - grid.X) * 7 / Math.Max(1, contentWidth)));
            int row = Math.Max(0, Math.Min(5, (location.Y - bodyTop) / rowH));
            int firstDayOffset = (int)new DateTime(args.Surface.CurrentDate.Year, args.Surface.CurrentDate.Month, 1).DayOfWeek;
            var cellDate = new DateTime(args.Surface.CurrentDate.Year, args.Surface.CurrentDate.Month, 1)
                .AddDays(row * 7 + col - firstDayOffset);

            // Hit-test event cards (the painter paints up to 3 cards per
            // cell starting at cellY + 24, each 20px tall with 2px gap).
            // Match Paint's geometry exactly so the click lands on what the
            // user sees. Resize handle is 4px (event cards are 20px tall).
            int cellX = grid.X + (contentWidth * col / 7);
            int cellW = Math.Max(1, contentWidth / 7);
            int evY = bodyTop + row * rowH + 24;
            const int evH = 20;
            const int evGap = 2;
            var dayEvents = args.EventService?.GetEventsForDate(cellDate) ?? new List<CalendarEvent>();
            int evCount = Math.Min(dayEvents.Count, 3);
            for (int i = 0; i < evCount; i++)
            {
                var evRect = new Rectangle(cellX + 4, evY, cellW - 8, evH);
                if (evRect.Contains(location))
                {
                    var edge = CalendarPainterHelpers.ResolveResizeEdge(location, evRect, 4);
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
                        Date = cellDate,
                        Event = dayEvents[i],
                        Bounds = evRect
                    };
                }
                evY += evH + evGap;
            }

            return new CalendarInteractionHitTestResult
            {
                TargetKind = CalendarInteractionTargetKind.DateCell,
                RequestedMode = CalendarInteractionMode.CreateEvent,
                Location = location,
                Date = cellDate
            };
        }

        public DateTime? GetDateTimeFromLocation(Point location, ViewPaintArgs args)
        {
            if (args.Surface == null) return null;
            var grid = args.Rects.CalendarGridRect;
            int detailW = Math.Min(DetailPanelWidthPx, Math.Max(0, grid.Width / 4));
            if (location.X >= grid.Right - detailW) return null;
            int dayHeaderHeight = DayHeaderHeightPx;
            if (location.Y < grid.Y + dayHeaderHeight) return null;
            int contentWidth = Math.Max(0, grid.Width - detailW);
            int bodyTop = grid.Y + dayHeaderHeight;
            int bodyHeight = Math.Max(0, grid.Height - dayHeaderHeight);
            int rowH = Math.Max(1, bodyHeight / 6);
            int col = Math.Max(0, Math.Min(6, (location.X - grid.X) * 7 / Math.Max(1, contentWidth)));
            int row = Math.Max(0, Math.Min(5, (location.Y - bodyTop) / rowH));
            int firstDayOffset = (int)new DateTime(args.Surface.CurrentDate.Year, args.Surface.CurrentDate.Month, 1).DayOfWeek;
            return new DateTime(args.Surface.CurrentDate.Year, args.Surface.CurrentDate.Month, 1)
                .AddDays(row * 7 + col - firstDayOffset);
        }

        private static void PaintDetailPanel(Graphics g, Rectangle rect, ViewPaintArgs args)
        {
            CalendarPainterHelpers.FillRoundedRect(g, rect, args.Metrics.CornerRadius, args.BackgroundColor);
            CalendarPainterHelpers.StrokeRoundedRect(g, rect, args.Metrics.CornerRadius, args.BorderColor);
            int pad = 16;
            int y = rect.Y + pad;
            var evt = args.SelectedEvent;
            if (evt == null)
            {
                CalendarPainterHelpers.DrawText(g, "No event selected", args.DayFont, args.ForegroundColor,
                    new Rectangle(rect.X + pad, y, rect.Width - pad * 2, 24),
                    StringAlignment.Near, StringAlignment.Near);
                return;
            }
            CalendarPainterHelpers.DrawText(g, evt.Title, args.HeaderFont, args.ForegroundColor,
                new Rectangle(rect.X + pad, y, rect.Width - pad * 2, 28), StringAlignment.Near, StringAlignment.Near);
            y += 36;
            CalendarPainterHelpers.DrawText(g,
                $"{evt.StartTime:MMMM d, yyyy}  {evt.StartTime:h:mm tt} - {evt.EndTime:h:mm tt}",
                args.DayFont, args.ForegroundColor,
                new Rectangle(rect.X + pad, y, rect.Width - pad * 2, 20), StringAlignment.Near, StringAlignment.Near);
        }

        private static void PaintEventCard(Graphics g, Rectangle rect, CalendarEvent evt, DateTime cellDate, int row, int col, ViewPaintArgs args)
        {
            if (rect.Width <= 0 || rect.Height <= 0) return;
            string cellKey = $"evt:{evt.Id}";
            var ctx = new CalendarCellContext(CalendarCellKind.EventBlock, evt, cellDate, args.Surface.ViewMode, row, col);
            if (CalendarPainterHelpers.TryDrawCellComponent(g, rect, cellKey, ctx, args)) return;

            CalendarPainterHelpers.FillRoundedRect(g, rect, 4, args.GetCategoryColor(evt.CategoryId));
            if (args.SelectedEvent?.Id == evt.Id)
                CalendarPainterHelpers.StrokeRoundedRect(g, rect, 4, args.PrimaryColor, 2f);
            CalendarPainterHelpers.DrawText(g, evt.Title, args.EventFont, args.ForegroundColor,
                new Rectangle(rect.X + 6, rect.Y, rect.Width - 8, rect.Height),
                StringAlignment.Near, StringAlignment.Center);
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
