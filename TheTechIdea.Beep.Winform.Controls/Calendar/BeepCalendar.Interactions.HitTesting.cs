using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    public partial class BeepCalendar
    {
        private CalendarInteractionHitTestResult ResolveInteractionTarget(Point location)
        {
            switch (_state.ViewMode)
            {
                case CalendarViewMode.Month:
                    return ResolveMonthInteraction(location);
                case CalendarViewMode.Week:
                case CalendarViewMode.WorkWeek:
                    return ResolveWeekInteraction(location);
                case CalendarViewMode.Day:
                    return ResolveDayInteraction(location);
                case CalendarViewMode.List:
                case CalendarViewMode.Agenda:
                    return ResolveListInteraction(location);
                default:
                    return new CalendarInteractionHitTestResult
                    {
                        TargetKind = CalendarInteractionTargetKind.EmptySurface,
                        RequestedMode = CalendarInteractionMode.SelectDate,
                        Location = location,
                        Date = _state.CurrentDate.Date
                    };
            }
        }

    }
}