using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Calendar.CellRender;
using TheTechIdea.Beep.Winform.Controls.Calendar.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Calendar.Rendering.ViewPainters
{
    /// <summary>
    /// Per-view painter for <see cref="CalendarViewMode.Week2"/>. Matches
    /// the c2 reference image: 7-day timed grid (Sun-Sat) with a right
    /// detail panel showing the currently selected event (title, date,
    /// time, location, attendees, link, etc.). All-day strip above the
    /// timed grid.
    /// </summary>
    public sealed class Week2ViewPainter : ICalendarViewPainter
    {
        public CalendarViewMode ViewMode => CalendarViewMode.Week2;
        public string Key => "week2";
        public string DisplayLabel => "Week 2";
        public int VisibleDayCount => 7;
        public bool IsTimedView => true;
        public bool IsMonthGrid => false;
        public bool RequiresLeftGutter => true;
        public bool HasAllDayStrip => true;
        public bool SupportsEventDrag => true;
        public bool IsHorizontalTimeAxis => false;

        public DateTime NavigatePrevious(DateTime d) => d.AddDays(-7);
        public DateTime NavigateNext(DateTime d) => d.AddDays(7);
        public string GetHeaderText(DateTime d) => $"Week of {d.AddDays(-(int)d.DayOfWeek):MMMM dd, yyyy}";
        public DateTime GetVisibleRangeStart(DateTime d) => d.AddDays(-(int)d.DayOfWeek).Date;
        public DateTime GetVisibleRangeEnd(DateTime d) => GetVisibleRangeStart(d).AddDays(7);

        private const int DetailPanelWidthPx = 320;
        private const int AllDayStripHeightPx = 24;
        private const int DayHeaderHeightPx = 36;
        private const int TimeColumnWidthPx = 60;

        public void Layout(ViewPaintArgs args) { }

        public void Paint(Graphics g, ViewPaintArgs args)
        {
            if (args.Surface == null) return;
            var grid = args.Rects.CalendarGridRect;
            int dayHeaderHeight = DayHeaderHeightPx;
            int timeColumnWidth = TimeColumnWidthPx;
            int detailW = Math.Min(DetailPanelWidthPx, Math.Max(0, grid.Width / 4));
            var contentRect = new Rectangle(grid.X, grid.Y,
                Math.Max(0, grid.Width - detailW), grid.Height);
            if (contentRect.Width <= 0 || contentRect.Height <= 0) return;
            var dayHeaderBand = new Rectangle(contentRect.X + timeColumnWidth, contentRect.Y,
                Math.Max(0, contentRect.Width - timeColumnWidth), dayHeaderHeight);
            var allDayStrip = new Rectangle(contentRect.X + timeColumnWidth, contentRect.Y + dayHeaderHeight,
                Math.Max(0, contentRect.Width - timeColumnWidth), AllDayStripHeightPx);
            var timedArea = new Rectangle(contentRect.X + timeColumnWidth, contentRect.Y + dayHeaderHeight + AllDayStripHeightPx,
                Math.Max(0, contentRect.Width - timeColumnWidth),
                Math.Max(0, contentRect.Height - dayHeaderHeight - AllDayStripHeightPx));
            int currentHour = DateTime.Now.Hour;
            DateTime startOfWeek = GetVisibleRangeStart(args.Surface.CurrentDate);

            PaintDetailPanel(g, new Rectangle(grid.Right - detailW, grid.Y, detailW, grid.Height), args);

            for (int day = 0; day < 7; day++)
            {
                var dayDate = startOfWeek.AddDays(day);
                var headerRect = CalendarPainterHelpers.GetColumnRect(dayHeaderBand, day, 7);
                PaintDayHeader(g, headerRect, dayDate, args);
            }

            for (int day = 0; day < 7; day++)
            {
                var dayDate = startOfWeek.AddDays(day).Date;
                var dayColumn = CalendarPainterHelpers.GetColumnRect(timedArea, day, 7);
                var allDayEvents = (args.EventService?.GetEventsForDate(dayDate) ?? new List<CalendarEvent>())
                    .Where(e => e.IsAllDay).ToList();
                for (int i = 0; i < allDayEvents.Count; i++)
                {
                    var evt = allDayEvents[i];
                    int colW = allDayStrip.Width / 7;
                    int x = allDayStrip.X + day * colW + 2;
                    int w = Math.Max(40, colW - 4);
                    var r = new Rectangle(x, allDayStrip.Y + 2, w, allDayStrip.Height - 4);
                    string cellKey = $"evt:{evt.Id}";
                    var eventCtx = new CalendarCellContext(CalendarCellKind.EventBlock, evt, dayDate, args.Surface.ViewMode, i, day);
                    if (CalendarPainterHelpers.TryDrawCellComponent(g, r, cellKey, eventCtx, args)) continue;

                    CalendarPainterHelpers.FillRoundedRect(g, r, 4, args.GetCategoryColor(evt.CategoryId));
                    CalendarPainterHelpers.DrawText(g, evt.Title, args.EventFont, args.ForegroundColor,
                        new Rectangle(r.X + 6, r.Y, r.Width - 8, r.Height),
                        StringAlignment.Near, StringAlignment.Center);
                }
                var timedEvents = (args.EventService?.GetEventsForDate(dayDate) ?? new List<CalendarEvent>())
                    .Where(e => !e.IsAllDay).ToList();
                for (int i = 0; i < timedEvents.Count; i++)
                {
                    var evt = timedEvents[i];
                    var eventRect = CalendarPainterHelpers.GetTimedEventRect(dayColumn, evt, dayDate, 4, 2, 18);
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
                    // W8 - delegate to developer's IBeepUIComponent TimeSlot factory
                    // for the (date, hour) cell when one is registered.
                    string slotKey = $"slot:{dayDate:yyyy-MM-dd}:{hour}";
                    var slotCtx = new CalendarCellContext(
                        CalendarCellKind.TimeSlot, null, dayDate,
                        args.Surface?.ViewMode ?? CalendarViewMode.Week2, hour, day);
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
            int timeColumnWidth = TimeColumnWidthPx;
            var timedArea = new Rectangle(grid.X + timeColumnWidth, grid.Y + dayHeaderHeight + AllDayStripHeightPx,
                Math.Max(0, grid.Width - detailW - timeColumnWidth),
                Math.Max(0, grid.Height - dayHeaderHeight - AllDayStripHeightPx));
            if (timedArea.Width <= 0 || timedArea.Height <= 0) return EmptyHit(args);
            int col = CalendarPainterHelpers.GetColumnIndex(timedArea, location.X, 7);
            if (col < 0 || col >= 7) return EmptyHit(args);
            DateTime dayDate = GetVisibleRangeStart(args.Surface.CurrentDate).AddDays(col).Date;
            var dayColumn = CalendarPainterHelpers.GetColumnRect(timedArea, col, 7);
            var dayEvents = args.EventService?.GetEventsForDate(dayDate) ?? new List<CalendarEvent>();
            for (int i = dayEvents.Count - 1; i >= 0; i--)
            {
                var evt = dayEvents[i];
                var eventRect = CalendarPainterHelpers.GetTimedEventRect(dayColumn, evt, dayDate, 4, 2, 18);
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
            int detailW = Math.Min(DetailPanelWidthPx, Math.Max(0, grid.Width / 4));
            if (location.X >= grid.Right - detailW) return null;
            int timeColumnWidth = TimeColumnWidthPx;
            int dayHeaderHeight = DayHeaderHeightPx;
            var timedArea = new Rectangle(grid.X + timeColumnWidth, grid.Y + dayHeaderHeight + AllDayStripHeightPx,
                Math.Max(0, grid.Width - detailW - timeColumnWidth),
                Math.Max(0, grid.Height - dayHeaderHeight - AllDayStripHeightPx));
            if (timedArea.Width <= 0 || timedArea.Height <= 0) return null;
            int minutesPerDay = 24 * 60;
            int minuteOffset = Math.Max(0, Math.Min(minutesPerDay - 1,
                (location.Y - timedArea.Top) * minutesPerDay / Math.Max(1, timedArea.Height)));
            int col = CalendarPainterHelpers.GetColumnIndex(timedArea, location.X, 7);
            if (col < 0 || col >= 7) col = 0;
            return GetVisibleRangeStart(args.Surface.CurrentDate).AddDays(col).Date.AddMinutes(minuteOffset);
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
            CalendarPainterHelpers.DrawText(g, evt.StartTime.ToString("dddd, MMMM d"), args.DayFont, args.ForegroundColor,
                new Rectangle(rect.X + pad, y, rect.Width - pad * 2, 20), StringAlignment.Near, StringAlignment.Near);
            y += 24;
            CalendarPainterHelpers.DrawText(g,
                $"{evt.StartTime:h:mm tt}  -  {evt.EndTime:h:mm tt}",
                args.DayFont, args.ForegroundColor,
                new Rectangle(rect.X + pad, y, rect.Width - pad * 2, 20), StringAlignment.Near, StringAlignment.Near);
        }

        private static void PaintDayHeader(Graphics g, Rectangle rect, DateTime dayDate, ViewPaintArgs args)
        {
            // W8 - delegate to developer's IBeepUIComponent DateCell factory
            // for the day-header cell when one is registered.
            string cellKey = $"date:{dayDate:yyyy-MM-dd}:header";
            var ctx = new CalendarCellContext(
                CalendarCellKind.DateCell, null, dayDate,
                args.Surface?.ViewMode ?? CalendarViewMode.Week2, 0, 0);
            if (CalendarPainterHelpers.TryDrawCellComponent(g, rect, cellKey, ctx, args)) return;

            CalendarPainterHelpers.FillRoundedRect(g, rect, args.Metrics.CornerRadius, args.BackgroundColor);
            CalendarPainterHelpers.DrawText(g, dayDate.ToString("ddd d"), args.DaysHeaderFont ?? args.HeaderFont,
                dayDate.Date == DateTime.Today ? args.TodayForeColor : args.ForegroundColor,
                rect, StringAlignment.Center, StringAlignment.Center);
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
            {
                CalendarPainterHelpers.StrokeRoundedRect(g, rect, args.Metrics.EventCornerRadius, args.PrimaryColor, 2f);
            }
            var title = (evt.StartTime.ToString("h:mm tt") + " " + evt.Title).Trim();
            var textRect = new Rectangle(rect.X + args.Metrics.EventAccentWidth + 4, rect.Y + 2,
                rect.Width - args.Metrics.EventAccentWidth - 6, Math.Max(0, rect.Height - 4));
            CalendarPainterHelpers.DrawText(g, title, args.EventFont ?? args.DayFont, args.ForegroundColor,
                textRect, StringAlignment.Near, StringAlignment.Near, centerVertically: false);
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
