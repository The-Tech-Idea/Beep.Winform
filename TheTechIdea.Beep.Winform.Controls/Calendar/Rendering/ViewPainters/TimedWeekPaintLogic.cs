using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Calendar.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Calendar.Rendering.ViewPainters
{
    /// <summary>
    /// Shared paint + hit-test logic for the week-style timed views
    /// (<see cref="WeekViewPainter"/>, <see cref="WorkWeekViewPainter"/>,
    /// <see cref="DayViewPainter"/>). Day-of-week headers across the top,
    /// 24 hour rows, time labels down the left, and timed event blocks.
    /// </summary>
    internal static class TimedWeekPaintLogic
    {
        public static void Paint(Graphics g, ViewPaintArgs args, int dayCount)
        {
            if (args.Surface == null) return;
            var surface = args.Surface;
            var grid = args.Rects.CalendarGridRect;
            int timeColumnWidth = surface.TimeColumnWidth;
            int dayHeaderHeight = surface.DayHeaderHeight;
            int currentHour = DateTime.Now.Hour;
            DateTime startOfWeek = dayCount == 5 ? surface.StartOfWorkWeek : surface.StartOfWeek;

            CalendarPainterHelpers.FillRoundedRect(g,
                new Rectangle(grid.X, grid.Y, timeColumnWidth, dayHeaderHeight),
                args.Metrics.CornerRadius, args.BackgroundColor);

            for (int day = 0; day < dayCount; day++)
            {
                var dayDate = startOfWeek.AddDays(day);
                var headerRect = dayCount == 1
                    ? new Rectangle(grid.X + timeColumnWidth, grid.Y,
                        Math.Max(0, grid.Width - timeColumnWidth), dayHeaderHeight)
                    : surface.GetWeekDayHeaderRect(day);
                PaintDayHeader(g, headerRect, dayDate, args);
            }

            // Per-day events
            for (int day = 0; day < dayCount; day++)
            {
                var dayDate = startOfWeek.AddDays(day).Date;
                var dayColumn = dayCount == 1
                    ? surface.TimedArea
                    : surface.GetWeekDayColumnRect(day);
                var dayEvents = args.EventService?.GetEventsForDate(dayDate) ?? new List<CalendarEvent>();
                foreach (var evt in dayEvents.OrderBy(e => e.StartTime))
                {
                    var eventRect = CalendarPainterHelpers.GetTimedEventRect(
                        dayColumn, evt, dayDate,
                        surface.EventInsetX, surface.EventInsetY, surface.MinEventHitHeight);
                    PaintEventBlock(g, eventRect, evt, args);
                }
            }

            // Time rows + labels (drawn last so they sit on top of events)
            for (int hour = 0; hour < 24; hour++)
            {
                var rowRect = surface.GetTimeRowRect(hour);
                var timeLabelRect = new Rectangle(grid.X, rowRect.Y, timeColumnWidth, rowRect.Height);
                PaintTimeLabel(g, timeLabelRect, hour, args);
                for (int day = 0; day < dayCount; day++)
                {
                    var dayDate = startOfWeek.AddDays(day).Date;
                    var columnRect = dayCount == 1
                        ? new Rectangle(grid.X + timeColumnWidth, rowRect.Y,
                            Math.Max(0, grid.Width - timeColumnWidth), rowRect.Height)
                        : new Rectangle(surface.GetWeekDayColumnRect(day).X, rowRect.Y,
                            surface.GetWeekDayColumnRect(day).Width, rowRect.Height);
                    PaintTimeSlot(g, columnRect, hour,
                        hour == currentHour && dayDate == DateTime.Today, args);
                }
            }
        }

        public static CalendarInteractionHitTestResult HitTest(Point location, ViewPaintArgs args, int dayCount)
        {
            if (args.Surface == null)
                return EmptyHit(location, args);
            var surface = args.Surface;
            var grid = args.Rects.CalendarGridRect;
            if (location.X < grid.X + surface.TimeColumnWidth || location.Y < grid.Y + surface.DayHeaderHeight)
                return EmptyHit(location, args);

            DateTime startOfWeek = dayCount == 5 ? surface.StartOfWorkWeek : surface.StartOfWeek;
            DateTime dayDate;
            Rectangle dayColumn;
            if (dayCount == 1)
            {
                dayDate = surface.CurrentDate.Date;
                dayColumn = surface.TimedArea;
            }
            else
            {
                int col = CalendarPainterHelpers.GetColumnIndex(surface.TimedArea, location.X, dayCount);
                if (col < 0 || col >= dayCount)
                    return EmptyHit(location, args);
                dayDate = startOfWeek.AddDays(col).Date;
                dayColumn = surface.GetWeekDayColumnRect(col);
            }

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

            return new CalendarInteractionHitTestResult
            {
                TargetKind = CalendarInteractionTargetKind.DateCell,
                RequestedMode = CalendarInteractionMode.CreateEvent,
                Location = location,
                Date = dayDate
            };
        }

        // ── Drawing helpers ──────────────────────────────────────────────

        private static void PaintDayHeader(Graphics g, Rectangle rect, DateTime dayDate, ViewPaintArgs args)
        {
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

        private static void PaintTimeSlot(Graphics g, Rectangle rect, int hour, bool isCurrentHour, ViewPaintArgs args)
        {
            var back = args.BackgroundColor;
            if (isCurrentHour) back = Color.FromArgb(40, args.PrimaryColor.R, args.PrimaryColor.G, args.PrimaryColor.B);
            g.FillRectangle(new SolidBrush(back), rect);
            g.DrawLine(new Pen(args.BorderColor), rect.X, rect.Bottom, rect.Right, rect.Bottom);
        }

        private static void PaintEventBlock(Graphics g, Rectangle rect, CalendarEvent evt, ViewPaintArgs args)
        {
            if (rect.Width <= 0 || rect.Height <= 0) return;
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

        private static CalendarInteractionHitTestResult EmptyHit(Point location, ViewPaintArgs args) =>
            new CalendarInteractionHitTestResult
            {
                TargetKind = CalendarInteractionTargetKind.EmptySurface,
                RequestedMode = CalendarInteractionMode.CreateEvent,
                Location = location,
                Date = args.Surface?.CurrentDate.Date ?? DateTime.Today
            };
    }
}
