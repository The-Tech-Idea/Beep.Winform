using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Calendar.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Calendar.Rendering.ViewPainters
{
    /// <summary>
    /// Per-view painter for <see cref="CalendarViewMode.Month"/>. Renders a 6×7
    /// grid of day cells + the first <c>MaxEventsPerCell</c> event bars per
    /// cell. Reads <c>ViewPaintArgs.Metrics</c> for layout + visual constants
    /// and switches on <c>ViewPaintArgs.ControlStyle</c> for Material3 vs
    /// Minimal variants.
    /// </summary>
    public sealed class MonthViewPainter : ICalendarViewPainter
    {
        public CalendarViewMode ViewMode => CalendarViewMode.Month;

        private static readonly string[] DayNames = { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };

        public void Layout(ViewPaintArgs args)
        {
            // Layout is computed by BeepCalendar.UpdateLayout -> CalendarSurfaceModel.Build
            // and surfaced through args.Surface. Nothing to cache here.
        }

        public void Paint(Graphics g, ViewPaintArgs args)
        {
            if (args.Surface == null) return;
            var surface = args.Surface;
            var metrics = args.Metrics;

            // Day-of-week header band
            for (int i = 0; i < 7; i++)
            {
                var headerRect = surface.GetColumnRect(surface.MonthHeaderBand, i, 7);
                bool isToday = (int)DateTime.Today.DayOfWeek == i;
                PaintDayHeader(g, headerRect, DayNames[i], isToday, args);
            }

            // Day cells + event bars
            for (int week = 0; week < 6; week++)
            {
                for (int day = 0; day < 7; day++)
                {
                    var cellDate = surface.FirstDayOfCalendar.AddDays(week * 7 + day);
                    Rectangle cellRect = surface.GetMonthCellRect(cellDate);
                    if (cellRect.IsEmpty) continue;

                    var dayEvents = args.EventService?.GetEventsForDate(cellDate) ?? new List<CalendarEvent>();
                    var state = CalendarViewStateHelper.BuildDayCellState(
                        cellDate,
                        surface.CurrentDate,
                        surface.SelectedDate,
                        surface.HoveredDate,
                        surface.FocusedDate,
                        surface.State?.IsKeyboardFocusVisible ?? false,
                        dayEvents.Count,
                        metrics.MaxEventsPerCell);

                    PaintDayCell(g, cellRect, cellDate, state, args);

                    int eventY = PaintCellEvents(g, cellRect, dayEvents, args, state.HasMoreEvents);
                    if (state.HasMoreEvents)
                    {
                        CalendarPainterHelpers.DrawText(g,
                            $"+{dayEvents.Count - metrics.MaxEventsPerCell} more",
                            args.EventFont, args.ForegroundColor,
                            new RectangleF(cellRect.X + 2, eventY, cellRect.Width - 4, args.EventFont?.Height ?? 16),
                            StringAlignment.Near, StringAlignment.Near,
                            StringTrimming.EllipsisCharacter, centerVertically: false);
                    }
                }
            }
        }

        public CalendarInteractionHitTestResult HitTest(Point location, ViewPaintArgs args)
        {
            if (args.Surface == null)
                return EmptyHit(location, args);

            var surface = args.Surface;
            if (location.Y < surface.MonthHeaderBand.Bottom)
                return EmptyHit(location, args);

            int col = CalendarPainterHelpers.GetColumnIndex(surface.MonthBody, location.X, 7);
            int row = CalendarPainterHelpers.GetRowIndex(surface.MonthBody, location.Y, 6);
            if (col < 0 || col >= 7 || row < 0 || row >= 6)
                return EmptyHit(location, args);

            var date = surface.FirstDayOfCalendar.AddDays(row * 7 + col);
            var cellRect = surface.GetMonthCellRect(date);
            if (cellRect.IsEmpty)
                return EmptyHit(location, args);

            var dayEvents = args.EventService?.GetEventsForDate(date) ?? new List<CalendarEvent>();
            var eventHit = ResolveMonthEventHit(location, date, cellRect, dayEvents, args);
            if (eventHit.Event != null)
            {
                return new CalendarInteractionHitTestResult
                {
                    TargetKind = CalendarInteractionTargetKind.EventBlock,
                    RequestedMode = CalendarInteractionMode.SelectEvent,
                    Location = location,
                    Date = eventHit.Event.StartTime.Date,
                    Event = eventHit.Event,
                    Bounds = eventHit.Bounds
                };
            }

            return new CalendarInteractionHitTestResult
            {
                TargetKind = CalendarInteractionTargetKind.DateCell,
                RequestedMode = CalendarInteractionMode.CreateEvent,
                Location = location,
                Date = date
            };
        }

        // ── Drawing helpers ──────────────────────────────────────────────

        private static void PaintDayHeader(Graphics g, Rectangle rect, string dayName, bool isToday, ViewPaintArgs args)
        {
            CalendarPainterHelpers.FillRoundedRect(g, rect, args.Metrics.CornerRadius, args.BackgroundColor);
            var color = isToday ? args.TodayForeColor : args.ForegroundColor;
            CalendarPainterHelpers.DrawText(g, dayName, args.DaysHeaderFont ?? args.HeaderFont,
                color, rect, StringAlignment.Center, StringAlignment.Center);
        }

        private static void PaintDayCell(Graphics g, Rectangle rect, DateTime date, DayCellState state, ViewPaintArgs args)
        {
            Color back = args.BackgroundColor;
            if (!state.IsCurrentMonth) back = args.OutOfMonthBackColor;
            else if (state.IsWeekend) back = args.WeekendBackColor;
            if (state.IsSelected) back = args.SelectedBackColor;
            if (state.IsHovered) back = args.HoverBackColor;

            CalendarPainterHelpers.FillRoundedRect(g, rect, args.Metrics.CornerRadius, back);
            CalendarPainterHelpers.StrokeRoundedRect(g, rect, args.Metrics.CornerRadius, args.BorderColor);

            // Today highlight (filled circle behind day number)
            if (state.IsToday)
            {
                int d = Math.Max(16, Math.Min(rect.Width, rect.Height) / 6);
                var todayRect = new Rectangle(rect.X + 4, rect.Y + 4, d, d);
                CalendarPainterHelpers.FillRoundedRect(g, todayRect, d / 2, args.TodayBackColor);
            }

            // Day number
            Color numColor = state.IsToday
                ? args.TodayForeColor
                : (state.IsCurrentMonth ? args.ForegroundColor : args.OutOfMonthForeColor);
            int numX = rect.X + 4;
            int numY = rect.Y + 2;
            int numH = Math.Max(14, args.DayFont?.Height ?? 14);
            CalendarPainterHelpers.DrawText(g, date.Day.ToString(),
                args.DayFont ?? args.HeaderFont, numColor,
                new Rectangle(numX, numY, rect.Width - 8, numH),
                StringAlignment.Far, StringAlignment.Near, centerVertically: false);
        }

        private static int PaintCellEvents(Graphics g, Rectangle cellRect, List<CalendarEvent> dayEvents,
            ViewPaintArgs args, bool hasMore)
        {
            int eventHeight = args.Metrics.EventBarHeight;
            int eventSpacing = args.Metrics.EventSpacing;
            int eventStartOffset = Math.Max(20, (args.DayFont?.Height ?? 14) + 6);
            int eventY = cellRect.Y + eventStartOffset;
            int count = Math.Min(dayEvents.Count, args.Metrics.MaxEventsPerCell);
            for (int i = 0; i < count; i++)
            {
                var evt = dayEvents[i];
                var eventRect = new Rectangle(cellRect.X + 2, eventY,
                    Math.Max(1, cellRect.Width - 4), eventHeight);
                PaintEventBar(g, eventRect, evt, args);
                eventY += eventHeight + eventSpacing;
            }
            return eventY;
        }

        private static void PaintEventBar(Graphics g, Rectangle rect, CalendarEvent evt, ViewPaintArgs args)
        {
            Color fill = args.GetCategoryColor(evt.CategoryId);
            if (args.SelectedEvent?.Id == evt.Id) fill = args.SelectedBackColor;
            if (args.HoveredEventId == evt.Id) fill = args.HoverBackColor;

            CalendarPainterHelpers.FillRoundedRect(g, rect, args.Metrics.EventCornerRadius, fill);
            if (args.Metrics.ShowEventAccentStripe)
            {
                var accent = new Rectangle(rect.X, rect.Y, args.Metrics.EventAccentWidth, rect.Height);
                CalendarPainterHelpers.FillRoundedRect(g, accent, 0, fill);
            }
            var textRect = new Rectangle(rect.X + args.Metrics.EventAccentWidth + 4, rect.Y,
                rect.Width - args.Metrics.EventAccentWidth - 6, rect.Height);
            CalendarPainterHelpers.DrawText(g, evt.Title,
                args.EventFont ?? args.DayFont, args.ForegroundColor, textRect,
                StringAlignment.Near, StringAlignment.Center);
        }

        private static (CalendarEvent Event, Rectangle Bounds) ResolveMonthEventHit(
            Point location, DateTime date, Rectangle cellRect, List<CalendarEvent> dayEvents, ViewPaintArgs args)
        {
            if (dayEvents.Count == 0) return (null, Rectangle.Empty);
            int eventHeight = args.Metrics.EventBarHeight;
            int eventSpacing = args.Metrics.EventSpacing;
            int eventStartOffset = Math.Max(20, (args.DayFont?.Height ?? 14) + 6);
            int eventY = cellRect.Y + eventStartOffset;
            int count = Math.Min(dayEvents.Count, args.Metrics.MaxEventsPerCell);
            for (int i = 0; i < count; i++)
            {
                var eventRect = new Rectangle(cellRect.X + 2, eventY,
                    Math.Max(1, cellRect.Width - 4), eventHeight);
                if (eventRect.Contains(location))
                    return (dayEvents[i], eventRect);
                eventY += eventHeight + eventSpacing;
            }
            return (null, Rectangle.Empty);
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
