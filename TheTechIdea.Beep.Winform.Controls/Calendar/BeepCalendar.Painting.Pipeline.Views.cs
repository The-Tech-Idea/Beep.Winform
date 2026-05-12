using System.Drawing;

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
                case CalendarViewMode.List:
                    DrawListViewWithPainter(g, painterCtx);
                    break;
            }
        }

        private void DrawWorkWeekViewWithPainter(Graphics g, CalendarPainterContext painterCtx)
        {
            // WorkWeek currently reuses week painter semantics.
            DrawWeekViewWithPainter(g, painterCtx);
        }

        private void DrawAgendaViewWithPainter(Graphics g, CalendarPainterContext painterCtx)
        {
            // Agenda currently reuses list painter semantics.
            DrawListViewWithPainter(g, painterCtx);
        }
    }
}