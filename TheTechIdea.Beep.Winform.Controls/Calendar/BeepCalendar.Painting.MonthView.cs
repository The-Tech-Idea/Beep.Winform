using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Calendar.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    public partial class BeepCalendar
    {
        private void DrawMonthViewWithPainter(Graphics g, CalendarPainterContext ctx)
        {
            var grid = _rects.CalendarGridRect;
            var firstDayOfMonth = new DateTime(_state.CurrentDate.Year, _state.CurrentDate.Month, 1);
            var firstDayOfCalendar = firstDayOfMonth.AddDays(-(int)firstDayOfMonth.DayOfWeek);
            int dayHeaderHeight = ScaleMetric(CalendarLayoutMetrics.DayHeaderHeight);
            int eventHeight = ScaleMetric(16);
            int eventSpacing = ScaleMetric(2);
            int eventStartOffset = ScaleMetric(30);

            var headerBand = new Rectangle(grid.X, grid.Y, grid.Width, Math.Min(dayHeaderHeight, grid.Height));
            var monthBody = new Rectangle(grid.X, headerBand.Bottom, grid.Width, Math.Max(0, grid.Bottom - headerBand.Bottom));

            DrawMonthHeaders(g, headerBand, ctx);

            // Day cells
            var eventsByDate = new Dictionary<DateTime, List<CalendarEvent>>();
            for (int week = 0; week < 6; week++)
            {
                for (int day = 0; day < 7; day++)
                {
                    var cellDate = firstDayOfCalendar.AddDays(week * 7 + day);
                    var rowRect = CalendarLayoutGeometry.GetRowRect(monthBody, week, 6);
                    var cellRect = CalendarLayoutGeometry.GetColumnRect(rowRect, day, 7);

                    var dateKey = cellDate.Date;
                    if (!eventsByDate.TryGetValue(dateKey, out var dayEvents))
                    {
                        dayEvents = _eventService.GetEventsForDate(cellDate);
                        eventsByDate[dateKey] = dayEvents;
                    }

                    var state = CalendarViewStateHelper.BuildDayCellState(
                        cellDate,
                        _state.CurrentDate,
                        _state.SelectedDate,
                        _state.HoveredDate,
                        _state.FocusedDate,
                        _state.IsKeyboardFocusVisible,
                        dayEvents.Count,
                        CalendarLayoutMetrics.MaxEventsPerCell);

                    _stylePainter.PaintDayCell(g, cellRect, cellDate, state, ctx);

                    int eventY = DrawMonthCellEvents(g, cellRect, dayEvents, eventHeight, eventSpacing, eventStartOffset, ctx);

                    if (state.HasMoreEvents)
                    {
                        using (var brush = new SolidBrush(ctx.ForegroundColor))
                        {
                            g.DrawString($"+{dayEvents.Count - CalendarLayoutMetrics.MaxEventsPerCell} more", EventFont,
                                brush, new PointF(cellRect.X + 2, eventY));
                        }
                    }
                }
            }
        }
    }
}
