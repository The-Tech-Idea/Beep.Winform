using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Calendar.Rendering;

namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    public partial class BeepCalendar
    {
        /// <summary>
        /// Single hit-test entry point. The per-view
        /// <see cref="ICalendarViewPainter"/> owns hit-testing for its
        /// own mode; this method just builds the
        /// <see cref="ViewPaintArgs"/> snapshot and dispatches.
        /// </summary>
        private CalendarInteractionHitTestResult ResolveInteractionTarget(Point location)
        {
            if (_viewPainter == null || _surfaceModel == null)
            {
                return new CalendarInteractionHitTestResult
                {
                    TargetKind = CalendarInteractionTargetKind.EmptySurface,
                    RequestedMode = CalendarInteractionMode.SelectDate,
                    Location = location,
                    Date = _state.CurrentDate.Date
                };
            }

            var args = new ViewPaintArgs
            {
                ControlStyle = _calendarStyle,
                Theme = _currentTheme,
                UseThemeColors = UseThemeColors,
                State = _state,
                Rects = _rects,
                Surface = _surfaceModel,
                EventService = _eventService,
                Events = _events,
                Categories = _categories,
                Resources = Resources,
                HeaderFont = HeaderFont,
                DayFont = DayFont,
                EventFont = EventFont,
                TimeFont = TimeFont,
                DaysHeaderFont = DaysHeaderFont,
                HoveredEventId = _hoveredEvent?.Id,
                HoveredDate = _hoveredDate,
                SelectedEvent = _state.SelectedEvent,
                Owner = this
            };
            args.ResolveThemeColors();

            return _viewPainter.HitTest(location, args);
        }
    }
}
