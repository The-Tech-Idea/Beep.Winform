using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Calendar.Rendering;

namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    public partial class BeepCalendar
    {
        private void DrawCalendarViewWithPainter(Graphics g, CalendarPainterContext painterCtx)
        {
            switch (_state.ViewMode)
            {
                case CalendarViewMode.Month:
                    DrawMonthViewWithPainter(g, painterCtx);
                    break;
                case CalendarViewMode.Week:
                    DrawWeekViewWithPainter(g, painterCtx);
                    break;
                case CalendarViewMode.WorkWeek:
                    DrawWorkWeekViewWithPainter(g, painterCtx);
                    break;
                case CalendarViewMode.Day:
                    DrawDayViewWithPainter(g, painterCtx);
                    break;
                case CalendarViewMode.Agenda:
                    DrawAgendaViewWithPainter(g, painterCtx);
                    break;
                case CalendarViewMode.Timeline:
                    DrawTimelineViewWithPainter(g);
                    break;
                case CalendarViewMode.List:
                    DrawListViewWithPainter(g, painterCtx);
                    break;
            }
        }

        private void DrawWorkWeekViewWithPainter(Graphics g, CalendarPainterContext painterCtx)
        {
            int dayOfWeek = (int)_state.CurrentDate.DayOfWeek;
            int mondayOffset = dayOfWeek == 0 ? 6 : dayOfWeek - 1;
            var startOfWorkWeek = _state.CurrentDate.Date.AddDays(-mondayOffset);
            DrawTimedWeekViewWithPainter(g, painterCtx, startOfWorkWeek, 5);
        }

        private void DrawAgendaViewWithPainter(Graphics g, CalendarPainterContext painterCtx)
        {
            // Agenda currently reuses list painter semantics.
            DrawListViewWithPainter(g, painterCtx);
        }

        private void DrawTimelineViewWithPainter(Graphics g)
        {
            var headerTextBounds = GetHeaderTextBounds();
            int headerLeft = System.Math.Max(0, headerTextBounds.Left - _rects.HeaderRect.X);
            int headerRight = System.Math.Max(0, _rects.HeaderRect.Right - headerTextBounds.Right);
            var renderCtx = new CalendarRenderContext(this, _currentTheme,
                HeaderFont, DayFont, EventFont, TimeFont, DaysHeaderFont,
                _state, _rects, _eventService, _categories, Resources,
                headerLeft, headerRight, GetDensityScale());

            new TimelineViewRenderer().Draw(g, renderCtx);
        }
    }
}
