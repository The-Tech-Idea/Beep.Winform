using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Calendar.CellRender;
using TheTechIdea.Beep.Winform.Controls.Calendar.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Calendar.Rendering.ViewPainters
{
    /// <summary>
    /// Per-view painter for <see cref="CalendarViewMode.Week1"/>. Matches
    /// the c1 reference image: 7-day timed grid (Sun-Sat) with a left
    /// sidebar (mini month, upcoming event card, time breakdown) and a
    /// header showing the current period. Day-of-week headers across the
    /// top, 24 hour rows, time labels down the left, and timed event
     /// blocks. Developer-supplied IBeepUIComponent factory
     /// (<c>CellComponentFactory</c>) overrides the default rendering
    /// for the corresponding cell kind.
    /// </summary>
    public sealed class Week1ViewPainter : ICalendarViewPainter
    {
        public CalendarViewMode ViewMode => CalendarViewMode.Week1;
        public string Key => "week1";
        public string DisplayLabel => "Week 1";
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

        private const int SidebarWidthPx = 260;
        private const int DayHeaderHeightPx = 36;
        private const int TimeColumnWidthPx = 60;
        private static readonly string[] DayNames = { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };

        public void Layout(ViewPaintArgs args) { }

        public void Paint(Graphics g, ViewPaintArgs args)
        {
            if (args.Surface == null) return;
            var grid = args.Rects.CalendarGridRect;
            int dayHeaderHeight = DayHeaderHeightPx;
            int timeColumnWidth = TimeColumnWidthPx;
            int sidebarW = Math.Min(SidebarWidthPx, Math.Max(0, grid.Width / 4));
            var contentRect = new Rectangle(grid.X + sidebarW, grid.Y,
                Math.Max(0, grid.Width - sidebarW), grid.Height);
            if (contentRect.Width <= 0 || contentRect.Height <= 0) return;
            var dayHeaderBand = new Rectangle(contentRect.X + timeColumnWidth, contentRect.Y,
                Math.Max(0, contentRect.Width - timeColumnWidth), dayHeaderHeight);
            var timedArea = new Rectangle(contentRect.X + timeColumnWidth, contentRect.Y + dayHeaderHeight,
                Math.Max(0, contentRect.Width - timeColumnWidth),
                Math.Max(0, contentRect.Height - dayHeaderHeight));
            int currentHour = DateTime.Now.Hour;
            DateTime startOfWeek = GetVisibleRangeStart(args.Surface.CurrentDate);

            PaintSidebar(g, new Rectangle(grid.X, grid.Y, sidebarW, grid.Height), args);

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
                var dayEvents = args.EventService?.GetEventsForDate(dayDate) ?? new List<CalendarEvent>();
                for (int i = 0; i < dayEvents.Count; i++)
                {
                    var evt = dayEvents[i];
                    var eventRect = CalendarPainterHelpers.GetTimedEventRect(
                        dayColumn, evt, dayDate, 4, 2, 18);
                    PaintEventBlock(g, eventRect, evt, dayDate, day, args);
                }
            }

            for (int hour = 0; hour < 24; hour++)
            {
                var rowRect = CalendarPainterHelpers.GetRowRect(timedArea, hour, 24);
                var timeLabelRect = new Rectangle(contentRect.X, rowRect.Y, timeColumnWidth, rowRect.Height);
                PaintTimeLabel(g, timeLabelRect, hour, args);
                for (int day = 0; day < 7; day++)
                {
                    var dayDate = startOfWeek.AddDays(day).Date;
                    var columnRect = CalendarPainterHelpers.GetColumnRect(
                        new Rectangle(timedArea.X, rowRect.Y, timedArea.Width, rowRect.Height),
                        day, 7);
                    PaintTimeSlot(g, columnRect, hour, hour == currentHour && dayDate == DateTime.Today,
                        dayDate, day, args);
                }
            }
        }

        public CalendarInteractionHitTestResult HitTest(Point location, ViewPaintArgs args)
        {
            if (args.Surface == null) return EmptyHit(args);
            var grid = args.Rects.CalendarGridRect;
            int dayHeaderHeight = DayHeaderHeightPx;
            int timeColumnWidth = TimeColumnWidthPx;
            int sidebarW = Math.Min(SidebarWidthPx, Math.Max(0, grid.Width / 4));
            var timedArea = new Rectangle(grid.X + sidebarW + timeColumnWidth, grid.Y + dayHeaderHeight,
                Math.Max(0, grid.Width - sidebarW - timeColumnWidth),
                Math.Max(0, grid.Height - dayHeaderHeight));
            if (timedArea.Width <= 0 || timedArea.Height <= 0) return EmptyHit(args);
            if (location.Y < grid.Y + dayHeaderHeight)
            {
                int col = CalendarPainterHelpers.GetColumnIndex(timedArea, location.X, 7);
                if (col < 0 || col >= 7) return EmptyHit(args);
                return new CalendarInteractionHitTestResult
                {
                    TargetKind = CalendarInteractionTargetKind.DateCell,
                    RequestedMode = CalendarInteractionMode.SelectDate,
                    Location = location,
                    Date = GetVisibleRangeStart(args.Surface.CurrentDate).AddDays(col)
                };
            }
            int dayCol = CalendarPainterHelpers.GetColumnIndex(timedArea, location.X, 7);
            if (dayCol < 0 || dayCol >= 7) return EmptyHit(args);
            DateTime dayDate = GetVisibleRangeStart(args.Surface.CurrentDate).AddDays(dayCol).Date;
            var dayColumn = CalendarPainterHelpers.GetColumnRect(timedArea, dayCol, 7);
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
            int sidebarW = Math.Min(SidebarWidthPx, Math.Max(0, grid.Width / 4));
            if (location.X < grid.X + sidebarW) return null;
            var timedArea = new Rectangle(grid.X + sidebarW + TimeColumnWidthPx, grid.Y + DayHeaderHeightPx,
                Math.Max(0, grid.Width - sidebarW - TimeColumnWidthPx),
                Math.Max(0, grid.Height - DayHeaderHeightPx));
            if (timedArea.Width <= 0 || timedArea.Height <= 0) return null;
            int minutesPerDay = 24 * 60;
            int minuteOffset = Math.Max(0, Math.Min(minutesPerDay - 1,
                (location.Y - timedArea.Top) * minutesPerDay / Math.Max(1, timedArea.Height)));
            int col = CalendarPainterHelpers.GetColumnIndex(timedArea, location.X, 7);
            if (col < 0 || col >= 7) col = 0;
            return GetVisibleRangeStart(args.Surface.CurrentDate).AddDays(col).Date.AddMinutes(minuteOffset);
        }

        // ── Drawing helpers (private to this view) ───────────────────────

        private static void PaintSidebar(Graphics g, Rectangle rect, ViewPaintArgs args)
        {
            CalendarPainterHelpers.FillRoundedRect(g, rect, args.Metrics.CornerRadius, args.BackgroundColor);
            CalendarPainterHelpers.StrokeRoundedRect(g, rect, args.Metrics.CornerRadius, args.BorderColor);
            int padding = 12;
            int cardH = 80;
            int y = rect.Y + padding;
            var card1 = new Rectangle(rect.X + padding, y, rect.Width - padding * 2, cardH);
            CalendarPainterHelpers.FillRoundedRect(g, card1, 6, args.SelectedBackColor);
            CalendarPainterHelpers.DrawText(g, "Upcoming event", args.DayFont, args.ForegroundColor,
                card1, StringAlignment.Near, StringAlignment.Near);
            y += cardH + padding;
            var card2 = new Rectangle(rect.X + padding, y, rect.Width - padding * 2, cardH);
            CalendarPainterHelpers.FillRoundedRect(g, card2, 6, args.SelectedBackColor);
            CalendarPainterHelpers.DrawText(g, "Time breakdown", args.DayFont, args.ForegroundColor,
                card2, StringAlignment.Near, StringAlignment.Near);
        }

        private static void PaintDayHeader(Graphics g, Rectangle rect, DateTime dayDate, ViewPaintArgs args)
        {
            // W8 - delegate to developer's IBeepUIComponent DateCell factory
            // for the day-header cell when one is registered.
            string cellKey = $"date:{dayDate:yyyy-MM-dd}:header";
            var ctx = new CalendarCellContext(
                CalendarCellKind.DateCell, null, dayDate,
                args.Surface?.ViewMode ?? CalendarViewMode.Week1, 0, 0);
            if (CalendarPainterHelpers.TryDrawCellComponent(g, rect, cellKey, ctx, args)) return;

            CalendarPainterHelpers.FillRoundedRect(g, rect, args.Metrics.CornerRadius, args.BackgroundColor);
            string text = dayDate.ToString("ddd d");
            CalendarPainterHelpers.DrawText(g, text, args.DaysHeaderFont ?? args.HeaderFont,
                dayDate.Date == DateTime.Today ? args.TodayForeColor : args.ForegroundColor,
                rect, StringAlignment.Center, StringAlignment.Center);
        }

        private static void PaintTimeLabel(Graphics g, Rectangle rect, int hour, ViewPaintArgs args)
        {
            string label = hour == 0 ? "12a" : hour < 12 ? $"{hour}a" : hour == 12 ? "12p" : $"{hour - 12}p";
            CalendarPainterHelpers.DrawText(g, label, args.TimeFont ?? args.DayFont, args.ForegroundColor, rect,
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
                args.Surface?.ViewMode ?? CalendarViewMode.Week1, hour, dayIndex);
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
            var ctx = new CalendarCellContext(CalendarCellKind.EventBlock, evt, dayDate, args.Surface.ViewMode, 0, dayIndex);
            if (CalendarPainterHelpers.TryDrawCellComponent(g, rect, cellKey, ctx, args)) return;

            CalendarPainterHelpers.FillRoundedRect(g, rect, args.Metrics.EventCornerRadius,
                args.GetCategoryColor(evt.CategoryId));
            if (args.SelectedEvent?.Id == evt.Id)
            {
                CalendarPainterHelpers.StrokeRoundedRect(g, rect, args.Metrics.EventCornerRadius, args.PrimaryColor, 2f);
            }
            if (args.Metrics.ShowEventAccentStripe)
            {
                var accent = new Rectangle(rect.X, rect.Y, args.Metrics.EventAccentWidth, rect.Height);
                CalendarPainterHelpers.FillRoundedRect(g, accent, 0, Color.FromArgb(80, 0, 0, 0));
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
