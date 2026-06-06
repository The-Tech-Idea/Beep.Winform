using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Calendar.CellRender;
using TheTechIdea.Beep.Winform.Controls.Calendar.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Calendar.Rendering.ViewPainters
{
    /// <summary>
    /// Per-view painter for <see cref="CalendarViewMode.WorkWeek"/>. 5-day
    /// timed grid (Mon-Fri) with day-of-week headers, 24 hour rows, time
    /// labels down the left, and timed event blocks. Self-contained — all
    /// paint, hit-test, and date-from-location logic lives here per the
    /// "every thing for each view should handled in that view" rule.
    /// </summary>
    public sealed class WorkWeekViewPainter : ICalendarViewPainter
    {
        private const int DayCount = 5;

        public CalendarViewMode ViewMode => CalendarViewMode.WorkWeek;
        public string Key => "workweek";
        public string DisplayLabel => "Work Week";
        public int VisibleDayCount => DayCount;
        public bool IsTimedView => true;
        public bool IsMonthGrid => false;
        public bool RequiresLeftGutter => true;
        public bool HasAllDayStrip => false;
        public bool SupportsEventDrag => true;
        public bool IsHorizontalTimeAxis => false;

        public DateTime NavigatePrevious(DateTime d) => d.AddDays(-7);
        public DateTime NavigateNext(DateTime d) => d.AddDays(7);
        public string GetHeaderText(DateTime d) => $"Work Week of {d.AddDays(-(((int)d.DayOfWeek + 6) % 7)):MMMM dd, yyyy}";
        public DateTime GetVisibleRangeStart(DateTime d)
        {
            int dayOfWeek = (int)d.DayOfWeek;
            int mondayOffset = dayOfWeek == 0 ? 6 : dayOfWeek - 1;
            return d.Date.AddDays(-mondayOffset);
        }
        public DateTime GetVisibleRangeEnd(DateTime d) => GetVisibleRangeStart(d).AddDays(DayCount);

        public void Layout(ViewPaintArgs args) { }

        public void Paint(Graphics g, ViewPaintArgs args)
        {
            if (args.Surface == null) return;
            var surface = args.Surface;
            var grid = args.Rects.CalendarGridRect;
            int timeColumnWidth = surface.TimeColumnWidth;
            int dayHeaderHeight = surface.DayHeaderHeight;
            int currentHour = DateTime.Now.Hour;
            DateTime startOfWorkWeek = GetVisibleRangeStart(surface.CurrentDate);

            CalendarPainterHelpers.FillRoundedRect(g,
                new Rectangle(grid.X, grid.Y, timeColumnWidth, dayHeaderHeight),
                args.Metrics.CornerRadius, args.BackgroundColor);

            for (int day = 0; day < DayCount; day++)
            {
                var dayDate = startOfWorkWeek.AddDays(day);
                var headerRect = CalendarPainterHelpers.GetColumnRect(
                    new Rectangle(grid.X + timeColumnWidth, grid.Y,
                        Math.Max(0, grid.Width - timeColumnWidth), dayHeaderHeight),
                    day, DayCount);
                PaintDayHeader(g, headerRect, dayDate, args);
            }

            for (int day = 0; day < DayCount; day++)
            {
                var dayDate = startOfWorkWeek.AddDays(day).Date;
                var dayColumn = CalendarPainterHelpers.GetColumnRect(surface.TimedArea, day, DayCount);
                var dayEvents = args.EventService?.GetEventsForDate(dayDate) ?? new List<CalendarEvent>();
                foreach (var evt in dayEvents.OrderBy(e => e.StartTime))
                {
                    var eventRect = CalendarPainterHelpers.GetTimedEventRect(
                        dayColumn, evt, dayDate,
                        surface.EventInsetX, surface.EventInsetY, surface.MinEventHitHeight);
                    PaintEventBlock(g, eventRect, evt, dayDate, day, args);
                }
            }

            for (int hour = 0; hour < 24; hour++)
            {
                var rowRect = surface.GetTimeRowRect(hour);
                var timeLabelRect = new Rectangle(grid.X, rowRect.Y, timeColumnWidth, rowRect.Height);
                PaintTimeLabel(g, timeLabelRect, hour, args);
                for (int day = 0; day < DayCount; day++)
                {
                    var dayDate = startOfWorkWeek.AddDays(day).Date;
                    var columnRect = CalendarPainterHelpers.GetColumnRect(
                        new Rectangle(surface.TimedArea.X, rowRect.Y, surface.TimedArea.Width, rowRect.Height),
                        day, DayCount);
                    PaintTimeSlot(g, columnRect, hour,
                        hour == currentHour && dayDate == DateTime.Today,
                        dayDate, day, args);
                }
            }
        }

        public CalendarInteractionHitTestResult HitTest(Point location, ViewPaintArgs args)
        {
            if (args.Surface == null) return EmptyHit(args);
            var surface = args.Surface;
            var grid = args.Rects.CalendarGridRect;
            // W2-Redo-7 GAP 1 - the time-column gutter (X < TimeColumnWidth) is
            // a time-of-day label strip, NOT a date cell. But the day-header
            // band (Y < DayHeaderHeight) IS a date cell (mirrors DayViewPainter
            // W2-Redo-6 GAP 3 fix and Week1/Week2/Week3/Week7). Previously the
            // OR check returned EmptyHit for both, making a click in the
            // WorkWeek day header trigger CreateEvent instead of SelectDate.
            if (location.X < grid.X + surface.TimeColumnWidth) return EmptyHit(args);
            if (location.Y < grid.Y + surface.DayHeaderHeight)
            {
                var headerBand = new Rectangle(grid.X + surface.TimeColumnWidth, grid.Y,
                    Math.Max(0, grid.Width - surface.TimeColumnWidth), surface.DayHeaderHeight);
                int headerCol = CalendarPainterHelpers.GetColumnIndex(headerBand, location.X, DayCount);
                if (headerCol < 0 || headerCol >= DayCount) return EmptyHit(args);
                return new CalendarInteractionHitTestResult
                {
                    TargetKind = CalendarInteractionTargetKind.DateCell,
                    RequestedMode = CalendarInteractionMode.SelectDate,
                    Location = location,
                    Date = GetVisibleRangeStart(surface.CurrentDate).AddDays(headerCol).Date
                };
            }

            int col = CalendarPainterHelpers.GetColumnIndex(surface.TimedArea, location.X, DayCount);
            if (col < 0 || col >= DayCount) return EmptyHit(args);
            var dayDate = GetVisibleRangeStart(surface.CurrentDate).AddDays(col).Date;
            var dayColumn = CalendarPainterHelpers.GetColumnRect(surface.TimedArea, col, DayCount);

            var events = args.EventService?.GetEventsForDate(dayDate) ?? new List<CalendarEvent>();
            foreach (var evt in events.OrderByDescending(e => e.StartTime))
            {
                var eventRect = CalendarPainterHelpers.GetTimedEventRect(
                    dayColumn, evt, dayDate,
                    surface.EventInsetX, surface.EventInsetY, surface.MinEventHitHeight);
                if (eventRect.Contains(location))
                {
                    var edge = CalendarPainterHelpers.ResolveResizeEdge(location, eventRect, 6);
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
                        Bounds = eventRect
                    };
                }
            }

            var dateWithTime = GetDateTimeFromLocation(location, args) ?? dayDate;
            return new CalendarInteractionHitTestResult
            {
                TargetKind = CalendarInteractionTargetKind.DateCell,
                RequestedMode = CalendarInteractionMode.CreateEvent,
                Location = location,
                Date = dateWithTime
            };
        }

        public DateTime? GetDateTimeFromLocation(Point location, ViewPaintArgs args)
        {
            if (args.Surface == null) return null;
            var surface = args.Surface;
            var timedArea = surface.TimedArea;
            if (timedArea.Width <= 0 || timedArea.Height <= 0) return null;

            int minutesPerDay = CalendarSurfaceModel.MinutesPerDay;
            int minuteOffset = Math.Max(0, Math.Min(minutesPerDay - 1,
                (location.Y - timedArea.Top) * minutesPerDay / Math.Max(1, timedArea.Height)));

            int col = CalendarPainterHelpers.GetColumnIndex(timedArea, location.X, DayCount);
            if (col < 0 || col >= DayCount) col = 0;
            return GetVisibleRangeStart(surface.CurrentDate).AddDays(col).AddMinutes(minuteOffset);
        }

        private static void PaintDayHeader(Graphics g, Rectangle rect, DateTime dayDate, ViewPaintArgs args)
        {
            // W8 - delegate to developer's IBeepUIComponent DateCell factory
            // for the day-header cell when one is registered.
            string cellKey = $"date:{dayDate:yyyy-MM-dd}:header";
            var ctx = new CalendarCellContext(
                CalendarCellKind.DateCell, null, dayDate,
                args.Surface?.ViewMode ?? CalendarViewMode.WorkWeek, 0, 0);
            if (CalendarPainterHelpers.TryDrawCellComponent(g, rect, cellKey, ctx, args)) return;

            CalendarPainterHelpers.FillRoundedRect(g, rect, args.Metrics.CornerRadius, args.BackgroundColor);
            string text = dayDate.ToString("ddd d");
            CalendarPainterHelpers.DrawText(g, text,
                args.DaysHeaderFont ?? args.HeaderFont,
                dayDate.Date == DateTime.Today ? args.TodayForeColor : args.ForegroundColor,
                rect, StringAlignment.Center, StringAlignment.Center);
        }

        private static void PaintTimeLabel(Graphics g, Rectangle rect, int hour, ViewPaintArgs args)
        {
            string label = hour == 0 ? "12a" : hour < 12 ? $"{hour}a" : hour == 12 ? "12p" : $"{hour - 12}p";
            CalendarPainterHelpers.DrawText(g, label,
                args.TimeFont ?? args.DayFont, args.ForegroundColor, rect,
                StringAlignment.Center, StringAlignment.Near, centerVertically: false);
        }

        private static void PaintTimeSlot(Graphics g, Rectangle rect, int hour, bool isCurrentHour,
            DateTime dayDate, int dayIndex, ViewPaintArgs args)
        {
            // W8 - delegate to developer's IBeepUIComponent TimeSlot factory
            // for the (date, hour) cell when one is registered.
            string cellKey = $"slot:{dayDate:yyyy-MM-dd}:{hour}";
            var ctx = new CalendarCellContext(
                CalendarCellKind.TimeSlot, null, dayDate,
                args.Surface?.ViewMode ?? CalendarViewMode.WorkWeek, hour, dayIndex);
            if (CalendarPainterHelpers.TryDrawCellComponent(g, rect, cellKey, ctx, args)) return;

            var back = args.BackgroundColor;
            if (isCurrentHour) back = Color.FromArgb(40, args.PrimaryColor.R, args.PrimaryColor.G, args.PrimaryColor.B);
            g.FillRectangle(new SolidBrush(back), rect);
            g.DrawLine(new Pen(args.BorderColor), rect.X, rect.Bottom, rect.Right, rect.Bottom);
        }

        private static void PaintEventBlock(Graphics g, Rectangle rect, CalendarEvent evt, DateTime dayDate, int dayIndex, ViewPaintArgs args)
        {
            if (rect.Width <= 0 || rect.Height <= 0) return;
            string cellKey = $"evt:{evt.Id}";
            var ctx = new CalendarCellContext(
                CalendarCellKind.EventBlock, evt, dayDate,
                args.Surface?.ViewMode ?? CalendarViewMode.WorkWeek, 0, dayIndex);
            if (CalendarPainterHelpers.TryDrawCellComponent(g, rect, cellKey, ctx, args)) return;

            Color fill = args.GetCategoryColor(evt.CategoryId);
            if (args.SelectedEvent?.Id == evt.Id) fill = args.SelectedBackColor;
            if (args.HoveredEventId == evt.Id) fill = args.HoverBackColor;

            CalendarPainterHelpers.FillRoundedRect(g, rect, args.Metrics.EventCornerRadius, fill);
            if (args.Metrics.ShowEventAccentStripe)
            {
                var accent = new Rectangle(rect.X, rect.Y, args.Metrics.EventAccentWidth, rect.Height);
                CalendarPainterHelpers.FillRoundedRect(g, accent, 0, Color.FromArgb(80, 0, 0, 0));
            }
            var textRect = new Rectangle(rect.X + args.Metrics.EventAccentWidth + 4, rect.Y + 2,
                rect.Width - args.Metrics.EventAccentWidth - 6, Math.Max(0, rect.Height - 4));
            var title = (evt.StartTime.ToString("h:mm tt") + " " + evt.Title).Trim();
            CalendarPainterHelpers.DrawText(g, title,
                args.EventFont ?? args.DayFont, args.ForegroundColor, textRect,
                StringAlignment.Near, StringAlignment.Near, centerVertically: false);
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
