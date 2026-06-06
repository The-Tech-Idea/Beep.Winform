using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Calendar.CellRender;
using TheTechIdea.Beep.Winform.Controls.Calendar.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Calendar.Rendering.ViewPainters
{
    /// <summary>
    /// Per-view painter for <see cref="CalendarViewMode.Week7"/>. Matches
    /// the c7 reference image: 7-day timed grid (Mon-Sun) with an
    /// appointment filter bar at the top and per-event status badges
    /// (Completed / Cancelled / Scheduled / In Progress / etc.).
    /// </summary>
    public sealed class Week7ViewPainter : ICalendarViewPainter
    {
        public CalendarViewMode ViewMode => CalendarViewMode.Week7;
        public string Key => "week7";
        public string DisplayLabel => "Week 7";
        public int VisibleDayCount => 7;
        public bool IsTimedView => true;
        public bool IsMonthGrid => false;
        public bool RequiresLeftGutter => true;
        public bool HasAllDayStrip => false;
        public bool SupportsEventDrag => true;
        public bool IsHorizontalTimeAxis => false;

        public DateTime NavigatePrevious(DateTime d) => d.AddDays(-7);
        public DateTime NavigateNext(DateTime d) => d.AddDays(7);
        public string GetHeaderText(DateTime d) => $"Week of {d.AddDays(-(int)d.DayOfWeek):MMMM dd, yyyy}";
        public DateTime GetVisibleRangeStart(DateTime d) => d.AddDays(-(int)d.DayOfWeek).Date;
        public DateTime GetVisibleRangeEnd(DateTime d) => GetVisibleRangeStart(d).AddDays(7);

        private const int FilterBarHeightPx = 44;
        private const int DayHeaderHeightPx = 36;
        private const int TimeColumnWidthPx = 60;
        private const int StatusBadgeHeightPx = 18;

        public void Layout(ViewPaintArgs args) { }

        public void Paint(Graphics g, ViewPaintArgs args)
        {
            if (args.Surface == null) return;
            var grid = args.Rects.CalendarGridRect;
            int dayHeaderHeight = DayHeaderHeightPx;
            int timeColumnWidth = TimeColumnWidthPx;
            int filterH = FilterBarHeightPx;
            var contentRect = new Rectangle(grid.X, grid.Y + filterH,
                grid.Width, Math.Max(0, grid.Height - filterH));
            if (contentRect.Width <= 0 || contentRect.Height <= 0) return;
            var dayHeaderBand = new Rectangle(contentRect.X + timeColumnWidth, contentRect.Y,
                Math.Max(0, contentRect.Width - timeColumnWidth), dayHeaderHeight);
            var timedArea = new Rectangle(contentRect.X + timeColumnWidth, contentRect.Y + dayHeaderHeight,
                Math.Max(0, contentRect.Width - timeColumnWidth),
                Math.Max(0, contentRect.Height - dayHeaderHeight));
            int currentHour = DateTime.Now.Hour;
            DateTime startOfWeek = GetVisibleRangeStart(args.Surface.CurrentDate);

            PaintFilterBar(g, new Rectangle(grid.X, grid.Y, grid.Width, filterH), args);

            for (int day = 0; day < 7; day++)
            {
                var dayDate = startOfWeek.AddDays(day);
                var headerRect = CalendarPainterHelpers.GetColumnRect(dayHeaderBand, day, 7);
                // W8 - delegate to developer's IBeepUIComponent DateCell factory
                // for the day-header cell when one is registered.
                string headerKey = $"date:{dayDate:yyyy-MM-dd}:header";
                var headerCtx = new CalendarCellContext(
                    CalendarCellKind.DateCell, null, dayDate,
                    args.Surface?.ViewMode ?? CalendarViewMode.Week7, 0, day);
                if (!CalendarPainterHelpers.TryDrawCellComponent(g, headerRect, headerKey, headerCtx, args))
                {
                    CalendarPainterHelpers.FillRoundedRect(g, headerRect, args.Metrics.CornerRadius, args.BackgroundColor);
                    CalendarPainterHelpers.DrawText(g, dayDate.ToString("ddd d"), args.DaysHeaderFont ?? args.HeaderFont,
                        dayDate.Date == DateTime.Today ? args.TodayForeColor : args.ForegroundColor,
                        headerRect, StringAlignment.Center, StringAlignment.Center);
                }
            }

            for (int day = 0; day < 7; day++)
            {
                var dayDate = startOfWeek.AddDays(day).Date;
                var dayColumn = CalendarPainterHelpers.GetColumnRect(timedArea, day, 7);
                var dayEvents = args.EventService?.GetEventsForDate(dayDate) ?? new List<CalendarEvent>();
                for (int i = 0; i < dayEvents.Count; i++)
                {
                    var evt = dayEvents[i];
                    var eventRect = CalendarPainterHelpers.GetTimedEventRect(dayColumn, evt, dayDate, 4, 2, 36);
                    PaintEventBlock(g, eventRect, evt, dayDate, day, args);
                }
            }

            for (int hour = 0; hour < 24; hour++)
            {
                var rowRect = CalendarPainterHelpers.GetRowRect(timedArea, hour, 24);
                var timeLabelRect = new Rectangle(contentRect.X, rowRect.Y, timeColumnWidth, rowRect.Height);
                CalendarPainterHelpers.DrawText(g,
                    hour == 0 ? "12 AM" : hour < 12 ? $"{hour} AM" : hour == 12 ? "12 PM" : $"{hour - 12} PM",
                    args.TimeFont ?? args.DayFont, args.ForegroundColor,
                    timeLabelRect, StringAlignment.Center, StringAlignment.Near, centerVertically: false);
                for (int day = 0; day < 7; day++)
                {
                    var dayDate = startOfWeek.AddDays(day).Date;
                    var columnRect = CalendarPainterHelpers.GetColumnRect(
                        new Rectangle(timedArea.X, rowRect.Y, timedArea.Width, rowRect.Height), day, 7);
                    string slotKey = $"slot:{dayDate:yyyy-MM-dd}:{hour}";
                    var slotCtx = new CalendarCellContext(
                        CalendarCellKind.TimeSlot, null, dayDate,
                        args.Surface?.ViewMode ?? CalendarViewMode.Week7, hour, day);
                    if (CalendarPainterHelpers.TryDrawCellComponent(g, columnRect, slotKey, slotCtx, args)) continue;

                    var back = args.BackgroundColor;
                    if (hour == currentHour && dayDate == DateTime.Today)
                        back = Color.FromArgb(40, args.PrimaryColor.R, args.PrimaryColor.G, args.PrimaryColor.B);
                    g.FillRectangle(new SolidBrush(back), columnRect);
                    g.DrawLine(new Pen(args.BorderColor), columnRect.X, columnRect.Bottom, columnRect.Right, columnRect.Bottom);
                }
            }
        }

        public CalendarInteractionHitTestResult HitTest(Point location, ViewPaintArgs args)
        {
            if (args.Surface == null) return EmptyHit(args);
            var grid = args.Rects.CalendarGridRect;
            if (location.Y < grid.Y + FilterBarHeightPx)
            {
                return new CalendarInteractionHitTestResult
                {
                    TargetKind = CalendarInteractionTargetKind.DateCell,
                    RequestedMode = CalendarInteractionMode.SelectDate,
                    Location = location,
                    Date = args.Surface.CurrentDate.Date
                };
            }
            int timeColumnWidth = TimeColumnWidthPx;
            int dayHeaderHeight = DayHeaderHeightPx;
            var timedArea = new Rectangle(grid.X + timeColumnWidth, grid.Y + FilterBarHeightPx + dayHeaderHeight,
                Math.Max(0, grid.Width - timeColumnWidth),
                Math.Max(0, grid.Height - FilterBarHeightPx - dayHeaderHeight));
            if (timedArea.Width <= 0 || timedArea.Height <= 0) return EmptyHit(args);
            int col = CalendarPainterHelpers.GetColumnIndex(timedArea, location.X, 7);
            if (col < 0 || col >= 7) return EmptyHit(args);
            DateTime dayDate = GetVisibleRangeStart(args.Surface.CurrentDate).AddDays(col).Date;
            var dayColumn = CalendarPainterHelpers.GetColumnRect(timedArea, col, 7);
            var dayEvents = args.EventService?.GetEventsForDate(dayDate) ?? new List<CalendarEvent>();
            for (int i = dayEvents.Count - 1; i >= 0; i--)
            {
                var evt = dayEvents[i];
                var eventRect = CalendarPainterHelpers.GetTimedEventRect(dayColumn, evt, dayDate, 4, 2, 36);
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
            var grid = args.Rects.CalendarGridRect;
            if (location.Y < grid.Y + FilterBarHeightPx) return null;
            int timeColumnWidth = TimeColumnWidthPx;
            int dayHeaderHeight = DayHeaderHeightPx;
            var timedArea = new Rectangle(grid.X + timeColumnWidth, grid.Y + FilterBarHeightPx + dayHeaderHeight,
                Math.Max(0, grid.Width - timeColumnWidth),
                Math.Max(0, grid.Height - FilterBarHeightPx - dayHeaderHeight));
            if (timedArea.Width <= 0 || timedArea.Height <= 0) return null;
            int minutesPerDay = 24 * 60;
            int minuteOffset = Math.Max(0, Math.Min(minutesPerDay - 1,
                (location.Y - timedArea.Top) * minutesPerDay / Math.Max(1, timedArea.Height)));
            int col = CalendarPainterHelpers.GetColumnIndex(timedArea, location.X, 7);
            if (col < 0 || col >= 7) col = 0;
            return GetVisibleRangeStart(args.Surface.CurrentDate).AddDays(col).Date.AddMinutes(minuteOffset);
        }

        private static void PaintFilterBar(Graphics g, Rectangle rect, ViewPaintArgs args)
        {
            CalendarPainterHelpers.FillRoundedRect(g, rect, args.Metrics.CornerRadius, args.BackgroundColor);
            CalendarPainterHelpers.StrokeRoundedRect(g, rect, args.Metrics.CornerRadius, args.BorderColor);
            int pad = 12;
            int x = rect.X + pad;
            string[] filters = { "Show all", "Completed", "Patient is waiting", "Cancelled", "Scheduled" };
            for (int i = 0; i < filters.Length; i++)
            {
                var f = filters[i];
                int w = Math.Max(80, (int)g.MeasureString(f, args.DayFont).Width + 24);
                var r = new Rectangle(x, rect.Y + (rect.Height - 26) / 2, w, 26);
                CalendarPainterHelpers.FillRoundedRect(g, r, 6,
                    i == 0 ? args.PrimaryColor : args.SelectedBackColor);
                CalendarPainterHelpers.DrawText(g, f, args.DayFont,
                    i == 0 ? Color.White : args.ForegroundColor, r, StringAlignment.Center, StringAlignment.Center);
                x += w + 6;
            }
        }

        private static void PaintEventBlock(Graphics g, Rectangle rect, CalendarEvent evt, DateTime dayDate, int dayIndex, ViewPaintArgs args)
        {
            if (rect.Width <= 0 || rect.Height <= 0) return;
            string cellKey = $"evt:{evt.Id}";
            var ctx = new CalendarCellContext(CalendarCellKind.EventBlock, evt, dayDate, args.Surface.ViewMode, 0, dayIndex);
            if (CalendarPainterHelpers.TryDrawCellComponent(g, rect, cellKey, ctx, args)) return;

            CalendarPainterHelpers.FillRoundedRect(g, rect, args.Metrics.EventCornerRadius,
                args.GetCategoryColor(evt.CategoryId));
            if (args.SelectedEvent?.Id == evt.Id)
                CalendarPainterHelpers.StrokeRoundedRect(g, rect, args.Metrics.EventCornerRadius, args.PrimaryColor, 2f);

            var title = (evt.StartTime.ToString("h:mm tt") + " " + evt.Title).Trim();
            var textRect = new Rectangle(rect.X + 6, rect.Y + 4,
                rect.Width - 12, Math.Max(0, rect.Height - StatusBadgeHeightPx - 8));
            CalendarPainterHelpers.DrawText(g, title, args.EventFont ?? args.DayFont, args.ForegroundColor,
                textRect, StringAlignment.Near, StringAlignment.Near, centerVertically: false);

            // Status badge
            var badgeRect = new Rectangle(rect.X + 6, rect.Bottom - StatusBadgeHeightPx - 4,
                Math.Max(60, rect.Width - 12), StatusBadgeHeightPx);
            string status = ResolveStatus(evt);
            Color badgeBack = ResolveStatusColor(status, args);
            CalendarPainterHelpers.FillRoundedRect(g, badgeRect, 4, badgeBack);
            CalendarPainterHelpers.DrawText(g, status, args.DayFont, Color.White, badgeRect,
                StringAlignment.Center, StringAlignment.Center);
        }

        private static string ResolveStatus(CalendarEvent evt)
        {
            if (evt == null) return "Scheduled";
            if (evt.Status == CalendarEventStatus.Cancelled) return "Cancelled";
            if (evt.Status == CalendarEventStatus.Confirmed && evt.IsAllDay) return "Scheduled";
            if (evt.EndTime < DateTime.Now) return "Completed";
            if (evt.StartTime <= DateTime.Now && evt.EndTime >= DateTime.Now) return "In Progress";
            return "Scheduled";
        }

        private static Color ResolveStatusColor(string status, ViewPaintArgs args)
        {
            return status switch
            {
                "Completed" => Color.FromArgb(76, 175, 80),
                "Cancelled" => Color.FromArgb(244, 67, 54),
                "In Progress" => Color.FromArgb(255, 152, 0),
                "Scheduled" => Color.FromArgb(33, 150, 243),
                _ => args.PrimaryColor
            };
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
